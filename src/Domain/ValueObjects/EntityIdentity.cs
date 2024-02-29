using Domain.Exceptions;

namespace Domain.ValueObjects;

/// <summary>
/// Represents the identity of an Entity
/// </summary>
public record EntityIdentity
{
    public Guid Value { get; }
    public EntityIdentity(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new InvalidEntityStateException(
                "EntityIdentity value can't be empty.");
        }
        Value = value;
    }

    public static EntityIdentity Random => new(Guid.NewGuid());

    public override string ToString()
    {
        return Value.ToString();
    }
}