using Domain.Entities.OrderAggregate;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using WebApi.Tests.TestCommon;

namespace WebApi.Tests.Features.Orders.OrdersMarkAsOrdered;

[Collection("Sequential")]
public class OrdersMarkAsOrdered_UseCaseTests
: IntegrationTestsBase
{
    private readonly CustomApplicationFactory _applicationFactory;
    private readonly HttpClient _client;
    
    public OrdersMarkAsOrdered_UseCaseTests()
    {
        _applicationFactory = CreateApplicationFactory();
        _client = _applicationFactory.CreateClient();
    }

    [Fact]
    public async Task MarksOrderAsOrdered()
    {
        // ************ ARRANGE ************

        SalesOrder existingOrder = new SalesOrder(EntityIdentity.Random);
        
        Assert.False(existingOrder.IsOrdered);
        
        _applicationFactory.AddEntitiesToDatabase(existingOrder.ToCollection());

        string endpoint = $"api/sale-orders/{existingOrder.Id}/mark-ordered";
        
        // ************ ACT ************

        await _client.PostAsync(endpoint, null);
        
        // ************ ASSERT ************

        ICollection<SalesOrder> orders = _applicationFactory.GetEntities(db =>
            db.SalesOrders.AsNoTracking().ToArray());

        Assert.Single(orders);
        Assert.Contains(orders, o =>
            o.IsOrdered);
    }
}