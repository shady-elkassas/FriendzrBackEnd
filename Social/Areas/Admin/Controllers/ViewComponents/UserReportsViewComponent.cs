using Microsoft.AspNetCore.Mvc;
using Social.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Social.Areas.Admin.Controllers.ViewComponents
{
    public class UserReportsViewComponent : ViewComponent
    {
        private readonly IUserReportService userReportService;

        public UserReportsViewComponent(IUserReportService userReportService )
        {
            this.userReportService = userReportService;
        }
        public IViewComponentResult Invoke()
        {
            return View(userReportService.GetData().OrderByDescending(x => x.RegistrationDate));
        }
    }
}
