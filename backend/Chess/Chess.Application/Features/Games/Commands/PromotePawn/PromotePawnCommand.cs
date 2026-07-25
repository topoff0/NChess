using Chess.Application.Contracts.Requests;
using Chess.Application.Contracts.Responses.GameProcess;
using Chess.Application.Features.Games.Common;
using Chess.Application.Interfaces;
using Chess.Core.Common;
using Chess.Core.Entities;
using Chess.Core.FEN;
using Chess.Core.Models;
using Chess.Core.MoveNotation;
using Chess.Core.Repositories;
using Chess.Core.Repositories.Common;
using Chess.Core.Search;
using MediatR;

namespace Chess.Application.Features.Games.Commands.PromotePawn;

public record PromotePawnCommand(PawnPromotionRequest Promotion, Guid PlayerId, string? PlayerName)
    : IRequest<GameCommandResult>;

public sealed class PromotePawnCommandHandler(IGameRepository gameRepository,
                                             IUnitOfWork unitOfWork,
                                             IChessMovementService movementService)
    : IRequestHandler<PromotePawnCommand, GameCommandResult>
{
    private readonly IGameRepository _gameRepository = gameRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IChessMovementService _movementService = movementService;

    public async Task<GameCommandResult> Handle(PromotePawnCommand request, CancellationToken token)
    {
        OnMoveResponse promoteResponse = await _movementService.HandlePawnPromotion(
            request.Promotion,
            request.PlayerId,
            token);

        var legalComputerMoves = _movementService.GetLegalMoves(promoteResponse.Fen);
        Board board = FenUtility.LoadBoardFromFen(promoteResponse.Fen);
        GameCondition? gameCondition = _movementService.GetGameCondition(board, legalComputerMoves);

        if (gameCondition.HasValue)
        {
            GameInfo? endedGame = await _gameRepository.GetByFirstPlayerIdAsync(request.PlayerId, token);
            if (endedGame is null)
            {
                return GameCommandResult.GameNotFound();
            }

            string? winner = gameCondition.Value == GameCondition.Lose ? request.PlayerName : "DRAW";

            endedGame.IsActiveGame = false;
            endedGame.Result = winner;
            endedGame.FinishedAt = DateTime.UtcNow;
            ApplyEndGameNotation(endedGame, promoteResponse, gameCondition.Value);
            await _unitOfWork.SaveChangesAsync(token);

            GameResponse endGameResponse = new(
                isSuccess: true,
                message: "Game ended",
                fen: promoteResponse.Fen,
                legalMoves: null,
                moveNotations: promoteResponse.MoveNotations,
                isGameEnded: true,
                winner: winner);

            return GameCommandResult.Success(endGameResponse);
        }

        var moveValues = SearchAlgorithm.Search(legalComputerMoves, board);

        MoveRequest computerMoveRequest = new()
        {
            StartSquare = moveValues.StartSquare,
            TargetSquare = moveValues.TargetSquare,
            FenBeforeMove = promoteResponse.Fen
        };

        promoteResponse = await _movementService.HandleMove(computerMoveRequest, request.PlayerId, token);

        var legalMoves = _movementService.GetLegalMoves(promoteResponse.Fen);
        board = FenUtility.LoadBoardFromFen(promoteResponse.Fen);
        gameCondition = _movementService.GetGameCondition(board, legalMoves);

        if (gameCondition.HasValue)
        {
            GameInfo? endedGame = await _gameRepository.GetActiveByFirstPlayerIdAsync(request.PlayerId, token);
            if (endedGame is null)
            {
                return GameCommandResult.GameNotFound();
            }

            string winner = gameCondition.Value == GameCondition.Draw ? "DRAW" : "Computer";

            endedGame.IsActiveGame = false;
            endedGame.Result = winner;
            endedGame.FinishedAt = DateTime.UtcNow;
            ApplyEndGameNotation(endedGame, promoteResponse, gameCondition.Value);
            await _unitOfWork.SaveChangesAsync(token);

            GameResponse endGameResponse = new(
                isSuccess: true,
                message: "Successful move",
                fen: promoteResponse.Fen,
                legalMoves: legalMoves,
                moveNotations: promoteResponse.MoveNotations,
                isGameEnded: true,
                winner: winner);

            return GameCommandResult.Success(endGameResponse);
        }

        GameResponse response = new(
            isSuccess: true,
            message: "Successful move",
            fen: promoteResponse.Fen,
            legalMoves: legalMoves,
            moveNotations: promoteResponse.MoveNotations,
            isGameEnded: false,
            winner: null);

        return GameCommandResult.Success(response);
    }

    private static void ApplyEndGameNotation(GameInfo game, OnMoveResponse moveResponse, GameCondition gameCondition)
    {
        if (game.Moves.Count == 0)
        {
            return;
        }

        int lastMoveIndex = game.Moves.Count - 1;
        string updatedMoveNotation = MoveNotation.ApplyEndGameNotation(game.Moves[lastMoveIndex], gameCondition);

        game.Moves[lastMoveIndex] = updatedMoveNotation;

        if (moveResponse.MoveNotations.Count > lastMoveIndex)
        {
            moveResponse.MoveNotations[lastMoveIndex] = updatedMoveNotation;
        }
    }
}
