using Account.Core.Entities;
using Account.Core.Repositories.Common;

namespace Account.Core.Repositories;

public interface IPlayerRepository : IRepository<Player>
{
    Task<Player?> GetByEmailAsync(string email, CancellationToken token = default);
    Task<bool> IsExistsByEmail(string email, CancellationToken token = default);
}
