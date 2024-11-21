using AutoMapper;
using CheckIN.Data.Model;
using CheckIN.Models;
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
using System.Drawing;
using ZXing;

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
                            Events = new List<Event>()
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
                var user = await _userManager.GetUserAsync(User);

                var userCustomer = await _context.UserCustomer
                    .Include(x => x.Customer)
                        .ThenInclude(x => x.TitoAccounts)!
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
                                var newUserEvent = new UserEvent()
                                {
                                    User = user,
                                    Event = eventEntity
                                };

                                eventEntity.UserEvents.Add(newUserEvent);

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
                var customer = _dbService.GetTitoAccountsAndEventsAndTicketsCurrentUser(user?.Id);
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

        //TODO Not for here. This endpoint response on scanned ticket
        [HttpPost]
        [Route("TicketScan")]
        public async Task<IActionResult> TicketScan([FromBody] QRCodeDataModel data)
        {
            var ticket = new Ticket();
            try
            {
                var user = await _userManager.GetUserAsync(User);

                var userEvent = _context.UserEvents
                    .Include(x => x.User)
                        .ThenInclude(x => x.UserEventTicketPermission)
                            .ThenInclude(x => x.TicketType)
                    //.Include(x => x.Event)
                    //    .ThenInclude(x => x.Tickets)
                    .FirstOrDefault(x => x.UserId == user.Id);

                if(userEvent == null)
                {
                    return BadRequest("Invalid user");
                }

                var vcard = VCardParse(data.QRCodeData!);
                var userPermission = userEvent.User.UserEventTicketPermission.FirstOrDefault(x => x.TicketType.Name == vcard.TicketType);

                if (userEvent.User.Permission == Models.Enums.Permission.Owner
                    || userEvent.User.Permission == Models.Enums.Permission.Administrator
                    || userPermission != null)
                {

                }

                    var customer = _dbService.GetTitoAccountsAndEventsAndTicketsCurrentUser(user?.Id);
                var account = customer?.Customer?.TitoAccounts?.FirstOrDefault(x => x.IsSelected);
                var selectedEvent = account?.Events.FirstOrDefault(x => x.IsSelected);
                //ticket = selectedEvent?.Tickets.FirstOrDefault(x => x.Slug == data.QRCodeData);

                //Must Scan QR witch contains VCard and entranceQR information
                
                var ticketModel = new TicketViewModel();
                ticketModel.FullName = ticket.FullName;
                ticketModel.CompanyName = ticket.CompanyName;
                ticketModel.JobPosition = ticket.JobTitle;
                ticketModel.VCard = ticket.QrCodeImage;

                string qrCodeText = DecodeQRCodeFromBase64(ticket.QrCodeImage);

                Console.WriteLine("Decoded text from QR code: " + qrCodeText);

                return null;
            }
            catch (Exception ex)
            {
                return View("Error");
            }            
        }

        private string DecodeQRCodeFromBase64(string base64String)
        {
            // Step 1: Convert the Base64 string to a byte array
            byte[] qrCodeBytes = Convert.FromBase64String(base64String);

            // Step 2: Convert the byte array to an Image
            Image qrCodeImage = ConvertToImage(qrCodeBytes);

            // Step 3: Decode the QR code
            return DecodeQRCode(qrCodeImage);
        }

        private string DecodeQRCode(Image qrCodeImage)
        {
            Bitmap bitmap = new Bitmap(qrCodeImage);
            BarcodeReader reader = new BarcodeReader();
            var result = reader.Decode(bitmap);
            return result?.Text;
        }

        private Image ConvertToImage(byte[] byteArray)
        {
            using (MemoryStream ms = new MemoryStream(byteArray))
            {
                Image img = Image.FromStream(ms);
                return img;
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

                if (titoAccount == null)
                {
                    return NotFound("TitoAccount not found");
                }

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Unauthorized("User not authenticated");
                }

                var userCustomer = await _context.UserCustomer
                    .Include(x => x.Customer)
                    .FirstOrDefaultAsync(x => x.UserId == user.Id);

                if (userCustomer == null || userCustomer.Customer == null)
                {
                    return BadRequest("UserCustomer or Customer not found");
                }

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

                        var newUserEvent = new UserEvent
                        {
                            UserId = user.Id,
                            Event = newEvent
                        };

                        newEvent.UserEvents.Add(newUserEvent);
                        newEvent.Customer = userCustomer.Customer;

                        titoAccount.Events.Add(newEvent);
                    }
                }

                // Save changes to the database
                await _context.SaveChangesAsync();

                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log exception (add your logging mechanism here)
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

        private static VCardParsingModel VCardParse(string vCardData)
        {
            var vCard = new VCardParsingModel();
            var lines = vCardData.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                if (line.StartsWith("N:"))
                {
                    var parts = line.Substring(2).Split(';');
                    if (parts.Length >= 2)
                    {
                        vCard.LastName = parts[0];
                        vCard.FirstName = parts[1];
                    }
                }
                else if (line.StartsWith("FN:"))
                {
                    vCard.FullName = line.Substring(3);
                }
                else if (line.StartsWith("ORG:"))
                {
                    vCard.Organization = line.Substring(4);
                }
                else if (line.StartsWith("TITLE:"))
                {
                    vCard.JobTitle = line.Substring(6);
                }
                else if (line.StartsWith("EMAIL:"))
                {
                    vCard.Email = line.Substring(6);
                }
                else if (line.StartsWith("TicketType:"))
                {
                    vCard.TicketType = line.Substring(11);
                }
                else
                {
                    // Handle unknown fields and store them in AdditionalFields
                    var separatorIndex = line.IndexOf(':');
                    if (separatorIndex > 0)
                    {
                        var key = line.Substring(0, separatorIndex).Trim();
                        var value = line.Substring(separatorIndex + 1).Trim();
                        vCard.AdditionalFields[key] = value;
                    }
                }
            }

            return vCard;
        }
    }
}

   
