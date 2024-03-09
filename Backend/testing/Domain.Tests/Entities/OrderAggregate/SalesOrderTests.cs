using Domain.Entities.OrderAggregate;
using Domain.Entities.OrderAggregate.DomainEvents;
using Domain.Exceptions;
using Domain.ValueObjects;

namespace Domain.Tests.Entities.OrderAggregate;

public class SalesOrderTests
{
    /*
     * In test projects, I always specify types explicitly (instead of using var) because a test is also
     * documentation. When reading a PR in a browser you normally wont see the type next to var. 
     */
    private SalesOrder CreateSut()
    {
        return new SalesOrder(EntityIdentity.Random);
    }
    

    [Fact]
    public void CreateOrder_InitializesOrder_AssertsInvariants()
    {
        // ************ ARRANGE ************

        EntityIdentity id = EntityIdentity.Random;

        // ************ ACT ************

        SalesOrder sut = new SalesOrder(id);

        // ************ ASSERT ************

        Assert.Equal(id.Value, sut.Id);
        Assert.True(sut.AssertInvariantsWasCalled);
    }

    [Fact]
    public void AddOrderLine_AddsOrderLineWithCorrectValues_AssertsInvariants()
    {
        // ************ ARRANGE ************

        SalesOrder sut = CreateSut();

        EntityIdentity lineId = EntityIdentity.Random;

        sut.AssertInvariantsWasCalled = false;

        // ************ ACT ************

        sut.AddLine(lineId, new NonEmptyString("Dog Food Bag"),
            new GreaterThanZeroInteger(10), Money.CreateInDollars(100));

        // ************ ASSERT ************

        Assert.Single(sut.SalesOrderLines);

        Assert.Contains(sut.SalesOrderLines, l =>
            l.Id == lineId.Value
            && l.Product == "Dog Food Bag"
            && l.Quantity == 10
            && l.UnitPrice == 100M
        );

        Assert.True(sut.AssertInvariantsWasCalled);
    }

    [Fact]
    public void EachLineTotalIsQuantityTimesUnitPrice()
    {
        // ************ ARRANGE ************

        SalesOrder sut = CreateSut();

        sut.AddLine(EntityIdentity.Random, new NonEmptyString("Dog Food Bag"),
            new GreaterThanZeroInteger(10), Money.CreateInDollars(100));

        // ************ ACT ************

        decimal result = sut.SalesOrderLines.First().Total;

        // ************ ASSERT ************

        // 10 units x $ 100 = $ 1000
        Assert.Equal(10 * 100M, result);
    }

    [Fact]
    public void OrderTotalIsTheTotalOfAllLines()
    {
        // ************ ARRANGE ************

        SalesOrder sut = CreateSut();

        sut.AddLine(EntityIdentity.Random, new NonEmptyString("Dog Food Bag"),
            new GreaterThanZeroInteger(5), Money.CreateInDollars(50)); // $250
        sut.AddLine(EntityIdentity.Random, new NonEmptyString("Cat Food Bag"),
            new GreaterThanZeroInteger(10), Money.CreateInDollars(100)); // $ 1000 

        // ************ ACT ************

        decimal result = sut.Total;

        // ************ ASSERT ************

        Assert.Equal(1250M, result);
    }

    [Fact]
    public void RemoveLine_RemovesLine_AssertsInvariants()
    {
        // ************ ARRANGE ************

        SalesOrder sut = CreateSut();

        EntityIdentity lineId = EntityIdentity.Random;
        
        sut.AddLine(lineId, new NonEmptyString("Tire 18 inch"),
            new GreaterThanZeroInteger(4), Money.CreateInDollars(99.99M));

        sut.AssertInvariantsWasCalled = false;

        // ************ ACT ************
        
        sut.RemoveLine(lineId);

        // ************ ASSERT ************

        Assert.Empty(sut.SalesOrderLines);
        Assert.True(sut.AssertInvariantsWasCalled);
    }

    [Fact]
    public void RemoveLine_WhenLineDoesNotExist_DoesNothing()
    {
        // ************ ARRANGE ************

        SalesOrder sut = CreateSut();

        // ************ ACT ************

        Exception? result = Record.Exception(()=>sut.RemoveLine(EntityIdentity.Random));
        
        // ************ ASSERT ************

        Assert.Null(result);
    }

    [Fact]
    public void UpdateLine_UpdatesLine_AssertsInvariants()
    {
        // ************ ARRANGE ************

        SalesOrder sut = CreateSut();
        EntityIdentity lineId = EntityIdentity.Random;
        
        sut.AddLine(lineId, new NonEmptyString("Bolts"),new(1000),
            Money.CreateInDollars(0.11M));

        sut.AssertInvariantsWasCalled = false;

        // ************ ACT ************
        
        sut.UpdateLine(lineId, new NonEmptyString("Large Bolts"), new(1110),
            Money.CreateInDollars(0.16M));

        // ************ ASSERT ************

        SalesOrderLine result = sut.SalesOrderLines.First();
        
        Assert.Equal("Large Bolts", result.Product);
        Assert.Equal(1110, result.Quantity);
        Assert.Equal(0.16M, result.UnitPrice);

        Assert.True(sut.AssertInvariantsWasCalled);
    }

    [Fact]
    public void UpdateLine_WhenLineDoesNotExist_ThrowsException()
    {
        /*
         * Trying to update a line that does not exist is not an idempotent operation, it is actually
         * some kind of error, probably a user error.
         */
        // ************ ARRANGE ************

        SalesOrder sut = CreateSut();

        // ************ ACT ************

        Exception? result = Record.Exception(()=>sut.UpdateLine(EntityIdentity.Random,
            new("Bolt"), new(300), Money.CreateInDollars(0.10M)));
        
        // ************ ASSERT ************

        Assert.NotNull(result);
        Assert.IsType<LineNotFoundException>(result);

    }

    [Fact]
    public void MarkAsOrdered_SetsOrderedToTrue_RaisesOrderOrderedDomainEvent_AssertsInvariants()
    {
        // ************ ARRANGE ************

        SalesOrder sut = CreateSut();

        EntityIdentity line1Id = EntityIdentity.Random;
        sut.AddLine(line1Id, new("Bolt"), new GreaterThanZeroInteger(10),
            Money.CreateInDollars(1));

        sut.AssertInvariantsWasCalled = false;
        Assert.False(sut.IsOrdered);
        
        // ************ ACT ************

        sut.MarkAsOrdered();
        
        // ************ ASSERT ************

        Assert.True(sut.IsOrdered);
        Assert.True(sut.AssertInvariantsWasCalled);

        Assert.Single(sut.DomainEvents);
        Assert.Contains(sut.DomainEvents, e =>
            e is OrderOrdered cev
            && cev.OrderId == sut.Id
            && cev.Lines.Count() == 1
            && cev.Lines.Any(l =>
                l.LineId == line1Id.Value
                && l.Product == "Bolt"
                && l.Quantity == 10));
    }

    [Fact]
    public void MarkAsOrdered_WhenIsAlreadyOrdered_DoesNotRaiseAdditionalDomainEvents()
    {
        /*
         * this prevents the creation of multiple packing lists if the user calls MarkAsOrdered several times (idempotency)
         */
        
        // ************ ARRANGE ************

        SalesOrder sut = CreateSut();
        sut.MarkAsOrdered();

        Assert.Single(sut.DomainEvents);

        // ************ ACT ************

        for (int i = 0; i < 10; i++)
        {
            sut.MarkAsOrdered();
        }
        
        // ************ ASSERT ************

        Assert.Single(sut.DomainEvents);
    }

    [Theory]
    [InlineData(true, true)]
    [InlineData(false, false)]
    public void DoesLineExists_TrueWhenLineExists(
        bool lineExists, bool expectedResult)
    {
        // ************ ARRANGE ************

        SalesOrder sut = CreateSut();

        EntityIdentity existingLineId = EntityIdentity.Random;
        EntityIdentity queryLineId = lineExists ? existingLineId : EntityIdentity.Random;
        
        sut.AddLine(existingLineId, new NonEmptyString("Dog Food Bag"),
            new(10), Money.CreateInDollars(100));

        // ************ ACT ************

        bool result = sut.DoesLineExists(queryLineId);

        // ************ ASSERT ************
        
        Assert.Equal(expectedResult, result);

    }

    [Fact]
    public void GetLineIds_ReturnsTheIdsForAllLines()
    {
        // ************ ARRANGE ************

        SalesOrder sut = CreateSut();
        EntityIdentity id1 = EntityIdentity.Random;
        EntityIdentity id2 = EntityIdentity.Random;
        
        sut.AddLine(id1, new NonEmptyString("Dog Food Bag"),
            new(10), Money.CreateInDollars(100));

        sut.AddLine(id2, new NonEmptyString("Dog Food Bag 2"),
            new(10), Money.CreateInDollars(100));
        
        // ************ ACT ************

        ICollection<EntityIdentity> result = sut.GetLineIds();

        // ************ ASSERT ************
        
        Assert.Equal(2, result.Count);

        Assert.Contains(result, r=>r == id1);
        Assert.Contains(result, r => r == id2);

    }

    [Theory]
    [InlineData("Cat food", "Dog foog", false)]
    [InlineData("Cat food", "Cat food", true)]
    [InlineData("Cat FOOd", "Cat food", true)]
    [InlineData("Cat food   ", "Cat food", true)]
    [InlineData("    Cat food", "Cat food", true)]
    public void Invariant_EachLineMustHaveADistinctProduct(
        string product1, string product2, bool shouldThrow)
    {
        // ************ ARRANGE ************

        SalesOrder? sut = CreateSut();

        AddLine(product1);

        // ************ ACT ************

        Exception? result = Record.Exception(() => { AddLine(product2); });

        // ************ ASSERT ************

        if (shouldThrow)
        {
            Assert.NotNull(result);
            Assert.IsType<InvalidEntityStateException>(result);
        }
        else
        {
            Assert.Null(result);
        }

        #region Details...

        void AddLine(string productName)
        {
            sut.AddLine(EntityIdentity.Random, new NonEmptyString(productName),
                new GreaterThanZeroInteger(10), Money.CreateInDollars(100));
        }

        #endregion
    }
}