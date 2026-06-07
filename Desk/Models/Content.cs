using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Desk.Models
{
    /// <summary>
    /// Represents content of a ticket as comments, attachments, events, etc.
    /// </summary>
    public class Content
    {
        /// <summary>
        /// The ID of the content.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The date and time of the content.
        /// </summary>
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:MMM d. HH:mm}")]
        public DateTime Timestamp { get; set; } = DateTime.Now;

        // Db:Users
        /// <summary>
        /// The identifier of the user associated with the content.
        /// </summary>
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        /// <summary>
        /// The user associated with the content.
        /// </summary>
        public User? User { get; set; }

        // Db:Tickets
        /// <summary>
        /// The identifier of the ticket associated with the content.
        /// </summary>
        [ForeignKey(nameof(Ticket))]
        public int TicketId { get; set; }
        /// <summary>
        /// The ticket associated with the content.
        /// </summary>
        public Ticket? Ticket { get; set; }

    }
}
