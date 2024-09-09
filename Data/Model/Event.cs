using System.ComponentModel.DataAnnotations;

namespace CheckIN.Data.Model
{
    public class Event
    {
        [Key]
        public Guid EventId { get; set; }

        public string? Type { get; set; }

        public string? StartDate { get; set; }

        public string? EndDate { get; set; }

        public string? CreatedAt { get; set; }

        public string? UpdatedAt { get; set; }

        public string? SecurityToken { get; set; }

        public string? Url { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public string? Currency { get; set; }

        public bool TestMode { get; set; }

        public int DiscountCodesCount { get; set; }

        public bool ShowDiscountCodeField { get; set; }

        public string? AccountSlug { get; set; }

        public List<string>? Locales { get; set; }

        public string? Location { get; set; }

        public bool Private { get; set; }

        public string? Slug { get; set; }

        public bool Live { get; set; }

        public bool IsSelected { get; set; }

        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; }

        public Guid TitoAccountId {  get; set; }

        public TitoAccount TitoAccount { get; set; }

        public virtual ICollection<UserEvent> UserEvents { get; set; } = new List<UserEvent>();

        public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

        public virtual ICollection<TicketType> TicketTypes { get; set; } = new List<TicketType>();
    }
}
