using Chess.Main.Core.Helpers.Castling;
using Chess.Main.Core.Movement;
using Humanizer;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Chess.Main.Models
{
    public sealed class Board
    {
        // Bitboards
        private ulong WhitePawns;
        private ulong WhiteKnights;
        private ulong WhiteBishops;
        private ulong WhiteRooks;
        private ulong WhiteQueens;
        private ulong WhiteKing;

        private ulong BlackPawns;
        private ulong BlackKnights;
        private ulong BlackBishops;
        private ulong BlackRooks;
        private ulong BlackQueens;
        private ulong BlackKing;

        private ulong WhitePieces;
        private ulong BlackPieces;

        private ulong allPieces;

        private bool CanWhiteKingCastle;
        private bool CanWhiteQueenCastle;
        private bool CanBlackKingCastle;
        private bool CanBlackQueenCastle;

        private bool IsWhiteTurn;

        private ulong? EnPassantTarget;

        private int DrawMoves;

        private int ComingMoveCount;

        public Board()
        {
            InitializeBoard();
        }

        public Board(ulong whitePawns, ulong whiteKnights, ulong whiteBishops, ulong whiteRooks,
                     ulong whiteQueens, ulong whiteKing, bool canWhiteKingCastle, bool canWhiteQueenCastle,

                     ulong blackPawns, ulong blackKnights, ulong blackBishops, ulong blackRooks,
                     ulong blackQueens, ulong blackKing, bool canBlackKingCastle, bool canBlackQueenCastle,

                     bool isWhiteTurn, int? enPassantSquare, int drawMoves, int comingMoveCount)
        {
            WhitePawns = whitePawns;
            WhiteKnights = whiteKnights;
            WhiteBishops = whiteBishops;
            WhiteRooks = whiteRooks;
            WhiteQueens = whiteQueens;
            WhiteKing = whiteKing;
            WhitePieces = whitePawns | whiteKnights | whiteBishops | whiteRooks | whiteQueens | whiteKing;
            CanWhiteKingCastle = canWhiteKingCastle;
            CanWhiteQueenCastle = canWhiteQueenCastle;

            BlackPawns = blackPawns;
            BlackKnights = blackKnights;
            BlackBishops = blackBishops;
            BlackRooks = blackRooks;
            BlackQueens = blackQueens;
            BlackKing = blackKing;
            BlackPieces = blackPawns | blackKnights | blackBishops | blackRooks | blackQueens | blackKing;
            CanBlackKingCastle = canBlackKingCastle;
            CanBlackQueenCastle = canBlackQueenCastle;

            allPieces = WhitePieces | BlackPieces;

            IsWhiteTurn = isWhiteTurn;
            DrawMoves = drawMoves;
            ComingMoveCount = comingMoveCount;

            if (enPassantSquare.HasValue)
            {
                EnPassantTarget = 1UL << enPassantSquare.Value;
            }
        }
        public Board(Board board)
        {
            // Copy bitboards
            WhitePawns = board.WhitePawns;
            WhiteKnights = board.WhiteKnights;
            WhiteBishops = board.WhiteBishops;
            WhiteRooks = board.WhiteRooks;
            WhiteQueens = board.WhiteQueens;
            WhiteKing = board.WhiteKing;
            WhitePieces = board.WhitePieces;

            BlackPawns = board.BlackPawns;
            BlackKnights = board.BlackKnights;
            BlackBishops = board.BlackBishops;
            BlackRooks = board.BlackRooks;
            BlackQueens = board.BlackQueens;
            BlackKing = board.BlackKing;
            BlackPieces = board.BlackPieces;

            allPieces = board.allPieces;

            // Copy castling rights
            CanWhiteKingCastle = board.CanWhiteKingCastle;
            CanWhiteQueenCastle = board.CanWhiteQueenCastle;
            CanBlackKingCastle = board.CanBlackKingCastle;
            CanBlackQueenCastle = board.CanBlackQueenCastle;

            // Copy game state
            IsWhiteTurn = board.IsWhiteTurn;
            EnPassantTarget = board.EnPassantTarget;
            DrawMoves = board.DrawMoves;
            ComingMoveCount = board.ComingMoveCount;
        }

        // Initial position
        public void InitializeBoard()
        {
            WhitePawns = 0x00_00_00_00_00_00_FF_00;
            WhiteKnights = 0x00_00_00_00_00_00_00_42;
            WhiteBishops = 0x00_00_00_00_00_00_00_24;
            WhiteRooks = 0x00_00_00_00_00_00_00_81;
            WhiteQueens = 0x00_00_00_00_00_00_00_10;
            WhiteKing = 0x00_00_00_00_00_00_00_08;
            WhitePieces = WhitePawns | WhiteKnights | WhiteBishops | WhiteRooks | WhiteQueens | WhiteKing;

            BlackPawns = 0x00_FF_00_00_00_00_00_00;
            BlackKnights = 0x42_00_00_00_00_00_00_00;
            BlackBishops = 0x24_00_00_00_00_00_00_00;
            BlackRooks = 0x81_00_00_00_00_00_00_00;
            BlackQueens = 0x10_00_00_00_00_00_00_00;
            BlackKing = 0x08_00_00_00_00_00_00_00;
            BlackPieces = BlackPawns | BlackKnights | BlackBishops | BlackRooks | BlackQueens | BlackKing;

            allPieces = WhitePieces | BlackPieces;

            // Set initial game state
            IsWhiteTurn = true;
            CanWhiteKingCastle = true;
            CanWhiteQueenCastle = true;
            CanBlackKingCastle = true;
            CanBlackQueenCastle = true;
            EnPassantTarget = null;
            DrawMoves = 0;
            ComingMoveCount = 1;
        }

        public ulong GetWhitePawns() => WhitePawns;
        public ulong GetWhiteKnights() => WhiteKnights;
        public ulong GetWhiteBishops() => WhiteBishops;
        public ulong GetWhiteRooks() => WhiteRooks;
        public ulong GetWhiteQueens() => WhiteQueens;
        public ulong GetWhiteKing() => WhiteKing;

        public ulong GetBlackPawns() => BlackPawns;
        public ulong GetBlackKnights() => BlackKnights;
        public ulong GetBlackBishops() => BlackBishops;
        public ulong GetBlackRooks() => BlackRooks;
        public ulong GetBlackQueens() => BlackQueens;
        public ulong GetBlackKing() => BlackKing;


        public ulong GetWhitePieces() => WhitePieces;
        public ulong GetBlackPieces() => BlackPieces;

        public ulong GetAllPieces() => allPieces;

        public bool GetIsWhiteTurn() => IsWhiteTurn;

        public bool GetCanWhiteKingCastle() => CanWhiteKingCastle;
        public bool GetCanWhiteQueenCastle() => CanWhiteQueenCastle;
        public bool GetCanBlackKingCastle() => CanBlackKingCastle;
        public bool GetCanBlackQueenCastle() => CanBlackQueenCastle;

        public ulong? GetEnPassantTarget() => EnPassantTarget;

        public int GetDrawMoves() => DrawMoves;

        public int GetComingMoveCount() => ComingMoveCount;

        public void MakeRegularMove(int startSquare, int targetSquare, ref Board board)
        {
            UpdateDrawConditions(startSquare, targetSquare, ref board);

            ulong startBit = 1UL << startSquare;
            ulong targetBit = 1UL << targetSquare;

            if (CastleHelper.IsCastleMove(startSquare, targetSquare, board))
            {
                bool isKingCastle = CastleHelper.IsKingCastle(startSquare, targetSquare);
                MakeCastleMove(ref board, isKingCastle);
            }
            else if (IsItEnPassantMove(startBit, targetBit, board))
            {
                MakeEnPassantMove(startBit, targetBit, ref board);
            }
            else
            {
                if (IsCaptureMove(targetBit, WhitePieces, BlackPieces))
                {
                    MakeMoveWithCapture(startBit, targetBit, ref board);
                }
                else MakeMoveWithoutCapture(startBit, targetBit, ref board);
            }

            board.IsWhiteTurn = !IsWhiteTurn;

        }

        private static void MakeMoveWithoutCapture(ulong startBit, ulong targetBit, ref Board board)
        {
            bool isItEnPassantMove = IsItEnPassantMove(startBit, targetBit);

            if (board.IsWhiteTurn) // Move white piece bitboard
            {
                if ((board.WhitePawns & startBit) != 0)
                {
                    BitMovement.MoveBit(moveBitboard: ref board.WhitePawns, startBit: startBit, targetBit: targetBit);
                    board.EnPassantTarget = isItEnPassantMove ? targetBit : startBit;
                }
                if ((board.WhiteKnights & startBit) != 0) BitMovement.MoveBit(moveBitboard: ref board.WhiteKnights, startBit: startBit, targetBit: targetBit);
                if ((board.WhiteBishops & startBit) != 0) BitMovement.MoveBit(moveBitboard: ref board.WhiteBishops, startBit: startBit, targetBit: targetBit);
                if ((board.WhiteRooks & startBit) != 0)
                {
                    BitMovement.MoveBit(moveBitboard: ref board.WhiteRooks, startBit: startBit, targetBit: targetBit);
                    if (startBit == (1UL << 0)) // a1 left rook
                    {
                        board.CanWhiteQueenCastle = false;
                    }
                    else if (startBit == (1UL << 7)) // h1 right rook
                    {
                        board.CanWhiteKingCastle = false;
                    }
                }
                if ((board.WhiteQueens & startBit) != 0) BitMovement.MoveBit(moveBitboard: ref board.WhiteQueens, startBit: startBit, targetBit: targetBit);
                if ((board.WhiteKing & startBit) != 0)
                {
                    BitMovement.MoveBit(moveBitboard: ref board.WhiteKing, startBit: startBit, targetBit: targetBit);
                    BanWhiteCastling(ref board);
                }


                BitMovement.Update_AllPieces_And_ColorPieces_Bitboards(allPiecesBitboard: ref board.allPieces,
                                                                       colorPiecesBitboard: ref board.WhitePieces,
                                                                       startBit: startBit,
                                                                       targetBit: targetBit);
            }
            else // Move black piece bitboard
            {
                if ((board.BlackPawns & startBit) != 0)
                {
                    BitMovement.MoveBit(moveBitboard: ref board.BlackPawns, startBit: startBit, targetBit: targetBit);
                    board.EnPassantTarget = isItEnPassantMove ? targetBit : startBit;
                }
                if ((board.BlackKnights & startBit) != 0) BitMovement.MoveBit(moveBitboard: ref board.BlackKnights, startBit: startBit, targetBit: targetBit);
                if ((board.BlackBishops & startBit) != 0) BitMovement.MoveBit(moveBitboard: ref board.BlackBishops, startBit: startBit, targetBit: targetBit);
                if ((board.BlackRooks & startBit) != 0)
                {
                    BitMovement.MoveBit(moveBitboard: ref board.BlackRooks, startBit: startBit, targetBit: targetBit);
                    if (startBit == (1UL << 56)) // a8 left rook
                    {
                        board.CanBlackQueenCastle = false;
                    }
                    else if (startBit == (1UL << 63)) // h8 right rook
                    {
                        board.CanBlackKingCastle = false;
                    }
                }
                if ((board.BlackQueens & startBit) != 0) BitMovement.MoveBit(moveBitboard: ref board.BlackQueens, startBit: startBit, targetBit: targetBit);
                if ((board.BlackKing & startBit) != 0)
                {
                    BitMovement.MoveBit(moveBitboard: ref board.BlackKing, startBit: startBit, targetBit: targetBit);
                    BanBlackCastling(ref board);
                }

                BitMovement.Update_AllPieces_And_ColorPieces_Bitboards(allPiecesBitboard: ref board.allPieces,
                                                                       colorPiecesBitboard: ref board.BlackPieces,
                                                                       startBit: startBit,
                                                                       targetBit: targetBit);
            }

            if (!isItEnPassantMove && board.EnPassantTarget != null)
                board.EnPassantTarget = null;
        }

        private static void MakeMoveWithCapture(ulong startBit, ulong targetBit, ref Board board)
        {
            DeletePieceOnTargetBit(targetBit, ref board);

            MakeMoveWithoutCapture(startBit, targetBit, ref board);
        }


        private static void MakeCastleMove(ref Board board, bool isKingCastle)
        {
            if (board.IsWhiteTurn) // White castle
            {
                ulong kingStartBit = board.WhiteKing;
                ulong kingTargetBit = isKingCastle ? 1UL << 1 : 1UL << 5;

                ulong rookStartBit = isKingCastle ? 1UL : 1UL << 7;
                ulong rookTargetBit = isKingCastle ? 1UL << 2 : 1UL << 4;

                BitMovement.MoveBit(ref board.WhiteKing, kingStartBit, kingTargetBit);
                BitMovement.MoveBit(ref board.WhiteRooks, rookStartBit, rookTargetBit);

                BitMovement.Update_AllPieces_And_ColorPieces_Bitboards(allPiecesBitboard: ref board.allPieces,
                                                                       colorPiecesBitboard: ref board.WhitePieces,
                                                                       startBit: kingStartBit,
                                                                       targetBit: kingTargetBit);

                BitMovement.Update_AllPieces_And_ColorPieces_Bitboards(allPiecesBitboard: ref board.allPieces,
                                                                       colorPiecesBitboard: ref board.WhitePieces,
                                                                       startBit: rookStartBit,
                                                                       targetBit: rookTargetBit);

                board.CanWhiteKingCastle = false;
                board.CanWhiteQueenCastle = false;
            }
            else // Black castle
            {
                ulong kingStartBit = 1UL << 59;
                ulong kingTargetBit = isKingCastle ? 1UL << 57 : 1UL << 61;

                ulong rookStartBit = isKingCastle ? 1UL << 56 : 1UL << 63;
                ulong rookTargetBit = isKingCastle ? 1UL << 58 : 1UL << 60;

                BitMovement.MoveBit(ref board.BlackKing, kingStartBit, kingTargetBit);
                BitMovement.MoveBit(ref board.BlackRooks, rookStartBit, rookTargetBit);

                BitMovement.Update_AllPieces_And_ColorPieces_Bitboards(allPiecesBitboard: ref board.allPieces,
                                                                       colorPiecesBitboard: ref board.BlackPieces,
                                                                       startBit: kingStartBit,
                                                                       targetBit: kingTargetBit);

                BitMovement.Update_AllPieces_And_ColorPieces_Bitboards(allPiecesBitboard: ref board.allPieces,
                                                                       colorPiecesBitboard: ref board.BlackPieces,
                                                                       startBit: rookStartBit,
                                                                       targetBit: rookTargetBit);

                board.CanBlackKingCastle = false;
                board.CanBlackQueenCastle = false;

            }

            board.EnPassantTarget = null;
        }

        private static void MakeEnPassantMove(ulong startBit, ulong targetBit, ref Board board)
        {
            ulong targetEnPassantPawnBit;
            if (board.IsWhiteTurn)
            {
                targetEnPassantPawnBit = targetBit >> 8; // Get bit of the opponent en passant pawn

                // Delete this pawn from board
                BitMovement.DeleteBit(ref board.BlackPawns, targetEnPassantPawnBit);
                BitMovement.DeleteBit(ref board.BlackPieces, targetEnPassantPawnBit);
                BitMovement.DeleteBit(ref board.allPieces, targetEnPassantPawnBit);

                // Move allied pawn and update board AllPieces and WhitePieces
                BitMovement.MoveBit(ref board.WhitePawns, startBit, targetBit);
                BitMovement.Update_AllPieces_And_ColorPieces_Bitboards(ref board.allPieces,
                                                                       ref board.WhitePieces,
                                                                       startBit,
                                                                       targetBit);
            }
            else
            {
                targetEnPassantPawnBit = targetBit << 8; // Get bit of the opponent en passant pawn

                // Delete this pawn from board
                BitMovement.DeleteBit(ref board.WhitePawns, targetEnPassantPawnBit);
                BitMovement.DeleteBit(ref board.WhitePieces, targetEnPassantPawnBit);
                BitMovement.DeleteBit(ref board.allPieces, targetEnPassantPawnBit);

                // Move allied pawn and update board AllPieces and WhitePieces
                BitMovement.MoveBit(ref board.BlackPawns, startBit, targetBit);
                BitMovement.Update_AllPieces_And_ColorPieces_Bitboards(ref board.allPieces,
                                                                       ref board.BlackPieces,
                                                                       startBit,
                                                                       targetBit);
            }
        }

        public void PromotePawn(ulong startBit, ulong targetBit, char chosenPiece, ref Board board)
        {
            if (IsCaptureMove(targetBit: targetBit, whitePieces: board.WhitePieces, blackPieces: board.BlackPieces))
                DeletePieceOnTargetBit(targetBit, ref board);

            if (board.IsWhiteTurn)
            {
                board.WhitePawns ^= startBit;
                board.WhitePieces ^= startBit;
                board.WhitePieces |= targetBit;
                switch (chosenPiece)
                {
                    case 'Q':
                        board.WhiteQueens |= targetBit;
                        break;
                    case 'R':
                        board.WhiteRooks |= targetBit;
                        break;
                    case 'B':
                        board.WhiteBishops |= targetBit;
                        break;
                    case 'N':
                        board.WhiteKnights |= targetBit;
                        break;
                }
            }
            else
            {
                board.BlackPawns ^= startBit;
                board.BlackPieces ^= startBit;
                board.BlackPieces |= targetBit;

                switch (chosenPiece)
                {
                    case 'q':
                        board.BlackQueens |= targetBit;
                        break;
                    case 'r':
                        board.BlackRooks |= targetBit;
                        break;
                    case 'b':
                        board.BlackBishops |= targetBit;
                        break;
                    case 'n':
                        board.BlackKnights |= targetBit;
                        break;
                }
            }

            board.IsWhiteTurn = !board.IsWhiteTurn;
        }

        private static bool IsItMoveForDraw(int startSquare, int targetSquare, Board board)
        {
            ulong startBit = 1UL << startSquare;
            ulong targetBit = 1UL << targetSquare;

            // If it's a pawn move, it's NOT a draw move
            if (board.IsWhiteTurn)
            {
                if ((startBit & board.WhitePawns) != 0) return false;
            }
            else
            {
                if ((startBit & board.BlackPawns) != 0) return false;
            }

            // If it's a capture, it's NOT a draw move
            if ((targetBit & (board.IsWhiteTurn ? board.BlackPieces : board.WhitePieces)) != 0)
                return false;

            // Otherwise, it's a draw move
            return true;

        }

        private static void UpdateDrawConditions(int startSquare, int targetSquare, ref Board board)
        {
            board.ComingMoveCount++;
            if (IsItMoveForDraw(startSquare, targetSquare, board))
                board.DrawMoves++;
            else
                board.DrawMoves = 0;
        }
        private static void BanBlackCastling(ref Board board)
        {
            board.CanBlackKingCastle = false;
            board.CanBlackQueenCastle = false;
        }
        private static void BanWhiteCastling(ref Board board)
        {
            board.CanWhiteKingCastle = false;
            board.CanWhiteQueenCastle = false;
        }

        private static bool IsCaptureMove(ulong targetBit, ulong whitePieces, ulong blackPieces)
        {
            return ((targetBit & whitePieces) != 0) || ((targetBit & blackPieces) != 0);
        }

        private static bool IsItEnPassantMove(ulong startBit, ulong targetBit)
        {
            return startBit << 16 == targetBit || startBit >> 16 == targetBit;
        }

        private static void DeletePieceOnTargetBit(ulong targetBit, ref Board board)
        {
            if (board.IsWhiteTurn)
            {
                if ((board.BlackPawns & targetBit) != 0) BitMovement.DeleteBit(ref board.BlackPawns, targetBit);
                else if ((board.BlackKnights & targetBit) != 0) BitMovement.DeleteBit(ref board.BlackKnights, targetBit);
                else if ((board.BlackBishops & targetBit) != 0) BitMovement.DeleteBit(ref board.BlackBishops, targetBit);
                else if ((board.BlackRooks & targetBit) != 0) BitMovement.DeleteBit(ref board.BlackRooks, targetBit);
                else if ((board.BlackQueens & targetBit) != 0) BitMovement.DeleteBit(ref board.BlackQueens, targetBit);

                BitMovement.DeleteBit(ref board.BlackPieces, targetBit);
            }
            else
            {
                if ((board.WhitePawns & targetBit) != 0) BitMovement.DeleteBit(ref board.WhitePawns, targetBit);
                else if ((board.WhiteKnights & targetBit) != 0) BitMovement.DeleteBit(ref board.WhiteKnights, targetBit);
                else if ((board.WhiteBishops & targetBit) != 0) BitMovement.DeleteBit(ref board.WhiteBishops, targetBit);
                else if ((board.WhiteRooks & targetBit) != 0) BitMovement.DeleteBit(ref board.WhiteRooks, targetBit);
                else if ((board.WhiteQueens & targetBit) != 0) BitMovement.DeleteBit(ref board.WhiteQueens, targetBit);

                BitMovement.DeleteBit(ref board.WhitePieces, targetBit);
            }

            BitMovement.DeleteBit(ref board.allPieces, targetBit);
        }

        private static bool IsItEnPassantMove(ulong startBit, ulong targetBit, Board board)
        {
            if (board.IsWhiteTurn)
            {
                if ((board.WhitePawns & startBit) != 0 && (board.BlackPieces & targetBit) == 0 &&
                     (startBit << 8) != targetBit && (startBit << 16) != targetBit)
                    return true;
            }
            else
            {
                if ((board.BlackPawns & startBit) != 0 && (board.WhitePieces & targetBit) == 0 &&
                     (startBit >> 8) != targetBit && (startBit >> 16) != targetBit)
                    return true;
            }
            return false;
        }
    }
}

// HOW BITBOARDS WORKS


// CHESS BOARD
/*
        a8 b8 c8 d8 e8 f8 g8 h8
        a7 b7 c7 d7 e7 f7 g7 h7
        a6 b6 c6 d6 e6 f6 g6 h6
        a5 b5 c5 d5 e5 f5 g5 h5
        a4 b4 c4 d4 e4 f4 g4 h4
        a3 b3 c3 d3 e3 f3 g3 h3
        a2 b2 c2 d2 e2 f2 g2 h2
        a1 b1 c1 d1 e1 f1 g1 h1
*/

// INDEXES OF THE BOARD
/*
        63 62 61 60 59 58 57 56
        55 54 53 52 51 50 49 48
        47 46 45 44 43 42 41 40
        39 38 37 36 35 34 33 32
        31 30 29 28 27 26 25 24
        23 22 21 20 19 18 17 16
        15 14 13 12 11 10  9  8
         7  6  5  4  3  2  1  0
 */


// Each byte describes one rank of the board.

// Examples for initial position:

//      White Pawns                         Black Pawns
/*
        0 0 0 0  0 0 0 0 : 00               0 0 0 0  0 0 0 0 : 00
        0 0 0 0  0 0 0 0 : 00               1 1 1 1  1 1 1 1 : FF
        0 0 0 0  0 0 0 0 : 00               0 0 0 0  0 0 0 0 : 00
        0 0 0 0  0 0 0 0 : 00               0 0 0 0  0 0 0 0 : 00
        0 0 0 0  0 0 0 0 : 00               0 0 0 0  0 0 0 0 : 00
        0 0 0 0  0 0 0 0 : 00               0 0 0 0  0 0 0 0 : 00
        1 1 1 1  1 1 1 1 : FF               0 0 0 0  0 0 0 0 : 00
        0 0 0 0  0 0 0 0 : 00               0 0 0 0  0 0 0 0 : 00
 */

//      White Knights                       Black Knights
/*
        0 0 0 0  0 0 0 0 : 00               0 1 0 0  0 0 1 0 : 42
        0 0 0 0  0 0 0 0 : 00               0 0 0 0  0 0 0 0 : 00
        0 0 0 0  0 0 0 0 : 00               0 0 0 0  0 0 0 0 : 00
        0 0 0 0  0 0 0 0 : 00               0 0 0 0  0 0 0 0 : 00
        0 0 0 0  0 0 0 0 : 00               0 0 0 0  0 0 0 0 : 00
        0 0 0 0  0 0 0 0 : 00               0 0 0 0  0 0 0 0 : 00
        0 0 0 0  0 0 0 0 : 00               0 0 0 0  0 0 0 0 : 00
        0 1 0 0  0 0 1 0 : 42               0 0 0 0  0 0 0 0 : 00
 */

//      White Bishops                       Black Bishops
/*
        0 0 0 0  0 0 0 0 : 00               0 0 1 0  0 1 0 0 : 24
        0 0 0 0  0 0 0 0 : 00               0 0 0 0  0 0 0 0 : 00
        0 0 0 0  0 0 0 0 : 00               0 0 0 0  0 0 0 0 : 00
        0 0 0 0  0 0 0 0 : 00               0 0 0 0  0 0 0 0 : 00
        0 0 0 0  0 0 0 0 : 00               0 0 0 0  0 0 0 0 : 00
        0 0 0 0  0 0 0 0 : 00               0 0 0 0  0 0 0 0 : 00
        0 0 0 0  0 0 0 0 : 00               0 0 0 0  0 0 0 0 : 00
        0 0 1 0  0 1 0 0 : 24               0 0 0 0  0 0 0 0 : 00
 */

//      White Rooks                         Black Rooks
/*
        0 0 0 0  0 0 0 0 : 00               1 0 0 0  0 0 0 1 : 81
        0 0 0 0  0 0 0 0 : 00               0 0 0 0  0 0 0 0 : 00
        0 0 0 0  0 0 0 0 : 00               0 0 0 0  0 0 0 0 : 00
        0 0 0 0  0 0 0 0 : 00               0 0 0 0  0 0 0 0 : 00
        0 0 0 0  0 0 0 0 : 00               0 0 0 0  0 0 0 0 : 00
        0 0 0 0  0 0 0 0 : 00               0 0 0 0  0 0 0 0 : 00
        0 0 0 0  0 0 0 0 : 00               0 0 0 0  0 0 0 0 : 00
        1 0 0 0  0 0 0 1 : 81               0 0 0 0  0 0 0 0 : 00
 */

//      White Queens                        Black Queens
/*
        0 0 0 0  0 0 0 0 : 00               0 0 0 0  1 0 0 0 : 08
        0 0 0 0  0 0 0 0 : 00               0 0 0 0  0 0 0 0 : 00
        0 0 0 0  0 0 0 0 : 00               0 0 0 0  0 0 0 0 : 00
        0 0 0 0  0 0 0 0 : 00               0 0 0 0  0 0 0 0 : 00
        0 0 0 0  0 0 0 0 : 00               0 0 0 0  0 0 0 0 : 00
        0 0 0 0  0 0 0 0 : 00               0 0 0 0  0 0 0 0 : 00
        0 0 0 0  0 0 0 0 : 00               0 0 0 0  0 0 0 0 : 00
        0 0 0 1  0 0 0 0 : 10               0 0 0 0  0 0 0 0 : 00
 */

    // White Kings                          Black Kings
/*
        0 0 0 0  0 0 0 0 : 00               0 0 0 1  0 0 0 0 : 10
        0 0 0 0  0 0 0 0 : 00               0 0 0 0  0 0 0 0 : 00
        0 0 0 0  0 0 0 0 : 00               0 0 0 0  0 0 0 0 : 00
        0 0 0 0  0 0 0 0 : 00               0 0 0 0  0 0 0 0 : 00
        0 0 0 0  0 0 0 0 : 00               0 0 0 0  0 0 0 0 : 00
        0 0 0 0  0 0 0 0 : 00               0 0 0 0  0 0 0 0 : 00
        0 0 0 0  0 0 0 0 : 00               0 0 0 0  0 0 0 0 : 00
        0 0 0 0  1 0 0 0 : 08               0 0 0 0  0 0 0 0 : 00
 */