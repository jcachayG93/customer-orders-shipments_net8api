using Domain.Entities.PackingListAggregate;

namespace WebApi.Features.PackingLists.Common;

public class PackingListRepository : IPackingListRepository
{
    private readonly AppDbContext _dbContext;

    public PackingListRepository(
        AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task AddAsync(PackingList packingList)
    {
        await _dbContext.AddAsync(packingList);
    }

    public async Task CommitChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}