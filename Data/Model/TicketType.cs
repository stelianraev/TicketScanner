using System.ComponentModel.DataAnnotations;

namespace CheckIN.Data.Model
{
    public class TicketType
    {
        [Key]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid EventId {  get; set; }

        public virtual Event Event { get; set; }

        public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}
