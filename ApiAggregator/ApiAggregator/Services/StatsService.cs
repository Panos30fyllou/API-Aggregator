using ApiAggregator.Interfaces;
using ApiAggregator.Models.Responses;
using System.Collections.Concurrent;

namespace ApiAggregator.Services;

public class StatsService : IStatsService
{
    private readonly ConcurrentDictionary<string, ConcurrentBag<double>> _apiResponseTimes = new();

    public void RecordRequest(string apiName, double responseTimeMs)
    {
        var responseTimes = _apiResponseTimes.GetOrAdd(
            apiName,
            _ => new ConcurrentBag<double>());

        responseTimes.Add(responseTimeMs);
    }

    public StatsResponse GetStats()
    {
        var response = new StatsResponse();

        foreach (var apiStats in _apiResponseTimes)
        {
            var responseTimes = apiStats.Value.ToArray();

            if (responseTimes.Length == 0)
                continue;

            response.ApiStats[apiStats.Key] = new ApiStatResponse
            {
                TotalRequests = responseTimes.Length,
                AverageResponseTimeMs = responseTimes.Average(),
                FastRequests = responseTimes.Count(x => x < 100),
                AverageRequests = responseTimes.Count(x => x >= 100 && x <= 200),
                SlowRequests = responseTimes.Count(x => x > 200)
            };
        }

        return response;
    }
}