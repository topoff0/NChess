using System.Text;
using Chess.Main.Core.Helpers.Castling;
using Chess.Main.Core.Helpers.Squares;
using Chess.Main.Core.Movement.Generator;
using Chess.Main.Models;

namespace Chess.Main.Core.MoveNotation
{
    public class GenerateMoveNotationRequest(int startSquare,
                                            int targetSquare,
                                            Board board,
                                            bool isCapture,
                                            char? movingPieceSymbol = null,
                                            char? chosenPromotionPiece = null,
                                            bool isItPromotionPawnMove = false)
    {
        public char MovingPieceSymbol { get; init; } = movingPieceSymbol.GetValueOrDefault();
        public int StartSquare { get; init; } = startSquare;
        public int TargetSquare { get; init; } = targetSquare;
        public Board Board { get; init; } = board;
        public bool IsCapture { get; init; } = isCapture;
        public bool IsItPromotionPawnMove { get; init; } = isItPromotionPawnMove;
        public char? ChosenPromotionPieceSymbol { get; init; } = chosenPromotionPiece;
    }

    public static class MoveNotation
    {
        public static string Generate(GenerateMoveNotationRequest request)
        {
            if (request.IsItPromotionPawnMove && request.ChosenPromotionPieceSymbol.HasValue)
                return GeneratePromotePawnNotation(request.StartSquare,
                                                   request.TargetSquare,
                                                   request.Board,
                                                   request.ChosenPromotionPieceSymbol.Value,
                                                   request.IsCapture);
            else
                return GenerateRegularMoveNotation(request.MovingPieceSymbol,
                                                   request.StartSquare,
                                                   request.TargetSquare,
                                                   request.Board,
                                                   request.IsCapture);
        }

        private static string GenerateRegularMoveNotation(char movingPieceSymbol,
                                                          int startSquare,
                                                          int targetSquare,
                                                          Board board,
                                                          bool isCapture)
        {
            StringBuilder moveNotationSB = new();

            bool isCastlingMove = CastleHelper.IsCastleMove(startSquare, targetSquare, board);

            // Castle move
            if (isCastlingMove)
            {
                moveNotationSB.Append(CastleHelper.IsKingCastle(startSquare, targetSquare) ? "O-O" : "O-O-O");
            }
            else // Regular move
            {
                moveNotationSB.Append(movingPieceSymbol);
                // If it capture move append 'x'
                if (isCapture)
                    moveNotationSB.Append('x');
            }

            // Append target square if it's not castle
            if (!isCastlingMove)
            {
                if (SquaresHelper.SquareIndexToStringSquare.TryGetValue(targetSquare, out var value))
                    moveNotationSB.Append(value);
                else moveNotationSB.Append("[unknown_square]");

                // Append '+' if king under attack
                if (KingMovement.IsKingUnderAttack(board))
                {
                    moveNotationSB.Append('+');
                }
            }
            // TODO implement end game notation

            return moveNotationSB.ToString();
        }

        private static string GeneratePromotePawnNotation(int startSquare,
                                                          int targetSquare,Board board,
                                                          char chosenPromotionPiece,
                                                          bool isCapture)
        {
            StringBuilder moveNotationSB = new();

            if (isCapture) // Append pawn file and symbol 'x' if it's capture move
            {
                char pawnFile = SquaresHelper.SquareIndexToStringSquare[startSquare][0];
                moveNotationSB.Append($"{pawnFile}x");
            }

            moveNotationSB.Append(SquaresHelper.SquareIndexToStringSquare[targetSquare]);
            moveNotationSB.Append($"={chosenPromotionPiece}");

            // Append '+' if king under attack
            if (KingMovement.IsKingUnderAttack(board))
            {
                moveNotationSB.Append('+');
            }

            // TODO implement end game notation
            return moveNotationSB.ToString();
        }
    }
}