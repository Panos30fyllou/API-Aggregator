using ApiAggregator.Models.Responses;

namespace ApiAggregator.Interfaces;

public interface IWeatherService
{       
    Task<OpenWeatherMapDto?> GetWeatherAsync(string? city);
}
