using Account.Application.Common.Errors;
using Account.Application.Common.Results;
using Account.Application.Features.Auth.Results;
using Account.Core.Repositories;
using Account.Core.Repositories.Common;
using Account.Core.Security;
using MediatR;

namespace Account.Application.Features.Auth.Commands.EmailAuthentication;

public record VerifyEmailCommand(string Email, string VerificationCode)
    : IRequest<ResultT<VerifyEmailResult>>;

public sealed class VerifyEmailCommandHandler(IUserRepository userRepository,
                                               IEmailVerificationCodeRepository codeRepository,
                                               IUnitOfWork unitOfWork,
                                               IVerificationCodeHasher codeHasher)
    : IRequestHandler<VerifyEmailCommand, ResultT<VerifyEmailResult>>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IEmailVerificationCodeRepository _codeRepository = codeRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IVerificationCodeHasher _codeHasher = codeHasher;

    public async Task<ResultT<VerifyEmailResult>> Handle(VerifyEmailCommand request, CancellationToken token)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, token);
        if (user is null)
            return Error.NotFound(ErrorCodes.UserNotFound, ErrorMessages.UserNotFound);

        var codeEntity = await _codeRepository.GetNotExpiredByEmailAsync(request.Email, token);
        if (codeEntity is null)
            return Error.NotFound(ErrorCodes.VerificationCodeNotFound, ErrorMessages.VerificationCodeNotFound);

        if (codeEntity.IsUsed)
            return Error.Failure(ErrorCodes.VerificationCodeAlreadyUsed, ErrorMessages.VerficaitonCodeAlreadyUsed);

        bool isCodeCorrect = _codeHasher.Verify(request.VerificationCode, codeEntity.HashedCode);
        if (!isCodeCorrect)
            return Error.Validation(new Dictionary<string, string[]>
                {
                    {
                        ErrorCodes.InvalidVerificationCode,
                        new[] { ErrorMessages.InvalidVerificationCode }
                    }
                });

        codeEntity.UseCode();

        await _unitOfWork.SaveChangesAsync(token);

        //TODO: return JWT token

        return new VerifyEmailResult(IsCodeCorrect: true);
    }
}
