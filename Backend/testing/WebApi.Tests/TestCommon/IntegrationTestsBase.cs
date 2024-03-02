using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebApi.Persistence;

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

        AppDbContext context = new AppDbContext(contextOptions);
            
        ServiceDescriptor? oldDbContext = ServiceDescriptor.Scoped(
            typeof(AppDbContext), 
            typeof(AppDbContext));
        
        if (oldDbContext is not null)
        {
            services.Remove(oldDbContext);
        }

        services.AddScoped(typeof(AppDbContext), _ => context);
    }
    
    public void Dispose()
    {
        // In a real project, here I would drop or delete the database (so each test can start with a clean instance)
    }
}