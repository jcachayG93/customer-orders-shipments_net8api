using Domain.Exceptions;
using Domain.ValueObjects;

namespace Domain.Tests.ValueObjects;

public class GreaterThanZeroIntegerTests
{
    /*
     * In test projects, I always specify types explicitly (instead of using var) because a test is also
     * documentation. When reading a PR in a browser you normally wont see the type next to var.
     */
    
    [Theory]
    [InlineData(-1, true)]
    [InlineData(0, true)]
    [InlineData(1, false)]
    [InlineData(int.MaxValue, false)]
    public void Constructor_AssertsVGreaterThanZeroValue(
        int value, bool shouldThrow)
    {
        // ************ ARRANGE ************

        // ************ ACT ************

        Exception? result = Record.Exception(() => new GreaterThanZeroInteger(value));

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
    public void Constructor_SetsValue()
    {
        // ************ ARRANGE ************

        // ************ ACT ************

        GreaterThanZeroInteger sut = new GreaterThanZeroInteger(10);

        // ************ ASSERT ************

        Assert.Equal(10, sut.Value);
    }
}