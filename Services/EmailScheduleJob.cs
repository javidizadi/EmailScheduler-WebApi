using Email_Scheduler_WebApi.Data;
using Quartz;
using Email_Scheduler_WebApi.Models;


namespace Email_Scheduler_WebApi.Services;

public class EmailScheduleJob : IJob
{
    private readonly EmailService _emailService;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EmailScheduleJob> _logger;

    public EmailScheduleJob(EmailService emailService, IServiceProvider serviceProvider,
        ILogger<EmailScheduleJob> logger)
    {
        _emailService = emailService;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var emailSchedule = (EmailSchedule) context.JobDetail.JobDataMap.Get("EmailScheduleDetail");

        try
        {
            await _emailService.SendMail(emailSchedule.SendTo, emailSchedule.Title, emailSchedule.Text);

            emailSchedule.IsCompleted = true;

            using (var scope = _serviceProvider.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                db.Update(emailSchedule);

                await db.SaveChangesAsync();
            }
        }
        catch (Exception e)
        {
            _logger.LogError(new EventId(), e, "Error In Doing Job");
        }
        finally
        {
            _emailService.Dispose();
        }
    }
}