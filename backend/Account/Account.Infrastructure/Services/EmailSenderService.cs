using System.Net;
using System.Net.Mail;
using Account.Application.Common.Configurations;
using Account.Application.Common.Interfaces;
using Account.Application.Common.Results;
using Account.Application.DTOs;
using Microsoft.Extensions.Options;
using SmtpClient = System.Net.Mail.SmtpClient;

namespace Account.Infrastructure.Services;

public class EmailSenderService(IOptions<EmailOptions> emailOptions) : IEmailSenderService
{
    private readonly EmailOptions _emailOptions = emailOptions.Value;
    private const string _displayName = "Pixel Chess";

    public async Task<Result> SendEmailAsync(SendEmailDto dto, CancellationToken token = default)
    {
        MailMessage mailMessage = new()
        {
            From = new MailAddress(_emailOptions.Email, _displayName),
            Subject = dto.Subject,
            IsBodyHtml = true,
            Body = dto.Body
        };

        mailMessage.To.Add(dto.Recipient);

        using var smtpClient = new SmtpClient();
        smtpClient.Host = _emailOptions.Host;
        smtpClient.Port = _emailOptions.Port;
        smtpClient.Credentials = new NetworkCredential(
            _emailOptions.Email, _emailOptions.Password);
        smtpClient.EnableSsl = true;

        await smtpClient.SendMailAsync(mailMessage, token);

        return Result.Success();
    }
}
