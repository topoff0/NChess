namespace Chess.DTO.Requests
{
    public class OnlineGameStartRequest(bool isPlayerPlayWhite, int firstPlayerId, int secondPlayerId = 0)
    : GameStartRequest(isPlayerPlayWhite)
    {
        public required int FirstPlayerId { get; init; } = firstPlayerId;
        public int SecondePlayerId { get; init; } = secondPlayerId;
    }
}