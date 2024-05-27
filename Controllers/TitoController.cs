using CheckIN.Models.TITo;
using CheckIN.Models.ViewModels;
using CheckIN.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CheckIN.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class TitoController : Controller
    {

        private readonly ILogger<HomeController> _logger;
        private readonly ITiToService _tiToService;
        private static TicketViewModel transferTicketModel;

        public TitoController(ILogger<HomeController> logger, ITiToService tiToService)
        {
            _tiToService = tiToService;
            _logger = logger;
        }

        [HttpGet]
        [Route("Ticket")]
        public IActionResult Ticket()
        {
            return View(transferTicketModel);
        }

        [HttpPost]
        [Route("Ticket")]
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

                var response = await _tiToService.GetTicketAndVCardAsync(token, checkListId, qrCodeData);
                string ticketContent = response.ticketContent;
                byte[] vCardContent = response.vCardContent;

                //var response = await _tiToService.GetTicket(token, checkListId, qrCodeData);

                var result = JsonConvert.DeserializeObject<TitoTicket>(ticketContent);
                //var getVCard = await _tiToService.GetVCard(token, qrCodeData);

                if (result == null)
                {
                    return NotFound("Ticket data is null after deserialization.");
                }

                var ticketModel = new TicketViewModel();
                ticketModel.FirstName = result.FirstName;
                ticketModel.LastName = result.LastName;
                ticketModel.CompanyName = result.CompanyName;
                ticketModel.Tags = result.Tags;
                ticketModel.VCard = Convert.ToBase64String(vCardContent);

                transferTicketModel = ticketModel;

                return this.View(transferTicketModel);

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
