using CheckIN.Models.TITo;

namespace CheckIN.Services
{
    public interface ITiToService
    {
        public Task<HttpResponseMessage> GetTickets(string checkListId);

        //public Task<(string ticketContent, byte[] vCardContent)> GetTicketAndVCardAsync(string titoToken, string checkInListId, string ticketId);

        public Task<string> GetTicketAsync(string titoToken, string checkListId, string ticketId);

        public Task<string> GetAllTicketsAsync(string titoToken, string accountSlug, string eventSlug);

        public Task<byte[]> GetVCardAsync(string titoToken, string ticketId);

        public Task<string> AuthenticateAsync(string titoToken);

        public Task<string> GetEventsAsync(string titoToken, string accountId);

        public Task<string> GetWebhookEndpoint(string titoToken, string accountSlug, string eventSlug, string id);

        public Task<string> CreateWebhookEndpoint(string titoToken, string accountSlug, string eventSlug);
    }
}
