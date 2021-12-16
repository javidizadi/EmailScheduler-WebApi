using System.ComponentModel.DataAnnotations;

namespace Email_Scheduler_WebApi.Models.DTOs.Requests;
public class ChangePasswordDto
{
    [Required]
    public string? CurrentPassword { get; set; }

    [Required]
    public string? NewPassword { get; set; }
}