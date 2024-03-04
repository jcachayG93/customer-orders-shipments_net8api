using System.Net.Http.Json;
using Domain.Entities.OrderAggregate;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using WebApi.Features.Orders.OrdersAddRemoveLines;
using WebApi.Tests.TestCommon;

namespace WebApi.Tests.Features.Orders.OrdersAddRemoveLines;

[Collection("Sequential")]
public class OrderAddRemoveLine_UseCaseTest
    : IntegrationTestsBase
{
    private readonly CustomApplicationFactory _applicationFactory;
    private readonly HttpClient _client;

    public OrderAddRemoveLine_UseCaseTest()
    {
        _applicationFactory = CreateApplicationFactory();
        _client = _applicationFactory.CreateClient();
    }

    [Fact]
    public async Task CanAddRemoveLines()
    {
        // ************ ARRANGE ************

        SalesOrder existingOrder = new SalesOrder(EntityIdentity.Random);

        _applicationFactory.AddEntitiesToDatabase(existingOrder.ToCollection());

        OrdersAddRemoveLinesCommand.OrderLineDto lines = new OrdersAddRemoveLinesCommand.OrderLineDto()
        {
            OrderLineId = Guid.NewGuid(),
            Product = "Small bolt",
            Quantity = 1000,
            UnitPrice = 0.03M
        };

        OrdersAddRemoveLinesCommand command = new OrdersAddRemoveLinesCommand()
        {
            OrderId = existingOrder.Id,
            Lines = lines.ToCollection()
        };

        string endpoint = "api/sale-orders/add-remove-lines";

        // ************ ACT ************

        await _client.PostAsJsonAsync(endpoint, command);

        // ************ ASSERT ************

        ICollection<SalesOrder> result = _applicationFactory.GetEntities(db =>
            db.SalesOrders.AsNoTracking().Include(e => e.SalesOrderLines).ToArray());

        /*
         * The unit tests are exhaustive, this one is just testing the complete slice.
         */
        Assert.Equal("Small bolt", result.First().SalesOrderLines.First().Product);
    }
}