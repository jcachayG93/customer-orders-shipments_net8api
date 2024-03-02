using Microsoft.EntityFrameworkCore;
using WebApi.Persistence;

namespace WebApi;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var cs = configuration
            .GetConnectionString("AppConnectionString");

        if (cs is null)
        {
            throw new InvalidOperationException("AppConnectionString is missing from configuration");
        }

        services
            .AddDbContext<AppDbContext>(options =>
            {
                options
                    //  #if DEBUG
                    //  .EnableDetailedErrors()
                    //  .EnableSensitiveDataLogging()
                    //  #endif
                    .UseNpgsql(cs);
            });

        return services;
    }
}