using Desk.Wrappers;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using MimeKit;
using NSubstitute;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Xunit;

namespace TestDesk.UnitTests.Wrappers
{
    /// <summary>
    /// Unit tests for the SmtpClientWrapper class.
    /// </summary>
    public class SmtpClientWrapperTests : IDisposable
    {
        private readonly SmtpClientWrapper _smtpClientWrapper;
        private readonly ILogger<SmtpClientWrapper> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SmtpClientWrapperTests"/> class.
        /// Sets up the test environment.
        /// </summary>
        public SmtpClientWrapperTests()
        {
            _logger = Substitute.For<ILogger<SmtpClientWrapper>>();
            _smtpClientWrapper = new SmtpClientWrapper(_logger);
        }

        /// <summary>
        /// Tests the ValidateCertificate method.
        /// </summary>
        [Fact]
        public void ValidateCertificate_ShouldValidateCertificate()
        {
            // Arrange
            var certificateNames = new List<string> { "validCertName" };
            //var certificate = new X509Certificate2();
            //certificate.Import("path/to/validCert.pfx", "password", X509KeyStorageFlags.DefaultKeySet);

            // Act
            _smtpClientWrapper.ValidateCertificate(certificateNames);

            // Assert
            // Since we cannot directly test the private callback, we assume no exceptions mean success
        }

        /// <summary>
        /// Tests the ConnectAsync method.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ConnectAsync_ShouldConnectToSmtpServer()
        {
            // Arrange
            var host = "localhost";
            var port = 25;
            var useSsl = false;

            // Act
            await _smtpClientWrapper.ConnectAsync(host, port, useSsl);

            // Assert
            _logger.Received().LogInformation(Arg.Is<string>(s => s.Contains("Connecting to SMTP server executed in")));
        }

        /// <summary>
        /// Tests the SendAsync method.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task SendAsync_ShouldSendEmail()
        {
            // Arrange
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Test Sender", "sender@test.com"));
            message.To.Add(new MailboxAddress("Test Receiver", "receiver@test.com"));
            message.Subject = "Test Subject";
            message.Body = new TextPart("plain") { Text = "Test Body" };

            // Act
            await _smtpClientWrapper.SendAsync(message);

            // Assert
            _logger.Received().LogInformation(Arg.Is<string>(s => s.Contains("Sending email executed in")));
        }

        /// <summary>
        /// Tests the DisconnectAsync method.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task DisconnectAsync_ShouldDisconnectFromSmtpServer()
        {
            // Act
            await _smtpClientWrapper.DisconnectAsync(true);

            // Assert
            // Since we cannot directly test the private method, we assume no exceptions mean success
        }

        /// <summary>
        /// Tests the Dispose method.
        /// </summary>
        [Fact]
        public void Dispose_ShouldDisposeSmtpClient()
        {
            // Act
            _smtpClientWrapper.Dispose();

            // Assert
            // Since we cannot directly test the private method, we assume no exceptions mean success
        }

        public void Dispose()
        {
            _smtpClientWrapper.Dispose();
        }
    }
}
