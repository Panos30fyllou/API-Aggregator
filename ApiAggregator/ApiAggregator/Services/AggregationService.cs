using ApiAggregator.Interfaces;
using ApiAggregator.Models.Enums;
using ApiAggregator.Models.Requests;
using ApiAggregator.Models.Responses;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace ApiAggregator.Services;

public class AggregationService : IAggregationService
{
	private readonly IGitHubService _gitHubService;
	private readonly IWeatherService _weatherService;
	private readonly INewsService _newsService;
	private readonly IStatsService _statsService;
	private readonly ILogger<AggregationService> _logger;
	private readonly IMemoryCache _cache;

	public AggregationService(
		IGitHubService gitHubService,
		IWeatherService weatherService,
		INewsService newsService,
		IStatsService statsService,
		ILogger<AggregationService> logger,
		IMemoryCache cache)
	{
		_gitHubService = gitHubService;
		_newsService = newsService;
		_weatherService = weatherService;
		_statsService = statsService;
		_logger = logger;
		_cache = cache;
	}

	public async Task<AggregationResponse> GetAggregatedDataAsync(AggregationRequest aggregationRequest)
	{
		var errors = new ConcurrentBag<string>();

		aggregationRequest = ValidateRequest(aggregationRequest);

		var githubTask = GetExternalApiResponseAsync(aggregationRequest, ExternalApi.GitHub, () => _gitHubService.GetRepositoriesAsync(aggregationRequest.SearchQuery), errors);
		var weatherTask = GetExternalApiResponseAsync(aggregationRequest, ExternalApi.Weather, () => _weatherService.GetWeatherAsync(aggregationRequest.City!), errors);
		var newsTask = GetExternalApiResponseAsync(aggregationRequest, ExternalApi.News, () => _newsService.GetNewsAsync(aggregationRequest.SearchQuery, aggregationRequest.SortBy), errors);

		await Task.WhenAll(githubTask, weatherTask, newsTask);

		var response = new AggregationResponse
		{
			Repositories = await githubTask ?? [],
			Weather = await weatherTask,
			News = await newsTask ?? [],
			Errors = errors.ToList()
		};

		return response;
	}


	#region Privates
	private AggregationRequest ValidateRequest(AggregationRequest request)
	{
		if (string.IsNullOrWhiteSpace(request.SearchQuery))
			throw new ArgumentException("SearchQuery must be provided.");

		if (string.IsNullOrWhiteSpace(request.City))
			request.City = "Athens";

		request.SearchQuery = request.SearchQuery.Trim().ToLower();
		request.City = request.City.Trim().ToLower();

		return request;
	}

	private async Task<T?> GetExternalApiResponseAsync<T>(AggregationRequest aggregationRequest, ExternalApi api, Func<Task<T>> externalApiCall, ConcurrentBag<string> errors)
	{
		var apiName = api.ToString();
		var cacheKey = GetCacheKey(api, aggregationRequest);

		if (_cache.TryGetValue<T>(cacheKey, out var cachedResult) && cachedResult is not null)
		{
			_logger.LogInformation("{ApiName} data returned from cache", apiName);
			return cachedResult;
		}

		return await CallExternalApiAsync(apiName, cacheKey, externalApiCall, errors);
	}

	private string GetCacheKey(ExternalApi api, AggregationRequest request)
	{
		return api switch
		{
			ExternalApi.GitHub => $"github:{request.SearchQuery}",
			ExternalApi.Weather => $"weather:{request.City}",
			ExternalApi.News => $"news:{request.SearchQuery}:{request.SortBy}",
			_ => throw new NotSupportedException($"Unsupported API: {api}")
		};
	}

	private async Task<T?> CallExternalApiAsync<T>(string apiName, string cacheKey, Func<Task<T>> externalApiCall, ConcurrentBag<string> errors)
	{
		var stopwatch = Stopwatch.StartNew();

		try
		{
			var result = await externalApiCall();
			if (result is not null)
				_cache.Set(cacheKey, result, TimeSpan.FromMinutes(5));

			return result;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "{ApiName} API failed", apiName);
			errors.Add($"{apiName} API failed.");
			return default;
		}
		finally
		{
			stopwatch.Stop();
			_statsService.RecordRequest(apiName, stopwatch.Elapsed.TotalMilliseconds);
		}
	}
	#endregion
}