using Quartz;
using Quartz.Spi;
using Email_Scheduler_WebApi.Models;


namespace Email_Scheduler_WebApi.Services;

public class QuartzHostedService : IHostedService
{
    private readonly ISchedulerFactory _schedulerFactory;
    private readonly IJobFactory _jobFactory;
    private readonly EmailScheduleHandler _schedulesHandler;

    public QuartzHostedService(
        ISchedulerFactory schedulerFactory,
        IJobFactory jobFactory,
        EmailScheduleHandler schedulesHandler)
    {
        _schedulerFactory = schedulerFactory;
        _jobFactory = jobFactory;
        _schedulesHandler = schedulesHandler;
    }

    private IScheduler? _scheduler;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
        _scheduler.JobFactory = _jobFactory;

        _schedulesHandler.OnScheduled += OnScheduled;
        _schedulesHandler.OnUnscheduled += OnUnscheduled;

        await _scheduler.Start(cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _scheduler?.Shutdown(cancellationToken)!;

        _schedulesHandler.OnScheduled -= OnScheduled;
        _schedulesHandler.OnUnscheduled -= OnUnscheduled;
    }

    private static IJobDetail CreateJob(EmailSchedule schedule)
    {
        var dataMap = new Dictionary<string, EmailSchedule>
        {
            {"EmailScheduleDetail", schedule}
        };

        return JobBuilder.Create<EmailScheduleJob>()
            .UsingJobData(new JobDataMap(dataMap))
            .WithIdentity(schedule.Id.ToString())
            .Build();
    }

    private static ITrigger CreateTrigger(EmailSchedule schedule)
    {
        var cronExpression = schedule.SendTime.ToString("s m H d MMM ? yyyy");

        return TriggerBuilder.Create()
            .WithIdentity(schedule.Id.ToString())
            .WithCronSchedule(cronExpression)
            .StartNow()
            .Build();
    }

    private async void OnScheduled(object? sender, EmailSchedule schedule)
    {
        var job = CreateJob(schedule);
        var trigger = CreateTrigger(schedule);

        await _scheduler?.ScheduleJob(job, trigger)!;
    }

    private async void OnUnscheduled(object? sender, int scheduleId)
    {
        var jobKey = new JobKey(scheduleId.ToString());
        
        if (await _scheduler?.CheckExists(jobKey)!)
        {
            await _scheduler?.UnscheduleJob(new TriggerKey(scheduleId.ToString()))!;
        }
    }
}