using CheckIN.Models;
using Microsoft.AspNetCore.Mvc;

namespace CheckIN.Controllers
{
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;

        public AdminController(ILogger<AdminController> logger)
        {
            _logger = logger;
        }

        public IActionResult Settings()
        {
            var cookies = this.Request.Cookies;
            var settingsModel = new SettingsFormModel();

            if (cookies.ContainsKey("CheckInNumber"))
            {
                settingsModel.CheckInNumber = cookies["CheckInNumber"]!;
            }
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

            return View(settingsModel);
        }

        [HttpPost]
        public IActionResult Settings(SettingsFormModel settingsModel)
        {
            if (settingsModel.CheckInNumber == null)
            {
                this.ModelState.AddModelError("CheckInNumber", "Please fill valid CheckInNumber from ti.to");
            }
            if (settingsModel.TiToToken == null)
            {
                this.ModelState.AddModelError("Token", "Please fill valid Token from ti.to");
            }
            if (settingsModel.SelectedCameraId == null)
            {
                this.ModelState.AddModelError("Camera", "Please select camera");
            }

            if(!this.ModelState.IsValid)
            {
                return View(settingsModel);
            }

            this.Response.Cookies.Append("CheckInNumber", settingsModel.CheckInNumber!);
            this.Response.Cookies.Append("TiToToken", settingsModel.TiToToken!);
            this.Response.Cookies.Append("SelectedCameraId", settingsModel.SelectedCameraId!);
            this.Response.Cookies.Append("SelectedCameraLabel", settingsModel.SelectedCameraLabel!);

            return RedirectToAction("Index", "Home");
        }
    }
}
