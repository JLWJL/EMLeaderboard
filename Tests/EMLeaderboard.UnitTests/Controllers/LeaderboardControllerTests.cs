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
        const long customerId = 111111111;
        //Arrange
        const decimal scoreChange = 100;
        
        //Act
        var result = _leaderboardController.UpdateScore(customerId, scoreChange);

        //Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.IsType<decimal>(okResult.Value);
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void UpdateScore_WithInvalidCustomerId_ReturnsBadRequest(long customerId)
    {
        // Arrange
        const decimal scoreChange = 100;

        // Act
        var result = _leaderboardController.UpdateScore(customerId, scoreChange);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Customer ID must be greater than 0", badRequestResult.Value!.ToString());
    }

    [Theory]
    [InlineData(-1000)]
    [InlineData(0)]
    [InlineData(1000)]
    public void UpdateScore_WithValidScore_ReturnsExpectedScore(decimal score)
    {
        // Arrange
        const long customerId = 111111111;

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
        const long customerId = 111111111;

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
    [InlineData(-1, 10)]
    [InlineData(0, -1)]
    [InlineData(10, 5)]
    public void GetCustomersByRank_WithInvalidRange_ReturnsBadRequest(int start, int end)
    {
        // Act
        var result = _leaderboardController.GetCustomersByRank(start, end);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Contains("Invalid start or end", badRequestResult.Value!.ToString());
    }

    [Theory]
    [InlineData(111111111, 0, 0)]
    [InlineData(111111111, 100, 50)]
    [InlineData(111111111, 1000, 0)]
    public void GetCustomersById_WithValidParameters_ReturnsOkResult(long customerId, decimal high, decimal low)
    {
        // Act
        var result = _leaderboardController.GetCustomersById(customerId, high, low);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var customers = Assert.IsType<List<Customer>>(okResult.Value);
        Assert.NotNull(customers);
    }

    [Theory]
    [InlineData(111111111, -1, 0)]
    [InlineData(111111111, 0, -1)]
    [InlineData(111111111, -10, -5)]
    public void GetCustomersById_WithNegativeRanges_ReturnsBadRequest(long customerId, decimal high, decimal low)
    {
        // Act
        var result = _leaderboardController.GetCustomersById(customerId, high, low);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Contains("must be greater than 0", badRequestResult.Value!.ToString());
    }
}