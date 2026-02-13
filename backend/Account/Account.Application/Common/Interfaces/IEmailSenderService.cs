using Account.Application.Common.DTOs;
using Account.Application.Common.Results;

namespace Account.Application.Common.Interfaces;

public interface IEmailSenderService
{
    Task<Result> SendEmailAsync(SendEmailDto dto, CancellationToken token = default);
}
