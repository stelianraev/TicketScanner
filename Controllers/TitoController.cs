using CheckIN.Data.Model;
using CheckIN.Models.TITo;
using CheckIN.Models.ViewModels;
using CheckIN.Services;
using Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            try
            {
                var user = await _userManager.GetUserAsync(User);

                var userCustomer = await _context.UserCustomer
                    .Include(x => x.Customer)
                    .Include(x => x.Customer.TitoAccounts)
                    .FirstOrDefaultAsync(x => x.UserId == user.Id);

                if (!titoSettings.IsRevoked)
                {
                    if (userCustomer.Customer.TitoToken != null)
                    {
                        titoSettings.Token = userCustomer.Customer.TitoToken;
                    }
                    else
                    {
                        userCustomer.Customer.TitoToken = titoSettings.Token;

                        //await _context.SaveChangesAsync();
                    }
                }
                else if (titoSettings.IsRevoked && userCustomer != null)
                {
                    userCustomer.Customer.TitoToken = titoSettings.Token;
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

                if (userCustomer!.Customer.TitoAccounts == null)
                {
                    userCustomer.Customer.TitoAccounts = new List<TitoAccount>();
                }
               
                foreach (var acc in authenticate.Accounts)
                {
                    var isAccountExist = userCustomer.Customer.TitoAccounts.FirstOrDefault(x => x.Name == acc);

                    if(isAccountExist == null)
                    {
                        var titoAcc = new TitoAccount()
                        {
                            Name = acc,
                            CustomerId = userCustomer.Customer.Id,
                            Events = new List<Event>()
                        };

                        userCustomer.Customer.TitoAccounts.Add(titoAcc);
                    }                   
                }

                await _context.SaveChangesAsync();

                return RedirectToAction("AdminSettings", "Admin");
            }
            catch (Exception ex)
            {
                // Logging exception
                return StatusCode(500, "Internal server error");
            }
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
