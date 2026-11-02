namespace CurrencyConverter.Api.Contracts;

/// <summary>
/// Represents the result of a currency conversion returned by the API.
/// </summary>
public sealed record ConversionResponse(
    decimal OriginalAmount,
    string SourceCurrency,
    decimal ConvertedAmount,
    string DestinationCurrency,
    decimal ExchangeRate,
    DateTimeOffset RateRetrievedAtUTC
);
