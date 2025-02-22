using EMLeaderboard.Contracts;
using EMLeaderboard.Controllers;
using EMLeaderboard.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestPlatform.Common.Utilities;
using Moq;

namespace EMLeaderboard.UnitTests.Controllers;

public class LeaderboardControllerTest
{
    private readonly LeaderboardController _leaderboardController;
    private readonly Mock<ILeaderboardService> _leaderboardServiceMock;
    public LeaderboardControllerTest()
    {
        _leaderboardServiceMock = new Mock<ILeaderboardService>();
        _leaderboardController = new LeaderboardController(_leaderboardServiceMock.Object);
    }

    #region UpdateScore
    [Fact]
    public async Task UpdateScore_WithValidParameters_CallsServiceUpdateScoreAsync()
    {
        //Arrange
        const long customerId = 111111111;
        const decimal scoreChange = 100;
        _leaderboardServiceMock.Setup(x => x.UpdateScoreAsync(customerId, scoreChange)).ReturnsAsync(100m);

        //Act
        var result = await _leaderboardController.UpdateScore(customerId, scoreChange);

        //Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        _leaderboardServiceMock.Verify(x => x.UpdateScoreAsync(customerId, scoreChange), Times.Once);
    }

     [Theory]
    [InlineData(-1000)]
    [InlineData(0)]
    [InlineData(1000)]
    public async Task UpdateScore_WithEdgeValidScore_CallsServiceUpdateScoreAsync(decimal score)
    {
        // Arrange
        const long customerId = 111111111;
        _leaderboardServiceMock.Setup(x => x.UpdateScoreAsync(customerId, score)).ReturnsAsync(score);

        // Act
        var result = await _leaderboardController.UpdateScore(customerId, score);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        _leaderboardServiceMock.Verify(x => x.UpdateScoreAsync(customerId, score), Times.Once);
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task UpdateScore_WithInvalidCustomerId_ReturnsBadRequest(long customerId)
    {
        // Arrange
        const decimal scoreChange = 100;

        // Act
        var result = await _leaderboardController.UpdateScore(customerId, scoreChange);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Customer ID must be greater than 0", badRequestResult.Value!.ToString());
        _leaderboardServiceMock.Verify(x => x.UpdateScoreAsync(customerId, scoreChange), Times.Never);
    }

   

    [Theory]
    [InlineData(-1001)]
    [InlineData(1001)]
    public async Task UpdateScore_WithInvalidScore_ReturnsBadRequest(decimal score)
    {
        // Arrange
        const long customerId = 111111111;

        // Act
        var result = await _leaderboardController.UpdateScore(customerId, score);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Contains("Score must be between -1000 and 1000", badRequestResult.Value!.ToString());
        _leaderboardServiceMock.Verify(x => x.UpdateScoreAsync(customerId, score), Times.Never);
    }
    #endregion

    #region GetCustomersByRank
    [Theory]
    [InlineData(0, 10)]
    [InlineData(0, 0)]
    [InlineData(10, 20)]
    public async Task GetCustomersByRank_WithValidRange_ReturnsOkResult(int start, int end)
    {
        // Act
        var result = await _leaderboardController.GetCustomersByRank(start, end);

        // Assert
        Assert.IsType<OkObjectResult>(result.Result);
        _leaderboardServiceMock.Verify(x => x.GetCustomersByRankAsync(start, end), Times.Once);
    }

    [Theory]
    [InlineData(-1, 10)]
    [InlineData(0, -1)]
    [InlineData(10, 5)]
    public async Task GetCustomersByRank_WithInvalidRange_ReturnsBadRequest(int start, int end)
    {
        // Act
        var result = await _leaderboardController.GetCustomersByRank(start, end);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Contains("Invalid start or end", badRequestResult.Value!.ToString());
        _leaderboardServiceMock.Verify(x => x.GetCustomersByRankAsync(start, end), Times.Never);
    }
    #endregion

    #region GetCustomersById
    [Theory]
    [InlineData(111111111, 0, 0)]
    [InlineData(111111111, 100, 50)]
    [InlineData(111111111, 1000, 0)]
    public async Task GetCustomersById_WithValidParameters_ReturnsOkResult(long customerId, int high, int low)
    {
        //Arrange
        _leaderboardServiceMock.Setup((x) => x.GetCustomersByIdAsync(It.IsAny<long>(), It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new List<CustomerScoreRank>());
        
        // Act
        var result = await _leaderboardController.GetCustomersById(customerId, high, low);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.IsType<List<CustomerScoreRank>>(okResult.Value);
        _leaderboardServiceMock.Verify(x => x.GetCustomersByIdAsync(customerId, high, low), Times.Once);
    }

    [Theory]
    [InlineData(111111111, -1, 0)]
    [InlineData(111111111, 0, -1)]
    [InlineData(111111111, -10, -5)]
    public async Task GetCustomersById_WithNegativeRanges_ReturnsBadRequest(long customerId, int high, int low)
    {
        // Act
        var result = await _leaderboardController.GetCustomersById(customerId, high, low);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Contains("must be greater than 0", badRequestResult.Value!.ToString());
        _leaderboardServiceMock.Verify(x => x.GetCustomersByIdAsync(customerId, high, low), Times.Never);
    }
    #endregion
}