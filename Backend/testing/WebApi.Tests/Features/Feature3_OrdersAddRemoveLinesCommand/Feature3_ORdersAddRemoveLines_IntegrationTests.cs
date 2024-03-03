using System.Net.Http.Json;
using Domain.Entities.OrderAggregate;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using WebApi.Features.OrdersAddRemoveLines;
using WebApi.Tests.TestCommon;

namespace WebApi.Tests.Features.Feature3_OrdersAddRemoveLinesCommand;

[Collection("Sequential")]
public class Feature3_ORdersAddRemoveLines_IntegrationTests
    : IntegrationTestsBase
{
    private readonly CustomApplicationFactory _applicationFactory;
    private readonly HttpClient _client;

    public Feature3_ORdersAddRemoveLines_IntegrationTests()
    {
        _applicationFactory = CreateApplicationFactory();
        _client = _applicationFactory.CreateClient();
    }

    [Fact]
    public async Task CanAddRemoveLines()
    {
        // ************ ARRANGE ************

        var existingOrder = new SalesOrder(EntityIdentity.Random);

        _applicationFactory.AddEntitiesToDatabase(existingOrder.ToCollection());

        var lines = new OrdersAddRemoveLinesCommand.OrderLineDto()
        {
            OrderLineId = Guid.NewGuid(),
            Product = "Small bolt",
            Quantity = 1000,
            UnitPrice = 0.03M
        };

        var command = new OrdersAddRemoveLinesCommand()
        {
            OrderId = existingOrder.Id,
            Lines = lines.ToCollection()
        };

        var endpoint = "api/sale-orders/add-remove-lines";

        // ************ ACT ************

        await _client.PostAsJsonAsync(endpoint, command);

        // ************ ASSERT ************

        var result = _applicationFactory.GetEntities(db =>
            db.SalesOrders.AsNoTracking().Include(e => e.SalesOrderLines).ToArray());

        /*
         * The unit tests are exhaustive, this one is just testing the complete slice.
         */
        Assert.Equal("Small bolt", result.First().SalesOrderLines.First().Product);
    }
}