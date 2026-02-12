using Account.Application.Commonon.Errors;
using Account.Application.Common.Results;
using Account.Application.DTOs.Errors;
using Account.Application.Features.Auth.DTOs.Requests;
using Account.Application.Features.Auth.Results;
using Account.Core.Repositories;
using Account.Core.Repositories.Common;
using Account.Core.Security;
using MediatR;

namespace Account.Application.Features.Auth.Commands.EmailRegistration;

public record VerifyEmailCommand(VerifyEmailDto Dto)
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
        if (string.IsNullOrEmpty(request.Dto.Email)) //TODO: Implement real email validation
            return Error.Validation(ErrorCodes.InvalidEmail, ErrorMessages.InvalidEmail);

        var user = await _userRepository.GetByEmailAsync(request.Dto.Email, token);
        if (user is null)
            return Error.NotFound(ErrorCodes.UserNotFound, ErrorMessages.UserNotFound);

        var codeEntity = await _codeRepository.GetActiveByEmailAsync(request.Dto.Email, token);
        if (codeEntity is null)
            return Error.NotFound(ErrorCodes.VerificationCodeNotFound, ErrorMessages.VerificationCodeNotFound);

        bool isCodeCorrect = _codeHasher.Verify(request.Dto.VerificationCode, codeEntity.HashedCode);
        if(!isCodeCorrect)
            return new VerifyEmailResult(IsCodeCorrect: false); 

        codeEntity.UseCode();

        await _unitOfWork.SaveChangesAsync(token);

        //TODO: return JWT token

        return new VerifyEmailResult(IsCodeCorrect: true); 
    }
}
