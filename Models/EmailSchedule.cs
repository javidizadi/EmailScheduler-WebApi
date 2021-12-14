using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Email_Scheduler_WebApi.Models;

public class EmailSchedule
{
    [Key] 
    public int Id { get; set; }

    [Required]
    public IdentityUser? User { get; set; }

    [Required]
    [MaxLength(100)]
    public string? SendTo { get; set; }

    [MaxLength(100)]
    public string? Title { get; set; }

    [Required]
    public string? Text { get; set; }

    [Required]
    public DateTime SendTime { get; set; }

    [Required]
    public bool IsCompleted { get; set; }
}