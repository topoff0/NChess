using Chess.Application.Contracts.Requests;
using Chess.Application.Contracts.Responses.GameProcess;
using Chess.Application.Features.Games.Common;
using Chess.Core.Models;

namespace Chess.API.Interfaces
{
    public interface IMovement
    {
        public Task<OnMoveResponse> HandleMove(MoveRequest request, int playerId, CancellationToken token);
        public Dictionary<int, List<int>> GetLegalMoves(string fen);
        public GameCondition? GetGameCondition(Board board, Dictionary<int, List<int>> legalMoves);

        public Task<OnMoveResponse> HandlePawnPromotion(PawnPromotionRequest request, int playerId, CancellationToken token);
    }
}
