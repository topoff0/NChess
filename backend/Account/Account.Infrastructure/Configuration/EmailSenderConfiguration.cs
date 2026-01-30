using Account.Application.Common.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Account.Infrastructure.Configuration;

public static class EmailSenderConfiguration
{
    public static IServiceCollection AddEmailSenderConfiguration(this IServiceCollection services,
                                                                 IConfiguration configuration)
    {
        services.Configure<EmailOptions>(
            configuration.GetSection(EmailOptions.EmailOptionsKey));


        return services;
    }
}
