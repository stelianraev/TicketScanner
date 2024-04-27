using System.ComponentModel.DataAnnotations;

namespace CheckIN.Models
{
    public class SettingsFormModel
    {
        [Required(ErrorMessage = "Please enter a valid token from ti.to")]
        [Display(Name = "Token")]
        public string TiToToken { get; set; }

        [Required(ErrorMessage = "Please enter a valid CheckInNumber")]
        [Display(Name = "CheckInNumber")]
        public string CheckInListId { get; set; }

        [Required(ErrorMessage = "Please select a camera.")]
        [Display(Name = "Camera")]
        public string SelectedCameraId { get; set; }
        public string SelectedCameraLabel { get; set; }

        public string SelectedPrinterId { get; set; } 

    }
}
