using Chess.Core.Entities;
using Chess.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Chess.Infrastructure.Persistence.Repositories;

public sealed class GameRepository(GamesDbContext dbContext) : IGameRepository
{
    private readonly GamesDbContext _dbContext = dbContext;

    public Task<GameInfo?> GetActiveByFirstPlayerIdAsync(Guid firstPlayerId, CancellationToken token)
    {
        return _dbContext.Games
            .FirstOrDefaultAsync(game => game.FirstPlayerId == firstPlayerId && game.IsActiveGame, token);
    }

    public Task<GameInfo?> GetByFirstPlayerIdAsync(Guid firstPlayerId, CancellationToken token)
    {
        return _dbContext.Games
            .FirstOrDefaultAsync(game => game.FirstPlayerId == firstPlayerId, token);
    }

    public Task<GameInfo?> GetActiveByPlayerIdAsync(Guid playerId, CancellationToken token)
    {
        return _dbContext.Games
            .FirstOrDefaultAsync(
                game => (game.FirstPlayerId == playerId || game.SecondPlayerId == playerId)
                    && game.IsActiveGame,
                token);
    }

    public async Task AddAsync(GameInfo game, CancellationToken token)
    {
        await _dbContext.Games.AddAsync(game, token);
    }
}
