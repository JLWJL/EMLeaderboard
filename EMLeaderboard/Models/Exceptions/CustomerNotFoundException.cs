public class CustomerNotFoundException : Exception
{
    public CustomerNotFoundException(decimal customerId):base($"Customer {customerId} not found")
    {
    }
}

