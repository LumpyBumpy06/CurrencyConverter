using CurrencyConverter.Core.Domain.Models;

namespace CurrencyConverter.Core.Tests.Domain.Models;

public class MoneyTests
{
    [Fact]
    public void Constructor_WithValidValues_CreatesMoney()
    {
        var currency = new Currency("GBP", "Pound Sterling");
        const decimal amount = 100.50m;

        var money = new Money(amount, currency);

        Assert.Equal(amount, money.Amount);
        Assert.Equal(currency, money.Currency);
    }

    [Fact]
    public void Constructor_WithNullCurrency_ThrowsArgumentNullException()
    {
        // null! to force compiler to say we want to pass null here
        Assert.Throws<ArgumentNullException>(() => new Money(100m, null!));
    }

    [Fact]
    public void Constructor_WithNegativeAmount_AllowsNegativeAmount()
    {
        var currency = new Currency("GBP", "Pound Sterling");
        var money = new Money(-100m, currency);
        Assert.Equal(-100m, money.Amount);
    }

    [Fact]
    public void Constructor_WithZeroAmount_AllowsZeroAmount()
    {
        var currency = new Currency("GBP", "Pound Sterling");
        var money = new Money(0m, currency);
        Assert.Equal(0m, money.Amount);
    }
}
