using Domain.Entities.PackingListAggregate;
using Domain.ValueObjects;

namespace Domain.Tests.Entities.PackingListAggregate;

public class PackingListTests
{
    [Fact]
    public void CanCreate()
    {
        // ************ ARRANGE ************

        EntityIdentity id = EntityIdentity.Random;

        EntityIdentity orderId = EntityIdentity.Random;
        
        // ************ ACT ************

        PackingList result = new PackingList(id, orderId);

        // ************ ASSERT ************

        Assert.Equal(id.Value, result.Id);
        Assert.Equal(orderId.Value, result.OrderId);
    }

    [Fact]
    public void CanAddLine()
    {
        // ************ ARRANGE ************

        PackingList sut = new PackingList(EntityIdentity.Random, EntityIdentity.Random);

        EntityIdentity line1Id = EntityIdentity.Random;
        EntityIdentity line2Id = EntityIdentity.Random;
        
        // ************ ACT ************
        
        sut.AddLine(line1Id, new("Bolt"), new(10));
        sut.AddLine(line2Id, new("Wire roll"), new(20));

        // ************ ASSERT ************

        Assert.Equal(2, sut.Lines.Count());
        Assert.Contains(sut.Lines, l =>
            l.Id == line1Id.Value
            && l.Product == "Bolt"
            && l.Quantity == 10
        );
        Assert.Contains(sut.Lines, l =>
            l.Id == line2Id.Value
            && l.Product == "Wire roll"
            && l.Quantity == 20
        );
    }
    
}