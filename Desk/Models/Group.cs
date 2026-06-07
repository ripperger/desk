using System.ComponentModel.DataAnnotations;
using Desk.Constants;

namespace Desk.Models
{
    public class Group
    {
        public int Id { get; set; }
        [Required(ErrorMessage = Messages.Required)]
        [StringLength(30, ErrorMessage = Messages.StringLength30, MinimumLength = 3)]
        public string Name { get; set; } = default!;

        /// <summary>
        /// A common email address for the group.
        /// Can be a distribution list.
        /// </summary>
        [Display(Name = "E-mail")]
        [EmailAddress(ErrorMessage = Messages.NotEmailFormat)]
        [StringLength(60, ErrorMessage = Messages.StringLength60, MinimumLength = 3)]
        public string? Email { get; set; }

        /// <summary>
        /// The ticket gets this role when it is assigned to this group.
        /// The user must have this role to see the ticket.
        /// It should be a Global Security Group (GSG) from the Active Directory.
        /// </summary>
        [Display(Name = "AD tagság")]
        public string? Role { get; set; }

        /// <summary>
        /// For the many to many relation with users
        /// </summary>
        public ICollection<UserGroup> UserGroups { get; set; } = new List<UserGroup>();
        
        /// <summary>
        /// Tickets that are assigned to the group
        /// </summary>
        public ICollection<Ticket>? Tickets { get; set; }

        
    }
}
