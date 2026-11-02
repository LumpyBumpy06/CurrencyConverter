using System.Net;
using System.Text.Json.Serialization;

using CurrencyConverter.Core.Application.Interfaces;
using CurrencyConverter.Core.Domain.Models;

namespace CurrencyConverter.Api.Infrastructure.Frankfurter;

/// <summary>
/// Retrieves exchange rates from the Frankfurter API v2.
/// </summary>
public sealed class FrankfurterExchangeRateSource : IExchangeRateSource
{
    private readonly HttpClient _httpClient;

    public FrankfurterExchangeRateSource(HttpClient httpClient)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        _httpClient = httpClient;
    }

    public async Task<ExchangeRate?> GetExchangeRateAsync(Currency baseCurrency, Currency quoteCurrency)
    {
        ArgumentNullException.ThrowIfNull(baseCurrency);
        ArgumentNullException.ThrowIfNull(quoteCurrency);

        string requestUri = $"rate/{baseCurrency.Code.ToLowerInvariant()}/{quoteCurrency.Code.ToLowerInvariant()}";

        using HttpResponseMessage response = await _httpClient.GetAsync(requestUri);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        await FrankfurterHttpResponse.EnsureSuccessStatusCodeAsync(response);

        FrankfurterRateResponse? rateResponse = await response.Content.ReadFromJsonAsync<FrankfurterRateResponse>();

        if (rateResponse is null)
        {
            throw new HttpRequestException("Frankfurter returned an empty rate response.");
        }

        if (rateResponse.Base != baseCurrency.Code || rateResponse.Quote != quoteCurrency.Code)
        {
            throw new HttpRequestException("Frankfurter returned an unexpected currency pair.");
        }

        if (rateResponse.Date == default)
        {
            throw new HttpRequestException("Frankfurter returned an invalid rate date.");
        }

        if (rateResponse.Rate <= 0)
        {
            throw new HttpRequestException("Frankfurter returned an invalid exchange rate.");
        }

        return new ExchangeRate(baseCurrency, quoteCurrency, rateResponse.Rate, DateTimeOffset.UtcNow);
    }

    private sealed record FrankfurterRateResponse(
        [property: JsonPropertyName("date")] DateOnly Date,
        [property: JsonPropertyName("base")] string Base,
        [property: JsonPropertyName("quote")] string Quote,
        [property: JsonPropertyName("rate")] decimal Rate);
}
