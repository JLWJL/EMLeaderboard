using EMLeaderboard.Services;

public class LeaderboardServicesTests
{
    private LeaderboardService _leaderboardService;

    public LeaderboardServicesTests()
    {
        _leaderboardService = new LeaderboardService();
    }

    #region UpdateScoreAsync
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
    #endregion

    #region GetCustomersByRank
    [Fact]
    public async Task GetCustomersByRank_WhenNoCustomers_ShouldReturnEmptyList()
    {
        //Act
        var result = await _leaderboardService.GetCustomersByRank(100,300);

        //Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetCustomersByRank_WhenNoStartOrEnd_ShouldReturnTop10()
    {
        //Arrange
        var customersBuilder = new ShuffledCustomersBuilder().WithNCustomers(50).Build();
        _leaderboardService = new LeaderboardService(customersBuilder);

        //Act
        var result = await _leaderboardService.GetCustomersByRank();

        //Assert
        Assert.Equal(10, result.Count);

        Assert.Equal(50, result.First().CustomerId);
        Assert.Equal(50, result.First().Score);
        Assert.Equal(1, result.First().Rank);

        Assert.Equal(41, result.Last().CustomerId);
        Assert.Equal(41, result.Last().Score);
        Assert.Equal(10, result.Last().Rank);
    }

    [Fact]
    public async Task GetCustomersByRank_WhenHaveStartNoEnd_ShouldReturn10CustomersFromStart(){
        //Arrange
        var totalCustomers = 20;
        var start = 3;
        
        var customersBuilder = new ShuffledCustomersBuilder().WithNCustomers(totalCustomers).Build();
        _leaderboardService = new LeaderboardService(customersBuilder);

        //Act
        var result = await _leaderboardService.GetCustomersByRank(start);

        //Assert
        Assert.Equal(10, result.Count);

        Assert.Equal(18, result.First().CustomerId);
        Assert.Equal(18, result.First().Score);
        Assert.Equal(3, result.First().Rank);

        Assert.Equal(9, result.Last().CustomerId);
        Assert.Equal(9, result.Last().Score);
        Assert.Equal(12, result.Last().Rank);
    }

    [Fact]
    public async Task GetCustomersByRank_WhenEndGreaterThanCustomers_ShouldReturnAllCustomersFromStart()
    {
        //Arrange
        var totalCustomers = 20;
        var start = 3;
        
        var customersBuilder = new ShuffledCustomersBuilder().WithNCustomers(totalCustomers).Build();
        _leaderboardService = new LeaderboardService(customersBuilder);

        //Act
        var result = await _leaderboardService.GetCustomersByRank(start, 30);
        
        //Assert
        Assert.Equal(18, result.Count);
        
        Assert.Equal(18, result.First().CustomerId);
        Assert.Equal(18, result.First().Score);
        Assert.Equal(3, result.First().Rank);
        
        Assert.Equal(1, result.Last().CustomerId);
        Assert.Equal(1, result.Last().Score);
        Assert.Equal(20, result.Last().Rank);
    }
    [Fact]
    public async Task GetCustomersByRank_WhenMultipleCustomersEqualScore_ShouldReturnSortedById(){
        //Arrange
        var customersBuilder = new ShuffledCustomersBuilder().WithNCustomers(10).WithNEqualScoreCustomers(5, 6).Build();
        _leaderboardService = new LeaderboardService(customersBuilder);

        //Act
        var result = await _leaderboardService.GetCustomersByRank(3, 10);
        var equalScoreCustomers = result.Where(x => x.Score == 6).ToList();

        //Assert
        Assert.Equal(8, result.Count);

        for(var i = 1; i < equalScoreCustomers.Count; i++){
            Assert.True(equalScoreCustomers[i-1].CustomerId < equalScoreCustomers[i].CustomerId);
        }
    }
    #endregion
    #endregion
}
