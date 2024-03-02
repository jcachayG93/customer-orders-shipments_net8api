using Domain.Entities.OrderAggregate;
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

    public async Task CommitChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}