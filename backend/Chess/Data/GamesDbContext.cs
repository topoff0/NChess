using Chess.Models;
using Microsoft.EntityFrameworkCore;

namespace Chess.Data
{
    public class GamesDbContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public GamesDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_configuration.GetConnectionString("ChessServiceConnection"));
        }


        public DbSet<GameInfo> Games { get; set; }
    }
}