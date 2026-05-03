using ApiAggregator.Models.ExternalApiDtos;

namespace ApiAggregator.Interfaces;

public interface IGitHubService
{
	Task<List<GitHubRepoDto>> GetRepositoriesAsync(string searchQuery);
}
