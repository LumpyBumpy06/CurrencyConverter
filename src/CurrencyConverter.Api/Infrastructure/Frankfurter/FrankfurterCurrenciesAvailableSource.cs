using System.Text.Json.Serialization;

using CurrencyConverter.Core.Application.Interfaces;
using CurrencyConverter.Core.Domain.Models;

namespace CurrencyConverter.Api.Infrastructure.Frankfurter;

/// <summary>
/// Retrieves supported currencies from the Frankfurter API v2.
/// </summary>
public sealed class FrankfurterCurrenciesAvailableSource : ICurrenciesAvailableSource
{
    private readonly HttpClient _httpClient;

    public FrankfurterCurrenciesAvailableSource(HttpClient httpClient)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<Currency>> GetCurrenciesAsync()
    {
        using HttpResponseMessage response = await _httpClient.GetAsync("currencies");

        await FrankfurterHttpResponse.EnsureSuccessStatusCodeAsync(response);

        FrankfurterCurrencyResponse[]? currencyResponses = await response.Content.ReadFromJsonAsync<FrankfurterCurrencyResponse[]>();

        if (currencyResponses is null)
        {
            throw new HttpRequestException("Frankfurter returned an empty currency response.");
        }

        return currencyResponses
            .Select(currency => new Currency(currency.IsoCode, currency.Name))
            .ToArray();
    }

    private sealed record FrankfurterCurrencyResponse(
        [property: JsonPropertyName("iso_code")] string IsoCode,
        [property: JsonPropertyName("name")] string Name);
}
