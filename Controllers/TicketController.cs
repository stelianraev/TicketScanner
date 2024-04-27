using CheckIN.Models.ViewModels;
using CheckIN.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CheckIN.Controllers
{
    public class TicketController : Controller
    {

        private readonly ILogger<HomeController> _logger;
        private readonly ITiToService _tiToService;

        public TicketController(ILogger<HomeController> logger, ITiToService tiToService)
        {
            _tiToService = tiToService;
            _logger = logger;
        }

        public IActionResult Ticket(TicketViewModel ticket)
        {
            return View(ticket);
        }

        [HttpPost]
        public async Task<IActionResult> Ticket([FromBody] QRCodeDataModel data)
        {
            var checkListId = this.Request.Cookies["CheckInListId"];
            var token = this.Request.Cookies["TiToToken"];

            if (string.IsNullOrEmpty(checkListId) || string.IsNullOrEmpty(token))
            {
                return BadRequest("Required cookies are missing.");
            }

            try
            {
                string qrCodeData = data.QRCodeData;
                var response = await _tiToService.GetTicket(token, checkListId, qrCodeData);

                if (!response.IsSuccessStatusCode)
                {
                    // Log the error or handle it accordingly
                    return StatusCode((int)response.StatusCode, "Failed to retrieve ticket information.");
                }

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<TicketViewModel>(content);

                if (result == null)
                {
                    return NotFound("Ticket data is null after deserialization.");
                }

                return this.View();
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }

    public class QRCodeDataModel
    {
        public string QRCodeData { get; set; }
    }
}
