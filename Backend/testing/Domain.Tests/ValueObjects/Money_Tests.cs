using Domain.Exceptions;
using Domain.ValueObjects;

namespace Domain.Tests.ValueObjects;

public class Money_Tests
{
    /*
     * In test projects, I always specify types explicitly (instead of using var) because a test is also
     * documentation. When reading a PR in a browser you normally wont see the type next to var.
     */
    
    [Theory]
    [InlineData(-1, true)]
    [InlineData(0, false)]
    [InlineData(1, false)]
    public void CreateDollars_AssertsAmountPositive(
        decimal value, bool shouldThrow)
    {
        // ************ ARRANGE ************

        // ************ ACT ************

        Exception? result = Record.Exception(() => Money.CreateInDollars(value));

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

        Money result = Money.CreateInDollars(100M);

        // ************ ASSERT ************

        Assert.Equal(100M, result.Amount);
    }
}