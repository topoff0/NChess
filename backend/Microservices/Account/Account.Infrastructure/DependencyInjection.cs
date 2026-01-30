using Account.Application.Common.Interfaces;
using Account.Application.Features.Auth.Commands.CreateProfile;
using Account.Application.Features.Auth.Commands.EmailRegistration;
using Account.Application.Features.Auth.Commands.Login;
using Account.Core.Repositories;
using Account.Core.Repositories.Common;
using Account.Core.Security;
using Account.Infrastructure.Configuration;
using Account.Infrastructure.Persistence;
using Account.Infrastructure.Persistence.Repositories;
using Account.Infrastructure.Persistence.Repositories.Common;
using Account.Infrastructure.Security;
using Account.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Account.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
                                                       IConfiguration configuration)
    {
        services.AddUsersDbContext(configuration);

        services.AddRepositories();

        services.AddConfigurationForServices(configuration);

        services.AddServices();

        services.AddSecurity();

        return services;
    }

    //TODO: Add cancellationToken
    public static async Task ApplyMigrationAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<UsersDbContext>();

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

    private static IServiceCollection AddUsersDbContext(this IServiceCollection services,
                                                        IConfiguration configuration)
    {

        var _connectionString = configuration.GetConnectionString("ChessAccountConnection")
            ?? throw new InvalidOperationException("Connection string 'UserServiceConnection' is not configured.");

        services.AddDbContext<UsersDbContext>(options =>
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

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IPlayerRepository, PlayerRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IEmailVerificationCodeRepository, EmailVerificationRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }

    private static IServiceCollection AddConfigurationForServices(this IServiceCollection services,
                                                                  IConfiguration configuration)
    {
        services.AddJwtAuthenticationConfiguration(configuration);

        services.AddEmailSenderConfiguration(configuration);

        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(typeof(StartEmailAuthCommand).Assembly);
            cfg.RegisterServicesFromAssembly(typeof(VerifyEmailCommand).Assembly);
            cfg.RegisterServicesFromAssembly(typeof(LoginCommand).Assembly);
            cfg.RegisterServicesFromAssembly(typeof(CreateProfileCommand).Assembly);
        });


        services.AddScoped<IJwtTokenService, JwtTokenService>();

        services.AddScoped<IEmailSenderService, EmailSenderService>();

        return services;
    }

    private static IServiceCollection AddSecurity(this IServiceCollection services)
    {
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IVerificationCodeHasher, VerificationCodeHasher>();

        return services;
    }
}
