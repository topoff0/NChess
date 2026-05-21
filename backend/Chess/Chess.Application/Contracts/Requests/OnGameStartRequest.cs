namespace Chess.Application.Contracts.Requests
{
    public class OnlineGameStartRequest(bool isPlayerPlayWhite, Guid firstPlayerId, Guid? secondPlayerId = null)
    : GameStartRequest(isPlayerPlayWhite)
    {
        public required Guid FirstPlayerId { get; init; } = firstPlayerId;
        public Guid? SecondPlayerId { get; init; } = secondPlayerId;
    }
}
