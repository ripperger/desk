using Desk.Constants;
using System.ComponentModel.DataAnnotations;

namespace Desk.Models.Authorization
{
    public class NewPassword : SpecialProperties
    {
        /// <summary>
        /// From SelectList
        /// </summary>
        [Required(ErrorMessage = Messages.Required)]
        [Display(Name = "Melyik rendszerhez?")]
        public string? ServiceType { get; set; }

        /// <summary>
        /// Users to give the authorization.
        /// Only usernames
        /// </summary>
        [Required(ErrorMessage = Messages.Required)]
        [Display(Name = "Felhasználónevek")]
        public IList<string> Users { get; set; } = default!;

    }

    public class NewPasswordViewModel : SpecialPropertiesViewModel
    {
        public new NewPassword? SpecialProperties { get; set; }
        public NewPasswordViewModel() : base()
        {
            SpecialProperties = new NewPassword();
        }

    }
}
