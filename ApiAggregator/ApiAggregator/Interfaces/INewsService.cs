using ApiAggregator.Models.Enums;
using ApiAggregator.Models.Responses;

namespace ApiAggregator.Interfaces;

public interface INewsService
{
    Task<List<NewsArticleDto>> GetNewsAsync(string? query, SortBy? sortBy);
}
