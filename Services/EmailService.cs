using System.Net;
using System.Net.Mail;
using System.Text;

using Email_Scheduler_WebApi.Models;

namespace Email_Scheduler_WebApi.Services;

public class EmailService
{
    private readonly SmtpClient _client;

    private readonly string _from;

    public EmailService(SmtpConfigs configs)
    {
        _client = new SmtpClient(configs.Host, configs.Port)
        {
            EnableSsl = true,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(configs.Username, configs.Password)
        };

        _from = configs.From;
    }

    public async Task SendMail(string? to, string? subject, string? body)
    {
        var message = new MailMessage(_from, to)
        {
            Subject = subject,
            Body = body,
            BodyEncoding = Encoding.UTF8,
            IsBodyHtml = true
        };
        await _client.SendMailAsync(message);
    }

    public void Dispose()
    {
        _client.Dispose();
    }
}