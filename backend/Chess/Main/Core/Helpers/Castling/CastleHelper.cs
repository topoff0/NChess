using Chess.Main.Core.Helpers.BitOperation;
using Chess.Main.Models;

namespace Chess.Main.Core.Helpers.Castling
{
    public static class CastleHelper
    {
        public static bool IsCastleMove(int startSquare, int targetSquare, Board board)
        {

            bool isWhiteTurn = board.GetIsWhiteTurn();
            ulong kingBit = isWhiteTurn ? board.GetWhiteKing() : board.GetBlackKing();
            bool isKingMoving = (kingBit & (1UL << startSquare)) != 0;

            if (!isKingMoving) return false; // not king move

            bool isTwoSquaresMove = Math.Abs(startSquare - targetSquare) == 2;

            return isTwoSquaresMove; // king moved only 1 square => it's not castle move
        }
        public static bool IsKingCastle(int startSquare, int targetSquare)
        {
            return startSquare > targetSquare;
        }
    }
}