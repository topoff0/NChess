using Account.Application.DTOs.Errors;
using Account.Application.DTOs.Results.Common;
using Account.Application.Features.Auth.DTOs;
using Account.Core.Repositories;
using Account.Core.Repositories.Common;
using Account.Core.Security;
using MediatR;

namespace Account.Application.Features.Auth.Commands.EmailRegistration;

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
        if (string.IsNullOrEmpty(request.Email))
            return Error.Validation("email.invalid", "Incorrect email format");

        var user = await _userRepository.GetByEmailAsync(request.Email, token);
        if (user is null)
            return Error.NotFound("user.NotFound", "User is not found");

        var codeEntity = await _codeRepository.GetActiveByEmailAsync(request.Email, token);
        if (codeEntity is null)
            return Error.NotFound("verificationCode.notFound", "Verification code is not found.");

        bool isCodeCorrect = _codeHasher.Verify(request.VerificationCode, codeEntity.HashedCode);
        if(!isCodeCorrect)
            return Error.Failure("verificationCode.Incorrect", "Incorrect verificationCode");

        codeEntity.UseCode();

        await _unitOfWork.SaveChangesAsync(token);
        return ResultT<VerifyEmailResult>.Success(new VerifyEmailResult(true)); 
    }
}
