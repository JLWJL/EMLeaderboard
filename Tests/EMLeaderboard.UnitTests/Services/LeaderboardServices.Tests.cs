using EMLeaderboard.Services;

public class LeaderboardServicesTests
{
    private readonly LeaderboardService _leaderboardService;

    public LeaderboardServicesTests()
    {
        _leaderboardService = new LeaderboardService();
    }

    [Fact]
    public async Task UpdateScoreAsync_WhenNewCustomerWithPositiveScoreChange_ShouldAddAndReturnScore()
    {
        // Arrange
        long customerId = 1;
        decimal scoreChange = 100;

        // Act
        var result = await _leaderboardService.UpdateScoreAsync(customerId, scoreChange);

        // Assert
        Assert.Equal(scoreChange, result);
    }

    [Fact]
    public async Task UpdateScoreAsync_WhenNewCustomerWithNegativeScoreChange_ShouldThrowArgumentException()
    {
        // Arrange
        long customerId = 1;
        decimal scoreChange = -100;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>   
            _leaderboardService.UpdateScoreAsync(customerId, scoreChange));
    }

    [Theory]
    [InlineData(100, 50, 150)]
    [InlineData(100, -50, 50)]
    public async Task UpdateScoreAsync_WhenExistingCustomerWithPositiveFinalScore_ShouldUpdateAndReturnScore(decimal initialScore, decimal scoreChange, decimal expectedScore)
    {
        // Arrange
        long customerId = 1;

        // Act
        await _leaderboardService.UpdateScoreAsync(customerId, initialScore);
        var result = await _leaderboardService.UpdateScoreAsync(customerId, scoreChange);

        // Assert
        Assert.Equal(expectedScore, result);
    }

    [Fact]
    public async Task UpdateScoreAsync_WhenExistingCustomerScoreLessOrEqualZero_ShouldReturnZero()
    {
        // Arrange
        long customerId = 1;
        decimal initialScore = 100;
        decimal scoreChange = -100;

        // Act
        await _leaderboardService.UpdateScoreAsync(customerId, initialScore);
        var result = await _leaderboardService.UpdateScoreAsync(customerId, scoreChange);

        // Assert
        Assert.Equal(initialScore + scoreChange, result);

    }
}
