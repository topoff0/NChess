using Account.Application.Common.Errors;
using Account.Application.Common.Results;
using Account.Application.Features.Auth.Results;
using Account.Application.Interfaces;
using Account.Application.Logger.Auth;
using Account.Core.Entities;
using Account.Core.Repositories;
using Account.Core.Repositories.Common;
using Account.Core.Security;
using MediatR;
using Microsoft.Extensions.Logging;

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
                                                IPasswordHasher passwordHasher,
                                                ILogger<CreateProfileCommandHandler> logger,
                                                IImageService imageService)
    : IRequestHandler<CreateProfileCommand, ResultT<CreateProfileResult>>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPlayerRepository _playerRepository = playerRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly IImageService _imageService = imageService;
    private readonly ILogger<CreateProfileCommandHandler> _logger = logger;

    public async Task<ResultT<CreateProfileResult>> Handle(CreateProfileCommand request, CancellationToken token)
    {
        try
        {
            _logger.LogStartCreateProfile();

            var user = await _userRepository.GetByEmailAsync(request.Email, token);
            if (user is null)
            {
                _logger.LogUserWithSuchEmailNotFoundCreateProfile(request.Email);
                return Error.NotFound(ErrorCodes.UserNotFound, ErrorMessages.UserNotFound);
            }

            if (user.Status == UserStatus.Active)
            {
                _logger.LogProfileAlreadyCreated(request.Email);
                return Error.Conflict(ErrorCodes.ProfileAlreadyCreated, ErrorMessages.ProfileAlreadyCreated);
            }

            var isUsernameExists = await _userRepository.IsExistsByUsernameAsync(request.Username, token);
            if (isUsernameExists)
            {
                _logger.LogUsernameAlreadyExists(request.Username);
                return Error.Conflict(ErrorCodes.UsernameAlreadyExists, ErrorMessages.UsernameAlreadyExists);
            }

            var imagePath = string.Empty;
            if (request.ProfileImage is not null)
            {
                imagePath = await _imageService.SaveProfileImageAsync(request.ProfileImage, token);
            }

            var hashedPassword = _passwordHasher.Hash(request.Password);
            user.Activate(request.Username, hashedPassword, imagePath);

            var player = Player.Create(user.Id);
            await _playerRepository.AddAsync(player, token);

            await _unitOfWork.SaveChangesAsync(token);

            _logger.LogSuccessfulCreateAccount();

            return ResultT<CreateProfileResult>.Success(new(IsCreated: true));
        }
        catch (Exception ex)
        {
            _logger.LogUnexpectedErrorCreateProfile(ex.Message);
            return Error.Failure(ErrorCodes.AuthUnexpectedError,
                                 ErrorMessages.AuthUnexpectedError);
        }
    }
}
