using Account.Core.Entities;
using Account.Core.Repositories.Common;

namespace Account.Core.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken token = default);
    Task<bool> IsExistsAndActiveByEmail(string email, CancellationToken token = default);
}
