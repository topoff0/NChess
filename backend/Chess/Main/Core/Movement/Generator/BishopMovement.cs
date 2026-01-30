using Chess.Main.Core.Helpers.MagicBitboards;
using Chess.Main.Models;

namespace Chess.Main.Core.Movement.Generator
{
    public static class BishopMovement
    {
        public static ulong Generate(int squareIndex, Board board, bool isWhiteTurn)
        {
            ulong mask = MagicBitboards.MagicBishopTable[squareIndex].Mask;
            ulong magic = MagicBitboards.MagicBishopTable[squareIndex].MagicNumber;

            int relevantBits = MagicBitboards.MagicBishopTable[squareIndex].RelevantBits;

            ulong allPieces = board.GetAllPieces();
            ulong blockers = allPieces & mask; // Blockers can be on the edge of the board (we are not interested in it)

            ulong alliedPieces = allPieces & (isWhiteTurn ? board.GetWhitePieces() : board.GetBlackPieces());

            ulong index = (blockers * magic) >> (64 - relevantBits);

            // Bishop can't capture allied pieces
            return MagicBitboards.MagicBishopTable[squareIndex].AttackTable[index] & ~alliedPieces;
        }
    }
}