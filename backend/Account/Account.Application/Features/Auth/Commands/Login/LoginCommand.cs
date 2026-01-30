using Account.Application.DTOs.Errors;
using Account.Application.DTOs.Results.Common;
using Account.Application.Features.Auth.DTOs.Requests;
using Account.Application.Features.Auth.DTOs.Results;
using Account.Core.Entities;
using Account.Core.Repositories;
using Account.Core.Repositories.Common;
using Account.Core.Security;
using MediatR;

namespace Account.Application.Features.Auth.Commands.Login;

public record LoginCommand(LoginDto Dto)
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
        if (string.IsNullOrEmpty(request.Dto.Email))
            return Error.Validation(ErrorCodes.InvalidEmail, ErrorMessages.InvalidEmail);

        var user = await _userRepository.GetByEmailAsync(request.Dto.Email, token);
        if (user is null)
            return Error.NotFound(ErrorCodes.UserNotFound, ErrorMessages.UserNotFound);

        if (user.Status != UserStatus.Active)
            return Error.Conflict(ErrorCodes.AccountNotActivated, ErrorCodes.AccountNotActivated);

        if (!_passwordHasher.Verify(request.Dto.Password, user.PasswordHash))
            return Error.Failure(ErrorCodes.IncorrectPassword, ErrorMessages.IncorrectPassword);

        user.UpdateLastLoginTime();

        await _unitOfWork.SaveChangesAsync(token);

        return new LoginResult(IsSuccess: true);
    }
}
