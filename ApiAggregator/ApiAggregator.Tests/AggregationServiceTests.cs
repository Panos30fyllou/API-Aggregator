using ApiAggregator.Interfaces;
using ApiAggregator.Models.Enums;
using ApiAggregator.Models.ExternalApiDtos;
using ApiAggregator.Models.Requests;
using ApiAggregator.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;

namespace ApiAggregator.Tests;

public class AggregationServiceTests
{
	[Fact]
	public async Task GetAggregatedDataAsync_ReturnsAggregatedData()
	{
		var gitHubServiceMock = new Mock<IGitHubService>();
		var weatherServiceMock = new Mock<IWeatherService>();
		var newsServiceMock = new Mock<INewsService>();
		var statsServiceMock = new Mock<IStatsService>();
		var loggerMock = new Mock<ILogger<AggregationService>>();

		var cache = new MemoryCache(new MemoryCacheOptions());

		var request = new AggregationRequest
		{
			SearchQuery = "dotnet",
			City = "Athens",
			SortBy = SortBy.Date
		};

		gitHubServiceMock
			.Setup(x => x.GetRepositoriesAsync("dotnet"))
			.ReturnsAsync(new List<GitHubRepoDto>
			{
				new GitHubRepoDto
				{
					Name = "aspnetcore",
					Url = "https://github.com/dotnet/aspnetcore",
					Stars = 100
				}
			});

		weatherServiceMock
			.Setup(x => x.GetWeatherAsync("athens"))
			.ReturnsAsync(new OpenWeatherMapDto
			{
				City = "Athens",
				Temperature = 20,
				Description = "clear sky"
			});

		newsServiceMock
			.Setup(x => x.GetNewsAsync("dotnet", SortBy.Date))
			.ReturnsAsync(new List<NewsArticleDto>
			{
				new NewsArticleDto
				{
					Title = "Dotnet news",
					Source = "Test Source",
					Url = "https://example.com",
					PublishedAt = DateTime.UtcNow
				}
			});

		var service = new AggregationService(
			gitHubServiceMock.Object,
			weatherServiceMock.Object,
			newsServiceMock.Object,
			statsServiceMock.Object,
			loggerMock.Object,
			cache);

		var result = await service.GetAggregatedDataAsync(request);

		Assert.Single(result.Repositories);
		Assert.NotNull(result.Weather);
		Assert.Single(result.News);
		Assert.Empty(result.Errors);

		Assert.Equal("aspnetcore", result.Repositories[0].Name);
		Assert.Equal("Athens", result.Weather.City);
		Assert.Equal("Dotnet news", result.News[0].Title);
	}

	[Fact]
	public async Task GetAggregatedDataAsync_UsesCache_ForRepeatedRequest()
	{
		var gitHubServiceMock = new Mock<IGitHubService>();
		var weatherServiceMock = new Mock<IWeatherService>();
		var newsServiceMock = new Mock<INewsService>();
		var statsServiceMock = new Mock<IStatsService>();
		var loggerMock = new Mock<ILogger<AggregationService>>();

		var cache = new MemoryCache(new MemoryCacheOptions());

		var request = new AggregationRequest
		{
			SearchQuery = "dotnet",
			City = "Athens",
			SortBy = SortBy.Date
		};

		gitHubServiceMock
			.Setup(x => x.GetRepositoriesAsync("dotnet"))
			.ReturnsAsync(new List<GitHubRepoDto>
			{
				new GitHubRepoDto
				{
					Name = "aspnetcore",
					Url = "https://github.com/dotnet/aspnetcore",
					Stars = 100
				}
			});

		weatherServiceMock
			.Setup(x => x.GetWeatherAsync("athens"))
			.ReturnsAsync(new OpenWeatherMapDto
			{
				City = "Athens",
				Temperature = 20,
				Description = "clear sky"
			});

		newsServiceMock
			.Setup(x => x.GetNewsAsync("dotnet", SortBy.Date))
			.ReturnsAsync(new List<NewsArticleDto>
			{
				new NewsArticleDto
				{
					Title = "Dotnet news",
					Source = "Test Source",
					Url = "https://example.com",
					PublishedAt = DateTime.UtcNow
				}
			});

		var service = new AggregationService(
			gitHubServiceMock.Object,
			weatherServiceMock.Object,
			newsServiceMock.Object,
			statsServiceMock.Object,
			loggerMock.Object,
			cache);

		await service.GetAggregatedDataAsync(request);
		await service.GetAggregatedDataAsync(request);

		gitHubServiceMock.Verify(
			x => x.GetRepositoriesAsync("dotnet"),
			Times.Once);
	}

	[Fact]
	public async Task GetAggregatedDataAsync_ReturnsPartialData_WhenGitHubFails()
	{
		var gitHubServiceMock = new Mock<IGitHubService>();
		var weatherServiceMock = new Mock<IWeatherService>();
		var newsServiceMock = new Mock<INewsService>();
		var statsServiceMock = new Mock<IStatsService>();
		var loggerMock = new Mock<ILogger<AggregationService>>();

		var cache = new MemoryCache(new MemoryCacheOptions());

		var request = new AggregationRequest
		{
			SearchQuery = "dotnet",
			City = "Athens",
			SortBy = SortBy.Date
		};

		gitHubServiceMock
			.Setup(x => x.GetRepositoriesAsync("dotnet"))
			.ThrowsAsync(new HttpRequestException("GitHub failed"));

		weatherServiceMock
			.Setup(x => x.GetWeatherAsync("athens"))
			.ReturnsAsync(new OpenWeatherMapDto
			{
				City = "Athens",
				Temperature = 20,
				Description = "clear sky"
			});

		newsServiceMock
			.Setup(x => x.GetNewsAsync("dotnet", SortBy.Date))
			.ReturnsAsync(new List<NewsArticleDto>
			{
				new NewsArticleDto
				{
					Title = "Dotnet news",
					Source = "Test Source",
					Url = "https://example.com",
					PublishedAt = DateTime.UtcNow
				}
			});

		var service = new AggregationService(
			gitHubServiceMock.Object,
			weatherServiceMock.Object,
			newsServiceMock.Object,
			statsServiceMock.Object,
			loggerMock.Object,
			cache);

		var result = await service.GetAggregatedDataAsync(request);

		Assert.Empty(result.Repositories);

		Assert.NotNull(result.Weather);
		Assert.Single(result.News);

		Assert.Contains("GitHub API failed.", result.Errors);
	}
}