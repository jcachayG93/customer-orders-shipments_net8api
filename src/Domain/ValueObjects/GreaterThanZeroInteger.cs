using Domain.Exceptions;

namespace Domain.ValueObjects;

/// <summary>
///     A positive, greater than zero integer number.
/// </summary>
public record GreaterThanZeroInteger
{
    public GreaterThanZeroInteger(int value)
    {
        /*
         * We use objects to encapsulate values so we can guarantee (assert) their correctness. If the value is
         * invalid, an exception will be thrown. This is defensive programming. A method using these value objects will
         * have to make less work (no need to verify)
         */
        if (value <= 0)
            throw new InvalidEntityStateException("GreaterThanZeroInteger value must be greater than zero.");

        Value = value;
    }

    public int Value { get; }
}