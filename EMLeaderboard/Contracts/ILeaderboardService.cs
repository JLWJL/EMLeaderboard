using EMLeaderboard.Models;

namespace EMLeaderboard.Contracts;

public interface ILeaderboardService
{
     public Task<decimal> UpdateScoreAsync(long customerId, decimal scoreDelta);
     public Task<List<CustomerScoreRank>> GetCustomersByRank(int start = 1, int? end = null);
     public Task<List<CustomerScoreRank>> GetCustomersById(long customerId, int high = 0, int low = 0);
}