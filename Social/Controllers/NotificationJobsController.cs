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

        [HttpGet("testEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> Get()
        {
            //RecurringJob.AddOrUpdate<IPushNotification>(j => j.SendUpdateProfileNotificationAfter24H(), cronExpression: "0 6 * * *", timeZone: TimeZoneInfo.Utc);
            //RecurringJob.AddOrUpdate<IPushNotification>(j => j.SendUpdateProfileNotificationAfter72H(), cronExpression: "0 6 * * *", timeZone: TimeZoneInfo.Utc);
            //RecurringJob.AddOrUpdate<IPushNotification>(j => j.SendNotificationForWomenOnly(), cronExpression: "0 6 * * *", timeZone: TimeZoneInfo.Utc);
            //RecurringJob.AddOrUpdate<IPushNotification>(j => j.SendNotificationIfUserHasRequestsUnanswered(), cronExpression: "0 6 * * *", timeZone: TimeZoneInfo.Utc);
            //RecurringJob.AddOrUpdate<IPushNotification>(j => j.SendNotificationIfUserHasMessagesNotRead(), cronExpression: "0 6 * * *", timeZone: TimeZoneInfo.Utc);
            //RecurringJob.AddOrUpdate<IPushNotification>(j => j.SendNotificationIfToUserHasMessagesNotRead(), cronExpression: "0 6 * * *", timeZone: TimeZoneInfo.Utc);
           await _pushNotification.SendWelcomeEmailForTest("shady_elkassas@hotmail.com");
           await _pushNotification.SendCompleteProfileEmailForTest("shady_elkassas@hotmail.com");
            return Ok("email send");
        }
       
    }
}
