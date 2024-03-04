using FluentValidation.Results;
using WebApi.Features.Orders.OrdersAddRemoveLines;

namespace WebApi.Tests.Features.Orders.OrdersAddRemoveLines;

public class OrdersAddRemoveLinesCommandValidatorTests
{
    private OrdersAddRemoveLinesCommandValidator CreateSut()
    {
        return new();
    }

    private OrdersAddRemoveLinesCommand CreateCommand(
        params OrdersAddRemoveLinesCommand.OrderLineDto[] lines
    )
    {
        return new()
        {
            OrderId = Guid.NewGuid(),
            Lines = lines
        };
    }

    [Theory]
    [InlineData(-1, false)]
    [InlineData(0, false)]
    [InlineData(1, true)]
    public void ValidatesLinesQuantityGreaterThanZero(
        int quantity, bool shouldBeValid)
    {
        // ************ ARRANGE ************


        OrdersAddRemoveLinesCommand.OrderLineDto line = new()
        {
            OrderLineId = Guid.NewGuid(),
            Product = "Bolt type A",
            Quantity = quantity,
            UnitPrice = 10M
        };

        OrdersAddRemoveLinesCommand command = CreateCommand(line);

        OrdersAddRemoveLinesCommandValidator sut = CreateSut();

        // ************ ACT ************

        ValidationResult? result = sut.Validate(command);

        // ************ ASSERT ************
        
        Assert.Equal(shouldBeValid, result.IsValid);

        if (!shouldBeValid)
        {
            Assert.Equal("The SalesOrder Line Quantity must be greater than zero.",
                result.Errors.First().ErrorMessage);
        }
    }

    [Theory]
    [InlineData(-1, false)]
    [InlineData(0, true)]
    [InlineData(1, true)]
    public void ValidatesUnitPricePositive(
        decimal unitPrice, bool shouldBeValid)
    {
        // ************ ARRANGE ************

        OrdersAddRemoveLinesCommand.OrderLineDto line = new()
        {
            OrderLineId = Guid.NewGuid(),
            Product = "Bolt A",
            Quantity = 10,
            UnitPrice = unitPrice
        };

        OrdersAddRemoveLinesCommand command = CreateCommand(line);

        OrdersAddRemoveLinesCommandValidator sut = CreateSut();
        
        // ************ ACT ************

        ValidationResult? result = sut.Validate(command);
        
        // ************ ASSERT ************
        
        Assert.Equal(shouldBeValid, result.IsValid);

        if (!shouldBeValid)
        {
            Assert.Equal("The SalesOrder Line Unit Price must be positive.",
                result.Errors.First().ErrorMessage);
        }

    }

    [Theory]
    [InlineData("Bolt A", "Bolt B", true)]
    [InlineData("Bolt A  ", "   Bolt A", false)]
    [InlineData("Bolt A", "bolt a", false)]
    public void ValidatesProductNameUnique(
        string productName1, string productName2, bool shouldBeValid)
    {
        // ************ ARRANGE ************

        OrdersAddRemoveLinesCommand.OrderLineDto line1 = CreateLine(productName1);
        OrdersAddRemoveLinesCommand.OrderLineDto line2 = CreateLine(productName2);

        OrdersAddRemoveLinesCommand command = CreateCommand(line1, line2);

        OrdersAddRemoveLinesCommandValidator sut = CreateSut();

        // ************ ACT ************

        ValidationResult? result = sut.Validate(command);

        // ************ ASSERT ************
        
        Assert.Equal(shouldBeValid, result.IsValid);

        if (!shouldBeValid)
        {
            Assert.Equal("Each SalesOrder line must have a unique product name.",
                result.Errors.First().ErrorMessage);
        }
        
        #region Details
        
        OrdersAddRemoveLinesCommand.OrderLineDto CreateLine(
            string productName)
        {
            return new()
            {
                OrderLineId = Guid.NewGuid(),
                Product = productName,
                Quantity = 10,
                UnitPrice = 10M
            };
        }
        
        #endregion
        
        
    }
}