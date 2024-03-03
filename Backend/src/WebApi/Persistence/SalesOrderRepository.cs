using Domain.Entities.OrderAggregate;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Persistence;

public class SalesOrderRepository
: ISalesOrdersRepository
{
    private readonly AppDbContext _dbContext;

    public SalesOrderRepository(
        AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task AddAsync(SalesOrder order)
    {
        await _dbContext.AddAsync(order);   
    }

    public async Task<ISalesOrderRoot?> GetByIdAsync(EntityIdentity id)
    {
        var result = await _dbContext.SalesOrders
            .Include(e => e.SalesOrderLines)
            .FirstOrDefaultAsync(o => o.Id == id.Value);

        return result;
    }

    public async Task CommitChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}