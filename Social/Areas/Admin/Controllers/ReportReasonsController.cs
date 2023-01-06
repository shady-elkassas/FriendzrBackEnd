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
    public class ReportReasonsController : Controller
    {
        private readonly IReportReasonService ReportReasonService;
        private readonly IStringLocalizer<SharedResource> localizer;

        public ReportReasonsController(IReportReasonService ReportReasonService, IStringLocalizer<SharedResource> _localizer)
        {
            this.ReportReasonService = ReportReasonService;
            localizer = _localizer;
        }
        public IActionResult Index()
        {
            return View(new ReportReasonVM());
        }
        public IActionResult Index2()
        {
            return View(new ReportReasonVM());
        }
        [HttpPost]
        public async Task<IActionResult> Create(ReportReasonVM model)
        {
            var Result = new CommonResponse<ReportReasonVM>();
            if (!ModelState.IsValid)
            {
                Result = CommonResponse<ReportReasonVM>.GetResult(localizer["invalidData"], ModelState.GetModelStateErrors());

            }
            else
                Result = await ReportReasonService.Create(model);

            return Ok(JObject.FromObject(Result, new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));
        }

        [HttpPost]

        public async Task<IActionResult> Edit(ReportReasonVM model)
        {
            var Result = new CommonResponse<ReportReasonVM>();
            if (!ModelState.IsValid)
            {
                Result = CommonResponse<ReportReasonVM>.GetResult(localizer["invalidData"], ModelState.GetModelStateErrors());
            }
            else
            {
                Result = await ReportReasonService.Edit(model);
            }
            return Ok(JObject.FromObject(Result, new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));
        }
        [HttpPost]
        public async Task<IActionResult> RemoveObj(Guid ID)
        {
            var Result = await ReportReasonService.Remove(ID);
            return Ok(JObject.FromObject(Result, new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));

        }
        [HttpGet]
        public async Task<IActionResult> GetObj(Guid ID)
        {
            var Result = await ReportReasonService.GetData(ID);
            return Ok(JObject.FromObject(Result, new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));


        }
        [HttpPost]
        public async Task<IActionResult> changeStatus(Guid ID, bool IsActive)
        {
            var Result = await ReportReasonService.ToggleActiveConfigration(ID, IsActive);
            return Ok(JObject.FromObject(Result, new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));

        }

        public IActionResult GetAll()
        {
            var Result = new { data = ReportReasonService.GetData() };
            return Ok(JObject.FromObject(Result, new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));



        }
    }
}
