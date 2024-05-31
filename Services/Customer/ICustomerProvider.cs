namespace CheckIN.Services.Customer
{
    public interface ICustomerProvider
    {
        string GetTenantId();

        void SetCustomerId(int customerId);
    }
}
