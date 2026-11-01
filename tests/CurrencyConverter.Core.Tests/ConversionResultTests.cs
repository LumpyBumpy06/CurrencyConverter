using System;
using System.Collections.Generic;
using System.Text;


using CurrencyConverter.Core.Domain.Models;

using Xunit;

namespace CurrencyConverter.Core.Tests;

public class ConversionResultTests
{
    [Fact]
    public void Constructor_WithValidValues_CreatesConversionResult()
    {
        var gbp = new Currency("GBP", "Pound Sterling");
        var eur = new Currency("EUR", "Euro");

        var originalMoney = new Money(100m, gbp);
        var exchangeRate = new ExchangeRate(gbp, eur, 1.17m, DateTimeOffset.UtcNow);
        var convertedMoney = new Money(117m, eur);


        var result = new ConversionResult(originalMoney, exchangeRate, convertedMoney);

        Assert.Equal(originalMoney, result.OriginalMoney);
        Assert.Equal(exchangeRate, result.Rate);
        Assert.Equal(convertedMoney, result.ConvertedMoney);
    }

    [Fact]
    public void Constructor_WithNullOriginalMoney_ThrowsArgumentNullException()
    {
        var gbp = new Currency("GBP", "Pound Sterling");
        var eur = new Currency("EUR", "Euro");

        var exchangeRate = new ExchangeRate(gbp, eur, 1.17m, DateTimeOffset.UtcNow);
        var convertedMoney = new Money(117m, eur);

        Assert.Throws<ArgumentNullException>(() => new ConversionResult(null!, exchangeRate, convertedMoney));
    }

    [Fact]
    public void Constructor_WithNullRate_ThrowsArgumentNullException()
    {
        var gbp = new Currency("GBP", "Pound Sterling");
        var originalMoney = new Money(100m, gbp);
        var convertedMoney = new Money(117m, new Currency("EUR", "Euro"));

        Assert.Throws<ArgumentNullException>(() => new ConversionResult(originalMoney, null!, convertedMoney));
    }

    [Fact]
    public void Constructor_WithNullConvertedMoney_ThrowsArgumentNullException()
    {
        var gbp = new Currency("GBP", "Pound Sterling");
        var eur = new Currency("EUR", "Euro");

        var originalMoney = new Money(100m, gbp);
        var exchangeRate = new ExchangeRate(gbp, eur, 1.17m, DateTimeOffset.UtcNow);

        Assert.Throws<ArgumentNullException>(() => new ConversionResult(originalMoney, exchangeRate, null!));
    }

    [Fact]
    public void Constructor_WhenOriginalCurrencyDoesNotMatchBaseCurrency_ThrowsArgumentException()
    {
        var usd = new Currency("USD", "US Dollar");
        var gbp = new Currency("GBP", "Pound Sterling");
        var eur = new Currency("EUR", "Euro");

        var originalMoney = new Money(100m, usd);
        var exchangeRate = new ExchangeRate(gbp, eur, 1.17m, DateTimeOffset.UtcNow);
        var convertedMoney = new Money(117m, eur);

        Assert.Throws<ArgumentException>(() => new ConversionResult(originalMoney, exchangeRate, convertedMoney));
    }

    [Fact]
    public void Constructor_WhenConvertedCurrencyDoesNotMatchQuoteCurrency_ThrowsArgumentException()
    {
        var gbp = new Currency("GBP", "Pound Sterling");
        var eur = new Currency("EUR", "Euro");
        var usd = new Currency("USD", "US Dollar");

        var originalMoney = new Money(100m, gbp);
        var exchangeRate = new ExchangeRate(gbp, eur, 1.17m, DateTimeOffset.UtcNow);
        var convertedMoney = new Money(117m, usd);

        Assert.Throws<ArgumentException>(() => new ConversionResult(originalMoney, exchangeRate, convertedMoney));
    }
}
