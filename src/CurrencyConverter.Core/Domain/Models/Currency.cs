using System;
using System.Collections.Generic;
using System.Text;

namespace CurrencyConverter.Core.Domain.Models;
public sealed record Currency
{
    public string Code { get; }
    public string Name { get; }

    public Currency(string code, string name)
    {
        if (string.IsNullOrWhiteSpace(code)) {
            throw new ArgumentException("Currency code cannot be empty.", nameof(code));
        }

        bool codeIsAlphabet = code.All(c => (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'));
        if (code.Length != IsoCurrencyCodeLength || !codeIsAlphabet) {
            throw new ArgumentException("Currency code must contain exactly three ASCII characters.", nameof(code));
        }

        if (string.IsNullOrWhiteSpace(name)) {
            throw new ArgumentException("Currency name cannot be empty.", nameof(name));
        }

        Code = code.ToUpperInvariant();
        Name = name.Trim();
    }
    private const int IsoCurrencyCodeLength = 3;
    private const int MaxSupportedDecimalPlaces = 3;
}
