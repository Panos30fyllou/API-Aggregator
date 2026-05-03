using ApiAggregator.Models.Responses;

namespace ApiAggregator.Interfaces;

public interface IStatsService
{
	void RecordRequest(string apiName, double responseTimeMs);
	StatsResponse GetStats();
}
