using Chess.Main.Models;
using Chess.Main.Evaluation;
using Chess.Main.Core.FEN;
using Chess.Main.Core.Movement.Generator;
using Chess.Main.Core.Helpers.BitOperation;

namespace Chess.Main.Search
{
    public static class SearchAlgorithm
    {
        private static readonly char[,] promotePieces = {{'Q', 'R', 'B', 'N'}, {'q', 'r', 'b', 'n'}};
        private const int MAX_DEPTH = 4;
        private static ComputerMoveValues bestMove;

        public static ComputerMoveValues Search(Dictionary<int, List<int>> moves, Board board)
        {
            if (moves == null || moves.Count == 0)
                throw new ArgumentException("No moves available");

            bestMove = null;
            int alpha = int.MinValue;
            int beta = int.MaxValue;
            
            AlphaBeta(board, MAX_DEPTH, alpha, beta, true);
            
            if (bestMove == null)
            {
                // Fallback to random move if no best move found
                Random rand = new();
                var validMoves = moves.Where(m => m.Value != null && m.Value.Count > 0).ToList();
                if (validMoves.Count == 0)
                    throw new ArgumentException("No valid moves available");

                var selectedMove = validMoves[rand.Next(validMoves.Count)];
                int startSquareIndex = selectedMove.Key;
                List<int> movesOnChosenSquare = selectedMove.Value;
                int targetSquare = movesOnChosenSquare[rand.Next(movesOnChosenSquare.Count)];

                bool isWhiteTurn = board.GetIsWhiteTurn();
                bool isItPawnMove = isWhiteTurn ? 
                                    (board.GetWhitePawns() & (1UL << startSquareIndex)) != 0 :
                                    (board.GetBlackPawns() & (1UL << startSquareIndex)) != 0;

                char? promotionPiece = null;
                bool isItPromotionPawnMove = IsItPromotionPawnMove(isWhiteTurn, targetSquare, isItPawnMove);
                if (isItPromotionPawnMove)
                {
                    promotionPiece = isWhiteTurn ? promotePieces[0, 0] : promotePieces[1, 0];
                }

                bestMove = new ComputerMoveValues(startSquareIndex, targetSquare, isItPromotionPawnMove, promotionPiece);
            }

            return bestMove;
        }

        private static int AlphaBeta(Board board, int depth, int alpha, int beta, bool maximizingPlayer)
        {
            if (depth == 0)
                return Eval.Evaluate(board);

            // Get legal moves using the Movement class approach
            Dictionary<int, List<int>> legalMoves = GetLegalMoves(board);
            
            if (legalMoves.Count == 0)
            {
                // If no moves available, check if it's checkmate or stalemate
                if (KingMovement.IsKingUnderAttack(board))
                    return maximizingPlayer ? int.MinValue : int.MaxValue;
                return 0; // Stalemate
            }

            if (maximizingPlayer)
            {
                int maxEval = int.MinValue;
                foreach (var startSquare in legalMoves.Keys)
                {
                    if (legalMoves[startSquare] == null || legalMoves[startSquare].Count == 0)
                        continue;

                    foreach (var targetSquare in legalMoves[startSquare])
                    {
                        Board newBoard = FenUtility.LoadBoardFromFen(FenUtility.GenerateFenFromBoard(board));
                        bool isItPawnMove = board.GetIsWhiteTurn() ? 
                            (board.GetWhitePawns() & (1UL << startSquare)) != 0 :
                            (board.GetBlackPawns() & (1UL << startSquare)) != 0;
                        
                        bool isPromotion = IsItPromotionPawnMove(board.GetIsWhiteTurn(), targetSquare, isItPawnMove);
                        char? promotionPiece = isPromotion ? (board.GetIsWhiteTurn() ? promotePieces[0, 0] : promotePieces[1, 0]) : null;
                        
                        if (isPromotion)
                            newBoard.PromotePawn(1UL << startSquare, 1UL << targetSquare, promotionPiece.Value, ref newBoard);
                        else
                            newBoard.MakeRegularMove(startSquare, targetSquare, ref newBoard);

                        int eval = AlphaBeta(newBoard, depth - 1, alpha, beta, false);
                        
                        if (eval > maxEval)
                        {
                            maxEval = eval;
                            if (depth == MAX_DEPTH)
                            {
                                bestMove = new ComputerMoveValues(startSquare, targetSquare, isPromotion, promotionPiece);
                            }
                        }
                        
                        alpha = Math.Max(alpha, eval);
                        if (beta <= alpha)
                            break;
                    }
                }
                return maxEval;
            }
            else
            {
                int minEval = int.MaxValue;
                foreach (var startSquare in legalMoves.Keys)
                {
                    if (legalMoves[startSquare] == null || legalMoves[startSquare].Count == 0)
                        continue;

                    foreach (var targetSquare in legalMoves[startSquare])
                    {
                        Board newBoard = FenUtility.LoadBoardFromFen(FenUtility.GenerateFenFromBoard(board));
                        bool isItPawnMove = board.GetIsWhiteTurn() ? 
                            (board.GetWhitePawns() & (1UL << startSquare)) != 0 :
                            (board.GetBlackPawns() & (1UL << startSquare)) != 0;
                        
                        bool isPromotion = IsItPromotionPawnMove(board.GetIsWhiteTurn(), targetSquare, isItPawnMove);
                        char? promotionPiece = isPromotion ? (board.GetIsWhiteTurn() ? promotePieces[0, 0] : promotePieces[1, 0]) : null;
                        
                        if (isPromotion)
                            newBoard.PromotePawn(1UL << startSquare, 1UL << targetSquare, promotionPiece.Value, ref newBoard);
                        else
                            newBoard.MakeRegularMove(startSquare, targetSquare, ref newBoard);

                        int eval = AlphaBeta(newBoard, depth - 1, alpha, beta, true);
                        minEval = Math.Min(minEval, eval);
                        
                        beta = Math.Min(beta, eval);
                        if (beta <= alpha)
                            break;
                    }
                }
                return minEval;
            }
        }

        private static Dictionary<int, List<int>> GetLegalMoves(Board board)
        {
            Dictionary<int, List<int>> legalMoves = new();
            Dictionary<char, ulong> piecesCollections = GetAlliedPieces(board);

            for (int square = 0; square < 64; square++)
            {
                foreach (ulong pieces in piecesCollections.Values)
                {
                    if ((pieces & (1UL << square)) != 0)
                    {
                        char pieceSymbol = piecesCollections.FirstOrDefault(x => (x.Value & (1UL << square)) != 0).Key;
                        ulong rawMoves = GetLegalMovesByPieceSymbol(square, pieceSymbol, board);

                        List<int> validMoves = new();
                        foreach (int targetSquare in BitHelper.SquareIndexesFromBitboard(rawMoves))
                        {
                            Board tempBoard = FenUtility.LoadBoardFromFen(FenUtility.GenerateFenFromBoard(board));
                            tempBoard.MakeRegularMove(square, targetSquare, ref tempBoard);
                            
                            if (KingMovement.WillKingBeInSafeAfterImagineMove(tempBoard))
                            {
                                validMoves.Add(targetSquare);
                            }
                        }
                        
                        if (validMoves.Count > 0)
                        {
                            legalMoves[square] = validMoves;
                        }
                    }
                }
            }

            return legalMoves;
        }

        private static Dictionary<char, ulong> GetAlliedPieces(Board board)
        {
            Dictionary<char, ulong> pieces = new();

            if(board.GetIsWhiteTurn())
            {
                pieces.Add('P', board.GetWhitePawns());
                pieces.Add('N', board.GetWhiteKnights());
                pieces.Add('B', board.GetWhiteBishops());
                pieces.Add('R', board.GetWhiteRooks());
                pieces.Add('Q', board.GetWhiteQueens());
                pieces.Add('K', board.GetWhiteKing());
            }
            else
            {
                pieces.Add('p', board.GetBlackPawns());
                pieces.Add('n', board.GetBlackKnights());
                pieces.Add('b', board.GetBlackBishops());
                pieces.Add('r', board.GetBlackRooks());
                pieces.Add('q', board.GetBlackQueens());
                pieces.Add('k', board.GetBlackKing());
            }

            return pieces;
        }

        private static ulong GetLegalMovesByPieceSymbol(int squareIndex, char pieceSymbol, Board board)
        {
            return pieceSymbol switch
            {
                'P' => PawnMovement.WhiteGenerate(squareIndex, board),
                'N' => KnightMovement.Generate(squareIndex, board, board.GetIsWhiteTurn()),
                'B' => BishopMovement.Generate(squareIndex, board, board.GetIsWhiteTurn()),
                'R' => RookMovement.Generate(squareIndex, board, board.GetIsWhiteTurn()),
                'Q' => QueenMovement.Generate(squareIndex, board, board.GetIsWhiteTurn()),
                'K' => KingMovement.Generate(squareIndex, board),
                'p' => PawnMovement.BlackGenerate(squareIndex, board),
                'n' => KnightMovement.Generate(squareIndex, board, board.GetIsWhiteTurn()),
                'b' => BishopMovement.Generate(squareIndex, board, board.GetIsWhiteTurn()),
                'r' => RookMovement.Generate(squareIndex, board, board.GetIsWhiteTurn()),
                'q' => QueenMovement.Generate(squareIndex, board, board.GetIsWhiteTurn()),
                'k' => KingMovement.Generate(squareIndex, board),
                _ => 0,
            };
        }

        private static bool IsItPromotionPawnMove(bool IsPlayerPlayWhite, int targetSquare, bool isPawnMove)
        {
            if (!isPawnMove)
                return false;
            ulong promotionRank = IsPlayerPlayWhite ? 0xff_00_00_00_00_00_00_00 : 0x00_00_00_00_00_00_00_ff;
            if ((promotionRank & (1UL << targetSquare)) == 0)
                return false;

            return true;
        }
    }
}