using CheckIN.Data.Model;
using CheckIN.Models;
using CheckIN.Models.TITo;
using CheckIN.Models.ViewModels;
using CheckIN.Services;
using Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net;

namespace CheckIN.Controllers
{
    [Authorize(Roles = "Admin,Owner")]
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly ITiToService _tiToService;

        public AdminController(ITiToService titoservice, ApplicationDbContext context, UserManager<User> userManager, ILogger<AdminController> logger)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _tiToService = titoservice;
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
            //var user = await _userManager.GetUserAsync(User);

            var userCustomer = await GetCurrentUserCustomerAsync();

            var titoAccounts = await _context.TitoAccounts
                .Include(x => x.Customer)
                .Where(x => x.CustomerId == userCustomer.CustomerId)
                .ToListAsync();

            var events = await _context.Events
                .Where(x => x.CustomerId == userCustomer.CustomerId)
                .ToListAsync();

            //var userCustomer = await _context.UserCustomer
            //    .Include(x => x.User)
            //    .Include(x => x.Customer)
            //    .FirstOrDefaultAsync(x => x.UserId == user.Id);

            

            if (userCustomer.Customer.TitoToken == null)
            {
                userCustomer.Customer.TitoToken = adminSettingsModel.TitoSettings?.Token;
            }

            var selectedtitoAccount = titoAccounts.FirstOrDefault(x => x.Name == adminSettingsModel.TitoSettings.Authenticate.SelectedAccount);
            var selectedEvent = events.FirstOrDefault(x => x.Slug == adminSettingsModel?.TitoSettings?.Authenticate?.SelectedEvent);
            //var titoEvent = titoAccount.Events.FirstOrDefault(x => x.Slug == adminSettingsModel?.TitoSettings?.Authenticate?.SelectedEvent);

            if (selectedtitoAccount != null)
            {
                selectedtitoAccount.IsSelected = true;
            }

            foreach (var acc in titoAccounts.Where(x => x.Id != selectedtitoAccount.Id))
            {
                acc.IsSelected = false;
            }

            if (selectedEvent != null)
            {
                selectedEvent.IsSelected = true;
            }

            foreach (var acc in events.Where(x => x.Slug != selectedEvent.Slug))
            {
                acc.IsSelected = false;
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
            //var currentUser = await _userManager.GetUserAsync(User);
            var userCustomer = await GetCurrentUserCustomerAsync();

            //var userCustomer = await _context.UserCustomer
            //.Include(x => x.User)
            //.Include(x => x.Customer)
            //.FirstOrDefaultAsync(x => x.UserId == currentUser.Id);

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

        //TODO maybe for analytics
        //[HttpGet]
        //public IActionResult Events()
        //{
        //   return RedirectToAction("Event", "Tito");
        //}

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

        //[HttpGet]
        //public IActionResult Tickets()
        //{

        //}

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
