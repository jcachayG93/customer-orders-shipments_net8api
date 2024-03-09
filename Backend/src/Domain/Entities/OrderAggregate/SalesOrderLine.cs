namespace Domain.Entities.OrderAggregate;

/// <summary>
///     A sales order line
/// </summary>
public class SalesOrderLine
{
    // Current version of EF Core requires a parameterless constructor.
    private SalesOrderLine()
    {
        
    }
    // We use an internal constructor so the child entity can't be created by accident from the application layer
    // but using the methods from the aggregate root (Order) instead
    internal SalesOrderLine(
        Guid id, string product, int quantity, decimal unitPrice)
    {
        Id = id;
        Product = product;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public Guid Id { get; private set; }

    /*
     * Some properties have internal setters, so they can be changed by other classes within the Domain project (assembly),
     * this is not perfect but is simple. The only one that should change the SalesOrderLine values is the SalesOrder aggregate,
     * so we expect developers working in the domain project to be knowledgeable about Domain Driven Design tactics.
     */
    public string Product { get; internal set; } = null!;

    public int Quantity { get; internal set; }

    public decimal UnitPrice { get; internal set; }

    public decimal Total => Quantity * UnitPrice;
}