namespace CheckIN.Data.Model
{
    public class Ticket
    {
        public string Id { get; set; } = null!;

        public int TicketId { get; set; }

        public string Slug { get; set; } = null!;

        public string? CompanyName { get; set; }

        public string? Email { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? FullName { get; set; }

        public string? PhoneNumber { get; set; }

        public decimal Price { get; set; }

        public string? State { get; set; }

        public int RegistrationId { get; set; }

        public string? DiscountCodeUsed { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public decimal TotalPaid { get; set; }

        public string? RegistrationSlug { get; set; }

        public string? ReleaseSlug { get; set; }

        public Guid EventId { get; set; }

        public Event Event { get; set; }

        //public virtual ICollection<Event>? Events { get; set; }

        public virtual ICollection<Attendee>? Attendees { get; set; }

        public bool IsScanned { get; set; }
    }
}
