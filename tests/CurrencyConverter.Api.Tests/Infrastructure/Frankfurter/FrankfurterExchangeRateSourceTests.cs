using System.Net;
using System.Text;

using CurrencyConverter.Api.Infrastructure.Frankfurter;
using CurrencyConverter.Core.Domain.Models;

namespace CurrencyConverter.Api.Tests.Infrastructure.Frankfurter;

public sealed class FrankfurterExchangeRateSourceTests
{
    private static readonly Uri BaseAddress = new("https://api.frankfurter.dev/v2/");

    [Fact]
    public async Task GetExchangeRateAsync_ReturnsExchangeRate_WhenResponseIsValid()
    {
        // Arrange
        string? requestedPath = null;

        var handler = new FakeHttpMessageHandler(request =>
        {
            requestedPath = request.RequestUri?.AbsolutePath;

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(
                    """
                    {
                        "date": "2026-10-30",
                        "base": "EUR",
                        "quote": "USD",
                        "rate": 1.16
                    }
                    """,
                    Encoding.UTF8,
                    "application/json")
            };
        });

        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = BaseAddress
        };

        var source = new FrankfurterExchangeRateSource(httpClient);

        var baseCurrency = new Currency("EUR", "Euro");
        var quoteCurrency = new Currency("USD", "United States Dollar");

        // Act
        ExchangeRate? result = await source.GetExchangeRateAsync(baseCurrency, quoteCurrency);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("EUR", result.BaseCurrency.Code);
        Assert.Equal("USD", result.QuoteCurrency.Code);
        Assert.Equal(1.16m, result.Rate);
        Assert.Equal("/v2/rate/eur/usd", requestedPath);
    }

    [Fact]
    public async Task GetExchangeRateAsync_ReturnsNull_WhenRateDoesNotExist()
    {
        // Arrange
        var handler = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.NotFound));

        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = BaseAddress
        };

        var source = new FrankfurterExchangeRateSource(httpClient);

        var baseCurrency = new Currency("EUR", "Euro");
        var quoteCurrency = new Currency("USD", "United States Dollar");

        // Act
        ExchangeRate? result = await source.GetExchangeRateAsync(baseCurrency, quoteCurrency);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetExchangeRateAsync_ThrowsHttpRequestException_WhenProviderReturnsServerError()
    {
        // Arrange
        var handler = new FakeHttpMessageHandler(_ =>
            new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
            {
                Content = new StringContent(
                    """{"status":503,"message":"Service unavailable"}""",
                    Encoding.UTF8,
                    "application/json")
            });

        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = BaseAddress
        };

        var source = new FrankfurterExchangeRateSource(httpClient);

        var baseCurrency = new Currency("EUR", "Euro");
        var quoteCurrency = new Currency("USD", "United States Dollar");

        // Act
        HttpRequestException exception = await Assert.ThrowsAsync<HttpRequestException>(() => source.GetExchangeRateAsync(baseCurrency, quoteCurrency));

        // Assert
        Assert.Equal(HttpStatusCode.ServiceUnavailable, exception.StatusCode);
    }
}
