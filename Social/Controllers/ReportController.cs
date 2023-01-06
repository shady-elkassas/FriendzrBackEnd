using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Social.Entity.DBContext;
using Social.Entity.ModelView;
using Social.Services;
using Social.Services.Attributes;
using Social.Services.Helpers;
using Social.Services.ModelView;
using Social.Services.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Social.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    [ServiceFilter(typeof(AuthorizeUser))]
    public class ReportController : ControllerBase
    {
        private readonly IReportReasonService reportReasonService;
        private readonly IChatGroupReportService chatGroupReportService;
        private readonly AuthDBContext authDBContext;
        private readonly IStringLocalizer<SharedResource> localizer;
        private readonly IEventReportService eventReportService;
        private readonly IUserReportService userReportService;

        public ReportController(IReportReasonService reportReasonService, IChatGroupReportService chatGroupReportService, AuthDBContext authDBContext, IStringLocalizer<SharedResource> localizer, IEventReportService eventReportService, IUserReportService userReportService)
        {
            this.reportReasonService = reportReasonService;
            this.chatGroupReportService = chatGroupReportService;
            this.authDBContext = authDBContext;
            this.localizer = localizer;
            this.eventReportService = eventReportService;
            this.userReportService = userReportService;
        }
        
        [Route("sendReport")]
        [HttpPost]
        public async Task<IActionResult> sendReport([FromForm] ReportsVM model)
        {

            var ReportReasonObj = await reportReasonService.GetData(model?.ReportReasonID ?? new Guid());
            if (ReportReasonObj == null)
            {
                return StatusCode(StatusCodes.Status406NotAcceptable,
                    new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,
                         localizer["No ReportReason Matched With this ReportReasonID"], null));

            }

            if (model?.ReportType == ReportType.Event)
            {
                var EventData = authDBContext.EventData.FirstOrDefault(x => x.EntityId == model.ID);
                if (EventData == null)
                {

                    return StatusCode(StatusCodes.Status406NotAcceptable,
                            new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,
                             localizer["No Event Matched With this ID"], null));
                }
                else
                {
                    var EventReportVM = new EventReportVM()
                    {
                        CreatedBy_UserID = HttpContext.GetUser().Id,
                        EventDataID = EventData.Id,
                        Message = model.Message,
                        RegistrationDate = DateTime.UtcNow,
                        ReportReasonID = model.ReportReasonID.ToString()
                    };
                    var Result = await eventReportService.Create(EventReportVM);

                    return StatusCode(StatusCodes.Status200OK,
                 new ResponseModel<object>(Result.Code, Result.Status,
                 Result?.Message ?? "", null));
                }

            }
            else if (model?.ReportType == ReportType.User)
            {
                var User = authDBContext.Users.FirstOrDefault(x => x.Id == model.ID);
                if (User == null)
                {

                    return StatusCode(StatusCodes.Status406NotAcceptable,
                            new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,
                             localizer["No user Matched With this ID"], null));
                }
                else
                {
                    var UserReportVM = new UserReportVM()
                    {
                        CreatedBy_UserID = HttpContext.GetUser().Id,
                        Message = model.Message,

                        UserID = model.ID,
                        ReportReasonID = model.ReportReasonID.ToString()
                    };
                    var Result = await userReportService.Create(UserReportVM);

                    return StatusCode(Result.Code,
                 new ResponseModel<object>(Result.Code, Result.Status,
                 "", Result));
                }
            }
            else if (model?.ReportType == ReportType.ChatGroup)
            {
                Guid.TryParse(model.ID, out Guid ChatGroupID);
                var chatgroup = authDBContext.ChatGroups.FirstOrDefault(x => x.ID == ChatGroupID);
                if (chatgroup == null)
                {

                    return StatusCode(StatusCodes.Status406NotAcceptable,
                            new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,
                             localizer["No chatgroup Matched With this ID"], null));
                }
                else
                {
                    var ChatGroupReportVM = new ChatGroupReportVM()
                    {
                        CreatedBy_UserID = HttpContext.GetUser().Id,
                        ChatGroupID = chatgroup.ID,
                        Message = model.Message,
                        RegistrationDate = DateTime.UtcNow,
                        ReportReasonID = model.ReportReasonID.ToString()
                    };
                    var Result = await chatGroupReportService.Create(ChatGroupReportVM);

                    return StatusCode(StatusCodes.Status200OK,
                 new ResponseModel<object>(Result.Code, Result.Status,
                 Result?.Message ?? "", null));
                }
            }


            else
            {
                return StatusCode(StatusCodes.Status406NotAcceptable,
                    new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,
                         localizer["InvalidReportType"], null));
            }
        }

    }
}
