using Email_Scheduler_WebApi.Data;
using Quartz;
using Email_Scheduler_WebApi.Models;


namespace Email_Scheduler_WebApi.Services;

public class EmailScheduleJob : IJob
{
    private readonly EmailService _emailService;
    private readonly IServiceProvider _serviceProvider;

    public EmailScheduleJob(EmailService emailService, IServiceProvider serviceProvider)
    {
        _emailService = emailService;
        _serviceProvider = serviceProvider;
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
        finally
        {
            _emailService.Dispose();
        }
    }
}