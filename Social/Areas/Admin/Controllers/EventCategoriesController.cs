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

    public class EventCategoriesController : Controller
    {
        private readonly IEventCategoryService EventCategoryService;
        private readonly IStringLocalizer<SharedResource> localizer;

        public EventCategoriesController(IEventCategoryService EventCategoryService, IStringLocalizer<SharedResource> _localizer)
        {
            this.EventCategoryService = EventCategoryService;
            localizer = _localizer;
        }
        public IActionResult Index()
        {
            return View(new EventCategoryVM());
        }

        [HttpPost]
        public async Task<IActionResult> Create(EventCategoryVM model)
        {
            var Result = new CommonResponse<EventCategoryVM>();
            if (!ModelState.IsValid)
            {
                Result = CommonResponse<EventCategoryVM>.GetResult(localizer["invalidData"], ModelState.GetModelStateErrors());

            }
            else
                Result = await EventCategoryService.Create(model);

            return Ok(JObject.FromObject(Result, new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));
        }

        [HttpPost]

        public async Task<IActionResult> Edit(EventCategoryVM model)
        {
            var Result = new CommonResponse<EventCategoryVM>();
            if (!ModelState.IsValid)
            {
                Result = CommonResponse<EventCategoryVM>.GetResult(localizer["invalidData"], ModelState.GetModelStateErrors());
            }
            else
            {
                Result = await EventCategoryService.Edit(model);
            }
            return Ok(JObject.FromObject(Result, new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));
        }
        [HttpPost]
        public async Task<IActionResult> RemoveObj(string ID)
        {
            var Result = await EventCategoryService.Remove(ID);
            return Ok(JObject.FromObject(Result, new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));

        }
        [HttpGet]
        public IActionResult GetObj(string ID)
        {
            var Result = EventCategoryService.GetData(ID);
            return Ok(JObject.FromObject(new { ID = Result.Id, Result.name, Result.EntityId }, new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));


        }
        [HttpPost]
        public async Task<IActionResult> changeStatus(string ID, bool IsActive)
        {
            var Result = await EventCategoryService.ToggleActiveConfigration(ID, IsActive);
            return Ok(JObject.FromObject(Result, new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));
        }

        public IActionResult GetAll()
        {
            var Result = new { data = EventCategoryService.GetData() };
            return Ok(JObject.FromObject(Result, new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));



        }
    }
}
