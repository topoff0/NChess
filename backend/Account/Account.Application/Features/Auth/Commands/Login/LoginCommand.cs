using Account.Application.Common.Errors;
using Account.Application.Common.Interfaces;
using Account.Application.Common.Results;
using Account.Application.Features.Auth.Results;
using Account.Application.Logger.Auth;
using Account.Core.Entities;
using Account.Core.Repositories;
using Account.Core.Repositories.Common;
using Account.Core.Security;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Account.Application.Features.Auth.Commands.Login;

public record LoginCommand(string Email, string Password)
    : IRequest<ResultT<LoginResult>>;

public sealed class LoginCommandHandler(IUserRepository userRepository,
                                        IUnitOfWork unitOfWork,
                                        IPasswordHasher passwordHasher,
                                        IJwtTokenService jwtService,
                                        ILogger<LoginCommandHandler> logger)
    : IRequestHandler<LoginCommand, ResultT<LoginResult>>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly IJwtTokenService _jwtService = jwtService;
    private readonly ILogger<LoginCommandHandler> _logger = logger;

    public async Task<ResultT<LoginResult>> Handle(LoginCommand request, CancellationToken token)
    {
        try
        {
            _logger.LogStartLogin();

            var user = await _userRepository.GetByEmailAsync(request.Email, token);
            if (user is null)
            {
                _logger.LogUserWithSuchEmailNotFoundLogin(request.Email);
                return Error.NotFound(ErrorCodes.UserNotFound, ErrorMessages.UserNotFound);
            }

            if (user.Status != UserStatus.Active)
            {
                _logger.LogUserWithSuchEmailNotActiveLogin(request.Email);
                return Error.Conflict(ErrorCodes.AccountNotActivated, ErrorCodes.AccountNotActivated);
            }

            if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
            {
                _logger.LogIncorrectPasswordLogin(request.Email);
                return Error.Validation(new Dictionary<string, string[]>
                {
                    {
                        ErrorCodes.InvalidPassword,
                        new[] { ErrorMessages.InvalidPassword }
                    }
                });
            }

            user.UpdateLastLoginTime();

            await _unitOfWork.SaveChangesAsync(token);

            var jwtToken = _jwtService.GenerateAccessToken(user.Id, user.Email);

            _logger.LogSuccessfulLogin(request.Email);

            return new LoginResult(jwtToken);
        }
        catch (Exception ex)
        {
            _logger.LogUnexpectedErrorLogin(ex.Message);
            return Error.Failure(ErrorCodes.AuthUnexpectedError,
                                 ErrorMessages.AuthUnexpectedError);
        }
    }
}
