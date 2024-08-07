﻿using AutoMapper;
using CheckIN.Data.Model;
using CheckIN.Models;
using CheckIN.Models.TITo;
using CheckIN.Models.TITo.Ticket;
using CheckIN.Models.TITo.Webhook;
using CheckIN.Models.ViewModels;
using CheckIN.Services;
using Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace CheckIN.Controllers
{
    [Authorize(Roles = "Admin,Owner")]
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly ITiToService _tiToService;
        private readonly IMapper _mapper;

        public AdminController(ITiToService titoservice, ApplicationDbContext context, UserManager<User> userManager, IMapper mapper, ILogger<AdminController> logger)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _tiToService = titoservice;
            _mapper = mapper;
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
            var userCustomer = await _context.UserCustomer
                .Include(x => x.Customer)
                    .ThenInclude(x => x.TitoAccounts)
                    .ThenInclude(x => x.Events)
                .FirstOrDefaultAsync(x => x.UserId == user.Id!);

            var settingsModel = new SettingsFormModel();
            settingsModel.TitoSettings = new TitoSettings();
            settingsModel.TitoSettings.Authenticate = new Authenticate();
            settingsModel.TitoSettings.Authenticate.Accounts = userCustomer.Customer.TitoAccounts?.Select(x => x.Name).ToList();

            var selectedTitoAccount = userCustomer.Customer.TitoAccounts.FirstOrDefault(x => x.IsSelected);

            Event selectedEvent = null;

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
            var userCustomer = await GetCurrentUserCustomerAsync();

            var accountsAndEvents = await _context.TitoAccounts
                .Include(x => x.Events)
                    .ThenInclude(x => x.Tickets)
                .Where(x => x.CustomerId == userCustomer.CustomerId)
                .ToListAsync();           

            if (userCustomer.Customer.TitoToken == null)
            {
                userCustomer.Customer.TitoToken = adminSettingsModel.TitoSettings?.Token;
            }

            var selectedtitoAccount = accountsAndEvents.FirstOrDefault(x => x.Name == adminSettingsModel.TitoSettings.Authenticate.SelectedAccount);
            var selectedEvent = selectedtitoAccount?.Events.FirstOrDefault(x => x.Slug == adminSettingsModel?.TitoSettings?.Authenticate?.SelectedEvent);

            if (selectedtitoAccount != null)
            {
                selectedtitoAccount.IsSelected = true;
            }

            foreach (var acc in accountsAndEvents.Where(x => x.Id != selectedtitoAccount.Id))
            {
                acc.IsSelected = false;
            }

            if (selectedEvent != null)
            {
                selectedEvent.IsSelected = true;
            }

            foreach (var acc in selectedtitoAccount.Events.Where(x => x.Slug != selectedEvent.Slug))
            {
                acc.IsSelected = false;
            }

            var tickets = await _tiToService.GetAllTicketsAsync(userCustomer.Customer.TitoToken, selectedtitoAccount.Name, selectedEvent.Slug);

            var parsedTickets = JsonConvert.DeserializeObject<TitoTicketsResponse>(tickets);

            if(parsedTickets != null && parsedTickets.Tickets.Any())
            {
                foreach (var titoTicket in parsedTickets.Tickets)
                {
                    var isTicketExist = selectedEvent.Tickets.FirstOrDefault(x => x.TicketId == titoTicket.Id);

                    if (isTicketExist == null)
                    {
                        var newTicket = new Ticket();
                        var ticket = _mapper.Map(titoTicket, newTicket);
                        selectedEvent.Tickets.Add(ticket);
                    }
                }
            }

            var webhooks = await _tiToService.GetWebhookEndpoint(userCustomer.Customer.TitoToken, selectedtitoAccount.Name, selectedEvent.Slug, null);
            var parsedWebhooks = JsonConvert.DeserializeObject<WebhookResponse>(webhooks);

            if(parsedWebhooks?.WebhookEndpints == null || !parsedWebhooks.WebhookEndpints.Any())
            {
                var newWebhook = await _tiToService.CreateWebhookEndpoint(userCustomer.Customer.TitoToken, selectedtitoAccount.Name, selectedEvent.Slug);
            }

            await _context.SaveChangesAsync();

            adminSettingsModel.TitoSettings.Authenticate.Accounts = userCustomer.Customer.TitoAccounts?.Select(x => x.Name).ToList();
            adminSettingsModel.TitoSettings.Authenticate.Events = selectedtitoAccount?.Events?.Select(x => x.Slug).ToList();

            this.Response.Cookies.Append("TiToToken", adminSettingsModel.TitoSettings?.Token!, new CookieOptions() { MaxAge = new TimeSpan(365, 0, 0, 0) });
            this.Response.Cookies.Append("SelectedTitoAccount", adminSettingsModel.TitoSettings?.Authenticate.SelectedAccount!, new CookieOptions() { MaxAge = new TimeSpan(365, 0, 0, 0) });

            //var connectToTitoResponse = await _tiToService.Connect(adminSettingsModel.TitoSettings.Token);
            //TODO handle if it is not connected
            //var authenticate = JsonConvert.DeserializeObject<Authenticate>(connectToTitoResponse)!;

            //adminSettingsModel.TitoSettings.Authenticate = authenticate;
            //return RedirectToAction("Settings", new { id = customerSettings.CustomerId });

            return this.View(adminSettingsModel);
        }

        [HttpGet]
        public async Task<IActionResult> Users()
        {
            var userCustomer = await GetCurrentUserCustomerAsync();

            var selectedAccount = await _context.TitoAccounts
                .Include(x => x.Events)
                .FirstOrDefaultAsync(x => x.CustomerId == userCustomer.CustomerId && x.IsSelected == true);

            var selectedEvent = selectedAccount.Events
                .FirstOrDefault(x => x.IsSelected == true);

            var usersFormModelList = new UsersFormModel();
            usersFormModelList.SelectedEvent = selectedEvent.Title;

            if (selectedEvent == null)
            {
                ModelState.AddModelError("Event", "Event is not selected. Please check settings and selet event");
                return this.View(usersFormModelList);
            }

            var users = _context.UserEvents
                .Include(x => x.Event)
                .Include(x => x.User)
                .Where(x => x.EventId == selectedEvent.EventId && x.UserId != userCustomer.UserId)
                .ToList();

            usersFormModelList.Users = new List<UserFormModel>();

            foreach (var user in users)
            {
                var tempUsersViewModel = new UserFormModel();
                tempUsersViewModel.Email = user.User.Email!;
                tempUsersViewModel.Password = user.User.PasswordHash!;
                tempUsersViewModel.Permission = user.User.Permision;
                tempUsersViewModel.Id = user.User.Id;

                usersFormModelList.Users.Add(tempUsersViewModel);
            }

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

            foreach(var ticket in selectedEvent.Tickets)
            {
                var newTicketViewModel = new TicketViewModel()
                {
                    FullName = ticket.FullName,
                    CompanyName = ticket.CompanyName,
                    JobPosition = ticket.JobTitle,
                    Email = ticket.Email,
                    CreatedAt = ticket.CreatedAt,
                    IsScanned = ticket.IsScanned,
                    PhoneNumber = ticket.PhoneNumber
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
