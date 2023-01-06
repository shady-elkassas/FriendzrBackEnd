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
    public class EventTypeListsController : Controller
    {
        private readonly IEventTypeListService EventTypeListService;
        private readonly IStringLocalizer<SharedResource> localizer;

        public EventTypeListsController(IEventTypeListService EventTypeListService, IStringLocalizer<SharedResource> _localizer)
        {
            this.EventTypeListService = EventTypeListService;
            localizer = _localizer;
        }
        public IActionResult Index()
        {
            return View(new EventTypeListVM());
        }
      
        [HttpPost]
        public async Task<IActionResult> Create(EventTypeListVM model)
        {
            var Result = new CommonResponse<EventTypeListVM>();
            if (!ModelState.IsValid)
            {
                Result = CommonResponse<EventTypeListVM>.GetResult(localizer["invalidData"], ModelState.GetModelStateErrors());

            }
            else
                Result = await EventTypeListService.Create(model);

            return Ok(JObject.FromObject(Result, new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));
        }

        [HttpPost]

        public async Task<IActionResult> Edit(EventTypeListVM model)
        {
            var Result = new CommonResponse<EventTypeListVM>();
            if (!ModelState.IsValid)
            {
                Result = CommonResponse<EventTypeListVM>.GetResult(localizer["invalidData"], ModelState.GetModelStateErrors());
            }
            else
            {
                Result = await EventTypeListService.Edit(model);
            }
            return Ok(JObject.FromObject(Result, new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));
        }
        [HttpPost]
        public async Task<IActionResult> RemoveObj(Guid ID)
        {
            var Result = await EventTypeListService.Remove(ID);
            return Ok(JObject.FromObject(Result, new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));

        }
        [HttpGet]
        public async Task<IActionResult> GetObj(Guid ID)
        {
            var Result = await EventTypeListService.GetData(ID);
            return Ok(JObject.FromObject(Result, new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));


        }
        [HttpPost]
        public async Task<IActionResult> changeStatus(Guid ID, bool IsActive)
        {
            var Result = await EventTypeListService.ToggleActiveConfigration(ID, IsActive);
            return Ok(JObject.FromObject(Result, new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));

        }

        public IActionResult GetAll()
        {
            var Result = new { data = EventTypeListService.GetData() };
            return Ok(JObject.FromObject(Result, new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));



        }
    }
}
