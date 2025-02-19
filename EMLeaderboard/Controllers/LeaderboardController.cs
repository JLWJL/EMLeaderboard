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
}