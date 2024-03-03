using System.Reflection;
using Domain.Entities.OrderAggregate;
using Domain.Entities.PackingListAggregate;
using Microsoft.EntityFrameworkCore;

namespace WebApi;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<SalesOrder> SalesOrders { get; set; } = null!;

    public DbSet<PackingList> PackingLists { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}