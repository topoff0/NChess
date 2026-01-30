namespace Chess.DTO.Requests
{
    public class PawnPromotionRequest
    {
        public int StartSquare { get; set; }
        public int TargetSquare { get; set; }
        public required string FenBeforeMove { get; set; }
        public char ChosenPiece { get; set; }
    }
}