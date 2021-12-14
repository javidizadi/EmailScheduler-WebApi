using Email_Scheduler_WebApi.Data;
using Email_Scheduler_WebApi.Models;

namespace Email_Scheduler_WebApi.Services;

public class SchedulerManager
{
    private readonly ApplicationDbContext _db;
    private readonly EmailScheduleHandler _emailScheduleHandler;

    public SchedulerManager(EmailScheduleHandler emailScheduleHandler, ApplicationDbContext db)
    {
        _db = db;
        _emailScheduleHandler = emailScheduleHandler;
    }

    public async Task Schedule(EmailSchedule schedule)
    {
        await _db.AddAsync(schedule);

        await _db.SaveChangesAsync();

        _emailScheduleHandler.AddSchedule(schedule);
    }

    public async Task UpdateSchedule(EmailSchedule newSchedule)
    {
        _db.EmailSchedules?.Update(newSchedule);

        await _db.SaveChangesAsync();

        _emailScheduleHandler.RemoveSchedule(newSchedule.Id);

        _emailScheduleHandler.AddSchedule(newSchedule);
    }

    public async Task UnSchedule(EmailSchedule schedule)
    {
        _db.Remove(schedule);

        await _db.SaveChangesAsync();

        _emailScheduleHandler.RemoveSchedule(schedule.Id);
    }
}