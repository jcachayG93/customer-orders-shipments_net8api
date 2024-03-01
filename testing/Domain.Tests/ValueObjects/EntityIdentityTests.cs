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

        Exception? result = Record.Exception(() =>
            new EntityIdentity(Guid.Empty));

        // ************ ASSERT ************

        Assert.NotNull(result);
        Assert.IsType<InvalidEntityStateException>(result);

    }

    [Fact]
    public void Constructor_SetsValue()
    {
        // ************ ARRANGE ************

        Guid id = Guid.NewGuid();
        
        // ************ ACT ************

        EntityIdentity sut = new EntityIdentity(id);

        // ************ ASSERT ************
        
        Assert.Equal(id, sut.Value);

    }

    
    [Fact]
    public void CanCreateWithRandomValue()
    {
        // ************ ARRANGE ************

        // ************ ACT ************

        EntityIdentity sut = EntityIdentity.Random;

        // ************ ASSERT ************
        
        Assert.NotEqual(Guid.Empty, sut.Value);

    }

    [Fact]
    public void ToString_ReturnsValue()
    {
        // ************ ARRANGE ************

        EntityIdentity sut = new EntityIdentity(Guid.NewGuid());
        
        // ************ ACT ************

        string result = sut.ToString();
        
        // ************ ASSERT ************

        Assert.Equal(sut.Value.ToString(), result);
    }
}