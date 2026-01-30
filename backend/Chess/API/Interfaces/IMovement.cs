using Chess.DTO.Requests;
using Chess.DTO.Responses.GameProcess;
using Chess.Main.Models;

namespace Chess.API.Interfaces
{
    public interface IMovement
    {
        enum GameCondition
        {
            WIN = 1,
            LOSE = -1,
            DRAW = 0,
        }

        public Task<OnMoveResponse> HandleMove(MoveRequest request, int playerId);
        public Dictionary<int, List<int>> GetLegalMoves(string fen);
        public GameCondition? GetGameCondition(Board board, Dictionary<int, List<int>> legalMoves);

        public Task<OnMoveResponse> HandlePawnPromotion(PawnPromotionRequest request, int playerId);
    }
}