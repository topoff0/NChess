using Chess.Application.Contracts.Requests;
using Chess.Application.Contracts.Responses.GameProcess;

namespace Chess.Application.Interfaces;

public interface IChessMovementService
{
    Dictionary<int, List<int>> GetLegalMoves(string fen);

    Task<OnMoveResponse> HandleMove(MoveRequest request, int playerId, CancellationToken token);
}
