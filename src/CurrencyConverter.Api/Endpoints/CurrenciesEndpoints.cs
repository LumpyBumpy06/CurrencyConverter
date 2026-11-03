using CurrencyConverter.Api.Contracts;
using CurrencyConverter.Core.Application.Interfaces;

namespace CurrencyConverter.Api.Endpoints;

public static class CurrenciesEndpoints
{
    // Extension method on IEndpointRouteBuilder
    public static IEndpointRouteBuilder MapCurrenciesEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/currencies", CurrenciesAsync)
        .WithName("GetCurrencies")
        .WithTags("Currencies")
        .Produces<CurrencyResponse[]>(StatusCodes.Status200OK);

        return endpoints;
    }

    // Core automatically injects ICurrenciesAvailableSource (our Frankfurter adapter) - then retrieves currencies
    // This was setup in Program.cs
    private static async Task<IResult> CurrenciesAsync(ICurrenciesAvailableSource currenciesSource)
    {
        try
        {
            var currencies = await currenciesSource.GetCurrenciesAsync();

            var response = currencies
                .OrderBy(currency => currency.Code, StringComparer.Ordinal)
                .Select(currency => new CurrencyResponse(currency.Code, currency.Name))
                .ToArray();

            return Results.Ok(response);
        }
        catch (HttpRequestException)
        {
            return Results.Problem(
                title: "Currency provider error",
                detail: "The currency provider could not retrieve the currency list.",
                statusCode: StatusCodes.Status502BadGateway);
        }
    }
}
