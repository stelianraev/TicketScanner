namespace CheckIN.Models
{
    public class VCardParsingModel
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string FullName { get; set; }
        public string Organization { get; set; }
        public string JobTitle { get; set; }
        public string Email { get; set; }
        public string TicketType { get; set; }
        public Dictionary<string, string> AdditionalFields { get; set; } = new Dictionary<string, string>();
    }
}
