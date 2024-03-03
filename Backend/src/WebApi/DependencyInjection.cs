using System.Reflection;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using WebApi.Features.Orders.Common;
using WebApi.Middleware;

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

        services.AddMediatR(c => c.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddScoped<ISalesOrdersRepository, SalesOrderRepository>();

        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        return services;
    }
}