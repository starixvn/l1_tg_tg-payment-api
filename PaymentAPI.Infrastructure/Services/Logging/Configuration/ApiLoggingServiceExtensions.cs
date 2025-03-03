using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PaymentAPI.Infrastructure.Services.Logging.Middleware;

namespace PaymentAPI.Infrastructure.Services.Logging.Configuration;

public static class ApiLoggingServiceExtensions
{
    public static IServiceCollection AddApiLoggingService(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<ApiLoggingOptions>? configureOptions = null)
    {
        // Register logging options
        if (configureOptions != null)
        {
            services.Configure(configureOptions);
        }
        else
        {
            services.Configure<ApiLoggingOptions>(options =>
                configuration.GetSection(nameof(ApiLoggingOptions)).Bind(options));
        }

        // Register logging service
        services.AddSingleton<IApiLoggingService, ApiLoggingService>();

        return services;
    }
} 