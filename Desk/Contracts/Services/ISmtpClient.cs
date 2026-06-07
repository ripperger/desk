using MimeKit;

/// <summary>
/// Interface for an SMTP client
/// </summary>
public interface ISmtpClient
{
    /// <summary>
    /// Validate a list of certificates
    /// </summary>
    /// <param name="certificateNames"></param>
    void ValidateCertificate(List<string> certificateNames);
    
    /// <summary>
    /// Connect to an SMTP server
    /// </summary>
    /// <param name="host"></param>
    /// <param name="port"></param>
    /// <param name="useSsl"></param>
    /// <returns></returns>
    Task ConnectAsync(string host, int port, bool useSsl);
    
    /// <summary>
    /// Send an email
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    Task SendAsync(MimeMessage message);
    
    /// <summary>
    /// Disconnect from an SMTP server
    /// </summary>
    /// <param name="quit"></param>
    /// <returns></returns>
    Task DisconnectAsync(bool quit);
    
    /// <summary>
    /// Dispose of the SMTP client
    /// </summary>
    void Dispose();


}
