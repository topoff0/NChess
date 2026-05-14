namespace Chess.Core.Repositories.Common;

public interface IUnitOfWork
{
    Task SaveChangesAsync(CancellationToken token);
}
