namespace Chess.API.Extensions;

public static class CorsExtension
{
    private const string FrontendDevOrigin = "http://localhost:5173";

    public static IServiceCollection AddCustomCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy
                    .WithOrigins(FrontendDevOrigin)
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        return services;
    }
}
