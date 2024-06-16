using CheckIN.Configuration;
using CheckIN.Models.TITo;
using Microsoft.Extensions.Options;

namespace CheckIN.Services
{
    public class TiToService : ITiToService
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private readonly TiToConfiguration _tiToConfiguration;
        public TiToService(IOptions<TiToConfiguration> titoConfiguration)
        {
            _tiToConfiguration = titoConfiguration.Value;
        }

        public async Task<string> Connect (string titoToken)
        {
            string url = _tiToConfiguration.BaseUrl + "hello";

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            request.Headers.Add("Authorization", "Token token=" + titoToken);
            request.Headers.Add("Accept", "application/json");

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                return response.StatusCode.ToString();
            }

            var content = await response.Content.ReadAsStringAsync();

            return content;
        }

        public async Task<string> GetEvents(string titoToken, string accountSlug)
        {
            string url = _tiToConfiguration.BaseUrl + accountSlug + "/events";

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            request.Headers.Add("Authorization", "Token token=" + titoToken);
            request.Headers.Add("Accept", "application/json");

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                return response.StatusCode.ToString();
            }

            var content = await response.Content.ReadAsStringAsync();

            return content;
        }


        public async Task<string> GetTicketAsync(string titoToken, string checkInListId, string ticketId)
        {
            string endpoint = checkInListId + "/tickets/" + ticketId;
            string url = _tiToConfiguration.BaseUrl + endpoint;

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            request.Headers.Add("Authorization", "Token token=" + titoToken);
            request.Headers.Add("Accept", "application/json");

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                //TODO
                // Log the error or handle it accordingly
                return response.StatusCode.ToString();
            }

            var content = await response.Content.ReadAsStringAsync();

            return content;
        }

        public async Task<byte[]> GetVCardAsync(string titoToken, string ticketId)
        {
            string endpoint = ticketId + "/vcard";
            string url = _tiToConfiguration.VCardURL + endpoint;

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            request.Headers.Add("Authorization", "Token token=" + titoToken);
            request.Headers.Add("Accept", "application/json");

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                //TODO
                // Log the error or handle it accordingly
                Console.WriteLine("image error");
            }

            byte[] imageBytes = await response.Content.ReadAsByteArrayAsync();
            return imageBytes;
        }

        //public async Task<(string ticketContent, byte[] vCardContent)> GetTicketAndVCardAsync(string titoToken, string checkInListId, string ticketId)
        //{
        //    var getTicketTask = GetTicketAsync(titoToken, checkInListId, ticketId);

        //    var getVCardTask = GetVCardAsync(titoToken, ticketId);

        //    await Task.WhenAll(getTicketTask, getVCardTask);

        //    return (getTicketTask.Result, getVCardTask.Result);
        //}

        public async Task<HttpResponseMessage> GetTickets(string checkList)
        {
            string endpoint = "chk_pK7sbQvPYWAwocdhOMVfrvA/tickets";
            string url = _tiToConfiguration.BaseUrl + endpoint;

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            request.Headers.Add("Authorization", _tiToConfiguration.Token);

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Response Body: " + responseBody);
            }
            else
            {
                Console.WriteLine("Error: " + response.StatusCode);
            }

            return response;
        }
    }
}
