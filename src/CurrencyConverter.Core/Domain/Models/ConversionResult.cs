namespace CurrencyConverter.Core.Domain.Models;

public sealed record ConversionResult
{
    public Money OriginalMoney { get; }
    public ExchangeRate Rate { get; }
    public Money ConvertedMoney { get; }

    public ConversionResult(
        Money originalMoney,
        ExchangeRate rate,
        Money convertedMoney)
    {
        ArgumentNullException.ThrowIfNull(originalMoney);
        ArgumentNullException.ThrowIfNull(rate);
        ArgumentNullException.ThrowIfNull(convertedMoney);

        if (originalMoney.Currency.Code != rate.BaseCurrency.Code)
        {
            throw new ArgumentException("The original money currency must match the exchange rate base currency.", nameof(originalMoney));
        }

        if (convertedMoney.Currency.Code != rate.QuoteCurrency.Code)
        {
            throw new ArgumentException("The converted money currency must match the exchange rate quote currency.", nameof(convertedMoney));
        }

        OriginalMoney = originalMoney;
        Rate = rate;
        ConvertedMoney = convertedMoney;
    }
}
