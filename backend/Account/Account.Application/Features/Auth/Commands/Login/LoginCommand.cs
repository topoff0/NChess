using Account.Application.Common.Errors;
using Account.Application.Common.Results;
using Account.Application.Features.Auth.Results;
using Account.Core.Entities;
using Account.Core.Repositories;
using Account.Core.Repositories.Common;
using Account.Core.Security;
using MediatR;

namespace Account.Application.Features.Auth.Commands.Login;

public record LoginCommand(string Email, string Password)
    : IRequest<ResultT<LoginResult>>;

public sealed class LoginCommandHandler(IUserRepository userRepository,
                                        IUnitOfWork unitOfWork,
                                        IPasswordHasher passwordHasher)
    : IRequestHandler<LoginCommand, ResultT<LoginResult>>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;

    public async Task<ResultT<LoginResult>> Handle(LoginCommand request, CancellationToken token)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, token);
        if (user is null)
            return Error.NotFound(ErrorCodes.UserNotFound, ErrorMessages.UserNotFound);

        if (user.Status != UserStatus.Active)
            return Error.Conflict(ErrorCodes.AccountNotActivated, ErrorCodes.AccountNotActivated);

        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
            return Error.Validation(new Dictionary<string, string[]>
                {
                    {
                        ErrorCodes.InvalidPassword,
                        new[] { ErrorMessages.InvalidPassword }
                    }
                });

        user.UpdateLastLoginTime();

        await _unitOfWork.SaveChangesAsync(token);

        return new LoginResult(IsSuccess: true);
    }
}
