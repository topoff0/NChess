using Chess.Main.Core.FEN;
using Chess.Main.Core.Helpers;
using Chess.Main.Core.Helpers.BitOperation;
using Chess.Main.Models;


namespace Chess.Main.Core.Movement.Generator
{
    public static class KingMovement
    {
        private static readonly ulong[] lookUpDefaultMoves = new ulong[64];

        static KingMovement()
        {
            InitializeDefaultMovesTable();
        }

        public static bool IsKingUnderAttack(Board board)
        {
            bool isWhiteTurn = board.GetIsWhiteTurn();
            // Get king bitboard
            ulong kingBit = isWhiteTurn ? board.GetWhiteKing() : board.GetBlackKing();
            // Get under attack squares
            ulong attackedSquares = GetUnderAttackSquares(board, !isWhiteTurn);
            
            return (kingBit & attackedSquares) != 0;
        }
        public static bool WillKingBeInSafeAfterImagineMove(Board board)
        {
            // Get king bitboard
            ulong kingBit = !board.GetIsWhiteTurn() ? board.GetWhiteKing() : board.GetBlackKing();
            // Get under attack squares
            ulong attackedSquares = GetUnderAttackSquares(board, board.GetIsWhiteTurn());
            
            return (kingBit & attackedSquares) == 0;
        }

        public static ulong Generate(int squareIndex, Board board, bool checkSafety = true)
        {
            ulong result = lookUpDefaultMoves[squareIndex];
            bool isWhite = board.GetIsWhiteTurn();
            ulong ourPieces = isWhite ? board.GetWhitePieces() : board.GetBlackPieces();
            
            result &= ~ourPieces;

            if (!checkSafety) 
                return result;
            // ?
            ulong attackedSquares = GetUnderAttackSquares(board, !isWhite);
            ulong validMoves = GetCastlingMask(board, attackedSquares);

            // Check all potential moves for king
            foreach (int targetSquare in BitHelper.SquareIndexesFromBitboard(result))
            {
                Board tempBoard = FenUtility.LoadBoardFromFen(FenUtility.GenerateFenFromBoard(board));
                
                // Imaginative move
                tempBoard.MakeRegularMove(squareIndex, targetSquare, ref tempBoard);
                
                // Check king safety in this move
                if (WillKingBeInSafeAfterImagineMove(tempBoard))
                {
                    validMoves |= 1UL << targetSquare;
                }
            }

            return validMoves;
        }

        private static void InitializeDefaultMovesTable()
        {
            for (int i = 0; i < 64; i++)
            {
                ulong moves = 0Ul;

                int rank = i / 8;
                int file = i % 8;

                for (int dr = -1; dr <= 1; dr++)
                {
                    for (int df = -1; df <= 1; df++)
                    {
                        if (dr == df && dr == 0) continue; // Skip king's square

                        int newRank = rank + dr;
                        int newFile = file + df;
                        if (newRank >= 0 && newRank < 8 && newFile >= 0 && newFile < 8)
                        {
                            moves |= 1UL << (newRank * 8 + newFile);
                        }
                    }
                }
                lookUpDefaultMoves[i] = moves;
            }
        }

        private static ulong GetUnderAttackSquares(Board board, bool isWhiteAttack)
        {            
            // Get all enemy pieces bitboards
            ulong enemyPawns = isWhiteAttack ? board.GetWhitePawns() : board.GetBlackPawns();
            ulong enemyKnights = isWhiteAttack ? board.GetWhiteKnights() : board.GetBlackKnights();
            ulong enemyBishops = isWhiteAttack ? board.GetWhiteBishops() : board.GetBlackBishops();
            ulong enemyRooks = isWhiteAttack ? board.GetWhiteRooks() : board.GetBlackRooks();
            ulong enemyQueens = isWhiteAttack ? board.GetWhiteQueens() : board.GetBlackQueens();
            ulong enemyKing = isWhiteAttack ? board.GetWhiteKing() : board.GetBlackKing();

            ulong attackedMask = 0UL;

            // Add to attacked mask pawns attack
            ulong pawnsAttack = isWhiteAttack
            ? (enemyPawns & Masks.NotHFile) << 7 | (enemyPawns & Masks.NotAFile) << 9
            : (enemyPawns & Masks.NotAFile) >> 7 | (enemyPawns & Masks.NotHFile) >> 9;
            attackedMask |= pawnsAttack;


            int knightsCount = BitHelper.BitsCount(enemyKnights);
            for (int i = 0; i < knightsCount; i++)
            {
                int squareIndex = BitHelper.GetFirstBitIndex(enemyKnights);
                attackedMask |= KnightMovement.Generate(squareIndex, board, isWhiteAttack);
                enemyKnights &= enemyKnights - 1; // delete first bit
            }

            // Add to attacked mask bishops and queens diagonal attacks
            ulong bishopsAndQueens = enemyBishops | enemyQueens;
            while(bishopsAndQueens != 0)
            {
                int squareindex = BitHelper.GetFirstBitIndex(bishopsAndQueens);
                attackedMask |= BishopMovement.Generate(squareindex, board, isWhiteAttack);
                bishopsAndQueens &= bishopsAndQueens - 1; // delete first bit
            }

            // Add to attacked mask rooks and queens orthogonal attacks
            ulong rooksAndQueens = enemyRooks | enemyQueens;
            while(rooksAndQueens != 0)
            {
                int squareIndex = BitHelper.GetFirstBitIndex(rooksAndQueens);
                attackedMask |= RookMovement.Generate(squareIndex, board, isWhiteAttack);
                rooksAndQueens &= rooksAndQueens - 1;
            }

            // Add to attacked mask king attack
            int enemyKingSquareIndex = BitHelper.GetFirstBitIndex(enemyKing);
            attackedMask |= KingMovement.Generate(enemyKingSquareIndex, board, false);

            // Return safety mask for king
            return attackedMask;
        }


        private static ulong GetCastlingMask(Board board, ulong attackedSquares)
        {
            ulong result = 0UL;

            ulong blockers = board.GetAllPieces();

            bool isWhiteTurn = board.GetIsWhiteTurn();

            int kingSquareIndex = isWhiteTurn ? 3 : 59;

            // The squares that the king must pass through
            ulong kingCastleSquares = isWhiteTurn ? 0x00_00_00_00_00_00_00_06UL : 0x06_00_00_00_00_00_00_00UL;
            ulong queenCastleSquares = isWhiteTurn ? 0x00_00_00_00_00_00_00_70UL : 0x70_00_00_00_00_00_00_00UL;

            // Check if the king is under attack now
            bool isKingUnderAttack = (attackedSquares & (1UL << kingSquareIndex)) != 0;

            // Check if the squares that the king must pass through are not under attack
            bool isKingCastleNotUnderAttack = (kingCastleSquares & attackedSquares) == 0;
            bool isQueenCastleNotUnderAttack = (queenCastleSquares & attackedSquares) == 0;

            // Check if the squares that the king must pass through are free
            bool isKingCastleFree = (blockers & kingCastleSquares) == 0;
            bool isQueenCastleFree = (blockers & queenCastleSquares) == 0;

            if (!isKingUnderAttack)
            {
                if (isWhiteTurn)
                {
                    if (board.GetCanWhiteKingCastle() && isKingCastleNotUnderAttack && isKingCastleFree)
                    result |= Masks.WhiteKingCastleMask;
                    if (board.GetCanWhiteQueenCastle() && isQueenCastleNotUnderAttack && isQueenCastleFree)
                    result |= Masks.WhiteQueenCastleMask;
                }
                else
                {
                    if (board.GetCanBlackKingCastle() && isKingCastleNotUnderAttack && isKingCastleFree)
                    result |= Masks.BlackKingCastleMask;
                    if (board.GetCanBlackQueenCastle() && isQueenCastleNotUnderAttack && isQueenCastleFree)
                    result |= Masks.BlackQueenCastleMask;
                }
            }

            return result;
        }
    }
}