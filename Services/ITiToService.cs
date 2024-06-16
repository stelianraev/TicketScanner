using CheckIN.Models.TITo;

namespace CheckIN.Services
{
    public interface ITiToService
    {
        public Task<HttpResponseMessage> GetTickets(string checkListId);

        //public Task<(string ticketContent, byte[] vCardContent)> GetTicketAndVCardAsync(string titoToken, string checkInListId, string ticketId);

        public Task<string> GetTicketAsync(string titoToken, string checkListId, string ticketId);

        public Task<byte[]> GetVCardAsync(string titoToken, string ticketId);

        public Task<string> Connect(string titoToken);
    }
}
