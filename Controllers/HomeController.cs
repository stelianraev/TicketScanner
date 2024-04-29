using CheckIN.Models;
using CheckIN.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

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

        //[HttpPost]
        //public async Task<IActionResult> ProcessQRData([FromBody] QRCodeDataModel data)
        //{
        //    string qrCodeData = data.QRCodeData;
        //    var ticket = await _tiToService.GetTicket(qrCodeData);
        //    //var tickets = await _tiToService.GetTickets(qrCodeData);
           
        //    return Ok();
        //}

        public IActionResult Registrations()
        {
            // Add logic for handling registrations here
            return View();
        }
    }

    //public class QRCodeDataModel
    //{
    //    public string QRCodeData { get; set; }
    //}
}
