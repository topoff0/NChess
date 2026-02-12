using Account.Application.Common.Errors;
using Account.Application.Common.Errors;
using Account.Application.Common.Results;
using Account.Application.DTOs.Errors;
using Account.Application.DTOs.Requests.EmailSender;
using Account.Application.Features.Auth.DTOs.Requests;
using Account.Application.Features.Auth.Results;
using Account.Core.Entities;
using Account.Core.Repositories;
using Account.Core.Repositories.Common;
using Account.Core.Security;
using MediatR;

namespace Account.Application.Features.Auth.Commands.EmailRegistration;

public record StartEmailAuthCommand(StartEmailAuthDto Dto)
    : IRequest<ResultT<IsUserExistsAndActiveResult>>;

public sealed class StartEmailAuthCommandHandler(IUserRepository userRepository,
                                                 IEmailVerificationCodeRepository codeRepository,
                                                 IUnitOfWork unitOfWork,
                                                 IEmailSenderService emailService,
                                                 IVerificationCodeHasher codeHasher)
    : IRequestHandler<StartEmailAuthCommand, ResultT<IsUserExistsAndActiveResult>>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IEmailVerificationCodeRepository _codeRepository = codeRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IEmailSenderService _emailService = emailService;
    private readonly IVerificationCodeHasher _codeHasher = codeHasher;

    public async Task<ResultT<IsUserExistsAndActiveResult>> Handle(StartEmailAuthCommand request, CancellationToken token)
    {
        if (string.IsNullOrEmpty(request.Dto.Email)) //TODO: Implement real email validation
            return Error.Validation(ErrorCodes.InvalidEmail, ErrorMessages.InvalidEmail);

        bool isExists = await _userRepository.IsExistsByEmailAsync(request.Dto.Email, token);
        if (isExists)
        {
            bool isActive = await _userRepository.IsActiveByEmailAsync(request.Dto.Email, token);
            if (isActive)
                return ResultT<IsUserExistsAndActiveResult>.Success(new(true, true));
        }

        string verificationCode = GenerateCode();
        var sendRequest = new SendEmailDto
        (
            Recipient: request.Dto.Email,
            Subject: "Pixel Chess",
            Body: $"""
                <h1>Email Verification</h1>
                <p>Your verification code is: <strong>{verificationCode}</strong></p>
                <p>This code will expire in 10 minutes.</p>
                <p>If you didn't request this, please ignore this email.</p>
                """
        );
        await _emailService.SendEmailAsync(sendRequest, token);

        var hashedCode = _codeHasher.Hash(verificationCode);
        await _codeRepository.AddAsync(EmailVerificationCode.Create(request.Dto.Email, hashedCode), token);

        if (isExists)
            await _userRepository.AddAsync(User.CreatePending(request.Dto.Email, AuthProvider.Email), token);

        await _unitOfWork.SaveChangesAsync(token);

        return ResultT<IsUserExistsAndActiveResult>.Success(new(false, false));
    }

    private static string GenerateCode()
    {
        Random rand = new();
        return rand.Next(100000, 999999).ToString();
    }
}

