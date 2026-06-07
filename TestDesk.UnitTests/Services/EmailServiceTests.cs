using Desk.Data;
using Desk.Models;
using Desk.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using NSubstitute;
using System.Threading.Tasks;
using Xunit;

namespace TestDesk.UnitTests.Services
{
    public class EmailServiceTests : IDisposable
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;
        private readonly DeskContext _context;
        private readonly ISmtpClient _smtpClient;
        private readonly EmailService _emailService;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailServiceTests"/> class.
        /// Sets up the test environment.
        /// </summary>
        public EmailServiceTests()
        {
            _configuration = Substitute.For<IConfiguration>();
            _configuration["Smtp:Server"].Returns("localhost");
            _configuration["Smtp:Port"].Returns("25");
            _configuration["Smtp:EnableSsl"].Returns("false");
            _configuration["Smtp:CertificateNames"].Returns("cert1,cert2");

            _logger = Substitute.For<ILogger<EmailService>>();
            _smtpClient = Substitute.For<ISmtpClient>();

            var options = new DbContextOptionsBuilder<DeskContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabaseForEmailService")
                .Options;
            _context = new DeskContext(options);
            _context.Database.EnsureDeleted(); // Clear the database
            _context.Database.EnsureCreated(); // Recreate the database

            _emailService = new EmailService(_configuration, _logger, _smtpClient, _context);
        }

        /// <summary>
        /// Tests the SendEmailAsync method.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task SendEmailAsync_ShouldSendEmail()
        {
            // Arrange
            //var from = new MailboxAddress("Test Sender", "sender@test.com");
            var to = new MailboxAddress("Test Receiver", "receiver@test.com");
            var subject = "Test Subject";
            var body = "<p>Test Body</p>";

            // Act
            await _emailService.SendEmailAsync(to, subject, body);

            // Assert
            await _smtpClient.Received(1).ConnectAsync("localhost", 25, false);
            await _smtpClient.Received(1).SendAsync(Arg.Any<MimeMessage>());
            //await _smtpClient.Received(1).DisconnectAsync(true);
        }

        /// <summary>
        /// Tests the SendEmailToReportedByAsync method.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task SendEmailToReportedByAsync_ShouldSendEmailToReportedBy()
        {
            // Arrange         
            var user = new User { UserName = "reportedby", FullName = "Reported By", Email = "reportedby@test.com" };
            _context.Users.Add(user);
            var ticket = new Ticket { Categorization = "TestCategoryA", ReportedById = user.Id, Description = "Test Ticket" };
            _context.Tickets.Add(ticket);
            _context.SaveChanges();

            var url = "http://test.com/ticket/" + ticket.Id;
            var subject = "Test Subject";
            var message = "Test Message";

            // Act
            await _emailService.SendEmailToReportedByAsync(url, ticket, subject, message);

            // Assert
            await _smtpClient.Received(1).SendAsync(Arg.Any<MimeMessage>());
        }

        /// <summary>
        /// Tests the SendEmailToAssignedAsync method.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task SendEmailToAssignedAsync_ShouldSendEmailToAssignedGroup()
        {
            // Arrange
            var user = new User { UserName = "reportedby", FullName = "Reported By", Email = "reportedby@test.com" };
            _context.Users.Add(user);
            var group = new Group { Name = "Assigned Group", Email = "assignedgroup@test.com" };
            _context.Groups.Add(group);
            var ticket = new Ticket { Categorization = "TestCategoryA", ReportedById = user.Id, AssignedToGroupId = group.Id, Description = "Test Ticket" };
            _context.Tickets.Add(ticket);
            _context.SaveChanges();

            var url = "http://test.com/ticket/" + ticket.Id;
            var subject = "Test Subject";
            var message = "Test Message";

            // Act
            await _emailService.SendEmailToAssignedAsync(url, ticket, subject, message);

            // Assert
            await _smtpClient.Received(1).SendAsync(Arg.Any<MimeMessage>());
        }

        // Teardown
        public void Dispose()
        {
            _smtpClient.Dispose();
            _context.Dispose();
        }
    }
}
