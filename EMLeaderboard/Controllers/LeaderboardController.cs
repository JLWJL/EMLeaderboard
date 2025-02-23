using EMLeaderboard.Models;
using EMLeaderboard.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace EMLeaderboard.Controllers;

[ApiController]
[Route("[controller]")]
public class LeaderboardController : ControllerBase
{
    private readonly ILeaderboardService _leaderboardService;

    public LeaderboardController(ILeaderboardService leaderboardService)
    {
        _leaderboardService = leaderboardService;
    }

    [HttpPost]
    [Route("~/customer/{customerId}/score/{score}")]
    public async Task<ActionResult<decimal>> UpdateScore([FromRoute] long customerId, [FromRoute] decimal score)
    {
        if(customerId < 1)
        {
            return BadRequest("Customer ID must be greater than 0");
        }

        if(score < -1000 || score > 1000)
        {
            return BadRequest("Score must be between -1000 and 1000");
        }

        var result = await _leaderboardService.UpdateScoreAsync(customerId, score);
        
        return Ok(result);
    }

    [HttpGet]
    [Route("")]
    public async Task<ActionResult<List<CustomerScoreRank>>> GetCustomersByRank([FromQuery] int start = 1, [FromQuery] int? end = null)
    {
        if (start < 0 || end < 0 || start > end)
        {
            return BadRequest("Invalid start or end");
        }
        
        //Assumption: if start and end are missing, retrieve top 10 customers
        if(end is null){
            end = start + 9;
        }

        var result = await _leaderboardService.GetCustomersByRankAsync(start, end);

        return Ok(result);
    }

    [HttpGet]
    [Route("{customerId}")]
    public async Task<ActionResult<List<Customer>>> GetCustomersById([FromRoute] long customerId, [FromQuery] int high = 0, [FromQuery] int low = 0)
    {
        if(high < 0 || low < 0 )
        {
            return BadRequest("{high} and {low} must be greater than 0");
        }

        var result = await _leaderboardService.GetCustomersByIdAsync(customerId, high, low);

        return Ok(result);
    }
}