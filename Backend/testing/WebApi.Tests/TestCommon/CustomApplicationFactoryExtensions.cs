using Microsoft.Extensions.DependencyInjection;

namespace WebApi.Tests.TestCommon;

public static class CustomApplicationFactoryExtensions
{
    /// <summary>
    /// Gets a service from the underlying Dependency Injection container.
    /// </summary>
    public static IServiceScope GetService<TServiceDefinition>(
        this CustomApplicationFactory customApplicationFactory,
        out TServiceDefinition? service)
    {
        IServiceScope scope = customApplicationFactory.Services.CreateScope();
        service = scope.ServiceProvider.GetService<TServiceDefinition>();

        return scope;
    }
}