using CheckIN.Data.Model;
using CheckIN.Models;
using CheckIN.Models.TITo;
using CheckIN.Models.ViewModels;
using CheckIN.Services;
using Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CheckIN.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class TitoController : Controller
    {
        private readonly ILogger<CheckInController> _logger;
        private readonly ITiToService _tiToService;
        private readonly string? _titoToken;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        private static TicketViewModel? transferTicketModel;

        public TitoController(UserManager<User> userManager, ITiToService tiToService, ApplicationDbContext context, ILogger<CheckInController> logger)
        {
            _tiToService = tiToService;
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        [Route("Connect")]
        public async Task<IActionResult> Connect([FromBody] TitoSettings titoSettings)
        {
            var user = await _userManager.GetUserAsync(User);
            var customerSettings = _context.CustomerSettings.FirstOrDefault(x => x.CustomerId == user!.CustomerId);

            if (!titoSettings.IsRevoked)
            {
                if (customerSettings != null)
                {
                    titoSettings.Token = customerSettings.TitoToken;
                }
                else
                {
                    customerSettings = new CustomerSettings()
                    {
                        Id = Guid.NewGuid().ToString(),
                        CustomerId = user!.CustomerId,
                        TitoToken = titoSettings.Token
                    };

                    _context.CustomerSettings.Add(customerSettings);
                }
            }
            else if(titoSettings.IsRevoked && titoSettings != null)
            {
                customerSettings!.TitoToken = titoSettings.Token;
            }
            else
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
            }

            var connectToTitoResponse = await _tiToService.Connect(titoSettings!.Token);

            if (connectToTitoResponse == "null" || connectToTitoResponse == "Unauthorized")
            {
                ModelState.AddModelError("Token", "Invalid token! Please enter a valid ti.to token.");

                return BadRequest(ModelState);
            }            

            var authenticate = JsonConvert.DeserializeObject<Authenticate>(connectToTitoResponse!);

            foreach(var acc in authenticate.Accounts)
            {
                var titoAcc = new TitoAccount()
                {
                    Name = acc,
                    CustomerSettings = customerSettings,
                    Events = new List<Event>()
                };

                customerSettings.TitoAccounts.Add(titoAcc);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("AdminSettings", "Admin");
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
                string? qrCodeData = data.QRCodeData;

                //var response = await _tiToService.GetTicketAndVCardAsync(token, checkListId, qrCodeData);
                //string ticketContent = response.ticketContent;
                //byte[] vCardContent = response.vCardContent;

                var response = await _tiToService.GetTicketAsync(token, checkListId, qrCodeData);

                var result = JsonConvert.DeserializeObject<TitoTicket>(response);
                var getVCard = await _tiToService.GetVCardAsync(token, qrCodeData);

                if (result == null)
                {
                    return NotFound("Ticket data is null after deserialization.");
                }

                var ticketModel = new TicketViewModel();
                ticketModel.FirstName = result.FirstName;
                ticketModel.LastName = result.LastName;
                ticketModel.CompanyName = result.CompanyName;
                ticketModel.Tags = result.Tags;
                ticketModel.VCard = Convert.ToBase64String(getVCard);

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
        public string? QRCodeData { get; set; }
    }
}
