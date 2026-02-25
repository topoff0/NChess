using Account.Application.Common.Errors;
using Account.Application.Common.Interfaces;
using Account.Application.Common.Results;
using Account.Application.Features.Auth.Results;
using Account.Application.Logger.Auth;
using Account.Core.Repositories;
using Account.Core.Repositories.Common;
using Account.Core.Security;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Account.Application.Features.Auth.Commands.EmailAuthentication;

public record VerifyEmailCommand(string Email, string VerificationCode)
    : IRequest<ResultT<VerifyEmailResult>>;

public sealed class VerifyEmailCommandHandler(IUserRepository userRepository,
                                               IEmailVerificationCodeRepository codeRepository,
                                               IUnitOfWork unitOfWork,
                                               IVerificationCodeHasher codeHasher,
                                               IJwtTokenService jwtService,
                                               ILogger<VerifyEmailCommandHandler> logger)
    : IRequestHandler<VerifyEmailCommand, ResultT<VerifyEmailResult>>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IEmailVerificationCodeRepository _codeRepository = codeRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IVerificationCodeHasher _codeHasher = codeHasher;
    private readonly IJwtTokenService _jwtService = jwtService;
    private readonly ILogger<VerifyEmailCommandHandler> _logger = logger;

    public async Task<ResultT<VerifyEmailResult>> Handle(VerifyEmailCommand request, CancellationToken token)
    {
        _logger.LogStartVerifyEmail();

        var user = await _userRepository.GetByEmailAsync(request.Email, token);
        if (user is null)
        {
            _logger.LogUserWithSuchEmailNotFound(request.Email);
            return Error.NotFound(ErrorCodes.UserNotFound, ErrorMessages.UserNotFound);
        }

        var codeEntity = await _codeRepository.GetNotExpiredByEmailAsync(request.Email, token);
        if (codeEntity is null)
        {
            _logger.LogVerificationCodeForThisEmailNotFoundVerify(request.Email);
            return Error.NotFound(ErrorCodes.VerificationCodeNotFound, ErrorMessages.VerificationCodeNotFound);
        }

        if (codeEntity.IsUsed)
        {
            _logger.LogVerificationCodeAlreadyUsed(request.VerificationCode, request.Email);
            return Error.Failure(ErrorCodes.VerificationCodeAlreadyUsed, ErrorMessages.VerficaitonCodeAlreadyUsed);
        }

        bool isCodeCorrect = _codeHasher.Verify(request.VerificationCode, codeEntity.HashedCode);
        if (!isCodeCorrect)
        {
            _logger.LogVerificationCodeForThisEmailNotCorrect(request.VerificationCode, request.Email);

            return Error.Validation(new Dictionary<string, string[]>
                {
                    {
                        ErrorCodes.InvalidVerificationCode,
                        new[] { ErrorMessages.InvalidVerificationCode }
                    }
                });
        }

        codeEntity.UseCode();

        await _unitOfWork.SaveChangesAsync(token);

        var jwtToken = _jwtService.GenerateAccessToken(user.Id, user.Email);

        _logger.LogSuccessfulEmailVerification(request.VerificationCode, request.Email);

        return new VerifyEmailResult(jwtToken);
    }
}
