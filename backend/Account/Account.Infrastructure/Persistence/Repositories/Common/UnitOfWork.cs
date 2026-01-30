//TODO: add logger (maybe not in this class)
using Account.Core.Repositories.Common;

namespace Account.Infrastructure.Persistence.Repositories.Common;

public class UnitOfWork(UsersDbContext context) : IUnitOfWork
{
    private readonly UsersDbContext _context = context;

    public async Task<int> SaveChangesAsync(CancellationToken token = default)
    {
        return await _context.SaveChangesAsync(token);
    }
}
