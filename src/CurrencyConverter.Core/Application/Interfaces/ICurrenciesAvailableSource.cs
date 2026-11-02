using CurrencyConverter.Core.Domain.Models;

namespace CurrencyConverter.Core.Application.Interfaces;

/// <summary>
/// Provides the currencies supported by an exchange-rate source.
/// </summary>
public interface ICurrenciesAvailableSource
{
    Task<IReadOnlyList<Currency>> GetCurrenciesAsync();
}
