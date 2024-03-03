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
        var repository = new Mock<ISalesOrdersRepository>();

        foreach (var aggregate in getReturns)
        {
            repository.Setup(x =>
                    x.GetByIdAsync(new(aggregate.Id)).Result)
                .Returns(aggregate);
        }
        

        return repository;
    }

    private Mock<ISalesOrderRoot> CreateSalesOrderMock()
    {
        var result = new Mock<ISalesOrderRoot>();
        var id = Guid.NewGuid();
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

        var command = CreateCommand(Guid.NewGuid());

        var repository = CreateRepositoryMock();

        var sut = CreateSut(repository.Object);
        
        // ************ ACT ************

        var result = await Record.ExceptionAsync(async () => await sut.Handle(command, CancellationToken.None));

        // ************ ASSERT ************

        Assert.NotNull(result);
        Assert.IsType<ValidationException>(result);

    }

    [Fact]
    public async Task LoadsAggregate()
    {
        // ************ ARRANGE ************

        var aggregate = CreateSalesOrderMock();
        
        var command = CreateCommand(aggregate.Object.Id);
        
        var repository = CreateRepositoryMock(
            aggregate.Object);

        var sut = CreateSut(repository.Object);

        // ************ ACT ************

        await sut.Handle(command, CancellationToken.None);

        // ************ ASSERT ************

        repository.Verify(x=>x.GetByIdAsync(new(command.OrderId)));
    }

    [Fact]
    public async Task WhenAggregateDoesNotExist_ThrowsException()
    {
        // ************ ARRANGE ************

        var repository = CreateRepositoryMock();
        
        var command = CreateCommand(Guid.NewGuid());

        var sut = CreateSut(repository.Object);

        // ************ ACT ************

        var result = await Record.ExceptionAsync(async () => await sut.Handle(command, CancellationToken.None));

        // ************ ASSERT ************

        Assert.NotNull(result);
        Assert.IsType<EntityNotFoundException>(result);
    }

    [Fact]
    public async Task DeletesAllLinesThatExistInTheAggregateButNotInTheCommand()
    {
        // ************ ARRANGE ************

        var lineToDeleteId = EntityIdentity.Random;
        var lineToKeep = EntityIdentity.Random;

        var aggregate = CreateSalesOrderMock();
        aggregate.Setup(x => x.GetLineIds())
            .Returns(lineToDeleteId.ToCollection(lineToKeep));

        var repository = CreateRepositoryMock(aggregate.Object);
        
        var commandLine1 = CreateLine(lineToKeep.Value);
        var command = CreateCommand(aggregate.Object.Id, commandLine1);

        var sut = CreateSut(repository.Object);
        
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

        var aggregate = CreateSalesOrderMock();
        var repository = CreateRepositoryMock(aggregate.Object);
        
        var line = CreateLine(Guid.NewGuid());
        var command = CreateCommand(aggregate.Object.Id, line);

        var sut = CreateSut(repository.Object);

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

        var aggregate = CreateSalesOrderMock();
        var repository = CreateRepositoryMock(aggregate.Object);
        
        var line = CreateLine(Guid.NewGuid());
        var command = CreateCommand(aggregate.Object.Id, line);

        var sut = CreateSut(repository.Object);

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

        var aggregate = CreateSalesOrderMock();
        var repository = CreateRepositoryMock(aggregate.Object);
        
        var line = CreateLine(Guid.NewGuid());
        var command = CreateCommand(aggregate.Object.Id, line);

        var sut = CreateSut(repository.Object);

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