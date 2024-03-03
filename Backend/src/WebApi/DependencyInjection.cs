using System.Reflection;
using Domain.Common;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WebApi.Features.Orders.Common;
using WebApi.Features.PackingLists.Common;
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

        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        services.AddMediatR(c =>
        {
            c.RegisterServicesFromAssemblies(typeof(Program).Assembly);
        });

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddScoped<ISalesOrdersRepository, SalesOrderRepository>();
        services.AddScoped<IPackingListRepository, PackingListRepository>();

        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        return services;
    }
}