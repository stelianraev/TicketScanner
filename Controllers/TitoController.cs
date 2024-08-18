using AutoMapper;
using CheckIN.Data.Model;
using CheckIN.Models.TITo;
using CheckIN.Models.TITo.Event;
using CheckIN.Models.TITo.Ticket;
using CheckIN.Models.ViewModels;
using CheckIN.Services;
using CheckIN.Services.DbContext;
using Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net.Sockets;

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
        private readonly DbService _dbService;

        private static TicketViewModel? transferTicketModel;

        public TitoController(UserManager<User> userManager, DbService dbService, IMapper mapper, ITiToService tiToService, ApplicationDbContext context, ILogger<CheckInController> logger)
        {
            _tiToService = tiToService;
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
            _dbService = dbService;
        }

        [HttpPost]
        [Route("Authenticate")]
        public async Task<IActionResult> AuthenticateAsync([FromBody] TitoSettings titoSettings)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                var userCustomer = await _context.UserCustomer
                    .Include(x => x.Customer)
                    .Include(x => x.Customer.TitoAccounts)
                    .FirstOrDefaultAsync(x => x.UserId == user!.Id);

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
                    userCustomer.Customer.TitoAccounts[0].IsSelected = true;
                    selectedAccount = userCustomer.Customer.TitoAccounts[0];


                    //return RedirectToAction("AdminSettings", "Admin");
                    //ModelState.AddModelError("Event account", "You are not select an account");
                    //return this.View(userCustomer);
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
                //TODO Reduce DB requests
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

                //Getting Accounts
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

                    //Getting Events
                    var accountEvents = await _tiToService.GetEventsAsync(userCustomer.Customer.TitoToken, acc);

                    var titoResponse = JsonConvert.DeserializeObject<TitoEventResponse>(accountEvents!);

                    if (titoResponse.Events != null)
                    {
                        foreach (var titoEvent in titoResponse.Events)
                        {
                            var eventEntity = _mapper.Map<Event>(titoEvent);
                            eventEntity.CustomerId = userCustomer.Customer.Id;

                            var isEventExist = await _context.Events.FirstOrDefaultAsync(x => x.Title == eventEntity.Title);

                            if (isEventExist == null)
                            {
                                await _context.Events.AddAsync(eventEntity);
                            }
                        }
                    }
                }

                var selectedAccount = userCustomer.Customer.TitoAccounts.FirstOrDefault(x => x.IsSelected);

                if (selectedAccount == null)
                {
                    ModelState.AddModelError("Event account", "You are not select an account");
                    return this.View(userCustomer);
                }

                //var addEvents = await _tiToService.GetEventsAsync(userCustomer.Customer.TitoToken, selectedAccount.Name);

                //var titoEventsDeserializer = JsonConvert.DeserializeObject<TitoEventResponse>(addEvents!);

                //if (titoEventsDeserializer.Events != null)
                //{
                //    foreach (var titoEvent in titoEventsDeserializer.Events)
                //    {
                //        var eventEntity = _mapper.Map<Event>(titoEvent);
                //        eventEntity.CustomerId = selectedAccount.CustomerId;

                //        if (selectedAccount.Events == null)
                //        {
                //            selectedAccount.Events = new List<Event>();
                //        }

                //        var existingEvent = selectedAccount.Events.FirstOrDefault(x => x.Slug == eventEntity.Slug);

                //        if (existingEvent != null)
                //        {
                //            existingEvent = eventEntity;
                //        }
                //        else
                //        {
                //            selectedAccount.Events.Add(eventEntity);
                //        }
                //    }
                //}

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

        //TODO Not for here. This endpoint response on scanned ticket
        [HttpPost]
        [Route("Ticket")]
        public async Task<IActionResult> Ticket([FromBody] QRCodeDataModel data)
        {
            var ticket = new Ticket();
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var customer = await _dbService.GetTitoAccountsAndEventsAndTicketsCurrentUserAsync(user?.Id);
                var account = customer?.Customer?.TitoAccounts?.FirstOrDefault(x => x.IsSelected);
                var selectedEvent = account?.Events.FirstOrDefault(x => x.IsSelected);
                ticket = selectedEvent?.Tickets.FirstOrDefault(x => x.Slug == data.QRCodeData);

                //TODO - posible problems when this is setted on True but something failed
                if (!ticket.IsCheckedIn)
                {
                    ticket.IsCheckedIn = true;
                }

                #region Check ticket in tito directry
                //check in tito
                //var checkListId = this.Request.Cookies["CheckInListId"];
                //var token = this.Request.Cookies["TiToToken"];

                //if (string.IsNullOrEmpty(checkListId) || string.IsNullOrEmpty(token))
                //{
                //    return BadRequest("Required cookies are missing.");
                //}

                //string? qrCodeData = data.QRCodeData;

                //var response = await _tiToService.GetTicketAsync(token, checkListId, qrCodeData);

                //var result = JsonConvert.DeserializeObject<TitoTicket>(response);
                //var getVCard = await _tiToService.GetVCardAsync(token, qrCodeData);

                //if (result == null)
                //{
                //    return NotFound("Ticket data is null after deserialization.");
                //}
                #endregion

                var ticketModel = new TicketViewModel();
                ticketModel.FullName = ticket.FullName;
                ticketModel.CompanyName = ticket.CompanyName;
                ticketModel.JobPosition = ticket.JobTitle;
                //ticketModel.VCard = ticket.;

                //ticketModel.FirstName = result.FirstName;
                //ticketModel.LastName = result.LastName;
                //ticketModel.CompanyName = result.CompanyName;
                //ticketModel.Tags = result.Tags;
                //ticketModel.VCard = Convert.ToBase64String(getVCard);

                transferTicketModel = ticketModel;

                return this.View(transferTicketModel);

            }
            catch (Exception ex)
            {
                // Log the exception
                ticket.IsCheckedIn = false;
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("Tickets")]
        public async Task<IActionResult> GetAllEventTickets(string titoToken, string accountSlug, string eventSlug)
        {
            //var checkListId = this.Request.Cookies["CheckInListId"];
            //var token = this.Request.Cookies["TiToToken"];

            if (string.IsNullOrEmpty(titoToken))
            {
                return BadRequest("Missing token.");
            }

            try
            {
                var response = await _tiToService.GetAllTicketsAsync(titoToken, accountSlug, eventSlug);

                var result = JsonConvert.DeserializeObject<TitoTicket[]>(response);

                if (result == null)
                {
                    return NotFound("Ticket data is null after deserialization.");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest("Bad Request");
                //todo
            }
        }

        [HttpPost]
        [Route("Event")]
        public async Task<IActionResult> Event([FromBody] EventRequestModel titoSettings)
        {
            try
            {
                var getEventsResponse = await _tiToService.GetEventsAsync(titoSettings!.Token, titoSettings!.Account);

                var result = JsonConvert.DeserializeObject<TitoEventResponse>(getEventsResponse);
                var titoAccount = await _context.TitoAccounts
                    .Include(x => x.Events)
                    .FirstOrDefaultAsync(x => x.Name == titoSettings.Account);


                var user = await _userManager.GetUserAsync(User);
                var userCustomer = await _context.UserCustomer
                    .Include(x => x.Customer)
                    .FirstOrDefaultAsync(x => x.UserId == user.Id);
                //var customer1 = await _context.User

                if (titoAccount.Events == null)
                {
                    titoAccount.Events = new List<Event>();
                }

                foreach (var ev in result.Events)
                {
                    if (!titoAccount.Events.Any(x => x.Title == ev.Title))
                    {
                        var newEvent = new Event();
                        _mapper.Map(ev, newEvent);

                        newEvent.UserEvents = new List<UserEvent>();

                        var newUserEvent = new UserEvent()
                        {
                            User = userCustomer.User,
                            Event = newEvent
                        };

                        newEvent.UserEvents.Add(newUserEvent);
                        newEvent.Customer = userCustomer.Customer;
                        titoAccount.Events.Add(newEvent);
                    }
                }

                await _context.SaveChangesAsync();

                return Ok(result);
            }
            catch (Exception ex)
            {
                // Logging exception
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        [Route("Webhook")]
        public async Task<IActionResult> Webhook([FromBody] TitoTicket ticket)
        {
            var accountsAndEvents = await _context.TitoAccounts
               .Include(x => x.Events)
                   .ThenInclude(x => x.Tickets)
               .FirstOrDefaultAsync(x => x.IsSelected);

            var selectedEvent = accountsAndEvents?.Events.FirstOrDefault(x => x.IsSelected);

            if (selectedEvent != null)
            {
                var existingTicket = selectedEvent.Tickets.FirstOrDefault(x => x.TicketId == ticket.Id);

                if (existingTicket == null)
                {
                    var newTicket = new Ticket();
                    var ticketMap = _mapper.Map(ticket, newTicket);
                    selectedEvent.Tickets.Add(ticketMap);
                }
            }

            await _context.SaveChangesAsync();

            return Ok();
        }
    }

    public class QRCodeDataModel
    {
        public string? QRCodeData { get; set; }
    }
}
