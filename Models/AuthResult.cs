namespace Email_Scheduler_WebApi.Models;

public class AuthResult
{
    public string? Token { get; set; }
    
    public bool Success { get; set; }
    
    public IEnumerable<string>? Errors { get; set; }
}