using CheckIN.Models;
using CheckIN.Services;
using Microsoft.AspNetCore.Mvc;

namespace CheckIN.Controllers
{
    [ApiController]
    [Route("")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ITiToService _tiToService;

        public HomeController(ILogger<HomeController> logger, ITiToService tiToService)
        {
            _tiToService = tiToService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("CheckIn")]
        public IActionResult CheckIn()
        {
            return View();
        }

        [HttpGet]
        [Route("Registrations")]
        public IActionResult Registrations()
        {
            // Add logic for handling registrations here
            return View();
        }
    }
}
