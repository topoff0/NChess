using Chess.Application.Contracts.Requests;
using Chess.Application.Contracts.Responses.GameProcess;
using Chess.Application.Features.Games.Common;
using Chess.Core.Models;

namespace Chess.Application.Interfaces;

public interface IChessMovementService
{
    Dictionary<int, List<int>> GetLegalMoves(string fen);

    Task<OnMoveResponse> HandleMove(MoveRequest request, int playerId, CancellationToken token);

    GameCondition? GetGameCondition(Board board, Dictionary<int, List<int>> legalMoves);

    Task<OnMoveResponse> HandlePawnPromotion(PawnPromotionRequest request, int playerId, CancellationToken token);
}
