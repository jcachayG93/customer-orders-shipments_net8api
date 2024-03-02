using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Features.OrdersCreate;
using WebApi.Tests.TestCommon;

namespace WebApi.Tests.Features;

[Collection("sequential")]
public class Feature1_CreateSalesOrder_IntegrationTests : IntegrationTestsBase
{
    private readonly HttpClient _client;
    private readonly CustomApplicationFactory _factory;

    public Feature1_CreateSalesOrder_IntegrationTests()
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
            db.Orders.AsNoTracking().ToList());

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
            db.Orders.AsNoTracking().ToList());

        Assert.Single(entities);

    }
}