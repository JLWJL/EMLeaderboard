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
    public decimal UpdateScore([FromRoute] long customerId, [FromRoute] decimal score)
    {
        if(score < -1000 || score > 1000)
        {
            throw new ArgumentOutOfRangeException(nameof(score), score, "Score must be between -1000 and 1000");
        }
        
        return 1000m;
    }
}