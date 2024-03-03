using Domain.Entities.PackingListAggregate;

namespace WebApi.Features.PackingLists.Common;

public interface IPackingListRepository
{
    /// <summary>
    /// Adds the packing list
    /// </summary>
    Task AddAsync(PackingList packingList);
    
    /// <summary>
    /// Unit of work pattern. We accumulate operations, and they are committed at once when we call this method.
    /// </summary>
    /// <returns></returns>
    Task CommitChangesAsync();
}