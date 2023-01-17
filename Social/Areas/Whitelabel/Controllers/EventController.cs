using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Primitives;
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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Social.Areas.WhiteLable.Controllers
{
    [Area("Whitelabel")]
    [ServiceFilter(typeof(AuthorizeWhiteLable))]
    public class EventController : Controller
    {
        private readonly IEventServ _Event;
        private readonly IUserService _userService;
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _environment;
        private readonly IFirebaseManager _firebaseManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly IEventTypeListService _eventTypeListService;
        private readonly IGlobalMethodsService _globalMethodsService;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;
        private readonly IEventReportService _eventReportService;
        private readonly IMessageServes _MessageServes;
        private readonly AuthDBContext _authDBContext;
        public EventController(IFirebaseManager firebaseManager, Microsoft.Extensions.Configuration.IConfiguration configuration,
            IWebHostEnvironment environment, IGlobalMethodsService globalMethodsService, UserManager<User> userManager,
            RoleManager<ApplicationRole> roleManager, IStringLocalizer<SharedResource> localizer, IEventServ Event,
            IEventTypeListService eventTypeListService, IUserService userService, IHttpContextAccessor httpContextAccessor,
            IEventReportService eventReportService, IMessageServes MessageServes, AuthDBContext authDBContext)
        {
            _Event = Event;
            _localizer = localizer;
            _userService = userService;
            _environment = environment;
            _userManager = userManager;
            _configuration = configuration;
            _firebaseManager = firebaseManager;
            _httpContextAccessor = httpContextAccessor;
            _globalMethodsService = globalMethodsService;
            _eventTypeListService = eventTypeListService;
            _eventReportService = eventReportService;
            _MessageServes = MessageServes;
            _authDBContext= authDBContext;  
        }


        public ActionResult Index()
        {
            //var adminusers = (await userManager.GetUsersInRoleAsync(StaticApplicationRoles.SuperAdmin.ToString())).ToList();
            ViewBag.allCategories = _Event.getcategory().Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.name });            
            var eventTypeList = _eventTypeListService.GetData().Where(e=>e.Name.Contains("WhiteLable")).ToList();
             eventTypeList = GetEventNames(eventTypeList);
            ViewBag.EventTypeList = eventTypeList.Select(x => new SelectListItem { Value = x.ID.ToString(), Text = x.Name });
            return View(new EventDataadminMV());
        }

        

        public ActionResult EventDetails(string EventID)
        {
            var EventData = _Event.GetEventbyid(EventID);

            var allEventAttends = _authDBContext.EventChatAttend.Where(e => e.EventDataid == EventData.Id);
            var attendedUsers = allEventAttends.Where(c => c.stutus != 2).Select(e => e.Userattend);
            var actualUsers = attendedUsers.Where(x => x.Email.ToLower().Contains("@owner") == false && !x.IsWhiteLabel.Value);
            ViewBag.eventUsers = new UserStatistics()
            {
                Updated = actualUsers.Count(),               
                Male_Count = actualUsers.Where(x => x.Gender == "male").Count(),
                Female_Count = actualUsers.Where(x => x.Gender == "female").Count(),
            };
            return View(EventData);
        }
        public IActionResult UserDetails(string UserID)
        {
            var user = _authDBContext.Users.Find(UserID);
            if (user == null)
                return NotFound();
            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult> Create(EventDataadminMV model)
        {
            _httpContextAccessor.HttpContext.Request.Headers.TryGetValue("Authorization", out StringValues authorizationToken);

            var Result = new CommonResponse<EventDataadminMV>();

            if (!ModelState.IsValid)
            {
                Result = CommonResponse<EventDataadminMV>.GetResult(_localizer["invalidData"], ModelState.GetModelStateErrors());
            }
            else
            {
               bool isCorrect= this.CheckListOfDate(new { eventdateList= model.eventdateList, eventdatetoList=model.eventdatetoList },model.eventfrom,model.eventto,model.allday);

                if(!isCorrect)
                {
                    Result = CommonResponse<EventDataadminMV>.GetResult("Some Date in the past", ModelState.GetModelStateErrors());
                }
                else
                {
                    UserDetails userData = null;
                    string imageName = null;
                    if (model.ImageFile != null)
                    {
                        var UniqName = await _globalMethodsService.uploadFileAsync("/Images/EventData/", model.ImageFile);

                        imageName = "/Images/EventData/" + UniqName;
                        model.Image = imageName;
                    }

                    if (authorizationToken.Count == 0)
                    {
                        authorizationToken = _httpContextAccessor.HttpContext.Request.Cookies["Authorization"];
                        model.userid = (await _userService.GetLoggedInUser(authorizationToken)).User.UserDetails.PrimaryId;
                        userData = (await _userService.GetLoggedInUser(authorizationToken)).User.UserDetails;
                    }

                    var result = await _Event.Create(model);
                    foreach (var item in result.Data)
                    {
                        await _MessageServes.addeventmessage(new EventMessageDTO
                        {
                            EventChatAttendid = item,
                            eventjoin = false,
                            Message = "",
                            Messagetype = 1,
                            EventId = model.EntityId,
                            Messagesdate = DateTime.Now.Date,
                            Messagestime = DateTime.Now.TimeOfDay
                        }, userData);
                    }

                    Result= CommonResponse<EventDataadminMV>.GetResult(200, true, _localizer["SavedSuccessfully"]);
                }
               
                
            }

            return Ok(JObject.FromObject(Result, new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));
        }

        private bool CheckListOfDate(dynamic dateList, TimeSpan? from, TimeSpan? to, bool? allday)
        {  
            var eventtolist= (List<DateTime>)dateList.eventdatetoList;
            if(((List<DateTime>)dateList.eventdateList).Count ==0 || ((List<DateTime>)dateList.eventdateList).Count != eventtolist.Count)
            {
                return false;
            }
           foreach (var (value,i) in ((List<DateTime>)dateList.eventdateList).Select((value, i)=>(value,i)))
           {
                if(value != null && eventtolist[i] !=null)
                {
                    var isCorrect = value.Date.Subtract(DateTime.Now.Date).TotalDays >0 && eventtolist[i].Date.Subtract(DateTime.Now.Date).TotalDays > 0
                                     && eventtolist[i].Date.Subtract(value.Date).TotalDays >=0;
                  
                    var isInSameDay = (value.Date.Subtract(DateTime.Now.Date).TotalDays == 0 && eventtolist[i].Date.Subtract(DateTime.Now.Date).TotalDays > 0)
                        || ((value.Date.Subtract(DateTime.Now.Date).TotalDays == 0 && eventtolist[i].Date.Subtract(DateTime.Now.Date).TotalDays >= 0 && allday == false &&
                        to?.Subtract(from.Value).TotalHours > 1));
                    if (isCorrect || isInSameDay)
                    {
                        
                        continue;
                    }
                    else
                    {
                        return false ;
                    }
                }
                return false;
           }            
           return true;
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EventDataadminMV model)
        {
            _httpContextAccessor.HttpContext.Request.Headers.TryGetValue("Authorization", out StringValues authorizationToken);

            var Result = new CommonResponse<EventDataadminMV>();
            if (!ModelState.IsValid)
            {
                Result = CommonResponse<EventDataadminMV>.GetResult(_localizer["invalidData"], ModelState.GetModelStateErrors());
            }
            
          else
            {
                model.eventdatetoList = new List<DateTime>() { model.eventdatetoList.FirstOrDefault() };
                model.eventdateList = new List<DateTime>() { model.eventdateList.FirstOrDefault() };
                bool isCorrect = this.CheckListOfDate(new { eventdateList = model.eventdateList, eventdatetoList = model.eventdatetoList }, model.eventfrom, model.eventto, model.allday);

                if (!isCorrect)
                {
                    Result = CommonResponse<EventDataadminMV>.GetResult("Some Date in the past", ModelState.GetModelStateErrors());
                }
                else
                {
                    
                    string imageName = null;
                    if (model.ImageFile != null)
                    {
                        var UniqName = await _globalMethodsService.uploadFileAsync("/Images/EventData/", model.ImageFile);

                        imageName = "/Images/EventData/" + UniqName;
                        model.Image = imageName;
                    }

                    int loggedinUser = 0;

                    if (authorizationToken.Count == 0)
                    {
                        authorizationToken = _httpContextAccessor.HttpContext.Request.Cookies["Authorization"];
                        loggedinUser = _userService.GetLoggedInUser(authorizationToken).Result.User.UserDetails.PrimaryId;
                    }

                    if (loggedinUser != model.userid)
                    {
                        Result = CommonResponse<EventDataadminMV>.GetResult(_localizer["You can't edit this event"], ModelState.GetModelStateErrors());
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

                                    await _firebaseManager.SendNotification(userto.FcmToken, fireBaseInfo);
                                }

                            }
                            catch
                            {
                                continue;
                            }
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

            return Ok(JObject.FromObject(CommonResponse<EventDataadminMV>.GetResult(200, true, _localizer["RemovedSuccessfully"]), new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));

        }

        [HttpGet]
        public IActionResult GetObj(string ID)
        {
            var Result = _Event.GetData(ID);
            if(Result!= null)
            {
                Result= FixEventName(Result);
                Result.eventdateList = new List<DateTime> { Result.eventdate };
                Result.eventdatetoList = new List<DateTime> { Result.eventdateto };
            }
            var data = JObject.FromObject(Result, new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() });
            // Fix serialization issue
            var listDateto=data.SelectToken("eventdatetoList");
            var dateto = data.SelectToken("eventdateto");
            listDateto.Replace(dateto);
            var listDate = data.SelectToken("eventdateList");
            var date = data.SelectToken("eventdate");
            listDate.Replace(date);

           // return Ok(JObject.FromObject(Result, new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));
           return Ok(data);

        }

        [HttpPost]
        public async Task<IActionResult> StopEvent(string ID, DateTime StopFrom, DateTime StopTo)
        {
            var Event = _Event.GetEventbyid(ID);
            Event.StopFrom = StopFrom;
            Event.StopTo = StopTo;

            await _Event.updateEvent(Event);
            var Result = CommonResponse<object>.GetResult(200, true, _localizer["UpdatedSuccessfully"]);
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
                return Ok(JObject.FromObject(CommonResponse<object>.GetResult(405, false, _localizer["UserLeavedEventCanNotChangeStatus"]), new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));
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

            return Ok(JObject.FromObject(CommonResponse<object>.GetResult(200, true, _localizer["StatusChangedSuccessfully"]), new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));

        }

        public IActionResult GetAll(int? Search_EventTypeListID, int? Search_EventCategoryID)
        {
            _httpContextAccessor.HttpContext.Request.Headers.TryGetValue("Authorization", out StringValues authorizationToken);

            LoggedinUser loggedinUser = null;

            if (authorizationToken.Count == 0)
            {
                authorizationToken = _httpContextAccessor.HttpContext.Request.Cookies["Authorization"];
                loggedinUser = _userService.GetLoggedInUser(authorizationToken).Result;
            }
            var paginationParamaters = new PaginationProcess().GetPaginationParamaters(Request);
            var paginationFilter = new PaginationFilter() { PageNumber = paginationParamaters.PageNumber, PageSize = paginationParamaters.PageSize,
                SortColumn = paginationParamaters.SortColumn, SortColumnDirection = paginationParamaters.SortColumnDirection};
            var totalRecord = _authDBContext.EventData.Where(x => x.UserId == loggedinUser.User.UserDetails.PrimaryId).Count();
            
            var whiteLableEvent = _Event.GetWhiteLableEvents(paginationFilter, loggedinUser.User.UserDetails.PrimaryId, Search_EventTypeListID, Search_EventCategoryID, out int filteredCount);
              

            whiteLableEvent = this.SetPublicEventName(whiteLableEvent).AsQueryable();
           
            var returnObj = new
            {
                draw = paginationParamaters.Draw,
                recordsTotal = totalRecord,
                recordsFiltered = filteredCount,
                data = whiteLableEvent
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

        public IActionResult GetEventReports(string ID)
        {
            var Result = new { data = _eventReportService.GetData(ID).OrderByDescending(x => x.RegistrationDate) };
            return Ok(JObject.FromObject(Result, new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));
        }

        private IEnumerable<EventDataadminMV> SetPublicEventName(IQueryable<EventDataadminMV> eventDataAdminMVs)
        {
            //var result = new List<EventDataadminMV>();
            foreach (var eventType in eventDataAdminMVs)
            {
                if (eventType.EventTypeListName.Contains("Public"))
                {
                    eventType.EventTypeListName = "Public";
                    yield return eventType;
                }
                else
                {
                    eventType.EventTypeListName = "Private";
                    yield return eventType;
                }
                //result.Add(eventType);
            }
            //return result.AsQueryable();
        }
        private List<EventTypeListVM> GetEventNames(List<EventTypeListVM> eventTypeList)
        {
            var eventTypes = new List<EventTypeListVM>();
            foreach (var eventType in eventTypeList)
            {
                if (eventType.Name.Contains("Public"))
                {
                    eventType.Name = "Public";
                }
                else
                {
                    eventType.Name = "Private";
                }
                eventTypes.Add(eventType);
            }
            return eventTypes;
        }

        private EventDataadminMV FixEventName(EventDataadminMV eventDataAdminMV)
        {
            if(eventDataAdminMV.EventTypeListName.Contains("WhiteLable"))
            {
                if(eventDataAdminMV.EventTypeListName.Contains("Public"))
                {
                    eventDataAdminMV.EventTypeListName = "Public";
                }
                else
                {
                    eventDataAdminMV.EventTypeListName = "Private";
                }

                return eventDataAdminMV;
            }
            return eventDataAdminMV;
        }

    }
}
