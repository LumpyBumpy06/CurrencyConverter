using System.Net;

using CurrencyConverter.Api.Contracts;
using CurrencyConverter.Core.Application.Exceptions;
using CurrencyConverter.Core.Application.Services;
using CurrencyConverter.Core.Domain.Models;

namespace CurrencyConverter.Api.Endpoints;

public static class ConversionsEndpoints
{
    public static IEndpointRouteBuilder MapConversionsEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/conversions", ConvertAsync)
            .WithName("ConvertCurrency")
            .WithTags("Conversions")
            .Produces<ConversionResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status502BadGateway);

        return endpoints;
    }

    // Core automatically injects CurrencyConversionService and also injects ICurrenciesAvailableSource as its constructor parameter
    // Notice this nested injection.
    private static async Task<IResult> ConvertAsync(
        decimal amount,
        string sourceCurrency,
        string destinationCurrency,
        CurrencyConversionService conversionService)
    {
        try
        {
            // Create the currencies from the supplied currency codes.
            var source = new Currency(sourceCurrency, sourceCurrency);
            var destination = new Currency(destinationCurrency, destinationCurrency);

            // Create the original amount of money.
            var originalMoney = new Money(amount, source);

            // Delegate the conversion to the Core service.
            var result = await conversionService.ConvertAsync(originalMoney, destination);

            // Map the Core result to our API response contract.
            var response = new ConversionResponse(
                result.OriginalMoney.Amount,
                result.OriginalMoney.Currency.Code,
                result.ConvertedMoney.Amount,
                result.ConvertedMoney.Currency.Code,
                result.Rate.Rate,
                result.Rate.RetrievedAtUTC);

            return Results.Ok(response);
        }
        catch (ExchangeRateNotFoundException)
        {
            // The provider returned no exchange rate.
            return Results.Problem(
                title: "Exchange rate not found",
                detail: "No exchange rate was found for the requested currency pair.",
                statusCode: StatusCodes.Status404NotFound);
        }
        catch (ArgumentException)
        {
            // The amount or currency code failed Core validation.
            return Results.Problem(
                title: "Invalid conversion request",
                detail: "Check that the amount is greater than zero and both currency codes are valid.",
                statusCode: StatusCodes.Status400BadRequest);
        }
        catch (HttpRequestException ex) when (
            ex.StatusCode is HttpStatusCode.BadRequest or HttpStatusCode.NotFound or HttpStatusCode.UnprocessableEntity)
        {
            // The provider rejected the requested currency pair.
            return Results.Problem(
                title: "Unsupported currency pair",
                detail: "The requested currency pair is not supported by the currency provider.",
                statusCode: StatusCodes.Status400BadRequest);
        }
        catch (HttpRequestException)
        {
            // The provider failed for another reason, such as an outage.
            return Results.Problem(
                title: "Currency provider error",
                detail: "The currency provider could not complete the request.",
                statusCode: StatusCodes.Status502BadGateway);
        }
    }
}
