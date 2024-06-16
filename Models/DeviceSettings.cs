using System.ComponentModel.DataAnnotations;

namespace CheckIN.Models
{
    public class DeviceSettings
    {
        [Required(ErrorMessage = "Please select a camera.")]
        [Display(Name = "Camera")]
        public string SelectedCameraId { get; set; }
        public string SelectedCameraLabel { get; set; }
    }
}
