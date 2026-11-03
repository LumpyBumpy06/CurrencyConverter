using CurrencyConverter.Core.Domain.Models;

namespace CurrencyConverter.Core.Application.Exceptions;

public sealed class ConversionAmountOutOfRangeException : Exception
{
    public ConversionAmountOutOfRangeException(Currency sourceCurrency, Currency targetCurrency, Exception innerException) : base(
            $"The converted amount from {sourceCurrency.Code} to {targetCurrency.Code} exceeds the supported decimal range.",
            innerException
        )
    {
    }
}
