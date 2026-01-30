namespace Chess.Main.Models
{
    public class ComputerMoveValues(int startSquare, int targetSquare, bool isItPromotionPawnMove, char? promotionPiece)
    {
        public int StartSquare { get; set; } = startSquare;
        public int TargetSquare { get; set; } = targetSquare;
        public bool IsItPromotionPawnMove { get; set; } = isItPromotionPawnMove;
        public char? PromotionPiece { get; set; } = promotionPiece;
    }
}