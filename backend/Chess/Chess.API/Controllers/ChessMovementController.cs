using System.Security.Claims;
using Chess.Application.Contracts.Requests;
using Chess.Application.Contracts.Responses;
using Chess.Application.Contracts.Responses.GameProcess;
using Chess.Application.Features.Games.Commands.MakeMove;
using Chess.Application.Features.Games.Commands.PromotePawn;
using Chess.Application.Features.Games.Commands.StartGame;
using Chess.Application.Features.Games.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chess.API.Controllers
{
    [ApiController]
    [Route("api/ChessMovement")]
    public class ChessMovementController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ChessMovementController> _logger;


        public ChessMovementController(IMediator mediator,
                                       ILogger<ChessMovementController> logger)
        {
            _mediator = mediator;
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
                int playerId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                string? playerName = User.FindFirst(ClaimTypes.Name)?.Value;
                MakeMoveCommand command = new(request, playerId, playerName);
                GameCommandResult result = await _mediator.Send(command, token);

                if (!result.IsGameFound)
                {
                    return NotFound("Game was not found");
                }

                return Ok(result.Response);
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
                string? playerName = User.FindFirst(ClaimTypes.Name)?.Value;
                PromotePawnCommand command = new(request, playerId, playerName);
                GameCommandResult result = await _mediator.Send(command, token);

                if (!result.IsGameFound)
                {
                    return NotFound("Game was not found");
                }

                return Ok(result.Response);
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
