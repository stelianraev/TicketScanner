namespace CheckIN.Services.Customer
{
    public class CustomerProvider : ICustomerProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CustomerProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetTenantId()
        {
            // Extract tenant ID from the request (e.g., from JWT token or subdomain)
            var customerId = _httpContextAccessor.HttpContext!.User.FindFirst("CustomerId")?.Value;
            return customerId;
        }

        public void SetCustomerId(int customerId)
        {
            _httpContextAccessor.HttpContext!.Items["CustomerId"] = customerId;
        }
    }
}
