using CurrencyConverter.Core.Application.Exceptions;
using CurrencyConverter.Core.Application.Interfaces;
using CurrencyConverter.Core.Domain.Models;

namespace CurrencyConverter.Core.Application.Services;

public sealed class CurrencyConversionService
{
    // readonly is const but can be assigned at runtime
    private readonly IExchangeRateSource _exchangeRateSource;

    public CurrencyConversionService(IExchangeRateSource exchangeRateSource)
    {
        ArgumentNullException.ThrowIfNull(exchangeRateSource);

        _exchangeRateSource = exchangeRateSource;
    }

    public async Task<ConversionResult> ConvertAsync(Money originalMoney, Currency targetCurrency)
    {
        ArgumentNullException.ThrowIfNull(originalMoney);
        ArgumentNullException.ThrowIfNull(targetCurrency);

        if (originalMoney.Amount <= 0m)
        {
            throw new ArgumentOutOfRangeException(nameof(originalMoney), originalMoney.Amount, "The amount to convert must be greater than zero.");
        }

        var exchangeRate = await _exchangeRateSource.GetExchangeRateAsync(originalMoney.Currency, targetCurrency);

        if (exchangeRate is null)
        {
            throw new ExchangeRateNotFoundException(originalMoney.Currency, targetCurrency);
        }

        decimal convertedAmount;

        try
        {
            convertedAmount = originalMoney.Amount * exchangeRate.Rate;
        }
        catch (OverflowException ex)
        {
            throw new ConversionAmountOutOfRangeException(originalMoney.Currency, targetCurrency, ex);
        }

        var convertedMoney = new Money(convertedAmount, targetCurrency);

        return new ConversionResult(originalMoney, exchangeRate, convertedMoney);
    }
}
