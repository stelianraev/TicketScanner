using CheckIN.Data.Model;
using System.ComponentModel.DataAnnotations;

namespace CheckIN.Models.TITo
{
    public class TitoSettings
    {
        [Required(ErrorMessage = "Please enter a valid token from ti.to")]
        public string Token { get; set; }

        public bool IsRevoked { get; set; }

        public Authenticate? Authenticate { get; set; }
    }
}
