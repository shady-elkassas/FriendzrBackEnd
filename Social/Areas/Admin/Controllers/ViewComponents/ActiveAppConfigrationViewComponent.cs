using Microsoft.AspNetCore.Mvc;
using Social.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Social.Areas.Admin.Controllers.ViewComponents
{
    public class ActiveAppConfigrationViewComponent : ViewComponent
    {
        private readonly IAppConfigrationService appConfigrationService;

        public ActiveAppConfigrationViewComponent(IAppConfigrationService appConfigrationService)
        {
            this.appConfigrationService = appConfigrationService;
        }
        public IViewComponentResult Invoke()
        {
            // var a = appConfigrationService.GetData().FirstOrDefault(x => x.IsActive == true);
            return View(appConfigrationService.GetData().FirstOrDefault(x => x.IsActive == true));
        }
    }
}
