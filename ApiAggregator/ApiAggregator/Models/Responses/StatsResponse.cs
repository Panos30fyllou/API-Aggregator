namespace ApiAggregator.Models.Responses;

public class StatsResponse
{
    public Dictionary<string, ApiStatResponse> ApiStats { get; set; } = [];
}