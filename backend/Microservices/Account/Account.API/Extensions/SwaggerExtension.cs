using Microsoft.OpenApi;

namespace Account.API.Extensions;

public static class SwaggerExtension
{
    public static IServiceCollection AddMySwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Chess API",
                Version = "1.0"
            });
        });

        return services;
    }

    public static IApplicationBuilder UseMySwagger(this IApplicationBuilder app, IHostEnvironment env)
    {
        if (!env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Chess API v1.0");
                options.RoutePrefix = string.Empty;
            });
        }

        return app;
    }



}
