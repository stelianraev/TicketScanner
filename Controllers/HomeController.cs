using CheckIN.Models;
using CheckIN.Services;
using Microsoft.AspNetCore.Mvc;

namespace CheckIN.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ITiToService _tiToService;

        public HomeController(ILogger<HomeController> logger, ITiToService tiToService)
        {
            _tiToService = tiToService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CheckIn()
        {
            return View();
        }

        public IActionResult Registrations()
        {
            // Add logic for handling registrations here
            return View();
        }
    }
}
