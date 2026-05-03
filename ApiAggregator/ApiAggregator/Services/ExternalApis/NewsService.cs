using System.Text.Json;
using ApiAggregator.Interfaces;
using ApiAggregator.Models.Enums;
using ApiAggregator.Models.ExternalApiDtos;

namespace ApiAggregator.Services.ExternalApis;

public class NewsService : INewsService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public NewsService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<List<NewsArticleDto>> GetNewsAsync(string searchQuery, SortBy? sortBy)
    {
        var apiKey = _configuration["ExternalApis:NewsApi:ApiKey"];

        var newsSortBy = sortBy switch
        {
            SortBy.Date => "publishedAt",
            SortBy.Relevance => "relevancy",
            SortBy.Popularity => "popularity",
            _ => "publishedAt"
        };

        var url = $"https://newsapi.org/v2/everything?q={Uri.EscapeDataString(searchQuery)}&sortBy={newsSortBy}&pageSize=5&apiKey={apiKey}";
        var response = await _httpClient.GetAsync(url);
        var json = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception($"NewsAPI error: {response.StatusCode} - {json}");
        
        
        var doc = JsonDocument.Parse(json);
        var articles = doc.RootElement.GetProperty("articles");
        var result = new List<NewsArticleDto>();

        foreach (var article in articles.EnumerateArray())
        {
            result.Add(new NewsArticleDto
            {
                Title = article.GetProperty("title").GetString(),
                Source = article.GetProperty("source").GetProperty("name").GetString(),
                Url = article.GetProperty("url").GetString(),
                PublishedAt = article.GetProperty("publishedAt").GetDateTime()
            });
        }

        return result;
    }
}