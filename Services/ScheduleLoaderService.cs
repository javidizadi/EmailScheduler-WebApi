using Microsoft.EntityFrameworkCore;
using Email_Scheduler_WebApi.Data;
using Email_Scheduler_WebApi.Models;


namespace Email_Scheduler_WebApi.Services;

public class ScheduleLoaderService : IHostedService
{
    private readonly EmailScheduleHandler _handler;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ScheduleLoaderService> _logger;

    public ScheduleLoaderService(EmailScheduleHandler handler, IServiceProvider serviceProvider,
        ILogger<ScheduleLoaderService> logger)
    {
        _handler = handler;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Schedules Loader Service Started...");

        List<EmailSchedule> schedules;

        using (var scope = _serviceProvider.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            schedules = await db.EmailSchedules?
                .Where(e => !e.IsCompleted && e.SendTime >= DateTime.Now.AddMinutes(-15))
                .ToListAsync(cancellationToken)!;
        }

        foreach (var schedule in schedules)
        {
            if (schedule.SendTime <= DateTime.Now)
            {
                schedule.SendTime = DateTime.Now.AddMinutes(1);
            }

            _handler.AddSchedule(schedule);
        }

        _logger.LogInformation("Schedules Loading Completed...");

        await StopAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}