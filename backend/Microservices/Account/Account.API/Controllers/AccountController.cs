using Account.Application.Common.Interfaces;
using Account.Application.DTOs.Requests.EmailSender;
using Microsoft.AspNetCore.Mvc;


namespace Account.API.Controllers;

[ApiController]
[Route("api/Account")]
public class AccountController(IEmailSenderService emailSenderService)
    : ControllerBase
{
    private readonly IEmailSenderService _emailSenderService = emailSenderService;
    [HttpPost("test")]
    public async Task<IActionResult> SendEmail(SendEmailDto dto, CancellationToken token)
    {
        var result = await _emailSenderService.SendEmailAsync(dto, token);

        if (!result.IsSuccess)
            return BadRequest();

        return Ok();
    }

}
