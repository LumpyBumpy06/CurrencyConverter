using CurrencyConverter.Api.Infrastructure.Frankfurter;
using CurrencyConverter.Core.Application.Interfaces;
using CurrencyConverter.Core.Application.Services;
using CurrencyConverter.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// OpenAPI
builder.Services.AddOpenApi();

// Frankfurter configuration
string frankfurterBaseUrl = builder.Configuration["Frankfurter:BaseUrl"] ?? throw new InvalidOperationException("Frankfurter:BaseUrl configuration is missing.");

if (!Uri.TryCreate(frankfurterBaseUrl, UriKind.Absolute, out Uri? frankfurterBaseUri))
{
    throw new InvalidOperationException("Frankfurter:BaseUrl must be a valid absolute URL.");
}

// Ensure the base URL ends with a slash so relative endpoint URLs
// are appended to the configured API path.
if (!frankfurterBaseUri.AbsoluteUri.EndsWith('/'))
{
    frankfurterBaseUri = new Uri(frankfurterBaseUri.AbsoluteUri + "/");
}

// Configure both typed HTTP clients.
Action<HttpClient> configureFrankfurterClient = client =>
{
    client.BaseAddress = frankfurterBaseUri;
};

// ExchangeRate adapter
builder.Services.AddHttpClient<IExchangeRateSource, FrankfurterExchangeRateSource>(configureFrankfurterClient);

// CurrenciesAvailable adapter
builder.Services.AddHttpClient<ICurrenciesAvailableSource, FrankfurterCurrenciesAvailableSource>(configureFrankfurterClient);

// Application services
builder.Services.AddScoped<CurrencyConversionService>();

var app = builder.Build();

// OpenAPI
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Register our API endpoints
app.MapCurrenciesEndpoints();
app.MapConversionsEndpoints();

// Health endpoint
app.MapGet("/health", () => Results.Ok(new
{
    Status = "Healthy"
}));

app.Run();
