using Domain.Entities.OrderAggregate;
using Domain.ValueObjects;

namespace WebApi.Features.Orders.Common;

/// <summary>
/// Domain Driven Design repository (DDD), abstracts the underlying storage technology.
/// </summary>
public interface ISalesOrdersRepository
{
    /*
     * What about cancellation tokens? Up to you. We don't need them for now.
     */
    
    /// <summary>
    /// Adds a sales order.
    /// </summary>
    /// <param name="order"></param>
    /// <returns></returns>
    Task AddAsync(SalesOrder order);

    /// <summary>
    /// Gets the aggregate
    /// </summary>
    Task<ISalesOrderRoot?> GetByIdAsync(EntityIdentity id);

    /// <summary>
    /// Unit of work pattern. We accumulate operations, and they are committed at once when we call this method.
    /// </summary>
    /// <returns></returns>
    Task CommitChangesAsync();
}