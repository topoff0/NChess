namespace Chess.DTO.Requests
{
    public record MoveRequest
    {
        public required int StartSquare { get; init; }
        public required int TargetSquare { get; init; }
        public required string FenBeforeMove { get; init; }
    }
}