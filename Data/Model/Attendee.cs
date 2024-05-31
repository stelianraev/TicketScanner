namespace CheckIN.Data.Model
{
    public class Attendee
    {
        public int AttendeeID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumebr { get; set; }
        public string CompanyName { get; set; }
        public string TicketReference { get; set; }
        public string RegistrationReference { get; set; }
        public string Tags { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set;}
    }
}
