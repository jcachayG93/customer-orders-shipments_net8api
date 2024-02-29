using Domain.Exceptions;
using Domain.ValueObjects;

namespace Domain.Tests.ValueObjects;

public class EntityIdentityTests
{
    [Fact]
    public void Constructor_AssertsValueNonEmpty()
    {
        // ************ ARRANGE ************

        // ************ ACT ************

        var result = Record.Exception(() =>
            new EntityIdentity(Guid.Empty));

        // ************ ASSERT ************

        Assert.NotNull(result);
        Assert.IsType<InvalidEntityStateException>(result);

    }

    [Fact]
    public void Constructor_SetsValue()
    {
        // ************ ARRANGE ************

        var id = Guid.NewGuid();
        
        // ************ ACT ************

        var sut = new EntityIdentity(id);

        // ************ ASSERT ************
        
        Assert.Equal(id, sut.Value);

    }

    
    [Fact]
    public void CanCreateWithRandomValue()
    {
        // ************ ARRANGE ************

        // ************ ACT ************

        var sut = EntityIdentity.Random;

        // ************ ASSERT ************
        
        Assert.NotEqual(Guid.Empty, sut.Value);

    }

    [Fact]
    public void ToString_ReturnsValue()
    {
        // ************ ARRANGE ************

        var sut = new EntityIdentity(Guid.NewGuid());
        
        // ************ ACT ************

        var result = sut.ToString();
        
        // ************ ASSERT ************

        Assert.Equal(sut.Value.ToString(), result);
    }
}