using Domain.Exceptions;

namespace Domain.ValueObjects;

/// <summary>
/// Represents a text value, for example, a name.
/// </summary>
public record NonEmptyString
{
    public string Value { get; }
    
    public NonEmptyString(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidEntityStateException("NonEmptyString must have a value.");
        }

        Value = value;
    }

    /// <summary>
    /// Determines if this is equivalent to another, ignoring case and surrounding white space.
    /// </summary>
    public bool IsEquivalentTo(NonEmptyString other)
    {
        return StringsAreEquivalent(Value, other.Value);
    }

    
    /// <summary>
    /// Determines if this is equivalent to another, ignoring case and surrounding white space.
    /// </summary>
    public bool IsEquivalentTo(string other)
    {
        return StringsAreEquivalent(Value, other);
    }

    private bool StringsAreEquivalent(string value1, string value2)
    {
        return string.Equals(value1.Trim(), value2.Trim(), StringComparison.InvariantCultureIgnoreCase);
    }

    public override string ToString()
    {
        return Value;
    }
}