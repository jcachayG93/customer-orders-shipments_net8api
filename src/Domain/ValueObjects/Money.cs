using Domain.Exceptions;

namespace Domain.ValueObjects;

public record Money
{
    public Currency Currency { get; private set; }

    public decimal Amount { get; private set; }

    private Money(Currency currency, decimal amount)
    {
        if (amount < 0)
        {
            throw new InvalidEntityStateException("Money amount must be positive.");
        }

        Currency = currency;
        Amount = amount;
    }

    public static Money CreateInDollars(decimal amount)
    {
        return new(Currency.Dollars, amount);
    }
}

public enum Currency
{
    Dollars
}