namespace ApiAggregator.Models.Responses;

public class NewsArticleDto
{
    public string? Title { get; set; }
    public string? Source { get; set; }
    public string? Url { get; set; }
    public DateTime? PublishedAt { get; set; }
}