using Account.API.DTOs;
using Account.Application.Features.Auth.Commands.CreateProfile;
using Account.Application.Features.Auth.Commands.EmailRegistration;
using Account.Application.Features.Auth.Commands.Login;
using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace Account.API.Controllers;

[ApiController]
[Route("api/Account")]
public class AccountController(IMediator mediator)
    : ControllerBase
{
    private readonly IMediator _mediator = mediator;


    [HttpGet("health")]
    public async Task<IActionResult> CheckHealth()
    {
        return Ok(new { status="health", timestamp = DateTime.UtcNow });
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

        if(!result.IsSuccess)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto,
                                           CancellationToken token)
    {
        var command = new LoginCommand(dto.Email, dto.Password);
        var result = await _mediator.Send(command, token);

        if(!result.IsSuccess)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

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

        var command = new CreateProfileCommand(imageBytes,
                                               dto.Email,
                                               dto.Username,
                                               dto.Password,
                                               dto.ConfirmPassword);
        var result = await _mediator.Send(command, token);

        if(!result.IsSuccess)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

}
