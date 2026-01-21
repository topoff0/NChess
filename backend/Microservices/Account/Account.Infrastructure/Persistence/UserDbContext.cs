using Account.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Account.Infrastructure.Persistence
{
    public class UserDbContext() : DbContext
    {
        public DbSet<Player> Players { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserDbContext).Assembly);
        }
    }
}
