using Account.Application.Common.Errors;
using Account.Application.Common.Results;
using Account.Application.Features.Auth.Results;
using Account.Core.Repositories;
using MediatR;

namespace Account.Application.Features.Auth.Queries.GetCurrentUser;

public record GetCurrentUserQuery(Guid UserId) : IRequest<ResultT<CurrentUserResult>>;

public sealed class GetCurrentUserQueryHandler(IUserRepository userRepository)
    : IRequestHandler<GetCurrentUserQuery, ResultT<CurrentUserResult>>
{
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<ResultT<CurrentUserResult>> Handle(GetCurrentUserQuery request,
                                                   CancellationToken token)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, token);

        if (user is null)
        {
            return Error.NotFound(ErrorCodes.UserNotFound, ErrorMessages.UserNotFound);
        }

        return new CurrentUserResult(
                user.Id,
                user.Email,
                user.Username,
                user.ImagePath,
                user.IsProfileCreated());
    }
}
