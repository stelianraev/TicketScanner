using CheckIN.Data.Model;
using CheckIN.Models;
using CheckIN.Models.TITo;
using CheckIN.Models.ViewModels;
using CheckIN.Services;
using Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CheckIN.Controllers
{
    [Authorize(Roles = "Admin")]
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
            var customerSettings = _context.CustomerSettings.FirstOrDefault(x => x.CustomerId == user!.CustomerId);

            var settingsModel = new SettingsFormModel();
            settingsModel.TitoSettings = new TitoSettings();

            if (customerSettings != null)
            {
                settingsModel.TitoSettings.Token = customerSettings.TitoToken;
            }

            return this.View(settingsModel);
        }


        [HttpPost]
        public async Task<IActionResult> AdminSettings(SettingsFormModel adminSettingsModel)
        {
            var user = await _userManager.GetUserAsync(User);
            var customerSettings = _context.CustomerSettings.FirstOrDefault(x => x.CustomerId == user!.CustomerId);

            if (customerSettings != null)
            {
                customerSettings.TitoToken = adminSettingsModel.TitoSettings.Token;
            }
            else
            {
                customerSettings = new CustomerSettings()
                {
                    Id = Guid.NewGuid().ToString(),
                    CustomerId = user!.CustomerId,
                    TitoToken = adminSettingsModel.TitoSettings.Token
                };

                _context.CustomerSettings.Add(customerSettings);
            }

            await _context.SaveChangesAsync();

            this.Response.Cookies.Append("TiToToken", adminSettingsModel.TitoSettings.Token!, new CookieOptions() { MaxAge = new TimeSpan(365, 0, 0, 0) });

            var connectToTitoResponse = await _tiToService.Connect(adminSettingsModel.TitoSettings.Token);
            //TODO handle if it is not connected
            var authenticate = JsonConvert.DeserializeObject<Authenticate>(connectToTitoResponse)!;

            adminSettingsModel.TitoSettings.Authenticate = authenticate;
            //return RedirectToAction("Settings", new { id = customerSettings.CustomerId });

            return this.View(adminSettingsModel);
        }

        [HttpGet]
        public async Task<IActionResult> Users()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var users = _context.Users.Where(x => x.CustomerId == currentUser!.CustomerId && x.Id != currentUser.Id).ToList();

            var usersViewModelList = new UsersViewModel();
            usersViewModelList.Users = new List<UserViewModel>();

            foreach (var user in users)
            {
                var tempUsersViewModel = new UserViewModel();
                tempUsersViewModel.Email = user.Email!;
                tempUsersViewModel.Password = user.PasswordHash!;
                tempUsersViewModel.Permission = user.Permision;
                tempUsersViewModel.Id = user.Id;

                usersViewModelList.Users.Add(tempUsersViewModel);
            }

            return this.View(usersViewModelList);
        }

        [HttpGet]
        public IActionResult Events()
        {
            return this.View();
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
    }
}
