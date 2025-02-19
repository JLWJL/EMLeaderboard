using EMLeaderboard.Controllers;

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
        Assert.IsType<decimal>(result);
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
        Assert.Equal(1000m, result);
    }

    [Theory]
    [InlineData(-1001)]
    [InlineData(1001)]
    public void UpdateScore_WithInvalidScore_ThrowsArgumentOutOfRangeException(decimal score)
    {
        // Arrange
        const long customerId = 1;

        // Act & Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => 
            _leaderboardController.UpdateScore(customerId, score));
        
        Assert.Equal("score", exception.ParamName);
        Assert.Contains("Score must be between -1000 and 1000", exception.Message);
    }

    
}