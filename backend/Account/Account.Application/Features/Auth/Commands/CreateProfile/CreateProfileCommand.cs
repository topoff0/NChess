using Account.Application.Common.Errors;
using Account.Application.Common.Results;
using Account.Application.Features.Auth.Results;
using Account.Core.Entities;
using Account.Core.Repositories;
using Account.Core.Repositories.Common;
using Account.Core.Security;
using MediatR;

namespace Account.Application.Features.Auth.Commands.CreateProfile;

public record CreateProfileCommand(byte[]? ProfileImage,
                                   string Email,
                                   string Username,
                                   string Password,
                                   string ConfirmPassword)
    : IRequest<ResultT<CreateProfileResult>>;

public sealed class CreateProfileCommandHandler(IUserRepository userRepository,
                                                IPlayerRepository playerRepository,
                                                IUnitOfWork unitOfWork,
                                                IPasswordHasher passwordHasher)
    : IRequestHandler<CreateProfileCommand, ResultT<CreateProfileResult>>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPlayerRepository _playerRepository = playerRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;

    public async Task<ResultT<CreateProfileResult>> Handle(CreateProfileCommand request, CancellationToken token)
    {
        //TODO: Validaation check

        var user = await _userRepository.GetByEmailAsync(request.Email, token);
        if (user is null)
            return Error.NotFound(ErrorCodes.UserNotFound, ErrorMessages.UserNotFound);

        var hashedPassword = _passwordHasher.Hash(request.Password);
        user.SetUser(request.Username, hashedPassword, "image/path"); // TODO: Create ImageService to handle images operations

        var player = Player.Create(user.Id);
        await _playerRepository.AddAsync(player, token);

        await _unitOfWork.SaveChangesAsync(token);

        return ResultT<CreateProfileResult>.Success(new(IsCreated: true));
    }
}
