using MediatR;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace WebApi.Tests.TestCommon;

/// <summary>
/// A base class for test that require an in memory DbContext but not the application (for that use IntegrationTestBase)
/// </summary>
public abstract class TestWithDbContextBase
{
    private readonly SqliteConnection _connection;

    protected TestWithDbContextBase()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();
        
    }

    protected AppDbContext CreateDbContext()
    {
        DbContextOptions<AppDbContext> contextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        AppDbContext result = new AppDbContext(contextOptions, Mock.Of<IDomainEventDispatcher>());
        
        result.Database.Migrate();

        return result;
    }
}