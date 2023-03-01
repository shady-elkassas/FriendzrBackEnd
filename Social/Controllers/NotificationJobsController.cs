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
        /// <summary>
        /// Test Email Jobs.
        /// </summary>
        /// <returns></returns>
        [HttpGet("testEmail")]
        public async Task<IActionResult> Get()
        {
            await _pushNotification.SendWelcomeEmailForTest("alaa.adel.fcis@gmail.com");
            await _pushNotification.SendCompleteProfileEmailForTest("alaa.adel.fcis@gmail.com");
           
            return Ok("email send");
        }
       
    }
}
