using EMLeaderboard.Models;
using Microsoft.AspNetCore.Mvc;

namespace EMLeaderboard.Controllers;

[ApiController]
[Route("[controller]")]
public class LeaderboardController : ControllerBase
{
    public LeaderboardController()
    {
        
    }

    [HttpPost]
    [Route("~/customer/{customerId}/score/{score}")]
    public ActionResult<decimal> UpdateScore([FromRoute] long customerId, [FromRoute] decimal score)
    {
        if(score < -1000 || score > 1000)
        {
            return BadRequest("Score must be between -1000 and 1000");
        }
        
        return Ok(1000m);
    }

    [HttpGet]
    [Route("")]
    public ActionResult<List<Customer>> GetCustomersByRank([FromQuery] int start, [FromQuery] int end)
    {
        if (start < 0 || end < 0 || start > end)
        {
            return BadRequest("Invalid start or end");
        }

        return Ok(new List<Customer>());
    }

    [HttpGet]
    [Route("{customerId}")]
    public ActionResult<List<Customer>> GetCustomersById([FromRoute] long customerId, [FromQuery] decimal? high = 0, [FromQuery] decimal? low = 0)
    {
        if(high < 0 || low < 0 )
        {
            return BadRequest("{high} and {low} must be greater than 0");
        }

        return Ok(new List<Customer>());
    }
}