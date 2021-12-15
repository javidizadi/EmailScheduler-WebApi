using System.ComponentModel.DataAnnotations;

namespace Email_Scheduler_WebApi.Models.DTOs.Requests;

public class ScheduleDto
{
    public string? Subject { get; set; }

    [Required] public string? Body { get; set; }

    [Required] [EmailAddress] public string? SendTo { get; set; }

    [Required] public DateTime SendTime { get; set; }
}