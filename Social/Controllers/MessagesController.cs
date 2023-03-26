using CRM.Services.Wrappers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Serilog;
using Social.Entity.DBContext;
using Social.Entity.Enums;
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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
namespace Social.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(AuthorizeUser))]
    public class MessagesController : ControllerBase
    {
        private readonly ILogger logger;
        private readonly IEventServ _Event;
        private readonly AuthDBContext _dBContext;
        private readonly IUserService _userService;
        private readonly IFrindRequest _FrindRequest;
        private readonly IMessageServes MessageServes;
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> userManager;
        private readonly IWebHostEnvironment _environment;
        private readonly IFirebaseManager firebaseManager;
        private readonly IErrorLogService _errorLogService;
        private readonly IChatGroupService chatGroupService;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly IGlobalMethodsService globalMethodsService;
        public MessagesController(IFrindRequest _FrindRequest, IChatGroupService chatGroupService, ILogger logger, IFirebaseManager firebaseManager, IGlobalMethodsService globalMethodsService, IConfiguration configuration, IWebHostEnvironment environment, UserManager<User> userManager, IMessageServes IMessageServes, IStringLocalizer<SharedResource> localizer, IEventServ Event, IUserService userService, IErrorLogService errorLogService, AuthDBContext dBContext)
        {
            _Event = Event;
            this.logger = logger;
            _localizer = localizer;
            _dBContext = dBContext;
            _environment = environment;
            _userService = userService;
            MessageServes = IMessageServes;
            _configuration = configuration;
            this.userManager = userManager;
            this._FrindRequest = _FrindRequest;
            _errorLogService = errorLogService;
            this.firebaseManager = firebaseManager;
            this.chatGroupService = chatGroupService;
            this.globalMethodsService = globalMethodsService;
        }


        [HttpPost]
        [Route("SendMessage")]
        public async Task<IActionResult> SendMessage([FromForm] MessageDTO MessageDTO)
        {
            DateTime dt;
            var ok = DateTime.TryParse(MessageDTO.Messagesdate, out dt);

            if (ok)
            {
                MessageDTO.Messagesdate = MessageDTO.Messagesdate;
            }
            else
            {
                var ds2 = MessageDTO.Messagesdate;
                var dt2 = DateTime.ParseExact(ds2, "dd-MM-yyyy", CultureInfo.CurrentCulture);
                MessageDTO.Messagesdate = dt2.ToString("yyyy-MM-dd");
            }
            try
            {

                string ext = ".jpg,.jpeg,.png,.PNG,.gif,.docx,.doc,.pdf,.PDF,.txt,.text,.Word,.ppt,.pptX,.xlsx,.odt, .xls ,.xps";
                if (MessageDTO.Messagetype == 4 && MessageDTO.EventLINKid == null)
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                  new ResponseModel<object>(StatusCodes.Status406NotAcceptable, true,
                  "Event LINK id required", null));
                }
                if (MessageDTO.Attach != null)
                {
                    foreach (var item in MessageDTO.Attach)
                    {
                        string extention = Path.GetExtension(item.FileName);
                        if (!ext.Contains(extention.ToLower()))
                        {
                            return StatusCode(StatusCodes.Status406NotAcceptable,
                          new ResponseModel<object>(StatusCodes.Status406NotAcceptable, true,
                          "Attach extention Not Acceptable", null));
                        }
                        if (MessageDTO.Messagetype == 2)
                        {
                            ext = ".jpg,.jpeg,.png,.PNG,.gif";
                            if (!ext.Contains(extention.ToLower()))
                            {
                                return StatusCode(StatusCodes.Status406NotAcceptable,
                              new ResponseModel<object>(StatusCodes.Status406NotAcceptable, true,
                              "message extention Not Acceptable", null));
                            }
                        }
                        else if (MessageDTO.Messagetype == 3)
                        {
                            ext = ".docx,.doc,.pdf,.PDF,.txt,.text,.Word,.ppt,.pptX,.xlsx,.odt, .xls ,.xps";
                            if (!ext.Contains(extention.ToLower()))
                            {
                                return StatusCode(StatusCodes.Status406NotAcceptable,
                              new ResponseModel<object>(StatusCodes.Status406NotAcceptable, true,
                              "file extention Not Acceptable", null));
                            }
                        }
                    }
                }
                if (MessageDTO.Messagetype == 5 &&  string.IsNullOrEmpty(MessageDTO.Longitude) && string.IsNullOrEmpty(MessageDTO.Latitude))
                {

                    return StatusCode(StatusCodes.Status406NotAcceptable,
                        new ResponseModel<object>(StatusCodes.Status406NotAcceptable, true,
                            "Location required", null));

                }

                var types = new List<int>
                {
                    1, // message
                    2, // image
                    3  // file
                };
                if (types.Contains(MessageDTO.Messagetype) && MessageDTO.Attach == null  && (MessageDTO.Message == null || MessageDTO.Message.Replace(" ", "") == ""))
                {

                    return StatusCode(StatusCodes.Status406NotAcceptable,
                  new ResponseModel<object>(StatusCodes.Status406NotAcceptable, true,
                  "massage data required", null));

                }


                if (MessageDTO.Messagetype == 4 && (MessageDTO.EventLINKid == null || MessageDTO.EventLINKid == ""))
                {

                    return StatusCode(StatusCodes.Status406NotAcceptable,
                  new ResponseModel<object>(StatusCodes.Status406NotAcceptable, true,
                  "link ID is  required", null));

                }



                var userDeatils = HttpContext.GetUser().User.UserDetails;
                var Deatils = this._userService.GetUserDetails(MessageDTO.UserId);
                Requestes Request = _FrindRequest.GetReque(userDeatils.PrimaryId, Deatils.PrimaryId);
                // Requestes Request2 = _FrindRequest.GetReque(Deatils.PrimaryId, userDeatils.PrimaryId);
                EventData eventdata = null;
                if (MessageDTO.Messagetype == 4)
                {
                    eventdata = _Event.getevent(MessageDTO.EventLINKid).FirstOrDefault();
                    
                    if (eventdata.eventdateto.Value.Date <= DateTime.Now.Date)
                    {
                        if ((eventdata.eventto != null && eventdata.eventdateto.Value.Date == DateTime.Now.Date && eventdata.eventto.Value >= DateTime.Now.TimeOfDay) || (eventdata.eventdateto.Value.Date == DateTime.Now.Date && eventdata.allday.Value == true))
                        {
                        }
                        else
                        {
                            return StatusCode(StatusCodes.Status406NotAcceptable, new ResponseModel<object>(StatusCodes.Status406NotAcceptable, true, "This event has expired", null));
                        }
                    }

                    bool privet = _Event.priveteventlink(MessageDTO.EventLINKid, userDeatils.UserId);
                    if (!privet)
                    {
                        return StatusCode(StatusCodes.Status406NotAcceptable,
                      new ResponseModel<object>(StatusCodes.Status406NotAcceptable, true,
                      "you can not share Private event", null));
                    }

                }
                string message = "You are not a friend with " + Deatils.User.DisplayedUserName;
                if (Request == null && !userDeatils.IsWhiteLabel.Value)
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                          new ResponseModel<object>(StatusCodes.Status406NotAcceptable, true,
                          message, null));
                }
                if (Request != null&&Request.status != 1)
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                          new ResponseModel<object>(StatusCodes.Status406NotAcceptable, true,
                          message, null));
                }
                var Messaghistory = MessageServes.getUserMessages(userDeatils.PrimaryId, Deatils.PrimaryId, true);
                string usermessid = "";
                MessageVIEWDTO MessageVIEWDTO = new MessageVIEWDTO();
                if (Messaghistory != null)
                {
                    usermessid = Messaghistory.Id;
                    //usermessid = MessageServes.getUserMessages(userDeatils.PrimaryId, Deatils.PrimaryId).Id;

                    MessageVIEWDTO = await MessageServes.addusermessage(MessageDTO, userDeatils, usermessid);

                }
                else
                {
                    UserMessages UserMessages = new UserMessages();

                    UserMessages.UserId = userDeatils.PrimaryId;
                    UserMessages.ToUserId = Deatils.PrimaryId;
                    UserMessages.startedin = DateTime.Now.Date;
                    usermessid = Convert.ToString(await MessageServes.addUserMessages(UserMessages));
                    MessageVIEWDTO = await MessageServes.addusermessage(MessageDTO, userDeatils, usermessid);
                }
                var muit = MessageServes.getUserMessages(usermessid, Deatils.UserId);
                var allateend = eventdata== null? new List<EventChatAttend>(): _Event.GetValidChatAttends(eventdata.Id).ToList();

                if (MessageDTO.Messagetype == 4 && eventdata != null)
                {
                    if (eventdata.Shars == null)
                    {
                        eventdata.Shars = 1;
                    }
                    else
                    {
                        eventdata.Shars += 1;
                    }
                    EventTracker eventTracker = new EventTracker()
                    {
                        EventId = eventdata.Id,
                        UserId = userDeatils.PrimaryId,
                        Date = DateTime.Now,
                        ActionType = EventActionType.Share.ToString()
                    };
                    _dBContext.EventTrackers.Add(eventTracker);

                    await _dBContext.SaveChangesAsync();
                }

                FireBaseData fireBaseInfo = new FireBaseData()
                {
                    Messagetype = MessageDTO.Messagetype,
                    date = DateTime.Parse(MessageDTO.Messagesdate).ConvertDateTimeToString(),
                    time = MessageDTO.Messagestime.ToString(@"hh\:mm"),
                    userimage = MessageVIEWDTO.Attach,

                    imageUrl = _configuration["BaseUrl"] + userDeatils.User.UserDetails.UserImage,
                    Title = userDeatils.userName,
                    name = userDeatils.User.DisplayedUserName,
                    Body = MessageDTO.Messagetype == 1 ? MessageDTO.Message : (MessageDTO.Messagetype == 4 ? (eventdata.Title + "(Shared Event)") : ((MessageDTO.Messagetype == 2 ? "photo" : "file"))),
                    muit = muit,
                    Action_code = userDeatils.UserId,
                    Action = "user_chat",

                    messageId = MessageVIEWDTO.Id,
                    senderId = userDeatils.UserId,
                    //============================================================
                    messsageImageURL = MessageVIEWDTO.Attach,

                    senderImage = _configuration["BaseUrl"] + userDeatils.User.UserDetails.UserImage,
                    eventtypeid = MessageDTO.Messagetype == 4 ? eventdata?.EventTypeList.entityID : null,
                    eventtypecolor = MessageDTO.Messagetype == 4 ? eventdata?.EventTypeList.color : null,
                    eventtype = MessageDTO.Messagetype == 4 ? eventdata?.EventTypeList.Name : null,
                    senderDisplayName = userDeatils.User.DisplayedUserName,
                    messsageLinkEveneventdate = MessageDTO.Messagetype == 4 ? eventdata?.eventdate.Value.ConvertDateTimeToString() : null,

                    messsageLinkEveneventdateto = MessageDTO.Messagetype == 4 ? eventdata?.eventdateto.Value.ConvertDateTimeToString() : null,
                    messsageLinkEvenMyEvent = MessageDTO.Messagetype == 4 ? eventdata?.UserId == userDeatils.PrimaryId ? true : false : false,
                    messsageLinkEvencategorie = MessageDTO.Messagetype == 4 ? eventdata?.categorie?.name : null,
                    messsageLinkEvencategorieimage = MessageDTO.Messagetype == 4 ? _configuration["BaseUrl"] + eventdata?.categorie?.image : null,

                    messsageLinkEvenTitle = MessageDTO.Messagetype == 4 ? eventdata?.Title : null,
                    //TODO: Abdelrahman (Fix Image Url)
                    // messsageLinkEvenImage = _configuration["BaseUrl"] + eventdata?.image,
                    messsageLinkEvenImage = MessageDTO.Messagetype == 4 ? (eventdata.EventTypeListid != 3 ? _configuration["BaseUrl"] : "") + eventdata?.image : null,
                    messsageLinkEvenId = MessageDTO.Messagetype == 4 ? eventdata?.EntityId : null,
                    // EncryptedID = StringCipher.EncryptString(item.EventData.EntityId),
                    messsageLinkEvenkey = MessageDTO.Messagetype == 4 ? eventdata?.UserId == userDeatils.PrimaryId ? 1 : (_Event.GetEventattend(allateend, eventdata?.EntityId, userDeatils.PrimaryId).type == true ? 2 : 3) : 0,
                    //lang = item.EventData.lang,
                    //lat = item.EventData.lat,
                    messsageLinkEventotalnumbert = MessageDTO.Messagetype == 4 ? eventdata != null ? eventdata.totalnumbert : 0 : 0,

                    messsageLinkEvenjoined = MessageDTO.Messagetype == 4 ? _Event.GetEventattend(allateend, eventdata?.EntityId, userDeatils.PrimaryId).count : 0,

                };
                await firebaseManager.SendNotification(Deatils?.FcmToken, fireBaseInfo);

                return StatusCode(StatusCodes.Status200OK,
                      new ResponseModel<object>(StatusCodes.Status200OK, true,
                      "Sended successfully", MessageVIEWDTO));

            }
            catch (Exception ex)
            {
                await _errorLogService.InsertErrorLog(new BWErrorLog(HttpContext.GetUser().UserId, "Messages/SendMessage", ex));
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<object>(StatusCodes.Status500InternalServerError, false, ex.Message, null));

            }
        }

        [HttpPost]
        [Route("SendMessageFix")]
        public async Task<IActionResult> SendMessageFix([FromForm] MessageDTO MessageDTO)
        {
            string ext = ".jpg,.jpeg,.png,.PNG,.gif,.docx,.doc,.pdf,.PDF,.txt,.text,.Word,.ppt,.pptX,.xlsx,.odt, .xls ,.xps";
            if (MessageDTO.Messagetype == 4 && string.IsNullOrWhiteSpace(MessageDTO.EventLINKid))
            {
                return StatusCode(StatusCodes.Status406NotAcceptable,
                  new ResponseModel<object>(StatusCodes.Status406NotAcceptable, true,
                  "Event LINK id required", null));
            }
            if (MessageDTO.Messagetype != 4 && MessageDTO.Attach == null && string.IsNullOrWhiteSpace(MessageDTO.Message))
            {
                return StatusCode(StatusCodes.Status406NotAcceptable,
                  new ResponseModel<object>(StatusCodes.Status406NotAcceptable, true,
                  "massage data required", null));
            }
            try
            {                
                
                if (MessageDTO.Attach != null)
                {
                    foreach (var item in MessageDTO.Attach)
                    {
                        string extention = Path.GetExtension(item.FileName);
                        if (!ext.Contains(extention.ToLower()))
                        {
                            return StatusCode(StatusCodes.Status406NotAcceptable,
                          new ResponseModel<object>(StatusCodes.Status406NotAcceptable, true,
                          "Attach extention Not Acceptable", null));
                        }
                        if (MessageDTO.Messagetype == 2)
                        {
                            ext = ".jpg,.jpeg,.png,.PNG,.gif";
                            if (!ext.Contains(extention.ToLower()))
                            {
                                return StatusCode(StatusCodes.Status406NotAcceptable,
                              new ResponseModel<object>(StatusCodes.Status406NotAcceptable, true,
                              "message extention Not Acceptable", null));
                            }
                        }
                        else if (MessageDTO.Messagetype == 3)
                        {
                            ext = ".docx,.doc,.pdf,.PDF,.txt,.text,.Word,.ppt,.pptX,.xlsx,.odt, .xls ,.xps";
                            if (!ext.Contains(extention.ToLower()))
                            {
                                return StatusCode(StatusCodes.Status406NotAcceptable,
                              new ResponseModel<object>(StatusCodes.Status406NotAcceptable, true,
                              "file extention Not Acceptable", null));
                            }
                        }
                    }
                }

                var userDeatils = HttpContext.GetUser().User.UserDetails;
                var Deatils = this._userService.GetUserDetails(MessageDTO.UserId);
                Requestes Request = _FrindRequest.GetReque(userDeatils.PrimaryId, Deatils.PrimaryId);                
                EventData eventdata = null;
               
                string message = "You are not a friend with " + Deatils.User.DisplayedUserName;
                if (Request == null)
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                          new ResponseModel<object>(StatusCodes.Status406NotAcceptable, true,
                          message, null));
                }
                else if (Request.status != 1)
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                          new ResponseModel<object>(StatusCodes.Status406NotAcceptable, true,
                          message, null));
                }
                var Messaghistory = MessageServes.getUserMessages(userDeatils.PrimaryId, Deatils.PrimaryId, true);
                string usermessid = "";
                MessageVIEWDTO MessageVIEWDTO = new MessageVIEWDTO();
                if (Messaghistory != null)
                {
                    usermessid = Messaghistory.Id;
                    //usermessid = MessageServes.getUserMessages(userDeatils.PrimaryId, Deatils.PrimaryId).Id;

                    MessageVIEWDTO = await MessageServes.addusermessage(MessageDTO, userDeatils, usermessid);

                }
                else
                {
                    UserMessages UserMessages = new UserMessages();

                    UserMessages.UserId = userDeatils.PrimaryId;
                    UserMessages.ToUserId = Deatils.PrimaryId;
                    UserMessages.startedin = DateTime.Now.Date;
                    usermessid = Convert.ToString(await MessageServes.addUserMessages(UserMessages));
                    MessageVIEWDTO = await MessageServes.addusermessage(MessageDTO, userDeatils, usermessid);
                }
                if (MessageDTO.Messagetype == 4)
                {
                    eventdata = _Event.getevent(MessageDTO.EventLINKid).FirstOrDefault();

                    if (eventdata.eventdateto.Value.Date <= DateTime.Now.Date)
                    {
                        if ((eventdata.eventto != null && eventdata.eventdateto.Value.Date == DateTime.Now.Date && eventdata.eventto.Value >= DateTime.Now.TimeOfDay) 
                            || (eventdata.eventdateto.Value.Date == DateTime.Now.Date && eventdata.allday.Value == true))
                        {
                        }
                        else
                        {
                            return StatusCode(StatusCodes.Status406NotAcceptable, new ResponseModel<object>(StatusCodes.Status406NotAcceptable, true, "This event has expired", null));
                        }
                    }

                    bool privet = _Event.priveteventlink(MessageDTO.EventLINKid, userDeatils.UserId);
                    if (!privet)
                    {
                        return StatusCode(StatusCodes.Status406NotAcceptable,
                      new ResponseModel<object>(StatusCodes.Status406NotAcceptable, true,
                      "you can not share Private event", null));
                    }
                    if (eventdata != null)
                    {
                        if (eventdata.Shars == null)
                        {
                            eventdata.Shars = 1;
                        }
                        else
                        {
                            eventdata.Shars += 1;
                        }
                        EventTracker eventTracker = new EventTracker()
                        {
                            EventId = eventdata.Id,
                            UserId = userDeatils.PrimaryId,
                            Date = DateTime.Now,
                            ActionType = EventActionType.Share.ToString()
                        };
                        _dBContext.EventTrackers.Add(eventTracker);

                        await _dBContext.SaveChangesAsync();
                    }

                }

                var muit = MessageServes.getUserMessages(usermessid, Deatils.UserId);
                var allateend = eventdata == null ? new List<EventChatAttend>(): _Event.GetValidChatAttends(eventdata.Id).ToList();

               

                FireBaseData fireBaseInfo = new FireBaseData()
                {
                    Messagetype = MessageDTO.Messagetype,
                    date = DateTime.Parse(MessageDTO.Messagesdate).ConvertDateTimeToString(),
                    time = MessageDTO.Messagestime.ToString(@"hh\:mm"),
                    userimage = MessageVIEWDTO.Attach,

                    imageUrl = _configuration["BaseUrl"] + userDeatils.User.UserDetails.UserImage,
                    Title = userDeatils.userName,
                    name = userDeatils.User.DisplayedUserName,
                    Body = MessageDTO.Messagetype == 1 ? MessageDTO.Message : (MessageDTO.Messagetype == 4 ? (eventdata.Title + "(Shared Event)") : ((MessageDTO.Messagetype == 2 ? "photo" : "file"))),
                    muit = muit,
                    Action_code = userDeatils.UserId,
                    Action = "user_chat",

                    messageId = MessageVIEWDTO.Id,
                    senderId = userDeatils.UserId,
                    //============================================================
                    messsageImageURL = MessageVIEWDTO.Attach,

                    senderImage = _configuration["BaseUrl"] + userDeatils.User.UserDetails.UserImage,
                    eventtypeid = MessageDTO.Messagetype == 4 ? eventdata?.EventTypeList.entityID : null,
                    eventtypecolor = MessageDTO.Messagetype == 4 ? eventdata?.EventTypeList.color : null,
                    eventtype = MessageDTO.Messagetype == 4 ? eventdata?.EventTypeList.Name : null,
                    senderDisplayName = userDeatils.User.DisplayedUserName,
                    messsageLinkEveneventdate = MessageDTO.Messagetype == 4 ? eventdata?.eventdate.Value.ConvertDateTimeToString() : null,

                    messsageLinkEveneventdateto = MessageDTO.Messagetype == 4 ? eventdata?.eventdateto.Value.ConvertDateTimeToString() : null,
                    messsageLinkEvenMyEvent = MessageDTO.Messagetype == 4 ? eventdata?.UserId == userDeatils.PrimaryId ? true : false : false,
                    messsageLinkEvencategorie = MessageDTO.Messagetype == 4 ? eventdata?.categorie?.name : null,
                    messsageLinkEvencategorieimage = MessageDTO.Messagetype == 4 ? _configuration["BaseUrl"] + eventdata?.categorie?.image : null,

                    messsageLinkEvenTitle = MessageDTO.Messagetype == 4 ? eventdata?.Title : null,
                    //TODO: Abdelrahman (Fix Image Url)
                    // messsageLinkEvenImage = _configuration["BaseUrl"] + eventdata?.image,
                    messsageLinkEvenImage = MessageDTO.Messagetype == 4 ? (eventdata.EventTypeListid != 3 ? _configuration["BaseUrl"] : "") + eventdata?.image : null,
                    messsageLinkEvenId = MessageDTO.Messagetype == 4 ? eventdata?.EntityId : null,
                    // EncryptedID = StringCipher.EncryptString(item.EventData.EntityId),
                    messsageLinkEvenkey = MessageDTO.Messagetype == 4 ? eventdata?.UserId == userDeatils.PrimaryId ? 1 : (_Event.GetEventattend(allateend, eventdata?.EntityId, userDeatils.PrimaryId).type == true ? 2 : 3) : 0,
                    //lang = item.EventData.lang,
                    //lat = item.EventData.lat,
                    messsageLinkEventotalnumbert = MessageDTO.Messagetype == 4 ? eventdata != null ? eventdata.totalnumbert : 0 : 0,

                    messsageLinkEvenjoined = MessageDTO.Messagetype == 4 ? _Event.GetEventattend(allateend, eventdata?.EntityId, userDeatils.PrimaryId).count : 0,

                };
                await firebaseManager.SendNotification(Deatils?.FcmToken, fireBaseInfo);

                return StatusCode(StatusCodes.Status200OK,
                      new ResponseModel<object>(StatusCodes.Status200OK, true,
                      "Sended successfully", MessageVIEWDTO));

            }
            catch (Exception ex)
            {
                await _errorLogService.InsertErrorLog(new BWErrorLog(HttpContext.GetUser().UserId, "Messages/SendMessage", ex));
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<object>(StatusCodes.Status500InternalServerError, false, ex.Message, null));

            }
        }
        [HttpPost]
        [Route("SendChatGroupMessage")]
        public async Task<IActionResult> SendChatGroupMessage([FromForm] ChatGroupSendMessageVM model)
        {
            var userDeatils = HttpContext.GetUser().User.UserDetails;
            if ((int)model.MessageType == 4)
            {
                bool privet = _Event.priveteventlink(model.EventLINKid);
                if (privet)
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                  new ResponseModel<object>(StatusCodes.Status406NotAcceptable, true,
                  "you can not share Private event", null));
                }

            }
            var Result = await chatGroupService.SendMessage(HttpContext.GetUser().User, model);
            return StatusCode(Result.StatusCode, Result);
        }


        [HttpPost]
        [Route("Sendnotifacation")]
        public async Task<IActionResult> Sendnotifacation([FromForm] string Title, [FromForm] string Body, [FromForm] string userid, [FromForm] IFormFile imageUrl)
        {
            try
            {
                var userDeatils = HttpContext.GetUser().User.UserDetails;

                var usernotif = this._userService.GetUserDetails(userid);
                string ext = ".jpg,.jpeg,.png,.PNG,.gif,.docx,.doc,.pdf,.PDF,.txt,.text,.Word,.ppt,.pptX,.xlsx,.odt, .xls ,.xps";
                if (imageUrl != null)
                {

                    {
                        string extention = Path.GetExtension(imageUrl.FileName);
                        if (!ext.Contains(extention.ToLower()))
                        {
                            return StatusCode(StatusCodes.Status406NotAcceptable,
                          new ResponseModel<object>(StatusCodes.Status406NotAcceptable, true,
                          "image extention Not Acceptable", null));
                        }
                    }
                }

                string imageName = null;

                if (imageUrl != null)
                {
                    var UniqName = await globalMethodsService.uploadFileAsync("/Images/", imageUrl);

                    imageName = "/Images/" + UniqName;

                }
                FireBaseData fireBaseInfo = new FireBaseData() { muit = false, Title = Title, imageUrl = _configuration["BaseUrl"] + (imageName == null ? "/Images/Logo.jpg" : imageName), Body = Body, Action_code = userDeatils.UserId, Action = "Notificationcs" };

                SendNotificationcs sendNotificationcs = new SendNotificationcs();

                try
                {
                    if (usernotif.FcmToken != null)
                    {
                        await firebaseManager.SendNotification(usernotif.FcmToken, fireBaseInfo);
                        //await sendNotificationcs.SendMessageAsync(usernotif.FcmToken, "Notificationcs", fireBaseInfo, _environment.WebRootPath);
                    }
                }
                catch { }
                var addnoti = MessageServes.getFireBaseData(usernotif.PrimaryId, fireBaseInfo);
                await MessageServes.addFireBaseDatamodel(addnoti);

                return StatusCode(StatusCodes.Status200OK,
                      new ResponseModel<object>(StatusCodes.Status200OK, true,
                      "Sended successfully", null));
            }
            catch (Exception ex)
            {
                await _errorLogService.InsertErrorLog(new BWErrorLog(HttpContext.GetUser().UserId, "Messages/Sendnotifacation", ex));
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<object>(StatusCodes.Status500InternalServerError, false, ex.Message, null));

            }
        }


        [HttpPost]
        [Route("SendEventMessage")]
        public async Task<IActionResult> SendEventMessage([FromForm] EventMessageDTO MessageDTO)
        {
            try
            {
                if (MessageDTO.Messagetype == 4 && MessageDTO.EventLINKid == null)
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                  new ResponseModel<object>(StatusCodes.Status406NotAcceptable, true,
                  "Event LINK id required", null));
                }
                string ext = ".jpg,.jpeg,.png,.PNG,.gif,.docx,.doc,.pdf,.PDF,.txt,.text,.Word,.ppt,.pptX,.xlsx,.odt, .xls ,.xps";
                if (MessageDTO.Attach != null)
                {
                    foreach (var item in MessageDTO.Attach)
                    {
                        string extention = Path.GetExtension(item.FileName);
                        if (!ext.Contains(extention.ToLower()))
                        {
                            return StatusCode(StatusCodes.Status406NotAcceptable,
                          new ResponseModel<object>(StatusCodes.Status406NotAcceptable, true,
                          "Attach extention Not Acceptable", null));
                        }
                        if (MessageDTO.Messagetype == 2)
                        {
                            ext = ".jpg,.jpeg,.png,.PNG,.gif";
                            if (!ext.Contains(extention.ToLower()))
                            {
                                return StatusCode(StatusCodes.Status406NotAcceptable,
                              new ResponseModel<object>(StatusCodes.Status406NotAcceptable, true,
                              "message extention Not Acceptable", null));
                            }
                        }
                        else if (MessageDTO.Messagetype == 3)
                        {
                            ext = ".docx,.doc,.pdf,.PDF,.txt,.text,.Word,.ppt,.pptX,.xlsx,.odt, .xls ,.xps";
                            if (!ext.Contains(extention.ToLower()))
                            {
                                return StatusCode(StatusCodes.Status406NotAcceptable,
                              new ResponseModel<object>(StatusCodes.Status406NotAcceptable, true,
                              "file extention Not Acceptable", null));
                            }
                        }
                    }
                }
                var types = new List<int>
                {
                    1, // message
                    2, // image
                    3  // file
                };
                if (types.Contains(MessageDTO.Messagetype) && MessageDTO.Attach == null && (MessageDTO.Message == null || MessageDTO.Message.Replace(" ", "") == ""))
                {

                    return StatusCode(StatusCodes.Status406NotAcceptable,
                  new ResponseModel<object>(StatusCodes.Status406NotAcceptable, true,
                  "massage data required", null));

                }
                if (MessageDTO.Messagetype == 5 && string.IsNullOrEmpty(MessageDTO.Longitude) && string.IsNullOrEmpty(MessageDTO.Latitude))
                {

                    return StatusCode(StatusCodes.Status406NotAcceptable,
                        new ResponseModel<object>(StatusCodes.Status406NotAcceptable, true,
                            "Location required", null));

                }
                if (MessageDTO.Messagetype == 4 && (MessageDTO.EventLINKid == null || MessageDTO.EventLINKid == ""))
                {

                    return StatusCode(StatusCodes.Status406NotAcceptable,
                  new ResponseModel<object>(StatusCodes.Status406NotAcceptable, true,
                  "link ID is  required", null));

                }
                var userDeatils = HttpContext.GetUser().User.UserDetails;
                if (MessageDTO.Messagetype == 4)
                {
                    bool privet = _Event.priveteventlink(MessageDTO.EventLINKid);
                    if (privet)
                    {
                        return StatusCode(StatusCodes.Status406NotAcceptable,
                      new ResponseModel<object>(StatusCodes.Status406NotAcceptable, true,
                      "you can not share Private event", null));
                    }

                }
                var cuserount = _Event.GetEventChatAttend(MessageDTO.EventId, userDeatils.UserId);
                if (cuserount != null)
                {

                    if (cuserount.removefromevent == true)
                    {
                        return StatusCode(StatusCodes.Status406NotAcceptable,
                             new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,
                             "You are deleted From Event ", null));
                    }
                    if (cuserount.leavechat == true)
                    {
                        return StatusCode(StatusCodes.Status406NotAcceptable,
                             new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,
                             "You Left Event Chat ", null));
                    }
                    if (cuserount.leave == true)
                    {
                        return StatusCode(StatusCodes.Status406NotAcceptable,
                             new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,
                             "You Left Event  ", null));
                    }
                    if (cuserount.EventData.IsActive != true && (cuserount.EventData.StopFrom != null ? (cuserount.EventData.StopFrom.Value >= DateTime.Now.Date || cuserount.EventData.StopTo.Value <= DateTime.Now.Date) : true))
                    {
                        return StatusCode(StatusCodes.Status406NotAcceptable,
                             new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,
                             "This event is not active now", null));
                    }
                }
                else
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                                                 new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,
                                                 "You are not subscribed ", null));
                }
                //var usermessdata = MessageServes.getEventChat(MessageDTO.EventId);
                //string usermessid = usermessdata.Id;
                var attend = _Event.GetEventChatAttend(MessageDTO.EventId, userDeatils.UserId);
                MessageDTO.EventChatAttendid = attend.Id;
                MessageVIEWDTO MessageVIEWDTO = await MessageServes.addeventmessage(MessageDTO, userDeatils);
                var Eventattends = _Event.getallattendevent(MessageDTO.EventId).ToList();
                var listuser = this._userService.GetUserDetails();
                var eventdata = _Event.getevent(MessageDTO.EventId).FirstOrDefault();
                var allateend = _Event.GetValidChatAttends(eventdata.Id).ToList();
                foreach (var even in Eventattends)
                {
                    if (even.UserattendId != userDeatils.PrimaryId)
                    {
                        try
                        {
                            //var userto = this._userService.GetUserDetails(even.Userattend.UserId);
                            var userto = listuser.FirstOrDefault(c => c.UserId == even.Userattend.UserId);
                            FireBaseData fireBaseInfo = new FireBaseData()
                            {
                                Title = userDeatils.userName + "@" + even.EventData.Title,
                                date = MessageDTO.Messagesdate.ConvertDateTimeToString(),
                                time = MessageDTO.Messagestime.ToString(@"hh\:mm"),
                                userimage = MessageVIEWDTO.Attach,
                                name = attend.EventData.Title,
                                //TODO:Abdelrahman (Fix Url Image)
                                //imageUrl = _configuration["BaseUrl"] + attend.EventData.image,
                                imageUrl = (attend.EventData.EventTypeListid != 3 ? _configuration["BaseUrl"] : "") + attend.EventData.image,
                                //Body = MessageDTO.Message + MessageDTO.Messagetype == "4" ? "Shared Event" : ((MessageDTO.Attach != null ? _configuration["BaseUrl"] + MessageVIEWDTO.Attach : "")),
                                // Body = MessageDTO.Message + (MessageDTO.Messagetype == 4 ? "Shared Event" : ((MessageDTO.Attach != null ? _configuration["BaseUrl"] + MessageVIEWDTO.Attach : ""))),

                                Body = MessageDTO.Messagetype == 1 ? MessageDTO.Message : (MessageDTO.Messagetype == 4 ? (eventdata.Title + "(Shared Event)") : ((MessageDTO.Messagetype == 2 ? "photo" : "file"))),

                                muit = even.muit,
                                isAdmin = even.EventData.UserId == userto.PrimaryId,
                                Messagetype = MessageDTO.Messagetype,
                                Action_code = MessageDTO.EventId,
                                Action = "event_chat",

                                messageId = MessageVIEWDTO.Id,
                                IsWhitelabel= userDeatils.IsWhiteLabel.HasValue? userDeatils.IsWhiteLabel.Value : false,
                                senderId = userDeatils.UserId,
                                //=====================================================================
                                messsageImageURL = MessageVIEWDTO.Attach,

                                senderImage = _configuration["BaseUrl"] + userDeatils.User.UserDetails.UserImage,

                                senderDisplayName = userDeatils.User.DisplayedUserName,
                                messsageLinkEveneventdate = eventdata?.eventdate.Value.ConvertDateTimeToString(),

                                messsageLinkEveneventdateto = eventdata?.eventdateto.Value.ConvertDateTimeToString(),
                                messsageLinkEvenMyEvent = eventdata?.UserId == userDeatils.PrimaryId ? true : false,
                                messsageLinkEvencategorie = eventdata?.categorie?.name,
                                messsageLinkEvencategorieimage = _configuration["BaseUrl"] + eventdata?.categorie?.image,
                                //categorieId = item.EventData.categorie.EntityId,
                                messsageLinkEvenTitle = eventdata?.Title,
                                eventtypeid = eventdata?.EventTypeList.entityID,
                                eventtypecolor = eventdata?.EventTypeList.color,
                                eventtype = eventdata?.EventTypeList.Name,
                                //TODO:Abdelrahman (Fix Url Image)
                                //messsageLinkEvenImage =  _configuration["BaseUrl"] + eventdata?.image,
                                messsageLinkEvenImage = (eventdata.EventTypeListid != 3 ? _configuration["BaseUrl"] : "") + eventdata?.image,
                                messsageLinkEvenId = eventdata?.EntityId,
                                // EncryptedID = StringCipher.EncryptString(item.EventData.EntityId),
                                messsageLinkEvenkey = eventdata?.UserId == userDeatils.PrimaryId ? 1 : (_Event.GetEventattend(allateend, eventdata?.EntityId, userDeatils.PrimaryId).type == true ? 2 : 3),
                                //lang = item.EventData.lang,
                                //lat = item.EventData.lat,
                                messsageLinkEventotalnumbert = eventdata != null ? eventdata.totalnumbert : 0,
                                //EntityId = item.EventData.categorie.EntityId,
                                //interests = _Event.GetINterestdata(m.Id).Distinct(),
                                messsageLinkEvenjoined = _Event.GetEventattend(allateend, eventdata?.EntityId, userDeatils.PrimaryId).count,
                            };
                            SendNotificationcs sendNotificationcs = new SendNotificationcs();
                            try
                            {
                                if (userto.FcmToken != null)
                                    //to fix duplicate notifications
                                    // await firebaseManager.SendNotification(userto.FcmToken, fireBaseInfo);
                                    await sendNotificationcs.SendMessageAsync(userto.FcmToken, "event_chat", fireBaseInfo, _environment.WebRootPath);
                            }
                            catch (Exception ex)
                            {
                               //return StatusCode(StatusCodes.Status500InternalServerError,
                               //   new ResponseModel<object>(StatusCodes.Status500InternalServerError, true,
                               //   "Failed to send the messages", ex));
                            }
                            var addnoti = MessageServes.getFireBaseData(userto.PrimaryId, fireBaseInfo);
                            await MessageServes.addFireBaseDatamodel(addnoti);
                        }
                        catch (Exception ex)
                        {
                            return StatusCode(StatusCodes.Status500InternalServerError,
                                   new ResponseModel<object>(StatusCodes.Status500InternalServerError, true,
                                   "Failed to send the messages", ex));
                        }
                    }
                }

                return StatusCode(StatusCodes.Status200OK,
                      new ResponseModel<object>(StatusCodes.Status200OK, true,
                      "Sended successfully", MessageVIEWDTO));

            }
            catch (Exception ex)
            {
                await _errorLogService.InsertErrorLog(new BWErrorLog(HttpContext.GetUser().UserId, "Messages/SendMessage", ex));
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<object>(StatusCodes.Status500InternalServerError, false, ex.Message, null));

            }
        }


        [HttpPost]
        [Route("UsersinChat")]
        public async Task<IActionResult> UsersChat([FromForm] int pageNumber, [FromForm] int pageSize, [FromForm] string Search)
        {
            try
            {

                var userDeatils = HttpContext.GetUser().User.UserDetails;               

                List<int> currentUserInterests = userDeatils.listoftags.Select(q => q.InterestsId).ToList();

                var validFilter = new PaginationFilter(pageNumber, pageSize);

                // var users =await MessageServes.getalllUserinconect(userDeatils.PrimaryId, userDeatils.UserId);
                var users = await MessageServes.getalllUserinconect(userDeatils.PrimaryId, userDeatils.UserId, Search);

                var pagedevent = users.Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                   .Take(validFilter.PageSize).OrderByDescending(x => x.latestdatevm.Date).ThenByDescending(x => x.latesttimevm).ToList();
                var pagedModel = new PagedResponse<List<UserDetailsvm>>(pagedevent, validFilter.PageNumber,
               pagedevent.Count(), users.Count());
                return StatusCode(StatusCodes.Status200OK,
                      new ResponseModel<object>(StatusCodes.Status200OK, true,
                      "Friends Chat", new
                      {
                          pageNumber = pagedModel.PageNumber,
                          pageSize = pagedModel.PageSize,
                          totalPages = pagedModel.TotalPages,
                          totalRecords = pagedModel.TotalRecords,

                          data = pagedModel.Data.Select(m => new
                          {
                              m.myevent,
                              m.Name,
                              m.id,
                              m.isCommunityGroup,
                              m.muit,
                              m.isChatGroup,
                              m.IsChatGroupAdmin,
                              m.LeaveGroup,
                              image = (m.eventtypeintid != 3 ? _configuration["BaseUrl"] : "") + m.Image,
                              m.messagestype,
                              messagesattach = m.messagesimage == "" ? "" : _configuration["BaseUrl"] + m.messagesimage,
                              m.isfrind,
                              m.ImageIsVerified,
                              m.IsWhiteLabel,
                              m.Leavevent,
                              m.Leaveventchat,
                              m.isevent,
                              m.isactive,
                              m.latestdate,
                              m.message_not_Read,
                              latesttime = DateTime.Parse(m.latesttime).ToString(@"HH\:mm"),
                              m.messages,
                              m.eventtype,
                              m.eventtypecolor,
                              m.eventtypeid,                            
                              //InterestMatchPercent = m.User.PrimaryId == userDeatils.PrimaryId ?
                              //Math.Round(((m.UserRequest.listoftags.Select(q => q.InterestsId).Intersect(currentUserInterests).Count() / 8m) * 100), 0) :
                              //Math.Round(((m.User.listoftags.Select(q => q.InterestsId).Intersect(currentUserInterests).Count() / 8m) * 100), 0)
                          })
                      }));

            }
            catch (Exception ex)
            {
                await _errorLogService.InsertErrorLog(new BWErrorLog(HttpContext.GetUser().UserId, "Messages/UsersChat", ex));
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<object>(StatusCodes.Status500InternalServerError, false, ex.Message, null));
            }
        }


        [HttpPost]
        [Route("SearshUsersinChat")]
        public async Task<IActionResult> SearshUsersinChat([FromForm] int pageNumber, [FromForm] int pageSize, [FromForm] string username)
        {
            try
            {

                var userDeatils = HttpContext.GetUser().User.UserDetails;


                var validFilter = new PaginationFilter(pageNumber, pageSize);


                var users = await MessageServes.getalllUserinconect(userDeatils.PrimaryId, userDeatils.UserId, username);

                var pagedevent = users.Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                   .Take(validFilter.PageSize).ToList();
                var pagedModel = new PagedResponse<List<UserDetailsvm>>(pagedevent, validFilter.PageNumber,
               pagedevent.Count(), users.Count());
                logger.Information($"User inchat End At {DateTime.Now.ToString()}");

                return StatusCode(StatusCodes.Status200OK,
                      new ResponseModel<object>(StatusCodes.Status200OK, true,
                      "Friends Chat", new
                      {
                          pageNumber = pagedModel.PageNumber,
                          pageSize = pagedModel.PageSize,
                          totalPages = pagedModel.TotalPages,
                          totalRecords = pagedModel.TotalRecords,

                          data = pagedModel.Data.Select(m => new
                          {
                              m.myevent,
                              m.Name,
                              m.id,
                              m.muit,
                              m.isChatGroup,
                              m.IsChatGroupAdmin,
                              m.LeaveGroup,
                              image = _configuration["BaseUrl"] + m.Image,
                              m.messagestype,
                              messagesattach = m.messagesimage == "" ? "" : _configuration["BaseUrl"] + m.messagesimage,
                              m.isfrind,
                              m.ImageIsVerified,
                              m.Leavevent,
                              m.isevent,
                              m.latestdate,
                              m.latesttime,
                              m.messages,
                          })
                      }));

            }
            catch (Exception ex)
            {
                await _errorLogService.InsertErrorLog(new BWErrorLog(HttpContext.GetUser().UserId, "Messages/UsersChat", ex));
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<object>(StatusCodes.Status500InternalServerError, false, ex.Message, null));
            }
            //try
            //{
            //    var userDeatils = HttpContext.GetUser().User.UserDetails;
            //    var users =await MessageServes.getalllUserinconect(userDeatils.PrimaryId, userDeatils.UserId);
            //    var pagedModel = users.Where(b => b.Name.ToLower().Contains(username.ToLower())).ToList();
            //    return StatusCode(StatusCodes.Status200OK,
            //          new ResponseModel<object>(StatusCodes.Status200OK, true,
            //          "Friends Chat", new
            //          {
            //              data = pagedModel.Select(m => new
            //              { m.Name, m.id, m.muit, image = _configuration["BaseUrl"] + m.Image, m.isevent, m.latestdate, m.latesttime, m.messages })
            //          }));

            //}
            //catch (Exception ex)
            //{
            //    await _errorLogService.InsertErrorLog(new BWErrorLog(HttpContext.GetUser().UserId, "Messages/UsersChat", ex));
            //    return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<object>(StatusCodes.Status500InternalServerError, false, ex.Message, null));
            //}
        }


        [HttpPost]
        [Route("Chatdata")]
        public async Task<IActionResult> Chatdata([FromForm] int pageNumber, [FromForm] int pageSize, [FromForm] string userid)
        {
            try
            {
                var CurrentUser = HttpContext.GetUser().User;
                var CurrentUserPrimaryId = CurrentUser.UserDetails.PrimaryId;
                var meDeatils = _userService.GetUserDetails(userid);
                var validFilter = new PaginationFilter(pageNumber, pageSize);
                var MessagesData = MessageServes.GetUserMessageData(CurrentUser?.Id, userid, validFilter);
                var allateend = _Event.allattendevent().ToList();
                var allReq = _dBContext.Requestes.ToList();
                var key = _FrindRequest.Getallkey(CurrentUserPrimaryId, meDeatils.PrimaryId, allReq);
                var messages = MessagesData.messagedatas.Select(item => new MessagedataVM
                {
                    Messages = item.Messages,
                    Latitude = item.Latitude,
                    Longitude = item.Longitude,
                    Messagesdate = item.Messagesdate.ConvertDateTimeToString(),
                    Messagestime = item.Messagestime.ToString(@"hh\:mm"),
                    Username = item.User.User.DisplayedUserName,
                    DisplayedUserName = item.User.User.UserName,
                    UserId = item.User.UserId,
                    Id = item.Id,
                    Messagetype = item.Messagetype,
                    Userimage = _configuration["BaseUrl"] + item.User.UserImage,
                    currentuserMessage = item.UserId == CurrentUserPrimaryId,
                    linkable = item.linkable,
                    EventLINKid = item.Messagetype == 4 ? item.EventData.EntityId : null,
                    Key = key,
                    IsWhitelabel=meDeatils.IsWhiteLabel.HasValue? meDeatils.IsWhiteLabel.Value:false,
                    EventData = (item.Messagetype == 4 && item.EventData.eventdateto.Value >= DateTime.Now) ? (new EventDatalinka
                    {
                        Id = item.EventData.EntityId,
                        Title = item.EventData.Title,
                        //TODO:Abdelrahman (Fix Url Image)
                        // Image = _configuration ["BaseUrl"] + item.EventData.image,
                        Image = (item.EventData.EventTypeListid != 3 ? _configuration["BaseUrl"] : "") + item.EventData.image,
                        categorie = item.EventData.categorie.name,
                        //categorieimage = _configuration ["BaseUrl"] + item.EventData.categorie.image,
                        joined = _Event.GetEventattend(allateend, item.EventData.EntityId, CurrentUser.UserDetails.PrimaryId).count,
                        totalnumbert = item.EventData.totalnumbert,

                        key = item.UserId == CurrentUser.UserDetails.PrimaryId ? 1 : (_Event.GetEventattend(allateend, item.EventData.EntityId, CurrentUser.UserDetails.PrimaryId).type == true ? 2 : 3),
                        eventtypeid = item.EventData.EventTypeList.entityID,
                        eventtypecolor = item.EventData.EventTypeList.color,
                        eventtype = item.EventData.EventTypeList.Name,
                        MyEvent = item.EventData.UserId == CurrentUser.UserDetails.PrimaryId ? true : false,

                        eventdate = item.EventData.eventdate.Value.ConvertDateTimeToString(),

                        eventdateto = item.EventData.eventdateto.Value.ConvertDateTimeToString(),
                        //allday= Convert.ToBoolean(item.EventData.allday),

                        //description=item.EventData.description,
                        //timefrom = item.EventData.eventfrom == null ? "" : item.EventData.eventfrom.Value.ToString(@"hh\:mm"),
                        //timeto = item.EventData.eventto == null ? "" : item.EventData.eventto.Value.ToString(@"hh\:mm"),
                        //timetext = item.EventData.allday == true ? "All Day" : ((item.EventData.eventfrom == null ? "" : item.EventData.eventfrom.Value.ToString(@"hh\:mm")) + "    to : " + (item.EventData.eventto == null ? "" : item.EventData.eventto.Value.ToString(@"hh\:mm"))),
                        //datetext = ((item.EventData.eventdate == null ? "" : item.EventData.eventdate.Value.ConvertDateTimeToString()) + "    to : " + (item.EventData.eventdateto == null ? "" : item.EventData.eventdateto.Value.ConvertDateTimeToString())),

                        //categorieId = item.EventData.categorie.EntityId,

                        //EncryptedID = StringCipher.EncryptString(item.EventData.EntityId),
                        //lang = item.EventData.lang,
                        //lat= item.EventData.lat,

                        //EntityId  =item.EventData.categorie.EntityId,
                        ////interests = _Event.GetINterestdata(m.Id).Distinct(),
                        //joined = _Event.GetEventattend(allateend, item.EventData.EntityId, CurrentUser.UserDetails.PrimaryId).count,
                    }) : null,
                    MessageAttachedVM = new List<MessageAttachedVM>() { new MessageAttachedVM { attached = item.MessagesAttached == null ? "" : _configuration["BaseUrl"] + item.MessagesAttached } },

                }).ToList();

                int rowCount = MessagesData.TotalCount;

                var totalPages = ((double)rowCount / (double)validFilter.PageSize);
                int roundedTotalPages = Convert.ToInt32(Math.Ceiling(totalPages));

                var pagedModel = new PagedResponse<List<MessagedataVM>>(messages, pageNumber, rowCount, MessagesData.TotalCount);
                return StatusCode(StatusCodes.Status200OK,
                      new ResponseModel<object>(StatusCodes.Status200OK, true,
                      "Chat", new
                      {
                          pageNumber = pageNumber,
                          pageSize = pageSize,
                          totalPages = roundedTotalPages,
                          rowCount,

                          data = pagedModel.Data
                      }));

            }
            catch (Exception ex)
            {
                await _errorLogService.InsertErrorLog(new BWErrorLog(HttpContext.GetUser().UserId, "Messages/UsersChat", ex));
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<object>(StatusCodes.Status500InternalServerError, false, ex.Message, null));

            }
            #region EngEman
            //try
            //{
            //    var userDeatils = HttpContext.GetUser().User.UserDetails;



            //    var Deatils = this._userService.GetUserDetails(userid);
            //    var usermessid = MessageServes.getUserMessages(userDeatils.PrimaryId, Deatils.PrimaryId);


            //    var data = MessageServes.getallMessagedata(usermessid.Id).ToList()
            //        .OrderByDescending(m => m.Messagesdate.Date).ThenByDescending(k => k.Messagestime).ToList();

            //    if (usermessid.deletedate != null || usermessid.Userdeletedate != null)
            //    {
            //        if (usermessid.delete == userDeatils.UserId)
            //        {
            //            data = data.Where(m => m.Messagesdate.Date >= usermessid.deletedate.Value.Date && (m.Messagesdate.Date == usermessid.deletedate.Value.Date ? m.Messagestime > usermessid.deleteTime : true)).ToList();
            //        }
            //        else if (usermessid.Todelete == userDeatils.UserId)
            //        {
            //            data = data.Where(m => m.Messagesdate.Date >= usermessid.Userdeletedate.Value.Date && (m.Messagesdate.Date == usermessid.deletedate.Value.Date ? m.Messagestime > usermessid.UserdeleteTime : true)).ToList();

            //        }
            //    }
            //    var validFilter = new PaginationFilter(pageNumber, pageSize);


            //    List<MessagedataVM> messages = new List<MessagedataVM>();

            //    foreach (var item in data)
            //    {
            //        MessagedataVM dat = new MessagedataVM();
            //        dat.Messages = item.Messages;
            //        dat.Messagesdate = item.Messagesdate.ConvertDateTimeToString();
            //        dat.Messagestime = item.Messagestime.ToString(@"hh\:mm");
            //        dat.Username = item.User.User.DisplayedUserName;
            //        dat.DisplayedUserName = item.User.User.UserName;
            //        dat.UserId = item.User.UserId;
            //        dat.Id = item.Id;
            //        dat.Messagetype = item.Messagetype;
            //        dat.Userimage = _configuration["BaseUrl"] + item.User.UserImage;
            //        dat.currentuserMessage = MessageServes.CheckUserMessagescolor(userDeatils.PrimaryId, item.Id);
            //        List<MessageAttachedVM> MessageAttachedVM = new List<MessageAttachedVM>();

            //        MessageAttachedVM Attacheddata = new MessageAttachedVM();
            //        Attacheddata.attached = (item.MessagesAttached == null || item.MessagesAttached == "") ? "" : _configuration["BaseUrl"] + item.MessagesAttached;
            //        // if(item.MessagesAttached != null&& item.MessagesAttached!= "")
            //        MessageAttachedVM.Add(Attacheddata);

            //        dat.MessageAttachedVM = MessageAttachedVM;
            //        messages.Add(dat);
            //    }

            //    messages = messages.Where(m => (m.Messages == null ? true : (m.Messages.Replace(" ", "") != "" && m.Messages.Replace(" ", "") != null)) && m.MessageAttachedVM.Count() != 0).ToList();

            //    var pagedevent = messages.Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
            //       .Take(validFilter.PageSize).ToList();
            //    var pagedModel = new PagedResponse<List<MessagedataVM>>(pagedevent, validFilter.PageNumber, pagedevent.Count(), messages.Count());
            //    return StatusCode(StatusCodes.Status200OK,
            //          new ResponseModel<object>(StatusCodes.Status200OK, true,
            //          "Chat", new
            //          {
            //              pageNumber = pagedModel.PageNumber,
            //              pageSize = pagedModel.PageSize,
            //              totalPages = pagedModel.TotalPages,
            //              totalRecords = pagedModel.TotalRecords,

            //              data = pagedModel.Data
            //          }));

            //}
            //catch (Exception ex)
            //{
            //    await _errorLogService.InsertErrorLog(new BWErrorLog(HttpContext.GetUser().UserId, "Messages/UsersChat", ex));
            //    return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<object>(StatusCodes.Status500InternalServerError, false, ex.Message, null));

            //}
            #endregion
        }


        [HttpPost]
        [Route("EventChat")]
        public async Task<IActionResult> EventChat([FromForm] int pageNumber, [FromForm] int pageSize, [FromForm] string Eventid)
        {
            try
            {
                var userDeatils = HttpContext.GetUser().User.UserDetails;
                var DbF = Microsoft.EntityFrameworkCore.EF.Functions;

                // string usermessid = MessageServes.geteventMessages(Eventid);
                var data = MessageServes.geteventMessagedata(Eventid)
                    .OrderByDescending(m => m.Messagesdate.Date).ThenByDescending(k => k.Messagestime).ToList();

                var attenddata = _Event.GetEventChatAttend(Eventid, userDeatils.UserId, true);
                List<MessagedataVM> messages = new List<MessagedataVM>();
                if (attenddata != null)
                {
                    

                    data = data.Where(n => n.Messagesdate.Date >= attenddata.JoinDate.Value.Date && (n.Messagesdate.Date == attenddata.JoinDate.Value.Date ? (n.Messagestime >= attenddata.Jointime) : true)).ToList();
                    //if (attenddata.deletedate != null && attenddata.delettime != null)
                    //{
                    //    data = data.Where(b => b.Messagesdate.Date >= attenddata.deletedate.Value.Date && b.Messagestime > attenddata.delettime).ToList();
                    //}

                    if (attenddata.delete == true)
                    {
                        data = data.Where(x => (DbF.DateDiffDay(attenddata.deletechatDate, x.Messagesdate) > 0) || ((DbF.DateDiffDay(attenddata.deletechatDate, x.Messagesdate) == 0 && (DbF.DateDiffSecond(attenddata.deletechattime, x.Messagestime) > 0)))).ToList();
                        //data = data.Where(b => b.Messagesdate.Date > attenddata.deletechatDate.Value.Date && b.Messagestime > attenddata.deletechattime).ToList();
                        //data = data.Where(b => b.Messagesdate.Date >= attenddata.deletechatDate.Value.Date && (b.Messagesdate.Date == attenddata.deletechatDate.Value.Date ? (b.Messagestime > attenddata.deletechattime) : true)).ToList();
                    }



                    if (attenddata.leavechat == true)
                    {
                        //data = data.Where(x => (DbF.DateDiffDay(attenddata.leveeventchatDate, x.Messagesdate) < 0) || ((DbF.DateDiffDay(attenddata.leveeventchatDate, x.Messagesdate) == 0 && (DbF.DateDiffSecond(attenddata.leveeventchattime, x.Messagestime) < 0)))).ToList();
                        data = data.Where(b => b.Messagesdate.Date <= attenddata.leveeventchatDate.Value.Date && (b.Messagesdate.Date.Date == attenddata.leveeventchatDate.Value.Date ? (b.Messagestime < attenddata.leveeventchattime) : true)).ToList();

                        //data = data.Where(b => b.Messagesdate.Date < attenddata.leveeventchatDate.Value.Date &&

                        //(b.Messagestime < attenddata.leveeventchattime)).ToList();
                    }


                    if (attenddata.leave == true)
                    {
                        data = data.Where(b => b.Messagesdate.Date <= attenddata.leaveeventDate.Value.Date && (b.Messagesdate.Date.Date == attenddata.leaveeventDate.Value.Date ? (b.Messagestime < attenddata.leaveeventtime) : true)).ToList();
                    }
                    if (attenddata.removefromevent == true)
                    {
                        data = data.Where(b => b.Messagesdate.Date <= attenddata.deletedate.Value.Date && (b.Messagesdate.Date.Date == attenddata.deletedate.Value.Date ? (b.Messagestime < attenddata.delettime) : true)).ToList();
                    }
                    //if (attenddata.stutus == 2)
                    //{
                    //    data = data.Where(b => b.Messagesdate.Date <= attenddata.leveeventchatDate.Value.Date && (b.Messagesdate.Date.Date == attenddata.leveeventchatDate.Value.Date ? (b.Messagestime < attenddata.leveeventchattime) : true)).ToList();

                    //}
                    var allateend = _Event.allattendevent().ToList();

                    foreach (var item in data)
                    {
                        MessagedataVM dat = new MessagedataVM();
                        dat.Messages = item.Messages;
                        dat.Latitude = item.Latitude;
                        dat.Longitude = item.Longitude;
                        dat.Messagesdate = item.Messagesdate.ConvertDateTimeToString();
                        dat.Messagestime = item.Messagestime.ToString(@"hh\:mm");
                        dat.Username = item.User.userName;
                        dat.Userimage = _configuration["BaseUrl"] + item.User.UserImage;
                        dat.UserId = item.User.UserId;
                        dat.currentuserMessage = MessageServes.CheckUserMessagescolor(userDeatils.PrimaryId, item.Id);
                        dat.Id = item.Id;
                        dat.Messagetype = item.Messagetype;
                        dat.IsWhitelabel = item.User.IsWhiteLabel.HasValue? item.User.IsWhiteLabel.Value:false;
                        List <MessageAttachedVM> MessageAttachedVM = new List<MessageAttachedVM>();
                        dat.EventLINKid = item.Messagetype == 4 ? item.EventData.EntityId : null;
                        dat.linkable = item.linkable;
                        dat.EventData = (item.Messagetype == 4 && item.EventData.eventdateto.Value >= DateTime.Now) ? (new EventDatalinka
                        {
                            //id, title, image, category, joined, totalNumber, event Date, key, myEvent
                            eventdate = item.EventData.eventdate.Value.ConvertDateTimeToString(),

                            eventdateto = item.EventData.eventdateto.Value.ConvertDateTimeToString(),
                            MyEvent = item.EventData.UserId == userDeatils.PrimaryId ? true : false,
                            categorie = item.EventData.categorie?.name,
                            categorieimage = _configuration["BaseUrl"] + item.EventData.categorie?.image,
                            //categorieId = item.EventData.categorie.EntityId,
                            Title = item.EventData.Title,
                            //#################################################################
                            //TODO:Abdelrahman (Fix Url )
                            // Image = _configuration["BaseUrl"] + item.EventData.image,
                            Image = (item.EventData.EventTypeListid != 3 ? _configuration["BaseUrl"] : "") + item.EventData.image,
                            Id = item.EventData.EntityId,
                            // EncryptedID = StringCipher.EncryptString(item.EventData.EntityId),
                            key = item.UserId == userDeatils.PrimaryId ? 1 : (_Event.GetEventattend(allateend, item.EventData.EntityId, userDeatils.PrimaryId).type == true ? 2 : 3),
                            eventtypeid = item.EventData.EventTypeList?.entityID,
                            eventtypecolor = item.EventData.EventTypeList?.color,
                            eventtype = item.EventData.EventTypeList?.Name,
                            totalnumbert = item.EventData.totalnumbert,
                            //EntityId = item.EventData.categorie.EntityId,
                            //interests = _Event.GetINterestdata(m.Id).Distinct(),
                            joined = _Event.GetEventattend(allateend, item.EventData.EntityId, userDeatils.PrimaryId).count,
                        }) : null;

                        MessageAttachedVM Attacheddata = new MessageAttachedVM();


                        Attacheddata.attached = (item.MessagesAttached == null || item.MessagesAttached == "") ? "" : _configuration["BaseUrl"] + item.MessagesAttached;
                        if (item.MessagesAttached != null && item.MessagesAttached != "")
                            MessageAttachedVM.Add(Attacheddata);

                        dat.MessageAttachedVM = MessageAttachedVM;
                        messages.Add(dat);
                    }
                }
                else
                {
                    data = null;
                }

                var validFilter = new PaginationFilter(pageNumber, pageSize);

                messages = messages.Where(m => (m.Messages == null ? true : (m.Messages.Replace(" ", "") != "" && m.Messages.Replace(" ", "") != null)) || m.MessageAttachedVM.Count() != 0).ToList();

                int rowCount = messages.Count();

                var totalPages = ((double)rowCount / (double)validFilter.PageSize);
                int roundedTotalPages = Convert.ToInt32(Math.Ceiling(totalPages));

                var pagedevent = messages.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

                var pagedModel = new PagedResponse<List<MessagedataVM>>(

                    pagedevent,
                    pageSize,
                    roundedTotalPages,
                    rowCount
                    );

                pagedModel.PageNumber = pageNumber;
                pagedModel.PageSize = pageSize;
                pagedModel.TotalPages = roundedTotalPages;



                return StatusCode(StatusCodes.Status200OK,
                      new ResponseModel<object>(StatusCodes.Status200OK, true,
                      "Chat", new
                      {
                          pagedModel

                          //Attendees = _Event.getattendevent(Eventid, userDeatils.UserId).Select(m => new
                          //{
                          //    DisplayedUserName = m.Userattend.User.UserName,
                          //    UserName = m.Userattend.User.DisplayedUserName,
                          //    m.Userattend.PrimaryId,
                          //    m.stutus,
                          //    image = _configuration["BaseUrl"] + m.Userattend.UserImage,
                          //    JoinDate = m.JoinDate == null ? DateTime.Now.Date.ConvertDateTimeToString() : m.JoinDate.Value.Date.ConvertDateTimeToString(),
                          //    interests = _Event.GetINterestdata(m.Userattend.PrimaryId).Select(m => new { m.Interests.name, m.InterestsId })
                          //}).ToList()
                      }));

            }
            catch (Exception ex)
            {
                await _errorLogService.InsertErrorLog(new BWErrorLog(HttpContext.GetUser().UserId, "Messages/EventChat", ex));
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<object>(StatusCodes.Status500InternalServerError, false, ex.Message, null));

            }
        }


        [HttpPost]
        [Route("NotificationData")]
        public async Task<IActionResult> FireBaseDatamodel([FromForm] int pageNumber, [FromForm] int pageSize)
        {
            try
            {
                var userDeatils = HttpContext.GetUser().User.UserDetails;

                //var user = await userManager.FindByIdAsync(userDeatils.UserId);

                //var data = this._authContext.FireBaseDatamodel.Include(m => m.User).ToList();

                var data = MessageServes.getFireBaseDatamodel(userDeatils.PrimaryId).OrderByDescending(m => m.CreatedAt);

                var validFilter = new PaginationFilter(pageNumber, pageSize);

                var pagedevent = data.Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                   .Take(validFilter.PageSize).ToList();
                List<FireBaseData> messages = new List<FireBaseData>();

                foreach (var item in pagedevent)
                {
                    FireBaseData dat = new FireBaseData();
                    dat.Title = item.Title;
                    dat.Action = item.Action;
                    dat.Action_code = item.Action_code;
                    dat.Body = item.Body;
                    dat.CreatedAt = item.CreatedAt.ToString(@"dd-MM-yyyy hh\:mm");
                    dat.NotificationDate = item.CreatedAt.ToString("dd-MM-yyyy") + "T" + item.CreatedAt.ToString("HH:mm:ss");
                    dat.imageUrl = item.imageUrl;
                    dat.Messagetype = item.Messagetype;
                    dat.id = item.id;
                    dat.Messagetype = item.Messagetype;
                    messages.Add(dat);
                }
                var pagedModel = new PagedResponse<List<FireBaseData>>(messages, validFilter.PageNumber,
              pagedevent.Count(), data.Count());
                return StatusCode(StatusCodes.Status200OK,
                      new ResponseModel<object>(StatusCodes.Status200OK, true,
                      "Chat", pagedModel));

            }
            catch (Exception ex)
            {
                await _errorLogService.InsertErrorLog(new BWErrorLog(HttpContext.GetUser().UserId, "Messages/NotificationData", ex));
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<object>(StatusCodes.Status500InternalServerError, false, ex.Message, null));

            }
        }


        [HttpPost]
        [Route("muitchat")]
        public async Task<IActionResult> muitchate([FromForm] muitchate muitchate)
        {
            try
            {
                var userDeatils = HttpContext.GetUser().User.UserDetails;

                var user = await userManager.FindByIdAsync(userDeatils.UserId);
                if (user == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound,
                    new ResponseModel<object>(StatusCodes.Status404NotFound, false,
                    _localizer["userdoesnotexist"], null));
                }
                if (muitchate.isevent)
                {


                    //var attend = _Event.GetuserEvent(muitchate.id, user.Id);
                    //attend.muit = muitchate.muit;
                    //await _Event.updateeventattend(attend);
                    var attendchat = _Event.GetEventChatAttend(muitchate.id, user.Id);
                    attendchat.muit = muitchate.muit;
                    await _Event.editeEventChatAttend(attendchat);
                    //var cuserount = _Event.GetEventChatAttend(model.EventDataid, loggedinUser.UserId);

                    return StatusCode(StatusCodes.Status200OK,
                      new ResponseModel<object>(StatusCodes.Status200OK, true,
                      "event muit  successfully", muitchate));

                }
                else
                {
                    var data = MessageServes.GetUserMessages(user.Id, muitchate.id);

                    if (data.muit == null && data.Tomuit == null)
                    {
                        data.muit = user.Id;
                    }
                    else if (data.muit == user.Id)
                    {
                        data.muit = null;
                    }
                    else if (data.Tomuit == user.Id)
                    {
                        data.Tomuit = null;
                    }
                    else if (data.muit != user.Id && data.Tomuit != user.Id)
                    {
                        if (data.muit == null)
                        {
                            data.muit = user.Id;
                        }
                        else if (data.Tomuit == null)
                        {
                            data.Tomuit = user.Id;
                        }

                    }
                    await MessageServes.updateUserMessages(data);
                    return StatusCode(StatusCodes.Status200OK,
                      new ResponseModel<object>(StatusCodes.Status200OK, true,
                      "chat muit  successfully", muitchate));
                }

            }
            catch (Exception ex)
            {
                await _errorLogService.InsertErrorLog(new BWErrorLog(HttpContext.GetUser().UserId, "Messages/muitchate", ex));
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<object>(StatusCodes.Status500InternalServerError, false, ex.Message, null));

            }
        }


        [HttpPost]
        [Route("Deletchat")]
        public async Task<IActionResult> Deletchat([FromForm] deletechat muitchate)
        {
            try
            {
                var loggedinUser = HttpContext.GetUser().User.UserDetails;

                if (muitchate?.DeleteDateTime == null)
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                   new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,
                    _localizer["DeleteDateTime is Required"], null));
                }
                if (muitchate.isevent)
                {


                    var attend = _Event.GetEventChatAttend(muitchate.id, loggedinUser.UserId);
                    attend.deletechatDate = muitchate?.DeleteDateTime?.Date ?? DateTime.Now.Date;
                    attend.deletechattime = muitchate?.DeleteDateTime?.TimeOfDay ?? DateTime.Now.TimeOfDay;
                    attend.UserNotreadcount = 0;
                    attend.delete = true;
                    await _Event.editeEventChatAttend(attend);

                    return StatusCode(StatusCodes.Status200OK,
                      new ResponseModel<object>(StatusCodes.Status200OK, true,
                      "event deleted  successfully", muitchate));

                }
                else
                {
                    var data = MessageServes.GetUserMessages(loggedinUser.UserId, muitchate.id);
                    if (data == null)
                    {
                        return StatusCode(StatusCodes.Status406NotAcceptable,
                       new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,
                        _localizer["No Chat With this userid"], null));
                    }
                    if (data.delete == null && data.Todelete == null)
                    {
                        data.delete = loggedinUser.UserId;

                        data.deletedate = muitchate?.DeleteDateTime?.Date ?? DateTime.Now.Date;
                        data.deleteTime = muitchate?.DeleteDateTime?.TimeOfDay ?? DateTime.Now.TimeOfDay;
                    }
                    else if (data.delete == loggedinUser.UserId)
                    {
                        data.UserNotreadcount = 0;
                        data.delete = loggedinUser.UserId;
                        data.deletedate = muitchate?.DeleteDateTime?.Date ?? DateTime.Now.Date;
                        data.deleteTime = muitchate?.DeleteDateTime?.TimeOfDay ?? DateTime.Now.TimeOfDay;
                    }
                    else if (data.Todelete == loggedinUser.UserId)
                    {
                        data.ToUserNotreadcount = 0;
                        data.Todelete = loggedinUser.UserId;
                        data.Userdeletedate = muitchate?.DeleteDateTime?.Date ?? DateTime.Now.Date;
                        data.UserdeleteTime = muitchate?.DeleteDateTime?.TimeOfDay ?? DateTime.Now.TimeOfDay;
                    }
                    else if (data.delete != loggedinUser.UserId && data.Todelete != loggedinUser.UserId)
                    {
                        if (data.delete == null)
                        {
                            data.delete = loggedinUser.UserId;
                            data.UserNotreadcount = 0;
                            data.deletedate = muitchate?.DeleteDateTime?.Date ?? DateTime.Now.Date;
                            data.deleteTime = muitchate?.DeleteDateTime?.TimeOfDay ?? DateTime.Now.TimeOfDay;
                        }
                        else if (data.Todelete == null)
                        {
                            data.Todelete = loggedinUser.UserId;
                            data.ToUserNotreadcount = 0;
                            data.Userdeletedate = muitchate?.DeleteDateTime?.Date ?? DateTime.Now.Date;
                            data.UserdeleteTime = muitchate?.DeleteDateTime?.TimeOfDay ?? DateTime.Now.TimeOfDay;
                        }

                    }
                    await MessageServes.updateUserMessages(data);
                    return StatusCode(StatusCodes.Status200OK,
                      new ResponseModel<object>(StatusCodes.Status200OK, true,
                      "chat deleted  successfully", null));
                }

            }
            catch (Exception ex)
            {
                await _errorLogService.InsertErrorLog(new BWErrorLog(HttpContext.GetUser().UserId, "Messages/Deletchat", ex));
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<object>(StatusCodes.Status500InternalServerError, false, ex.Message, null));

            }
        }


    }
}
