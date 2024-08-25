namespace CheckIN.Data.Model
{
    public class TicketType
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid EventId {  get; set; }

        public Event Event { get; set; }
    }
}
