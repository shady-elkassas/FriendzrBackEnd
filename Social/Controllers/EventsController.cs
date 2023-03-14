
using CRM.Services.Wrappers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Social.Entity.DBContext;
using Social.Entity.Enums;
using Social.Entity.Models;
using Social.Entity.ModelView;
using Social.FireBase;
using Social.Sercices;
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

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Social.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    [ServiceFilter(typeof(AuthorizeUser))]
    public class EventsController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        private readonly IUserService _userService;
        private readonly IErrorLogService _errorLogService;
        private readonly IEventServ _Event;
        private readonly IEventTypeListService _EventTypeListService;
        private readonly IFrindRequest _FrindRequest;
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly IMessageServes MessageServes;
        public AuthDBContext _authContext;
        private readonly IAppConfigrationService appConfigrationService;
        private readonly IFirebaseManager firebaseManager;
        private readonly IGlobalMethodsService globalMethodsService;
        public EventsController(IFrindRequest FrindRequest, IAppConfigrationService appConfigrationService, AuthDBContext authContext, IFirebaseManager firebaseManager, IGlobalMethodsService globalMethodsService, IConfiguration configuration, IWebHostEnvironment environment, UserManager<User> userManager, IEventTypeListService EventTypeListService, IStringLocalizer<SharedResource> localizer, IMessageServes MessageServes, IEventServ Event, IUserService userService, IErrorLogService errorLogService)
        {
            _FrindRequest = FrindRequest;
            _authContext = authContext;
            this.firebaseManager = firebaseManager;
            this.MessageServes = MessageServes;
            this.globalMethodsService = globalMethodsService;
            this.userManager = userManager;
            this._Event = Event;
            this.appConfigrationService = appConfigrationService;
            this._environment = environment;
            this._userService = userService;
            _EventTypeListService = EventTypeListService;
            this._errorLogService = errorLogService;
            this._configuration = configuration;
            this._localizer = localizer;
        }

        [HttpPost]
        [Route("AddEventData")]
        public async Task<IActionResult> ADD([FromForm] string ListOfUserIDs, [FromForm] TimeSpan? Creattime, [FromForm] DateTime? CreatDate, [FromForm] IFormFile Eventimage, [FromForm] string categoryid, [FromForm] EventData model)
        {
            try
            {

                if (model.eventtype == null)
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                        new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                         _localizer["event type is Required"], null));
                }

                var typedate = await _EventTypeListService.GetData((Guid)model.eventtype);
                var dataconfig = appConfigrationService.GetData().FirstOrDefault();

                var loggedinUser = HttpContext.GetUser();
                var CHECKEVENTLOCATION = _userService.CHECKEVENTLOCATION(loggedinUser.User.UserDetails.lat, loggedinUser.User.UserDetails.lang, model.lat, model.lang, dataconfig);
                var olddata = _Event.getallevent();
                var datavalid = olddata.FirstOrDefault(m => m.lat == model.lat && m.lang == model.lang && m.Title == model.Title && m.eventdate == Convert.ToDateTime(model.eventdate));
                if (datavalid != null)
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                       new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,
                        _localizer["This event already exists"], null));
                }
                if (CHECKEVENTLOCATION == false)
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                        new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,
                         _localizer["Please select a nearby Location "], null));
                }
                if (string.IsNullOrEmpty(ListOfUserIDs) != false && typedate.Privtekey == true)
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                       new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                        _localizer["Please select attendees for your event"], null));
                }

                if (CreatDate == null)
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                        new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,
                         _localizer["CreatDateisrequired"], null));
                }
                if (Creattime == null)
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                        new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                         _localizer["Creattime is required"], null));
                }
                if (model.lang == "" || model.lang == null || model.lat == "" || model.lat == null)
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                        new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                         _localizer["locations is required"], null));
                }

                if (model.eventdate == null || model.eventdateto == null)
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                        new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                         _localizer["start and end date is required"], null));
                }
                if ((model.eventfrom == null || model.eventto == null) && model.allday == false)
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                        new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                         _localizer["start and end time is required"], null));
                }

                if (model.eventdate < DateTime.Now.Date || model.eventdate > DateTime.Now.Date.AddYears(1))
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                        new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                         _localizer["EventdatemustNotinthepast"], null));
                }
                if (model.eventdateto > model.eventdate.Value.AddMonths(1))
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                        new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                         _localizer["Maximumallowedeventperiod"], null));
                }
                if ((model.totalnumbert + 1) < 3)
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                        new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                         _localizer["AttendeesNumbershouldnotbelessthan2"], null));
                }
                if (model.eventdate > model.eventdateto)
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                        new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                         _localizer["EventstartdatemustNotolderthanEventenddate"], null));
                }
                if (model.eventfrom >= model.eventto)
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                        new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                         _localizer["EventstarttimemustNotolderthanEventendtime"], null));
                }
                if (model.Title == null)
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                        new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                         _localizer["Title is Required"], null));
                }
                if (model.description == null)
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                        new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                         _localizer["description is Required"], null));
                }

                if (dataconfig.EventTitle_MinLength != null)
                {
                    if (model.Title.Length < dataconfig.EventTitle_MinLength )
                    {
                        return StatusCode(StatusCodes.Status406NotAcceptable,
                            new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                                      _localizer["Event  Title Min Length is "] + dataconfig.EventTitle_MinLength , null));

                    }
                }
                if (dataconfig.EventTitle_MaxLength != null)
                {
                    if ( model.Title.Length > dataconfig.EventTitle_MaxLength)
                    {
                        return StatusCode(StatusCodes.Status406NotAcceptable,
                            new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                                _localizer["Event Title Max Length is "] + dataconfig.EventTitle_MaxLength, null));

                    }
                }
                if (dataconfig.EventDetailsDescription_MaxLength != null)
                {
                    if ( model.description.Length > dataconfig.EventDetailsDescription_MaxLength)
                    {
                        return StatusCode(StatusCodes.Status406NotAcceptable,
                            new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                              _localizer["Event Details Description Max Length is "] + dataconfig.EventDetailsDescription_MaxLength, null));
                    }
                }
                if (dataconfig.EventDetailsDescription_MinLength != null)
                {
                    if (model.description.Length < dataconfig.EventDetailsDescription_MinLength)
                    {
                        return StatusCode(StatusCodes.Status406NotAcceptable,
                            new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                                _localizer["Event Details Description Min Length is "] + dataconfig.EventDetailsDescription_MinLength, null));
                    }
                }
                //if (model.description.Length > 150)
                //{
                //    return StatusCode(StatusCodes.Status406NotAcceptable,
                //        new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                //         _localizer["descriptionismustNotmorethan150characters."], null));
                //}
                if (model.totalnumbert == 0)
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                        new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                         _localizer["total number is Required"], null));
                }
                if (model.totalnumbert > 5000)
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                        new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                         _localizer["TotalNumbermustnottobemorethan5000"], null));
                }
                if (categoryid == "" || categoryid == null)
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                        new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                         _localizer["category is Required"], null));
                }
                if (model.allday == null)
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                        new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                         _localizer["All day is Required"], null));
                }


                var UserId = loggedinUser.User.UserDetails;
                var listdata = _FrindRequest.GetallFrendes(UserId.PrimaryId, null);

                if (typedate.Privtekey == true)
                {
                    var users = JsonConvert.DeserializeObject<List<string>>(ListOfUserIDs).ToList();
                    var userDeatils = this._userService.GetLISTUserDetails(users).ToList();
                    if ((model.totalnumbert + 1) < users.Count)
                    {


                        return StatusCode(StatusCodes.Status406NotAcceptable,
                               new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                                _localizer[" can not add more than "] + model.totalnumbert, null));


                    }
                    foreach (var user in users)
                    {
                        if (listdata.FirstOrDefault(v => v.status == 1 && (v.User.UserId == user || v.UserRequest.UserId == user)) == null)
                        {
                            return StatusCode(StatusCodes.Status406NotAcceptable,
                                   new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                                    _localizer[" can only add your friends to the event"], null));

                        }
                    }
                }
                string imageName = null;

                if (Eventimage != null)
                {
                    var UniqName = await globalMethodsService.uploadFileAsync("/Images/EventData/", Eventimage);

                    imageName = "/Images/EventData/" + UniqName;
                    model.image = imageName;
                }
                else
                {

                    return StatusCode(StatusCodes.Status406NotAcceptable,
                        new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                         _localizer["Event Image is Required"], null));
                }

                //var userDeatils = this._userService.GetUserDetails(loggedinUser.UserId);


                model.UserId = UserId.PrimaryId;
                model.categorieId = _Event.getcategory().Where(n => n.EntityId == categoryid).FirstOrDefault().Id;
                model.EntityId = Guid.NewGuid().ToString();
                model.EventTypeListid = typedate.ID;
                model.CreatedDate = DateTime.Now;
                var cuserount = await _Event.InsertEvent(model);
                if (cuserount != null)
                {
                    //eventattend eventattend = new eventattend();
                    //eventattend.EventDataid = model.Id;
                    //eventattend.UserattendId = UserId.PrimaryId;
                    //eventattend.JoinDate = CreatDate;

                    var JoinTimeForMessage = Creattime.HasValue ? ((TimeSpan)Creattime).Add(new TimeSpan(0, 0, 2)) : Creattime;
                    var a = await _Event.InsertEventChatAttend(new EventChatAttend { Jointime = Creattime.Value, EventDataid = model.Id, UserattendId = UserId.PrimaryId, JoinDate = CreatDate, ISAdmin = true });

                    await MessageServes.addeventmessage(new EventMessageDTO { EventChatAttendid = a.Id, eventjoin = false, Message = "", Messagetype = 1, EventId = model.EntityId, Messagesdate = CreatDate.Value, Messagestime = JoinTimeForMessage.Value }, loggedinUser.User.UserDetails);

                    if (string.IsNullOrEmpty(ListOfUserIDs) == false && typedate.Privtekey == true)
                    {
                        {
                            try
                            {
                                var list = JsonConvert.DeserializeObject<List<string>>(ListOfUserIDs).ToList();
                                var userDeatils = this._userService.GetLISTUserDetails(list).ToList();


                                foreach (var item in userDeatils)
                                {
                                    EventTracker eventTracker = new EventTracker()
                                    {
                                        EventId = model.Id,
                                        UserId = item.PrimaryId,
                                        Date = DateTime.Now,
                                        ActionType = EventActionType.Attend.ToString()
                                    };

                                    _authContext.EventTrackers.Add(eventTracker);

                                    model.Attendees += 1;
                                    //eventattend eventattendprivet = new eventattend();
                                    //eventattendprivet.EventDataid = model.Id;
                                    //eventattendprivet.UserattendId = item.PrimaryId;
                                    //eventattendprivet.JoinDate = CreatDate;

                                    //var JoinTimeForMessage = Creattime.HasValue ? ((TimeSpan)Creattime).Add(new TimeSpan(0, 0, 2)) : Creattime;
                                    var aprivit = await _Event.InsertEventChatAttend(new EventChatAttend { Jointime = Creattime.Value, EventDataid = model.Id, UserattendId = item.PrimaryId, JoinDate = CreatDate, ISAdmin = false });

                                    await MessageServes.addeventmessage(new EventMessageDTO { EventChatAttendid = aprivit.Id, eventjoin = false, Message = "", Messagetype = 1, EventId = model.EntityId, Messagesdate = CreatDate.Value, Messagestime = JoinTimeForMessage.Value }, item);
                                    try
                                    {


                                        FireBaseData fireBaseInfo = new FireBaseData() { Title = model.Title, Body = UserId.User.DisplayedUserName + " added you in a private Event   " + model.Title, imageUrl = _configuration["BaseUrl"] + model.image, Action_code = model.EntityId, muit = false, Action = "Check_private_events" };
                                        var addnoti = MessageServes.getFireBaseData(item.PrimaryId, fireBaseInfo, CreatDate, Creattime);
                                        await MessageServes.addFireBaseDatamodel(addnoti);
                                        SendNotificationcs sendNotificationcs = new SendNotificationcs();
                                        if (item.FcmToken != null)
                                            await firebaseManager.SendNotification(item.FcmToken, fireBaseInfo);
                                    }
                                    catch
                                    {

                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                var M = ex;
                                M = ex;
                            }
                        }
                    }
                    else
                    {
                        var allblock = this._authContext.Requestes;

                        var alluse = _userService.allusersaroundevent(Convert.ToDouble(model.lat), Convert.ToDouble(model.lang)).ToList();
                        foreach (var item in alluse)
                        {
                            try
                            {
                                int userid = UserId.PrimaryId;
                                if (item.PrimaryId != userid)
                                {

                                    var blockod = allblock.Where(m => ((m.UserId == userid && m.UserRequestId == item.PrimaryId) || (m.UserId == item.PrimaryId && m.UserRequestId == userid)) && m.status == 2).ToList();
                                    if (blockod != null)
                                    {
                                        FireBaseData fireBaseInfo = new FireBaseData() { Title = model.Title, Body = "Check this hot event near you!", imageUrl = ((model.EventTypeListid == 3 ? "" : _configuration["BaseUrl"]) + model.image), Action_code = model.EntityId, muit = false, Action = "Check_events_near_you" };
                                        var addnoti = MessageServes.getFireBaseData(item.PrimaryId, fireBaseInfo, CreatDate, Creattime);
                                        await MessageServes.addFireBaseDatamodel(addnoti);
                                        SendNotificationcs sendNotificationcs = new SendNotificationcs();
                                        if (item.FcmToken != null)
                                            await firebaseManager.SendNotification(item.FcmToken, fireBaseInfo);
                                        //await sendNotificationcs.SendMessageAsync(item.FcmToken, "Check_events_near_you", fireBaseInfo, _environment.WebRootPath);
                                    }
                                }
                            }
                            catch
                            {
                                continue;
                            }
                        }
                    }

                    return StatusCode(StatusCodes.Status200OK,
                          new ResponseModel<object>(StatusCodes.Status200OK, true,
                          _localizer["YourEventhasbeenAddedsuccessfully"], new
                          {
                              eventdate = model.eventdate.Value.ConvertDateTimeToString(),

                              eventdateto = model.eventdateto.Value.ConvertDateTimeToString(),
                              model.allday,
                              Id = model.EntityId,
                              model.description,
                              model.lang,
                              model.lat,
                              timefrom = model.eventfrom == null ? "" : model.eventfrom.Value.ToString(@"hh\:mm"),
                              timeto = model.eventto == null ? "" : model.eventto.Value.ToString(@"hh\:mm"),
                              model.Title,
                              image = _configuration["BaseUrl"] + model.image,
                              categorie = model.categorie?.name,
                              categorieimage = _configuration["BaseUrl"] + model.categorie?.image,
                              eventtypeid = typedate.EntityId,
                              eventtypecolor = typedate.color,
                              eventtype = typedate.Name,
                              model.totalnumbert,
                              // interests = _Event.GetINterestdata(m.Id).Distinct(),
                              joined = _Event.GetEventattend(model.EntityId),
                              Attendees = _Event.getattendevent(model.EntityId, loggedinUser.UserId).Select(m => new
                              {
                                  DisplayedUserName = m.Userattend.User.UserName,
                                  UserName = m.Userattend.User.DisplayedUserName,
                                  m.Userattend.UserId,
                                  m.stutus,
                                  image = _configuration["BaseUrl"] + m.Userattend.UserImage,
                                  JoinDate = m.JoinDate == null ? CreatDate.Value.ConvertDateTimeToString() : m.JoinDate.Value.Date.ConvertDateTimeToString(),
                                  interests = _Event.GetINterestdata(m.Userattend.PrimaryId).Select(m => new { m.Interests.name, m.InterestsId }).ToList()
                              }).ToList()

                          }));
                }


                return StatusCode(StatusCodes.Status200OK,
                      new ResponseModel<object>(StatusCodes.Status200OK, true,
                       _localizer["YourEventhasbeenAddedsuccessfully"], cuserount));

            }
            catch (Exception ex)
            {
                await _errorLogService.InsertErrorLog(new BWErrorLog(HttpContext.GetUser().UserId, "Events/AddEventData", ex));
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<object>(StatusCodes.Status500InternalServerError, false, ex.Message, null));

            }
        }

        [HttpPost]
        [Route("updateEventData")]
        public async Task<IActionResult> update([FromForm] string ListOfUserIDs, [FromForm] string eventId, [FromForm] IFormFile Eventimage, [FromForm] EventData model)
        {
            try
            {
                var dataconfig = appConfigrationService.GetData().FirstOrDefault();
                if (model.eventtype == null)
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                        new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                         _localizer["event type is Required"], null));
                }
                var typedate = await _EventTypeListService.GetData((Guid)model.eventtype);
                var loggedinUser = HttpContext.GetUser();

                var eventattend = _Event.getattendevent(eventId);
                EventData modeldata = eventattend.FirstOrDefault().EventData;
                //if (typedate.Result.key == "private".ToLower() && modeldata.eventtype.ToLower() != "private".ToLower())
                //var CHECKEVENTLOCATION = _userService.CHECKEVENTLOCATION(loggedinUser.User.UserDetails.lat, loggedinUser.User.UserDetails.lang, model.lat, model.lang, dataconfig);
                //if (CHECKEVENTLOCATION == false)
                //{
                //    return StatusCode(StatusCodes.Status406NotAcceptable,
                //        new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,
                //         _localizer["Please select a nearby LOcation "], null));
                //}
                if (typedate?.Privtekey == true && modeldata.EventTypeList?.key != true)
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                        new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                         _localizer["can't be changed from friendzr  to private Event"], null));
                }
                if (model.totalnumbert < modeldata.totalnumbert && eventattend.Count() > model.totalnumbert)
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                        new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                         _localizer["limit number for attendees can't be less than the current attendees"], null));
                }
                if (model.eventdate == null || model.eventdateto == null)
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                        new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                         _localizer["start and end date is required"], null));
                }
                if ((model.eventfrom == null || model.eventto == null) && model.allday == false)
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                        new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                         _localizer["start and end time is required"], null));
                }

                if (model.eventdateto < DateTime.Now.Date || model.eventdate > DateTime.Now.Date.AddYears(1))
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                        new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                         _localizer["EventdatemustNotinthepast"], null));
                }
                if (model.eventdateto > model.eventdate.Value.AddMonths(1))
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                        new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                         _localizer["Maximumallowedeventperiod"], null));
                }
                if ((model.totalnumbert + 1) < 3)
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                        new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                         _localizer["AttendeesNumbershouldnotbelessthan2"], null));
                }
                if (model.eventdate > model.eventdateto)
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                        new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                         _localizer["EventstartdatemustNotolderthanEventenddate"], null));
                }
                if (model.eventfrom >= model.eventto)
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                        new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                         _localizer["EventstarttimemustNotolderthanEventendtime"], null));
                }
                if (model.Title == null)
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                        new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                         _localizer["Title is Required"], null));
                }
                if (model.description == null)
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                        new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                         _localizer["description is Required"], null));
                }
                if (dataconfig.EventTitle_MinLength != null)
                {
                    if (model.Title.Length < dataconfig.EventTitle_MinLength)
                    {
                        return StatusCode(StatusCodes.Status406NotAcceptable,
                            new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                                      _localizer["Event  Title Min Length is "] + dataconfig.EventTitle_MinLength, null));

                    }
                }
                if (dataconfig.EventTitle_MaxLength != null)
                {
                    if (model.Title.Length > dataconfig.EventTitle_MaxLength)
                    {
                        return StatusCode(StatusCodes.Status406NotAcceptable,
                            new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                                _localizer["Event Title Max Length is "] + dataconfig.EventTitle_MaxLength, null));

                    }
                }
                if (dataconfig.EventDetailsDescription_MaxLength != null)
                {
                    if (model.description.Length > dataconfig.EventDetailsDescription_MaxLength)
                    {
                        return StatusCode(StatusCodes.Status406NotAcceptable,
                            new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                              _localizer["Event Details Description Max Length is "] + dataconfig.EventDetailsDescription_MaxLength, null));
                    }
                }
                if (dataconfig.EventDetailsDescription_MinLength != null)
                {
                    if (model.description.Length < dataconfig.EventDetailsDescription_MinLength)
                    {
                        return StatusCode(StatusCodes.Status406NotAcceptable,
                            new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                                _localizer["Event Details Description Min Length is "] + dataconfig.EventDetailsDescription_MinLength, null));
                    }
                }
                //if (model.Title.Length > 32)
                //{
                //    return StatusCode(StatusCodes.Status406NotAcceptable,
                //        new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                //         _localizer["TitlemustNotmorethan32characters."], null));
                //}
                //if (model.description.Length > 150)
                //{
                //    return StatusCode(StatusCodes.Status406NotAcceptable,
                //        new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                //         _localizer["descriptionismustNotmorethan150characters."], null));
                //}
                if (model.totalnumbert == 0)
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                        new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                         _localizer["total number is Required"], null));
                }
                if (model.totalnumbert > 5000)
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                        new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                         _localizer["TotalNumbermustnottobemorethan5000"], null));
                }

                if (model.allday == null)
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                        new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                         _localizer["All day is Required"], null));
                }
                string imageName = null;
                if (modeldata == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound,
                    new ResponseModel<object>(StatusCodes.Status404NotFound, false,
                    "Sorry, the event does not exist!", null));
                }
                var UserId = loggedinUser.User.UserDetails;
                var listdata = _FrindRequest.GetallFrendes(UserId.PrimaryId, null);


                if (typedate?.Privtekey == true)
                {
                    var list = JsonConvert.DeserializeObject<List<string>>(ListOfUserIDs).ToList();
                    var userDeatils = this._userService.GetLISTUserDetails(list).ToList();

                    if (model.totalnumbert < list.Count)
                    {


                        return StatusCode(StatusCodes.Status406NotAcceptable,
                               new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                                _localizer[" can not add more than "] + model.totalnumbert, null));


                    }
                    foreach (var item in list)
                    {
                        if (listdata.FirstOrDefault(v => v.status == 1 && (v.User.UserId == item || v.UserRequest.UserId == item)) == null)
                        {
                            return StatusCode(StatusCodes.Status406NotAcceptable,
                                   new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                                    _localizer[" can only add your friends to the event"], null));

                        }
                    }
                }
                if (Eventimage != null)
                {
                    globalMethodsService.DeleteFiles(modeldata.image, "");
                    var UniqName = await globalMethodsService.uploadFileAsync("/Images/EventData/", Eventimage);
                    imageName = "/Images/EventData/" + UniqName;

                    model.image = imageName;
                    modeldata.image = imageName;
                }

                // var userDeatils = this._userService.GetUserDetails(loggedinUser.UserId);
                modeldata.Title = model.Title;

                modeldata.description = model.description;
                modeldata.status = model.status;
                modeldata.eventdateto = model.eventdateto;
                modeldata.showAttendees = model.showAttendees;
                modeldata.totalnumbert = model.totalnumbert;
                modeldata.eventtype = model.eventtype;
                modeldata.allday = model.allday;
                if ((model.eventdate > DateTime.Now.Date))
                {

                    modeldata.eventdate = model.eventdate;
                }
                modeldata.eventfrom = model.eventfrom;
                modeldata.eventto = model.eventto;
                modeldata.EventTypeListid = typedate.ID;
                await _Event.updateEvent(modeldata);
                var cuserount = _Event.getevent(model.EntityId);
                if (cuserount != null)
                {
                    //var data = cuserount.Select(m => new
                    //{
                    //    eventdate = m.eventdate.Value.ConvertDateTimeToString(),

                    //    eventdateto = m.eventdateto.Value.ConvertDateTimeToString(),
                    //    m.allday,
                    //    timefrom = m.eventfrom == null ? "" : m.eventfrom.Value.ToString(@"hh\:mm"),
                    //    timeto = m.eventto == null ? "" : m.eventto.Value.ToString(@"hh\:mm"),

                    //    m.Title,
                    //    image = _configuration["BaseUrl"] + m.image,
                    //    Id = m.EntityId,
                    //    categorie = m.categorie.name,
                    //    categorieimage = _configuration["BaseUrl"] + m.categorie.image,

                    //    m.totalnumbert,
                    //    m.description,
                    //    //interests = _Event.GetINterestdata(m.Id).Distinct(),
                    //    joined = _Event.GetEventattend(m.EntityId),
                    //    Attendees = _Event.getattendevent(m.EntityId, loggedinUser.UserId).Select(m => new
                    //    {
                    //        DisplayedUserName = m.Userattend.User.UserName,
                    //        UserName = m.Userattend.User.DisplayedUserName,
                    //        m.Userattend.UserId,
                    //        m.stutus,
                    //        image = _configuration["BaseUrl"] + m.Userattend.UserImage,
                    //        JoinDate = m.JoinDate == null ? DateTime.Now.Date.ConvertDateTimeToString() : m.JoinDate.Value.Date.ConvertDateTimeToString(),
                    //        interests = _Event.GetINterestdata(m.Userattend.PrimaryId).Select(m => new { m.Interests.name, m.InterestsId }).ToList()
                    //    }).ToList()
                    //}).ToList();
                    var allateend = _Event.allattendevent();
                    if (string.IsNullOrEmpty(ListOfUserIDs) == false && typedate?.Privtekey == true)
                    {
                        {
                            try
                            {
                                var list = JsonConvert.DeserializeObject<List<string>>(ListOfUserIDs).ToList();
                                var userDeatils = this._userService.GetLISTUserDetails(list).ToList();


                                foreach (var item in userDeatils)
                                {
                                    var CHECUSERATTEND = eventattend.FirstOrDefault(M => M.UserattendId == item.PrimaryId);

                                    if (CHECUSERATTEND == null)
                                    {

                                        EventTracker eventTracker = new EventTracker()
                                        {
                                            EventId = model.Id,
                                            UserId = item.PrimaryId,
                                            Date = DateTime.Now,
                                            ActionType = EventActionType.Attend.ToString()
                                        };

                                        _authContext.EventTrackers.Add(eventTracker);

                                        model.Attendees += 1;

                                        //eventattend eventattendprivet = new eventattend();
                                        //eventattendprivet.EventDataid = modeldata.Id;
                                        //eventattendprivet.UserattendId = item.PrimaryId;
                                        //eventattendprivet.JoinDate = DateTime.Now;

                                        //var JoinTimeForMessage = Creattime.HasValue ? ((TimeSpan)Creattime).Add(new TimeSpan(0, 0, 2)) : Creattime;
                                        var aprivit = await _Event.InsertEventChatAttend(new EventChatAttend { Jointime = DateTime.Now.TimeOfDay, EventDataid = modeldata.Id, UserattendId = item.PrimaryId, JoinDate = DateTime.Now, ISAdmin = false });

                                        await MessageServes.addeventmessage(new EventMessageDTO { EventChatAttendid = aprivit.Id, eventjoin = false, Message = "", Messagetype = 1, EventId = model.EntityId, Messagesdate = DateTime.Now, Messagestime = DateTime.Now.TimeOfDay }, item);
                                        try
                                        {


                                            FireBaseData fireBaseInfo = new FireBaseData() { Title = model.Title, Body = UserId.User.DisplayedUserName + " added you in a private Event   " + model.Title, imageUrl = _configuration["BaseUrl"] + model.image, Action_code = model.EntityId, muit = false, Action = "Check_private_events" };
                                            var addnoti = MessageServes.getFireBaseData(item.PrimaryId, fireBaseInfo, DateTime.Now, DateTime.Now.TimeOfDay);
                                            await MessageServes.addFireBaseDatamodel(addnoti);
                                            SendNotificationcs sendNotificationcs = new SendNotificationcs();
                                            if (item.FcmToken != null)
                                                await firebaseManager.SendNotification(item.FcmToken, fireBaseInfo);
                                        }
                                        catch
                                        {

                                        }
                                    }
                                    else
                                    {
                                        if (CHECUSERATTEND.stutus == 1)
                                        {
                                            CHECUSERATTEND.delete = false;
                                            CHECUSERATTEND.removefromevent = false;
                                            CHECUSERATTEND.leave = false;
                                            CHECUSERATTEND.leavechat = false;
                                            CHECUSERATTEND.stutus = 0;
                                            CHECUSERATTEND.JoinDate = DateTime.Now.Date;
                                            CHECUSERATTEND.Jointime = DateTime.Now.TimeOfDay;
                                            await _Event.editeEventChatAttend(CHECUSERATTEND);
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                var M = ex;
                                M = ex;
                            }
                        }
                    }
                    var Eventattend = _Event.getallChatattendevent(eventId, allateend).ToList();

                    foreach (var even in Eventattend)
                    {
                        if (even.UserattendId != loggedinUser.User.UserDetails.PrimaryId)
                        {
                            try
                            {
                                var userto = this._userService.GetUserDetails(even.Userattend.UserId);

                                FireBaseData fireBaseInfo = new FireBaseData()
                                {
                                    Title = modeldata.Title,
                                    Body = "Changes have been made to this event!",
                                    imageUrl = _configuration["BaseUrl"] + modeldata.image,
                                    muit = false,
                                    Action_code = modeldata.EntityId,
                                    Action = "event_Updated"
                                };

                                SendNotificationcs sendNotificationcs = new SendNotificationcs();
                                if (userto.FcmToken != null)
                                {

                                    await firebaseManager.SendNotification(userto.FcmToken, fireBaseInfo);
                                }
                                //await firebaseManager.SendNotification(Deatils?.FcmToken, fireBaseInfo);


                                //await sendNotificationcs.SendMessageAsync(userto.FcmToken, "event_Updated", fireBaseInfo, _environment.WebRootPath);

                                var addnoti = MessageServes.getFireBaseData(userto.PrimaryId, fireBaseInfo);
                                await MessageServes.addFireBaseDatamodel(addnoti);
                            }
                            catch
                            {
                                continue;
                            }
                        }
                    }

                    return StatusCode(StatusCodes.Status200OK,
                             new ResponseModel<object>(StatusCodes.Status200OK, true,
                            _localizer["YourEventhasbeenUpdatedsuccessfully"], null));

                }
                return StatusCode(StatusCodes.Status200OK,
                      new ResponseModel<object>(StatusCodes.Status200OK, true,
                      _localizer["YourEventhasbeenUpdatedsuccessfully!"], cuserount));

            }
            catch (Exception ex)
            {
                await _errorLogService.InsertErrorLog(new BWErrorLog(HttpContext.GetUser().UserId, "Events/Update", ex));
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<object>(StatusCodes.Status500InternalServerError, false, ex.Message, null));

            }
        }

        [HttpPost]
        [Route("DeletEventData")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> DELETE([FromForm] string id)
        {
            try
            {

                var loggedinUser = HttpContext.GetUser();


                var Event = _Event.GetEventbyid(id);


                if (Event != null)
                {
                    if (Event.UserId != loggedinUser.User.UserDetails.PrimaryId)
                    {
                        //await _Event.deleteEvent(id);
                        return StatusCode(StatusCodes.Status406NotAcceptable,
                          new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,
                          _localizer["The event does not belong to you!"], null));
                    }

                    await _Event.deleteEvent(id);
                    return StatusCode(StatusCodes.Status200OK,
                      new ResponseModel<object>(StatusCodes.Status200OK, true,
                     _localizer["Your Event has been Deleted successfully!"], null));
                }
                else
                {
                    return StatusCode(StatusCodes.Status404NotFound,
                    new ResponseModel<object>(StatusCodes.Status404NotFound, false,
                    "Sorry, the event does not exist!", null));
                }

            }
            catch (Exception ex)
            {
                await _errorLogService.InsertErrorLog(new BWErrorLog(HttpContext.GetUser().UserId, "Events/DeletEventData", ex));
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<object>(StatusCodes.Status500InternalServerError, false, ex.Message, null));

            }
        }

        [HttpPost]
        [Route("Deletecategory")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> DELETEcategory([FromForm] string id)
        {
            try
            {

                var loggedinUser = HttpContext.GetUser();
                // get user
                var user = await userManager.FindByIdAsync(loggedinUser.UserId);
                if (user == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound,
                    new ResponseModel<object>(StatusCodes.Status404NotFound, false,
                    _localizer["userdoesnotexist"], null));
                }

                var Event = _Event.Getcategorybyid(id);
                if (Event != null)
                {
                    await _Event.deletecategory(id);
                    return StatusCode(StatusCodes.Status200OK,
                      new ResponseModel<object>(StatusCodes.Status200OK, true,
                      "Your category has been Deleted successfully ", null));
                }

                else
                {
                    return StatusCode(StatusCodes.Status404NotFound,
                    new ResponseModel<object>(StatusCodes.Status404NotFound, false,
                    "Sorry, the category does not exist!", null));
                }

            }
            catch (Exception ex)
            {
                await _errorLogService.InsertErrorLog(new BWErrorLog(HttpContext.GetUser().UserId, "Events/Addcategory", ex));
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<object>(StatusCodes.Status500InternalServerError, false, ex.Message, null));

            }
        }

        [HttpPost]
        [Route("GetAllcategory")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> GetAllcategory()
        {
            try
            {
                var loggedinUser = HttpContext.GetUser();

                // get user
                var user = await userManager.FindByIdAsync(loggedinUser.UserId);
                if (user == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound,
                    new ResponseModel<object>(StatusCodes.Status404NotFound, false,
                    _localizer["userdoesnotexist"], null));
                }

                var data = _Event.getcategory();


                return StatusCode(StatusCodes.Status200OK,
                new ResponseModel<object>(StatusCodes.Status200OK, true,
                " the Interests data!", data.Select(m => new { Id = m.EntityId, m.name })));


            }
            catch (Exception ex)
            {
                await _errorLogService.InsertErrorLog(new BWErrorLog(HttpContext.GetUser().UserId, "Events/getallcategory", ex));
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<object>(StatusCodes.Status500InternalServerError, false, ex.Message, null));

            }
        }

        [HttpPost]
        [Route("Addcategory")]
        //[Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> ADDcategory([FromForm] category model, [FromForm] IFormFile image)
        {
            try
            {

                var loggedinUser = HttpContext.GetUser();

                // get user
                var user = await userManager.FindByIdAsync(loggedinUser.UserId);
                if (user == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound,
                    new ResponseModel<object>(StatusCodes.Status404NotFound, false,
                    _localizer["userdoesnotexist"], null));
                }
                var all = _Event.getcategory().Where(v => v.name == model.name).FirstOrDefault();
                if (all == null)
                {
                    string imageName = "";
                    if (image != null)
                    {
                        var UniqName = await globalMethodsService.uploadFileAsync("/Images/EventData/", image);

                        imageName = "/Images/EventData/" + UniqName;
                        model.image = imageName;
                    }
                    await _Event.Insertcategory(model);

                    return StatusCode(StatusCodes.Status200OK,
                          new ResponseModel<object>(StatusCodes.Status200OK, true,
                          "Your category has been Added successfully !", _Event.Getcategorybyid(model.EntityId)));
                }
                else
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                                              new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,
                                              "Your category already added", _Event.Getcategorybyid(all.EntityId)));

                }
            }
            catch (Exception ex)
            {
                await _errorLogService.InsertErrorLog(new BWErrorLog(HttpContext.GetUser().UserId, "Events/Addcategory", ex));
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<object>(StatusCodes.Status500InternalServerError, false, ex.Message, null));

            }
        }

        [HttpPost]
        [Route("updatecategory")]
        //[Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> updatecategory([FromForm] string categoryid, [FromForm] IFormFile image, [FromForm] category modeldata)
        {
            try
            {

                var loggedinUser = HttpContext.GetUser();
                // get user
                var user = await userManager.FindByIdAsync(loggedinUser.UserId);
                if (user == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound,
                    new ResponseModel<object>(StatusCodes.Status404NotFound, false,
                    _localizer["userdoesnotexist"], null));
                }
                category model = _Event.Getcategorybyid(categoryid);
                if (model == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound,
                    new ResponseModel<object>(StatusCodes.Status404NotFound, false,
                    "Sorry, the category does not exist!", null));
                }
                model.name = modeldata.name;

                var all = _Event.getcategory().Where(v => v.name == model.name && v.Id != model.Id).FirstOrDefault();
                if (all == null)
                {
                    string imageName = "";
                    if (image != null)
                    {
                        var UniqName = await globalMethodsService.uploadFileAsync("/Images/EventData/", image);

                        imageName = "/Images/EventData/" + UniqName;
                        model.image = imageName;
                    }
                    await _Event.updatecategory(model);

                    return StatusCode(StatusCodes.Status200OK,
                       new ResponseModel<object>(StatusCodes.Status200OK, true,
                       "Your category has been updated successfully !", _Event.Getcategorybyid(model.EntityId)));
                }
                else
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                                              new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,
                                              "Your category already added", _Event.Getcategorybyid(all.EntityId)));

                }

            }
            catch (Exception ex)
            {
                await _errorLogService.InsertErrorLog(new BWErrorLog(HttpContext.GetUser().UserId, "Events/updatecategory", ex));
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<object>(StatusCodes.Status500InternalServerError, false, ex.Message, null));

            }
        }

        [HttpPost]
        [Route("joinEvent")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> joinEvent([FromForm] eventattendMV model)
        {
            try
            {
                var t = _Event.GetEventbyid(model.EventDataid);
                if (t == null || t.eventdateto < DateTime.Now.Date || (t.allday == false ? (t.eventdateto == DateTime.Now.Date ? t.eventto <= DateTime.Now.TimeOfDay : false) : false))
                {
                    return StatusCode(StatusCodes.Status404NotFound,
                         new ResponseModel<object>(StatusCodes.Status404NotFound, true,
                         _localizer["Sorry, event ended!"], null));
                }
                var loggedinUser = HttpContext.GetUser();
                model.UserattendId = loggedinUser.User.UserDetails.Id;
                int count = _Event.GetEventattend(model.EventDataid);

                var cuserount = _Event.GetuserEvent(model.EventDataid, loggedinUser.User.Id);
                var cuseroun = _Event.getevent(model.EventDataid);
                var useratte = _Event.getattendevent(cuseroun.FirstOrDefault().EntityId, loggedinUser.User.Id).Where(b => b.UserattendId == loggedinUser.User.UserDetails.PrimaryId).FirstOrDefault();
                var data = cuseroun.Select(m => new
                {
                    eventdate = m.eventdate.Value.ConvertDateTimeToString(),

                    eventdateto = m.eventdateto.Value.ConvertDateTimeToString(),
                    m.allday,
                    m.checkout_details,
                    m.description,
                    timefrom = m.eventfrom == null ? "" : m.eventfrom.Value.ToString(@"hh\:mm"),
                    timeto = m.eventto == null ? "" : m.eventto.Value.ToString(@"hh\:mm"),
                    timetext = m.allday == true ? "All Day" : ((m.eventfrom == null ? "" : m.eventfrom.Value.ToString(@"hh\:mm")) + "    to : " + (m.eventto == null ? "" : m.eventto.Value.ToString(@"hh\:mm"))),
                    datetext = ((m.eventdate == null ? "" : m.eventdate.Value.ConvertDateTimeToString()) + "    to : " + (m.eventdateto == null ? "" : m.eventdateto.Value.ConvertDateTimeToString())),
                    MyEvent = m.UserId == loggedinUser.User.UserDetails.PrimaryId ? true : false,
                    categorie = m.categorie?.name,
                    categorieimage = _configuration["BaseUrl"] + m.categorie?.image,
                    categorieid = m.categorie?.Id,
                    m.Title,
                    image = (m.EventTypeListid != 3 ? _configuration["BaseUrl"] : "") + m.image,
                    Id = m.EntityId,
                    EncryptedID = StringCipher.EncryptString(m.EntityId),
                    m.lang,
                    m.showAttendees,
                    m.lat,
                    eventtypeid = m.EventTypeList.entityID,
                    eventtypecolor = m.EventTypeList.color,
                    eventtype = m.EventTypeList.Name,
                    m.totalnumbert,
                    m.categorie?.EntityId,
                    key = m.UserId == loggedinUser.User.UserDetails.PrimaryId ? 1 : (useratte == null ? 2 : 3),
                    //interests = _Event.GetINterestdata(m.Id).Distinct(),
                    joined = _Event.GetEventattend(m.EntityId),
                    interestStatistic = _Event.GetEventattendstat(model.EventDataid),
                    GenderStatistic = _Event.GetEventattendgender(model.EventDataid),
                    leveevent = (m.UserId == loggedinUser.User.UserDetails.PrimaryId ? 1 :
                        (useratte == null ? 2 : 3)) == 2 ? 0 : (_Event.GetEventChatAttend(model.EntityId, loggedinUser.UserId).leavechat ? 2 : 1)
                }

                );



                if (cuserount != null)
                {
                    if (cuserount.stutus == 2)
                    {
                        return StatusCode(StatusCodes.Status406NotAcceptable,
                          new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,
                          _localizer["YouareBlocked"], null));
                    }
                    if (cuserount.stutus == 1 || (cuserount.stutus == 6 || cuserount.leave == true))
                    {
                        if (count < t.totalnumbert)
                        {

                            var EventChatAttend = _Event.GetEventChatAttend(model.EventDataid, loggedinUser.UserId);

                            EventChatAttend.delete = false;
                            EventChatAttend.removefromevent = false;
                            EventChatAttend.leave = false;
                            EventChatAttend.leavechat = false;
                            EventChatAttend.stutus = 0;
                            EventChatAttend.JoinDate = model.JoinDate;
                            EventChatAttend.Jointime = model.Jointime;
                            await _Event.editeEventChatAttend(EventChatAttend);
                            return StatusCode(StatusCodes.Status200OK,
                              new ResponseModel<object>(StatusCodes.Status200OK, true,
                              _localizer["YouhavesuccessfullyReturntoevent"], data.FirstOrDefault()));
                        }
                        else
                        {
                            return StatusCode(StatusCodes.Status404NotFound,
                          new ResponseModel<object>(StatusCodes.Status404NotFound, true,
                          _localizer["Sorry,Therearenoplaces!"], null));
                        }
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status406NotAcceptable,
                              new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,
                              _localizer["Youarealreadysubscribed"], data.FirstOrDefault()));
                    }
                }
                else if (count < t.totalnumbert)
                {
                    int coun = (count / t.totalnumbert) * 100;
                    if (cuserount != null)
                    {
                        if (cuserount.stutus == 1 || (cuserount.stutus == 6 || cuserount.leave == true))
                        {

                            var EventChatAttend = _Event.GetEventChatAttend(model.EventDataid, loggedinUser.UserId);
                            EventChatAttend.delete = false;
                            EventChatAttend.removefromevent = false;
                            EventChatAttend.leave = false;
                            EventChatAttend.leavechat = false;
                            EventChatAttend.stutus = 0;
                            EventChatAttend.JoinDate = model.JoinDate;
                            EventChatAttend.Jointime = model.Jointime;
                            await _Event.editeEventChatAttend(EventChatAttend);
                            if ((coun >= 30 && coun <= 45) || (coun >= 55 && coun <= 60) || (coun >= 90 && coun <= 100))
                            {
                                FireBaseData fireBaseInfo = new FireBaseData()
                                {
                                    muit = false,
                                    Title = data.FirstOrDefault().Title,
                                    imageUrl = _configuration["BaseUrl"] + data.FirstOrDefault().image,
                                    Body = coun + "% of capacity filled!",
                                    Action_code = data.FirstOrDefault().Id,
                                    Action = "event_attend"
                                };
                                var addnoti = MessageServes.getFireBaseData(loggedinUser.User.UserDetails.PrimaryId, fireBaseInfo, model.JoinDate, model.Jointime);
                                await MessageServes.addFireBaseDatamodel(addnoti);
                                try
                                {
                                    SendNotificationcs sendNotificationcs = new SendNotificationcs();
                                    if (loggedinUser.User.UserDetails.FcmToken != null)
                                        await firebaseManager.SendNotification(loggedinUser.User.UserDetails.FcmToken, fireBaseInfo);
                                    //await sendNotificationcs.SendMessageAsync(loggedinUser.User.UserDetails.FcmToken, "event_attend", fireBaseInfo, _environment.WebRootPath);
                                }
                                catch
                                {

                                    return StatusCode(StatusCodes.Status200OK,
                              new ResponseModel<object>(StatusCodes.Status200OK, true,
                             _localizer["YouhavesuccessfullyReturntoevent"], data.FirstOrDefault()));
                                }

                            }
                            return StatusCode(StatusCodes.Status200OK,
                              new ResponseModel<object>(StatusCodes.Status200OK, true,
                             _localizer["YouhavesuccessfullyReturntoevent"], data.FirstOrDefault()));
                        }
                    }
                    //eventattend eventattend = new eventattend();
                    //eventattend.EventDataid = t.Id;
                    //eventattend.UserattendId = loggedinUser.User.UserDetails.PrimaryId;
                    //eventattend.JoinDate = model.JoinDate;

                    var EventChatAttend2 = _Event.GetEventChatAttend(model.EventDataid, loggedinUser.UserId);
                    if (EventChatAttend2 != null)
                    {
                        EventChatAttend2.delete = false;
                        EventChatAttend2.leavechat = false;
                        EventChatAttend2.leave = false;
                        EventChatAttend2.removefromevent = false;
                        EventChatAttend2.stutus = 0;
                        EventChatAttend2.JoinDate = model.JoinDate;
                        EventChatAttend2.Jointime = model.Jointime;
                        await _Event.editeEventChatAttend(EventChatAttend2);
                    }
                    else
                    {
                        EventTracker eventTracker = new EventTracker()
                        {
                            EventId = cuseroun.FirstOrDefault().Id,
                            UserId = loggedinUser.User.UserDetails.PrimaryId,
                            Date = DateTime.Now,
                            ActionType = EventActionType.Attend.ToString()
                        };

                        _authContext.EventTrackers.Add(eventTracker);
                        cuseroun.FirstOrDefault().Attendees += 1;
                        EventChatAttend2 = await _Event.InsertEventChatAttend(new EventChatAttend { Jointime = model.Jointime, EventDataid = t.Id, UserattendId = loggedinUser.User.UserDetails.PrimaryId, JoinDate = model.JoinDate, ISAdmin = false });
                    }
                    await MessageServes.addeventmessage(new EventMessageDTO { eventjoin = true, EventChatAttendid = EventChatAttend2.Id, Message = "", Messagetype = 1, EventId = model.EventDataid, Messagestime = model.Jointime.Value, Messagesdate = model.JoinDate.Value }, loggedinUser.User.UserDetails);


                    if ((coun >= 30 && coun <= 45) || (coun >= 55 && coun <= 60) || (coun >= 90 && coun <= 100))
                    {
                        FireBaseData fireBaseInfo = new FireBaseData()
                        {
                            muit = false,
                            Title = data.FirstOrDefault().Title,
                            imageUrl = _configuration["BaseUrl"] + data.FirstOrDefault().image,
                            Body = "event attend %" + coun,
                            Action_code = data.FirstOrDefault().Id,
                            Action = "event_attend"
                        };
                        var addnoti = MessageServes.getFireBaseData(loggedinUser.User.UserDetails.PrimaryId, fireBaseInfo, model.JoinDate, model.Jointime);
                        await MessageServes.addFireBaseDatamodel(addnoti);
                        try
                        {
                            SendNotificationcs sendNotificationcs = new SendNotificationcs();
                            if (loggedinUser.User.UserDetails.FcmToken != null)
                                await firebaseManager.SendNotification(loggedinUser.User.UserDetails.FcmToken, fireBaseInfo);
                            //await sendNotificationcs.SendMessageAsync(loggedinUser.User.UserDetails.FcmToken, "event_attend", fireBaseInfo, _environment.WebRootPath);
                        }
                        catch
                        {

                            return StatusCode(StatusCodes.Status200OK,
                                  new ResponseModel<object>(StatusCodes.Status200OK, true,
                                 _localizer["Youhavesuccessfullysubscribedtoevent"], data.FirstOrDefault()));
                        }

                    }
                    return StatusCode(StatusCodes.Status200OK,
                          new ResponseModel<object>(StatusCodes.Status200OK, true,
                        _localizer["Youhavesuccessfullysubscribedtoevent"], data.FirstOrDefault()));
                }

                else
                {
                    return StatusCode(StatusCodes.Status404NotFound,
                          new ResponseModel<object>(StatusCodes.Status404NotFound, true,
                         _localizer["Sorry,Therearenoplaces!"], null));
                }
            }
            catch (Exception ex)
            {
                await _errorLogService.InsertErrorLog(new BWErrorLog(HttpContext.GetUser().UserId, "Events/joinevent", ex));
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<object>(StatusCodes.Status500InternalServerError, false, "You Can Not Join Event Now Try Again ", null));

            }
        }

        [HttpPost]
        [Route("leaveEvent")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> leaveevent([FromForm] string EventDataid, [FromForm] DateTime? leaveeventDate, [FromForm] TimeSpan? leaveeventtime)
        {
            try
            {

                var loggedinUser = HttpContext.GetUser();
                int count = _Event.GetEventattend(EventDataid);
                var t = _Event.GetEventbyid(EventDataid);
                //var EventChatAttend = _Event.GetEventChatAttendbyid(EventDataid);
                var cuserount = _Event.GetuserEvent(EventDataid, loggedinUser.User.Id);
                if (leaveeventDate == null)
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                        new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                         _localizer["leave event Date is Required"], null));
                }
                if (leaveeventtime == null)
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                        new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                         _localizer["leave event time is Required"], null));
                }
                if (cuserount == null)
                {

                    return StatusCode(StatusCodes.Status406NotAcceptable,
                          new ResponseModel<object>(StatusCodes.Status406NotAcceptable, true,
                           _localizer["Youarenotsubscribed"], null));
                }
                else if (cuserount.JoinDate.Value.Date > leaveeventDate.Value.Date || (cuserount.JoinDate.Value.Date == leaveeventDate.Value.Date && cuserount.Jointime > leaveeventtime))
                {

                    return StatusCode(StatusCodes.Status406NotAcceptable,
                          new ResponseModel<object>(StatusCodes.Status406NotAcceptable, true,
                           _localizer["Youarenotsubscribed"], null));
                }
                else
                {

                    var cuseroun = _Event.getevent(EventDataid);

                    var EventChatAttend = _Event.GetEventChatAttend(EventDataid, loggedinUser.User.Id);
                    if (EventChatAttend.stutus == 1 || EventChatAttend.stutus == 2)
                    {

                        return StatusCode(StatusCodes.Status406NotAcceptable,
                              new ResponseModel<object>(StatusCodes.Status406NotAcceptable, true,
                               _localizer["Youarenotsubscribed"], null));
                    }

                    EventTracker eventTracker = await _authContext.EventTrackers.FirstOrDefaultAsync(q => q.EventId == t.Id && q.UserId == loggedinUser.User.UserDetails.PrimaryId && q.ActionType == EventActionType.Attend.ToString());

                    if (eventTracker != null)
                    {
                        if (t.Attendees == null || t.Attendees == 0)
                        {
                            t.Attendees = 0;
                        }
                        else
                        {
                            t.Attendees -= 1;
                        }
                        _authContext.EventTrackers.Remove(eventTracker);
                    }

                    EventChatAttend.leaveeventDate = leaveeventDate;
                    EventChatAttend.leaveeventtime = leaveeventtime;
                    EventChatAttend.leave = true;
                    EventChatAttend.stutus = 1;
                    await _Event.editeEventChatAttend(EventChatAttend);

                    var useratte = _Event.getattendevent(cuseroun.FirstOrDefault().EntityId, loggedinUser.User.Id).Where(b => b.UserattendId == loggedinUser.User.UserDetails.PrimaryId).FirstOrDefault();
                    var data = cuseroun.Select(m => new
                    {
                        eventdate = m.eventdate.Value.ConvertDateTimeToString(),
                        eventdateto = m.eventdateto.Value.ConvertDateTimeToString(),
                        m.allday,
                        m.description,
                        timefrom = m.eventfrom == null ? "" : m.eventfrom.Value.ToString(@"hh\:mm"),
                        timeto = m.eventto == null ? "" : m.eventto.Value.ToString(@"hh\:mm"),
                        categorie = m.categorie?.name,
                        categorieimage = _configuration["BaseUrl"] + m.categorie?.image,
                        m.checkout_details,
                        m.Title,
                        image = (m.EventTypeListid != 3 ? _configuration["BaseUrl"] : "") + m.image,
                        Id = m.EntityId,
                        m.lang,
                        m.lat,
                        m.totalnumbert,
                        key = m.UserId == loggedinUser.User.UserDetails.PrimaryId ? 1 : (useratte == null ? 2 : 3),
                    });
                    //await MessageServes.deleteeventmessage(loggedinUser.User.Id, EventDataid);
                    return StatusCode(StatusCodes.Status200OK,
                          new ResponseModel<object>(StatusCodes.Status200OK, true,
                           _localizer["YouhavesuccessfullyleaveEvent"], data));
                }

            }
            catch (Exception ex)
            {
                await _errorLogService.InsertErrorLog(new BWErrorLog(HttpContext.GetUser().UserId, "Events/leaveevent", ex));
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<object>(StatusCodes.Status500InternalServerError, false, ex.Message, null));

            }
        }

        [HttpPost]
        [Route("getallEvent")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> getallEvent([FromForm] int pageNumber, [FromForm] int pageSize)
        {
            try
            {

                var loggedinUser = HttpContext.GetUser();
                var cuserount = _Event.getallevent(loggedinUser.User.UserDetails.PrimaryId).ToList().Where(m => m.eventdateto.Value.Date.AddDays(5) >= DateTime.Now.Date).ToList().OrderByDescending(m => m.Id).ToList();

                {
                    var allateend = _Event.allattendevent();
                    var validFilter = new PaginationFilter(pageNumber, pageSize);
                    int itemcount = cuserount.Count();
                    var pagedevent = cuserount.Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                       .Take(validFilter.PageSize).ToList();
                    var pagedModel = new PagedResponse<List<EventData>>(pagedevent, validFilter.PageNumber,
               pagedevent.Count(), cuserount.Count());
                    return StatusCode(StatusCodes.Status200OK,
                          new ResponseModel<object>(StatusCodes.Status200OK, true,
                          "data ", new
                          {
                              pageNumber = pagedModel.PageNumber,
                              pageSize = pagedModel.PageSize,
                              totalPages = pagedModel.TotalPages,
                              totalRecords = pagedModel.TotalRecords,

                              data = _Event.getallattendevent(allateend, loggedinUser.User.UserDetails.PrimaryId, pagedModel.Data)

                          }));
                }


            }
            catch (Exception ex)
            {
                await _errorLogService.InsertErrorLog(new BWErrorLog(HttpContext.GetUser().UserId, "Events/getallevent", ex));
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<object>(StatusCodes.Status500InternalServerError, false, ex.Message, null));

            }
        }

        [HttpPost]
        [Route("getMyEvent")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> getMyEvent([FromForm] int pageNumber, [FromForm] int pageSize, [FromForm] string Search)
        {
            try
            {
                var loggedinUser = HttpContext.GetUser();
                //var cuserount = _Event.getalluserevent(
                int id = (loggedinUser.User.UserDetails.PrimaryId);
                {

                    var allateend = _Event.allEventChatAttend();

                    var cuserount = _Event.getalluserevent(id, allateend).Select(m => m.EventData).Distinct().OrderByDescending(m => m.Id);
                    if (Search != null)
                    {
                        cuserount = _Event.getalluserevent(id, allateend).Where(m => m.EventData.Title.Contains(Search)).Select(m => m.EventData).Distinct().OrderByDescending(m => m.Id);

                    }
                    var validFilter = new PaginationFilter(pageNumber, pageSize);
                    int itemcount = cuserount.Count();
                    var pagedevent = cuserount.Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                       .Take(validFilter.PageSize).ToList();


                    bool eventHasExpired = false;
                    if(cuserount.Count() <= 0)
                    {
                        string[] result = { };
                       return StatusCode(StatusCodes.Status200OK, new ResponseModel<object>(StatusCodes.Status200OK, false, "No events there", new
                       {
                           pageNumber = 1,
                           pageSize = 0,
                           totalPages = 0,
                           totalRecords = 0,
                           data = result
                       }));
                    }
                    if ((cuserount.FirstOrDefault().eventdateto.Value.Date <= DateTime.Now.Date))
                    {
                        if ((cuserount.FirstOrDefault().eventto != null && cuserount.FirstOrDefault().eventdateto.Value.Date == DateTime.Now.Date && cuserount.FirstOrDefault().eventto.Value >= DateTime.Now.TimeOfDay) || (cuserount.FirstOrDefault().eventdateto.Value.Date == DateTime.Now.Date && cuserount.FirstOrDefault().allday.Value == true))
                        {
                            eventHasExpired = false;
                        }
                        else
                        {
                            eventHasExpired = true;
                        }
                    }

                    var pagedModel = new PagedResponse<List<EventData>>(pagedevent, validFilter.PageNumber, pagedevent.Count(), cuserount.Count());
                    return StatusCode(StatusCodes.Status200OK,
                          new ResponseModel<object>(StatusCodes.Status200OK, true,
                          "data ", new
                          {
                              pageNumber = pagedModel.PageNumber,
                              pageSize = pagedModel.PageSize,
                              totalPages = pagedModel.TotalPages,
                              totalRecords = pagedModel.TotalRecords,

                              data = pagedModel.Data.Select(m => new
                              {
                                  description = m.description,
                                  categorie = m.categorie?.name,
                                  categorieimage = _configuration["BaseUrl"] + m.categorie?.image,
                                  eventtypeid = m.EventTypeList.entityID,
                                  EventHasExpired = eventHasExpired,
                                  eventtypecolor = m.EventTypeList.color,
                                  eventtype = m.EventTypeList.Name,
                                  lat = m.lat,
                                  lang = m.lang,
                                  Id = m.EntityId,
                                  OrderByDes = m.Id,
                                  eventdate = m.eventdate.Value.ConvertDateTimeToString(),
                                  showAttendees = m.showAttendees == null ? false : m.showAttendees,
                                  eventdateto = m.eventdateto.Value.ConvertDateTimeToString(),
                                  allday = Convert.ToBoolean(m.allday),
                                  timefrom = m.eventfrom == null ? "" : m.eventfrom.Value.ToString(@"hh\:mm"),
                                  timeto = m.eventto == null ? "" : m.eventto.Value.ToString(@"hh\:mm"),
                                  Title = m.Title,
                                  m.checkout_details,
                                  joined = _Event.GetEventattend(m.EntityId, allateend),
                                  image = (m.EventTypeListid != 3 ? _configuration["BaseUrl"] : "") + m.image,
                                  totalnumbert = m.totalnumbert,
                                  key = m.UserId == loggedinUser.User.UserDetails.PrimaryId ? 1 : (allateend.Where(n => n.EventData.EntityId == m.EntityId).Where(b => b.UserattendId == loggedinUser.User.UserDetails.PrimaryId).Where(m => m.stutus != 1 && m.stutus != 2).FirstOrDefault() == null ? 2 : 3),
                                  Attendees = allateend.Where(n => (n.EventData.UserId == loggedinUser.User.UserDetails.PrimaryId && n.EventData.EntityId == m.EntityId)).Select(m => new
                                  {
                                      image = _configuration["BaseUrl"] + m.Userattend.UserImage,
                                      UserName = m.Userattend.User.DisplayedUserName,
                                      DisplayedUserName = m.Userattend.User.UserName,
                                      m.Userattend.UserId,
                                      m.stutus,
                                      JoinDate = m.JoinDate == null ? DateTime.Now.Date.ConvertDateTimeToString() : m.JoinDate.Value.Date.ConvertDateTimeToString(),
                                      interests = _Event.GetINterestdata(m.Userattend.PrimaryId).Select(m => new { m.Interests.name, m.InterestsId }).ToList(),
                                      MyEvent = m.UserattendId == m.EventData.UserId ? true : false,
                                  }).ToList().Take(3)
                              })

                          }));
                }




            }
            catch (Exception ex)
            {
                await _errorLogService.InsertErrorLog(new BWErrorLog(HttpContext.GetUser().UserId, "Events/getallevent", ex));
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<object>(StatusCodes.Status500InternalServerError, false, ex.Message, null));

            }
        }

        [HttpPost]
        [Route("getEvent")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> getEvent([FromForm] string id)
        {
            try
            {
                id = StringCipher.TryDecryptString(id) ?? id;
                var loggedinUser = HttpContext.GetUser();
                var allateend = _Event.AllEventChatAttendByEventId(id);
                var cuserount = _Event.getevent(id);
                if(cuserount.Count==0)
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                          new ResponseModel<object>(StatusCodes.Status406NotAcceptable, true,
                          "the event id not found ", null));
                }
                var allatt = _Event.getallEventChatAttend(cuserount.FirstOrDefault().EntityId, allateend).ToList();
                var useratte = allatt.Where(b => b.UserattendId == loggedinUser.User.UserDetails.PrimaryId).Where(m => m.stutus != 1 && m.stutus != 2).FirstOrDefault();

                if (cuserount.FirstOrDefault().EventTypeList?.key == true && useratte == null)
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                          new ResponseModel<object>(StatusCodes.Status406NotAcceptable, true,
                          "this event is Private ", null));
                }

                if (cuserount?.Count > 0)
                {
                    var DATEAVE = allatt.Where(n => n.EventData.UserId == loggedinUser.User.UserDetails.PrimaryId ||
                    (n.EventData.showAttendees == true ?
                    ((allatt.FirstOrDefault(m => m.UserattendId == loggedinUser.User.UserDetails.PrimaryId && m.stutus != 1 && m.stutus != 2) == null ? false : true)) : false)).ToList();

                    bool eventHasExpired = false;

                    if ((cuserount.FirstOrDefault().eventdateto.Value.Date <= DateTime.Now.Date))
                    {
                        if ((cuserount.FirstOrDefault().eventto != null && cuserount.FirstOrDefault().eventdateto.Value.Date == DateTime.Now.Date && cuserount.FirstOrDefault().eventto.Value >= DateTime.Now.TimeOfDay) || (cuserount.FirstOrDefault().eventdateto.Value.Date == DateTime.Now.Date && cuserount.FirstOrDefault().allday.Value == true))
                        {
                            eventHasExpired = false;
                        }
                        else
                        {
                            eventHasExpired = true;
                        }
                    }

                    var data = cuserount.Select(m => new
                    {
                        eventdate = m.eventdate.Value.ConvertDateTimeToString(),

                        eventdateto = m.eventdateto.Value.ConvertDateTimeToString(),
                        m.allday,
                        EventHasExpired = eventHasExpired,
                        m.description,
                        timefrom = m.eventfrom == null ? "" : m.eventfrom.Value.ToString(@"hh\:mm"),
                        timeto = m.eventto == null ? "" : m.eventto.Value.ToString(@"hh\:mm"),
                        timetext = m.allday == true ? "All Day" : ((m.eventfrom == null ? "" : m.eventfrom.Value.ToString(@"hh\:mm")) + "    to : " + (m.eventto == null ? "" : m.eventto.Value.ToString(@"hh\:mm"))),
                        datetext = ((m.eventdate == null ? "" : m.eventdate.Value.ConvertDateTimeToString()) + "    to : " + (m.eventdateto == null ? "" : m.eventdateto.Value.ConvertDateTimeToString())),
                        MyEvent = m.UserId == loggedinUser.User.UserDetails.PrimaryId ? true : false,
                        categorie = m.categorie?.name,
                        categorieimage = _configuration["BaseUrl"] + m.categorie?.image,
                        categorieid = m.categorie?.Id,
                        m.Title,
                        image = (m.EventTypeListid != 3 ? _configuration["BaseUrl"] : "") + m.image,
                        Id = m.EntityId,
                        EncryptedID = StringCipher.EncryptString(m.EntityId),
                        m.lang,
                        m.showAttendees,
                        m.lat,
                        m.checkout_details,
                        eventtypeid = m.EventTypeList.entityID,
                        eventtypecolor = m.EventTypeList.color,
                        eventtype = m.EventTypeList.Name,
                        eventTypeName= m.EventTypeList.Name.Contains("White") ? "Whitelabel" : m.EventTypeList.Name,
                        m.totalnumbert,
                        m.categorie?.EntityId,
                        key = m.UserId == loggedinUser.User.UserDetails.PrimaryId ? 1 : (useratte == null ? 2 : 3),
                        interests = _Event.GetINterestdata(m.Id).Distinct(),
                        joined = _Event.GetEventattend(m.EntityId),
                        interestStatistic = _Event.GetEventattendstat(id),
                        GenderStatistic = _Event.GetEventattendgender(id),
                        leveevent = (m.UserId == loggedinUser.User.UserDetails.PrimaryId ? 1 :
                        (useratte == null ? 2 : 3)) == 2 ? 0 : (_Event.GetEventChatAttend(cuserount.FirstOrDefault().EntityId, loggedinUser.UserId) == null ? 1 : (_Event.GetEventChatAttend(cuserount.FirstOrDefault().EntityId, loggedinUser.UserId).leavechat ? 2 : 1)),
                        Attendees = DATEAVE.Select(m => new
                        {
                            image = _configuration["BaseUrl"] + m.Userattend.UserImage,
                            UserName = m.Userattend.User.DisplayedUserName,
                            DisplayedUserName = m.Userattend.User.UserName,
                            m.Userattend.UserId,
                            m.stutus,
                            JoinDate = m.JoinDate == null ? DateTime.Now.Date.ConvertDateTimeToString() : m.JoinDate.Value.Date.ConvertDateTimeToString(),
                            interests = _Event.GetINterestdata(m.Userattend.PrimaryId).Select(m => new { m.Interests.name, m.InterestsId }).ToList(),
                            MyEvent = m.EventData.User.PrimaryId == m.Userattend.PrimaryId ? true : false,
                        }).ToList().Take(3)
                    });

                    if (cuserount.FirstOrDefault().Views == null)
                    {
                        cuserount.FirstOrDefault().Views = 1;
                    }
                    else
                    {
                        cuserount.FirstOrDefault().Views += 1;
                    }
                    EventTracker eventTracker = new EventTracker()
                    {
                        EventId = cuserount.FirstOrDefault().Id,
                        UserId = loggedinUser.User.UserDetails.PrimaryId,
                        Date = DateTime.Now,
                        ActionType = EventActionType.View.ToString()
                    };

                    _authContext.EventTrackers.Add(eventTracker);

                    await _authContext.SaveChangesAsync();


                    return StatusCode(StatusCodes.Status200OK,
                          new ResponseModel<object>(StatusCodes.Status200OK, true,
                          "data ", data.FirstOrDefault()));
                }
                else
                {


                    return StatusCode(StatusCodes.Status404NotFound,
                          new ResponseModel<object>(StatusCodes.Status404NotFound, true,
                          "not event found ", null));
                }


            }
            catch (Exception ex)
            {
                await _errorLogService.InsertErrorLog(new BWErrorLog(HttpContext.GetUser().UserId, "Events/getevent", ex));
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<object>(StatusCodes.Status500InternalServerError, false, ex.Message, null));

            }
        }

        [HttpPost]
        [Route("getEventAttende")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> getEventAttendees([FromForm] string id, [FromForm] int pageNumber, [FromForm] int pageSize, [FromForm] string search)
        {
            try
            {

                var loggedinUser = HttpContext.GetUser();
                var validFilter = new PaginationFilter(pageNumber, pageSize);

                var cuserount = _Event.getevent(id);
                if (cuserount != null && cuserount.Count > 0)
                {
                    var user = loggedinUser.User;
                    var userid = user.Id;
                    int preid = user.UserDetails.PrimaryId;
                    var listdata = _Event.getEventChatAttend(id, userid).Where(x => (search == null || search == "" || x.Userattend.User.DisplayedUserName.ToLower().Trim().Contains(search.Trim().ToLower())))
                        .ToList();

                    var pagedLands = listdata.Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).ToList();
                    var pagedModel = new PagedResponse<List<EventChatAttend>>(pagedLands, validFilter.PageNumber,
                pagedLands.Count(), listdata.Count());
                    return StatusCode(StatusCodes.Status200OK,
                     new ResponseModel<object>(StatusCodes.Status200OK, true,
                     "data", new
                     {
                         pageNumber = pagedModel.PageNumber,
                         pageSize = pagedModel.PageSize,
                         totalPages = pagedModel.TotalPages,
                         totalRecords = pagedModel.TotalRecords,
                         data = pagedModel.Data.Select(m => new
                         {
                             MyEvent = m.EventData.UserId == m.UserattendId ? true : false,
                             JoinDate = m.JoinDate == null ? DateTime.Now.Date.ConvertDateTimeToString() : m.JoinDate.Value.Date.ConvertDateTimeToString(),
                             UserName = m.Userattend.User.DisplayedUserName,
                             m.Userattend.UserId,
                             m.stutus,
                             image = _configuration["BaseUrl"] + m.Userattend.UserImage
                         }).ToList()
                     }));




                }
                else
                {


                    return StatusCode(StatusCodes.Status404NotFound,
                          new ResponseModel<object>(StatusCodes.Status404NotFound, true,
                          "not event found ", null));
                }


            }
            catch (Exception ex)
            {
                await _errorLogService.InsertErrorLog(new BWErrorLog(HttpContext.GetUser().UserId, "Events/geteventattend", ex));
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<object>(StatusCodes.Status500InternalServerError, false, ex.Message, null));

            }
        }

        [HttpPost]
        [Route("Clickoutevent")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> Clickoutevent([FromForm] eventclickout model)
        {
            try
            {

                var loggedinUser = HttpContext.GetUser();
                var Event = _Event.GetEventbyid(model.EventDataid);


                if (Event != null)
                {
                    if (Event.User.UserId == model.UserattendId)
                    {
                        //await _Event.deleteEvent(id);
                        return StatusCode(StatusCodes.Status406NotAcceptable,
                          new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,
                          _localizer["The event  belong to you!,Event creator can't delete or lock "], null));
                    }
                }
                // get user
                var user = await userManager.FindByIdAsync(loggedinUser.UserId);
                if (user == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound,
                    new ResponseModel<object>(StatusCodes.Status404NotFound, false,
                    _localizer["userdoesnotexist"], null));
                }
                var EventChatAttend = _Event.GetEventChatAttend(model.EventDataid, _userService.getallUserDetails().Where(m => m.UserId == model.UserattendId).FirstOrDefault().UserId);

                if (EventChatAttend != null)
                {

                    EventChatAttend.deletedate = DateTime.Now.Date;
                    EventChatAttend.delettime = DateTime.Now.TimeOfDay;
                    EventChatAttend.removefromevent = true;
                    EventChatAttend.stutus = model.stutus;
                    await _Event.editeEventChatAttend(EventChatAttend);
                    var cuseroun = _Event.getevent(model.EventDataid);
                    var useratte = _Event.getattendevent(cuseroun.FirstOrDefault()?.EntityId, user.Id)?.Where(b => b.Userattend.UserId == user.Id).FirstOrDefault();
                    var data = cuseroun.Select(m => new
                    {
                        eventdate = m.eventdate.Value.ConvertDateTimeToString(),
                        eventdateto = m.eventdateto.Value.ConvertDateTimeToString(),
                        m.allday,
                        m.description,
                        timefrom = m.eventfrom == null ? "" : m.eventfrom.Value.ToString(@"hh\:mm"),
                        timeto = m.eventto == null ? "" : m.eventto.Value.ToString(@"hh\:mm"),
                        categorie = m.categorie?.name,
                        categorieimage = _configuration["BaseUrl"] + m.categorie?.image,

                        m.Title,
                        image = (m.EventTypeListid != 3 ? _configuration["BaseUrl"] : "") + m.image,
                        Id = m.EntityId,
                        m.lang,
                        m.lat,
                        m.totalnumbert,
                        //key = m.UserId == userDeatils.PrimaryId ? 1 : (useratte == null ? 2 : 3),
                    });
                    var mressage = model.stutus == 1 ? " You deleted this account successfully" : "You blocked this account successfully";
                    return StatusCode(StatusCodes.Status200OK,
                          new ResponseModel<object>(StatusCodes.Status200OK, true,
                         mressage, data));
                }

                else
                {
                    return StatusCode(StatusCodes.Status200OK,
                          new ResponseModel<object>(StatusCodes.Status200OK, true,
                          "Sorry,There are no subscribed with same name !", null));
                }
            }
            catch (Exception ex)
            {
                await _errorLogService.InsertErrorLog(new BWErrorLog(HttpContext.GetUser().UserId, "Events/clickout", ex));
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<object>(StatusCodes.Status500InternalServerError, false, ex.Message, null));

            }
        }

        [HttpPost]
        [Route("leaveeventchat")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> leaveeventchat([FromForm] eventclickout model)
        {
            try
            {

                var loggedinUser = HttpContext.GetUser();

                // get user
                var user = await userManager.FindByIdAsync(loggedinUser.UserId);
                if (user == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound,
                    new ResponseModel<object>(StatusCodes.Status404NotFound, false,
                    _localizer["userdoesnotexist"], null));
                }

                var cuserount = _Event.GetEventChatAttend(model.EventDataid, loggedinUser.UserId);


                if (cuserount != null)
                {
                    //cuserount.stutus = 3;
                    cuserount.leveeventchatDate = model.ActionDate;
                    cuserount.leveeventchattime = model.Actiontime;
                    cuserount.leavechat = true;
                    await _Event.editeEventChatAttend(cuserount);
                    var cuseroun = _Event.getevent(model.EventDataid);
                    var useratte = _Event.getattendevent(cuseroun.FirstOrDefault().EntityId, user.Id).Where(b => b.Userattend.UserId == user.Id).FirstOrDefault();
                    var data = cuseroun.Select(m => new
                    {
                        eventdate = m.eventdate.Value.ConvertDateTimeToString(),
                        eventdateto = m.eventdateto.Value.ConvertDateTimeToString(),
                        m.allday,
                        m.description,
                        timefrom = m.eventfrom == null ? "" : m.eventfrom.Value.ToString(@"hh\:mm"),
                        timeto = m.eventto == null ? "" : m.eventto.Value.ToString(@"hh\:mm"),
                        categorie = m.categorie?.name,
                        categorieimage = _configuration["BaseUrl"] + m.categorie?.image,

                        m.Title,
                        image = (m.EventTypeListid != 3 ? _configuration["BaseUrl"] : "") + m.image,
                        Id = m.EntityId,
                        m.lang,
                        m.lat,
                        m.totalnumbert,
                        //key = m.UserId == userDeatils.PrimaryId ? 1 : (useratte == null ? 2 : 3),
                    });
                    return StatusCode(StatusCodes.Status200OK,
                          new ResponseModel<object>(StatusCodes.Status200OK, true,
                          "  you hade left this event chat", data));
                }

                else
                {
                    return StatusCode(StatusCodes.Status200OK,
                          new ResponseModel<object>(StatusCodes.Status200OK, true,
                          "Sorry,There are no subscribed with same name !", null));
                }
            }
            catch (Exception ex)
            {
                await _errorLogService.InsertErrorLog(new BWErrorLog(HttpContext.GetUser().UserId, "Events/clickout", ex));
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<object>(StatusCodes.Status500InternalServerError, false, ex.Message, null));

            }
        }

        [HttpPost]
        [Route("joineventchat")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> joineventchat([FromForm] eventclickout model)
        {
            try
            {

                var loggedinUser = HttpContext.GetUser();

                // get user
                var user = await userManager.FindByIdAsync(loggedinUser.UserId);
                if (user == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound,
                    new ResponseModel<object>(StatusCodes.Status404NotFound, false,
                    _localizer["userdoesnotexist"], null));
                }

                var cuserount = _Event.GetEventChatAttend(model.EventDataid, loggedinUser.UserId);

                if (cuserount != null)
                {
                    cuserount.stutus = 0;
                    cuserount.leveeventchatDate = null;
                    cuserount.leveeventchattime = null;
                    cuserount.leavechat = false;
                    await _Event.editeEventChatAttend(cuserount);
                    var cuseroun = _Event.getevent(model.EventDataid);
                    var useratte = _Event.getattendevent(cuseroun.FirstOrDefault().EntityId, user.Id).Where(b => b.Userattend.UserId == user.Id).FirstOrDefault();
                    var data = cuseroun.Select(m => new
                    {
                        eventdate = m.eventdate.Value.ConvertDateTimeToString(),
                        eventdateto = m.eventdateto.Value.ConvertDateTimeToString(),
                        m.allday,
                        m.description,
                        timefrom = m.eventfrom == null ? "" : m.eventfrom.Value.ToString(@"hh\:mm"),
                        timeto = m.eventto == null ? "" : m.eventto.Value.ToString(@"hh\:mm"),
                        categorie = m.categorie?.name,
                        categorieimage = _configuration["BaseUrl"] + m.categorie?.image,

                        m.Title,
                        image = (m.EventTypeListid != 3 ? _configuration["BaseUrl"] : "") + m.image,
                        Id = m.EntityId,
                        m.lang,
                        m.lat,
                        m.totalnumbert,
                        //key = m.UserId == userDeatils.PrimaryId ? 1 : (useratte == null ? 2 : 3),
                    });
                    return StatusCode(StatusCodes.Status200OK,
                          new ResponseModel<object>(StatusCodes.Status200OK, true,
                          " You have joined this event chat", data));
                }

                else
                {
                    return StatusCode(StatusCodes.Status200OK,
                          new ResponseModel<object>(StatusCodes.Status200OK, true,
                          "Sorry,There are no subscribed with same name !", null));
                }
            }
            catch (Exception ex)
            {
                await _errorLogService.InsertErrorLog(new BWErrorLog(HttpContext.GetUser().UserId, "Events/clickout", ex));
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<object>(StatusCodes.Status500InternalServerError, false, ex.Message, null));

            }
        }

        [HttpPost]
        [Route("locationEvente")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> geteventbylocation([FromForm] string lang, [FromForm] string lat)
        {
            try
            {
                var loggedinUser = HttpContext.GetUser();
                var cuserount = _Event.geteventbylocation(lang, lat);
                var allateend = _Event.allattendevent();
                var data = cuserount.Select(m => new
                {
                    eventdate = m.eventdate.Value.ConvertDateTimeToString(),

                    eventdateto = m.eventdateto.Value.ConvertDateTimeToString(),
                    allday = m.allday,
                    m.description,
                    timefrom = m.eventfrom == null ? "" : m.eventfrom.Value.ToString(@"hh\:mm"),
                    timeto = m.eventto == null ? "" : m.eventto.Value.ToString(@"hh\:mm"),
                    category = m.categorie?.name,
                    categorieId = m.categorie?.EntityId,
                    categorieimage = _configuration["BaseUrl"] + m.categorie?.image,
                    eventtypeid = m.EventTypeList.entityID,
                    eventtypecolor = m.EventTypeList.color,
                    eventtype = m.EventTypeList.Name,
                    m.Title,
                    image = (m.EventTypeListid != 3 ? _configuration["BaseUrl"] : "") + m.image,
                    Id = m.EntityId,
                    m.lang,
                    m.lat,
                    m.totalnumbert,
                    key = m.UserId == loggedinUser.User.UserDetails.PrimaryId ? 1 : ((_Event.getallEventChatAttend(m.EntityId, allateend).Where(b => b.UserattendId == loggedinUser.User.UserDetails.PrimaryId).Where(m => m.stutus != 1 && m.stutus != 2).FirstOrDefault()) == null ? 2 : 3),

                    joined = _Event.GetEventattend(m.EntityId),
                    Attendees = _Event.getattendevent(m.EntityId, loggedinUser.User.Id).Select(m => new
                    {
                        image = _configuration["BaseUrl"] + m.Userattend.UserImage,
                        m.Userattend.User.UserName,
                        m.Userattend.UserId,
                        m.stutus,
                        interests = _Event.GetINterestdata(m.Userattend.PrimaryId).Select(m => new { m.Interests.name, m.InterestsId }).ToList(),
                        JoinDate = m.JoinDate == null ? DateTime.Now.Date.ConvertDateTimeToString() : m.JoinDate.Value.Date.ConvertDateTimeToString()
                    }
                        )
                });
                return StatusCode(StatusCodes.Status200OK,
                     new ResponseModel<object>(StatusCodes.Status200OK, true,
                     "data ", data));

            }
            catch (Exception ex)
            {
                await _errorLogService.InsertErrorLog(new BWErrorLog(HttpContext.GetUser().UserId, "Events/locationevent", ex));
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<object>(StatusCodes.Status500InternalServerError, false, ex.Message, null));

            }
        }

        [HttpPost]
        [Route("Genderbylocation")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> Genderbylocation([FromForm] string lang, [FromForm] string lat)
        {
            try
            {
                var loggedinUser = HttpContext.GetUser();
                var cuserount = _Event.getUserDetailsbylocation(lang, lat);


                return StatusCode(StatusCodes.Status200OK,
                     new ResponseModel<object>(StatusCodes.Status200OK, true,
                     "data ", cuserount));

            }
            catch (Exception ex)
            {
                await _errorLogService.InsertErrorLog(new BWErrorLog(HttpContext.GetUser().UserId, "event/Genderbylocation", ex));
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<object>(StatusCodes.Status500InternalServerError, false, ex.Message, null));

            }
        }

        [HttpPost]
        [Route("AllLocationEvente")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> geteventalllocation()
        {
            try
            {

                var loggedinUser = HttpContext.GetUser();


                var cuserount = _Event.getAlleventlocation();



                return StatusCode(StatusCodes.Status200OK,
                      new ResponseModel<object>(StatusCodes.Status200OK, true,
                      "data ", cuserount));



            }
            catch (Exception ex)
            {
                await _errorLogService.InsertErrorLog(new BWErrorLog(HttpContext.GetUser().UserId, "Events/alllocations", ex));
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<object>(StatusCodes.Status500InternalServerError, false, ex.Message, null));

            }
        }

        [HttpPost]
        [Route("Eventsaroundme")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> Eventsaroundme([FromForm] string lang, [FromForm] string lat, [FromForm] int pageNumber, [FromForm] int pageSize, [FromForm] string categories)
        {
            try
            {
                var appcon = appConfigrationService.GetData().FirstOrDefault();
                var loggedinUser = HttpContext.GetUser();

                var userDeatils = loggedinUser.User.UserDetails;
                if (!string.IsNullOrWhiteSpace(lang) && !string.IsNullOrWhiteSpace(lat) /*lang != "" && lang != null && lat != "" && lat != null*/)
                {
                    userDeatils.lang = lang;
                    userDeatils.lat = lat;
                    this._userService.UpdateUserDetails(userDeatils);
                }
                var cuserount =
                    _Event.getAlleventUserlocations(pageNumber,pageSize,loggedinUser.User.UserDetails,appcon,categories);


                    //_Event.getAlleventlocation(pageNumber, pageSize, loggedinUser.User.UserDetails.PrimaryId, loggedinUser.User.UserDetails.lat == null ?
                    //(double)0 : Convert.ToDouble(loggedinUser.User.UserDetails.lat), loggedinUser.User.UserDetails.lang == null ? 0 : Convert.ToDouble(loggedinUser.User.UserDetails.lang),
                    //Convert.ToInt32(loggedinUser.User.UserDetails.Manualdistancecontrol == 0 ? 1 : loggedinUser.User.UserDetails.Manualdistancecontrol * 1000),
                    //loggedinUser.User.UserDetails.Gender, loggedinUser.User.UserDetails, appcon, categories);

                return StatusCode(StatusCodes.Status200OK,
                      new ResponseModel<object>(StatusCodes.Status200OK, true,
                      "data ", cuserount));

            }
            catch (Exception ex)
            {
                await _errorLogService.InsertErrorLog(new BWErrorLog(HttpContext.GetUser().UserId, "Events/AllLocationEvente", ex));
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<object>(StatusCodes.Status500InternalServerError, false, ex.Message, null));

            }
        }
        [HttpPost]
        [Route("EventsAroundUser")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> FilterEventsAroundMe([FromForm] string lang, [FromForm] string lat, [FromForm] int pageNumber, [FromForm] int pageSize, [FromForm] string categories  , [FromForm] string dateCriteria, [FromForm] DateTime? startDate, [FromForm]  DateTime? endDate)
        {
            try
            {
                var appConfiguration = appConfigrationService.GetData().FirstOrDefault();
                var loggedInUser = HttpContext.GetUser();

                var userDetails = loggedInUser.User.UserDetails;
                if (!string.IsNullOrWhiteSpace(lang) && !string.IsNullOrWhiteSpace(lat))
                {
                    userDetails.lang = lang;
                    userDetails.lat = lat;
                    _userService.UpdateUserDetails(userDetails);
                }
                var eventsAroundUser =
                    _Event.GetAllEventsUserLocationsWithDateFilter(pageNumber, pageSize, loggedInUser.User.UserDetails, appConfiguration, categories ,dateCriteria,startDate,endDate);


                return StatusCode(StatusCodes.Status200OK,
                      new ResponseModel<object>(StatusCodes.Status200OK, true,
                      "data ", eventsAroundUser));

            }
            catch (Exception ex)
            {
                await _errorLogService.InsertErrorLog(new BWErrorLog(HttpContext.GetUser().UserId, "Events/AllLocationEvente", ex));
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<object>(StatusCodes.Status500InternalServerError, false, ex.Message, null));

            }
        }

        [HttpPost]
        [Route("EventsDataAroundUser")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> FilterEventsDataAroundUser([FromForm] string lang, [FromForm] string lat, [FromForm] int pageNumber, [FromForm] int pageSize, [FromForm] string categories, [FromForm] string dateCriteria, [FromForm] DateTime? startDate, [FromForm] DateTime? endDate)
        {
            try
            {
                var appConfiguration = appConfigrationService.GetData().FirstOrDefault();
                var loggedInUser = HttpContext.GetUser();

                var userDetails = loggedInUser.User.UserDetails;
                if (!string.IsNullOrWhiteSpace(lang) && !string.IsNullOrWhiteSpace(lat))
                {
                    userDetails.lang = lang;
                    userDetails.lat = lat;
                    _userService.UpdateUserDetails(userDetails);
                }
                var eventsAroundUser =
                    _Event.GetEventsLocationsWithDateFilter(pageNumber, pageSize, loggedInUser.User.UserDetails, appConfiguration, categories, dateCriteria, startDate, endDate);


                return StatusCode(StatusCodes.Status200OK,
                    new ResponseModel<object>(StatusCodes.Status200OK, true,
                        "data ", eventsAroundUser));

            }
            catch (Exception ex)
            {
                await _errorLogService.InsertErrorLog(new BWErrorLog(HttpContext.GetUser().UserId, "Events/AllLocationEvente", ex));
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<object>(StatusCodes.Status500InternalServerError, false, ex.Message, null));

            }
        }


        [HttpPost]
        [Route("EventsByLocation")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> FilterEventsByLocation([FromForm] string lang, [FromForm] string lat, [FromForm] string categories, [FromForm] string dateCriteria, [FromForm] DateTime? startDate, [FromForm] DateTime? endDate)
        {
            try
            {
                var appConfiguration = appConfigrationService.GetData().FirstOrDefault();
                var loggedInUser = HttpContext.GetUser();

                var eventsAroundUser =
                    _Event.GetAllEventsByLocationsWithDateFilter(lang, lat, loggedInUser.User.UserDetails, appConfiguration, categories, dateCriteria, startDate, endDate);


                return StatusCode(StatusCodes.Status200OK,
                    new ResponseModel<object>(StatusCodes.Status200OK, true,
                        "data ", eventsAroundUser));

            }
            catch (Exception ex)
            {
                await _errorLogService.InsertErrorLog(new BWErrorLog(HttpContext.GetUser().UserId, "Events/AllLocationEvente", ex));
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<object>(StatusCodes.Status500InternalServerError, false, ex.Message, null));

            }
        }

        [HttpPost]
        [Route("OnlyEventsAround")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> Eventsaround([FromForm] string lang, [FromForm] string lat, [FromForm] int pageNumber
            , [FromForm] int pageSize, [FromForm] string categories)
        {
            try
            {
                var appcon = appConfigrationService.GetData().FirstOrDefault();
                var loggedinUser = HttpContext.GetUser();

                var userDeatils = loggedinUser.User.UserDetails;
                if (lang != "" && lang != null && lat != "" && lat != null)
                {
                    userDeatils.lang = lang;
                    userDeatils.lat = lat;
                    this._userService.UpdateUserDetails(userDeatils);
                }

                (List<EventVM> eventData, int totalRowCount) = await _Event.getAlleventlocation_2(loggedinUser.User.UserDetails.PrimaryId, loggedinUser.User.UserDetails.lat == null ? (double)0 : Convert.ToDouble(loggedinUser.User.UserDetails.lat), loggedinUser.User.UserDetails.lang == null ? 0 : Convert.ToDouble(loggedinUser.User.UserDetails.lang), Convert.ToInt32(loggedinUser.User.UserDetails.Manualdistancecontrol == 0 ? 1 : loggedinUser.User.UserDetails.Manualdistancecontrol * 1000), loggedinUser.User.UserDetails.Gender, loggedinUser.User.UserDetails, appcon, categories, pageNumber, pageSize);

                var validFilter = new PaginationFilter(pageNumber, pageSize);
                var pagedevent = eventData;
                var pagedModel = new PagedResponse<List<EventVM>>(pagedevent, validFilter.PageNumber,
                pagedevent.Count(), eventData.Count());


                var totalPages = ((double)totalRowCount / (double)validFilter.PageSize);
                int roundedTotalPages = Convert.ToInt32(Math.Ceiling(totalPages));

                return StatusCode(StatusCodes.Status200OK,
                      new ResponseModel<object>(StatusCodes.Status200OK, true,
                      "data ", new
                      {
                          pageNumber = pagedModel.PageNumber,
                          pageSize = pagedModel.PageSize,
                          totalPages = roundedTotalPages,//pagedModel.TotalPages,
                          totalRecords = totalRowCount, //pagedModel.TotalRecords,
                          data = pagedModel.Data
                      }));


            }
            catch (Exception ex)
            {
                await _errorLogService.InsertErrorLog(new BWErrorLog(HttpContext.GetUser().UserId, "Events/AllLocationEvente", ex));
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<object>(StatusCodes.Status500InternalServerError, false, ex.Message, null));

            }
        }


        [HttpPost]
        [Route("OnlyEventsAroundUser")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> OnlyEventsAroundUser([FromForm] string lang, [FromForm] string lat, [FromForm] int pageNumber
           , [FromForm] int pageSize, [FromForm] string categories, [FromForm] string dateCriteria, [FromForm] DateTime? startDate, [FromForm] DateTime? endDate)
        {
            try
            {
                var appcon = appConfigrationService.GetData().FirstOrDefault();
                var loggedinUser = HttpContext.GetUser();

                var userDeatils = loggedinUser.User.UserDetails;
                if (lang != "" && lang != null && lat != "" && lat != null)
                {
                    userDeatils.lang = lang;
                    userDeatils.lat = lat;
                    this._userService.UpdateUserDetails(userDeatils);
                }

                (List<EventVM> eventData, int totalRowCount) = await _Event.getAlleventlocationWithDateFilter(loggedinUser.User.UserDetails.PrimaryId, loggedinUser.User.UserDetails.lat == null ? (double)0 : Convert.ToDouble(loggedinUser.User.UserDetails.lat), loggedinUser.User.UserDetails.lang == null ? 0 : Convert.ToDouble(loggedinUser.User.UserDetails.lang), Convert.ToInt32(loggedinUser.User.UserDetails.Manualdistancecontrol == 0 ? 1 : loggedinUser.User.UserDetails.Manualdistancecontrol * 1000), loggedinUser.User.UserDetails.Gender, loggedinUser.User.UserDetails, appcon, categories, pageNumber, pageSize,dateCriteria,startDate,endDate);

                var validFilter = new PaginationFilter(pageNumber, pageSize);
                var pagedevent = eventData;
                var pagedModel = new PagedResponse<List<EventVM>>(pagedevent, validFilter.PageNumber,
                pagedevent.Count(), eventData.Count());


                var totalPages = ((double)totalRowCount / (double)validFilter.PageSize);
                int roundedTotalPages = Convert.ToInt32(Math.Ceiling(totalPages));

                return StatusCode(StatusCodes.Status200OK,
                      new ResponseModel<object>(StatusCodes.Status200OK, true,
                      "data ", new
                      {
                          pageNumber = pagedModel.PageNumber,
                          pageSize = pagedModel.PageSize,
                          totalPages = roundedTotalPages,//pagedModel.TotalPages,
                          totalRecords = totalRowCount, //pagedModel.TotalRecords,
                          data = pagedModel.Data
                      }));


            }
            catch (Exception ex)
            {
                await _errorLogService.InsertErrorLog(new BWErrorLog(HttpContext.GetUser().UserId, "Events/AllLocationEvente", ex));
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<object>(StatusCodes.Status500InternalServerError, false, ex.Message, null));

            }
        }


        [HttpPost]
        [Route("AddEventColor")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> EventColor([FromForm] EventColor model)
        {
            try
            {

                var loggedinUser = HttpContext.GetUser();

                await _Event.InsertEventColor(model);

                return StatusCode(StatusCodes.Status200OK,
                      new ResponseModel<object>(StatusCodes.Status200OK, true,
                      "Your EventColor has been Added successfully !", _Event.getEventColor().Where(m => m.Id == model.Id).FirstOrDefault()));

            }
            catch (Exception ex)
            {
                await _errorLogService.InsertErrorLog(new BWErrorLog(HttpContext.GetUser().UserId, "Events/AddInterests", ex));
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<object>(StatusCodes.Status500InternalServerError, false, ex.Message, null));

            }

        }

        [HttpGet]
        [Route("RecommendedEvent")]
        public async Task<IActionResult> RecommendedEvent([FromQuery] string eventId, [FromQuery] bool? previous)
        {
            try
            {
                var userDetails = HttpContext.GetUser().User.UserDetails;

                var (events, message) = await _Event.RecommendedEvent(userDetails, eventId, previous);

                return StatusCode(StatusCodes.Status200OK, new ResponseModel<RecommendedEventViewModel>(StatusCodes.Status200OK, true, message, events));
            }
            catch (Exception ex)
            {
                await _errorLogService.InsertErrorLog(new BWErrorLog(HttpContext.GetUser().UserId, "Events/RecommendedEvent", ex));
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<RecommendedEventViewModel>(StatusCodes.Status500InternalServerError, false, ex.Message, null));
            }
        }

        [HttpPost]
        [Route("geteventtype")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> geteventtype()
        {
            try
            {
                return StatusCode(StatusCodes.Status200OK,
                      new ResponseModel<object>(StatusCodes.Status200OK, true,
                      "event type", _EventTypeListService.GetData().Where(m => m.ID != 3 && m.ID != 4 && m.ID != 5 && m.ID!=6)));
            }
            catch (Exception ex)
            {
                await _errorLogService.InsertErrorLog(new BWErrorLog(HttpContext.GetUser().UserId, "Events/AddInterests", ex));
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<object>(StatusCodes.Status500InternalServerError, false, ex.Message, null));

            }
        }

    }
}
