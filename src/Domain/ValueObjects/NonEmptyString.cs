using Domain.Exceptions;

namespace Domain.ValueObjects;

public record NonEmptyString
{
    public NonEmptyString(string value)
    {
/*
 * We use objects to encapsulate values so we can guarantee (assert) their correctness. If the value is
 * invalid, an exception will be thrown. This is defensive programming. A method using these value objects will
 * have to make less work (no need to verify)
 */
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidEntityStateException("NonEmptyString must have a value.");

        Value = value;
    }

    public string Value { get; }

    public bool IsEquivalentTo(NonEmptyString other)
    {
        return StringsAreEquivalent(Value, other.Value);
    }

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