namespace CheckIN.Models.ViewModels
{
    public class TicketViewModel
    {
        public int Id { get; set; }
        public string Slug { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string CompanyName { get; set; }
        public string ReleaseTitle { get; set; }
        public string Reference { get; set; }
        public string RegistrationReference { get; set; }
        public string Tags { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
