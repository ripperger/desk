using MailKit.Net.Smtp;
using Microsoft.Extensions.Hosting;
using MimeKit;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Desk.Wrappers
{

    public class SmtpClientWrapper : ISmtpClient
    {
        private readonly SmtpClient _client;
        private readonly ILogger<SmtpClientWrapper> _logger;
        public SmtpClientWrapper(ILogger<SmtpClientWrapper> logger)
        {
            _client = new SmtpClient();
            _logger = logger;

        }

        // Custom certificate validation
        public void ValidateCertificate(List<string> certificateNames)
        {
            _client.ServerCertificateValidationCallback = (s, c, h, e) =>
            {
                var certificate = c as X509Certificate2;
                if (certificate == null) return false;
                foreach (var validCertName in certificateNames)
                {
                    if (certificate.Subject.Contains($"CN={validCertName}"))
                    {
                        _logger.LogInformation("The certificate {validCertName} is valid.", validCertName);
                        return true;
                    }
                }
                return false;
            };
        }

        public async Task ConnectAsync(string host, int port, bool useSsl)
        {
            if (!_client.IsConnected)
            {
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();

                await _client.ConnectAsync(host, port, useSsl);

                stopwatch.Stop();
                var elapsedMs = stopwatch.ElapsedMilliseconds;

                _logger.LogInformation("Connecting to SMTP server executed in {ElapsedMilliseconds}ms", elapsedMs);
            }
        }

        public async Task SendAsync(MimeMessage message)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            await _client.SendAsync(message);

            stopwatch.Stop();
            var elapsedMs = stopwatch.ElapsedMilliseconds;

            _logger.LogInformation("Sending email executed in {ElapsedMilliseconds}ms", elapsedMs);
        }

        public async Task DisconnectAsync(bool quit)
        {
            await _client.DisconnectAsync(quit);
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
