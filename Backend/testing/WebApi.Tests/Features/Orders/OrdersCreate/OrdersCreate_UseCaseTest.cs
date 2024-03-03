using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using WebApi.Features.Orders.OrdersCreate;
using WebApi.Tests.TestCommon;

namespace WebApi.Tests.Features.Orders.OrdersCreate;

[Collection("sequential")]
public class OrdersCreate_UseCaseTest : IntegrationTestsBase
{
    private readonly HttpClient _client;
    private readonly CustomApplicationFactory _factory;

    public OrdersCreate_UseCaseTest()
    {
        _factory = CreateApplicationFactory();
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task CanCreateSalesOrder()
    {
        // ************ ARRANGE ************

        var command = new OrdersCreateOrderCommand()
        {
            OrderId = Guid.NewGuid()
        };
        
        var endpoint = $"api/sale-orders/{command.OrderId}";
        
        // ************ ACT ************

        var response = await _client.PostAsJsonAsync(endpoint, command);
        
        // ************ ASSERT ************

        var entities = _factory.GetEntities(db =>
            db.SalesOrders.AsNoTracking().ToList());

        Assert.Single(entities);
        Assert.Contains(entities, e =>
            e.Id == command.OrderId);
        
        response.AssertIsSuccessful();
    }

    [Fact]
    public async Task CreateSalesOrderIsIdempotent()
    {
        // ************ ARRANGE ************
        
        var command = new OrdersCreateOrderCommand()
        {
            OrderId = Guid.NewGuid()
        };
        
        var endpoint = $"api/sale-orders/{command.OrderId}";
        
        // ************ ACT ************

        for (var i = 0; i < 10; i++)
        {
            await _client.PostAsJsonAsync(endpoint, command);
        }
        
        // ************ ASSERT ************

        var entities = _factory.GetEntities(db =>
            db.SalesOrders.AsNoTracking().ToList());

        Assert.Single(entities);

    }
}