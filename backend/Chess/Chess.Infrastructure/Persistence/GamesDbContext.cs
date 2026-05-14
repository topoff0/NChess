using Chess.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Chess.Infrastructure.Persistence
{
    public class GamesDbContext(DbContextOptions<GamesDbContext> options) : DbContext(options)
    {
        public DbSet<GameInfo> Games => Set<GameInfo>();
    }
}
