using System.Security.Claims;
using Account.API.DTOs;
using Account.Application.Features.Auth.Commands.CreateProfile;
using Account.Application.Features.Auth.Commands.EmailAuthentication;
using Account.Application.Features.Auth.Queries.GetCurrentUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Account.API.Controllers;

[ApiController]
[Route("api/Account")]
public class AccountController(IMediator mediator)
    : ControllerBase
{
    // CONSTANTS
    private const string UserIdClaimType = "userId";


    private readonly IMediator _mediator = mediator;

    [HttpGet("health")]
    public async Task<IActionResult> CheckHealth()
    {
        return Ok(new { status = "health", timestamp = DateTime.UtcNow });
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser(CancellationToken token)
    {
        var userIdValue = User.FindFirstValue(UserIdClaimType);

        if (!Guid.TryParse(userIdValue, out var userId))
        {
            return Unauthorized();
        }

        var query = new GetCurrentUserQuery(userId);
        var result = await _mediator.Send(query, token);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpPost("start-email-auth")]
    public async Task<IActionResult> StartEmailAuthentication(StartEmailAuthDto dto,
                                                              CancellationToken token)
    {
        var command = new StartEmailAuthCommand(dto.Email);
        var result = await _mediator.Send(command, token);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail(VerifyEmailDto dto,
                                                 CancellationToken token)
    {
        var command = new VerifyEmailCommand(dto.Email, dto.VerificationCode);
        var result = await _mediator.Send(command, token);

        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [Authorize]
    [HttpPost("create-profile")]
    public async Task<IActionResult> CreateProfile([FromForm] CreateProfileDto dto,
                                                   CancellationToken token)
    {
        byte[]? imageBytes = null;
        if (dto.ProfileImage is not null)
        {
            using var ms = new MemoryStream();
            await dto.ProfileImage.CopyToAsync(ms, token);
            imageBytes = ms.ToArray();
        }

        var email = User.FindFirstValue(ClaimTypes.Email);
        if (email is null)
            return Unauthorized();

        var command = new CreateProfileCommand(imageBytes,
                                               email,
                                               dto.Username);
        var result = await _mediator.Send(command, token);

        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

}
