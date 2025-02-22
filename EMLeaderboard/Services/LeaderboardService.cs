using EMLeaderboard.Contracts;
using EMLeaderboard.Models;

namespace EMLeaderboard.Services;

public class LeaderboardService : ILeaderboardService
{
    private SortedSet<Customer> _sortedCustomers;
    private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
    private readonly Dictionary<long, Customer> _customers = new();

    private static readonly IComparer<Customer> CustomerScoreComparer = Comparer<Customer>.Create((x, y) =>
    {
        var scoreComparison = y.Score.CompareTo(x.Score);
        return scoreComparison != 0 ? scoreComparison : x.CustomerId.CompareTo(y.CustomerId);
    });

    public LeaderboardService()
    {
        _sortedCustomers = new SortedSet<Customer>(CustomerScoreComparer);
    }

    public LeaderboardService(IEnumerable<Customer> customers)
    {
        _sortedCustomers = new SortedSet<Customer>(customers, CustomerScoreComparer);
    }

    public Task<decimal> UpdateScoreAsync(long customerId, decimal scoreChange)
    {

        _lock.EnterWriteLock();
        try{
            if(_customers.TryGetValue(customerId, out var existingCustomer)){
                if(existingCustomer.Score > 0){
                    _sortedCustomers.Remove(existingCustomer);
                }

                UpdateCustomerScore(existingCustomer, scoreChange);

                if(existingCustomer.Score > 0){
                    _sortedCustomers.Add(existingCustomer);
                }

                return Task.FromResult(existingCustomer.Score);
                
            }else if(scoreChange > 0){
                var newCustomer = new Customer{
                    CustomerId = customerId,
                    Score = scoreChange
                };
                if(_customers.TryAdd(customerId, newCustomer)){
                    _sortedCustomers.Add(newCustomer);
                }

                return Task.FromResult(newCustomer.Score);
            }

            //Assumption: if no customer found and scoreChange is <= 0, then inform the invalid action via exception
            throw new ArgumentException($"Cannot apply score change {scoreChange} to non-existent customer {customerId}");
        }
        finally{
            _lock.ExitWriteLock();
        }

    }

    public Task<List<CustomerScoreRank>> GetCustomersByRank(int start = 1, int? end = null)
    {
        //Assumption: if start and end are missing, retrieve top 10 customers
        if(end is null || end < start){
            end = start + 9;
        }

        _lock.EnterReadLock();
        try{
            var currentIndex = 1;
            var customerScoreRanks = new List<CustomerScoreRank>();

            foreach(var customer in _sortedCustomers){
                if(currentIndex >= start && currentIndex <= end){
                    customerScoreRanks.Add(new CustomerScoreRank{
                        CustomerId = customer.CustomerId,
                        Score = customer.Score,
                        Rank = currentIndex
                    });
                }

                if(currentIndex == end){
                    break;
                }

                currentIndex++;
            }

            return Task.FromResult(customerScoreRanks);
        }
        finally{
            _lock.ExitReadLock();
        }
    }

    public Task<List<CustomerScoreRank>> GetCustomersById(long customerId, decimal? high, decimal? low)
    {
        // Method implementation goes here
        throw new NotImplementedException();
    }

    private Customer UpdateCustomerScore(Customer customer, decimal scoreChange){
        var newScore = customer.Score + scoreChange;
        if(newScore <= 0){
            customer.Score = 0;
        }else{
            customer.Score = newScore;
        }
        return customer;
    }
} 