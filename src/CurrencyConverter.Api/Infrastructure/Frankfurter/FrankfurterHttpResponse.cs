using System.Text.Json;
using System.Text.Json.Serialization;

namespace CurrencyConverter.Api.Infrastructure.Frankfurter;

/// <summary>
/// Handles unsuccessful HTTP responses from Frankfurter.
/// </summary>
internal static class FrankfurterHttpResponse
{
    internal static async Task EnsureSuccessStatusCodeAsync(HttpResponseMessage response)
    {
        ArgumentNullException.ThrowIfNull(response);

        if (response.IsSuccessStatusCode)
        {
            return;
        }

        FrankfurterErrorResponse? errorResponse = null;
        bool errorBodyWasInvalid = false;

        try
        {
            errorResponse = await response.Content.ReadFromJsonAsync<FrankfurterErrorResponse>();
        }
        catch (JsonException)
        {
            errorBodyWasInvalid = true;
        }

        string message = errorResponse?.Message ??
            (errorBodyWasInvalid ? "Frankfurter returned an unreadable error response." : response.ReasonPhrase ?? "An unknown error occurred.");

        throw new HttpRequestException($"Frankfurter returned HTTP {(int)response.StatusCode}: {message}", null, response.StatusCode);
    }

    private sealed record FrankfurterErrorResponse(
        [property: JsonPropertyName("status")] int Status,
        [property: JsonPropertyName("message")] string? Message);
}
