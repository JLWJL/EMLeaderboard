using EMLeaderboard.Contracts;
using EMLeaderboard.Models;
using EMLeaderboard.Models.Exceptions;

namespace EMLeaderboard.Services;

public class LeaderboardService : ILeaderboardService
{
    private SortedSet<Customer> _sortedCustomers;
    //TODO: inject data source?
    private readonly ReaderWriterLockSlim _lock = new ();
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

    public LeaderboardService(List<Customer> customers)
    {
        _customers = customers.ToDictionary((c)=>c.CustomerId);
        _sortedCustomers = new SortedSet<Customer>(customers, CustomerScoreComparer);
    }

    public Task<decimal> UpdateScoreAsync(long customerId, decimal scoreChange)
    {
        if(customerId < 1){
            throw new ArgumentException("Customer ID must be greater than 0");
        }

        if(scoreChange < -1000 || scoreChange > 1000){
            throw new ArgumentException("Score must be between -1000 and 1000");
        }

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

    public Task<List<CustomerScoreRank>> GetCustomersByRankAsync(int start = 1, int? end = null)
    {
        //Assumption: if end is missing, retrieve 10 customers
        if(end is null || end == 0){
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

    public Task<List<CustomerScoreRank>> GetCustomersByIdAsync(long customerId, int high = 0, int low = 0)
    {
        
        _lock.EnterReadLock();
        try{
            //Assumption: throw exception if customer is not found
            if(!_customers.TryGetValue(customerId, out _)){
                throw new CustomerNotFoundException(customerId);
            }

            var result = new List<CustomerScoreRank>();
            var higherNeibours = new Queue<CustomerScoreRank>(high);
            var currentRank = 1;
            int  maxCustomerIndex = 0;

            foreach(var c in _sortedCustomers)
            {  
                if(c.CustomerId == customerId){
                    //when target found, add higher neighbours and itself
                    result.AddRange(higherNeibours);
                    result.Add(new CustomerScoreRank{
                        CustomerId = c.CustomerId,
                        Score = c.Score,
                        Rank = currentRank
                    });
                    maxCustomerIndex = currentRank + low;
                }
                else if(result.Count == 0 && high != 0 )
                {
                    //when target not found, add current to higher neighbours queue and maintain the queue size
                    if(higherNeibours.Count == high){
                        higherNeibours.Dequeue();
                    }
                    higherNeibours.Enqueue(new CustomerScoreRank{
                        CustomerId = c.CustomerId,
                        Score = c.Score,
                        Rank = currentRank
                    });
                }
                else if(currentRank <= maxCustomerIndex)
                {
                    //add lower neighbours after 
                    result.Add(new CustomerScoreRank{
                        CustomerId = c.CustomerId,
                        Score = c.Score,
                        Rank = currentRank
                    });
                }

                currentRank++;
            }

            return Task.FromResult(result);
        }
        finally{
            _lock.ExitReadLock();
        }
    }

    private void UpdateCustomerScore(Customer customer, decimal scoreChange){
        var newScore = customer.Score + scoreChange;
        if(newScore <= 0){
            customer.Score = 0;
        }else{
            customer.Score = newScore;
        }
    }
} 