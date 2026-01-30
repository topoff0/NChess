using Chess.Main.Core.Helpers.MagicBitboards;
using Chess.Main.Models;

namespace Chess.Main.Core.Movement.Generator
{
    public class QueenMovement
    {
        public static ulong Generate(int squareIndex, Board board, bool isWhiteTurn)
        {
            ulong bishopMask = MagicBitboards.MagicBishopTable[squareIndex].Mask;
            ulong rookMask = MagicBitboards.MagicRookTable[squareIndex].Mask;

            ulong bishopMagic = MagicBitboards.MagicBishopTable[squareIndex].MagicNumber;
            ulong rookMagic = MagicBitboards.MagicRookTable[squareIndex].MagicNumber;

            int relevantBishopBits = MagicBitboards.MagicBishopTable[squareIndex].RelevantBits;
            int relevantRookBits = MagicBitboards.MagicRookTable[squareIndex].RelevantBits;

            ulong blockers = board.GetAllPieces();

            ulong bishopBlockers = blockers & bishopMask;
            ulong rookBlockers = blockers & rookMask;

            ulong bishopIndex = (bishopBlockers * bishopMagic) >> (64 - relevantBishopBits);
            ulong rookIndex = (rookBlockers * rookMagic) >> (64 - relevantRookBits);


            // TODO : IT IS NOT WORKING (there must be null in initial position); 
            ulong bishopMoves = MagicBitboards.MagicBishopTable[squareIndex].AttackTable[bishopIndex];
            ulong rookMoves = MagicBitboards.MagicRookTable[squareIndex].AttackTable[rookIndex];

            ulong bishopMoves2 = BishopMovement.Generate(squareIndex, board, isWhiteTurn);

            ulong alliedPieces = isWhiteTurn ? board.GetWhitePieces() : board.GetBlackPieces();

            return (bishopMoves | rookMoves) & ~alliedPieces;
        }
    }
}