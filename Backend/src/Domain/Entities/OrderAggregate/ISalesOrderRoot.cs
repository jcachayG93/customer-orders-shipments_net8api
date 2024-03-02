using Domain.ValueObjects;

namespace Domain.Entities.OrderAggregate;

/// <summary>
/// The SalesOrder aggregate root
/// </summary>
public interface ISalesOrderRoot
{
    /*
     * I expose the Aggregate as an interface because:
     * 1. Depend upon abstractions not concretions (Bob C. Martin)
     * 2. Clients should interact with an aggregate only by its root methods and properties. Eric Evans.
     * 3. Easier to test (me)
     */
    Guid Id { get; }

    /// <summary>
    /// Determines if a line with the specified Id exists.
    /// </summary>
    bool DoesLineExists(EntityIdentity lineId);

    /// <summary>
    /// Gets all the line Id values
    /// </summary>
    /// <returns></returns>
    ICollection<EntityIdentity> GetLineIds();

    /// <summary>
    ///     Adds an order line
    /// </summary>
    void AddLine(
        EntityIdentity id, NonEmptyString productName, GreaterThanZeroInteger quantity, Money unitPrice);

    /// <summary>
    /// Removes the specified line, ignores if the line does not exist.
    /// </summary>
    void RemoveLine(EntityIdentity lineId);

    /// <summary>
    /// Updates an existing line. Ignores when the line does not exist.
    /// </summary>
    void UpdateLine(EntityIdentity lineId, NonEmptyString product, GreaterThanZeroInteger quantity,
        Money unitPrice);
}