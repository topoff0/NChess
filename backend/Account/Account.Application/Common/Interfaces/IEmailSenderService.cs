using Account.Application.Common.Results;

namespace Account.Application.Common.Interfaces;

public interface IEmailSenderService
{
    Task<Result> SendEmailAsync(string recipient, string subject, string body, CancellationToken token = default);
}
