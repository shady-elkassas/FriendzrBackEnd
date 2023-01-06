using Microsoft.AspNetCore.Mvc;
using Social.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Social.Areas.Admin.Controllers.ViewComponents
{
    public class EventReportsViewComponent : ViewComponent
    {
        private readonly IEventReportService eventReportService;

        public EventReportsViewComponent(IEventReportService eventReportService )
        {
            this.eventReportService = eventReportService;
        }
        public IViewComponentResult Invoke()
        {
            return View(eventReportService.GetData().OrderByDescending(x => x.RegistrationDate));
        }
    }
}
