using System.Reflection;
using Domain.Common;
using Domain.Entities.OrderAggregate;
using Domain.Entities.PackingListAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace WebApi;

public class AppDbContext(DbContextOptions<AppDbContext> options, IMediator mediator) : DbContext(options)
{
    private readonly IMediator _mediator = mediator;
    public DbSet<SalesOrder> SalesOrders { get; set; } = null!;

    public DbSet<PackingList> PackingLists { get; set; } = null!;

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

        var allEvents = changedEntities.SelectMany(e => e.DomainEvents).ToArray();
        
        foreach (var de in allEvents)
        {
            await _mediator.Publish(de);
        } 
        return await base.SaveChangesAsync(cancellationToken);
    }
}