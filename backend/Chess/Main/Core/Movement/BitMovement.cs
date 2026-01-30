using Chess.Main.Models;

namespace Chess.Main.Core.Movement
{
    public static class BitMovement
    {
        public static void MoveBit(ref ulong moveBitboard, ulong startBit, ulong targetBit)
        {
            moveBitboard &= ~startBit;
            moveBitboard |= targetBit;
        }

        public static void DeleteBit(ref ulong captureBitboard, ulong targetBit)
        {
            captureBitboard &= ~targetBit;
        }

        public static void Update_AllPieces_And_ColorPieces_Bitboards(ref ulong allPiecesBitboard,
                                                                      ref ulong colorPiecesBitboard,
                                                                      ulong startBit,
                                                                      ulong targetBit)
        {
            allPiecesBitboard &= ~startBit;
            colorPiecesBitboard &= ~startBit;

            allPiecesBitboard |= targetBit;
            colorPiecesBitboard |= targetBit;
        }

        public static void EnPassantMove(ref ulong moveBitboard, ulong startBit, ulong targetBit,
            ref ulong enemyPawnsBitboard, ref ulong enemyPieces, ref ulong allPieces, ulong enPassantBitboard)
        {
            moveBitboard &= ~startBit;
            moveBitboard |= targetBit;

            enemyPawnsBitboard &= ~enPassantBitboard;
            enemyPieces &= ~enPassantBitboard;
            allPieces &= ~enPassantBitboard;
        }

    }
}