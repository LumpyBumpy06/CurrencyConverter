using CurrencyConverter.Core.Domain.Models;

namespace CurrencyConverter.Core.Application.Interfaces;

/// <summary>
/// Provides exchange rates between currencies.
/// </summary>
public interface IExchangeRateSource
{
    /// <summary>
    /// Gets the exchange rate from a base currency to a quote currency.
    /// </summary>
    /// <returns>
    /// The exchange rate, or null if no rate is available for the requested pair.
    /// </returns>
    Task<ExchangeRate?> GetExchangeRateAsync(
        Currency baseCurrency,
        Currency quoteCurrency
    );
}
