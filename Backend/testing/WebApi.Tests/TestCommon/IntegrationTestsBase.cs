using MediatR;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace WebApi.Tests.TestCommon;

public abstract class IntegrationTestsBase : IDisposable
{
    
    protected IntegrationTestsBase()
    {
       
    }

    public CustomApplicationFactory CreateApplicationFactory(
        Action<IServiceCollection>? additionalConfigureServiceAction = null)
    {
        return new CustomApplicationFactory(
            b =>
            {
                b.ConfigureServices(services =>
                {
                    /*
                     * Instead of the real DbContext, we will use an SqlLite in-memory implementation.
                     */
                    ReplaceAppDbContextWithInMemoryImplementation(services);
                    
                    if (additionalConfigureServiceAction is not null)
                    {
                        additionalConfigureServiceAction(services);
                    }
                });
            });
    }

    /// <summary>
    /// Replaces the AppDbContext to work with an in-memory SqlLite database, overriding the app configuration.
    /// </summary>
    private static void ReplaceAppDbContextWithInMemoryImplementation(IServiceCollection services)
    {
        // Use an in-memory implementation instead of the real database
        //https://learn.microsoft.com/en-us/ef/core/testing/testing-without-the-database#sqlite-in-memory
        
        SqliteConnection connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        DbContextOptions<AppDbContext> contextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;
        
        services.TestReplaceScopedService<AppDbContext, AppDbContext>(
            typeof(AppContext),
            sp =>
            {
                var dispatcher = sp.GetService<IDomainEventDispatcher>()!;
                var result = new AppDbContext(contextOptions, dispatcher);
                result.Database.Migrate();
                return result;
            });
    }
    
    public void Dispose()
    {
        // In a real project, here I would drop or delete the database (so each test can start with a clean instance)
    }
}