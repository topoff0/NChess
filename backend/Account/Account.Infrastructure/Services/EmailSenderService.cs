using System.Net;
using System.Net.Mail;
using Account.Application.Common.Configurations;
using Account.Application.Common.Interfaces;
using Account.Application.Common.Results;
using Microsoft.Extensions.Options;
using SmtpClient = System.Net.Mail.SmtpClient;

namespace Account.Infrastructure.Services;

public class EmailSenderService(IOptions<EmailOptions> emailOptions) : IEmailSenderService
{
    private readonly EmailOptions _emailOptions = emailOptions.Value;

    public async Task<Result> SendEmailAsync(string recipient, string subject, string body, CancellationToken token = default)
    {
        //TODO: Add try-catch with logger
        MailMessage mailMessage = new()
        {
            From = new MailAddress(_emailOptions.Email, "Pixel Chess"),
            Subject = subject,
            IsBodyHtml = true,
            Body = body
        };

        mailMessage.To.Add(recipient);

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
