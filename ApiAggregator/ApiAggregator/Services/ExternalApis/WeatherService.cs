using System.Text.Json;
using ApiAggregator.Interfaces;
using ApiAggregator.Models.ExternalApiDtos;

namespace ApiAggregator.Services.ExternalApis;

public class WeatherService : IWeatherService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public WeatherService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<OpenWeatherMapDto?> GetWeatherAsync(string city)
    {
        var apiKey = _configuration["ExternalApis:OpenWeather:ApiKey"];

        var url = $"https://api.openweathermap.org/data/2.5/weather?q={Uri.EscapeDataString(city)}&units=metric&appid={apiKey}";
        var json = await _httpClient.GetStringAsync(url);
        var doc = JsonDocument.Parse(json);

        return new OpenWeatherMapDto
        {
            City = doc.RootElement.GetProperty("name").GetString(),
            Temperature = doc.RootElement.GetProperty("main").GetProperty("temp").GetDouble(),
            Description = doc.RootElement.GetProperty("weather")[0].GetProperty("description").GetString()
        };
    }
}