using System.Net.Http.Json;
using Domain.Entities.OrderAggregate;
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

        OrdersCreateOrderCommand command = new OrdersCreateOrderCommand()
        {
            OrderId = Guid.NewGuid()
        };
        
        string endpoint = $"api/sale-orders/{command.OrderId}";
        
        // ************ ACT ************

        HttpResponseMessage response = await _client.PostAsJsonAsync(endpoint, command);
        
        // ************ ASSERT ************

        ICollection<SalesOrder> entities = _factory.GetEntities(db =>
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
        
        OrdersCreateOrderCommand command = new OrdersCreateOrderCommand()
        {
            OrderId = Guid.NewGuid()
        };
        
        string endpoint = $"api/sale-orders/{command.OrderId}";
        
        // ************ ACT ************

        for (int i = 0; i < 10; i++)
        {
            await _client.PostAsJsonAsync(endpoint, command);
        }
        
        // ************ ASSERT ************

        ICollection<SalesOrder> entities = _factory.GetEntities(db =>
            db.SalesOrders.AsNoTracking().ToList());

        Assert.Single(entities);

    }
}