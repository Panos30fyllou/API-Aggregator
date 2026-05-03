using ApiAggregator.Interfaces;
using ApiAggregator.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace ApiAggregator.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StatsController : ControllerBase
{
    private readonly IStatsService _statsService;

    public StatsController(IStatsService statsService)
    {
        _statsService = statsService;
    }

    [HttpGet]
    public ActionResult<StatsResponse> GetStats()
    {
        return Ok(_statsService.GetStats());
    }
}