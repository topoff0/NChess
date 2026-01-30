using Chess.Main.Models;

namespace Chess.Main.Evaluation
{
    public static class Eval
    {
        private static readonly int[] PieceValues = { 100, 300, 300, 500, 900, 20000 }; // Pawn, Knight, Bishop, Rook, Queen, King

        public static int Evaluate(Board board)
        {
            int score = 0;
            
            // Material counting
            score += CountPieces(board.GetWhitePawns()) * PieceValues[0];
            score += CountPieces(board.GetWhiteKnights()) * PieceValues[1];
            score += CountPieces(board.GetWhiteBishops()) * PieceValues[2];
            score += CountPieces(board.GetWhiteRooks()) * PieceValues[3];
            score += CountPieces(board.GetWhiteQueens()) * PieceValues[4];
            score += CountPieces(board.GetWhiteKing()) * PieceValues[5];

            score -= CountPieces(board.GetBlackPawns()) * PieceValues[0];
            score -= CountPieces(board.GetBlackKnights()) * PieceValues[1];
            score -= CountPieces(board.GetBlackBishops()) * PieceValues[2];
            score -= CountPieces(board.GetBlackRooks()) * PieceValues[3];
            score -= CountPieces(board.GetBlackQueens()) * PieceValues[4];
            score -= CountPieces(board.GetBlackKing()) * PieceValues[5];

            return board.GetIsWhiteTurn() ? score : -score;
        }

        private static int CountPieces(ulong bitboard)
        {
            int count = 0;
            while (bitboard != 0)
            {
                count++;
                bitboard &= (bitboard - 1); // Clear the least significant 1-bit
            }
            return count;
        }
    }
}