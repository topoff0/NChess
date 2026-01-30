namespace Chess.DTO.Requests
{
    public class GameStartRequest(bool isPlayerPlayWhite)
    {
        public bool IsPlayerPlayWhite { get; set; } = isPlayerPlayWhite;
    }
}