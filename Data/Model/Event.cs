using System.ComponentModel.DataAnnotations;

namespace CheckIN.Data.Model
{
    public class Event
    {
        [Key]
        public string EventId { get; set; }

        public string CustomerId { get; set; }
        public Customer Customer { get; set; }

        public string EventName { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string TitoAccountId {  get; set; }

        public TitoAccount TitoAccount { get; set; }

        public virtual ICollection<UserEvent> UserEvents { get; set; }

        public virtual ICollection<Ticket> Tickets { get; set; }
    }
}
