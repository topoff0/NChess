using Account.Application.Auth.Logger;
using Account.Application.Common.Errors;
using Account.Application.Common.Interfaces;
using Account.Application.Common.Results;
using Account.Application.DTOs;
using Account.Application.Features.Auth.Results;
using Account.Core.Entities;
using Account.Core.Repositories;
using Account.Core.Repositories.Common;
using Account.Core.Security;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Account.Application.Features.Auth.Commands.EmailAuthentication;

public record StartEmailAuthCommand(string Email)
    : IRequest<ResultT<IsUserExistsAndActiveResult>>;

public sealed class StartEmailAuthCommandHandler(IUserRepository userRepository,
                                                 IEmailVerificationCodeRepository codeRepository,
                                                 IUnitOfWork unitOfWork,
                                                 IEmailSenderService emailService,
                                                 IVerificationCodeHasher codeHasher,
                                                 ILogger<StartEmailAuthCommandHandler> logger)
    : IRequestHandler<StartEmailAuthCommand, ResultT<IsUserExistsAndActiveResult>>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IEmailVerificationCodeRepository _codeRepository = codeRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IEmailSenderService _emailService = emailService;
    private readonly IVerificationCodeHasher _codeHasher = codeHasher;
    private readonly ILogger<StartEmailAuthCommandHandler> _logger = logger;

    public async Task<ResultT<IsUserExistsAndActiveResult>> Handle(StartEmailAuthCommand request, CancellationToken token)
    {
        try
        {
            _logger.LogStartEmailAuthentication();

            bool isExists = await _userRepository.IsExistsByEmailAsync(request.Email, token);
            if (isExists)
            {
                bool isActive = await _userRepository.IsActiveByEmailAsync(request.Email, token);
                if (isActive)
                {
                    _logger.LogUserWithSuchEmailAlreadyExistsAndActive(request.Email);
                    return ResultT<IsUserExistsAndActiveResult>.Success(new(true, true));
                }
            }

            string verificationCode = GenerateCode();
            var sendRequest = new SendEmailDto
            (
                Recipient: request.Email,
                Subject: "Verification Code",
                Body: $"""
                <h1>Email Verification</h1>
                <p>Your verification code is: <strong>{verificationCode}</strong></p>
                <p>This code will expire in 10 minutes.</p>
                <p>If you didn't request this, please ignore this email.</p>
                """
            );
            await _emailService.SendEmailAsync(sendRequest, token);

            var hashedCode = _codeHasher.Hash(verificationCode);
            await _codeRepository.AddAsync(EmailVerificationCode.Create(request.Email, hashedCode), token);

            if (!isExists)
            {
                _logger.LogUserWithSuchEmailNotExistsAndNotActive(request.Email);
                await _userRepository.AddAsync(User.CreatePending(request.Email, AuthProvider.Email), token);
            }
            else
            {
                _logger.LogUserWithSuchEmailExistsButNotActive(request.Email);
            }

            await _unitOfWork.SaveChangesAsync(token);

            _logger.LogSuccessfulStartEmailAuth();
            return ResultT<IsUserExistsAndActiveResult>.Success(new(isExists, false));
        }
        catch (Exception ex)
        {
            _logger.LogUnexpectedErrorStartEmailAuth(ex.Message);
            return Error.Failure(ErrorCodes.AuthUnexpectedError,
                                 ErrorMessages.AuthUnexpectedError);
        }
    }

    private static string GenerateCode()
    {
        Random rand = new();
        return rand.Next(100000, 999999).ToString();
    }
}

