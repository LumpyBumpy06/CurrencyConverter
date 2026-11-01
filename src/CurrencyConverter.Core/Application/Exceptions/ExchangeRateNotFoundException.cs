using System;
using System.Collections.Generic;
using System.Text;

using CurrencyConverter.Core.Domain.Models;

namespace CurrencyConverter.Core.Application.Exceptions;

public sealed class ExchangeRateNotFoundException : Exception
{
    public ExchangeRateNotFoundException(Currency baseCurrency, Currency quoteCurrency) : base(
            $"No exchange rate is available from " +
            $"{baseCurrency.Code} to {quoteCurrency.Code}."
        )
    { }
}

