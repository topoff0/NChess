using System.Text;
using Account.Application.Common.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Account.Infrastructure.Configuration;

public static class JwtConfiguration
{
    public static IServiceCollection AddJwtAuthenticationConfiguration(this IServiceCollection services,
                                                                       IConfiguration configuration)
    {
        var jwtOptions = configuration.GetSection(JwtOptions.JwtOptionsKey).Get<JwtOptions>()
            ?? throw new InvalidOperationException("Jwt section is not configured.");

        // Add jwt settings into IOptions
        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.JwtOptionsKey))
            .Validate(o => !string.IsNullOrWhiteSpace(o.Secret), "JWT Secret is required.")
            .Validate(o => o.Secret.Length >= 32, "JWT Secret must be at least 32 characters.")
            .Validate(o => !string.IsNullOrWhiteSpace(o.Issuer), "JWT Issuer is required.")
            .Validate(o => !string.IsNullOrWhiteSpace(o.Audience), "JWT Audience is required.")
            .Validate(o => o.AccessTokenExpiryMinutes > 0, "Access token expiry must be positive.")
            .Validate(o => o.RefreshTokenExpiryDays > 0, "Refresh token expiry must be positive.")
            .ValidateOnStart();


        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey =
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret)),

                ValidateIssuer = true,
                ValidIssuer = jwtOptions.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtOptions.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });

        return services;
    }
}
