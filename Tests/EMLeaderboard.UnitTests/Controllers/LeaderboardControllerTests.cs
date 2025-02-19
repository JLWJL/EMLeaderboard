using EMLeaderboard.Controllers;
using EMLeaderboard.Models;
using Microsoft.AspNetCore.Mvc;

namespace EMLeaderboard.UnitTests.Controllers;

public class LeaderboardControllerTest
{
    private readonly LeaderboardController _leaderboardController;
    
    public LeaderboardControllerTest()
    {
        _leaderboardController = new();
    }

    [Fact]
    public void UpdateScore_ReturnsCurrentScoreOfDecimalType()
    {
        //Arrange
        const long customerId = 111111111;
        const decimal scoreChange = 100;
        
        //Act
        var result = _leaderboardController.UpdateScore(customerId, scoreChange);

        //Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.IsType<decimal>(okResult.Value);
    }

    [Theory]
    [InlineData(-1000)]
    [InlineData(0)]
    [InlineData(1000)]
    public void UpdateScore_WithValidScore_ReturnsExpectedScore(decimal score)
    {
        // Arrange
        const long customerId = 1;

        // Act
        var result = _leaderboardController.UpdateScore(customerId, score);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(1000m, okResult.Value);
    }

    [Theory]
    [InlineData(-1001)]
    [InlineData(1001)]
    public void UpdateScore_WithInvalidScore_ReturnsBadRequest(decimal score)
    {
        // Arrange
        const long customerId = 1;

        // Act
        var result = _leaderboardController.UpdateScore(customerId, score);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Contains("Score must be between -1000 and 1000", badRequestResult.Value!.ToString());
    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(0, 0)]
    [InlineData(10, 20)]
    public void GetCustomersByRank_WithValidRange_ReturnsOkResult(int start, int end)
    {
        // Act
        var result = _leaderboardController.GetCustomersByRank(start, end);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var customers = Assert.IsType<List<Customer>>(okResult.Value);
        Assert.NotNull(customers);
    }

    [Theory]
    [InlineData(-1, 10)]    // negative start
    [InlineData(0, -1)]     // negative end
    [InlineData(10, 5)]     // start greater than end
    public void GetCustomersByRank_WithInvalidRange_ReturnsBadRequest(int start, int end)
    {
        // Act
        var result = _leaderboardController.GetCustomersByRank(start, end);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Contains("Invalid start or end", badRequestResult.Value!.ToString());
    }
}