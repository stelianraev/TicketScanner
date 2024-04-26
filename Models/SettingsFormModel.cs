using System.ComponentModel.DataAnnotations;

namespace CheckIN.Models
{
    public class SettingsFormModel
    {
        [Url(ErrorMessage = "Please enter a valid token from ti.to")]
        [Display(Name = "Token")]
        public string TiToToken { get; set; }

        [Url(ErrorMessage = "Please enter a valid CheckInNumber")]
        [Display(Name = "CheckInNumber")]
        public string CheckInNumber { get; set; }

        [Url(ErrorMessage = "Please select a camera.")]
        [Display(Name = "Camera")]
        public string SelectedCameraId { get; set; }
        public string SelectedCameraLabel { get; set; }

    }
}
