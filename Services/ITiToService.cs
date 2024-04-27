using CheckIN.Models.TITo;

namespace CheckIN.Services
{
    public interface ITiToService
    {
        public Task<HttpResponseMessage> GetTickets(string checkListId);

        public Task<HttpResponseMessage> GetTicket(string titoToken, string checkListId, string ticketId);
    }
}
