using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Social.Entity.ModelView;
using Social.Services.Services;
using System.Linq;

namespace Social.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    [ServiceFilter(typeof(Services.Attributes.AuthorizeUser))]

    public class ReportReasonController : ControllerBase
    {
        private readonly IReportReasonService reportReasonService;

        public ReportReasonController(IReportReasonService reportReasonService)
        {
            this.reportReasonService = reportReasonService;
        }
        [Route("getAllReportReasons")]
        [Consumes("application/x-www-form-urlencoded")]
        [HttpGet]

        public IActionResult getAllReportReasons()
        {
            var Result = reportReasonService.GetData().Where(x => x.IsActive == true).Select(x => new
            {
                x.ID,
                x.Name
            });
            return StatusCode(StatusCodes.Status200OK,
                         new ResponseModel<object>(StatusCodes.Status200OK, true,
                         "", Result));

        }
    }
}
