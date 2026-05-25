using Account.Application.Common.Results;
using Account.Application.DTOs;
using Account.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Account.Infrastructure.Services;

public sealed class FakeEmailSenderService : IEmailSenderService
{
    private readonly ILogger<FakeEmailSenderService> _logger;

    public Task<Result> SendEmailAsync(SendEmailDto dto, CancellationToken token = default)
    {
        _logger.LogWarning("Fake email sender service used. Recipient: {Recipient}. Subject: {Subject}. Body: {Body}",
                           dto.Recipient,
                           dto.Subject,
                           dto.Body);

        return Task.FromResult(Result.Success);
    }
}
