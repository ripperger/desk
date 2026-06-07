using Desk.Data;
using Desk.Models;
using MimeKit;

namespace Desk.Contracts.Services
{
    /// <summary>
    /// Interface for email related services
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Send an email asynchronously
        /// </summary>
        /// <param name="to"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        Task SendEmailAsync(MailboxAddress to, string subject, string body); // with default 'from'
        
        /// <summary>
        /// Send an email asynchronously to the reporter user with a url for the ticket
        /// It calls the SendEmailAsync method
        /// </summary>
        /// <param name="url">For the Details view of the ticket</param>
        /// <param name="ticket"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        Task SendEmailToReportedByAsync(string url, Ticket ticket, string subject, string message);
        
        /// <summary>
        /// Send an email asynchronously to the assigned group with a url for the ticket
        /// </summary>
        /// <param name="url">For the Details view of the ticket</param>
        /// <param name="ticket"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        Task SendEmailToAssignedAsync(string url, Ticket ticket, string subject, string message);
        
        /// <summary>
        /// Dispose the SmtpClient
        /// </summary>
        void DisposeSmtpClientAsync();
    }

}
