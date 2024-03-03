using Domain.Entities.OrderAggregate;
using Domain.Entities.PackingListAggregate;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Moq;
using WebApi.Features.PackingLists.Common;
using WebApi.Tests.TestCommon;

namespace WebApi.Tests.Features.PackingLists.PackingListCreate;

[Collection("Sequential")]
public class PackingListCreate_UseCaseTests
: IntegrationTestsBase
{
    private readonly CustomApplicationFactory _applicationFactory;
    private readonly HttpClient _client;
  


    public PackingListCreate_UseCaseTests()
    {
        _applicationFactory = CreateApplicationFactory();
        _client = _applicationFactory.CreateClient();
    }

    private static SalesOrder CreateSalesOrderWithOneLine()
    {
        var result = new SalesOrder(EntityIdentity.Random);
        result.AddLine(EntityIdentity.Random, new NonEmptyString("Bolt"),
            new(10), Money.CreateInDollars(100));
        
        return result;
    }

    private static string CreateMarkAsOrderedEndpoint(Guid salesOrderId)
    {
        return $"api/sale-orders/{salesOrderId}/mark-ordered";
    }

    [Fact]
    public async Task WhenASalesOrderIsOrdered_APackingListIsCreated()
    {
        // ************ ARRANGE ************

        var existingSalesOrder = CreateSalesOrderWithOneLine();
        _applicationFactory.AddEntitiesToDatabase(existingSalesOrder.ToCollection());

        var endPoint = CreateMarkAsOrderedEndpoint(existingSalesOrder.Id);
        
        // ************ ACT ************

        await _client.PostAsync(endPoint, null);
        
        // ************ ASSERT ************

        var result = _applicationFactory.GetEntities(db =>
            db.PackingLists.AsNoTracking().Include(l=>l.Lines).ToArray());

        Assert.Single(result);
        
        Assert.Contains(result, x =>
            x.OrderId == existingSalesOrder.Id);

        var packingList = result.First();
        Assert.Single(packingList.Lines);

        Assert.Contains(packingList.Lines, l =>
            l.Id == existingSalesOrder.SalesOrderLines.First().Id
            && l.Product == existingSalesOrder.SalesOrderLines.First().Product
            && l.Quantity == existingSalesOrder.SalesOrderLines.First().Quantity
        );

    }

    [Fact]
    public async Task WhenASalesOrderIsOrdered_ButCreatingThePackingListFails_TheOrderIsNotSaved()
    {
        /*
         * Suppose we mark an order as ordered, raise the domain event and, but an error prevents the creation of the
         * packing list. We could have a situation in which the order is ordered but the packing list does not exist.
         * There are two ways to handle this:
         * 1. Add both operations in the same transaction.
         * 2. Apply corrective actions.
         *
         * We chose the first, this test proves that.
         */
        
        // ************ ARRANGE ************
        
        // Force the Packinglist repository to throw an exception on add.
        var packingListRepo = new Mock<IPackingListRepository>();
        packingListRepo.Setup(x => x.AddAsync(It.IsAny<PackingList>()))
            .ThrowsAsync(new Exception());

        // Replace the IPackingListRepository in the DI Container so it returns the one that throws an exception
        // (defined in the previous line)
        var applicationFactory = CreateApplicationFactory(services =>
            services.TestReplaceScopedService<IPackingListRepository, PackingListRepository>(
                typeof(PackingListRepository), _ => packingListRepo.Object));

        var client = applicationFactory.CreateClient();

        var existingSalesOrder = CreateSalesOrderWithOneLine();
        
        applicationFactory.AddEntitiesToDatabase(existingSalesOrder.ToCollection());

        var endpoint = CreateMarkAsOrderedEndpoint(existingSalesOrder.Id);

        // ************ ACT ************

        await client.PostAsync(endpoint, null);

        // ************ ASSERT ************
        
        // The ordered was not marked as ordered
        var orders = applicationFactory.GetEntities(db =>
            db.SalesOrders.AsNoTracking().ToArray());
        Assert.True(orders.All(o=>!o.IsOrdered));
        
        // No packing list was created
        var packingLists = applicationFactory.GetEntities(db =>
            db.PackingLists.AsNoTracking().ToArray());

        Assert.Empty(packingLists);
    }

    [Fact]
    public async Task CreatingAPackingListIsIdempotent()
    {
        // ************ ARRANGE ************
        
        /*
         * Business rule: when an order is ordered, a packing list is created.
         * This means ONE packing list. So, if we order the order multiple times then one is created the first time only.
         */
        
        var existingSalesOrder = CreateSalesOrderWithOneLine();
        
        _applicationFactory.AddEntitiesToDatabase(existingSalesOrder.ToCollection());

        var endpoint = CreateMarkAsOrderedEndpoint(existingSalesOrder.Id);

        // ************ ACT ************

        for (var i = 0; i < 10; i++)
        {
            await _client.PostAsync(endpoint, null);
        }
        
        // ************ ASSERT ************

        var packingLists = _applicationFactory.GetEntities(db =>
            db.PackingLists.AsNoTracking().ToArray());

        Assert.Single(packingLists);
    }
}