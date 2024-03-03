using System.Reflection;
using Domain.Common;
using Domain.Entities.OrderAggregate;
using Domain.Entities.PackingListAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace WebApi;

public class AppDbContext : DbContext
{
    private readonly IDomainEventDispatcher _dispatcher;
    public DbSet<SalesOrder> SalesOrders { get; set; } = null!;

    public DbSet<PackingList> PackingLists { get; set; } = null!;

    public AppDbContext(
        DbContextOptions<AppDbContext> options,
        IDomainEventDispatcher dispatcher) : base(options)
    {
        _dispatcher = dispatcher;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        var changedEntities = ChangeTracker.Entries()
            .Select(e => e.Entity)
            .OfType<Entity>().ToArray();

        foreach (var e in changedEntities)
        {
            await _dispatcher.Dispatch(e);
        }
        return await base.SaveChangesAsync(cancellationToken);
    }
}