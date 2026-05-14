using Chess.Core.Entities;

namespace Chess.Core.Repositories;

public interface IGameRepository
{
    Task<GameInfo?> GetActiveByFirstPlayerIdAsync(int firstPlayerId, CancellationToken token);

    Task<GameInfo?> GetByFirstPlayerIdAsync(int firstPlayerId, CancellationToken token);

    Task<GameInfo?> GetActiveByPlayerIdAsync(int playerId, CancellationToken token);

    Task AddAsync(GameInfo game, CancellationToken token);
}
