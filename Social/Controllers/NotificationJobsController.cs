using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Social.Services.PushNotification;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Social.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationJobsController : ControllerBase
    {
        private readonly IPushNotification _pushNotification;
        public NotificationJobsController(IPushNotification pushNotification)
        {
            _pushNotification = pushNotification;
        }

        [HttpGet("Recurring")]
        [AllowAnonymous]
        public async Task<IActionResult> Get()
        {
            //RecurringJob.AddOrUpdate<IPushNotification>(j => j.SendUpdateProfileNotificationAfter24H(), cronExpression: Cron.Daily, timeZone: TimeZoneInfo.Utc);
            //RecurringJob.AddOrUpdate<IPushNotification>(j => j.SendUpdateProfileNotificationAfter72H(), cronExpression: Cron.Daily, timeZone: TimeZoneInfo.Utc);
            //RecurringJob.AddOrUpdate<IPushNotification>(j => j.SendNotificationForWomenOnly(), cronExpression: Cron.Daily, timeZone: TimeZoneInfo.Utc);
            //RecurringJob.AddOrUpdate<IPushNotification>(j => j.SendNotificationIfUserHasRequestsUnanswered(), cronExpression: Cron.Daily, timeZone: TimeZoneInfo.Utc);
          await  _pushNotification.SendNotificationIfUserHasRequestsUnanswered();
            return Ok("Job Fired Successfully");
        }
       
    }
}
