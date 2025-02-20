using EMLeaderboard.Models;

namespace EMLeaderboard.Contracts;

public interface ILeaderboardService
{
     public Task<decimal> UpdateScoreAsync(long customerId, decimal scoreDelta);
     public Task<List<CustomerScoreRank>> GetCustomersByRank(int? start, int? end);
     public Task<List<CustomerScoreRank>> GetCustomersById(long customerId, decimal? high, decimal? low);
}