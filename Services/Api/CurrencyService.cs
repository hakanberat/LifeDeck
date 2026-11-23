
using System.Net.Http.Json;
using LifeDeck.Models;

namespace LifeDeck.Services.Api;

public sealed class CurrencyService
{
    private readonly HttpClient _http;

    public CurrencyService(HttpClient http)
    {
        _http = http;
    }

    public async Task<CurrencyRate> GetRateAsync(
        string fromCurrency,
        string toCurrency)
    {
        // Normalize currency codes
        fromCurrency = fromCurrency.Trim().ToUpperInvariant();
        toCurrency = toCurrency.Trim().ToUpperInvariant();

        // Validate currency codes
        static bool IsValidCode(string code) =>
            code.Length == 3 &&
            code.All(c => c is >= 'A' and <= 'Z');

        if (!IsValidCode(fromCurrency) ||
            !IsValidCode(toCurrency))
        {
            throw new ArgumentException("Invalid currency code.");
        }

        // Request exchange rate from Frankfurter API v2
        var url =
            $"https://api.frankfurter.dev/v2/rate/" +
            $"{fromCurrency}/{toCurrency}";

        var result = await _http.GetFromJsonAsync<CurrencyRate>(url);

        if (result is null || result.Rate <= 0)
        {
            throw new InvalidOperationException(
                "Invalid exchange rate received.");
        }

        return result;
    }
}
