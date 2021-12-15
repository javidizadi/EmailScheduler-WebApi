namespace Email_Scheduler_WebApi.Models.DTOs.Responses;

public class ScheduleResponseDto
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? Text { get; set; }

    public string? SendTo { get; set; }

    public DateTime SendTime { get; set; }

    public bool IsCompleted { get; set; }
}