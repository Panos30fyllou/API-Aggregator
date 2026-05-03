namespace ApiAggregator.Models.Responses;

public class ApiStatResponse
{
	public int TotalRequests { get; set; }
	public double AverageResponseTimeMs { get; set; }
	public int FastRequests { get; set; }
	public int AverageRequests { get; set; }
	public int SlowRequests { get; set; }
}