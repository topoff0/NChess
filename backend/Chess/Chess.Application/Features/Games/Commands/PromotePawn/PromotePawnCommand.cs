using Chess.Application.Contracts.Requests;
using Chess.Application.Contracts.Responses.GameProcess;
using Chess.Application.Features.Games.Common;
using Chess.Application.Interfaces;
using Chess.Core.Entities;
using Chess.Core.FEN;
using Chess.Core.Models;
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

            endedGame.IsActiveGame = false;
            await _unitOfWork.SaveChangesAsync(token);

            GameResponse endGameResponse = new(
                isSuccess: true,
                message: "Game ended",
                fen: promoteResponse.Fen,
                legalMoves: null,
                moveNotations: promoteResponse.MoveNotations,
                isGameEnded: true,
                winner: gameCondition.Value == GameCondition.Lose ? request.PlayerName : "DRAW");

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
        gameCondition = _movementService.GetGameCondition(board, legalMoves);

        if (gameCondition.HasValue)
        {
            GameInfo? endedGame = await _gameRepository.GetActiveByFirstPlayerIdAsync(request.PlayerId, token);
            if (endedGame is null)
            {
                return GameCommandResult.GameNotFound();
            }

            endedGame.IsActiveGame = false;
            await _unitOfWork.SaveChangesAsync(token);

            GameResponse endGameResponse = new(
                isSuccess: true,
                message: "Successful move",
                fen: promoteResponse.Fen,
                legalMoves: legalMoves,
                moveNotations: promoteResponse.MoveNotations,
                isGameEnded: true,
                winner: gameCondition == GameCondition.Draw ? "DRAW" : "Computer");

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
}
