namespace EMLeaderboard.Models.Exceptions;

public class CustomerNotFoundException : NotFoundException
{
    public CustomerNotFoundException(decimal customerId):base("Customer", customerId)
    {
    }
}