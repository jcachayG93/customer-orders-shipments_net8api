using Domain.Exceptions;
using Domain.ValueObjects;

namespace Domain.Entities.OrderAggregate;

/// <summary>
///     A Sales order, represents a purchase done by a customer from the supplier.
/// </summary>
public class SalesOrder
{
    /*
     * this is the backing field for the Lines property, this way the field is a list (class) but the
     * client will read the Lines property, which is an abstraction, enforcing: Depend upon abstractions, not upon concretions.
     * Why? From inside the class, the List has many handy methods we can use to modify it, but we would not like to allow an external
     * client doing so, because an aggregate must be altered by its root methods only.
     *
     * From a clients perspective, they receive a collection, and they can use it as any concrete type they need, they can read
     * but can't modify.
     *
     * Now, this is not 100% true. They could still cast the ICollection to a List, but that requires an extra effort which would not
     * be accidental. To me, the idea, is to prevent errors.
     */
    private readonly List<SalesOrderLine> _lines = new();

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

    public ICollection<SalesOrderLine> Lines => _lines;

    /// <summary>
    /// Determines if a line with the specified Id exists.
    /// </summary>
    public bool DoesLineExists(EntityIdentity lineId)
    {
        return _lines.Any(l =>
            l.Id == lineId.Value);
    }

    public decimal Total => _lines.Sum(x => x.Total);

    /*
     * Here I have a property that is used mainly for testing. In general that is not a good idea, but there is a cost
     * benefit analysis I made. Having this property makes testing so much easier. I see little harm but a lot of benefit.
     * Setting this property would not change the aggregate behavior, it is more like having an indicator, something to read.
     */

    /// <summary>
    ///     True when AssertEntityStateIsValid was called.
    /// </summary>
    public bool AssertInvariantsWasCalled { get; private set; }


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

        _lines.Add(line);

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

        var byProductName = _lines.GroupBy(x => x.Product.Trim().ToUpper());
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