using System;
using System.Collections.Generic;
using System.Text;

namespace CurrencyConverter.Core.Domain.Models;

public sealed record Money
{
    public decimal Amount { get; }
    public Currency Currency { get; }

    public Money(decimal amount, Currency currency)
    {
        ArgumentNullException.ThrowIfNull(currency);

        Amount = amount;
        Currency = currency;
    }
}
