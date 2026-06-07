using Desk.Constants;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace Desk.Models
{
    public class BaseUser
    {
        [Key]
        public int Id { get; set; }

        [BindNever]                         // Don't bind this property
        public string? Sid { get; set; }    // From Active Directory: objectSid

        [Display(Name = "Törölt")]
        public bool IsDeleted { get; set; } = false;

        [Required(ErrorMessage = Messages.Required)]
        [Display(Name = "Felhasználónév")]
        [StringLength(30, ErrorMessage = Messages.StringLength30, MinimumLength = 3)]
        public string? UserName { get; set; }    // From Active Directory: samaccountname

        [Display(Name = "Teljes név")]
        [StringLength(280, MinimumLength = 3)]
        public string? FullName { get; set; }   // From Active Directory: displayName

        [Display(Name = "E-mail")]
        [EmailAddress(ErrorMessage = "Nem emailcím formátum.")]
        [StringLength(60, ErrorMessage = Messages.StringLength60, MinimumLength = 3)]
        public string? Email { get; set; }      // From Active Directory: mail

        [Display(Name = "Telefonszám")]
        [StringLength(30, ErrorMessage = Messages.StringLength30, MinimumLength = 3)]
        public string? Phone { get; set; }      // From Active Directory: telephoneNumber

        [Display(Name = "Szervezet")]
        public string? Department { get; set; }     // TODO

        [Display(Name = "VIP")]
        public bool IsVip { get; set; } = false;    // From AD (title, user_export.ps1): IsManager

        // METHODS

        /// <summary>
        /// Returns the distinct part of the Sid
        /// </summary>
        /// <returns></returns>
        public string FormatSid()
        {
            if (Sid == null) return string.Empty;

            return Sid.Split("-").Last();
        }
    }
}
