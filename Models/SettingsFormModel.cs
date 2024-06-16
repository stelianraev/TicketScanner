using CheckIN.Models.TITo;
using System.ComponentModel.DataAnnotations;

namespace CheckIN.Models
{
    public class SettingsFormModel : DeviceSettings
    {
        public TitoSettings? TitoSettings { get; set; }

        //[Required(ErrorMessage = "Please enter a valid CheckInNumber")]
        //[Display(Name = "CheckInNumber")]
        //public string CheckInListId { get; set; }

        //public string? PrinterName { get; set; } 

    }
}
