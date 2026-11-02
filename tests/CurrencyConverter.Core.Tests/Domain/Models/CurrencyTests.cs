using CurrencyConverter.Core.Domain.Models;

namespace CurrencyConverter.Core.Tests.Domain.Models;

public class CurrencyTests
{
    [Fact]
    public void Constructor_WithValidValues_CreatesCurrency()
    {
        const string code = "GBP";
        const string name = "Pound Sterling";

        var currency = new Currency(code, name);

        Assert.Equal(code, currency.Code);
        Assert.Equal(name, currency.Name);
    }

    [Fact]
    public void Constructor_WithLowercaseCode_ConvertsCodeToUppercase()
    {
        var currency = new Currency("gbp", "Pound Sterling");
        Assert.Equal("GBP", currency.Code);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void Constructor_WithEmptyCode_ThrowsArgumentException(string code)
    {
        Assert.Throws<ArgumentException>(() => new Currency(code, "Pound Sterling"));
    }

    [Theory]
    [InlineData("GB")]
    [InlineData("GBPT")]
    [InlineData("1BP")]
    [InlineData("G£P")]
    public void Constructor_WithInvalidCode_ThrowsArgumentException(string code)
    {
        Assert.Throws<ArgumentException>(() => new Currency(code, "Pound Sterling"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void Constructor_WithEmptyName_ThrowsArgumentException(string name)
    {
        Assert.Throws<ArgumentException>(() => new Currency("GBP", name));
    }
}
