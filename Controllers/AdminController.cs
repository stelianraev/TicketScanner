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

            if(selectedEvent != null)
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

            var userCustomer = await _context.UserCustomer
                .Include(x => x.Customer)
                    .ThenInclude(x => x.TitoAccounts)
                        .ThenInclude(x => x.Events)
                .FirstOrDefaultAsync(x => x.UserId == user.Id!);

            //var customerSettings = _context.CustomerSettings.FirstOrDefault(x => x.CustomerId == user!.CustomerId);

            if (userCustomer.Customer.TitoToken == null)
            {
                userCustomer.Customer.TitoToken = adminSettingsModel.TitoSettings?.Token;                
            }

            var titoAccount = userCustomer.Customer.TitoAccounts.FirstOrDefault(x => x.Name == adminSettingsModel.TitoSettings.Authenticate.SelectedAccount);
            var titoEvent = titoAccount.Events.FirstOrDefault(x => x.Slug == adminSettingsModel?.TitoSettings?.Authenticate?.SelectedEvent);

            if (titoAccount != null)
            {
                titoAccount.IsSelected = true;
            }

            foreach (var acc in userCustomer.Customer.TitoAccounts.Where(x => x.Id != titoAccount.Id))
            {
                acc.IsSelected = false;
            }

            if (titoEvent != null)
            {
                titoEvent.IsSelected = true;
            }

            foreach (var acc in titoAccount.Events.Where(x => x.Slug != titoEvent.Slug))
            {
                acc.IsSelected = false;
            }

            await _context.SaveChangesAsync();

            adminSettingsModel.TitoSettings.Authenticate.Accounts = userCustomer.Customer.TitoAccounts?.Select(x => x.Name).ToList();
            adminSettingsModel.TitoSettings.Authenticate.Events = titoAccount?.Events?.Select(x => x.Title).ToList();

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
            var currentUser = await _userManager.GetUserAsync(User);

            var selectedEvent = _context.UserEvents
                .Include(x => x.Event)
                .FirstOrDefault(x => x.UserId == currentUser.Id && x.Event.IsSelected == true);

            var usersFormModelList = new UsersFormModel();

            if (selectedEvent == null)
            {
                ModelState.AddModelError("Event", "Event is not selected. Please check settings and selet event");
                return this.View(usersFormModelList);
            }

            var users = _context.UserEvents
                .Include(x => x.Event)
                .Include(x => x.User)
                .Where(x => x.EventId == selectedEvent.EventId && x.UserId != currentUser.Id)
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
    }
}
