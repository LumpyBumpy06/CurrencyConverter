namespace CurrencyConverter.Core.Domain.Models;

public sealed record ExchangeRate
{
    public Currency BaseCurrency { get; }
    public Currency QuoteCurrency { get; }
    public decimal Rate { get; }
    public DateTimeOffset RetrievedAtUTC { get; }

    public ExchangeRate(
        Currency baseCurrency,
        Currency quoteCurrency,
        decimal rate,
        DateTimeOffset retrievedAtUTC)
    {
        ArgumentNullException.ThrowIfNull(baseCurrency);
        ArgumentNullException.ThrowIfNull(quoteCurrency);

        if (rate <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(rate), "Exchange rate must be greater than zero.");
        }

        if (baseCurrency.Code == quoteCurrency.Code && rate != 1m)
        {
            throw new ArgumentException("An exchange rate between the same currency must be 1.", nameof(rate));
        }

        BaseCurrency = baseCurrency;
        QuoteCurrency = quoteCurrency;
        Rate = rate;
        RetrievedAtUTC = retrievedAtUTC.ToUniversalTime();
    }
}
