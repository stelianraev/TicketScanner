namespace CheckIN.Services.Customer
{
    public interface ICustomerProvider
    {
        string GetCustomerId();

        void SetCustomerId(int customerId);
    }
}
