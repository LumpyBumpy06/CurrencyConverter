# Currency Converter

A currency converter built with C# and .NET 10, using Blazor for the frontend and an ASP.NET Core API for currency conversions.

Exchange-rate data is provided by [Frankfurter](https://frankfurter.dev/).

## Technology

- C# / .NET 10
- ASP.NET Core Minimal APIs
- Blazor (Interactive Server)
- xUnit
- Frankfurter API

## Project Structure

- **Core:** Domain models and conversion logic.
- **API:** HTTP endpoints and Frankfurter integration.
- **Web:** Blazor user interface.
- **Tests:** Unit tests for Core and API adapter code.

Currency providers are accessed through interfaces and registered centrally using dependency injection in `Program.cs`. 
This allows providers to be replaced without changing endpoint code and makes testing with fake implementations easier.

## Running Locally

Start the API:

```bash
dotnet run --project src/CurrencyConverter.Api --launch-profile http
```

In a second terminal, start the frontend:

```bash
dotnet run --project src/CurrencyConverter.Web
```

Open the URL displayed by the Web application.

## Running Tests

```bash
dotnet test CurrencyConverter.slnx
```

## AI Usage

Used LLms as a development assistant for the following taks:
- Syntax and best practices for C#, .NET and Blazor.
- Suggesting, explaing and reviewing code implementations.
- Generating suggested test cases.
- Generating ci/cd workflow
- Developing the frontend UI.
- Preparing project documentation.

AI generated suggestions were reviewed and adapted during development. I made the implementation decisions, worked through the code to understand it, and built and tested the application locally using automated tests, curl and manual UI testing.

## Future Improvements / TODO

### Core

- Add bounds checking to `Money` to test against loss of decimal accuracy.
- Add `CancellationToken` support to `IExchangeRateSource` and `CurrencyConversionService`, allowing external requests to be cancelled when no longer needed.
- Optimise same currency conversions. A conversion such as GBP to GBP should return an exchange rate of 1 without calling the external provider.
- Clarify distinction between the provider's rate date (`RateDate`) and when the rate was retrieved by our backend (`RetrievedAt`).

### Testing

- Add a test proving the exchange rate source is not called for same currency conversions.
- Add tests for rounding, precision and cancellation.
- Consider API integration tests to verify the endpoints and HTTP responses together.