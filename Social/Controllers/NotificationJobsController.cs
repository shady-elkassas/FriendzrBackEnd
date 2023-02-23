using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Social.Services.PushNotification;
using System;

namespace Social.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationJobsController : ControllerBase
    {

        public NotificationJobsController()
        {
        }

        [HttpGet("Recurring")]
        public IActionResult Get()
        {
            RecurringJob.AddOrUpdate<IPushNotification>(j => j.SendUpdateProfileNotificationAfter24H(), cronExpression: Cron.Daily, timeZone: TimeZoneInfo.Utc);
            RecurringJob.AddOrUpdate<IPushNotification>(j => j.SendUpdateProfileNotificationAfter72H(), cronExpression: Cron.Daily, timeZone: TimeZoneInfo.Utc);
            RecurringJob.AddOrUpdate<IPushNotification>(j => j.SendNotificationForWomenOnly(), cronExpression: Cron.Daily, timeZone: TimeZoneInfo.Utc);

            return Ok("Job Fired Successfully");
        }
       
    }
}
