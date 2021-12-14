using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Email_Scheduler_WebApi.Models.DTOs.Requests;

public class UserCredentialsDto
{
    [Required] [EmailAddress] public string? Email { get; set; }

    [Required] [PasswordPropertyText] public string? Password { get; set; }
}