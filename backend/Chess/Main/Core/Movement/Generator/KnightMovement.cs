using Chess.Main.Core.Helpers;
using Chess.Main.Models;

namespace Chess.Main.Core.Movement.Generator
{
    public static class KnightMovement
    {
        // Knight moves (just for better understanding)
        /*
        k - knight position
        x - possible moves
                . . . . . . . .
                . . . x . x . . 
                . . x . . . x . 
                . . . . k . . . 
                . . x . . . x . 
                . . . x . x . . 
                . . . . . . . . 
                . . . . . . . .
                ---------------
                A B C D E F G H
        */

        public static ulong Generate(int squareIndex, Board board, bool isWhiteTurn)
        {
            ulong bitboardPosition = 1UL << squareIndex;
            
            ulong knight = (isWhiteTurn ? board.GetWhiteKnights() : board.GetBlackKnights()) & bitboardPosition;
            if (knight == 0) return 0; // if there are no allied knights on the board

            ulong alliedPieces = isWhiteTurn ? board.GetWhitePieces() : board.GetBlackPieces();

            // Clockwise
            ulong NNE = ((knight & Masks.NotHFile) << 15) & ~alliedPieces;
            ulong NEE = ((knight & Masks.NotGHFile) << 6) & ~alliedPieces;

            ulong SEE = ((knight & Masks.NotGHFile) >> 10) & ~alliedPieces;
            ulong SSE = ((knight & Masks.NotHFile) >> 17) & ~alliedPieces;

            ulong SSW = ((knight & Masks.NotAFile) >> 15) & ~alliedPieces;
            ulong SWW = ((knight & Masks.NotABFile) >> 6) & ~alliedPieces;

            ulong NWW = ((knight & Masks.NotABFile) << 10) & ~alliedPieces;
            ulong NNW = ((knight & Masks.NotAFile) << 17) & ~alliedPieces;

            return NNE | NEE | SEE | SSE | SSW | SWW  | NWW | NNW;
        }
    }
}