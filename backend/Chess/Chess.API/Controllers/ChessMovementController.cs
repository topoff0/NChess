using System.Security.Claims;
using Chess.API.Interfaces;
using Chess.Application.Contracts.Requests;
using Chess.Application.Contracts.Responses;
using Chess.Application.Contracts.Responses.GameProcess;
using Chess.Application.Features.Games.Commands.StartGame;
using Chess.Core.Entities;
using Chess.Core.FEN;
using Chess.Core.Models;
using Chess.Core.Repositories;
using Chess.Core.Repositories.Common;
using Chess.Core.Search;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chess.API.Controllers
{
    [ApiController]
    [Route("api/ChessMovement")]
    public class ChessMovementController : ControllerBase
    {
        private readonly IMovement _movement;
        private readonly IMediator _mediator;
        private readonly IGameRepository _gameRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ChessMovementController> _logger;


        public ChessMovementController(IMovement movementAPI,
                                       IMediator mediator,
                                       IGameRepository gameRepository,
                                       IUnitOfWork unitOfWork,
                                       ILogger<ChessMovementController> logger)
        {
            _movement = movementAPI;
            _mediator = mediator;
            _gameRepository = gameRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpPost("OnGameStart")]
        [Authorize]
        public async Task<IActionResult> OnGameStart([FromBody] GameStartRequest request,
                                                     CancellationToken token)
        {
            try
            {
                int firstPlayerId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                StartGameCommand command = new(firstPlayerId, request.IsPlayerPlayWhite);
                GameResponse response = await _mediator.Send(command, token);
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
        public async Task<IActionResult> MakeMove([FromBody] MoveRequest request,
                                                  CancellationToken token)
        {
            try
            {
                // Get player that playing current game from jwt token
                int playerId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                // Make player move
                OnMoveResponse moveResponse = await _movement.HandleMove(request, playerId, token);

                // * Make computer move
                var legalComputerMoves = _movement.GetLegalMoves(moveResponse.Fen);
                Board board = FenUtility.LoadBoardFromFen(moveResponse.Fen);
                IMovement.GameCondition? gameCondition = _movement.GetGameCondition(board, legalComputerMoves);
                if (gameCondition.HasValue)
                {
                    GameInfo? endedGame = await _gameRepository.GetByFirstPlayerIdAsync(playerId, token);
                    if (endedGame != null)
                    {
                        endedGame.IsActiveGame = false;
                        await _unitOfWork.SaveChangesAsync(token);
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
                    moveResponse = await _movement.HandlePawnPromotion(promotionRequest, playerId, token);
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
                    moveResponse = await _movement.HandleMove(computerMoveRequest, 0, token);
                }

                var legalMoves = _movement.GetLegalMoves(moveResponse.Fen);
                board = FenUtility.LoadBoardFromFen(moveResponse.Fen); // Get updated board state
                gameCondition = _movement.GetGameCondition(board, legalMoves);

                GameResponse response;

                if (gameCondition.HasValue)
                {
                    GameInfo? endedGame = await _gameRepository.GetByFirstPlayerIdAsync(playerId, token);
                    if (endedGame != null)
                    {
                        endedGame.IsActiveGame = false;
                        await _unitOfWork.SaveChangesAsync(token);
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
        public async Task<IActionResult> PromotePawn([FromBody] PawnPromotionRequest request,
                                                     CancellationToken token)
        {
            try
            {
                int playerId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                OnMoveResponse promoteResponse = await _movement.HandlePawnPromotion(request, playerId, token);

                // * Make computer move
                var legalComputerMoves = _movement.GetLegalMoves(promoteResponse.Fen);
                Board board = FenUtility.LoadBoardFromFen(promoteResponse.Fen);
                IMovement.GameCondition? gameCondition = _movement.GetGameCondition(board, legalComputerMoves);
                if (gameCondition.HasValue)
                {
                    GameInfo? endedGame = await _gameRepository.GetByFirstPlayerIdAsync(playerId, token);
                    if (endedGame != null)
                    {
                        endedGame.IsActiveGame = false;
                        await _unitOfWork.SaveChangesAsync(token);
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
                promoteResponse = await _movement.HandleMove(computerMoveRequest, 0, token);

                var legalMoves = _movement.GetLegalMoves(promoteResponse.Fen);

                gameCondition = _movement.GetGameCondition(board, legalMoves);

                GameResponse response;

                if (gameCondition.HasValue)
                {
                    GameInfo? endedGame = await _gameRepository.GetActiveByFirstPlayerIdAsync(playerId, token);
                    if (endedGame != null)
                    {
                        endedGame.IsActiveGame = false;
                        await _unitOfWork.SaveChangesAsync(token);
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
