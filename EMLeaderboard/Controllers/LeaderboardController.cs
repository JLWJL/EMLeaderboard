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

   /// <summary>
    /// Updates a customer's score by adding the specified value
    /// </summary>
    /// <param name="customerId">The unique identifier of the customer</param>
    /// <param name="score">Value between -1000 and +1000 to add to current score</param>
    /// <remarks>
    /// If score is positive, it increases customer's score
    /// If score is negative, it decreases customer's score
    /// If customer doesn't exist, creates a new customer
    /// </remarks>
    /// <response code="200">Returns the updated total score</response>
    /// <response code="400">If score is outside valid range</response>
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


   /// <summary>
/// Retrieves a range of customers from the leaderboard ordered by their rank
/// </summary>
/// <param name="start">Starting rank position (inclusive, minimum value is 1)</param>
/// <param name="end">Ending rank position (inclusive, must be greater than or equal to start)</param>
/// <remarks>
/// Only returns customers with scores greater than zero.
/// Rankings are determined by:
/// - Higher scores rank higher
/// - For equal scores, lower customer ID ranks higher
/// </remarks>
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

    /// <summary>
    /// Retrieves a customer and their neighboring customers from the leaderboard by customer ID
    /// </summary>
    /// <param name="customerId">The unique identifier of the target customer</param>
    /// <param name="high">Number of neighbors to retrieve with ranks higher than the target customer. Default is 0.</param>
    /// <param name="low">Number of neighbors to retrieve with ranks lower than the target customer. Default is 0.</param>
    /// <remarks>
    /// Returns:
    /// - The target customer if found
    /// - Up to 'high' number of customers ranked above the target
    /// - Up to 'low' number of customers ranked below the target
    /// 
    /// The response will be ordered by rank (highest rank first).
    /// </remarks>
    /// <response code="200">Returns the target customer and specified number of neighbors</response>
    /// <response code="400">If high or low parameters are negative</response>
    /// <response code="404">If the target customer is not found</response>
    /// <returns>List of customers including the target and specified neighbors</returns>
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