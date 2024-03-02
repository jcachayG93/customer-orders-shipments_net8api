
using System.Net.Http.Json;
using Domain.Entities.OrderAggregate;
using Domain.ValueObjects;
using WebApi.Features.OrdersGetOrderList;
using WebApi.Tests.TestCommon;

namespace WebApi.Tests.Features.OrdersGetOrderList;

[Collection("Sequential")]
public class Feature2_OrdersGetOrderListQuery_IntegrationTests 
: IntegrationTestsBase
{
    private readonly CustomApplicationFactory _applicationFactory;
    private readonly HttpClient _client;

    public Feature2_OrdersGetOrderListQuery_IntegrationTests()
    {
        _applicationFactory = CreateApplicationFactory();
        _client = _applicationFactory.CreateClient();
    }
    private SalesOrder CreateSalesOrderWithLines()
    {
        var rng = new Random();
        
        var result = new SalesOrder(EntityIdentity.Random);
        for (var i = 0; i < 5; i++)
        {
            var quantity = rng.Next(1, 10);
            var unitPrice = (decimal)rng.Next(10, 30);
            result.AddLine(
                EntityIdentity.Random,
                new(Guid.NewGuid().ToString()),
                new(quantity),
                Money.CreateInDollars(unitPrice));
        }

        return result;
    }
    
    [Fact]
    public async Task CanGetOrderList()
    {
        // ************ ARRANGE ************

        var orders = Enumerable.Range(0, 10)
            .Select(i => CreateSalesOrderWithLines())
            .ToArray();

        _applicationFactory.AddEntitiesToDatabase(orders);

        var endpoint = "api/sale-orders/";
        
        // ************ ACT ************

        var response = await _client.GetFromJsonAsync<ICollection<SalesOrderLookup>>(endpoint);
        
        // ************ ASSERT ************

        Assert.NotNull(response);
        Assert.Equal(10, response.Count);

        foreach (var resultingLookup in response)
        {
            var input = orders
                .First(o => o.Id == resultingLookup.OrderId);
            
            Assert.Equal(input.Total, resultingLookup.Total);
        }
    }    
}