namespace CheckIN.Data.Model
{
    public class UserEvent
    {
        public string UserEventId { get; set; }
        public string UserId { get; set; }
        public string EventId { get; set; }

        public User User { get; set; }

        public Event Event { get; set; }
    }
}
