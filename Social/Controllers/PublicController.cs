using CRM.Services.Wrappers;
using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Social.Entity.DBContext;
using Social.Entity.Enums;
using Social.Entity.Migrations;
using Social.Entity.Models;
using Social.Entity.ModelView;
using Social.Sercices.Helpers;
using Social.Services;
using Social.Services.FireBase_Helper;
using Social.Services.Helpers;
using Social.Services.ModelView;
using Social.Services.Services;
using System;
using System.Collections.Generic;
using System.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Social.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class PublicController : ControllerBase
    {
        private readonly IEventServ _eventServ;
        private readonly EmailHelper _emailHelper;
        private readonly IUserService _userService;
        private readonly AuthDBContext _authContext;
        private readonly IFrindRequest _friendRequest;
        private readonly IMessageServes _messageServes;
        private readonly IConfiguration _configuration;
        private readonly IFirebaseManager _firebaseManager;
        private readonly IErrorLogService _errorLogService;
        private readonly IGlobalMethodsService _globalMethodsService;
        private readonly IEventTypeListService _eventTypeListService;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly IAppConfigrationService _appConfigrationService;
        private readonly ExternalEventUtility _externalEventUtility;
        public PublicController(IUserService userService, IErrorLogService errorLogService, AuthDBContext authDBContext, IAppConfigrationService appConfigrationService,
            IConfiguration configuration, IStringLocalizer<SharedResource> stringLocalizer, IFrindRequest friendRequest, IEventServ eventServ,
            IEventTypeListService eventTypeListService, IMessageServes messageServes, IFirebaseManager firebaseManager, IGlobalMethodsService globalMethodsService,
            EmailHelper emailHelper, ExternalEventUtility externalEventUtility)
        {
            _eventServ = eventServ;
            _emailHelper = emailHelper;
            _userService = userService;
            _localizer = stringLocalizer;
            _authContext = authDBContext;
            _friendRequest = friendRequest;
            _configuration = configuration;
            _messageServes = messageServes;
            _firebaseManager = firebaseManager;
            _errorLogService = errorLogService;
            _eventTypeListService = eventTypeListService;
            _globalMethodsService = globalMethodsService;
            _appConfigrationService = appConfigrationService;
            _externalEventUtility = externalEventUtility;
        }

        #region Feeds

        /// <summary>
        ///    Feeds Api
        /// </summary> 
        [HttpPost]
        [Route("AllUserPublic")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> AllUserPublic([FromForm] int pageNumber, [FromForm] int pageSize, [FromForm] double degree, [FromForm] string userlang, [FromForm] string userlat)
        {
            try
            {
                if (userlang == null || userlat == null)
                {
                    return StatusCode(StatusCodes.Status400BadRequest,
                    new ResponseModel<object>(StatusCodes.Status400BadRequest, false,
                     _localizer["please allow your location"], null));
                }

                List<Requestes> allRequestes = await _authContext.Requestes.ToListAsync();

                var lat = userlat is null ? 0 : Convert.ToDouble(userlat);
                var lang = userlang is null ? 0 : Convert.ToDouble(userlang);

                if (lat == 0 || lang == 0)
                {
                    return StatusCode(StatusCodes.Status200OK, new ResponseModel<object>(StatusCodes.Status200OK, true, "No data ", new
                    {
                        pageNumber = 0,
                        pageSize = pageSize,
                        totalPages = 0,
                        totalRecords = 0,
                        data = new List<Feeds>()
                    }));
                }

                var appcon = _appConfigrationService.GetData().FirstOrDefault();

                List<UserDetails> userDetailsList = (degree == 0) ? await _userService.PublicAllUsers(lat, lang, appcon) : await _userService.PublicAllUsersDirection(lat, lang, degree, appcon);

                var validFilter = new PaginationFilter(pageNumber, pageSize);

                var data = userDetailsList.Select(m => new Feeds
                {
                    Gender = m.Gender,
                    userId = m.UserId,
                    lang = m.lang,
                    lat = m.lat,
                    ImageIsVerified = m.ImageIsVerified ?? false,
                    DisplayedUserName = m.User.UserName,
                    UserName = m.User.DisplayedUserName,
                    email = m.User.Email,
                    image = string.IsNullOrEmpty(m.UserImage) ? "" : _configuration["BaseUrl"] + m.UserImage,
                    key = 0
                }).Where(k => k.key != 4 && k.key != 5).ToList();

                var pagedLands = data.Skip((validFilter.PageNumber - 1) * validFilter.PageSize).Take(validFilter.PageSize).ToList();

                var pagedModel = new PagedResponse<List<Feeds>>(pagedLands, validFilter.PageNumber, pagedLands.Count(), data.Count());

                return StatusCode(StatusCodes.Status200OK, new ResponseModel<object>(StatusCodes.Status200OK, true, "Your data ", new
                {
                    pageNumber = pagedModel.PageNumber,
                    pageSize = pagedModel.PageSize,
                    totalPages = pagedModel.TotalPages,
                    totalRecords = pagedModel.TotalRecords,
                    data = pagedModel.Data
                }));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<object>(StatusCodes.Status500InternalServerError, false, ex.Message, null));
            }
        }

        #endregion


        #region Event

        [HttpPost]
        [Route("GetCategories")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                var data = _eventServ.getcategory();

                return StatusCode(StatusCodes.Status200OK,
                new ResponseModel<object>(StatusCodes.Status200OK, true,
                "Categories!", data.Select(m => new { Id = m.EntityId, m.name })));

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<object>(StatusCodes.Status500InternalServerError, false, ex.Message, null));

            }
        }

        [HttpPost]
        [Route("EventsAroundMe")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> EventsAroundMe([FromForm] string lang, [FromForm] string lat, [FromForm] int pageNumber, [FromForm] int pageSize, [FromForm] string categories)
        {
            try
            {
                var appcon = _appConfigrationService.GetData().FirstOrDefault();

                var cuserount = _eventServ.GetAllEventLocations(pageNumber, pageSize, Convert.ToDouble(lat), Convert.ToDouble(lang), appcon, categories);

                return StatusCode(StatusCodes.Status200OK,
                      new ResponseModel<object>(StatusCodes.Status200OK, true,
                      "data ", cuserount));

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<object>(StatusCodes.Status500InternalServerError, false, ex.Message, null));

            }
        }

        [HttpPost]
        [Route("OnlyEventsAround")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> OnlyEventsAround([FromForm] string lang, [FromForm] string lat, [FromForm] int pageNumber, [FromForm] int pageSize, [FromForm] string categories)
        {
            try
            {
                var appcon = _appConfigrationService.GetData().FirstOrDefault();

                (List<EventVM> eventData, int totalRowCount) = await _eventServ.GetAllEventLocations_2(Convert.ToDouble(lat), Convert.ToDouble(lang), appcon, categories, pageNumber, pageSize);

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
                          totalPages = roundedTotalPages,
                          totalRecords = totalRowCount,
                          data = pagedModel.Data
                      }));

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<object>(StatusCodes.Status500InternalServerError, false, ex.Message, null));

            }
        }

        [HttpPost]
        [Route("PublicAddEventData")]
        public async Task<IActionResult> PublicAddEventData([FromForm] AddExternalEventDataModel model)
        {
            try
            {

                #region Guard Clause

                if (model == null)
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                        new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,
                         _localizer["EventDataReqired"], null));
                }

                #endregion

                #region Validate Event Data

                //External Event Type Id ===> (265583AA-2511-4FD3-883E-6CAF1F8E4355)
                EventTypeListVM typedate = await _eventTypeListService.GetData(Guid.Parse("265583AA-2511-4FD3-883E-6CAF1F8E4355"));

                var dataconfig = _appConfigrationService.GetData().FirstOrDefault();

                List<EventData> oldEvents = _eventServ.getallexternalevent();

                EventData eventExists = oldEvents.FirstOrDefault(m => m.lat == model.latitude && m.lang == model.longitude && m.Title == model.Title && m.eventdate == Convert.ToDateTime(model.eventdate));


                if (model.longitude == "" || model.longitude == null || model.latitude == "" || model.latitude == null)
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                        new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                         _localizer["LocationsIsRequired"], null));
                }

                if (model.eventdate == null || model.eventdateto == null)
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                        new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                         _localizer["StartAndEndDateIsRequired"], null));
                }
                if ((model.timefrom == null || model.timeto == null) && model.allday == false)
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                        new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                         _localizer["StartAndEndTimeIsRequired"], null));
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
                if (model.eventdate > model.eventdateto)
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                        new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                         _localizer["EventstartdatemustNotolderthanEventenddate"], null));
                }
                if (model.timefrom >= model.timeto)
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                        new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                         _localizer["EventstarttimemustNotolderthanEventendtime"], null));
                }
                if (string.IsNullOrEmpty(model.Title))
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                        new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                         _localizer["TitleIsRequired"], null));
                }
                if (string.IsNullOrEmpty(model.description))
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                        new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                         _localizer["DescriptionIsRequired"], null));
                }
                if (dataconfig.EventTitle_MinLength != null)
                {
                    if (model.Title.Length >= dataconfig.EventTitle_MinLength && model.Title.Length >= dataconfig.EventTitle_MaxLength)
                    {
                        return StatusCode(StatusCodes.Status406NotAcceptable,
                            new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                                      _localizer[" EventTitleMinLengthIs "] + dataconfig.EventTitle_MinLength + _localizer[" EventTitleMaxLengthIs "] + dataconfig.EventTitle_MinLength, null));

                    }
                }
                if (dataconfig.EventDetailsDescription_MaxLength != null)
                {
                    if (model.description.Length >= dataconfig.EventDetailsDescription_MinLength && model.description.Length >= dataconfig.EventDetailsDescription_MaxLength)
                    {
                        return StatusCode(StatusCodes.Status406NotAcceptable,
                            new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                             _localizer[" EventDetailsDescriptionMinLengthIs "] + dataconfig.EventDetailsDescription_MinLength + _localizer[" EventDetailsDescriptionMaxLengthIs "] + dataconfig.EventDetailsDescription_MaxLength, null));
                    }
                }
                if (model.categorie == "" || model.categorie == null)
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                        new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                         _localizer["CategoryIsRequired"], null));
                }
                if (model.allday == null)
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                        new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                         _localizer["AllDayIsRequired"], null));
                }
                if (string.IsNullOrEmpty(model.checkout_details))
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                        new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                         _localizer["CheckOutDetailsIsRequired"], null));
                }
                if (string.IsNullOrEmpty(model.image))
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                         new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                          _localizer["EventImageIsRequired"], null));
                }
                if (eventExists != null)
                {
                    try
                    {
                        _eventServ.UpdateExistEvent(eventExists, model, typedate.ID);
                        await _eventServ.UpdateEventAddressFromGoogle(eventExists);

                        int result = await _authContext.SaveChangesAsync();

                        string message = _localizer["Added"];

                        if (result > 0) return StatusCode(StatusCodes.Status200OK, new ResponseModel<object>(StatusCodes.Status200OK, true, _localizer["YourEventhasbeenAddedsuccessfully"], message));
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                    return StatusCode(StatusCodes.Status200OK, new ResponseModel<object>(StatusCodes.Status200OK, true, _localizer["NotAdded"], _localizer["NotAdded"]));

                }

                #endregion

                #region Insert Events In To Database 

                EventData newevEntData = new EventData()
                {
                    Title = model.Title,
                    eventtype = Guid.Parse("265583AA-2511-4FD3-883E-6CAF1F8E4355"),
                    image = model.image,
                    description = model.description,
                    status = model.status,
                    categorieId = _eventServ.ExtractEventCategory(model.categorie),
                    EntityId = Guid.NewGuid().ToString(),
                    EventTypeListid = typedate.ID,
                    lang = model.longitude,
                    lat = model.latitude,
                    allday = model.allday,
                    totalnumbert = model.totalnumbert == 0 ? 1000 : model.totalnumbert,
                    eventdate = model.eventdate,
                    eventdateto = model.eventdateto,
                    eventfrom = model.timefrom,
                    eventto = model.timeto,
                    CityID = model.CityID,
                    CountryID = model.CountryID,
                    StopTo = model.StopTo,
                    CreatedDate = DateTime.Now,
                    StopFrom = model.StopFrom,
                    checkout_details = model.checkout_details,
                    showAttendees = model.showAttendees,
                    IsActive = true,
                    UserId = 269
                };

                var cuserount = await _eventServ.InsertEvent(newevEntData);

                EventChatAttend eventChatAttend = new EventChatAttend() { Jointime = DateTime.Now.TimeOfDay, EventDataid = newevEntData.Id, UserattendId = newevEntData.UserId, JoinDate = DateTime.Now, ISAdmin = true };

                EventChatAttend newEventChatAttend = await _eventServ.InsertEventChatAttend(eventChatAttend);

                #endregion

                if (cuserount != null)
                {
                    string message = _localizer["Added"];
                    return StatusCode(StatusCodes.Status200OK, new ResponseModel<object>(StatusCodes.Status200OK, true, _localizer["YourEventhasbeenAddedsuccessfully"], message));
                }

                return StatusCode(StatusCodes.Status200OK, new ResponseModel<object>(StatusCodes.Status200OK, true, _localizer["NotAdded"], cuserount));

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<object>(StatusCodes.Status500InternalServerError, false, ex.Message, null));

            }
        }

        [HttpPost]
        [Route("PublicAddjsonFileEventData")]
        public async Task<IActionResult> PublicAddjsonFileEventData([FromForm] IFormFile jsonFile)
        {
            try
            {
                #region Guard Clause

                if (jsonFile == null)
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                        new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,
                         _localizer["JsonFileIsReqired"], null));
                }

                string fileContent = null;

                using (var reader = new StreamReader(jsonFile.OpenReadStream()))
                {
                    fileContent = reader.ReadToEnd();
                }

                List<AddExternalEventDataModel> jsonEventsData = JsonConvert.DeserializeObject<List<AddExternalEventDataModel>>(fileContent);
                List<AddExternalEventDataModel> NegelectedEvents = new List<AddExternalEventDataModel>();

                if (jsonEventsData == null || jsonEventsData.Count() == 0)
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                        new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                         _localizer["JsonFileIsEmpty"], null));
                }

                InsertedEventResultViewModel insertedEventResultViewModel = new InsertedEventResultViewModel();

                insertedEventResultViewModel.Total_File_Events = jsonEventsData.Count();

                #endregion

                #region Validate Event Data

                //External Event Type Id ===> (265583AA-2511-4FD3-883E-6CAF1F8E4355)
                EventTypeListVM typedate = await _eventTypeListService.GetData(Guid.Parse("265583AA-2511-4FD3-883E-6CAF1F8E4355"));

                var dataconfig = _appConfigrationService.GetData().FirstOrDefault();

                List<EventData> oldEvents = _eventServ.getallexternalevent();
                List<int> updatedEvents = new List<int>();
                var eventMetaData = new EventMetaData();
                foreach (var evnt in jsonEventsData.ToList())
                {
                    EventData eventExists = oldEvents.FirstOrDefault(m => m.lat == evnt.latitude && m.lang == evnt.longitude && m.Title == evnt.Title && m.eventdate == Convert.ToDateTime(evnt.eventdate));

                    if (evnt.longitude == "" || evnt.longitude == null || evnt.latitude == "" || evnt.latitude == null)
                    {
                        NegelectedEvents.Add(evnt);
                        eventMetaData.EmptyCountEventlongitudeLatitude = NegelectedEvents.Where(evnt => evnt.longitude == "" || evnt.longitude == null || evnt.latitude == "" || evnt.latitude == null).Count();
                        jsonEventsData.Remove(evnt);
                        continue;
                    }
                    if (evnt.eventdate == null || evnt.eventdateto == null)
                    {
                        NegelectedEvents.Add(evnt);
                        eventMetaData.EventDateOrEventDateToNull = NegelectedEvents.Where(evnt => evnt.eventdate == null || evnt.eventdateto == null).Count();
                        jsonEventsData.Remove(evnt);
                        continue;
                    }
                    if ((evnt.timefrom == null || evnt.timeto == null) && evnt.allday == false)
                    {
                        NegelectedEvents.Add(evnt);
                        eventMetaData.EventTimeFromToNull = NegelectedEvents.Where(evnt => (evnt.timefrom == null || evnt.timeto == null) && evnt.allday == false).Count();
                        jsonEventsData.Remove(evnt);
                        continue;
                    }
                    //if (evnt.eventdate < DateTime.Now.Date || evnt.eventdate > DateTime.Now.Date.AddYears(1))
                    //{
                    //    NegelectedEvents.Add(evnt);
                    //    jsonEventsData.Remove(evnt);
                    //    continue;
                    //}
                    //if (evnt.eventdateto > evnt.eventdate.Value.AddMonths(1)) 
                    //{
                    //    NegelectedEvents.Add(evnt);
                    //    jsonEventsData.Remove(evnt);
                    //    continue;
                    //}
                    if (evnt.eventdate > evnt.eventdateto)
                    {
                        NegelectedEvents.Add(evnt);
                        eventMetaData.EmptyCountEventDateOldThanEventDateto = NegelectedEvents.Where(evnt => evnt.eventdate > evnt.eventdateto).Count();
                        jsonEventsData.Remove(evnt);
                        continue;
                    }
                    //if (evnt.timefrom >= evnt.timeto)
                    //{
                    //    jsonEventsData.Remove(evnt);
                    //    continue;
                    //}
                    if (string.IsNullOrEmpty(evnt.Title))
                    {
                        NegelectedEvents.Add(evnt);
                        eventMetaData.EmptyCountTitle = NegelectedEvents.Where(evnt => string.IsNullOrEmpty(evnt.Title)).Count();
                        jsonEventsData.Remove(evnt);
                        continue;
                    }
                    if (string.IsNullOrEmpty(evnt.description) && eventExists == null)
                    {
                        NegelectedEvents.Add(evnt);
                        eventMetaData.EmptyCountDescription = NegelectedEvents.Where(evnt => string.IsNullOrEmpty(evnt.description)).Count();
                        // jsonEventsData.Remove(evnt);
                        evnt.description = "-";
                        continue;
                    }
                    if (dataconfig != null && dataconfig.EventTitle_MinLength != null)
                    {
                        if (evnt.Title.Length >= dataconfig.EventTitle_MinLength && evnt.Title.Length >= dataconfig.EventTitle_MaxLength)
                        {
                            NegelectedEvents.Add(evnt);
                            eventMetaData.EventTitleLengthLargerConfig = NegelectedEvents.Where(evnt => evnt.Title.Length >= dataconfig.EventTitle_MinLength && evnt.Title.Length >= dataconfig.EventTitle_MaxLength).Count();
                            jsonEventsData.Remove(evnt);
                            continue;
                        }
                    }
                    if (dataconfig != null && dataconfig.EventDetailsDescription_MaxLength != null)
                    {
                        if (evnt.description.Length >= dataconfig.EventDetailsDescription_MinLength && evnt.description.Length >= dataconfig.EventDetailsDescription_MaxLength)
                        {
                            NegelectedEvents.Add(evnt);
                            eventMetaData.EventDescriptionLengthLargerConfig = NegelectedEvents.Where(evnt => evnt.description.Length >=
                            dataconfig.EventDetailsDescription_MinLength && evnt.description.Length >= dataconfig.EventDetailsDescription_MaxLength).Count();
                            jsonEventsData.Remove(evnt);
                            continue;
                        }
                    }
                    if (evnt.allday == null)
                    {
                        NegelectedEvents.Add(evnt);
                        eventMetaData.EmptyCountAllDayNull = NegelectedEvents.Where(evnt => evnt.allday == null).Count();
                        jsonEventsData.Remove(evnt);
                        continue;
                    }
                    if (string.IsNullOrEmpty(evnt.checkout_details))
                    {
                        NegelectedEvents.Add(evnt);
                        eventMetaData.EventCheckoutDetailsNull = NegelectedEvents.Where(evnt => string.IsNullOrEmpty(evnt.checkout_details)).Count();
                        jsonEventsData.Remove(evnt);
                        continue;
                    }
                    if (eventExists != null)
                    {
                        //_eventServ.UpdateExistEvent(eventExists, evnt, typedate.ID);
                        updatedEvents.Add(eventExists.Id);
                        insertedEventResultViewModel.Updated_Events += 1;

                        jsonEventsData.Remove(evnt);
                        continue;
                    }

                }

                #endregion


                #region Insert Events In To Database 

                List<EventData> newEvents = jsonEventsData.Select(q => new EventData()
                {
                    Title = q.Title,
                    eventtype = Guid.Parse("265583AA-2511-4FD3-883E-6CAF1F8E4355"),
                    image = q.image,
                    description = q.description,
                    status = q.status,
                    categorieId = _eventServ.ExtractEventCategory(q.categorie.Trim()),
                    EntityId = Guid.NewGuid().ToString(),
                    EventTypeListid = typedate.ID,
                    lang = q.longitude,
                    lat = q.latitude,
                    allday = q.allday,
                    totalnumbert = q.totalnumbert == 0 ? 1000 : q.totalnumbert,
                    eventdate = q.eventdate,
                    eventdateto = q.eventdateto,
                    eventfrom = q.timefrom,
                    eventto = q.timeto,
                    CityID = q.CityID,
                    CountryID = q.CountryID,
                    StopTo = q.StopTo,
                    StopFrom = q.StopFrom,
                    CreatedDate = DateTime.Now,
                    checkout_details = q.checkout_details,
                    showAttendees = q.showAttendees,
                    IsActive = true,
                    UserId = 269,

                }).ToList();

                List<EventData> createdEvents = await _eventServ.InsertEvents(newEvents);

                List<EventChatAttend> eventChatAttends = newEvents.Select(q => new EventChatAttend() { EntityId = Guid.NewGuid().ToString(), Jointime = DateTime.Now.TimeOfDay, EventDataid = q.Id, UserattendId = q.UserId, JoinDate = DateTime.Now, ISAdmin = true }).ToList();

                List<EventChatAttend> newEventChatAttends = await _eventServ.InsertEventChatAttends(eventChatAttends);

                #endregion

                await _authContext.SaveChangesAsync();
                insertedEventResultViewModel.EventMetaData = eventMetaData;
                if (createdEvents != null && createdEvents.Count() != 0)
                {
                    insertedEventResultViewModel.Inserted_Events = createdEvents.Count();
                    insertedEventResultViewModel.Failed_Events = (insertedEventResultViewModel.Inserted_Events + insertedEventResultViewModel.Updated_Events) == 0 ? (insertedEventResultViewModel.Total_File_Events) : (insertedEventResultViewModel.Total_File_Events - (insertedEventResultViewModel.Inserted_Events + insertedEventResultViewModel.Updated_Events));

                    await _eventServ.UpdateEventsAddressFromGoogle(createdEvents);

                    return StatusCode(StatusCodes.Status200OK, new ResponseModel<object>(StatusCodes.Status200OK, true, _localizer["YourEventhasbeenAddedsuccessfully"], insertedEventResultViewModel));
                }

                //if(result == 0) return StatusCode(StatusCodes.Status200OK, new ResponseModel<object>(StatusCodes.Status200OK, true, _localizer["NotAdded"], new InsertedEventResultViewModel { Total_File_Events = jsonEventsData.Count() , Failed_Events = jsonEventsData.Count() , Inseted_Events = 0 , Updated_Events = 0 }));

                insertedEventResultViewModel.Inserted_Events = createdEvents.Count();
                insertedEventResultViewModel.Failed_Events = insertedEventResultViewModel.Total_File_Events - (insertedEventResultViewModel.Inserted_Events + insertedEventResultViewModel.Updated_Events);

                // await _eventServ.UpdateEventsAddressFromGoogle(oldEvents.Where(q => updatedEvents.Contains(q.Id)).ToList());

                return StatusCode(StatusCodes.Status200OK, new ResponseModel<object>(StatusCodes.Status200OK, true, _localizer["Added"], insertedEventResultViewModel));

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<object>(StatusCodes.Status500InternalServerError, false, ex.Message, null));

            }
        }

        #endregion


        #region Help & Test Apis (For Development Purposes Only !!!) :)

        [AllowAnonymous]
        [HttpGet]
        [Route("Test")]
        public async Task<IActionResult> Test()
        {
            return Ok("Test ===> Work :) !!!");
        }

        [HttpGet]
        [Route("SendEmail")]
        public async Task<IActionResult> SendEmail([FromQuery] string email)
        {
            //  await _emailHelper.SendEmailregistration(email, "Test Email", "Your Email Sent !!!  Just For Test :) ", 123456, "www.google.com", "Test User");
            await _emailHelper.SendWelcomeEmail(email);
            return Ok("Email Sent");
        }

        [HttpGet]
        [Route("SendNotification/{FcmToken}")]
        public async Task<IActionResult> SendNotification(string FcmToken)
        {
            FireBaseData fireBaseInfo = new FireBaseData() { Title = "Test", Body = "Hello From Test Notification", imageUrl = "www.google.com", Action_code = "Test", muit = false, Action = "Test" };

            await _firebaseManager.SendNotification(FcmToken, fireBaseInfo);

            return Ok("Notification Sent");

        }


        [HttpPost]
        [Route("DeleteUser/{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            try
            {
                UserDetails userDetails = await _authContext.UserDetails.FirstOrDefaultAsync(q => q.UserId == userId);

                var result = await _authContext.Database.ExecuteSqlRawAsync($"EXEC deleteAccount @UserID_int = {userDetails.PrimaryId},@UserID_string = '{userDetails.UserId}'");

                return StatusCode(StatusCodes.Status200OK, result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);

            }
        }

        /// <summary>
        /// Date Format yyyy-mm-dd
        /// </summary>
        /// <param name="totalCount"></param>
        /// <param name="minDate"></param>
        /// <param name="maxDate"></param>
        /// <returns></returns>
        // for job
        [HttpPost]
        public async Task<IActionResult> AddExternalEvents(int totalCount, [FromForm] string?  minDate, [FromForm] string? maxDate)
        {
            var externalEvents = await _externalEventUtility.GetExternalEvents(totalCount,minDate,maxDate);
            var result = await this.ExportExternalEvents(externalEvents);
            return StatusCode(StatusCodes.Status200OK, new ResponseModel<object>(StatusCodes.Status200OK, true, _localizer["Added"], result));
        }

        
        // for web
        [Route("ExportExternalEvents")]
        [HttpPost]
        public async Task<IActionResult> ExportExternalEvents()
        {
            var externalEvents = await _externalEventUtility.GetEvents();
            var result = await this.ExportExternalEvents(externalEvents);
            return StatusCode(StatusCodes.Status200OK, result);
        }
       

        private async Task<InsertedEventResultViewModel> ExportExternalEvents(ExternalEventDataResponse externalEvents)
        {
            try
            {
                var id = await _eventTypeListService.GetEventTypeListId("External");
                var dataconfig = _appConfigrationService.GetData().FirstOrDefault();

                List<EventData> oldEvents = _eventServ.getallexternalevent();
                List<ExternalEventData> NegelectedEvents = new List<ExternalEventData>();
                List<int> updatedEvents = new List<int>();
                var eventMetaData = new EventMetaData();
                InsertedEventResultViewModel insertedEventResultViewModel = new InsertedEventResultViewModel();
                insertedEventResultViewModel.Total_File_Events = externalEvents.TotalCount;
                foreach (var evnt in externalEvents.ExternalEventData.ToList())
                {
                    EventData eventExists = oldEvents.FirstOrDefault(m => m.lat == evnt.venue.latitude && m.lang == evnt.venue.longitude && m.Title
                                            == evnt.eventname && m.eventdate == Convert.ToDateTime(evnt.date));

                    if (evnt.venue.longitude == "" || evnt.venue.longitude == null || evnt.venue.latitude == "" || evnt.venue.latitude == null)
                    {
                        NegelectedEvents.Add(evnt);
                        eventMetaData.EmptyCountEventlongitudeLatitude = NegelectedEvents.Where(evnt => evnt.venue.longitude == "" || evnt.venue.longitude == null || evnt.venue.latitude == "" || evnt.venue.latitude == null).Count();
                        externalEvents.ExternalEventData.Remove(evnt);
                        continue;
                    }
                    if (evnt.date == null || evnt.enddate == null)
                    {
                        NegelectedEvents.Add(evnt);
                        eventMetaData.EventDateOrEventDateToNull = NegelectedEvents.Where(evnt => evnt.date == null || evnt.enddate == null).Count();
                        externalEvents.ExternalEventData.Remove(evnt);
                        continue;
                    }
                    if (evnt.openingtimes.doorsopen == null || evnt.openingtimes.doorsclose == null)
                    {
                        NegelectedEvents.Add(evnt);
                        eventMetaData.EventTimeFromToNull = NegelectedEvents.Where(evnt => (evnt.openingtimes.doorsopen == null || evnt.openingtimes.doorsclose == null) && evnt.allday == false).Count();
                        externalEvents.ExternalEventData.Remove(evnt);
                        continue;
                    }

                    if (evnt.date > evnt.enddate)
                    {
                        NegelectedEvents.Add(evnt);
                        eventMetaData.EmptyCountEventDateOldThanEventDateto = NegelectedEvents.Where(evnt => evnt.date > evnt.enddate).Count();
                        externalEvents.ExternalEventData.Remove(evnt);
                        continue;
                    }
                    if (string.IsNullOrEmpty(evnt.eventname))
                    {
                        NegelectedEvents.Add(evnt);
                        eventMetaData.EmptyCountTitle = NegelectedEvents.Where(evnt => string.IsNullOrEmpty(evnt.eventname)).Count();
                        externalEvents.ExternalEventData.Remove(evnt);
                        continue;
                    }
                    if (string.IsNullOrEmpty(evnt.description) && eventExists == null)
                    {
                        NegelectedEvents.Add(evnt);
                        eventMetaData.EmptyCountDescription = NegelectedEvents.Where(evnt => string.IsNullOrEmpty(evnt.description)).Count();

                        evnt.description = "-";
                        continue;
                    }
                    if (dataconfig != null && dataconfig.EventTitle_MinLength != null)
                    {
                        if (evnt.eventname.Length >= dataconfig.EventTitle_MinLength && evnt.eventname.Length >= dataconfig.EventTitle_MaxLength)
                        {
                            NegelectedEvents.Add(evnt);
                            eventMetaData.EventTitleLengthLargerConfig = NegelectedEvents.Where(evnt => evnt.eventname.Length >= dataconfig.EventTitle_MinLength
                            && evnt.eventname.Length >= dataconfig.EventTitle_MaxLength).Count();
                            externalEvents.ExternalEventData.Remove(evnt);
                            continue;
                        }
                    }
                    if (dataconfig != null && dataconfig.EventDetailsDescription_MaxLength != null)
                    {
                        if (evnt.description.Length >= dataconfig.EventDetailsDescription_MinLength && evnt.description.Length >= dataconfig.EventDetailsDescription_MaxLength)
                        {
                            NegelectedEvents.Add(evnt);
                            eventMetaData.EventDescriptionLengthLargerConfig = NegelectedEvents.Where(evnt => evnt.description.Length >=
                            dataconfig.EventDetailsDescription_MinLength && evnt.description.Length >= dataconfig.EventDetailsDescription_MaxLength).Count();
                            externalEvents.ExternalEventData.Remove(evnt);
                            continue;
                        }
                    }
                    if (string.IsNullOrEmpty(evnt.link))
                    {
                        NegelectedEvents.Add(evnt);
                        eventMetaData.EventCheckoutDetailsNull = NegelectedEvents.Where(evnt => string.IsNullOrEmpty(evnt.link)).Count();
                        externalEvents.ExternalEventData.Remove(evnt);
                        continue;
                    }                    
                    if (eventExists != null)
                    {

                        updatedEvents.Add(eventExists.Id);
                        insertedEventResultViewModel.Updated_Events += 1;
                        externalEvents.ExternalEventData.Remove(evnt);
                        continue;
                    }

                }

                #endregion
                // Calculate All day 
                externalEvents = this.SetAllDayProperty(externalEvents);
                #region Insert Events In To Database 
                var urlExtention = "?sktag=15153";
                List<EventData> newEvents = externalEvents.ExternalEventData.Select(q => new EventData()
                {
                    Title = q.eventname,
                    eventtype = Guid.Parse("265583AA-2511-4FD3-883E-6CAF1F8E4355"),
                    image = q.xlargeimageurl,
                    description = q.description,
                    status = q.status,
                    categorieId = q.categorieId,
                    SubCategoriesIds = q.SubCategoriesIds,
                    EntityId = Guid.NewGuid().ToString(),
                    EventTypeListid = id,
                    lang = q.venue.longitude,
                    lat = q.venue.latitude,
                    allday = q.allday,
                    totalnumbert = q.goingtos == 0 ? 1000 : q.goingtos,
                    eventdate = q.date,
                    eventdateto = q.enddate,
                    eventfrom = q.openingtimes.doorsopen,
                    eventto = q.openingtimes.doorsclose,
                    CityID = q.CityID,
                    CountryID = q.CountryID,
                    StopTo = q.StopTo,
                    StopFrom = q.StopFrom,
                    CreatedDate = DateTime.Now,
                    checkout_details = q.link+urlExtention,
                    showAttendees = q.showAttendees,
                    IsActive = true,
                    UserId = 269,
                    EventTypeCode = EventTypes.Skiddle.ToString()

                }).ToList();                
                List<EventData> createdEvents = await _eventServ.InsertEvents(newEvents);

                List<EventChatAttend> eventChatAttends = newEvents.Select(q => new EventChatAttend() { EntityId = Guid.NewGuid().ToString(), Jointime = DateTime.Now.TimeOfDay, EventDataid = q.Id, UserattendId = q.UserId, JoinDate = DateTime.Now, ISAdmin = true }).ToList();

                List<EventChatAttend> newEventChatAttends = await _eventServ.InsertEventChatAttends(eventChatAttends);
                await _authContext.SaveChangesAsync();


                insertedEventResultViewModel.Inserted_Events = createdEvents.Count;
                #endregion

                insertedEventResultViewModel.EventMetaData = eventMetaData;
                if (createdEvents != null && createdEvents.Count() != 0)
                {
                    insertedEventResultViewModel.Inserted_Events = createdEvents.Count();
                    insertedEventResultViewModel.Failed_Events = (insertedEventResultViewModel.Inserted_Events + insertedEventResultViewModel.Updated_Events) == 0 ? (insertedEventResultViewModel.Total_File_Events) : (insertedEventResultViewModel.Total_File_Events - (insertedEventResultViewModel.Inserted_Events + insertedEventResultViewModel.Updated_Events));

                    await _eventServ.UpdateEventsAddressFromGoogle(createdEvents);

                    return insertedEventResultViewModel;
                    //return StatusCode(StatusCodes.Status200OK, new ResponseModel<object>(StatusCodes.Status200OK, true, _localizer["YourEventhasbeenAddedsuccessfully"], insertedEventResultViewModel));
                }

                insertedEventResultViewModel.Inserted_Events = createdEvents.Count();
                insertedEventResultViewModel.Failed_Events = insertedEventResultViewModel.Total_File_Events - (insertedEventResultViewModel.Inserted_Events + insertedEventResultViewModel.Updated_Events);
                return insertedEventResultViewModel;
                //return StatusCode(StatusCodes.Status200OK, new ResponseModel<object>(StatusCodes.Status200OK, true, _localizer["Added"], insertedEventResultViewModel));
            }


            catch (global::System.Exception ex)
            {
                throw ex;
            }

        }



        private ExternalEventDataResponse SetAllDayProperty(ExternalEventDataResponse externalEvents)
        {
            var externalEventData= new List<ExternalEventData>();
            foreach (var item in externalEvents.ExternalEventData)
            {
                if(item.startdate != null && item.enddate!= null && item.openingtimes.doorsopen != null && item.openingtimes.doorsclose != null)
                {
                    if(item.enddate.Subtract(item.startdate).Days <=1)
                    {
                        var totalHours = item.openingtimes.doorsclose.Subtract(item.openingtimes.doorsopen).TotalHours;
                        var totalHoursValue = totalHours < 0 ? totalHours += 24 : totalHours;
                        if (totalHoursValue >= 12)
                        {
                            item.allday = true;
                        }
                        else
                        {
                            item.allday = false;
                        }
                    }
                    else
                    {
                        item.allday = false;
                    }
                }
                else
                {
                    item.allday = false;
                }
                externalEventData.Add(item);
            }

            externalEvents.ExternalEventData = externalEventData;
            return externalEvents;
        }


        //private ExternalEventTaskMasterResponse SetAllDayPropertyForTicketMaster(ExternalEventTaskMasterResponse externalEvents)
        //{
        //    var externalEventData = new List<Event>();
        //    foreach (var item in externalEvents.ExternalEventDataForTciketMaster)
        //    {
        //        if (item.sales.presales.Where(x => x.startDateTime != null && x.endDateTime != null && item.ticketing.safeTix.enabled == true).Any() )
        //        {
        //            //ask
        //            if (item.sales.presales.FirstOrDefault().endDateTime.Subtract(item.sales.presales.FirstOrDefault().startDateTime).Days <= 1)
        //            {
        //                //var totalHours = item.openingtimes.doorsclose.Subtract(item.openingtimes.doorsopen).TotalHours;
        //                var totalHours = 7;
        //                var totalHoursValue = totalHours < 0 ? totalHours += 24 : totalHours;
        //                if (totalHoursValue >= 12)
        //                {
        //                    item.allday = true;
        //                }
        //                else
        //                {
        //                    item.allday = false;
        //                }
        //            }
        //            else
        //            {
        //                item.allday = false;
        //            }
        //        }
        //        else
        //        {
        //            item.allday = false;
        //        }
        //        externalEventData.Add(item);
        //    }

        //    externalEvents.ExternalEventDataForTciketMaster = externalEventData;
        //    return externalEvents;
        //}
    }
}
