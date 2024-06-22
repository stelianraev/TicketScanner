using CheckIN.Data.Model;
using CheckIN.Models;
using CheckIN.Models.TITo;
using Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CheckIN.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public UserController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Settings(TitoSettings? titoSettings)
        {
            var cookies = this.Request.Cookies;
            var settingsModel = new SettingsFormModel();
            settingsModel.TitoSettings = titoSettings;

            if (cookies.ContainsKey("TiToToken"))
            {
                settingsModel.TitoSettings!.Token = cookies["TiToToken"]!;
            }

            return this.View(settingsModel);
        }

        [HttpPost]
        public async Task<IActionResult> Settings(SettingsFormModel settingsModel)
        {
            //if (settingsModel.CheckInListId == null)
            //{
            //    this.ModelState.AddModelError("CheckInListId", "Please fill valid CheckInListId from ti.to");
            //}
            if (string.IsNullOrEmpty(settingsModel.TitoSettings!.Token))
            {
                var user = await _userManager.GetUserAsync(User);

                var userCustomer = await _context.UserCustomer.FirstOrDefaultAsync(x => x.UserId == user.Id);
                //var customer = _context.Customers.FirstOrDefault(x => x.Id == user!.CustomerId);

                if(userCustomer.Customer != null)
                {
                    var customer = _context.Customers.FirstOrDefault(x => x.Id == userCustomer.CustomerId);
                    settingsModel.TitoSettings!.Token = customer!.TitoToken;

                    ModelState.Remove(nameof(settingsModel.TitoSettings.Token));
                }
                else
                {
                    this.ModelState.AddModelError("Customer", "Cannot find customer related to this user");
                }                
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
            this.Response.Cookies.Append("TiToToken", settingsModel.TitoSettings!.Token!, new CookieOptions() { MaxAge = new TimeSpan(365, 0, 0, 0) });
            this.Response.Cookies.Append("SelectedCameraId", settingsModel.SelectedCameraId!, new CookieOptions() { MaxAge = new TimeSpan(365, 0, 0, 0) });
            this.Response.Cookies.Append("SelectedCameraLabel", settingsModel.SelectedCameraLabel!, new CookieOptions() { MaxAge = new TimeSpan(365, 0, 0, 0) });
            //this.Response.Cookies.Append("PrinterName", settingsModel.PrinterName!, new CookieOptions() { MaxAge = new TimeSpan(365, 0, 0, 0) });

            return RedirectToAction("Index", "CheckIn");
        }
    }
}
