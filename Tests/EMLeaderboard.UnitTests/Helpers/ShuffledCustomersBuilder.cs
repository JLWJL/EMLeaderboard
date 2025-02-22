using EMLeaderboard.Services;
using EMLeaderboard.Models;
public class ShuffledCustomersBuilder{
    private readonly List<Customer> _newCustomers = new();

    public ShuffledCustomersBuilder WithNEqualScoreCustomers(int numberOfCustomers, decimal score){
        for(var i = 0; i < numberOfCustomers; i++){
            _newCustomers.Add(new Customer{
                CustomerId = _newCustomers.Count + 1,
                Score = score
            });
        }
        return this;
    }

    public ShuffledCustomersBuilder WithNCustomers(int numberOfCustomers, decimal scoreFrom = 1){
        for(var i = 0; i < numberOfCustomers; i++){
            _newCustomers.Add(new Customer{
                CustomerId = _newCustomers.Count + 1,
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
