namespace Chess.Core.Entities;

public class GameInfo
{
    public int Id { get; set; }
    public required List<string> Fens { get; set; }
    public required List<string> Moves { get; set; }
    public bool IsActiveGame { get; set; } = true;

    public Guid FirstPlayerId { get; set; }
    public Guid? SecondPlayerId { get; set; }

    public string? Result { get; set; }
    public DateTime? FinishedAt { get; set; }
}
