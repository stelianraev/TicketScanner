using CheckIN.Data.Model;
using CheckIN.Models;
using CheckIN.Models.ViewModels;
using Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CheckIN.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public AdminController(ApplicationDbContext context, UserManager<User> userManager, ILogger<AdminController> logger)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Settings()
        {
            var cookies = this.Request.Cookies;
            var settingsModel = new SettingsFormModel();

            if (cookies.ContainsKey("CheckInListId"))
            {
                settingsModel.CheckInListId = cookies["CheckInListId"]!;
            }
            //TODO Remove Tito Settings Must be on AdminLevel
            if (cookies.ContainsKey("TiToToken"))
            {
                settingsModel.TiToToken = cookies["TiToToken"]!;
            }
            if (cookies.ContainsKey("SelectedCameraId"))
            {
                settingsModel.SelectedCameraId = cookies["SelectedCameraId"]!;
            }
            if (cookies.ContainsKey("SelectedCameraLabel"))
            {
                settingsModel.SelectedCameraLabel = cookies["SelectedCameraLabel"]!;
            }
            //if (cookies.ContainsKey("PrinterName"))
            //{
            //    settingsModel.PrinterName = cookies["PrinterName"]!;
            //}

            return View(settingsModel);
        }

        [HttpPost]
        public IActionResult Settings(SettingsFormModel settingsModel)
        {
            //if (settingsModel.CheckInListId == null)
            //{
            //    this.ModelState.AddModelError("CheckInListId", "Please fill valid CheckInListId from ti.to");
            //}
            if (settingsModel.TiToToken == null)
            {
                this.ModelState.AddModelError("Token", "Please fill valid Token from ti.to");
            }
            if (settingsModel.SelectedCameraId == null)
            {
                this.ModelState.AddModelError("Camera", "Please select camera");
            }
            //if (settingsModel.PrinterName == null)
            //{
            //    this.ModelState.AddModelError("Printer", "Please select printer");
            //}

            if (!this.ModelState.IsValid)
            {
                return View(settingsModel);
            }

            //this.Response.Cookies.Append("CheckInListId", settingsModel.CheckInListId!, new CookieOptions() { MaxAge = new TimeSpan(365, 0, 0, 0)});
            this.Response.Cookies.Append("TiToToken", settingsModel.TiToToken!, new CookieOptions() { MaxAge = new TimeSpan(365, 0, 0, 0) });
            this.Response.Cookies.Append("SelectedCameraId", settingsModel.SelectedCameraId!, new CookieOptions() { MaxAge = new TimeSpan(365, 0, 0, 0) });
            this.Response.Cookies.Append("SelectedCameraLabel", settingsModel.SelectedCameraLabel!, new CookieOptions() { MaxAge = new TimeSpan(365, 0, 0, 0) });
            //this.Response.Cookies.Append("PrinterName", settingsModel.PrinterName!, new CookieOptions() { MaxAge = new TimeSpan(365, 0, 0, 0) });

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public IActionResult AdminDashboard()
        {
            var a = User.FindAll(ClaimTypes.Role).ToList();
            return this.View();
        }

        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public IActionResult AdminSettings()
        {
            var cookies = this.Request.Cookies;
            var settingsModel = new SettingsFormModel();

            if (cookies.ContainsKey("TiToToken"))
            {
                settingsModel.TiToToken = cookies["TiToToken"]!;
            }
           
            return this.View(settingsModel);
        }

        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminSettings(SettingsFormModel adminSettingsModel)
        {
            var user = await _userManager.GetUserAsync(User);
            var customerSettings = _context.CustomerSettings.FirstOrDefault(x => x.CustomerId == user!.CustomerId);

            if(customerSettings != null)
            {
                customerSettings.TitoToken = adminSettingsModel.TiToToken;
            }
            else
            {
                var newSettings = new CustomerSettings()
                {
                    Id = Guid.NewGuid().ToString(),
                    CustomerId = user!.CustomerId,
                    TitoToken = adminSettingsModel.TiToToken
                };

                _context.CustomerSettings.Add(newSettings);
                await _context.SaveChangesAsync();
            }

            this.Response.Cookies.Append("TiToToken", adminSettingsModel.TiToToken!, new CookieOptions() { MaxAge = new TimeSpan(365, 0, 0, 0) });

            return this.View();
        }

        [HttpGet]
        //[Authorize(Roles = "Admin")]
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

    }
}
