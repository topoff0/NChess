using Account.Application.Common.Interfaces;
using Account.Application.DTOs.Errors;
using Account.Application.DTOs.Results.Common;
using Account.Application.Features.Auth.DTOs.Requests;
using Account.Application.Features.Auth.DTOs.Results;
using Account.Core.Entities;
using Account.Core.Repositories;
using Account.Core.Repositories.Common;
using MediatR;

namespace Account.Application.Features.Auth.Commands.EmailRegistration;

public record StartEmailAuthCommand(StartEmailRegistrationDto Dto)
    : IRequest<ResultT<IsUserExistsResult>>;

public sealed class StartEmailAuthCommandHandler(IUserRepository userRepository,
                                                 IEmailVerificationCodeRepository codeRepository,
                                                 IUnitOfWork unitOfWork,
                                                 IEmailServiceSender emailService)
    : IRequestHandler<StartEmailAuthCommand, ResultT<IsUserExistsResult>>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IEmailVerificationCodeRepository _codeRepository = codeRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IEmailServiceSender _emailService = emailService;

    public async Task<ResultT<IsUserExistsResult>> Handle(StartEmailAuthCommand request, CancellationToken token)
    {
        if (string.IsNullOrEmpty(request.Dto.Email)) //TODO: Implement real email validation
            return Error.Validation("email.invalid", "Email format is not valid");

        bool isPlayerExists = await _userRepository.IsExistsByEmail(request.Dto.Email, token);
        if (!isPlayerExists)
        {
            string code = GenerateCode();

            await _emailService.SendVerificationCodeAsync(request.Dto.Email, code, token);
            await _codeRepository.AddAsync(EmailVerificationCode.Create(request.Dto.Email, code), token);
            await _userRepository.AddAsync(User.CreatePending(request.Dto.Email, AuthProvider.Email), token);

            await _unitOfWork.SaveChangesAsync(token);

            return new IsUserExistsResult(IsExists: false);
        }

        return new IsUserExistsResult(IsExists: true);
    }

    private static string GenerateCode()
    {
        Random rand = new();
        return rand.Next(100000, 999999).ToString();
    }
}

