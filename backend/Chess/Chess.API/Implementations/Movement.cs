using Chess.API.Interfaces;
using Chess.Application.Contracts.Requests;
using Chess.Application.Contracts.Responses.GameProcess;
using Chess.Application.Features.Games.Common;
using Chess.Application.Interfaces;
using Chess.Core.Entities;
using Chess.Core.FEN;
using Chess.Core.Helpers.BitOperation;
using Chess.Core.Helpers.Squares;
using Chess.Core.Movement.Generator;
using Chess.Core.Models;
using Chess.Core.MoveNotation;
using Chess.Core.Repositories;
using Chess.Core.Repositories.Common;


namespace Chess.API.Implementations
{
    public class Movement(IGameRepository gameRepository, IUnitOfWork unitOfWork)
        : IMovement, IChessMovementService
    {
        private readonly IGameRepository _gameRepository = gameRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public Dictionary<int, List<int>> GetLegalMoves(string fen)
        {
            Dictionary<int, List<int>> legalMoves = [];

            Board board = FenUtility.LoadBoardFromFen(fen);

            Dictionary<char, ulong> piecesCollections = GetAlliedPieces(board);

            for (int square = 0; square < 64; square++)
                {
                    foreach (ulong pieces in piecesCollections.Values)
                    {
                        if ((pieces & (1UL << square)) != 0)
                        {
                            char pieceSymbol = piecesCollections.FirstOrDefault(x => (x.Value & (1UL << square)) != 0).Key;

                            ulong rawMoves = GetLegalMovesByPieceSymbol(square, pieceSymbol, board);

                            List<int> validMoves = [];
                            foreach (int targetSquare in BitHelper.SquareIndexesFromBitboard(rawMoves))
                            {
                                Board tempBoard = FenUtility.LoadBoardFromFen(fen);
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
            Dictionary<char, ulong> pieces = [];

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
                'K' => KingMovement.Generate(squareIndex, board, board.GetIsWhiteTurn()),
                'p' => PawnMovement.BlackGenerate(squareIndex, board),
                'n' => KnightMovement.Generate(squareIndex, board, board.GetIsWhiteTurn()),
                'b' => BishopMovement.Generate(squareIndex, board, board.GetIsWhiteTurn()),
                'r' => RookMovement.Generate(squareIndex, board, board.GetIsWhiteTurn()),
                'q' => QueenMovement.Generate(squareIndex, board, board.GetIsWhiteTurn()),
                'k' => KingMovement.Generate(squareIndex, board),
                _ => 0,
            };
        }

        public async Task<OnMoveResponse> HandleMove(MoveRequest request, int playerId, CancellationToken token)
        {
            Board board = FenUtility.LoadBoardFromFen(request.FenBeforeMove);
            char movingPieceSymbol = SquaresHelper.GetPieceSymbolFromSquare(board, request.StartSquare).GetValueOrDefault();

            // For move notation (must be checked before the move)
            bool isCaptureMove = SquaresHelper.IsPieceOnSquare(board, request.TargetSquare);

            // Make move (update board)
            board.MakeRegularMove(request.StartSquare, request.TargetSquare, ref board);

            // Find current game in db
            GameInfo? game = await _gameRepository.GetActiveByPlayerIdAsync(playerId, token);
            if (game == null)
                return new OnMoveResponse(null, null); // ? Maybe change this response

            // Generate FEN from updated board
            string fenAfterMove = FenUtility.GenerateFenFromBoard(board);

            // Generate move notation
            GenerateMoveNotationRequest moveNotationRequest = new(request.StartSquare,
                                                                  request.TargetSquare,
                                                                  board,
                                                                  isCaptureMove,
                                                                  movingPieceSymbol);
            string moveNotation = MoveNotation.Generate(moveNotationRequest);

            // Update game info in database
            game.Fens.Add(fenAfterMove);
            game.Moves.Add(moveNotation);
            await _unitOfWork.SaveChangesAsync(token);

            List<string> moveNotations = game.Moves;
            var response = new OnMoveResponse(fenAfterMove, moveNotations);

            return response;
        }

        public GameCondition? GetGameCondition(Board board, Dictionary<int, List<int>> legalMoves)
        {
            if (legalMoves.Count == 0)
            {
                if (KingMovement.IsKingUnderAttack(board)) // here board after move
                {
                    return GameCondition.Lose;
                }
                return GameCondition.Draw;
            }

            return null;
        }

        public async Task<OnMoveResponse> HandlePawnPromotion(PawnPromotionRequest request, int playerId, CancellationToken token)
        {
            GameInfo? game = await _gameRepository.GetActiveByPlayerIdAsync(playerId, token);
            if (game == null)
                return new OnMoveResponse(null, null); // ? Maybe change this response

            Board board = FenUtility.LoadBoardFromFen(request.FenBeforeMove);

            bool isCaptureMove = SquaresHelper.IsPieceOnSquare(board, request.TargetSquare);

            // Make promotion move (update board)
            ulong startBit = 1Ul << request.StartSquare;
            ulong targetBit = 1UL << request.TargetSquare;
            board.PromotePawn(startBit, targetBit, request.ChosenPiece, ref board);

            // Generate FEN from updated board
            string fenAfterMove = FenUtility.GenerateFenFromBoard(board);

            GenerateMoveNotationRequest moveNotationRequest = new(request.StartSquare,
                                                                  request.TargetSquare,
                                                                  board,
                                                                  isCaptureMove,
                                                                  null,
                                                                  request.ChosenPiece,
                                                                  isItPromotionPawnMove: true
                                                                  );
            string moveNotation = MoveNotation.Generate(moveNotationRequest);

            // Update game info in database
            game.Fens.Add(fenAfterMove);
            game.Moves.Add(moveNotation.ToString());
            await _unitOfWork.SaveChangesAsync(token);

            return new OnMoveResponse(fenAfterMove, game.Moves);
        }

        public Task<OnMoveResponse> OnMove(MoveRequest request, int playerId)
        {
            throw new NotImplementedException();
        }
    }
}
