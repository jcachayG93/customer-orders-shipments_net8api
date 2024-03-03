namespace Domain.Entities.PackingListAggregate;

/// <summary>
/// Details for one particular product
/// </summary>
public class PackingListLine
{
    public Guid Id { get; private set; }

    public string Product { get; private set; }

    public int Quantity { get; private set; }

    // For now, ef core requies a parameterless constructor, but note this one is private.
    private PackingListLine()
    {
        
    }
    
    public PackingListLine(Guid id, string product, int quantity)
    {
        Id = id;
        Product = product;
        Quantity = quantity;
    }
}