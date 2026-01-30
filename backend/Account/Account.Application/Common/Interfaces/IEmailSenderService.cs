using Account.Application.DTOs.Requests.EmailSender;
using Account.Application.DTOs.Results.Common;

namespace Account.Application.Common.Interfaces;

public interface IEmailSenderService
{
    Task<Result> SendEmailAsync(SendEmailDto dto, CancellationToken token);
}
