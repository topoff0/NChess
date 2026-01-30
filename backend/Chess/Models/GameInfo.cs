namespace Chess.Models
{
    public class GameInfo
    {
        public int Id { get; set; }
        public required List<string> Fens { get; set; }
        public required List<string> Moves { get; set; }
        public bool IsActiveGame { get; set; } = true;

        public int FirstPlayerId { get; set; }
        public int SecondPlayerId { get; set; }
    }
}