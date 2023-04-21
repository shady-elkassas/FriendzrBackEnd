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
    public class TicketMasterController : ControllerBase
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
        public TicketMasterController(IUserService userService, IErrorLogService errorLogService, AuthDBContext authDBContext, IAppConfigrationService appConfigrationService,
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


        ///// <summary>
        ///// TicketMaster
        ///// </summary>
        ///// <param name="totalCount"></param>
        ///// <param name="minDate"></param>
        ///// <param name="maxDate"></param>
        ///// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddTicketMasterExternalEvents(int totalCount, [FromForm] string? minDate, [FromForm] string? maxDate)
        {
            var externalEvents = await _externalEventUtility.GetExternalTiceketMasterEvents(totalCount, minDate, maxDate);
            var result = await this.ExportExternalTicketMasterEvents(externalEvents);
            return StatusCode(StatusCodes.Status200OK, new ResponseModel<object>(StatusCodes.Status200OK, true, _localizer["Added"], result));
        }

        ///// <summary>
        ///// TicketMaster
        ///// </summary>
        ///// <returns></returns>
        [Route("ExportExternalTicketMasterEvents")]
        [HttpPost]
        public async Task<IActionResult> ExportExternalTicketMasterEvents()
        {
            var externalEvents = await _externalEventUtility.GetTicketmasterEvents();
            var result = await this.ExportExternalTicketMasterEvents(externalEvents);
            return StatusCode(StatusCodes.Status200OK, result);
        }

        private async Task<InsertedEventResultViewModel> ExportExternalTicketMasterEvents(ExternalEventTaskMasterResponse externalEvents)
        {
            try
            {
                var id = await _eventTypeListService.GetEventTypeListId("External");
                var dataconfig = _appConfigrationService.GetData().FirstOrDefault();

                List<EventData> oldEvents = _eventServ.getallexternalevent();
                List<Event> NegelectedEvents = new List<Event>();
                List<int> updatedEvents = new List<int>();
                var eventMetaData = new EventMetaData();
                InsertedEventResultViewModel insertedEventResultViewModel = new InsertedEventResultViewModel();
                insertedEventResultViewModel.Total_File_Events = externalEvents.TotalCount;
                //foreach (var evnt in externalEvents.ExternalEventDataForTciketMaster.ToList())
                //{
                //    //ask
                //    //EventData eventExists = oldEvents.FirstOrDefault(m => evnt._embedded.venues.FirstOrDefault(x => m.lat == x.location.latitude && 
                //    //m.lang == x.location.longitude && m.Title == x.name));  


                //    EventData eventExists = oldEvents.FirstOrDefault(m => m.lat == evnt._embedded.venues.Where(x => x.location.latitude != null).FirstOrDefault().location.latitude &&
                //    m.lang == evnt._embedded.venues.Where(x => x.location.latitude != null).FirstOrDefault().location.longitude &&
                //    m.Title == evnt.name);

                //    if (evnt._embedded.venues.Where(x => x.location.latitude != null).FirstOrDefault().location.longitude == "" || evnt._embedded.venues.Where(x => x.location.latitude != null).FirstOrDefault().location.longitude == null || evnt._embedded.venues.Where(x => x.location.latitude != null).FirstOrDefault().location.latitude == "" || evnt._embedded.venues.Where(x => x.location.latitude != null).FirstOrDefault().location.latitude == null)
                //    {
                //        NegelectedEvents.Add(evnt);
                //        eventMetaData.EmptyCountEventlongitudeLatitude = NegelectedEvents.Where(evnt => evnt._embedded.venues.Where(x => x.location.latitude != null).FirstOrDefault().location.longitude == "" || evnt._embedded.venues.Where(x => x.location.latitude != null).FirstOrDefault().location.longitude == null || evnt._embedded.venues.Where(x => x.location.latitude != null).FirstOrDefault().location.latitude == "" || evnt._embedded.venues.Where(x => x.location.latitude != null).FirstOrDefault().location.latitude == null).Count();
                //        externalEvents.ExternalEventDataForTciketMaster.Remove(evnt);
                //        continue;
                //    }
                //    var starttime = evnt.sales.presales.Where(x => x.startDateTime == null).Any();
                //    var endTime = evnt.sales.presales.Where(x => x.endDateTime == null).Any();


                //    var startandEndTob = evnt.sales.presales.FirstOrDefault();

                //    if (starttime || endTime)
                //    {
                //        NegelectedEvents.Add(evnt);
                //        eventMetaData.EventDateOrEventDateToNull = NegelectedEvents.Where(evnt => startandEndTob.startDateTime == null || startandEndTob.endDateTime == null).Count();
                //        externalEvents.ExternalEventDataForTciketMaster.Remove(evnt);
                //        continue;
                //    }
                //    //if (evnt.openingtimes.doorsopen == null || evnt.openingtimes.doorsclose == null)
                //    //{
                //    //    NegelectedEvents.Add(evnt);
                //    //    eventMetaData.EventTimeFromToNull = NegelectedEvents.Where(evnt => (evnt.openingtimes.doorsopen == null || evnt.openingtimes.doorsclose == null) && evnt.allday == false).Count();
                //    //    externalEvents.ExternalEventDataForTciketMaster.Remove(evnt);
                //    //    continue;
                //    //}

                //    if (startandEndTob.startDateTime > startandEndTob.endDateTime)
                //    {
                //        NegelectedEvents.Add(evnt);
                //        eventMetaData.EmptyCountEventDateOldThanEventDateto = NegelectedEvents.Where(evnt => startandEndTob.startDateTime > startandEndTob.endDateTime).Count();
                //        externalEvents.ExternalEventDataForTciketMaster.Remove(evnt);
                //        continue;
                //    }
                //    if (string.IsNullOrEmpty(evnt.name))
                //    {
                //        NegelectedEvents.Add(evnt);
                //        eventMetaData.EmptyCountTitle = NegelectedEvents.Where(evnt => string.IsNullOrEmpty(evnt.name)).Count();
                //        externalEvents.ExternalEventDataForTciketMaster.Remove(evnt);
                //        continue;
                //    }
                //    if (string.IsNullOrEmpty(evnt.promoter.description) && eventExists == null)
                //    {
                //        NegelectedEvents.Add(evnt);
                //        eventMetaData.EmptyCountDescription = NegelectedEvents.Where(evnt => string.IsNullOrEmpty(evnt.promoter.description)).Count();

                //        evnt.promoter.description = "-";
                //        continue;
                //    }
                //    if (dataconfig != null && dataconfig.EventTitle_MinLength != null)
                //    {
                //        if (evnt.name.Length >= dataconfig.EventTitle_MinLength && evnt.name.Length >= dataconfig.EventTitle_MaxLength)
                //        {
                //            NegelectedEvents.Add(evnt);
                //            eventMetaData.EventTitleLengthLargerConfig = NegelectedEvents.Where(evnt => evnt.name.Length >= dataconfig.EventTitle_MinLength
                //            && evnt.name.Length >= dataconfig.EventTitle_MaxLength).Count();
                //            externalEvents.ExternalEventDataForTciketMaster.Remove(evnt);
                //            continue;
                //        }
                //    }
                //    if (dataconfig != null && dataconfig.EventDetailsDescription_MaxLength != null)
                //    {
                //        if (evnt.promoter.description.Length >= dataconfig.EventDetailsDescription_MinLength && evnt.promoter.description.Length >= dataconfig.EventDetailsDescription_MaxLength)
                //        {
                //            NegelectedEvents.Add(evnt);
                //            eventMetaData.EventDescriptionLengthLargerConfig = NegelectedEvents.Where(evnt => evnt.promoter.description.Length >=
                //            dataconfig.EventDetailsDescription_MinLength && evnt.promoter.description.Length >= dataconfig.EventDetailsDescription_MaxLength).Count();
                //            externalEvents.ExternalEventDataForTciketMaster.Remove(evnt);
                //            continue;
                //        }
                //    }
                //    if (string.IsNullOrEmpty(evnt.url))
                //    {
                //        NegelectedEvents.Add(evnt);
                //        eventMetaData.EventCheckoutDetailsNull = NegelectedEvents.Where(evnt => string.IsNullOrEmpty(evnt.url)).Count();
                //        externalEvents.ExternalEventDataForTciketMaster.Remove(evnt);
                //        continue;
                //    }
                //    if (eventExists != null)
                //    {

                //        updatedEvents.Add(eventExists.Id);
                //        insertedEventResultViewModel.Updated_Events += 1;
                //        externalEvents.ExternalEventDataForTciketMaster.Remove(evnt);
                //        continue;
                //    }

                //}

                //#endregion
                // Calculate All day 
                //externalEvents = this.SetAllDayProperty(externalEvents);
                #region Insert Events In To Database 
                var urlExtention = "?sktag=15153";
                List<EventData> newEvents = externalEvents.ExternalEventDataForTciketMaster.Select(q => new EventData()
                {
                    //EventId = q.id,
                    Title = q.name,
                    eventtype = Guid.Parse("265583AA-2511-4FD3-883E-6CAF1F8E4355"),
                    image = q.images.FirstOrDefault().url,
                    description = q.promoters.FirstOrDefault().description,
                    status = q.dates.status.code,
                    // categorieId = q.categorieId,
                    //type of the event is presented in the same table
                    categorieId =  _eventServ.ExtractEventCategory(q.classifications.FirstOrDefault().segment.id.Trim()),
                    //CategoryName = q.classifications.FirstOrDefault().segment.name,
                    EntityId = Guid.NewGuid().ToString(),
                    EventTypeListid =id ,
                    lang = q._embedded.venues.FirstOrDefault().location.longitude,
                    lat = q._embedded.venues.FirstOrDefault().location.latitude,
                    //no need for alldays
                    allday = false,

                    ///get page total Number in a seperated function
                    totalnumbert = 1000,
                    eventdate = q.dates.start.dateTime,
                    eventdateto = q.dates.start.dateTime.AddHours(10),
                    //
                    eventfrom = q.dates.start.dateTime.TimeOfDay,
                    eventto = q.dates.start.dateTime.AddHours(10).TimeOfDay,
                    //city name 
                    //CityName = q._embedded.venues.FirstOrDefault().city.name,
                    ////country name
                    // CountryName = q._embedded.venues.FirstOrDefault().country.name,
                    ////ask city and country ID
                    //CityID = q.CityID,
                    //CountryID = q.CountryID,
                    //StopTo = q.StopTo,
                    //StopFrom = q.StopFrom,
                    CreatedDate = DateTime.Now,
                    checkout_details = q._embedded.venues.FirstOrDefault() + urlExtention,
                   // showAttendees = q._embedded.venues.FirstOrDefault(x=> x.postalCode.),
                    IsActive = true,
                    UserId = 269,
                    EventTypeCode = EventTypes.TicketMaster.ToString()

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
                    //to save in the city and country
                    //wont be able to save in the city or country table
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


    }
}
