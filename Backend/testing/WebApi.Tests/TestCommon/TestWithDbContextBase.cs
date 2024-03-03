using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using WebApi.Persistence;

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
        var contextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        return new(contextOptions);
    }
}