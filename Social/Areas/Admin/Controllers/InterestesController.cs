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

    public class InterestesController : Controller
    {
        private readonly IInterestsService interestsService;
        private readonly IStringLocalizer<SharedResource> localizer;

        public InterestesController(IInterestsService interestsService, IStringLocalizer<SharedResource> _localizer)
        {
            this.interestsService = interestsService;
            localizer = _localizer;
        }
        public IActionResult Index()
        {
            return View(new InterestsVM());
        }
     
        [HttpPost]
        public async Task<IActionResult> Create(InterestsVM model)
        {
            var Result = new CommonResponse<InterestsVM>();
            if (!ModelState.IsValid)
            {
                Result = CommonResponse<InterestsVM>.GetResult(localizer["invalidData"], ModelState.GetModelStateErrors());

            }
            else
                Result = await interestsService.Create(model);

            return Ok(JObject.FromObject(Result, new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));
        }

        [HttpPost]

        public async Task<IActionResult> Edit(InterestsVM model)
        {
            var Result = new CommonResponse<InterestsVM>();
            if (!ModelState.IsValid)
            {
                Result = CommonResponse<InterestsVM>.GetResult(localizer["invalidData"], ModelState.GetModelStateErrors());
            }
            else
            {
                Result = await interestsService.Edit(model);
            }
            return Ok(JObject.FromObject(Result, new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));
        }
        [HttpPost]
        public async Task<IActionResult> RemoveObj(string ID)
        {
            var Result = await interestsService.Remove(ID);
            return Ok(JObject.FromObject(Result, new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));

        }
        [HttpGet]
        public IActionResult GetObj(string ID)
        {
            var Result =  interestsService.GetData(ID);
            return Ok(JObject.FromObject(new{ ID = Result.Id, Result.name, Result.EntityId }, new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));


        }
        [HttpPost]
        public async Task<IActionResult> changeStatus(string ID, bool IsActive)
        {
            var Result = await interestsService.ToggleActiveConfigration(ID, IsActive);
            return Ok(JObject.FromObject(Result, new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));
        }

        public IActionResult GetAll()
        {
            var Result = new { data = interestsService.GetData().Where(x=>x.IsSharedForAllUsers==true) };
            return Ok(JObject.FromObject(Result, new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));



        }
    }
}
