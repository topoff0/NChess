using Chess.Core.Repositories;
using Chess.Core.Repositories.Common;
using Chess.Infrastructure.Configuration;
using Chess.Infrastructure.Persistence;
using Chess.Infrastructure.Persistence.Repositories;
using Chess.Infrastructure.Persistence.Repositories.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Chess.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
                                                       IConfiguration configuration)
    {
        services.AddConfigurations(configuration);
        services.AddGamesDbContext(configuration);
        services.AddRepositories();

        return services;
    }

    private static IServiceCollection AddGamesDbContext(this IServiceCollection services,
                                                        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("ChessServiceConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'ChessServiceConnection' is not configured.");

        services.AddDbContext<GamesDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IGameRepository, GameRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }

    private static IServiceCollection AddConfigurations(this IServiceCollection services,
                                                        IConfiguration configuration)
    {
        services.AddJwtConfiguration(configuration);

        return services;
    }

    // ================ APP ====================
    
    public static async Task ApplyMigrationsAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<GamesDbContext>();
        await context.Database.MigrateAsync();
    }
}
