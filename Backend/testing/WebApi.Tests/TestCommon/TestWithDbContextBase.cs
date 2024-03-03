using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using WebApi.Persistence;

namespace WebApi.Tests.TestCommon;

/// <summary>
/// A base class for test that require an in memory DbContext but not the application (for that use IntegrationTestBase)
/// </summary>
public abstract class TestWithDbContextBase
{
    public AppDbContext TestDbContext { get;}

    protected TestWithDbContextBase()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var contextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        TestDbContext = new AppDbContext(contextOptions);
    }
}