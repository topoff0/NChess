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

namespace Chess.Application.Features.Games.Commands.MakeMove;

public record MakeMoveCommand(MoveRequest Move, int PlayerId, string? PlayerName)
    : IRequest<GameCommandResult>;

public sealed class MakeMoveCommandHandler(IGameRepository gameRepository,
                                          IUnitOfWork unitOfWork,
                                          IChessMovementService movementService)
    : IRequestHandler<MakeMoveCommand, GameCommandResult>
{
    private readonly IGameRepository _gameRepository = gameRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IChessMovementService _movementService = movementService;

    public async Task<GameCommandResult> Handle(MakeMoveCommand request, CancellationToken token)
    {
        OnMoveResponse moveResponse = await _movementService.HandleMove(request.Move, request.PlayerId, token);

        var legalComputerMoves = _movementService.GetLegalMoves(moveResponse.Fen);
        Board board = FenUtility.LoadBoardFromFen(moveResponse.Fen);
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
                fen: moveResponse.Fen,
                legalMoves: null,
                moveNotations: moveResponse.MoveNotations,
                isGameEnded: true,
                winner: gameCondition.Value == GameCondition.Lose ? request.PlayerName : "DRAW");

            return GameCommandResult.Success(endGameResponse);
        }

        var moveValues = SearchAlgorithm.Search(legalComputerMoves, board);

        if (moveValues.IsItPromotionPawnMove && moveValues.PromotionPiece.HasValue)
        {
            PawnPromotionRequest promotionRequest = new()
            {
                StartSquare = moveValues.StartSquare,
                TargetSquare = moveValues.TargetSquare,
                FenBeforeMove = moveResponse.Fen,
                ChosenPiece = moveValues.PromotionPiece.Value
            };

            Console.WriteLine(moveValues.PromotionPiece);
            moveResponse = await _movementService.HandlePawnPromotion(promotionRequest, request.PlayerId, token);
        }
        else
        {
            MoveRequest computerMoveRequest = new()
            {
                StartSquare = moveValues.StartSquare,
                TargetSquare = moveValues.TargetSquare,
                FenBeforeMove = moveResponse.Fen
            };

            moveResponse = await _movementService.HandleMove(computerMoveRequest, 0, token);
        }

        var legalMoves = _movementService.GetLegalMoves(moveResponse.Fen);
        board = FenUtility.LoadBoardFromFen(moveResponse.Fen);
        gameCondition = _movementService.GetGameCondition(board, legalMoves);

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
                fen: moveResponse.Fen,
                legalMoves: legalMoves,
                moveNotations: moveResponse.MoveNotations,
                isGameEnded: true,
                winner: gameCondition.Value == GameCondition.Draw ? "DRAW" : "Computer");

            return GameCommandResult.Success(endGameResponse);
        }

        GameResponse response = new(
            isSuccess: true,
            message: "Successful move",
            fen: moveResponse.Fen,
            legalMoves: legalMoves,
            moveNotations: moveResponse.MoveNotations,
            isGameEnded: false,
            winner: null);

        return GameCommandResult.Success(response);
    }
}
