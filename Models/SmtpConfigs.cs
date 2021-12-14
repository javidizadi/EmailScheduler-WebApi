namespace Email_Scheduler_WebApi.Models;

public class SmtpConfigs
{
    public string Host { get; }
    
    public int Port { get; }
    
    public string From { get; }
    
    public string Username { get; }
    
    public string Password { get; }

    public SmtpConfigs(IConfiguration configuration)
    {
        Host = configuration["SMTP:Host"];

        Port = int.Parse(configuration["SMTP:Port"]);

        Username = configuration["SMTP:Username"];

        Password = configuration["SMTP:Password"];

        From = configuration["SMTP:From"];
    }
}