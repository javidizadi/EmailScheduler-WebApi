namespace Email_Scheduler_WebApi.Configuration;

public class AuthResult
{
    public string? Token { get; set; }
    
    public bool Success { get; set; }
    
    public IEnumerable<string>? Type { get; set; }
}