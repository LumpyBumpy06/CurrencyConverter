namespace CurrencyConverter.Web.Models;

public sealed record CurrencyOption(string Code, string Name);

public sealed record ConversionResponse(
    decimal OriginalAmount,
    string SourceCurrency,
    decimal ConvertedAmount,
    string DestinationCurrency,
    decimal ExchangeRate,
    DateTimeOffset RateRetrievedAtUTC
);
