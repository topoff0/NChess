using System.Text;
using Chess.Main.Core.Helpers.BitOperation;
using Chess.Main.Core.Helpers.Squares;
using Chess.Main.Models;

namespace Chess.Main.Core.FEN
{
    public static class FenUtility
    {
        public static string GenerateFenFromBoard(Board board)
        {
            StringBuilder fenBuilder = new();

            // Generate positions string
            int emptyCount = 0;
            for (int rank = 7; rank >= 0; rank--)
            {
                for (int file = 7; file >= 0; file--)
                {
                    int squareIndex = rank * 8 + file;
                    char? pieceSymbol = SquaresHelper.GetPieceSymbolFromSquare(board, squareIndex);

                    if (!pieceSymbol.HasValue) emptyCount++;
                    else
                    {
                        if (emptyCount > 0)
                        {
                            fenBuilder.Append(emptyCount);
                            emptyCount = 0;
                        }
                        fenBuilder.Append(pieceSymbol.Value);
                    }
                }
                if (emptyCount > 0)
                {
                    fenBuilder.Append(emptyCount);
                    emptyCount = 0;
                }
                if (rank > 0) fenBuilder.Append('/');
            }

            // Whose turn
            fenBuilder.Append(board.GetIsWhiteTurn() ? " w" : " b");

            // Castling
            StringBuilder castling = new();
            if (board.GetCanWhiteKingCastle()) castling.Append('K');
            if (board.GetCanWhiteQueenCastle()) castling.Append('Q');
            if (board.GetCanBlackKingCastle()) castling.Append('k');
            if (board.GetCanBlackQueenCastle()) castling.Append('q');
            fenBuilder.Append(' ').Append(castling.Length == 0 ? '-' : castling);

            // En passant
            ulong? enPassantTarget = board.GetEnPassantTarget();
            if(enPassantTarget.HasValue)
            {
                int square = BitHelper.GetFirstBitIndex(enPassantTarget.Value);
                string squareName = SquaresHelper.StringSquareToSquareIndex.FirstOrDefault(x => x.Value == square).Key;
                fenBuilder.Append(' ').Append(squareName);
            }
            else fenBuilder.Append(" -");

            // Moves without capture
            fenBuilder.Append(' ').Append(board.GetDrawMoves());

            // Coming move count
            fenBuilder.Append(' ').Append(board.GetComingMoveCount());

            return fenBuilder.ToString();
        }
        
        public static Board LoadBoardFromFen(string fen)
        {
            string[] fenArr = fen.Split(' ');
            string positions = fenArr[0];
            string whoseTurn = fenArr[1];
            string castling = fenArr[2];
            string strEnPassantSquare = fenArr[3];
            string strMovesWithoutCapture = fenArr[4];
            string strComingMoveCount = fenArr[5];


            int bitPosition = 63;

            ulong whitePawns = 0UL, whiteKnights = 0UL, whiteBishops = 0UL, whiteRooks = 0UL, whiteQueens = 0UL, whiteKing = 0UL;
            ulong blackPawns = 0UL, blackKnights = 0UL, blackBishops = 0UL, blackRooks = 0UL, blackQueens = 0UL, blackKing = 0UL;

            foreach (char symbol in positions)
            {
                // New rank
                if(symbol == '/')
                {
                    continue;
                }
                // Skip squares
                if (char.IsDigit(symbol))
                {
                    bitPosition -= (int)char.GetNumericValue(symbol);
                    continue;
                }

                int pieceType = Piece.GetPieceTypeFromSymbol(symbol);

                // Upper - white, lower - black
                bool isWhite = char.IsUpper(symbol);

                if(isWhite)
                {
                    switch (pieceType)
                    {
                        case Piece.Pawn: whitePawns |= 1UL << bitPosition; break;
                        case Piece.Knight: whiteKnights |= 1UL << bitPosition; break;
                        case Piece.Bishop: whiteBishops |= 1UL << bitPosition; break;
                        case Piece.Rook: whiteRooks |= 1UL << bitPosition; break;
                        case Piece.Queen: whiteQueens |= 1UL << bitPosition; break;
                        case Piece.King: whiteKing |= 1UL << bitPosition; break;
                    }
                }
                else
                {
                    switch (pieceType)
                    {
                        case Piece.Pawn: blackPawns |= 1UL << bitPosition; break;
                        case Piece.Knight: blackKnights |= 1UL << bitPosition; break;
                        case Piece.Bishop: blackBishops |= 1UL << bitPosition; break;
                        case Piece.Rook: blackRooks |= 1UL << bitPosition; break;
                        case Piece.Queen: blackQueens |= 1UL << bitPosition; break;
                        case Piece.King: blackKing |= 1UL << bitPosition; break;
                    }
                }
                bitPosition--;
            }

            bool isWhiteTurn = whoseTurn[0] == 'w';

            bool canWhiteKingCastle = castling.Contains('K');
            bool canWhiteQueenCastle = castling.Contains('Q');
            bool canBlackKingCastle = castling.Contains('k');
            bool canBlackQueenCastle = castling.Contains('q');

            int? enPassantSquare = null;
            if (strEnPassantSquare != "-")
            {
                enPassantSquare = SquaresHelper.StringSquareToSquareIndex.TryGetValue(strEnPassantSquare, out int value) ? value : null;
            }

            _ = int.TryParse(strMovesWithoutCapture, out int movesWithoutCapture);
            _ = int.TryParse(strComingMoveCount, out int comingMoveCount);
  
            // ! DO NOT CHANGE THE ORDER
            return new Board(whitePawns, whiteKnights, whiteBishops, whiteRooks, whiteQueens, whiteKing,
                             canWhiteKingCastle, canWhiteQueenCastle,

                             blackPawns, blackKnights, blackBishops, blackRooks, blackQueens, blackKing,
                             canBlackKingCastle, canBlackQueenCastle,
                             
                             isWhiteTurn, enPassantSquare, movesWithoutCapture, comingMoveCount);
        }
    }

    
}