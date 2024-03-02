using Domain.Exceptions;
using Domain.ValueObjects;
#pragma warning disable CS8604 // Possible null reference argument.

namespace Domain.Tests.ValueObjects;

public class NonEmptyString_Tests
{
    [Theory]
    [InlineData("Hello", false)]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData("   ", true)]
    public void Constructor_AssertsValueNonEmpty(string? value, bool shouldThrow)
    {
        // ************ ARRANGE ************

        // ************ ACT ************

        Exception? result = Record.Exception(() => new NonEmptyString(value));

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

        NonEmptyString sut = new NonEmptyString("Hello");

        // ************ ASSERT ************
        
        Assert.Equal("Hello", sut.Value);

    }

    [Fact]
    public void ToString_ReturnsValue()
    {
        // ************ ARRANGE ************
        
        NonEmptyString sut = new NonEmptyString("Hello");

        // ************ ACT ************

        string? result = sut.ToString();
        
        // ************ ASSERT ************

        Assert.Equal("Hello", result);
    }

    [Theory]
    [InlineData("hello", "hello", true)]
    [InlineData("hello", "  hello", true)]
    [InlineData("hello", "hello  ", true)]
    [InlineData("hello", "heLLo", true)]
    [InlineData("hello", "hello2", false)]
    public void IsEquivalentTo_ObjectOverload_IgnoresCasingAndSurroundingWhiteSpace(
        string value1, string value2, bool shouldBeEquivalent)
    {
        // ************ ARRANGE ************

        NonEmptyString sut1 = new NonEmptyString(value1);
        NonEmptyString sut2 = new NonEmptyString(value2);

        // ************ ACT ************

        bool result = sut1.IsEquivalentTo(sut2);

        // ************ ASSERT ************
        
        Assert.Equal(shouldBeEquivalent, result);

    }
    
    
    [Theory]
    [InlineData("hello", "hello", true)]
    [InlineData("hello", "  hello", true)]
    [InlineData("hello", "hello  ", true)]
    [InlineData("hello", "heLLo", true)]
    [InlineData("hello", "hello2", false)]
    public void IsEquivalentTo_StringOverload_IgnoresCasingAndSurroundingWhiteSpace(
        string value1, string value2, bool shouldBeEquivalent)
    {
        // ************ ARRANGE ************

        NonEmptyString sut1 = new NonEmptyString(value1);

        // ************ ACT ************

        bool result = sut1.IsEquivalentTo(value2);

        // ************ ASSERT ************
        
        Assert.Equal(shouldBeEquivalent, result);

    }
}