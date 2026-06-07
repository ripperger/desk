using Desk.Constants;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Desk.Models
{
    /// <summary>
    /// Represents a user in the system that can log in and create, see or manage tickets.
    /// </summary>
    public class User : BaseUser
    {

        [DataType(DataType.Date)]
        [Display(Name = "Utolsó belépés")]
        public DateTime LastLogin { get; set; } = DateTime.Now;

        public ICollection<Ticket>? CreatedTickets { get; set; }
        public ICollection<Ticket>? ReportedTickets { get; set; }
        public ICollection<Ticket>? AssignedTickets { get; set; }
        public ICollection<Event>? Events { get; set; }
        public ICollection<Comment>? Comments { get; set; }


        // Many to many
        [Display(Name = "Csoport")]
        public ICollection<UserGroup> UserGroups { get; set; } = new List<UserGroup>();

        //public ICollection<UserSetting>? Settings { get; set; }

    }

    public class UserViewModel : User
    {
        public List<int> SelectedGroupIds { get; set; } = new List<int>();
        public List<SelectListItem> Groups { get; set; } = new List<SelectListItem>();
    }
}
