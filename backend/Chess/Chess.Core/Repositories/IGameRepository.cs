using Chess.Core.Entities;

namespace Chess.Core.Repositories;

public interface IGameRepository
{
    Task<GameInfo?> GetActiveByFirstPlayerIdAsync(Guid firstPlayerId, CancellationToken token);

    Task<GameInfo?> GetByFirstPlayerIdAsync(Guid firstPlayerId, CancellationToken token);

    Task<GameInfo?> GetActiveByPlayerIdAsync(Guid playerId, CancellationToken token);

    Task AddAsync(GameInfo game, CancellationToken token);
}
