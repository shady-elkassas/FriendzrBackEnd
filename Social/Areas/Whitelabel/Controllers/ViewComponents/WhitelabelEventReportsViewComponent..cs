using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Social.Entity.DBContext;
using Social.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Social.Areas.WhiteLable.Controllers.ViewComponents
{
    public class WhitelabelEventReportsViewComponent : ViewComponent
    {
        private readonly IEventReportService eventReportService;
        private readonly AuthDBContext _authDBContext;
        private readonly IUserService _userService;
        public WhitelabelEventReportsViewComponent(IEventReportService eventReportService, AuthDBContext authDBContext, IUserService userService)
        {
            this.eventReportService = eventReportService;
            this._authDBContext = authDBContext;
            _userService = userService;
        }
        public IViewComponentResult Invoke()
        {

            string authorizationToken;
            HttpContext.Request.Cookies.TryGetValue("Authorization", out authorizationToken);

            var loggedinUser = this._userService.GetLoggedInUser(authorizationToken).Result;

            if (loggedinUser == null)
            {
                throw new Exception($"No User Found:{ StatusCodes.Status404NotFound }");
            }
           // var userId = loggedinUser.User.UserDetails.PrimaryId;
            var allEvents = _authDBContext.EventData.Where(n => n.UserId == loggedinUser.User.UserDetails.PrimaryId 
            && n.IsActive == true && (n.EventTypeListid == 5 || n.EventTypeListid == 6)).ToList();
            return View(eventReportService.GetData(allEvents.Select(e=>e.Id).ToList()).OrderByDescending(x => x.RegistrationDate));
        }
    }
}
