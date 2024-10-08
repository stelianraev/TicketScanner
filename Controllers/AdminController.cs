﻿using AutoMapper;
using CheckIN.Data.Model;
using CheckIN.Models;
using CheckIN.Models.TITo;
using CheckIN.Models.TITo.Ticket;
using CheckIN.Models.TITo.Webhook;
using CheckIN.Models.ViewModels;
using CheckIN.Services;
using CheckIN.Services.DbContext;
using CheckIN.Services.VCard.Helpers;
using CheckIN.Services.VCard.Models;
using Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using QRCoder;

namespace CheckIN.Controllers
{
    [Authorize(Roles = "Admin,Owner")]
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly DbService _dbService;
        private readonly ITiToService _tiToService;
        private readonly IMapper _mapper;

        public AdminController(ITiToService titoservice, DbService dbService, ApplicationDbContext context, UserManager<User> userManager, IMapper mapper, ILogger<AdminController> logger)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _tiToService = titoservice;
            _mapper = mapper;
            _dbService = dbService;
        }

        //[HttpGet]
        //public IActionResult Settings()
        //{
        //    var cookies = this.Request.Cookies;
        //    var settingsModel = new SettingsFormModel();

        //    if (cookies.ContainsKey("CheckInListId"))
        //    {
        //        settingsModel.CheckInListId = cookies["CheckInListId"]!;
        //    }
        //    //TODO Remove Tito Settings Must be on AdminLevel
        //    if (cookies.ContainsKey("TiToToken"))
        //    {
        //        settingsModel.TiToToken = cookies["TiToToken"]!;
        //    }
        //    if (cookies.ContainsKey("SelectedCameraId"))
        //    {
        //        settingsModel.SelectedCameraId = cookies["SelectedCameraId"]!;
        //    }
        //    if (cookies.ContainsKey("SelectedCameraLabel"))
        //    {
        //        settingsModel.SelectedCameraLabel = cookies["SelectedCameraLabel"]!;
        //    }
        //    //if (cookies.ContainsKey("PrinterName"))
        //    //{
        //    //    settingsModel.PrinterName = cookies["PrinterName"]!;
        //    //}

        //    return View(settingsModel);
        //}        

        [HttpGet]
        public IActionResult LoginWith()
        {
            return this.View();
        }


        [HttpGet]
        public IActionResult AdminDashboard()
        {
            return this.View();
        }

        [HttpGet]
        public async Task<IActionResult> AdminSettings()
        {
            var user = await _userManager.GetUserAsync(User);
            //var userCustomer = await _dbService.GetAllTitoAccountUserEventsAndEventsForCurrentCustomer(user?.Id);

            var userCustomer = await _context.UserCustomer
                .Include(x => x.User)
                .Include(x => x.Customer)
                    .ThenInclude(x => x.TitoAccounts)!
                        .ThenInclude(x => x.Events)
                .FirstOrDefaultAsync(x => x.UserId == user.Id!);

            var settingsModel = new SettingsFormModel();
            settingsModel.TitoSettings = new TitoSettings();
            settingsModel.TitoSettings.Authenticate = new Authenticate();
            settingsModel.TitoSettings.Authenticate.Accounts = userCustomer.Customer.TitoAccounts?.Select(x => x.Name).ToList();

            var selectedTitoAccount = userCustomer.Customer?.TitoAccounts?.FirstOrDefault(x => x.IsSelected);

            Event? selectedEvent = null;

            if (selectedTitoAccount != null)
            {
                settingsModel.TitoSettings.Authenticate.Events = selectedTitoAccount.Events.Select(x => x.Slug).ToList();
                selectedEvent = selectedTitoAccount.Events.FirstOrDefault(x => x.IsSelected);
                settingsModel.TitoSettings.Authenticate.SelectedAccount = selectedTitoAccount.Name;
            }
            else
            {
                var cookies = this.Request.Cookies;
                if (cookies.ContainsKey("SelectedTitoAccount"))
                {
                    settingsModel.TitoSettings!.Authenticate.SelectedAccount = cookies["SelectedTitoAccount"]!;
                }
            }

            if (selectedEvent != null)
            {
                settingsModel.TitoSettings.Authenticate.SelectedEvent = selectedEvent!.Slug;
            }

            if (userCustomer.Customer.TitoToken != null)
            {
                settingsModel.TitoSettings.Token = userCustomer.Customer.TitoToken;
            }

            return this.View(settingsModel);
        }


        [HttpPost]
        public async Task<IActionResult> AdminSettings(SettingsFormModel adminSettingsModel)
        {
            var user = await _userManager.GetUserAsync(User);
            //var userCustomer = await _dbService.GetAllTitoAccountUserEventsAndEventsForCurrentCustomer(user?.Id);

            var userCustomer = await _context.UserCustomer
                .Include(x => x.Customer)
                    .ThenInclude(x => x.TitoAccounts)!
                        .ThenInclude(x => x.Events)
                            .ThenInclude(x => x.Tickets)
                                 .FirstOrDefaultAsync(x => x.UserId.Equals(user.Id));

            var titoToken = userCustomer?.Customer.TitoToken;

            if (titoToken == null)
            {
                titoToken = adminSettingsModel.TitoSettings?.Token;
            }

            //var selectedtitoAccount = accountsAndEvents.FirstOrDefault(x => x.Name == adminSettingsModel.TitoSettings.Authenticate.SelectedAccount);
            var selectedtitoAccount = userCustomer?.Customer?.TitoAccounts?.FirstOrDefault(x => x.Name == adminSettingsModel.TitoSettings.Authenticate.SelectedAccount);
            var selectedEvent = selectedtitoAccount?.Events.FirstOrDefault(x => x.Slug == adminSettingsModel?.TitoSettings?.Authenticate?.SelectedEvent);

            if (selectedtitoAccount != null)
            {
                selectedtitoAccount.IsSelected = true;
                _context.Entry(selectedtitoAccount).State = EntityState.Modified;
            }

            //foreach (var acc in accountsAndEvents.Where(x => x.Id != selectedtitoAccount.Id))
            foreach (var acc in userCustomer!.Customer?.TitoAccounts.Where(x => x.Id != selectedtitoAccount?.Id))
            {
                acc.IsSelected = false;
            }

            if (selectedEvent != null)
            {
                selectedEvent.IsSelected = true;
                _context.Entry(selectedEvent).State = EntityState.Modified;
            }

            foreach (var acc in selectedtitoAccount.Events.Where(x => x.Slug != selectedEvent.Slug))
            {
                acc.IsSelected = false;
            }

            var tickets = await _tiToService.GetAllTicketsAsync(titoToken, selectedtitoAccount.Name, selectedEvent!.Slug!);

            var parsedTickets = JsonConvert.DeserializeObject<TitoTicketsResponse>(tickets);

            if (parsedTickets != null && parsedTickets.Tickets.Any())
            {
                foreach (var titoTicket in parsedTickets.Tickets)
                {
                    var existingTicket = selectedEvent.Tickets.FirstOrDefault(x => x.TicketId == titoTicket.Id);

                    if (existingTicket != null)
                    {
                        //var isTicketExist = existingTicketType?.TicketType.Tickets?.FirstOrDefault(x => x.TicketId == titoTicket.Id);

                        //if (isTicketExist != null)
                        //{
                        continue;
                        //}
                    }
                    //}
                    //else
                    //{
                    //    //var newTicketType = new TicketType()
                    //    //{
                    //    //    Name = titoTicket!.Type
                    //    //};

                    //    //_context.TicketTypes.Add(newTicketType);

                    //    //var newEventType = new EventTicketTypes();
                    //    //newEventType.EventId = selectedEvent.EventId;
                    //    //newEventType.TicketType = newTicketType;

                    //    //selectedEvent.TicketTypes.Add(newEventType);
                    //}

                    Contact contact = new Contact()
                    {
                        FirstName = titoTicket.FirstName,
                        LastName = titoTicket.LastName,
                        Email = titoTicket.Email,
                        Phones = new List<Phone>()
                            {
                                {new Phone(){Number = titoTicket.PhoneNumber} }
                            },
                        Organization = titoTicket.CompanyName,
                        Title = titoTicket.JobTitle,
                        XType = titoTicket.ReleaseTitle
                    };

                    var ticketTypeExist = selectedEvent.TicketTypes.FirstOrDefault(x => x.Name == titoTicket.ReleaseTitle);
                    if (ticketTypeExist == null)
                    {
                        TicketType newTicketType = new TicketType
                        {
                            Name = titoTicket.ReleaseTitle!
                        };

                        selectedEvent.TicketTypes.Add(newTicketType);
                    }

                    string vcardcontents = FileHelper.CreateVCard(contact);
                    byte[] qrCodeImage = null;

                    using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
                    {
                        QRCodeData qrCodeData = qrGenerator.CreateQrCode(vcardcontents, QRCodeGenerator.ECCLevel.Q);
                        using (PngByteQRCode qrCode = new PngByteQRCode(qrCodeData))
                        {
                            qrCodeImage = qrCode.GetGraphic(20);
                        }
                    }

                    string base64QrCode = Convert.ToBase64String(qrCodeImage);
                    var newTicket = new Ticket();
                    var ticket = _mapper.Map(titoTicket, newTicket);
                    ticket.QrCodeImage = base64QrCode;
                    selectedEvent.Tickets.Add(ticket);
                }
            }

            var webhooks = await _tiToService.GetWebhookEndpoint(titoToken, selectedtitoAccount.Name, selectedEvent.Slug!, null);
            var parsedWebhooks = JsonConvert.DeserializeObject<WebhookResponse>(webhooks);

            if (parsedWebhooks?.WebhookEndpints == null || !parsedWebhooks.WebhookEndpints.Any())
            {
                var newWebhook = await _tiToService.CreateWebhookEndpoint(titoToken, selectedtitoAccount.Name, selectedEvent.Slug!);
            }

            await _context.SaveChangesAsync();

            adminSettingsModel.TitoSettings.Authenticate.Accounts = userCustomer.Customer.TitoAccounts?.Select(x => x.Name).ToList();
            adminSettingsModel.TitoSettings.Authenticate.Events = selectedtitoAccount?.Events?.Select(x => x.Slug).ToList();

            this.Response.Cookies.Append("TiToToken", titoToken, new CookieOptions() { MaxAge = new TimeSpan(365, 0, 0, 0) });
            this.Response.Cookies.Append("SelectedTitoAccount", adminSettingsModel.TitoSettings?.Authenticate.SelectedAccount!, new CookieOptions() { MaxAge = new TimeSpan(365, 0, 0, 0) });

            return this.View(adminSettingsModel);
        }

        [HttpGet]
        public async Task<IActionResult> Users()
        {
            var user = await _userManager.GetUserAsync(User);
            var userCustomer = await _dbService.GetAllTitoAccountUserEventsAndEventsForCurrentCustomer(user?.Id);

            var usersFormModelList = new UsersFormModel();
            var selectedAccount = userCustomer?.Customer.TitoAccounts?.FirstOrDefault(x => x.IsSelected);
            var selectedEvent = selectedAccount?.Events.FirstOrDefault(x => x.IsSelected);

            if (selectedEvent == null)
            {
                ModelState.AddModelError("Event", "Event is not selected. Please check settings and select event");
                return this.View(usersFormModelList);
            }

            usersFormModelList.SelectedEvent = selectedEvent?.Title;

            usersFormModelList.TicketTypeList = selectedEvent.TicketTypes.Select(x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = x.Name, Text = x.Name }).ToList();

            usersFormModelList.Users = new List<UserFormModel>();

            foreach (var eventUser in selectedEvent.UserEvents)
            {
                var tempUsersViewModel = new UserFormModel();
                tempUsersViewModel.Email = eventUser.User.Email!;
                tempUsersViewModel.Password = eventUser.User.PasswordHash!;
                tempUsersViewModel.Permission = eventUser.User.Permission;
                tempUsersViewModel.Id = eventUser.User.Id;
                tempUsersViewModel.TicketTypesPermission = eventUser.User.UserEventTicketPermission.Select(x => x.TicketType.Name).ToList();

                usersFormModelList.Users.Add(tempUsersViewModel);
            }

            usersFormModelList.NewUser = new UserFormModel();
            return this.View(usersFormModelList);
        }


        [HttpGet]
        public async Task<IActionResult> Tickets()
        {
            var userCustomer = await GetCurrentUserCustomerAsync();

            var accountsAndEvents = await _context.TitoAccounts
                .Include(x => x.Events)
                    .ThenInclude(x => x.Tickets)
                    .FirstOrDefaultAsync(x => x.IsSelected);

            var selectedEvent = accountsAndEvents?.Events.FirstOrDefault(x => x.IsSelected);

            var ticketsViewList = new List<TicketViewModel>();

            foreach (var ticket in selectedEvent!.Tickets)
            {
                var newTicketViewModel = new TicketViewModel()
                {
                    FullName = ticket.FullName,
                    CompanyName = ticket.CompanyName,
                    JobPosition = ticket.JobTitle,
                    Email = ticket.Email,
                    Slug = ticket.Slug,
                    CreatedAt = ticket.CreatedAt,
                    IsCheckedIn = ticket.IsCheckedIn,
                    TicketType = ticket.TicketType,
                    PhoneNumber = ticket.PhoneNumber,
                    VCard = ticket.QrCodeImage
                };

                ticketsViewList.Add(newTicketViewModel);
            }

            return this.View(ticketsViewList);
        }

        [HttpPost]
        public IActionResult Events(Authenticate authenticate)
        {

            return this.View();
        }

        [HttpGet]
        public IActionResult CheckIn()
        {
            return RedirectToAction("Index", "CheckIn");
        }

        [HttpGet]
        public IActionResult Scanner()
        {
            return RedirectToAction("Scanning", "CheckIn");
        }

        private async Task<UserCustomer> GetCurrentUserCustomerAsync()
        {
            var userId = _userManager.GetUserId(User);
            var userCustomer = await _context.UserCustomer
                .Include(x => x.Customer)
                .FirstOrDefaultAsync(x => x.UserId.ToString() == userId);

            return userCustomer;
        }
    }
}
