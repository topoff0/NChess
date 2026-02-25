using Account.Application.Behaviours;
using Account.Application.Features.Auth.Commands.CreateProfile;
using Account.Application.Features.Auth.Commands.EmailAuthentication;
using Account.Application.Features.Auth.Commands.Login;
using Account.Application.Features.Auth.Validation;
using Account.Application.Interfaces;
using Account.Core.Repositories;
using Account.Core.Repositories.Common;
using Account.Core.Security;
using Account.Infrastructure.Configuration;
using Account.Infrastructure.Persistence;
using Account.Infrastructure.Persistence.Repositories;
using Account.Infrastructure.Persistence.Repositories.Common;
using Account.Infrastructure.Security;
using Account.Infrastructure.Services;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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

        services.AddMediatRConfiguration();

        services.AddSecurity();

        services.AddValidators();

        return services;
    }

    public static async Task ApplyMigrationAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<UsersDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<UsersDbContext>>();

        try
        {
            var pendingMigrations = await context.Database.GetPendingMigrationsAsync();

            if (pendingMigrations.Any())
            {
                if (logger.IsEnabled(LogLevel.Information))
                {
                    logger.LogInformation("Applying {Count} pending migrations...", pendingMigrations.Count());
                }
                await context.Database.MigrateAsync();
                logger.LogInformation("Database migrations applied successfully.");
            }
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "An error occurred while applying database migrations.");
            throw;
        }
    }

    // --- Private ---

    private static IServiceCollection AddUsersDbContext(this IServiceCollection services,
                                                        IConfiguration configuration)
    {

        var _connectionString = configuration.GetConnectionString("ChessAccountConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'ChessAccountConnection' is not configured.");

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
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IEmailSenderService, EmailSenderService>();
        services.AddScoped<IImageService, ImageService>();

        return services;
    }

    private static IServiceCollection AddMediatRConfiguration(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(StartEmailAuthCommand).Assembly);
            cfg.RegisterServicesFromAssembly(typeof(VerifyEmailCommand).Assembly);
            cfg.RegisterServicesFromAssembly(typeof(LoginCommand).Assembly);
            cfg.RegisterServicesFromAssembly(typeof(CreateProfileCommand).Assembly);

            cfg.AddOpenBehavior(typeof(ValidationPipelineBehaviour<,>));
        });

        return services;
    }

    private static IServiceCollection AddSecurity(this IServiceCollection services)
    {
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IVerificationCodeHasher, VerificationCodeHasher>();
        services.AddScoped<IRefreshTokenHasher, RefreshTokenHasher>();

        return services;
    }

    private static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<StartEmailAuthCommandValidator>();

        return services;
    }

}
