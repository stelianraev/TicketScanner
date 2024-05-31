namespace CheckIN.Data.Model
{
    public class Ticket
    {
        public string TicketId { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string? Type { get; set; }

        public virtual ICollection<Event>? Events { get; set; }

        public virtual ICollection<Attendee>? Attendees { get; set; }
    }
}
