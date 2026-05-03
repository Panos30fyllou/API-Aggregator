using ApiAggregator.Models.ExternalApiDtos;

namespace ApiAggregator.Models.Responses;

public class AggregationResponse
{
	public List<GitHubRepoDto> Repositories { get; set; } = [];
	public OpenWeatherMapDto? Weather { get; set; }
	public List<NewsArticleDto> News { get; set; } = [];
	public List<string> Errors { get; set; } = [];
}