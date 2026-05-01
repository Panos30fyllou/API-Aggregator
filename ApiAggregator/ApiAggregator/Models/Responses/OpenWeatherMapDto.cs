namespace ApiAggregator.Models.Responses;

public class OpenWeatherMapDto
{
    public string? City { get; set; }
    public double Temperature { get; set; }
    public string? Description { get; set; }
}