using Account.Application.Common.Interfaces;
using Account.Application.DTOs.Requests.EmailSender;
using Account.Application.Features.Auth.Commands.CreateProfile;
using Account.Application.Features.Auth.Commands.EmailRegistration;
using Account.Application.Features.Auth.Commands.Login;
using Account.Application.Features.Auth.DTOs.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace Account.API.Controllers;

[ApiController]
[Route("api/Account")]
public class AccountController(IEmailSenderService emailSenderService,
                               IMediator mediator)
    : ControllerBase
{
    private readonly IMediator _mediator = mediator;
    private readonly IEmailSenderService _emailSenderService = emailSenderService;

    // [HttpPost("test")]
    // public async Task<IActionResult> SendEmail(SendEmailDto dto, CancellationToken token)
    // {
    //     var result = await _emailSenderService.SendEmailAsync(dto, token);
    //
    //     if (!result.IsSuccess)
    //         return BadRequest();
    //
    //     return Ok();
    // }

    [HttpPost("start-email-auth")]
    public async Task<IActionResult> StartEmailAuthentication([FromBody] StartEmailAuthDto dto,
                                                            CancellationToken token)
    {
        var command = new StartEmailAuthCommand(dto);
        var result = await _mediator.Send(command, token);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailDto dto,
                                                 CancellationToken token)
    {
        var command = new VerifyEmailCommand(dto);
        var result = await _mediator.Send(command, token);

        if(!result.IsSuccess)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto,
                                           CancellationToken token)
    {
        var command = new LoginCommand(dto);
        var result = await _mediator.Send(command, token);

        if(!result.IsSuccess)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpPost("create-profile")]
    public async Task<IActionResult> CreateProfile([FromBody] CreateProfileDto dto,
                                                   CancellationToken token)
    {
        var command = new CreateProfileCommand(dto);
        var result = await _mediator.Send(command, token);

        if(!result.IsSuccess)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

}
