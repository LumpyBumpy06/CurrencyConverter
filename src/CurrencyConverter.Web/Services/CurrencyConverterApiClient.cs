using System.Globalization;
using System.Net.Http.Json;

using CurrencyConverter.Web.Models;

namespace CurrencyConverter.Web.Services;

public sealed class CurrencyConverterApiClient
{
    private readonly HttpClient _httpClient;

    public CurrencyConverterApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<CurrencyOption>> GetCurrenciesAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<CurrencyOption>>("api/currencies")
            ?? throw new InvalidOperationException("No currencies were returned by the API.");
    }

    public async Task<ConversionResponse> ConvertAsync(decimal amount, string sourceCurrency, string destinationCurrency)
    {
        var amountText = amount.ToString(CultureInfo.InvariantCulture);

        var url = $"api/conversions?amount={amountText}" +
                  $"&sourceCurrency={Uri.EscapeDataString(sourceCurrency)}" +
                  $"&destinationCurrency={Uri.EscapeDataString(destinationCurrency)}";

        return await _httpClient.GetFromJsonAsync<ConversionResponse>(url)
            ?? throw new InvalidOperationException("No conversion result was returned by the API.");
    }
}
