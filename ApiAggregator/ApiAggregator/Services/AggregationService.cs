using ApiAggregator.Interfaces;
using ApiAggregator.Models.Requests;
using ApiAggregator.Models.Responses;

namespace ApiAggregator.Services;

public class AggregationService : IAggregationService
{
    private readonly IGitHubService _gitHubService;
    private readonly IWeatherService _weatherService;
    private readonly INewsService _newsService;

    public AggregationService(
        IGitHubService gitHubService,
        IWeatherService weatherService,
        INewsService newsService)
    {
        _gitHubService = gitHubService;
        _newsService = newsService;
        _weatherService = weatherService;
    }

    public async Task<AggregationResponse> GetAggregatedDataAsync(AggregationRequest aggregationRequest)
    {
        var githubTask = _gitHubService.GetRepositoriesAsync(aggregationRequest.SearchQuery);
        var weatherTask = _weatherService.GetWeatherAsync(aggregationRequest.City);
        var newsTask = _newsService.GetNewsAsync(aggregationRequest.SearchQuery, aggregationRequest.SortBy);

        await Task.WhenAll(githubTask, weatherTask, newsTask);

        return new AggregationResponse
        {
            Repositories = await githubTask,
            Weather = await weatherTask,
            News = await newsTask
        };
    }
}