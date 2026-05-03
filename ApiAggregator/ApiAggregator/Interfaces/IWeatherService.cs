using ApiAggregator.Models.ExternalApiDtos;

namespace ApiAggregator.Interfaces;

public interface IWeatherService
{
    Task<OpenWeatherMapDto?> GetWeatherAsync(string city);
}
