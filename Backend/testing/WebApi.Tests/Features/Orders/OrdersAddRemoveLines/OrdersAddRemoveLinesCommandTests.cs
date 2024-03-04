using Domain.Entities.OrderAggregate;
using Domain.ValueObjects;
using Moq;
using WebApi.Exceptions;
using WebApi.Features.Orders.Common;
using WebApi.Features.Orders.OrdersAddRemoveLines;
using WebApi.Tests.TestCommon;

namespace WebApi.Tests.Features.Orders.OrdersAddRemoveLines;

public class OrdersAddRemoveLinesCommandTests
{
    private readonly TestValidatorMock<OrdersAddRemoveLinesCommand> _validator = new();
    private Mock<ISalesOrdersRepository> CreateRepositoryMock(
        params ISalesOrderRoot[] getReturns)
    {
        Mock<ISalesOrdersRepository> repository = new Mock<ISalesOrdersRepository>();

        foreach (ISalesOrderRoot aggregate in getReturns)
        {
            repository.Setup(x =>
                    x.GetByIdAsync(new(aggregate.Id)).Result)
                .Returns(aggregate);
        }
        

        return repository;
    }

    private Mock<ISalesOrderRoot> CreateSalesOrderMock()
    {
        Mock<ISalesOrderRoot> result = new Mock<ISalesOrderRoot>();
        Guid id = Guid.NewGuid();
        result.Setup(x => x.Id)
            .Returns(id);

        result.Setup(x => x.GetLineIds())
            .Returns(Array.Empty<EntityIdentity>());

        return result;
    }

    private OrdersAddRemoveLinesCommand CreateCommand(
        Guid orderId,
        params OrdersAddRemoveLinesCommand.OrderLineDto[] lines)
    {
        return new()
        {
            OrderId = orderId,
            Lines = lines
        };
    }

    private OrdersAddRemoveLinesCommand.OrderLineDto CreateLine(Guid lineId)
    {
        return new()
        {
            OrderLineId = lineId,
            Product = Guid.NewGuid().ToString(),
            Quantity = 10,
            UnitPrice = 10
        };
    }

    private OrdersAddRemoveLinesCommand.Handler CreateSut(
        ISalesOrdersRepository repository)
    {
        return new(repository, _validator.Object);
    }

    [Fact]
    public async Task AssertsCommandIsValid()
    {
        // ************ ARRANGE ************
        
        _validator.SetupIsValid(false);

        OrdersAddRemoveLinesCommand command = CreateCommand(Guid.NewGuid());

        Mock<ISalesOrdersRepository> repository = CreateRepositoryMock();

        OrdersAddRemoveLinesCommand.Handler sut = CreateSut(repository.Object);
        
        // ************ ACT ************

        Exception? result = await Record.ExceptionAsync(async () => await sut.Handle(command, CancellationToken.None));

        // ************ ASSERT ************

        Assert.NotNull(result);
        Assert.IsType<ValidationException>(result);

    }

    [Fact]
    public async Task LoadsAggregate()
    {
        // ************ ARRANGE ************

        Mock<ISalesOrderRoot> aggregate = CreateSalesOrderMock();
        
        OrdersAddRemoveLinesCommand command = CreateCommand(aggregate.Object.Id);
        
        Mock<ISalesOrdersRepository> repository = CreateRepositoryMock(
            aggregate.Object);

        OrdersAddRemoveLinesCommand.Handler sut = CreateSut(repository.Object);

        // ************ ACT ************

        await sut.Handle(command, CancellationToken.None);

        // ************ ASSERT ************

        repository.Verify(x=>x.GetByIdAsync(new(command.OrderId)));
    }

    [Fact]
    public async Task WhenAggregateDoesNotExist_ThrowsException()
    {
        // ************ ARRANGE ************

        Mock<ISalesOrdersRepository> repository = CreateRepositoryMock();
        
        OrdersAddRemoveLinesCommand command = CreateCommand(Guid.NewGuid());

        OrdersAddRemoveLinesCommand.Handler sut = CreateSut(repository.Object);

        // ************ ACT ************

        Exception? result = await Record.ExceptionAsync(async () => await sut.Handle(command, CancellationToken.None));

        // ************ ASSERT ************

        Assert.NotNull(result);
        Assert.IsType<EntityNotFoundException>(result);
    }

    [Fact]
    public async Task DeletesAllLinesThatExistInTheAggregateButNotInTheCommand()
    {
        // ************ ARRANGE ************

        EntityIdentity lineToDeleteId = EntityIdentity.Random;
        EntityIdentity lineToKeep = EntityIdentity.Random;

        Mock<ISalesOrderRoot> aggregate = CreateSalesOrderMock();
        aggregate.Setup(x => x.GetLineIds())
            .Returns(lineToDeleteId.ToCollection(lineToKeep));

        Mock<ISalesOrdersRepository> repository = CreateRepositoryMock(aggregate.Object);
        
        OrdersAddRemoveLinesCommand.OrderLineDto commandLine1 = CreateLine(lineToKeep.Value);
        OrdersAddRemoveLinesCommand command = CreateCommand(aggregate.Object.Id, commandLine1);

        OrdersAddRemoveLinesCommand.Handler sut = CreateSut(repository.Object);
        
        // ************ ACT ************

        await sut.Handle(command, CancellationToken.None);
        
        // ************ ASSERT ************

        // Gets aggregate line ids so it can check if they are missing from the command
        aggregate.Verify(x=>x.GetLineIds());
        
        // Remove the line that is missing from the command.
        aggregate.Verify(x=>x.RemoveLine(lineToDeleteId));
        
        // leaves the line that is present in the aggregate and the command.
        aggregate.Verify(x=>x.RemoveLine(lineToKeep), Times.Never);
    }

    [Fact]
    public async Task InsertsLinesThatDoNotExistInTheAggregate()
    {
        // ************ ARRANGE ************

        Mock<ISalesOrderRoot> aggregate = CreateSalesOrderMock();
        Mock<ISalesOrdersRepository> repository = CreateRepositoryMock(aggregate.Object);
        
        OrdersAddRemoveLinesCommand.OrderLineDto line = CreateLine(Guid.NewGuid());
        OrdersAddRemoveLinesCommand command = CreateCommand(aggregate.Object.Id, line);

        OrdersAddRemoveLinesCommand.Handler sut = CreateSut(repository.Object);

        // Line does not exist in the aggregate so it will be added.
        aggregate.Setup(x => x.DoesLineExists(new(line.OrderLineId)))
            .Returns(false);

        // ************ ACT ************

        await sut.Handle(command, CancellationToken.None);

        // ************ ASSERT ************
        
        aggregate.Verify(x=> x.AddLine(
            new(line.OrderLineId),
            new(line.Product),
            new(line.Quantity),
            Money.CreateInDollars(line.UnitPrice)));

    }

    [Fact]
    public async Task UpdatesLinesThatDoExistInTheAggregate()
    {
        // ************ ARRANGE ************

        Mock<ISalesOrderRoot> aggregate = CreateSalesOrderMock();
        Mock<ISalesOrdersRepository> repository = CreateRepositoryMock(aggregate.Object);
        
        OrdersAddRemoveLinesCommand.OrderLineDto line = CreateLine(Guid.NewGuid());
        OrdersAddRemoveLinesCommand command = CreateCommand(aggregate.Object.Id, line);

        OrdersAddRemoveLinesCommand.Handler sut = CreateSut(repository.Object);

        // Line DOES exist in the aggregate so it will be updated
        aggregate.Setup(x => x.DoesLineExists(new(line.OrderLineId)))
            .Returns(true);

        // ************ ACT ************

        await sut.Handle(command, CancellationToken.None);

        // ************ ASSERT ************
        
        aggregate.Verify(x=> x.UpdateLine(
            new(line.OrderLineId),
            new(line.Product),
            new(line.Quantity),
            Money.CreateInDollars(line.UnitPrice)));

    }

    [Fact]
    public async Task CommitsChanges()
    {
        // ************ ARRANGE ************

        Mock<ISalesOrderRoot> aggregate = CreateSalesOrderMock();
        Mock<ISalesOrdersRepository> repository = CreateRepositoryMock(aggregate.Object);
        
        OrdersAddRemoveLinesCommand.OrderLineDto line = CreateLine(Guid.NewGuid());
        OrdersAddRemoveLinesCommand command = CreateCommand(aggregate.Object.Id, line);

        OrdersAddRemoveLinesCommand.Handler sut = CreateSut(repository.Object);

        // Line does not exist in the aggregate so it will be added.
        aggregate.Setup(x => x.DoesLineExists(new(line.OrderLineId)))
            .Returns(false);

        // ************ ACT ************

        await sut.Handle(command, CancellationToken.None);

        // ************ ASSERT ************
        
        repository.Verify(x=>
            x.CommitChangesAsync());

    }
}