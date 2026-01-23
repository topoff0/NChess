using Account.Application.Common.Interfaces;
using Account.Infrastructure.Persistence;
using Account.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Account.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure
        (
          this IServiceCollection services,
          IConfiguration configuration
        )
    {
        services.AddUsersDbContext(configuration);

        services.AddScoped<IJwtTokenService, JwtTokenService>();

        return services;
    }

    //TODO: Add cancellationToken
    public static async Task ApplyMigrationAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<UserDbContext>();

        try
        {
            // TODO: Add logger
            var pendingingMigraitons = await context.Database.GetPendingMigrationsAsync();

            if (pendingingMigraitons.Any())
            {
                await context.Database.MigrateAsync();
            }
        }
        catch (Exception)
        {
            // TODO: Add logger
            throw;
        }
    }

    // --- Private ---

    private static IServiceCollection AddUsersDbContext
        (
           this IServiceCollection services,
           IConfiguration configuration
        )
    {

        var _connectionString = configuration.GetConnectionString("UserServiceConnection")
          ?? throw new InvalidOperationException("Connection string 'UserServiceConnection' is not configured.");

        services.AddDbContext<UserDbContext>(options =>
            {
                options.UseNpgsql(_connectionString, npgOpt =>
                {
                    npgOpt.EnableRetryOnFailure
                (
                 maxRetryCount: 5,
                 maxRetryDelay: TimeSpan.FromSeconds(15),
                 errorCodesToAdd: null
                );
                });
            });

        return services;
    }
}
