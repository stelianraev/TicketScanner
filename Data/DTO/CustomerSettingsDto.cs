namespace CheckIN.Data.DTO
{
    public class CustomerSettingsDto
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string TitoToken { get; set; }
        public List<TitoAccountDto> TitoAccounts { get; set; }
    }
}
