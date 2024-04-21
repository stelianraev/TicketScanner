using CheckIN.Models.TITo;

namespace CheckIN.Services
{
    public interface ITiToService
    {
        public Task<HttpResponseMessage> GetTickets(string checkList);

        public Task<HttpResponseMessage> GetTicket(string ticketId);
    }
}
