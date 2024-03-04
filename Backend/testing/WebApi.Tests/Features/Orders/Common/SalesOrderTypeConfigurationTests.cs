using Domain.Entities.OrderAggregate;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using WebApi.Tests.TestCommon;

namespace WebApi.Tests.Features.Orders.Common;

public class SalesOrderTypeConfigurationTests
: TestWithDbContextBase
{
    [Fact]
    public async Task CanStoreAndLoadSalesOrder()
    {
        // ************ ARRANGE ************

        SalesOrder salesOrder = new SalesOrder(EntityIdentity.Random);
        
        salesOrder.AddLine(
            EntityIdentity.Random, 
            new("bolt"),
            new(10),
            Money.CreateInDollars(0.95M));

        // ************ ACT ************

        AppDbContext dbContext1 = CreateDbContext();
        await dbContext1.AddAsync(salesOrder);
        await dbContext1.SaveChangesAsync();

        AppDbContext dbContext2 = CreateDbContext();

        SalesOrder result = await dbContext2.SalesOrders.AsNoTracking()
            .Include(e => e.SalesOrderLines).FirstAsync();
        
        // ************ ASSERT ************
        
        Assert.Equivalent(salesOrder, result);

    }
}