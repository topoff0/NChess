namespace Chess.Application.Contracts.Responses.GameProcess
{
    public class OnMoveResponse(string fen, List<string> moveNotations)
    {
        public string Fen { get; set; } = fen;
        public List<string> MoveNotations { get; set; } = moveNotations;
    }
}
