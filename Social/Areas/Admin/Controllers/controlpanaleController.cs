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

    public class controlpanaleController : Controller
    {
        private readonly IAppConfigrationService appConfigrationService;
        private readonly IStringLocalizer<SharedResource> localizer;

        public controlpanaleController(IAppConfigrationService appConfigrationService, IStringLocalizer<SharedResource> _localizer)
        {
            this.appConfigrationService = appConfigrationService;
            localizer = _localizer;
        }
        public IActionResult Index()
        {
            return View(new AppConfigrationVM_Required());
        }
        [HttpGet]
        public async Task<IActionResult> Edit(Guid ID)
        {
            var Result = await appConfigrationService.GetData(ID);
           
            return View(Result);
        }
        public IActionResult Index2()
        {
            return View(new AppConfigrationVM());
        }
        [HttpPost]
        public async Task<IActionResult> Create(AppConfigrationVM_Required model)
        {
            var Result = new CommonResponse<AppConfigrationVM>();
            if (!ModelState.IsValid)
            {
                Result = CommonResponse<AppConfigrationVM>.GetResult(localizer["invalidData"], ModelState.GetModelStateErrors());

            }
            else
            { Result = await appConfigrationService.Create(model); }
            return Ok(JObject.FromObject(Result, new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));
        }
      
        [HttpPost]
        public async Task<IActionResult> _Edit(AppConfigrationVM model)//Edit Withput Check Validation required
        {
            var Result = new CommonResponse<AppConfigrationVM>();
            if (!ModelState.IsValid)
            {
                Result = CommonResponse<AppConfigrationVM>.GetResult(localizer["invalidData"], ModelState.GetModelStateErrors());
            }
            else
            {
                Result = await appConfigrationService.Edit(model);
            }
                return Ok(JObject.FromObject(Result, new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));
        }  
        public async Task<IActionResult> Edit(AppConfigrationVM_Required model)
        {
            var Result = new CommonResponse<AppConfigrationVM>();
            if (!ModelState.IsValid)
            {
                Result = CommonResponse<AppConfigrationVM>.GetResult(localizer["invalidData"], ModelState.GetModelStateErrors());
            }
            else
            {
                Result = await appConfigrationService.Edit(model);
            }
                return Ok(JObject.FromObject(Result, new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));
        }
        [HttpPost]
        public async Task<IActionResult> RemoveObj(Guid ID)
        {
            var Result = await appConfigrationService.Remove(ID);
            return Ok(JObject.FromObject(Result, new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));

        }
        [HttpGet]
        public async Task<IActionResult> GetObj(Guid ID)
        {
            var Result = await appConfigrationService.GetData(ID);
            return Ok(JObject.FromObject(Result, new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));


        }   
        [HttpPost]
        public  async Task<IActionResult> changeStatus(Guid ID,bool IsActive)
        {
                var Result =await appConfigrationService.ToggleActiveConfigration(ID,IsActive);
            return Ok(JObject.FromObject(Result, new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));
            
        }

        public IActionResult GetAll()
        {
            var Result = new { data =  appConfigrationService.GetData() };
            return Ok(JObject.FromObject(Result, new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));
        }
    }
}
