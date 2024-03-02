using Domain.Entities.OrderAggregate;
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

        // ************ ACT ************

        sut.AddLine(lineId, new NonEmptyString("Dog Food Bag"),
            new GreaterThanZeroInteger(10), Money.CreateInDollars(100));

        // ************ ASSERT ************

        Assert.Equal(1, sut.Lines.Count);

        Assert.Contains(sut.Lines, l =>
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

        decimal result = sut.Lines.First().Total;

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