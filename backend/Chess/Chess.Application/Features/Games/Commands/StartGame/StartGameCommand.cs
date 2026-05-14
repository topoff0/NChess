using Chess.Application.Contracts.Requests;
using Chess.Application.Contracts.Responses.GameProcess;
using Chess.Application.Interfaces;
using Chess.Core.Entities;
using Chess.Core.FEN;
using Chess.Core.Models;
using Chess.Core.Repositories;
using Chess.Core.Repositories.Common;
using Chess.Core.Search;
using MediatR;

namespace Chess.Application.Features.Games.Commands.StartGame;

public record StartGameCommand(int PlayerId, bool IsPlayerPlayWhite)
    : IRequest<GameResponse>;

public sealed class StartGameCommandHandler(IGameRepository gameRepository,
                                           IUnitOfWork unitOfWork,
                                           IChessMovementService movementService)
    : IRequestHandler<StartGameCommand, GameResponse>
{
    private const string InitialFen =
        "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

    private readonly IGameRepository _gameRepository = gameRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IChessMovementService _movementService = movementService;

    public async Task<GameResponse> Handle(StartGameCommand request, CancellationToken token)
    {
        GameInfo? activeGame = await _gameRepository.GetActiveByFirstPlayerIdAsync(request.PlayerId, token);

        if (activeGame is not null)
        {
            string lastFenOfActiveGame = activeGame.Fens[^1];
            var legalMoves = _movementService.GetLegalMoves(lastFenOfActiveGame);

            return new GameResponse(
                isSuccess: true,
                message: "You are already playing",
                fen: lastFenOfActiveGame,
                legalMoves: legalMoves,
                moveNotations: activeGame.Moves,
                isGameEnded: false,
                winner: null);
        }

        string fen = InitialFen;

        GameInfo newGame = new()
        {
            Fens = [fen],
            Moves = [],
            IsActiveGame = true,
            FirstPlayerId = request.PlayerId,
            SecondPlayerId = 0
        };

        await _gameRepository.AddAsync(newGame, token);
        await _unitOfWork.SaveChangesAsync(token);

        List<string> moveNotations = [];

        if (!request.IsPlayerPlayWhite)
        {
            var legalComputerMoves = _movementService.GetLegalMoves(fen);
            Board board = FenUtility.LoadBoardFromFen(fen);
            var moveValues = SearchAlgorithm.Search(legalComputerMoves, board);

            MoveRequest computerMoveRequest = new()
            {
                StartSquare = moveValues.StartSquare,
                TargetSquare = moveValues.TargetSquare,
                FenBeforeMove = fen
            };

            OnMoveResponse moveResponse = await _movementService.HandleMove(computerMoveRequest, 0, token);
            fen = moveResponse.Fen;
        }

        var legalMovesAfterStart = _movementService.GetLegalMoves(fen);

        return new GameResponse(
            isSuccess: true,
            message: "Game successfully started",
            fen: fen,
            legalMoves: legalMovesAfterStart,
            moveNotations: moveNotations,
            isGameEnded: false,
            winner: null);
    }
}
