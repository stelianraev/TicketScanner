namespace CheckIN.Data.DTO
{
    public class CustomerSettingsDto
    {
        public string Id { get; set; }
        public string CustomerId { get; set; }
        public string TitoToken { get; set; }
        public List<TitoAccountDto> TitoAccounts { get; set; }
    }
}
