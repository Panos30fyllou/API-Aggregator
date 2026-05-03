using ApiAggregator.Models.Enums;
using ApiAggregator.Models.ExternalApiDtos;

namespace ApiAggregator.Interfaces;

public interface INewsService
{
	Task<List<NewsArticleDto>> GetNewsAsync(string query, SortBy? sortBy);
}
