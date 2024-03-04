using Domain.Entities.OrderAggregate;
using Domain.ValueObjects;
using Moq;
using WebApi.Exceptions;
using WebApi.Features.Orders.Common;
using WebApi.Features.Orders.OrderMarkAsOrdered;

namespace WebApi.Tests.Features.Orders.OrdersMarkAsOrdered;

public class OrderMarkAsOrderedCommandTests
{
    
    private Mock<ISalesOrdersRepository> CreateRepository(ISalesOrderRoot? getByIdReturns)
    {
        Mock<ISalesOrdersRepository> result = new Mock<ISalesOrdersRepository>();
        result.Setup(x => x.GetByIdAsync(It.IsAny<EntityIdentity>())
            .Result).Returns(getByIdReturns);

        return result;
    }

    private Mock<ISalesOrderRoot> CreateSalesOrder()
    {
        return new();
    }

    private OrderMarkAsOrderedCommand.Handler CreateSut(ISalesOrdersRepository repository)
    {
        return new(repository);
    }

    private OrderMarkAsOrderedCommand CreateCommand()
    {
        return new()
        {
            OrderId = Guid.NewGuid()
        };
    }
    
    [Fact]
    public async Task WhenOrderNotFound_ThrowsOrderNotFound()
    {
        // ************ ARRANGE ************

        Mock<ISalesOrdersRepository> repository = CreateRepository(null);

        OrderMarkAsOrderedCommand command = CreateCommand();

        OrderMarkAsOrderedCommand.Handler sut = CreateSut(repository.Object);

        // ************ ACT ************

        Exception? result = await Record
            .ExceptionAsync(async () => await sut.Handle(command, CancellationToken.None));

        // ************ ASSERT ************

        Assert.NotNull(result);
        Assert.IsType<EntityNotFoundException>(result);

    }

    [Fact]
    public async Task GetsOrderFromRepository_MarksAsOrdered_CommitsChanges()
    {
        // ************ ARRANGE ************

        Mock<ISalesOrderRoot> aggregate = CreateSalesOrder();

        Mock<ISalesOrdersRepository> repository = CreateRepository(aggregate.Object);

        OrderMarkAsOrderedCommand command = CreateCommand();

        OrderMarkAsOrderedCommand.Handler sut = CreateSut(repository.Object);
        
        // ************ ACT ************

        await sut.Handle(command, CancellationToken.None);

        // ************ ASSERT ************
        
        // Gets the aggregate
        repository.Verify(x=>x.GetByIdAsync(new(command.OrderId)));
        
        // Marks as ordered
        aggregate.Verify(x=>x.MarkAsOrdered());
        
        // commits changes
        repository.Verify(x=> x.CommitChangesAsync());

    }
}