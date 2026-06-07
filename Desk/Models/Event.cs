using Desk.Constants;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Desk.Models
{
    /// <summary>
    /// Represents a single event.
    /// </summary>
    public class Event : Content
    {

        /// <summary>
        /// The type of event.
        /// </summary>
        [Required]
        public string Type { get; set; } = default!;

        /// <summary>
        /// A brief description of the event.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Returns a string representation of the event.
        /// </summary>
        /// <returns>A string representation of the event.</returns>
        public override string ToString()
        {
            return $"{Dictionaries.EventType[Type]} - {Description}";
        }
    }
}
