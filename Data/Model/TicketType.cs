namespace CheckIN.Data.Model
{
    public class TicketType
    {
        public string TicketTypeId { get; set; }

        public string Name { get; set; }

        public virtual ICollection<Ticket> Tickets { get; set;} = new List<Ticket>();
    }
}
