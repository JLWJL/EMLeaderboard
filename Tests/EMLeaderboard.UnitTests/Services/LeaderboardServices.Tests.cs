using EMLeaderboard.Models.Exceptions;
using EMLeaderboard.Services;
using EMLeaderboard.UnitTests.Helpers;

namespace EMLeaderboard.UnitTests.Services;

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
        var result = await _leaderboardService.GetCustomersByRankAsync(100,300);

        //Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetCustomersByRank_WhenNoStartOrEnd_ShouldReturnTop10()
    {
        //Arrange
        var shuffledCustomers = new ShuffledCustomersBuilder().WithNCustomers(50).Build();
        _leaderboardService = new LeaderboardService(shuffledCustomers);

        //Act
        var result = await _leaderboardService.GetCustomersByRankAsync();

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
        
        var shuffledCustomers = new ShuffledCustomersBuilder().WithNCustomers(totalCustomers).Build();
        _leaderboardService = new LeaderboardService(shuffledCustomers);

        //Act
        var result = await _leaderboardService.GetCustomersByRankAsync(start);

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
        
        var shuffledCustomers = new ShuffledCustomersBuilder().WithNCustomers(totalCustomers).Build();
        _leaderboardService = new LeaderboardService(shuffledCustomers);

        //Act
        var result = await _leaderboardService.GetCustomersByRankAsync(start, 30);
        
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
        var result = await _leaderboardService.GetCustomersByRankAsync(3, 10);
        var equalScoreCustomers = result.Where(x => x.Score == 6).ToList();

        //Assert
        Assert.Equal(8, result.Count);

        for(var i = 1; i < equalScoreCustomers.Count; i++){
            Assert.True(equalScoreCustomers[i-1].CustomerId < equalScoreCustomers[i].CustomerId);
        }
    }

    [Fact]
    public async Task GetCustomersByRank_WhenStartNonExisting_ShouldReturnEmptyList()
    {
        var result = await _leaderboardService.GetCustomersByRankAsync(10);
        Assert.Empty(result);
    }
    #endregion

    #region GetCustomersById

    [Fact]
    public async Task GetCustomersById_WhenCustomerIdIsNotFound_ShouldThrowCustomerNotFoundException()
    {
        //Act & Assert
        await Assert.ThrowsAsync<CustomerNotFoundException>(() => _leaderboardService.GetCustomersByIdAsync(123));        
    }

    [Fact]
    public async Task GetCustomersById_WhenCustomersIsTheFirst_ShouldNotHaveHigherNeighbours()
    {
        //Arrange
        var shuffledCustomers = new ShuffledCustomersBuilder().WithNCustomers(10).Build();
        _leaderboardService = new LeaderboardService(shuffledCustomers);
        
        //Act
        var result = await _leaderboardService.GetCustomersByIdAsync(10, 3, 5);
        
        //Assert
        Assert.Equal(6, result.Count);
        Assert.Equal(10, result.First().CustomerId);
        Assert.Equal(1, result.First().Rank);
        
        Assert.Equal(5, result.Last().CustomerId);
    }
    
    [Fact]
    public async Task GetCustomersById_WhenCustomersIsTheLast_ShouldNotHaveLowerNeighbours()
    {
        //Arrange
        var shuffledCustomers = new ShuffledCustomersBuilder().WithNCustomers(10).Build();
        _leaderboardService = new LeaderboardService(shuffledCustomers);
        
        //Act
        var result = await _leaderboardService.GetCustomersByIdAsync(1, 3, 5);
        
        //Assert
        Assert.Equal(4, result.Count);
        Assert.Equal(4, result.First().CustomerId);
        Assert.Equal(7, result.First().Rank);
        
        Assert.Equal(1, result.Last().CustomerId);
    }

    [Fact]
    public async Task GetCustomersById_WhenHighExceeds_ShouldReturnAllBeforeTargetCustomer()
    {
        //Arrange
        var shuffledCustomers = new ShuffledCustomersBuilder().WithNCustomers(10).Build();
        _leaderboardService = new LeaderboardService(shuffledCustomers);
        
        //Act
        var result = await _leaderboardService.GetCustomersByIdAsync(6, 8, 2);
        
        //Assert
        Assert.Equal(7, result.Count);
        Assert.Equal(10, result.First().CustomerId);
        Assert.Equal(1, result.First().Rank);
        
        Assert.Equal(4, result.Last().CustomerId);
    }
    
    [Fact]
    public async Task GetCustomersById_WhenLowExceeds_ShouldReturnAllAfterTargetCustomer()
    {
        //Arrange
        var shuffledCustomers = new ShuffledCustomersBuilder().WithNCustomers(10).Build();
        _leaderboardService = new LeaderboardService(shuffledCustomers);
        
        //Act
        var result = await _leaderboardService.GetCustomersByIdAsync(2, 2, 5);
        
        //Assert
        Assert.Equal(4, result.Count);
        Assert.Equal(4, result.First().CustomerId);
        Assert.Equal(7, result.First().Rank);
        
        Assert.Equal(1, result.Last().CustomerId);
    }

    [Fact]
    public async Task GetCustomersById_WhenZeroHighAndLow_ShouldReturnTargetCustomer()
    {
        //Arrange
        var shuffledCustomers = new ShuffledCustomersBuilder().WithNCustomers(10).Build();
        _leaderboardService = new LeaderboardService(shuffledCustomers);
        
        //Act
        var result = await _leaderboardService.GetCustomersByIdAsync(2);
        
        //Assert
        Assert.Single(result);
        Assert.Equal(2, result.First().CustomerId);
        Assert.Equal(9, result.First().Rank);
    }
    
    [Fact]
    public async Task GetCustomersById_WhenHighAndLowWithinRange_ShouldReturnCorrectHighAndLowNeighbours()
    {
        //Arrange
        var shuffledCustomers = new ShuffledCustomersBuilder().WithNCustomers(15).Build();
        _leaderboardService = new LeaderboardService(shuffledCustomers);
        
        //Act
        var result = await _leaderboardService.GetCustomersByIdAsync(8, 3, 5);
        
        //Assert
        Assert.Equal(9, result.Count);
        Assert.Equal(11, result.First().CustomerId);
        Assert.Equal(5, result.First().Rank);
        
        Assert.Equal(3, result.Last().CustomerId);
    }
    #endregion
}