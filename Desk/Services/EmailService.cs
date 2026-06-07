using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Desk.Constants;
using Desk.Models;
using Microsoft.EntityFrameworkCore;
using Desk.Data;
using System.Net.Sockets;
using Desk.Controllers;
using Desk.Contracts.Services;
using System.Net.Security;
using NuGet.Protocol.Plugins;

namespace Desk.Services
{
    public class EmailService : IEmailService
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly bool _enableSsl;
        private readonly List<string> _certificateNames;
        private readonly ILogger<EmailService> _logger;
        private readonly ISmtpClient _smtpClient;
        private readonly DeskContext _context;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger, ISmtpClient smtpClient, DeskContext context)
        {
            _smtpServer = configuration["Smtp:Server"] ?? throw new ArgumentNullException("Smtp:Server");
            _smtpPort = int.Parse(configuration["Smtp:Port"] ?? throw new ArgumentNullException("Smtp:Port"));
            _enableSsl = bool.Parse(configuration["Smtp:EnableSsl"] ?? throw new ArgumentNullException("Smtp:EnableSsl"));
            string certificateNames = configuration["Smtp:CertificateNames"] ?? "";
            _certificateNames = certificateNames.Split(',').ToList<string>();
            _logger = logger;
            _smtpClient = smtpClient;
            _context = context;

        }

        /// <summary>
        /// Sending email asynchronously.
        /// The sender email ('from') is fixed.
        /// </summary>
        /// <param name="to"></param>
        /// <param name="subject"></param>
        /// <param name="body">With Html format</param>
        /// <returns></returns>
        public async Task SendEmailAsync(MailboxAddress to, string subject, string body)
        {
            // Message properties
            var message = new MimeMessage();
            message.From.Add(Email.deskNoReplyAddress);
            message.To.Add(to);
            message.Subject = subject;
            // message.Body = new TextPart("plain") { Text = body };    // Plain text body
            message.Body = new TextPart("html") { Text = body };        // HTML body

            try
            {
                // Custom certificate validation
                _smtpClient.ValidateCertificate(_certificateNames);
                
                // Connecting to SMTP server
                await _smtpClient.ConnectAsync(_smtpServer, _smtpPort, _enableSsl);

                // Sending email
                await _smtpClient.SendAsync(message);

                // Disconnecting at disposal
                //await _smtpClient.DisconnectAsync(true);
            }
            catch (SocketException ex)
            {
                _logger.LogError(ex, "Socket error while sending email. SMTP server is possibly not reachable.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while sending email");
                throw;
            }
            finally
            {
                //_smtpClient.Dispose(); // Disposing through the EmailService class
            }
        }

        /// <summary>
        /// Sending email to the user who reported the ticket
        /// </summary>
        /// <param name="url">For the ticket details</param>
        /// <param name="ticket"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendEmailToReportedByAsync(string url, Ticket ticket, string subject, string message)
        {
            var reportedBy = _context.Users.FirstOrDefault(u => u.Id == ticket.ReportedById);
            if (reportedBy != null && reportedBy.Email != null)
            {
                string htmlBody = Email.SetHtmlBody(subject, message, reportedBy.FullName, url, ticket, reportedBy, Email.Signature);
                subject = $"{subject} ({ticket.ReferenceId()})";
                MailboxAddress to = new MailboxAddress(reportedBy.FullName, reportedBy.Email);

                await SendEmailAsync(to, subject, htmlBody);
            }
        }

        /// <summary>
        /// Sending an email to the group which is assigned to the ticket
        /// </summary>
        /// <param name="url">For the ticket details</param>
        /// <param name="ticket"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendEmailToAssignedAsync(string url, Ticket ticket, string subject, string message)
        {
            var reportedBy = _context.Users.FirstOrDefault(u => u.Id == ticket.ReportedById);
            var group = _context.Groups.FirstOrDefault(g => g.Id == ticket.AssignedToGroupId);
            if (group != null && group.Email != null)
            {
                string htmlBody = Email.SetHtmlBody(subject, message, group.Name, url, ticket, reportedBy, null);
                subject = $"{subject} ({ticket.ReferenceId()})";
                MailboxAddress to = new MailboxAddress(group.Name, group.Email);

                await SendEmailAsync(to, subject, htmlBody);
            }
        }

        public async void DisposeSmtpClientAsync()
        {
            await _smtpClient.DisconnectAsync(true); 
            _smtpClient.Dispose();
        }
    }
}
