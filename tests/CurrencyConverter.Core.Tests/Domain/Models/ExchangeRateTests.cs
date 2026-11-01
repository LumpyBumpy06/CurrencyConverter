using System;
using System.Collections.Generic;
using System.Text;

using CurrencyConverter.Core.Domain.Models;

using Xunit;

namespace CurrencyConverter.Core.Tests.Domain.Models;

public class ExchangeRateTests
{
    [Fact]
    public void Constructor_WithValidValues_CreatesExchangeRate()
    {
        var gbp = new Currency("GBP", "Pound Sterling");
        var eur = new Currency("EUR", "Euro");
        const decimal rate = 1.17m;
        var retrievedAt = DateTimeOffset.UtcNow;

        var exchangeRate = new ExchangeRate(gbp, eur, rate, retrievedAt);

        Assert.Equal(gbp, exchangeRate.BaseCurrency);
        Assert.Equal(eur, exchangeRate.QuoteCurrency);
        Assert.Equal(rate, exchangeRate.Rate);
        Assert.Equal(retrievedAt, exchangeRate.RetrievedAtUTC);
    }

    [Fact]
    public void Constructor_WithNullBaseCurrency_ThrowsArgumentNullException()
    {
        var eur = new Currency("EUR", "Euro");
        Assert.Throws<ArgumentNullException>(() => new ExchangeRate(null!, eur, 1.17m, DateTimeOffset.UtcNow));
    }

    [Fact]
    public void Constructor_WithNullQuoteCurrency_ThrowsArgumentNullException()
    {
        var gbp = new Currency("GBP", "Pound Sterling");
        Assert.Throws<ArgumentNullException>(() => new ExchangeRate(gbp, null!, 1.17m, DateTimeOffset.UtcNow));
    }

    [Fact]
    public void Constructor_WithZeroRate_ThrowsArgumentOutOfRangeException()
    {
        var gbp = new Currency("GBP", "Pound Sterling");
        var eur = new Currency("EUR", "Euro");
        Assert.Throws<ArgumentOutOfRangeException>(() => new ExchangeRate(gbp, eur, 0m, DateTimeOffset.UtcNow));
    }

    [Fact]
    public void Constructor_WithNegativeRate_ThrowsArgumentOutOfRangeException()
    {
        var gbp = new Currency("GBP", "Pound Sterling");
        var eur = new Currency("EUR", "Euro");
        Assert.Throws<ArgumentOutOfRangeException>(() => new ExchangeRate(gbp, eur, -1m, DateTimeOffset.UtcNow));
    }

    [Fact]
    public void Constructor_WithSameCurrencyAndRateOfOne_CreatesExchangeRate()
    {
        var gbp = new Currency("GBP", "Pound Sterling");
        var exchangeRate = new ExchangeRate(gbp, gbp, 1m, DateTimeOffset.UtcNow);

        Assert.Equal(gbp, exchangeRate.BaseCurrency);
        Assert.Equal(gbp, exchangeRate.QuoteCurrency);
        Assert.Equal(1m, exchangeRate.Rate);
    }

    [Fact]
    public void Constructor_WithSameCurrencyAndRateOtherThanOne_ThrowsArgumentException()
    {
        var gbp = new Currency("GBP", "Pound Sterling");
        Assert.Throws<ArgumentException>(() => new ExchangeRate(gbp, gbp, 1.5m, DateTimeOffset.UtcNow));
    }

    [Fact]
    public void Constructor_WithNonUtcTimestamp_NormalisesTimestampToUtc()
    {
        var gbp = new Currency("GBP", "Pound Sterling");
        var eur = new Currency("EUR", "Euro");
        var timestamp = new DateTimeOffset(2026, 11, 1, 12, 0, 0, TimeSpan.FromHours(2));

        var exchangeRate = new ExchangeRate(gbp, eur, 1.17m, timestamp);

        Assert.Equal(timestamp.ToUniversalTime(), exchangeRate.RetrievedAtUTC);
        Assert.Equal(TimeSpan.Zero, exchangeRate.RetrievedAtUTC.Offset);
    }
}
