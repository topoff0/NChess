using Account.Core.Entities;
using Account.Core.Repositories.Common;

namespace Account.Core.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken token = default);
    Task<RefreshToken?> GetRefreshTokenByHashAsync(string hash, CancellationToken token = default);

    Task<bool> IsExistsByEmailAsync(string email, CancellationToken token = default);
    Task<bool> IsExistsByUsernameAsync(string username, CancellationToken token = default);
    Task<bool> IsActiveByEmailAsync(string email, CancellationToken token = default);
}
