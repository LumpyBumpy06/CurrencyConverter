using System.Net;
using System.Text;

using CurrencyConverter.Api.Infrastructure.Frankfurter;

namespace CurrencyConverter.Api.Tests.Infrastructure.Frankfurter;

public sealed class FrankfurterCurrenciesAvailableSourceTests
{
    [Fact]
    public async Task GetCurrenciesAsync_ReturnsCurrenciesInProviderResponseOrder()
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
                    [
                        {
                            "iso_code": "USD",
                            "name": "United States Dollar"
                        },
                        {
                            "iso_code": "EUR",
                            "name": "Euro"
                        }
                    ]
                    """,
                    Encoding.UTF8,
                    "application/json")
            };
        });

        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.frankfurter.dev/v2/")
        };

        var source = new FrankfurterCurrenciesAvailableSource(httpClient);

        // Act
        var result = await source.GetCurrenciesAsync();

        // Assert
        Assert.Equal(2, result.Count);

        // Verify the currencies are mapped correctly.
        Assert.Equal("USD", result[0].Code);
        Assert.Equal("United States Dollar", result[0].Name);

        Assert.Equal("EUR", result[1].Code);
        Assert.Equal("Euro", result[1].Name);

        // Verify the adapter requested the correct endpoint.
        Assert.Equal("/v2/currencies", requestedPath);
    }

    [Fact]
    public async Task GetCurrenciesAsync_ThrowsHttpRequestException_WhenProviderReturnsServerError()
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
            BaseAddress = new Uri("https://api.frankfurter.dev/v2/")
        };

        var source = new FrankfurterCurrenciesAvailableSource(httpClient);

        // Act
        HttpRequestException exception = await Assert.ThrowsAsync<HttpRequestException>(() => source.GetCurrenciesAsync());

        // Assert
        Assert.Equal(HttpStatusCode.ServiceUnavailable, exception.StatusCode);
    }
}
