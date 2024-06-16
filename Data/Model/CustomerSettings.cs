namespace CheckIN.Data.Model
{
    public class CustomerSettings
    {
        public string Id { get; set; }
        public string CustomerId { get; set; }
        public Customer Customer { get; set; }

        public string TitoToken { get; set; }
        public virtual ICollection<TitoAccount>? TitoAccounts { get; set; }
    }
}
