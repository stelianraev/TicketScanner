namespace CheckIN.Models.ViewModels
{
    public class TicketViewModel
    {
        public int Id { get; set; }
        public string Slug { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string JobPosition { get; set; }
        public string PhoneNumber { get; set; }
        public string CompanyName { get; set; }
        public string TicketType { get; set; }
        public string Tags { get; set; }

        public bool IsCheckedIn { get; set; }

        public DateTime CreatedAt { get; set; }

        public string VCard { get; set; }
    }
}
