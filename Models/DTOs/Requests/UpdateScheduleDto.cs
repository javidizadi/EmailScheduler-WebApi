using System.ComponentModel.DataAnnotations;

namespace Email_Scheduler_WebApi.Models.DTOs.Requests;

public class UpdateScheduleDto : ScheduleDto
{
    [Required] public int Id { get; set; }
}