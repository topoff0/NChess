using Account.Application.Common.Interfaces;
using Account.Application.DTOs.Errors;
using Account.Application.DTOs.Requests.EmailSender;
using Account.Application.DTOs.Results.Common;
using Account.Application.Features.Auth.DTOs.Requests;
using Account.Application.Features.Auth.DTOs.Results;
using Account.Core.Entities;
using Account.Core.Repositories;
using Account.Core.Repositories.Common;
using MediatR;

namespace Account.Application.Features.Auth.Commands.EmailRegistration;

public record StartEmailAuthCommand(StartEmailAuthDto Dto)
    : IRequest<ResultT<IsUserExistsAndActiveResult>>;

public sealed class StartEmailAuthCommandHandler(IUserRepository userRepository,
                                                 IEmailVerificationCodeRepository codeRepository,
                                                 IUnitOfWork unitOfWork,
                                                 IEmailSenderService emailService)
    : IRequestHandler<StartEmailAuthCommand, ResultT<IsUserExistsAndActiveResult>>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IEmailVerificationCodeRepository _codeRepository = codeRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IEmailSenderService _emailService = emailService;

    public async Task<ResultT<IsUserExistsAndActiveResult>> Handle(StartEmailAuthCommand request, CancellationToken token)
    {
        if (string.IsNullOrEmpty(request.Dto.Email)) //TODO: Implement real email validation
            return Error.Validation(ErrorCodes.InvalidEmail, ErrorMessages.InvalidEmail);

        bool isExistsAndActive = await _userRepository.IsExistsAndActiveByEmail(request.Dto.Email, token);
        if (isExistsAndActive)
        {
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

        await _codeRepository.AddAsync(EmailVerificationCode.Create(request.Dto.Email, verificationCode), token);
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

