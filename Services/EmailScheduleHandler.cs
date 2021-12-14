using Email_Scheduler_WebApi.Models;

namespace Email_Scheduler_WebApi.Services;

public class EmailScheduleHandler
{
    public event EventHandler<EmailSchedule> OnScheduled = delegate { };
    public event EventHandler<int> OnUnscheduled = delegate { };

    public void AddSchedule(EmailSchedule schedule)
    {
        OnScheduled.Invoke(null, schedule);
    }

    public void RemoveSchedule(int scheduleId)
    {
        OnUnscheduled.Invoke(null, scheduleId);
    }
}