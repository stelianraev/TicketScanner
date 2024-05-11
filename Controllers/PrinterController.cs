using CheckIN.Models;
using CheckIN.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.Drawing.Printing;
using System.Text;
using System.Web;

namespace CheckIN.Controllers
{
    public class PrinterController : Controller
    {
        [HttpGet]
        public IActionResult GetPrinters()
        {
            string[] printers = PrinterSettings.InstalledPrinters.Cast<string>().ToArray();
            return Json(printers);
        }

        [HttpPost]
        public IActionResult Print([FromBody] PrintRequest request)
        {
            try
            {
                var printModel = new StringBuilder();
                printModel.AppendLine("Name:" + request.DocumentContent.FirstName + " " + request.DocumentContent.LastName);
                printModel.AppendLine("Email:" + request.DocumentContent.Email);
                printModel.AppendLine("Company Name:" + request.DocumentContent.CompanyName);
                printModel.AppendLine("Tags:" + request.DocumentContent.Tags);

                PrintDocument printDoc = new PrintDocument();
                printDoc.PrinterSettings.PrinterName = HttpUtility.UrlDecode(request.PrinterName);
                printDoc.PrintPage += (sender, e) =>
                {
                    // Print document content
                    e.Graphics.DrawString(printModel.ToString(), new Font("Arial", 12), Brushes.Black, new PointF(100, 100));
                };
                printDoc.Print();
                return Ok("Document printed successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error printing document: {ex.Message}");
            }
        }
    }
}

public class PrintRequest
{
    public string PrinterName { get; set; }
    public TicketViewModel DocumentContent { get; set; }
}
