using EMLeaderboard.Models;

namespace EMLeaderboard.Contracts;

public interface ILeaderboardService
{
     public Task<decimal> UpdateScoreAsync(long customerId, decimal scoreDelta);
     public Task<List<CustomerScoreRank>> GetCustomersByRankAsync(int start = 1, int? end = null);
     public Task<List<CustomerScoreRank>> GetCustomersByIdAsync(long customerId, int high = 0, int low = 0);
}