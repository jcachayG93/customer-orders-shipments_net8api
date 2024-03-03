using Domain.Exceptions;

namespace Domain.ValueObjects;

/// <summary>
///     Represents the identity of an Entity
/// </summary>
public record EntityIdentity
{
    public EntityIdentity(Guid value)
    {
        /*
         * We use objects to encapsulate values so we can guarantee (assert) their correctness. If the value is
         * invalid, an exception will be thrown. This is defensive programming. A method using these value objects will
         * have to make less work (no need to verify)
         */
        if (value == Guid.Empty)
            throw new InvalidEntityStateException(
                "EntityIdentity value can't be empty.");
        Value = value;
    }

    public Guid Value { get; }

    public static EntityIdentity Random => new(Guid.NewGuid());

    public override string ToString()
    {
        return Value.ToString();
    }
}