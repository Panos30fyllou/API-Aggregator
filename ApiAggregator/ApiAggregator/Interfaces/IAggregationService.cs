using ApiAggregator.Models.Requests;
using ApiAggregator.Models.Responses;

namespace ApiAggregator.Interfaces;

public interface IAggregationService
{
    Task<AggregationResponse> GetAggregatedDataAsync(AggregationRequest aggregationRequest);
}
