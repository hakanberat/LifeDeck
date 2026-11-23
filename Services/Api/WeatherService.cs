
using System.Globalization;
using System.Text.Json;
using LifeDeck.Models;

namespace LifeDeck.Services.Api;

public sealed class WeatherService
{
    private readonly HttpClient _http;

    public WeatherService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<WeatherLocation>> SearchCitiesAsync(
        string city)
    {
        city = city.Trim();

        if (city.Length < 2)
            return new();

        var url =
            "https://geocoding-api.open-meteo.com/v1/search" +
            $"?name={Uri.EscapeDataString(city)}" +
            "&count=5&language=en&format=json";

        using var response = await _http.GetAsync(url);
        response.EnsureSuccessStatusCode();

        using var stream =
            await response.Content.ReadAsStreamAsync();

        using var json = await JsonDocument.ParseAsync(stream);

        if (!json.RootElement.TryGetProperty(
                "results", out var results))
        {
            return new();
        }

        var locations = new List<WeatherLocation>();

        foreach (var item in results.EnumerateArray())
        {
            string? region = item.TryGetProperty(
                "admin1", out var admin)
                ? admin.GetString()
                : null;

            locations.Add(new WeatherLocation(
                item.GetProperty("name").GetString() ?? "",
                item.GetProperty("country").GetString() ?? "",
                region,
                item.GetProperty("latitude").GetDouble(),
                item.GetProperty("longitude").GetDouble()
            ));
        }

        return locations;
    }

    public async Task<WeatherNow> GetCurrentWeatherAsync(
        WeatherLocation location)
    {
        var latitude = location.Latitude.ToString(
            CultureInfo.InvariantCulture);

        var longitude = location.Longitude.ToString(
            CultureInfo.InvariantCulture);

        var url =
            "https://api.open-meteo.com/v1/forecast" +
            $"?latitude={latitude}&longitude={longitude}" +
            "&current=temperature_2m,relative_humidity_2m," +
            "apparent_temperature,precipitation,weather_code," +
            "wind_speed_10m,wind_direction_10m," +
            "wind_gusts_10m,pressure_msl" +
            "&wind_speed_unit=kmh&timezone=auto";

        using var response = await _http.GetAsync(url);
        response.EnsureSuccessStatusCode();

        using var stream =
            await response.Content.ReadAsStreamAsync();

        using var json = await JsonDocument.ParseAsync(stream);

        var current = json.RootElement.GetProperty("current");

        return new WeatherNow(
            TemperatureC:
                current.GetProperty("temperature_2m").GetDouble(),

            FeelsLikeC:
                current.GetProperty("apparent_temperature").GetDouble(),

            Humidity:
                current.GetProperty("relative_humidity_2m").GetInt32(),

            WindSpeedKmh:
                current.GetProperty("wind_speed_10m").GetDouble(),

            WindDirection:
                current.GetProperty("wind_direction_10m").GetInt32(),

            WindGustKmh:
                current.GetProperty("wind_gusts_10m").GetDouble(),

            PrecipitationMm:
                current.GetProperty("precipitation").GetDouble(),

            PressureHpa:
                current.GetProperty("pressure_msl").GetDouble(),

            WeatherCode:
                current.GetProperty("weather_code").GetInt32(),

            LocalTime:
                current.GetProperty("time").GetString() ?? ""
        );
    }
}
