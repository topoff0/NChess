namespace Account.API.Extensions;

public static class CorsExtension
{
    public static IServiceCollection AddMyCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policyOpt =>
            {
                policyOpt
                    .WithOrigins("http://localhost:5173")
                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
        return services;
    }
}
