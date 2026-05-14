using Chess.Core.Repositories.Common;

namespace Chess.Infrastructure.Persistence.Repositories.Common;

public sealed class UnitOfWork(GamesDbContext dbContext) : IUnitOfWork
{
    private readonly GamesDbContext _dbContext = dbContext;

    public Task SaveChangesAsync(CancellationToken token)
    {
        return _dbContext.SaveChangesAsync(token);
    }
}
