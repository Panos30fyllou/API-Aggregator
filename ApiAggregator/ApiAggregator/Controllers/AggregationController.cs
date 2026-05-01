using ApiAggregator.Interfaces;
using ApiAggregator.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace ApiAggregator.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AggregationController : ControllerBase
{
    private readonly IAggregationService _aggregationService;

    public AggregationController(IAggregationService aggregationService)
    {
        _aggregationService = aggregationService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAggregatedData([FromQuery] AggregationRequest aggregationRequest)
    {
        var response = await _aggregationService.GetAggregatedDataAsync(aggregationRequest);

        return Ok(response);
    }
}