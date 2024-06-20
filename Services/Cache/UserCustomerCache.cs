namespace CheckIN.Services.Cache
{
    public class UserCustomerCache
    {
        public Guid? CustomerId { get; set; }

        public Guid? UserId { get; set; }

        public string CustomerName { get; set; } = null!;
    }
}
