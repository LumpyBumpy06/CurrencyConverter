namespace CurrencyConverter.Api.Contracts;
// Don't want to expose internals (our Core's Currency Model), instead define something we are giving to the users.

/// <summary>
/// Represents a currency returned by our API for consumers.
/// </summary>
public sealed record CurrencyResponse(string Code, string Name);