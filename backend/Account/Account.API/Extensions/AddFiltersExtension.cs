using Account.API.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Account.API.Extensions;

public static class AddFiltersExtension
{
    public static IServiceCollection AddControllersWithFilters(this IServiceCollection services)
    {
        services.AddControllers(options =>
        {
            options.Filters.Add<ValidateModelAttribute>();
        });

        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        });

        return services;
    }
}
