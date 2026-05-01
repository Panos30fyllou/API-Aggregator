using ApiAggregator.Models.Responses;

namespace ApiAggregator.Interfaces;

public interface IGitHubService
{
    Task<List<GitHubRepoDto>> GetRepositoriesAsync(string? searchQuery);
}
