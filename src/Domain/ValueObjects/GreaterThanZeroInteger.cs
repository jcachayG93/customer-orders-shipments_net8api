using Domain.Exceptions;

namespace Domain.ValueObjects;

/// <summary>
/// A positive, greater than zero integer number.
/// </summary>
public record GreaterThanZeroInteger
{
    public int Value { get; }

    public GreaterThanZeroInteger(int value)
    {
        if (value <= 0)
        {
            throw new InvalidEntityStateException("GreaterThanZeroInteger value must be greater than zero.");
        }

        Value = value;
    }
}