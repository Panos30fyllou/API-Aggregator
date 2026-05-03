using ApiAggregator.Interfaces;
using ApiAggregator.Models.ExternalApiDtos;
using System.Text.Json;

namespace ApiAggregator.Services.ExternalApis;

public class GitHubService : IGitHubService
{
    private readonly HttpClient _httpClient;

    public GitHubService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<GitHubRepoDto>> GetRepositoriesAsync(string searchQuery)
    {
        var url = $"https://api.github.com/search/repositories?q={Uri.EscapeDataString(searchQuery)}&sort=stars&order=desc&per_page=5";
        var json = await _httpClient.GetStringAsync(url);
        var doc = JsonDocument.Parse(json);
        var items = doc.RootElement.GetProperty("items");

        var result = new List<GitHubRepoDto>();

        foreach (var item in items.EnumerateArray())
        {
            result.Add(new GitHubRepoDto
            {
                Name = item.GetProperty("name").GetString(),
                Url = item.GetProperty("html_url").GetString(),
                Stars = item.GetProperty("stargazers_count").GetInt32()
            });
        }

        return result;
    }
}