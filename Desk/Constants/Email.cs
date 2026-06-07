using Desk.Models;
using MimeKit;
using System;

namespace Desk.Constants
{
    /// <summary>
    /// Contains constants related to email.
    /// </summary>
    public static class Email
    {

        /// <summary>
        /// Subject of the email sent to the user when a new ticket is created.
        /// </summary>
        public const string NewTicketSubject = "Új Bejelentés";
        /// <summary>
        /// Default message of the email sent to the user when a new ticket is created.
        /// </summary>
        public const string NewTicketMessage = "Az alábbi bejelentés érkezett a rendszerbe:";

        public static string SetHtmlBody(string subject, string message, string? toName, string url, Ticket ticket, User? reportedBy, string? signature)
        {
            return $@"
                    <!DOCTYPE html> 
                    <html> 
                        <head> 
                            <title>{subject}</title> 
                        </head> 
                        <body style=""font-family:Segoe UI, sans-serif;""> 
                            <h4>Tisztelt {toName}!</h4> 
                            <p>{message}</p> 
                            <ul> 
                                <li>
                                    <strong>Hivakozási szám: </strong> 
                                    <a href=""{url}"">{ticket.ReferenceId()}</a>
                                </li>" 
                                + (reportedBy != null ? $@"
                                <li><strong>Bejelentő:</strong> {reportedBy?.FullName}</li>" : "")
                                + $@"
                                <li><strong>Összegzés:</strong> {ticket.GetSummary()}</li>
                            </ul> 
                            {signature}
                            <p>{Logo}</p>
                        </body> 
                    </html>";
        }
    }
}
