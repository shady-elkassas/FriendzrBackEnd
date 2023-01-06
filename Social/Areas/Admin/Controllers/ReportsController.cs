using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Social.Entity.ModelView;
using Social.Services;
using Social.Services.Attributes;
using Social.Services.Helpers;
using Social.Services.ModelView;
using Social.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Social.Areas.Admin.Controllers
{
    [Area("Admin")]
    [ServiceFilter(typeof(AuthorizeUser))]
    public class ReportsController : Controller
    {
       
        private readonly IEventReportService eventReportService;
        private readonly IStringLocalizer<SharedResource> localizer;
        private readonly IUserReportService userReportService;

        public ReportsController(IEventReportService eventReportService, IStringLocalizer<SharedResource> _localizer,IUserReportService userReportService)
        {
       
            this.eventReportService = eventReportService;
            localizer = _localizer;
            this.userReportService = userReportService;
        }
        public IActionResult Events()
        {
            return View();
        } 
        public IActionResult Users()
        {
            return View();
        }     
        public IActionResult GetAllEventReports()
        {
            var Result = new { data = eventReportService.GetData().OrderByDescending(x => x.RegistrationDate) };
            return Ok(JObject.FromObject(Result, new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));
        }
        public IActionResult GetEventReports(string ID)
        {
            var Result = new { data = eventReportService.GetData(ID).OrderByDescending(x => x.RegistrationDate) };
            return Ok(JObject.FromObject(Result, new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));
        }
        public IActionResult GetAllUserReports()
        {
            var Result = new { data = userReportService.GetData().OrderByDescending(x => x.RegistrationDate) };
            return Ok(JObject.FromObject(Result, new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));
        }
        public IActionResult GetUserReports(string UserID)
        {
            var Result = new { data = userReportService.GetAllReportsInUser(UserID).OrderByDescending(x => x.RegistrationDate) };
            return Ok(JObject.FromObject(Result, new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));
        }
    }
}
