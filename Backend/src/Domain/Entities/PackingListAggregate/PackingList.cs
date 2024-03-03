using Domain.ValueObjects;

namespace Domain.Entities.PackingListAggregate;

/// <summary>
/// A list of contents from an order that need to be packed in a container so it can be shipped to the customer.
/// </summary>
public class PackingList
{
    public Guid Id { get; private set; }

    /// <summary>
    /// This is not an association but a reference to the Order (there is no association in a database table but,
    /// conceptually, the packing list belongs to an order)
    /// </summary>
    public Guid OrderId { get; private set; }

    public IEnumerable<PackingListLine> Lines { get; private set; } = new List<PackingListLine>();
    
    // For now, ef core requies a parameterless constructor, but note this one is private.
    private PackingList()
    {
        
    }

    public PackingList(EntityIdentity id, EntityIdentity orderId)
    {
        Id = id.Value;
        OrderId = orderId.Value;
    }

    public void AddLine(EntityIdentity lineId, NonEmptyString product, GreaterThanZeroInteger quantity)
    {
        var line = new PackingListLine(
            lineId.Value, product.Value, quantity.Value);
        
        ((Lines as List<PackingListLine>)!).Add(line);
    }
}