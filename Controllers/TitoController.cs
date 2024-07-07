using AutoMapper;
using Azure;
using CheckIN.Data.Model;
using CheckIN.Models;
using CheckIN.Models.TITo;
using CheckIN.Models.TITo.Event;
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
        private readonly IMapper _mapper;
        private readonly ILogger<CheckInController> _logger;
        private readonly ITiToService _tiToService;
        //private readonly string? _titoToken;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        private static TicketViewModel? transferTicketModel;

        public TitoController(UserManager<User> userManager, IMapper mapper, ITiToService tiToService, ApplicationDbContext context, ILogger<CheckInController> logger)
        {
            _tiToService = tiToService;
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
        }

        [HttpPost]
        [Route("Authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] TitoSettings titoSettings)
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

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var connectToTitoResponse = await _tiToService.AuthenticateAsync(titoSettings!.Token);

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

                    if (isAccountExist == null)
                    {
                        var titoAcc = new TitoAccount()
                        {
                            Name = acc,
                            CustomerId = userCustomer.Customer.Id,
                            Events = new List<CheckIN.Data.Model.Event>()
                        };

                        userCustomer.Customer.TitoAccounts.Add(titoAcc);
                    }
                }

                var selectedAccount = userCustomer.Customer.TitoAccounts.FirstOrDefault(x => x.IsSelected);

                if (selectedAccount == null)
                {
                    ModelState.AddModelError("Event account", "You are not select an account");
                    return this.View(userCustomer);
                }

                var addEvents = await _tiToService.GetEventsAsync(userCustomer.Customer.TitoToken, selectedAccount.Name);

                var titoEventsDeserializer = JsonConvert.DeserializeObject<TitoEventResponse>(addEvents!);

                if (titoEventsDeserializer.Events != null)
                {
                    foreach (var titoEvent in titoEventsDeserializer.Events)
                    {
                        var eventEntity = _mapper.Map<Event>(titoEvent);
                        eventEntity.CustomerId = selectedAccount.CustomerId;

                        if (selectedAccount.Events == null)
                        {
                            selectedAccount.Events = new List<Event>();
                        }

                        var existingEvent = selectedAccount.Events.FirstOrDefault(x => x.Slug == eventEntity.Slug);

                        if (existingEvent != null)
                        {
                            existingEvent = eventEntity;
                        }

                        selectedAccount.Events.Add(eventEntity);
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


        /// <summary>
        /// Update Customers Tito Account and Customers Tito Events
        /// </summary>
        /// <param name="titoSettings"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("UpdateSettings")]
        public async Task<IActionResult> UpdateSettings([FromBody] TitoSettings titoSettings)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                var userCustomer = await _context.UserCustomer
                    .Include(x => x.Customer)
                        .ThenInclude(x => x.TitoAccounts)
                            .ThenInclude(x => x.Events)
                    .FirstOrDefaultAsync(x => x.UserId == user.Id);

                if (userCustomer?.Customer.TitoToken == null)
                {
                    ModelState.AddModelError(userCustomer.Customer.TitoToken, "You must add ti.to token");
                }

                if (!ModelState.IsValid)
                {
                    return this.View(ModelState);
                }

                var connectToTitoResponse = await _tiToService.AuthenticateAsync(titoSettings!.Token);

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

                    if (isAccountExist == null)
                    {
                        var titoAcc = new TitoAccount()
                        {
                            Name = acc,
                            CustomerId = userCustomer.Customer.Id,
                            Events = new List<CheckIN.Data.Model.Event>()
                        };

                        userCustomer.Customer.TitoAccounts.Add(titoAcc);
                    }
                }

                var selectedAccount = userCustomer.Customer.TitoAccounts.FirstOrDefault(x => x.IsSelected);

                if (selectedAccount == null)
                {
                    ModelState.AddModelError("Event account", "You are not select an account");
                    return this.View(userCustomer);
                }

                var addEvents = await _tiToService.GetEventsAsync(userCustomer.Customer.TitoToken, selectedAccount.Name);

                var titoEventsDeserializer = JsonConvert.DeserializeObject<TitoEventResponse>(addEvents!);

                if (titoEventsDeserializer.Events != null)
                {
                    foreach (var titoEvent in titoEventsDeserializer.Events)
                    {
                        var eventEntity = _mapper.Map<Event>(titoEvent);
                        eventEntity.CustomerId = selectedAccount.CustomerId;

                        if (selectedAccount.Events == null)
                        {
                            selectedAccount.Events = new List<Event>();
                        }

                        var existingEvent = selectedAccount.Events.FirstOrDefault(x => x.Slug == eventEntity.Slug);

                        if (existingEvent != null)
                        {
                            existingEvent = eventEntity;
                        }
                        else
                        {
                            selectedAccount.Events.Add(eventEntity);
                        }
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

        [HttpPost]
        [Route("Event")]
        public async Task<IActionResult> Event([FromBody] EventRequestModel titoSettings)
        {
            try
            {
                //var user = await _userManager.GetUserAsync(User);

                //var userCustomer = await _context.UserCustomer
                //    .Include(x => x.Customer)
                //    .Include(x => x.Customer.TitoAccounts)
                //    .FirstOrDefaultAsync(x => x.UserId == user.Id);

                //var titoAccountId = userCustomer.Customer.TitoAccounts.FirstOrDefault(x => x.IsSelected);

                //if (titoAccountId == null)
                //{
                var getEventsResponse = await _tiToService.GetEventsAsync(titoSettings!.Token, titoSettings!.Account);

                var result = JsonConvert.DeserializeObject<TitoEventResponse>(getEventsResponse);

                var user = await _userManager.GetUserAsync(User);

                var userCustomer = await _context.UserCustomer
                    .Include(x => x.Customer)
                        .ThenInclude(x => x.TitoAccounts)
                            .ThenInclude(x => x.Events)
                    .FirstOrDefaultAsync(x => x.UserId == user.Id!);


                //}

                //if (getEventsResponse == "null" || getEventsResponse == "Unauthorized")
                //{
                //    ModelState.AddModelError("Token", "Invalid token! Please enter a valid ti.to token.");
                //    return BadRequest(ModelState);
                //}

                //var authenticate = JsonConvert.DeserializeObject<Authenticate>(connectToTitoResponse!);

                //if (userCustomer!.Customer.TitoAccounts == null)
                //{
                //    userCustomer.Customer.TitoAccounts = new List<TitoAccount>();
                //}

                //foreach (var acc in authenticate.Accounts)
                //{
                //    var isAccountExist = userCustomer.Customer.TitoAccounts.FirstOrDefault(x => x.Name == acc);

                //    if (isAccountExist == null)
                //    {
                //        var titoAcc = new TitoAccount()
                //        {
                //            Name = acc,
                //            CustomerId = userCustomer.Customer.Id,
                //            Events = new List<Event>()
                //        };

                //        userCustomer.Customer.TitoAccounts.Add(titoAcc);
                //    }
                //}

                //await _context.SaveChangesAsync();

                return Ok(result);
            }
            catch (Exception ex)
            {
                // Logging exception
                return StatusCode(500, "Internal server error");
            }
        }
    }

    public class QRCodeDataModel
    {
        public string? QRCodeData { get; set; }
    }
}
