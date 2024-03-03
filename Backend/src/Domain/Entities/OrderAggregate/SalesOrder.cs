using Domain.Exceptions;
using Domain.ValueObjects;

namespace Domain.Entities.OrderAggregate;

/// <summary>
///     A Sales order, represents a purchase done by a customer from the supplier.
/// </summary>
public class SalesOrder : ISalesOrderRoot
{
    

    // Current version of EF Core requires a parameterless constructor.
    private SalesOrder()
    {
        
    }

    public SalesOrder(
        EntityIdentity id)
    {
        Id = id.Value;

        AssertInvariants();
    }
    /*
     * This is a Domain Driven Design Aggregate.
     * 1. It has a child entity (OrderLine)
     * 2. Encapsulates invariants (rules that must be true at all times)
     * 3. Encapsulates data (changes can be made only by public methods)
     * 4. Represents a real business object (an Sales Order)
     * 5. Has behavior (for example, it can calculate its total, it can add lines, etc)
     */

    /*
     * Fields have private setters, why?
     *  Because we will use EF Core as an ORM.
     * Ideally fields would be get only unless they are mutable, but, sometimes we need to compromise.w
     */
    public Guid Id { get; private set; }

    public IEnumerable<SalesOrderLine> SalesOrderLines { get; private set; } = new List<SalesOrderLine>();

    /// <summary>
    /// Determines if a line with the specified Id exists.
    /// </summary>
    public bool DoesLineExists(EntityIdentity lineId)
    {
        return SalesOrderLines.Any(l =>
            l.Id == lineId.Value);
    }

    public ICollection<EntityIdentity> GetLineIds()
    {
        return SalesOrderLines.Select(l =>
            new EntityIdentity(l.Id)).ToArray();
    }

    public decimal Total => SalesOrderLines.Sum(x => x.Total);

    /*
     * Here I have a property that is used mainly for testing. In general that is not a good idea, but there is a cost
     * benefit analysis I made. Having this property makes testing so much easier. I see little harm but a lot of benefit.
     * Setting this property would not change the aggregate behavior, it is more like having an indicator, something to read.
     */

    /// <summary>
    ///     True when AssertEntityStateIsValid was called.
    /// </summary>
    public bool AssertInvariantsWasCalled { get; set; }


    /// <summary>
    ///     Adds an order line
    /// </summary>
    public void AddLine(
        EntityIdentity id, NonEmptyString productName, GreaterThanZeroInteger quantity, Money unitPrice)
    {
        /*
         * Here we use objects as parameters, these objects have assertions for correctness in their constructors. For example,
         * NonEmptyString is guaranteed not to be null, empty or white space, so we do not have to check that, making this method
         * smaller. This is called defensive programming. We follow the fail fast principle (if the underlying value was invalid, the
         * client will get an exception thrown even before calling this method)
         */

        var line = new SalesOrderLine(
            id.Value, productName.Value, quantity.Value, unitPrice.Amount);
        
        ((SalesOrderLines as List<SalesOrderLine>)!).Add(line);

        AssertInvariants();
    }

    /// <summary>
    /// Removes the specified line, ignores if the line does not exist.
    /// </summary>
    public void RemoveLine(EntityIdentity lineId)
    {
        var match = SalesOrderLines.FirstOrDefault(l => l.Id == lineId.Value);

        if (match is not null)
        {
            ((SalesOrderLines as List<SalesOrderLine>)!).Remove(match);
            
            AssertInvariants();
        }
    }

    /// <summary>
    /// Updates an existing line. Ignores when the line does not exist.
    /// </summary>
    public void UpdateLine(EntityIdentity lineId, NonEmptyString product, GreaterThanZeroInteger quantity,
        Money unitPrice)
    {
        var line = SalesOrderLines.FirstOrDefault(l =>
            l.Id == lineId.Value);

        if (line is null)
        {
            throw new LineNotFoundException(Id, lineId.Value);
        }

        line.Product = product.Value;
        line.UnitPrice = unitPrice.Amount;
        line.Quantity = quantity.Value;
        AssertInvariants();
    }
    /// <summary>
    ///     Throws an exception if the entity state is not valid.
    /// </summary>
    private void AssertInvariants()
    {
        /*
         * Domain Driven Design (DDD). Invariants are business rules that must always be true.
         * I centralize invariants in a single method because, as per DDD, an aggregate must be correct at all times,
         * centralizing the invariants reduces the risk of a developer forgetting to make the assertion in a method.
         *
         */
        AssertInvariantsWasCalled = true;

        var byProductName = SalesOrderLines.GroupBy(x => x.Product.Trim().ToUpper());
        var firstDuplicate = byProductName.FirstOrDefault(g =>
            g.Count() > 1);

        if (firstDuplicate is not null)
        {
            /*
             * Performance vs readability: using ToArray could be done once and store the result in a variable,
             * but the more variables you have the less readable the code is. Performance hit is tiny.
             */

            var name1 = firstDuplicate.ToArray()[0];
            var name2 = firstDuplicate.ToArray()[1];

            throw new InvalidEntityStateException(
                "OrderLines product names must be unique. These two names are considered a " +
                $"duplication: {name1}, {name2}");
        }
    }
}