using System.ComponentModel.DataAnnotations;
using Desk.Constants;

namespace Desk.Models
{
    public class Supplier
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(30, ErrorMessage = Messages.StringLength30, MinimumLength = 3)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Required to forward the ticket
        /// </summary>
        [Display(Name = "E-mail")]
        [EmailAddress(ErrorMessage = Messages.NotEmailFormat)]
        [StringLength(30, ErrorMessage = Messages.StringLength30, MinimumLength = 3)]
        public string? Email { get; set; }

        [StringLength(30, ErrorMessage = Messages.StringLength30, MinimumLength = 3)]
        public string? Phone { get; set; }
        
        /// <summary>
        /// Tickets that was forwarded to the supplier
        /// </summary>
        public ICollection<Ticket>? Tickets { get; set; }

    }
}
