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
                .FirstOrDefaultAsync(x => x.UserId == user.Id!);

            var settingsModel = new SettingsFormModel();
            settingsModel.TitoSettings = new TitoSettings();

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
                .FirstOrDefaultAsync(x => x.UserId == user.Id!);

            //var customerSettings = _context.CustomerSettings.FirstOrDefault(x => x.CustomerId == user!.CustomerId);

            if (userCustomer.Customer.TitoToken == null)
            {
                userCustomer.Customer.TitoToken = adminSettingsModel.TitoSettings.Token;

                await _context.SaveChangesAsync();
            }

            this.Response.Cookies.Append("TiToToken", adminSettingsModel.TitoSettings.Token!, new CookieOptions() { MaxAge = new TimeSpan(365, 0, 0, 0) });

            var connectToTitoResponse = await _tiToService.Connect(adminSettingsModel.TitoSettings.Token);
            //TODO handle if it is not connected
            var authenticate = JsonConvert.DeserializeObject<Authenticate>(connectToTitoResponse)!;

            adminSettingsModel.TitoSettings.Authenticate = authenticate;
            //return RedirectToAction("Settings", new { id = customerSettings.CustomerId });

            return this.View(adminSettingsModel);
        }

        [HttpGet]
        public async Task<IActionResult> Users(Guid customerId)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var users = _context.Users
                .Include(x => x.UserCustomers)
                .Where(x => x.Id == customerId && x.Id != currentUser.Id).ToList();

            var usersFormModelList = new UsersFormModel();
            usersFormModelList.Users = new List<UserFormModel>();

            foreach (var user in users)
            {
                var tempUsersViewModel = new UserFormModel();
                tempUsersViewModel.Email = user.Email!;
                tempUsersViewModel.Password = user.PasswordHash!;
                tempUsersViewModel.Permission = user.Permision;
                tempUsersViewModel.Id = user.Id;

                usersFormModelList.Users.Add(tempUsersViewModel);
            }

            return this.View(usersFormModelList);
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
