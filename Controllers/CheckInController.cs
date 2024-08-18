using CheckIN.Data.Model;
using CheckIN.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CheckIN.Controllers
{
    [ApiController]
    [Route("CheckIn")]
    public class CheckInController : Controller
    {
        private readonly ILogger<CheckInController> _logger;
        //private readonly ITiToService _tiToService;

        public CheckInController(ILogger<CheckInController> logger /*ITiToService tiToService*/)
        {
            //_tiToService = tiToService;
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

        [HttpGet]
        [Route("Scanning")]
        public IActionResult Scanning()
        {
            return View();
        }
    }
}
