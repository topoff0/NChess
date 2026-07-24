using Chess.Core.Models;

namespace Chess.Core.Movement.Generator;

public class QueenMovement
{
    public static ulong Generate(int squareIndex, Board board, bool isWhiteTurn)
    {
        ulong bishopMoves = BishopMovement.Generate(squareIndex, board, isWhiteTurn);
        ulong rookMoves = RookMovement.Generate(squareIndex, board, isWhiteTurn);

        return bishopMoves | rookMoves;
    }
}
