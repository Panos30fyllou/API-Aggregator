using ApiAggregator.Models.Enums;

namespace ApiAggregator.Models.Requests;

public class AggregationRequest
{
    public string? SearchQuery { get; set; }
    public string? City { get; set; }
    public SortBy? SortBy { get; set; }
}