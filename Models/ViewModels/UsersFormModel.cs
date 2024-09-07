using Microsoft.AspNetCore.Mvc.Rendering;

namespace CheckIN.Models.ViewModels
{
    public class UsersFormModel
    {
        public List<UserFormModel>? Users { get; set; }
        public UserFormModel NewUser { get; set; }
        public string? SelectedEvent { get; set; }

        public List<SelectListItem>? TicketTypeList { get; set; }
    }
}
