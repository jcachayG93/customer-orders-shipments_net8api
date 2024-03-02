using Domain.Exceptions;

namespace Domain.ValueObjects;

public record Money
{
    private Money(Currency currency, decimal amount)
    {
        /*
         * We use objects to encapsulate values so we can guarantee (assert) their correctness. If the value is
         * invalid, an exception will be thrown. This is defensive programming. A method using these value objects will
         * have to make less work (no need to verify)
         */
        if (amount < 0) throw new InvalidEntityStateException("Money amount must be positive.");

        Currency = currency;
        Amount = amount;
    }

    public Currency Currency { get; private set; }

    public decimal Amount { get; private set; }

    public static Money CreateInDollars(decimal amount)
    {
        return new Money(Currency.Dollars, amount);
    }
}

public enum Currency
{
    Dollars
}