using System.ComponentModel.DataAnnotations;

namespace CheckIN.Data.Model
{
    public class UserEvent
    {
        [Key]
        public Guid UserEventId { get; set; }

        public Guid UserId { get; set; }

        public Guid EventId { get; set; }

        public User User { get; set; }

        public Event Event { get; set; }
    }
}
