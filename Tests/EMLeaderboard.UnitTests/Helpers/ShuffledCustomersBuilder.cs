using EMLeaderboard.Models;

namespace EMLeaderboard.UnitTests.Helpers;

public class ShuffledCustomersBuilder{
    private readonly List<Customer> _newCustomers = new();

    public ShuffledCustomersBuilder WithNEqualScoreCustomers(int numberOfCustomers, decimal score){
        var currentNumberOfCustomers = _newCustomers.Count;
        for(var i = 0; i < numberOfCustomers; i++){
            _newCustomers.Add(new Customer{
                CustomerId = ++currentNumberOfCustomers,
                Score = score
            });
        }
        return this;
    }

    public ShuffledCustomersBuilder WithNCustomers(int numberOfCustomers, decimal scoreFrom = 1)
    {
        var currentNumberOfCustomers = _newCustomers.Count;
        for(var i = 0; i < numberOfCustomers; i++){
            _newCustomers.Add(new Customer{
                CustomerId = ++currentNumberOfCustomers,
                Score = scoreFrom++
            });
        }

        return this;
    }

    public List<Customer> Build(){
        var arrayToShuffle = _newCustomers.ToArray();
        Random.Shared.Shuffle(arrayToShuffle);
        return arrayToShuffle.ToList();
    }
}