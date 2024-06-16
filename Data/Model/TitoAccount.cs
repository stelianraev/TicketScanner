namespace CheckIN.Data.Model
{
    public class TitoAccount
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string CustomerSettingsId { get; set; }

        public CustomerSettings CustomerSettings { get; set; }

        public virtual ICollection<Event> Events { get; set; }
    }
}
