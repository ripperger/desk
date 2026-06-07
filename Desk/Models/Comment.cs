using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Desk.Constants;

namespace Desk.Models
{
    /// <summary>
    /// Represents a comment to the ticket.
    /// </summary>
    public class Comment : Content
    {

        /// <summary>
        /// The text content of the comment.
        /// </summary>
        [Required(ErrorMessage = Messages.Required)]
        [Display(Name = "Hozzászólás szövege")]
        [StringLength(280, ErrorMessage = "Nem lehet hosszabb 280 karakternél.")]
        public string Text { get; set; } = default!;

        /// <summary>
        /// Indicates whether the comment is visible to the reporter.
        /// </summary>
        [Display(Name = "Bejelentő számára látható")]
        public bool IsVisibleToReporter { get; set; } = false;

        public override string ToString()
        {
            return $"{Text}";
        }

    }
}
