using Domain.Exceptions;
using Domain.ValueObjects;

namespace Domain.Tests.ValueObjects;

public class Money_Tests
{
    [Theory]
    [InlineData(-1, true)]
    [InlineData(0, false)]
    [InlineData(1, false)]
    public void CreateDollars_AssertsAmountPositive(
        decimal value, bool shouldThrow)
    {
        // ************ ARRANGE ************

        // ************ ACT ************

        var result = Record.Exception(() => Money.CreateInDollars(value));

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

    }
    
    [Fact]
    public void CanCreateDollars()
    {
        // ************ ARRANGE ************

        // ************ ACT ************

        var result = Money.CreateInDollars(100M);
        
        // ************ ASSERT ************

        Assert.Equal(100M, result.Amount);
    }
}