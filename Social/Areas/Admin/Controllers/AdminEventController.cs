using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Social.Entity.DBContext;
using Social.Entity.Models;
using Social.Entity.ModelView;
using Social.FireBase;
using Social.Services;
using Social.Services.Attributes;
using Social.Services.FireBase_Helper;
using Social.Services.Helpers;
using Social.Services.ModelView;
using Social.Services.Services;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
namespace Social.Areas.Admin.Controllers
{
    [Area("Admin")]
    [ServiceFilter(typeof(AuthorizeUser))]
    public class AdminEventController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly IUserService _userService;
        private readonly IStringLocalizer<SharedResource> localizer;
        private readonly IEventServ _Event;
        private readonly IEventTypeListService eventTypeListService;
        private readonly IWebHostEnvironment _environment;
        private readonly IFirebaseManager firebaseManager;
        private readonly IGlobalMethodsService globalMethodsService;
        private readonly IConfiguration _configuration;
        private readonly AuthDBContext _authContext;
        public AdminEventController(IFirebaseManager firebaseManager, IConfiguration configuration,
            IWebHostEnvironment environment, IGlobalMethodsService globalMethodsService, UserManager<User> userManager, RoleManager<ApplicationRole> roleManager,
           IStringLocalizer<SharedResource> localizer,
           IEventServ Event,
           IEventTypeListService eventTypeListService,
           IUserService userService, AuthDBContext authContext)
        {
            this.firebaseManager = firebaseManager;
            _configuration = configuration;
            this.userManager = userManager;
            this._Event = Event;
            this.eventTypeListService = eventTypeListService;
            this._environment = environment;
            this.globalMethodsService = globalMethodsService;
            this._userService = userService;
            this.localizer = localizer;
            _authContext = authContext;
        }


        public ActionResult Index()
        {
            //var adminusers = (await userManager.GetUsersInRoleAsync(StaticApplicationRoles.SuperAdmin.ToString())).ToList();
            ViewBag.allCategories = _Event.getcategory().Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.name });
            ViewBag.EventTypeList = eventTypeListService.GetData().Select(x => new SelectListItem { Value = x.ID.ToString(), Text = x.Name });
            return View(new EventDataadminMV());
        }

        public ActionResult EventDetails(string EventID)
        {
            var EventData = _Event.GetEventbyid(EventID);
            return View(EventData);
        }
        [HttpPost]
        public async Task<IActionResult> Create(EventDataadminMV model)
        {
            var Result = new CommonResponse<EventDataadminMV>();

            if (!ModelState.IsValid)
            {
                Result = CommonResponse<EventDataadminMV>.GetResult(localizer["invalidData"], ModelState.GetModelStateErrors());
            }
            else
            {
                string imageName = null;
                if (model.ImageFile != null)
                {
                    var UniqName = await globalMethodsService.uploadFileAsync("/Images/EventData/", model.ImageFile);

                    imageName = "/Images/EventData/" + UniqName;
                    model.Image = imageName;
                }

                model.userid = _userService.getallUserDetails().FirstOrDefault(b => b.User.Email != "Owner@Owner.com").PrimaryId;

                Result = await _Event.Create(model);
            }

            return Ok(JObject.FromObject(Result, new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));
        }
        [HttpPost]

        public async Task<IActionResult> Edit(EventDataadminMV model)
        {
            var Result = new CommonResponse<EventDataadminMV>();
            if (!ModelState.IsValid)
            {
                Result = CommonResponse<EventDataadminMV>.GetResult(localizer["invalidData"], ModelState.GetModelStateErrors());
            }
            else
            {
                string imageName = null;
                if (model.ImageFile != null)
                {
                    var UniqName = await globalMethodsService.uploadFileAsync("/Images/EventData/", model.ImageFile);

                    imageName = "/Images/EventData/" + UniqName;
                    model.Image = imageName;
                }
                var adminid = _userService.getallUserDetails().FirstOrDefault(b => b.User.Email != "Owner@Owner.com").PrimaryId;
                if (adminid != model.userid)
                {
                    Result = CommonResponse<EventDataadminMV>.GetResult(localizer["You can't edit this event"], ModelState.GetModelStateErrors());
                    return Ok(JObject.FromObject(Result, new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));
                }
                Result = await _Event.Edit(model);
                var allateend = _Event.allattendevent();
                var Eventattend = _Event.getallChatattendevent(model.EntityId, allateend).ToList();

                foreach (var even in Eventattend)
                {
                    //if (even.UserattendId != Result.userid)
                    {
                        try
                        {
                            var userto = this._userService.GetUserDetails(even.Userattend.UserId);

                            FireBaseData fireBaseInfo = new FireBaseData()
                            {
                                Title = "update Event Data",
                                Body = model.Title,
                                imageUrl = _configuration["BaseUrl"] + model.Image,
                                muit = false,
                                Action_code = model.EntityId,
                                Action = "event_Updated"
                            };

                            SendNotificationcs sendNotificationcs = new SendNotificationcs();
                            if (userto.FcmToken != null)
                            {

                                await firebaseManager.SendNotification(userto.FcmToken, fireBaseInfo);
                            }
                            //await firebaseManager.SendNotification(Deatils?.FcmToken, fireBaseInfo);


                            //await sendNotificationcs.SendMessageAsync(userto.FcmToken, "event_Updated", fireBaseInfo, _environment.WebRootPath);


                        }
                        catch
                        {
                            continue;
                        }
                    }
                }

            }
            return Ok(JObject.FromObject(Result, new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));
        }
        [HttpPost]
        public async Task<IActionResult> RemoveObj(string ID)
        {
            await _Event.deleteEvent(ID);
            //var Result = await _Event.Remove(ID);

            return Ok(JObject.FromObject(CommonResponse<EventDataadminMV>.GetResult(200, true, localizer["RemovedSuccessfully"]), new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));

        }


        [HttpGet]
        public IActionResult GetObj(string ID)
        {
            var Result = _Event.GetData(ID);
            return Ok(JObject.FromObject(Result, new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));


        }
        [HttpPost]
        public async Task<IActionResult> StopEvent(string ID, DateTime StopFrom, DateTime StopTo)
        {
            var Event = _Event.GetEventbyid(ID);
            Event.StopFrom = StopFrom;
            Event.StopTo = StopTo;

            await _Event.updateEvent(Event);
            var Result = CommonResponse<object>.GetResult(200, true, localizer["UpdatedSuccessfully"]);
            return Ok(JObject.FromObject(Result, new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));
        }
        [HttpPost]
        public async Task<IActionResult> changeStatus(string ID, bool IsActive)
        {
            var Result = await _Event.ToggleActiveConfigration(ID, IsActive);
            return Ok(JObject.FromObject(Result, new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));

        }
        [HttpPost]
        public async Task<IActionResult> changeEventChatAttendStatus(string ID, int Status)
        {
            var eventChatAttend = _Event.GetEventChatAttendbyid(ID);
            if (eventChatAttend.leavechat == true)
            {
                return Ok(JObject.FromObject(CommonResponse<object>.GetResult(405, false, localizer["UserLeavedEventCanNotChangeStatus"]), new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));
            }
            if (Status == 1 || Status == 2)
            {
                eventChatAttend.deletedate = DateTime.Now.Date;
                eventChatAttend.delettime = DateTime.Now.TimeOfDay;
                eventChatAttend.removefromevent = true;
            }
            else
            {
                eventChatAttend.delete = false;
                eventChatAttend.removefromevent = false;
                eventChatAttend.leave = false;
                eventChatAttend.leavechat = false;
                eventChatAttend.stutus = 0;
                eventChatAttend.JoinDate = DateTime.Now.Date;
                eventChatAttend.Jointime = DateTime.Now.TimeOfDay;
                eventChatAttend.removefromevent = false;
            }
            eventChatAttend.stutus = Status;

            await _Event.editeEventChatAttend(eventChatAttend);

            return Ok(JObject.FromObject(CommonResponse<object>.GetResult(200, true, localizer["StatusChangedSuccessfully"]), new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));

        }
       
        [HttpPost]
        public IActionResult GetAll(int? Search_EventTypeListID, int? Search_EventCategoryID)
        {
            var sharedLink = _configuration.GetValue<string>("UrlToShare");  
            var paginationParamater = new PaginationProcess().GetPaginationParamaters(Request);
            var paginationFilter = new PaginationFilter() { PageNumber= paginationParamater.PageNumber, PageSize= paginationParamater.PageSize,
                SortColumn= paginationParamater.SortColumn, SortColumnDirection= paginationParamater.SortColumnDirection, SearchValue= paginationParamater
            .SearchValue};
            var data = _Event.GetData(paginationFilter,Search_EventTypeListID, Search_EventCategoryID, out int filterRecord);
            // Put SharedLink
            var dataList= data.ToList();
            dataList.ToList().ForEach(e => e.SharedLink = String.Format(sharedLink,e.EventTypeListName, e.EntityId));
            data=dataList.AsQueryable();
            //get total count of data in table
            int totalRecord = _authContext.EventData.Count();             
            
            var returnObj = new
            {
                draw = paginationParamater.Draw,
                recordsTotal = totalRecord,
                recordsFiltered = filterRecord,
                data = data
            };
            return Ok(JObject.FromObject(returnObj, new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));
        }
    
    public IActionResult GetEventsInCity(int CityID)
        {
            var Result = new { data = _Event.GetData(x => x.CityID == CityID).OrderByDescending(x => x.EventAttendCount).Take(10).ToList() };
            return Ok(JObject.FromObject(Result, new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));

        }
        public IActionResult GetEventsInCategory(int CategoryID)
        {
            var Result = new { data = _Event.GetData(x => x.categorieId == CategoryID).OrderByDescending(x => x.EventAttendCount).Take(10).ToList() };
            return Ok(JObject.FromObject(Result, new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));

        }
        [HttpGet]

        public IActionResult eventChatAttends(string ID)
        {

            var EventData = _Event.GetEventbyid(ID);
            var data = EventData.EventChatAttend.Where(x => x.Userattend != null).ToList().Select(x => new
            {
                UserName = x.Userattend.User.DisplayedUserName,
                Status = x.stutus,
                ID = x.EntityId,
                IsAdmin = x.EventData.UserId == x.Userattend.PrimaryId,
                EvemtDataID = x.EventData.EntityId,
            }).ToList();
            var Result = new { data = data };
            return Ok(JObject.FromObject(Result, new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));
        }

        [HttpPost]
        public async Task<IActionResult> ImportExternalEvents()
        {
            try
            {
                var baseUrl = string.Format("{0}://{1}", Request.Scheme, Request.Host.Value);
                var baseAddress = new Uri(baseUrl);
                var httpClient = new HttpClient { BaseAddress = baseAddress };

                var response = await httpClient.PostAsync("/api/Public/ExportExternalEvents", null);

                var responseHeaders = response.Headers.ToString();
                var result = await response.Content.ReadAsStringAsync();
                var status = (int)response.StatusCode;
                var headers = responseHeaders;
                var responseData = JsonConvert.DeserializeObject<InsertedEventResultViewModel>(result);
                return Ok(JObject.FromObject(new {response,responseData,}, new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));

            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
