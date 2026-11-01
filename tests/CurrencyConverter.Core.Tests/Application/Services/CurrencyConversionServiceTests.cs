using CurrencyConverter.Core.Application.Exceptions;
using CurrencyConverter.Core.Application.Interfaces;
using CurrencyConverter.Core.Application.Services;
using CurrencyConverter.Core.Domain.Models;

namespace CurrencyConverter.Core.Tests.Application.Services;

public class CurrencyConversionServiceTests
{
    private static readonly Currency Gbp = new("GBP", "British Pound");
    private static readonly Currency Eur = new("EUR", "Euro");

    [Fact]
    public async Task ConvertAsync_WithValidAmount_ReturnsCorrectConversion()
    {
        var exchangeRate = new ExchangeRate(Gbp, Eur, 1.17m, DateTimeOffset.UtcNow);

        var source = new FakeExchangeRateSource(exchangeRate);
        var converter = new CurrencyConversionService(source);

        var originalMoney = new Money(100m, Gbp);

        ConversionResult result = await converter.ConvertAsync(originalMoney, Eur);

        Assert.Equal(100m, result.OriginalMoney.Amount);
        Assert.Equal(Gbp, result.OriginalMoney.Currency);
        Assert.Equal(1.17m, result.Rate.Rate);
        Assert.Equal(117m, result.ConvertedMoney.Amount);
        Assert.Equal(Eur, result.ConvertedMoney.Currency);
    }

    [Fact]
    public async Task ConvertAsync_WithDifferentRate_UsesProvidedRate()
    {
        var exchangeRate = new ExchangeRate(Gbp, Eur, 1.25m, DateTimeOffset.UtcNow);

        var source = new FakeExchangeRateSource(exchangeRate);
        var converter = new CurrencyConversionService(source);

        var originalMoney = new Money(80m, Gbp);

        var result = await converter.ConvertAsync(originalMoney, Eur);

        Assert.Equal(100m, result.ConvertedMoney.Amount);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public async Task ConvertAsync_WithNonPositiveAmount_ThrowsArgumentOutOfRangeException(decimal amount)
    {
        var source = new FakeExchangeRateSource(null);
        var converter = new CurrencyConversionService(source);

        var originalMoney = new Money(amount, Gbp);

        var act = () => converter.ConvertAsync(originalMoney, Eur);

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(act);
    }

    [Fact]
    public async Task ConvertAsync_WhenExchangeRateUnavailable_ThrowsExchangeRateNotFoundException()
    {
        var source = new FakeExchangeRateSource(null);
        var converter = new CurrencyConversionService(source);

        var originalMoney = new Money(100m, Gbp);

        var act = () => converter.ConvertAsync(originalMoney, Eur);

        await Assert.ThrowsAsync<ExchangeRateNotFoundException>(act);
    }

    [Fact]
    public async Task ConvertAsync_RequestsRateUsingCorrectCurrencies()
    {
        var exchangeRate = new ExchangeRate(Gbp, Eur, 1.17m, DateTimeOffset.UtcNow);

        var source = new FakeExchangeRateSource(exchangeRate);
        var converter = new CurrencyConversionService(source);

        var originalMoney = new Money(100m, Gbp);

        await converter.ConvertAsync(originalMoney, Eur);

        Assert.Equal(Gbp, source.RequestedBaseCurrency);
        Assert.Equal(Eur, source.RequestedQuoteCurrency);
    }

    [Fact]
    public async Task ConvertAsync_WithNullOriginalMoney_ThrowsArgumentNullException()
    {
        var source = new FakeExchangeRateSource(null);
        var converter = new CurrencyConversionService(source);

        var act = () => converter.ConvertAsync(null!, Eur);

        await Assert.ThrowsAsync<ArgumentNullException>(act);
    }

    [Fact]
    public async Task ConvertAsync_WithNullTargetCurrency_ThrowsArgumentNullException()
    {
        var source = new FakeExchangeRateSource(null);
        var converter = new CurrencyConversionService(source);

        var originalMoney = new Money(100m, Gbp);

        var act = () => converter.ConvertAsync(originalMoney, null!);

        await Assert.ThrowsAsync<ArgumentNullException>(act);
    }

    [Fact]
    public async Task ConvertAsync_WithSameCurrency_ReturnsOriginalAmount()
    {
        var exchangeRate = new ExchangeRate(Gbp, Gbp, 1m, DateTimeOffset.UtcNow);

        var source = new FakeExchangeRateSource(exchangeRate);
        var converter = new CurrencyConversionService(source);

        var originalMoney = new Money(100m, Gbp);

        var result = await converter.ConvertAsync(originalMoney, Gbp);

        Assert.Equal(100m, result.ConvertedMoney.Amount);
        Assert.Equal(Gbp, result.ConvertedMoney.Currency);
        Assert.Equal(originalMoney, result.ConvertedMoney);
    }

    private sealed class FakeExchangeRateSource(ExchangeRate? exchangeRate) : IExchangeRateSource
    {
        public Currency? RequestedBaseCurrency { get; private set; }

        public Currency? RequestedQuoteCurrency { get; private set; }

        public Task<ExchangeRate?> GetExchangeRateAsync(Currency baseCurrency, Currency quoteCurrency)
        {
            RequestedBaseCurrency = baseCurrency;
            RequestedQuoteCurrency = quoteCurrency;

            return Task.FromResult(exchangeRate);
        }
    }
}
