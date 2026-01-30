using System.Security.Claims;
using Chess.API.Interfaces;
using Chess.Data;
using Chess.DTO.Requests;
using Chess.DTO.Responses;
using Chess.DTO.Responses.GameProcess;
using Chess.Main.Core.FEN;
using Chess.Main.Models;
using Chess.Main.Search;
using Chess.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Chess.API.Controllers
{
    [ApiController]
    [Route("api/ChessMovement")]
    public class ChessMovementController : ControllerBase
    {
        private readonly IMovement _movement;
        private readonly GamesDbContext _db;
        private readonly ILogger<ChessMovementController> _logger;


        public ChessMovementController(IMovement movementAPI, GamesDbContext db, ILogger<ChessMovementController> logger)
        {
            _movement = movementAPI;
            _db = db;
            _logger = logger;
        }

        [HttpPost("OnGameStart")]
        [Authorize]
        public async Task<IActionResult> OnGameStart([FromBody] GameStartRequest request)
        {
            try
            {
                int firstPlayerId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                GameInfo? activeGame = await _db.Games.FirstOrDefaultAsync(x => x.FirstPlayerId == firstPlayerId && x.IsActiveGame);

                if (activeGame != null)
                {
                    string lastFenOfActiveGame = activeGame.Fens[^1];
                    var legalMoves_ = _movement.GetLegalMoves(lastFenOfActiveGame);

                    GameResponse existGameResponse = new(
                    isSuccess: true, message: "You are already playing", fen: lastFenOfActiveGame,
                    legalMoves: legalMoves_, moveNotations: activeGame.Moves, isGameEnded: false,
                    winner: null);

                    return Ok(existGameResponse);
                }

                string fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

                GameInfo newGame = new()
                {
                    Fens = [fen],
                    Moves = [],
                    IsActiveGame = true,
                    FirstPlayerId = firstPlayerId,
                    SecondPlayerId = 0
                };
                _db.Games.Add(newGame);
                await _db.SaveChangesAsync();

                List<string> moveNotations = [];

                if (!request.IsPlayerPlayWhite) // player plays black
                {
                    var legalComputerMoves = _movement.GetLegalMoves(fen);
                    Board board = FenUtility.LoadBoardFromFen(fen);
                    var moveValues = SearchAlgorithm.Search(legalComputerMoves, board);

                    MoveRequest computerMoveRequest = new()
                    {
                        StartSquare = moveValues.StartSquare,
                        TargetSquare = moveValues.TargetSquare,
                        FenBeforeMove = fen
                    };

                    OnMoveResponse moveResponse = await _movement.HandleMove(computerMoveRequest, 0);
                    fen = moveResponse.Fen;
                }


                var legalMoves = _movement.GetLegalMoves(fen);

                GameResponse response = new(
                    isSuccess: true, message: "Game successfully started", fen: fen,
                    legalMoves: legalMoves, moveNotations, isGameEnded: false, winner: null);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message);
                BaseResponse response = new(
                    isSuccess: false, "Something went wrong while trying to create the game");
                return BadRequest(response);
            }
        }

        [HttpPost("MakeMove")]
        [Authorize]
        public async Task<IActionResult> MakeMove([FromBody] MoveRequest request)
        {
            try
            {
                // Get player that playing current game from jwt token
                int playerId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                // Make player move
                OnMoveResponse moveResponse = await _movement.HandleMove(request, playerId);

                // * Make computer move
                var legalComputerMoves = _movement.GetLegalMoves(moveResponse.Fen);
                Board board = FenUtility.LoadBoardFromFen(moveResponse.Fen);
                IMovement.GameCondition? gameCondition = _movement.GetGameCondition(board, legalComputerMoves);
                if (gameCondition.HasValue)
                {
                    GameInfo? endedGame = await _db.Games.FirstOrDefaultAsync(g => g.FirstPlayerId == playerId);
                    if (endedGame != null)
                    {
                        endedGame.IsActiveGame = false;
                        await _db.SaveChangesAsync();
                        GameResponse endGameResponse = new(
                                isSuccess: true, message: "Game ended", fen: moveResponse.Fen,
                                legalMoves: null, moveNotations: moveResponse.MoveNotations, isGameEnded: true,
                                winner: gameCondition.Value == IMovement.GameCondition.LOSE ? User.FindFirst(ClaimTypes.Name)?.Value : "DRAW"
                            );

                        return Ok(endGameResponse);
                    }
                    return NotFound("Game was not found");
                }

                var moveValues = SearchAlgorithm.Search(legalComputerMoves, board);

                if (moveValues.IsItPromotionPawnMove && moveValues.PromotionPiece.HasValue)
                {
                    PawnPromotionRequest promotionRequest = new()
                    {
                        StartSquare = moveValues.StartSquare,
                        TargetSquare = moveValues.TargetSquare,
                        FenBeforeMove = moveResponse.Fen,
                        ChosenPiece = moveValues.PromotionPiece.Value
                    };
                    Console.WriteLine(moveValues.PromotionPiece);
                    moveResponse = await _movement.HandlePawnPromotion(promotionRequest, playerId);
                }
                else
                {
                    MoveRequest computerMoveRequest = new()
                    {
                        StartSquare = moveValues.StartSquare,
                        TargetSquare = moveValues.TargetSquare,
                        FenBeforeMove = moveResponse.Fen
                    };
                    // Update moveResponse after computer move
                    moveResponse = await _movement.HandleMove(computerMoveRequest, 0);
                }

                var legalMoves = _movement.GetLegalMoves(moveResponse.Fen);
                board = FenUtility.LoadBoardFromFen(moveResponse.Fen); // Get updated board state
                gameCondition = _movement.GetGameCondition(board, legalMoves);

                GameResponse response;

                if (gameCondition.HasValue)
                {
                    GameInfo? endedGame = await _db.Games.FirstOrDefaultAsync(g => g.FirstPlayerId == playerId);
                    if (endedGame != null)
                    {
                        endedGame.IsActiveGame = false;
                        await _db.SaveChangesAsync();
                        response = new(
                            isSuccess: true, message: "Game ended", fen: moveResponse.Fen,
                            legalMoves: legalMoves, moveNotations: moveResponse.MoveNotations, isGameEnded: true,
                            winner: gameCondition.Value == IMovement.GameCondition.DRAW ? "DRAW" : "Computer");

                        return Ok(response);
                    }
                    return NotFound("Game was not found");
                }

                response = new(
                            isSuccess: true, message: "Successful move", fen: moveResponse.Fen,
                            legalMoves: legalMoves, moveNotations: moveResponse.MoveNotations, isGameEnded: false,
                            winner: null
                        );

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception was occurred while trying to make move: {ex.Message}");
                GameResponse badResponse = new(isSuccess: false,
                        message: "An error occurred on your move", fen: request.FenBeforeMove,
                        legalMoves: null, moveNotations: null, isGameEnded: false, winner: null
                    );
                return BadRequest(badResponse);
            }
        }

        [HttpPost("PromotePawn")]
        [Authorize]
        public async Task<IActionResult> PromotePawn([FromBody] PawnPromotionRequest request)
        {
            try
            {
                int playerId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                OnMoveResponse promoteResponse = await _movement.HandlePawnPromotion(request, playerId);

                // * Make computer move
                var legalComputerMoves = _movement.GetLegalMoves(promoteResponse.Fen);
                Board board = FenUtility.LoadBoardFromFen(promoteResponse.Fen);
                IMovement.GameCondition? gameCondition = _movement.GetGameCondition(board, legalComputerMoves);
                if (gameCondition.HasValue)
                {
                    GameInfo? endedGame = await _db.Games.FirstOrDefaultAsync(g => g.FirstPlayerId == playerId);
                    if (endedGame != null)
                    {
                        endedGame.IsActiveGame = false;
                        await _db.SaveChangesAsync();
                        GameResponse endGameResponse = new(
                                isSuccess: true, message: "Game ended", fen: promoteResponse.Fen,
                                legalMoves: null, moveNotations: promoteResponse.MoveNotations, isGameEnded: true,
                                winner: gameCondition.Value == IMovement.GameCondition.LOSE ? User.FindFirst(ClaimTypes.Name)?.Value : "DRAW"
                            );

                        return Ok(endGameResponse);
                    }
                    return NotFound("Game was not found");
                }

                var moveValues = SearchAlgorithm.Search(legalComputerMoves, board);

                MoveRequest computerMoveRequest = new()
                {
                    StartSquare = moveValues.StartSquare,
                    TargetSquare = moveValues.TargetSquare,
                    FenBeforeMove = promoteResponse.Fen
                };
                // Update promoteResponse after computer move
                promoteResponse = await _movement.HandleMove(computerMoveRequest, 0);

                var legalMoves = _movement.GetLegalMoves(promoteResponse.Fen);

                gameCondition = _movement.GetGameCondition(board, legalMoves);

                GameResponse response;

                if (gameCondition.HasValue)
                {
                    GameInfo? endedGame = await _db.Games.FirstOrDefaultAsync(g => g.FirstPlayerId == playerId && g.IsActiveGame == true);
                    if (endedGame != null)
                    {
                        endedGame.IsActiveGame = false;
                        await _db.SaveChangesAsync();
                        response = new(
                            isSuccess: true, message: "Successful move", fen: promoteResponse.Fen,
                            legalMoves: legalMoves, moveNotations: promoteResponse.MoveNotations, isGameEnded: true,
                            winner: gameCondition == IMovement.GameCondition.DRAW ? "DRAW" : "Computer");

                        return Ok(response);
                    }
                    return NotFound("Game was not found");
                }

                response = new(
                            isSuccess: true, message: "Successful move", fen: promoteResponse.Fen,
                            legalMoves: legalMoves, moveNotations: promoteResponse.MoveNotations, isGameEnded: false,
                            winner: null
                        );

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception in PromotePawn: {ex.Message}");
                GameResponse response = new(isSuccess: false, message: "Something went wrong while processing this move",
                    fen: null, legalMoves: null, null, false, null);
                return BadRequest(response);
            }

        } 
    }
}