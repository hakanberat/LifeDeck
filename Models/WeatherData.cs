
namespace LifeDeck.Models;

public sealed record WeatherLocation(
    string Name,
    string Country,
    string? Region,
    double Latitude,
    double Longitude
);

public sealed record WeatherNow(
    double TemperatureC,
    double FeelsLikeC,
    int Humidity,
    double WindSpeedKmh,
    int WindDirection,
    double WindGustKmh,
    double PrecipitationMm,
    double PressureHpa,
    int WeatherCode,
    string LocalTime
);
