using GoogleMaps.LocationServices;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Social.Entity.Common;
using Social.Entity.DBContext;
using Social.Entity.Models;
using Social.Entity.ModelView;
using Social.Services.Helpers;
using Social.Services.ModelView;
using Social.Services.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Social.Services.Implementation
{
    public class EventServ : IEventServ
    {
        private readonly AuthDBContext _authContext;
        private readonly IConfiguration _configuration;
        private readonly IGoogleLocationService googleLocationService;
        private readonly ICountryService countryService;
        private readonly ICityService cityService;
        private readonly IStringLocalizer<SharedResource> localizer;
        private readonly IHttpContextAccessor httpContextAccessor;
        //private readonly IUserService userService;        
        public EventServ(AuthDBContext authContext, IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration,
            IGoogleLocationService googleLocationService,
            ICountryService countryService,
            ICityService cityService,
            IStringLocalizer<SharedResource> _localizer)
        {
            this.httpContextAccessor = httpContextAccessor;
            this._configuration = configuration;
            this.googleLocationService = googleLocationService;
            this.countryService = countryService;
            this.cityService = cityService;
            this._authContext = authContext;
            localizer = _localizer;            
        }

        //public async Task deleteEvent(string id)
        //{
        //    try
        //    {
        //        var EventData = _authContext.EventData.FirstOrDefault(x => x.EntityId == id);
        //        _authContext.Messagedata.RemoveRange(EventData.Messagedata);
        //        _authContext.SaveChanges();
        //        _authContext.EventChatAttend.RemoveRange(EventData.EventChatAttend);
        //        _authContext.SaveChanges();
        //        _authContext.EventReports.RemoveRange(EventData.EventReports);
        //        _authContext.SaveChanges();
        //        _authContext.EventData.Remove(EventData);
        //        _authContext.SaveChanges();

        //        var notif = _authContext.FireBaseDatamodel.Where(n => n.Action_code == EventData.EntityId).ToList();
        //        _authContext.FireBaseDatamodel.RemoveRange(notif);
        //        await _authContext.SaveChangesAsync();
        //        _authContext.DeletedEventLogs.Add(new DeletedEventLog()
        //        {
        //            DateTime = DateTime.UtcNow,
        //            latitude = EventData.lat,
        //            longitude = EventData.lang,
        //            EventCategoryID = EventData.categorieId ?? 0,
        //            EventDataJson = JsonConvert.SerializeObject(new { UserID = EventData.Id, EventData.Title })
        //        });
        //        _authContext.SaveChanges();
        //    }
        //    catch (Exception ex)
        //    {
        //        var a = ex;
        //    }
        //}

        //Commenten By Saeed

        public async Task deleteEvent(string id)
        {
            try
            {
                // var eventchat = this._authContext.EventChat.Include(m => m.EventData).Where(r => r.EventData.EntityId == id).FirstOrDefault();
                var EventData = GetEventbyid(id);
                var EventChatAttend = this._authContext.EventChatAttend.Include(m => m.EventData).Where(r => r.EventData.EntityId == id).FirstOrDefault();
                var eventmessagechat = this._authContext.Messagedata.Where(r => r.EventChatAttend.EventData.EntityId == id || r.EventData.EntityId == id).ToList();
                var EventReports = this._authContext.EventReports.Where(r => r.EventData.EntityId == id);
                List<EventTracker> eventTrackers = await _authContext.EventTrackers.Where(q => q.EventId == EventData.Id).ToListAsync(); ;

                _authContext.EventTrackers.RemoveRange(eventTrackers);
                _authContext.EventReports.RemoveRange(EventReports);
                _authContext.SaveChanges();
                _authContext.Messagedata.RemoveRange(eventmessagechat);
                _authContext.SaveChanges();
                if (EventChatAttend != null)
                    _authContext.EventChatAttend.Remove(EventChatAttend);
                _authContext.SaveChanges();
                _authContext.SaveChanges();
                _authContext.EventData.Remove(EventData);
                await _authContext.SaveChangesAsync();
                var notif = _authContext.FireBaseDatamodel.Where(n => n.Action_code == EventData.EntityId).ToList();
                _authContext.FireBaseDatamodel.RemoveRange(notif);
                await _authContext.SaveChangesAsync();
                _authContext.DeletedEventLogs.Add(new DeletedEventLog()
                {
                    DateTime = DateTime.UtcNow,
                    latitude = EventData.lat,
                    longitude = EventData.lang,
                    EventCategoryID = EventData.categorieId ?? 0,
                    EventDataJson = JsonConvert.SerializeObject(new { UserID = EventData.Id, EventData.Title })
                });
                _authContext.SaveChanges();
            }
            catch { }
        }

        public async Task deleteInterests(string id)
        {
            var interste = getInterests(id);
            var interstid = interste.Id;
            _authContext.listoftags.RemoveRange(_authContext.listoftags.Where(x => x.InterestsId == interstid));

            _authContext.Interests.Remove(interste);
            await _authContext.SaveChangesAsync();
        }
        public int GetEventattend(string id, List<EventChatAttend> eventattend)
        {
            return eventattend.Where(n => n.EventData.EntityId == id && n.stutus != 1 && n.stutus != 2).Count();
        }

        public int GetEventattend(string id)
        {
            var data = this._authContext.EventChatAttend.Where(n => n.EventData.EntityId == id && n.stutus != 1 && n.stutus != 2).ToList();

            return (data.Count());
        }

        public (int count, bool type) GetEventattend(List<EventChatAttend> eventattend, string id, int userid)
        {
            var data = eventattend.Where(n => n.EventData.EntityId == id && n.stutus != 1 && n.stutus != 2).ToList();
            bool type = data.FirstOrDefault(m => m.UserattendId == userid) == null ? false : true;
            return ((data.Count(), true));
        }
        public List<interlistdata> GetEventattendstat(string id)
        {
            var data = this._authContext.EventChatAttend.Include(m => m.Userattend)
                .Where(n => n.EventData.EntityId == id && n.stutus != 1 && n.stutus != 2).ToList();

            var count = data.Select(m => m.Userattend.PrimaryId).ToList();
            var allinters = this._authContext.listoftags.Include(m => m.Interests).ToList()
                .Where(m => count.Contains(Convert.ToInt32(m.UserId))).ToList();
            var datalist = allinters.Count();
            var returndata = allinters.GroupBy(m => m.InterestsId)
            .Select(m => new interlistdata
            {
                Count = ((m.Where(x => data.Any(xx => xx.UserattendId != null && xx.UserattendId == x.UserId)).Count() * 100 / data.Count)),
                name = m.FirstOrDefault().Interests.name
            }).OrderByDescending(m => m.Count).Take(3).ToList();
            if (returndata.Count < 3)
            {
                if (returndata.Count == 2)
                {
                    returndata.Add(new interlistdata { Count = 0, name = "" });
                }
                if (returndata.Count == 1)
                {
                    returndata.Add(new interlistdata { Count = 0, name = "" });
                    returndata.Add(new interlistdata { Count = 0, name = "" });
                }
                if (returndata.Count == 0)
                {

                    returndata.Add(new interlistdata { Count = 0, name = "" });
                    returndata.Add(new interlistdata { Count = 0, name = "" });
                    returndata.Add(new interlistdata { Count = 0, name = "" });
                }
            }
            returndata = (returndata.OrderByDescending(m => m.Count).ToList());
            return returndata;
        }
        public List<listdata> GetEventattendgender(string id)
        {
            var data = this._authContext.EventChatAttend.Include(m => m.Userattend).Where(n => n.EventData.EntityId == id && n.stutus != 1 && n.stutus != 2).ToList();
            var count = data.Select(m => m.Userattend).Where(m => m.Gender != null).ToList();

            var mal = count.Where(m => m.Gender.ToString().ToLower() == ("male".ToLower())).ToList();
            var Femal = count.Where(m => m.Gender.ToString().ToLower().Contains("Femal".ToLower())).ToList();
            var malFemal = count.Where(m => !(m.Gender.ToString().ToLower().Contains("mal".ToLower())) && !(m.Gender.ToString().ToLower().Contains("Femal".ToLower()))).ToList();
            var Male = mal.Count() == 0 ? 0 : ((mal.Count * 100 / count.Count()));
            var Female = Femal.Count() == 0 ? 0 : ((Femal.Count * 100 / count.Count()));
            var other = malFemal.Count() == 0 ? 0 : ((malFemal.Count * 100 / count.Count()));
            List<listdata> listdata = new List<listdata>();
            listdata.Add(new listdata { Count = Male, Key = "Male" });
            listdata.Add(new listdata { Count = Female, Key = "Female" });
            listdata.Add(new listdata { Count = other, Key = "Other Gender" });
            return (listdata.ToList());
        }
        public class listdata
        {
            public int Count { get; set; }
            public string Key { get; set; }
        }
        public class interlistdata
        {
            public int Count { get; set; }
            public string name { get; set; }
        }
        public IEnumerable<EventChatAttend> getattendevent(string id, String userid)
        {
            var data = this._authContext.EventChatAttend.Where(n => (n.EventData.UserId != null ? (n.EventData.User.UserId == userid) : true) && n.EventData.EntityId == id && n.stutus != 1 && n.stutus != 2).Distinct().ToList();

            return (data.ToList());
        }
        public IEnumerable<EventChatAttend> getattendevent(string id)
        {
            var data = this._authContext.EventChatAttend.Where(n => n.EventData.EntityId == id).Distinct().ToList();

            return (data.ToList());
        }      
        public bool priveteventlink(string id, String userid)
        {
            var datalist = this._authContext.EventChatAttend.Where(n => n.EventData.EntityId == id).ToList();
            var data = datalist.Where(n => (n.EventData.EventTypeList?.key == true) ? (n.Userattend.UserId == userid && n.stutus != 1 && n.stutus != 2) : true);

            return (data.FirstOrDefault() == null ? false : true);
        }
        public bool priveteventlink(string id)
        {
            var datalist = this._authContext.EventChatAttend.Where(n => n.EventData.EntityId == id).ToList();
            var data = datalist.Where(n => n.EventData.EventTypeList?.key == true).FirstOrDefault();

            return (data == null ? false : true);
        }
        public List<EventChatAttend> getEventChatAttend(string id, String userid)
        {
            var data = this._authContext.EventChatAttend.Where(k => k.EventData.EntityId == id);
            var booda = data.FirstOrDefault(m => m.Userattend.UserId == userid && m.stutus != 1 && m.stutus != 2) == null ? false : true;
            data = data.Where(n => (n.EventData.UserId != null ? (n.EventData.User.UserId == userid || (n.EventData.showAttendees == true ? (booda) : false)) : true) && n.stutus != 1 && n.stutus != 2).Distinct();

            return (data.ToList());
        }


        public IEnumerable<EventChatAttend> allattendevent()
        {
            var data = this._authContext.EventChatAttend.Include(e=>e.EventData);
            return (data);
        }

        public IEnumerable<EventChatAttend> GetValidChatAttends(int eventId)            
        {
            var data = this._authContext.EventChatAttend.Where(e => e.EventDataid == eventId && e.stutus != 1 && e.stutus != 2).ToList();
            return (data);
        }

        public List<EventChatAttend> allEventChatAttend()
        {
            var data = this._authContext.EventChatAttend.Include(q => q.EventData);
            return (data.ToList());
        }


        public List<EventChatAttend> AllEventChatAttendByEventId(string eventid)
        {
            var data = this._authContext.EventChatAttend.Include(q => q.EventData).Where(q => q.EventData.EntityId == eventid);
            return (data.ToList());
        }

        public IEnumerable<EventChatAttend> allattendevent(int id, string Search = null)
        {
            var data = this._authContext.EventChatAttend.Where(m => m.EventData.eventdateto.Value.Date.AddDays(5) >= DateTime.Now.Date && (Search != null ? m.EventData.Title.Contains(Search) : true) && ((m.EventData.UserId == id && m.stutus != 1 && m.stutus != 2 && m.leave != true) || (m.EventData.UserId != id && m.UserattendId == id && m.stutus != 1 && m.stutus != 2 && m.leave != true))).ToList();
            ;
            return (data);
        }
        public List<EventVM> getallattendevent(IEnumerable<EventChatAttend> allateend, int PrimaryId, List<EventData> pagedModel)
        {

            var redata = pagedModel.Select(m => new EventVM
            {

                description = m.description,
                categorie = m.categorie?.name,
                categorieimage = _configuration["BaseUrl"] + m.categorie?.image,

                lat = m.lat,
                lang = m.lang,
                Id = m.EntityId,
                OrderByDes = m.Id,
                eventdate = m.eventdate.Value.ConvertDateTimeToString(),

                eventdateto = m.eventdateto.Value.ConvertDateTimeToString(),
                allday = Convert.ToBoolean(m.allday),
                timefrom = m.eventfrom == null ? "" : m.eventfrom.Value.ToString(@"hh\:mm"),
                timeto = m.eventto == null ? "" : m.eventto.Value.ToString(@"hh\:mm"),
                Title = m.Title,
                joined = allateend.Where(n => n.EventData.EntityId == m.EntityId && n.stutus != 1 && n.stutus != 2).Count(),
                image = (m.EventTypeListid != 3 ? _configuration["BaseUrl"] : "") + m.image,
                totalnumbert = m.totalnumbert,
                key = m.UserId == PrimaryId ? 1 : (allateend.Where(n => n.EventData.EntityId == m.EntityId).Where(b => b.UserattendId == PrimaryId).Where(m => m.stutus != 1 && m.stutus != 2).FirstOrDefault() == null ? 2 : 3),
                ////bloked =(m.UserId == PrimaryId ?  false: (allateend.Where(n => n.EventData.EntityId == m.EntityId && n.UserattendId == PrimaryId && n.stutus == 2) == null ? false : true))

            }).ToList();

            return (redata);
        }

        public List<EventChatAttend> getallEventChatAttend(string id, IEnumerable<EventChatAttend> data)
        {
            data = data.Where(n => n.EventData.EntityId == id && n.stutus != 1 && n.stutus != 2).Distinct().ToList();

            return (data.ToList());
        }
        public List<EventChatAttend> getallChatattendevent(string id, IEnumerable<EventChatAttend> data)
        {
            data = data.Where(n => n.EventData.EntityId == id && n.stutus != 1 && n.stutus != 2 && n.stutus != 3).Distinct().ToList();
            return (data.ToList());
        }
        public List<EventChatAttend> getalluserevent(int id, List<EventChatAttend> data)
        {
            data = data.Where(n => n.UserattendId == id && n.stutus != 1 && n.stutus != 2 && n.stutus != 3 && n.EventData.IsActive == true && (n.EventData.StopFrom != null ? (n.EventData.StopFrom.Value >= DateTime.Now.Date || n.EventData.StopTo.Value <= DateTime.Now.Date) : true)).Distinct().ToList();
            return (data.ToList());
        }
        public List<EventChatAttend> getallEventChatAttend(string id)
        {
            var data = this._authContext.EventChatAttend.Include(m => m.Userattend).Include(m => m.Userattend.User).Where(n => n.EventData.EntityId == id && n.stutus != 1 && n.stutus != 2 && n.stutus != 3).Distinct();

            return (data.ToList());
        }
        public EventChatAttend GetuserEvent(string id, string userid)
        {
            var data = this._authContext.EventChatAttend.Where(n => n.EventData.EntityId == id && n.Userattend.UserId == userid).FirstOrDefault();

            return (data);
        }

        public EventChatAttend GetEventChatAttend(string id, string userid, bool message = false)
        {
            var data = this._authContext.EventChatAttend.Where(n => n.EventData.EntityId == id && n.Userattend.UserId == userid).FirstOrDefault();
            if (message && data != null)
            {

                data.UserNotreadcount = 0;

                _authContext.SaveChanges();
            }
            else
            {

            }
            return (data);
        }
        public EventData GetEventbyid(string id)
        {
            var data = this._authContext.EventData.FirstOrDefault(m => m.EntityId == id);
            return (data);
        }
        public EventData GeteventbyPrimaryId(string id)
        {
            var data = this._authContext.EventData.FirstOrDefault(m => m.Id == int.Parse(id));
            return (data);
        }

        public EventChatAttend GetEventChatAttendbyid(string id)
        {
            var data = this._authContext.EventChatAttend.FirstOrDefault(x => x.EntityId == id);
            return (data);
        }
        public category Getcategorybyid(string id)
        {
            var data = this._authContext.category.Where(m => m.EntityId == id).FirstOrDefault();

            return (data);
        }
        public List<Interests> getInterests()
        {
            var data = this._authContext.Interests.ToList();
            return (data);
        }
        public List<EventColor> getEventColor()
        {
            var data = this._authContext.EventColor.ToList();
            return (data);
        }
        public Interests getInterests(string id)
        {
            var data = this._authContext.Interests.Where(m => m.EntityId == id).FirstOrDefault();

            return (data);
        }
        public WhatBestDescripsMe getWhatBestDescripsMe(string id)
        {
            var data = this._authContext.WhatBestDescripsMe.Where(m => m.EntityId == id).FirstOrDefault();

            return (data);
        }
        public preferto getpreferto(string id)
        {
            var data = this._authContext.preferto.Where(m => m.EntityId == id).FirstOrDefault();

            return (data);
        }
        public async Task<EventData> InsertEvent(EventData eventData)
        {
            eventData.IsActive = true;
            _authContext.EventData.Add(eventData);
            await UpdateEventAddressFromGoogle(eventData);
            await _authContext.SaveChangesAsync();
            return eventData;
        }
        public async Task InsertEventColor(EventColor code)
        {
            // code.EntityId = Guid.NewGuid().ToString();
            _authContext.EventColor.Add(code);
            await _authContext.SaveChangesAsync();
        }


        public async Task<EventChatAttend> InsertEventChatAttend(EventChatAttend eventChatAttend)
        {
            dynamic result = null;
            try
            {
                eventChatAttend.EntityId = Guid.NewGuid().ToString();
                 result = await _authContext.EventChatAttend.AddAsync(eventChatAttend);
                eventChatAttend.Id = result.Entity.Id;
                await _authContext.SaveChangesAsync();
                
            }
            catch (Exception ex)
            {

            }
            return (eventChatAttend);
        }

        public bool CHECKChatAttend(EventChatAttend code)
        {

            var BOOLCHECK = _authContext.EventChatAttend.FirstOrDefault(M => M.EventDataid == code.EventDataid && M.UserattendId == code.UserattendId);

            return (BOOLCHECK == null ? true : false);
        }
        public async Task editeEventChatAttend(EventChatAttend code)
        {
            code.UserNotreadcount = 0;
            _authContext.EventChatAttend.Update(code);
            await _authContext.SaveChangesAsync();
        }
        public async Task InsertInterests(Interests code)
        {
            code.EntityId = Guid.NewGuid().ToString();
            _authContext.Interests.Add(code);
            await _authContext.SaveChangesAsync();
        }

        public async Task updateEvent(EventData eventData)
        {
            _authContext.EventData.Update(eventData);
            await UpdateEventAddressFromGoogle(eventData);
            await _authContext.SaveChangesAsync();

        }

        public async Task updateInterests(Interests code)
        {
            _authContext.Interests.Update(code);
            await _authContext.SaveChangesAsync();
        }

        public List<EventData> getmyevent(string id)
        {
            var data = this._authContext.EventData.Where(n => n.UserId == null ? true : n.User.Id == id && n.IsActive == true && (n.StopFrom != null ? (n.StopFrom.Value >= DateTime.Now.Date || n.StopTo.Value <= DateTime.Now.Date) : true)).ToList();

            return (data);
        }

        public List<EventData> getallevent(int id)
        {
            var allblock = this._authContext.EventChatAttend;
            var blockod = allblock.Where(m => (m.UserattendId == id) && m.stutus == 2).Select(m => m.EventDataid).ToList();
            var data = this._authContext.EventData.Where(m => m.IsActive == true && (m.StopFrom != null ? (m.StopFrom.Value >= DateTime.Now.Date || m.StopTo.Value <= DateTime.Now.Date) : true)).ToList();
            data = data.Where(n => !blockod.Contains(n.Id)).ToList();
            return (data);
        }
        public List<EventData> getallexternalevent()
        {
            var data = this._authContext.EventData.Where(m => m.IsActive == true && m.EventTypeListid == 3 && (m.StopFrom != null ? (m.StopFrom.Value >= DateTime.Now.Date || m.StopTo.Value <= DateTime.Now.Date) : true)).ToList();

            return (data);
        }
        public List<EventData> getallevent()
        {
            var data = this._authContext.EventData.Where(m => m.IsActive == true && m.EventTypeListid != 3 && (m.StopFrom != null ? (m.StopFrom.Value >= DateTime.Now.Date || m.StopTo.Value <= DateTime.Now.Date) : true)).ToList();

            return (data);
        }
        public List<EventData> getalluserevent(int id)
        {
            var data = this._authContext.EventChatAttend.Include(m => m.EventData).Include(m => m.EventData.categorie).Where(m => (m.EventData.UserId == id && m.stutus != 1 && m.stutus != 2) || (m.EventData.UserId != id && m.UserattendId == id && m.stutus != 1 && m.stutus != 2)).Select(m => m.EventData).Include(m => m.categorie).Distinct().ToList();

            return (data);
        }
        public List<category> getcategory()
        {
            var data = this._authContext.category.Where(M => M.IsActive != false).ToList();

            return (data);
        }
        public List<EventData> getevent(string id)
        {
            var data = this._authContext.EventData.Where(m => m.EntityId == id && m.IsActive == true && (m.StopFrom != null ? (m.StopFrom.Value >= DateTime.Now.Date || m.StopTo.Value <= DateTime.Now.Date) : true)).ToList();

            return (data);
        }
        public List<EventData> geteventbylocation(string lang, string lat)
        {
            var data = this._authContext.EventChatAttend.Where(m => m.EventData.lang == lang && m.EventData.lat == lat && m.EventData.IsActive == true && (m.EventData.StopFrom != null ? (m.EventData.StopFrom.Value >= DateTime.Now.Date || m.EventData.StopTo.Value <= DateTime.Now.Date) : true)).Where(m => m.EventData.eventdateto.Value.Date >= DateTime.Now.Date).Select(m => m.EventData).ToList();

            return (data);
        }
        public peoplocationDataMV getUserDetailsbylocation(string lang, string lat)
        {
            var count = this._authContext.UserDetails.Where(m => m.lang == lang && m.lat == lat && m.Gender != null).ToList();
            peoplocationDataMV Eventlocation = new peoplocationDataMV();

            var color = _authContext.EventColor.FirstOrDefault();
            if (count.Count != 0)
            {
                Eventlocation.lang = Convert.ToDecimal(lang);
                Eventlocation.lat = Convert.ToDecimal(lat);
                var mal = count.Where(m => m.Gender.ToString().ToLower() == ("male".ToLower())).ToList();
                var Femal = count.Where(m => m.Gender.ToString().ToLower().Contains("Femal".ToLower())).ToList();
                var malFemal = count.Where(m => !(m.Gender.ToString().ToLower().Contains("mal".ToLower())) && !(m.Gender.ToString().ToLower().Contains("Femal".ToLower()))).ToList();
                Eventlocation.MalePercentage = mal.Count() == 0 ? 0 : ((mal.Count * 100 / count.Count()));
                Eventlocation.Femalepercentage = Femal.Count() == 0 ? 0 : ((Femal.Count * 100 / count.Count()));
                Eventlocation.otherpercentage = malFemal.Count() == 0 ? 0 : ((malFemal.Count * 100 / count.Count()));
                Eventlocation.totalUsers = count.Count();
                Eventlocation.color = count == null ? count.Count() < 5 ? "#0BBEA1" : (count.Count() < 10 ? "#e7b416" : "#cc3232")
                    : count.Count() < color.emptynumber ? color.emptycolor : (count.Count() < color.middlenumber ? color.middlecolor : color.crowdedcolor);
                ;
            }
            return (Eventlocation);




        }

        public List<EventlocationDataMV> getAlleventlocation()
        {
            var data = this._authContext.EventData.ToList().Where(m => m.eventdateto.Value.Date >= DateTime.Now.Date && m.IsActive == true && (m.StopFrom != null ? (m.StopFrom.Value >= DateTime.Now.Date || m.StopTo.Value <= DateTime.Now.Date) : true)).ToList();
            var location = data.Select(n => new { n.lang, n.lat }).ToList().Distinct();
            List<EventlocationDataMV> list = new List<EventlocationDataMV>();
            foreach (var item in location)
            {
                EventlocationDataMV Eventlocation = new EventlocationDataMV();

                var events = data.Where(m => m.lang == item.lang && m.lat == item.lat).ToList();
                Eventlocation.lang = Convert.ToDecimal(item.lang);
                Eventlocation.lat = Convert.ToDecimal(item.lat);
                Eventlocation.EventData = events.Select(m => new EventDataMV
                {
                    eventdate = m.eventdate.Value.ConvertDateTimeToString(),

                    eventdateto = m.eventdateto.Value.ConvertDateTimeToString(),
                    allday = Convert.ToBoolean(m.allday),
                    description = m.description,
                    timefrom = m.eventfrom == null ? "" : m.eventfrom.Value.ToString(),
                    timeto = m.eventto == null ? "" : m.eventto.Value.ToString(),
                    category = m.categorie?.name,
                    categorieimage = _configuration["BaseUrl"] + m.categorie?.image,

                    categorieId = (m.categorie == null ? "" : m.categorie.EntityId),
                    Title = m.Title,
                    Image = (m.EventTypeListid != 3 ? _configuration["BaseUrl"] : "") + m.image,
                    Id = m.EntityId,

                    totalnumbert = m.totalnumbert,
                }).ToList();
                Eventlocation.count = events.Count();
                list.Add(Eventlocation);
            }
            return (list);
        }
        //    public bool displayLocation(double Latitude, double Longitude, double lat, double lang)

        //        {
        //        var geocoder;
        //        geocoder = new google.maps.Geocoder();
        //        var latlng = new google.maps.LatLng(latitude, longitude);

        //        geocoder.geocode(
        //    { 'latLng': latlng}, 
        //    function(results, status) {
        //            if (status == google.maps.GeocoderStatus.OK)
        //            {
        //                if (results[0])
        //                {
        //                    var add = results[0].formatted_address;
        //                    var value = add.split(",");

        //                    count = value.length;
        //                    country = value[count - 1];
        //                    state = value[count - 2];
        //                    city = value[count - 3];
        //                    x.innerHTML = "city name is: " + city;
        //                }
        //                else
        //                {
        //                    x.innerHTML = "address not found";
        //                }
        //            }
        //            else
        //            {
        //                x.innerHTML = "Geocoder failed due to: " + status;
        //            }
        //        }
        //);
        //    }
        public double CalculateDistance(double Latitude, double Longitude, double lat, double lang)
        {
            var d1 = Latitude * (Math.PI / 180.0);
            var num1 = Longitude * (Math.PI / 180.0);
            var d2 = lat * (Math.PI / 180.0);
            var num2 = lang * (Math.PI / 180.0) - num1;
            var d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) +
                     Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);
            var daa = 6376500.0 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));
            return daa;
        }
        public async Task<(List<EventVM>, int totalRowCount)> getAlleventlocation_2(int id, double myLat, double myLon, double dis, string Gender, UserDetails user, AppConfigrationVM AppConfigrationVM, string categories, int pageNumber, int pageSize)
        {
            int distance = ((AppConfigrationVM.DistanceShowNearbyEvents_Min == null ? 0 : (int)AppConfigrationVM.DistanceShowNearbyEvents_Min) * 1000);
            int distancemax = ((AppConfigrationVM.DistanceShowNearbyEvents_Max == null ? 0 : (int)AppConfigrationVM.DistanceShowNearbyEvents_Max) * 1000);

            var allrequest = await _authContext.Requestes.Where(m => m.status == 2 && (m.UserRequestId == id || m.UserId == id)).Select(m => (id == m.UserId ? m.UserRequestId : m.UserId)).ToListAsync();

            IQueryable<EventData> eventDataList = _authContext.EventData.Include(q => q.EventChatAttend).Where(n => n.EventChatAttend.Any(q => q.UserattendId == id ? true : (!allrequest.Contains(n.UserId))));

            if (categories != null)
            {
                List<string> deserializedCategories = JsonConvert.DeserializeObject<List<string>>(categories);

                if (deserializedCategories != null && deserializedCategories.Count() != 0)
                {
                    eventDataList = eventDataList.Where(q => deserializedCategories.Contains(q.categorie == null ? null : q.categorie.EntityId));
                }
            }

            List<EventData> eventDatas = await eventDataList.ToListAsync();

            List<EventChatAttend> EventChatAttends = eventDatas.SelectMany(q => q.EventChatAttend).ToList();

            List<int> blockod = EventChatAttends.Where(q => q.UserattendId == id && q.stutus == 2).Select(m => m.EventDataid).ToList();

            List<EventData> data = EventChatAttends.Where(m => !blockod.Contains(m.EventDataid)).Where(m => m.EventData.eventdateto.Value.Date >= DateTime.Now.Date && m.EventData.IsActive == true && (m.EventData.StopFrom != null ? (m.EventData.StopFrom.Value >= DateTime.Now.Date || m.EventData.StopTo.Value <= DateTime.Now.Date) : true) && (m.EventData.EventTypeList.key == true ? (m.UserattendId == id && m.stutus != 1 && m.stutus != 2) : true)).Select(m => m.EventData).Distinct().ToList();
            ///TODO: Filtering using UserCode
            try
            {
                if (!string.IsNullOrEmpty(user.Code))
                {
                    data = data.Where(e => (e.IsForWhiteLableOnly.HasValue && !e.IsForWhiteLableOnly.Value) || e.EventTypeListid != 6 ||
                           (_authContext.UserDetails.FirstOrDefault(u => u.PrimaryId == e.UserId).IsWhiteLabel.Value
                           && _authContext.UserDetails.FirstOrDefault(u => u.PrimaryId == e.UserId).Code == user.Code)).ToList();

                }
                else
                {
                    data = data.Where(e => (e.IsForWhiteLableOnly.HasValue && !e.IsForWhiteLableOnly.Value) || e.EventTypeListid != 6).ToList();
                }

            }
            catch (Exception ex)
            {

                throw;
            }
            ///

            data = data.Where(p => CalculateDistance(myLat, myLon, Convert.ToDouble(p.lat), Convert.ToDouble(p.lang)) <= (distancemax) && CalculateDistance(myLat, myLon, Convert.ToDouble(p.lat), Convert.ToDouble(p.lang)) >= (distance) && p.IsActive == true).ToList();

            int totalRowCount = data.Count();

            List<EventVM> list = new List<EventVM>();

            List<EventVM> nearbyEvents = data.Select(q => new EventVM()
            {
                Id = q.EntityId,
                DistanceBetweenLocationAndEvent = CalculateDistance(myLat, myLon, Convert.ToDouble(q.lat), Convert.ToDouble(q.lang))
            }).OrderBy(q => q.DistanceBetweenLocationAndEvent).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();


            foreach (var m in data.Where(q => nearbyEvents.Select(q => q.Id).ToList().Contains(q.EntityId)))
            {
                EventVM EventVM = new EventVM();
                EventVM.description = m.description;
                EventVM.categorie = m.categorie?.name;
                EventVM.categorieimage = _configuration["BaseUrl"] + m.categorie?.image;
                EventVM.lat = m.lat;
                EventVM.lang = m.lang;

                EventVM.DistanceBetweenLocationAndEvent = CalculateDistance(myLat, myLon, Convert.ToDouble(m.lat), Convert.ToDouble(m.lang));

                EventVM.Id = m.EntityId;
                EventVM.OrderByDes = m.Id;
                EventVM.eventdate = m.eventdate.Value.ConvertDateTimeToString();
                EventVM.eventtypeid = m.EventTypeList.entityID;
                EventVM.eventtypecolor = (m.eventtype == null && m.eventtype == Guid.Parse("265583AA-2511-4FD3-883E-6CAF1F8E4355")) ? "#0BBEA1" : "#00284c";
                EventVM.eventtype = m.EventTypeList.Name;
                EventVM.eventdateto = m.eventdateto.Value.ConvertDateTimeToString();
                EventVM.allday = Convert.ToBoolean(m.allday);
                EventVM.timefrom = m.eventfrom == null ? "" : m.eventfrom.Value.ToString(@"hh\:mm");
                EventVM.timeto = m.eventto == null ? "" : m.eventto.Value.ToString(@"hh\:mm");
                EventVM.Title = m.Title;
                EventVM.joined = GetEventattend(m.EntityId, EventChatAttends);
                EventVM.image = (m.EventTypeListid != 3 ? _configuration["BaseUrl"] : "") + m.image;
                EventVM.totalnumbert = m.totalnumbert;
                EventVM.key = m.UserId == id ? 1 : (EventChatAttends.Where(n => n.EventData.EntityId == m.EntityId).Where(b => b.UserattendId == id).Where(m => m.stutus != 1 && m.stutus != 2).FirstOrDefault() == null ? 2 : 3);
                EventVM.color = colorvalue(GetEventattend(m.EntityId, EventChatAttends), m.totalnumbert);
                EventVM.eventColor = m.EventTypeList.color;
                EventVM.UserImage = m.EventTypeListid == 5 || m.EventTypeListid == 6 ? _configuration["BaseUrl"] + m.User.UserImage : "";
                EventVM.EventTypeName = m.EventTypeList.Name.Contains("White") ? "Whitelabel" : m.EventTypeList.Name;
                list.Add(EventVM);
            }
            return (list, totalRowCount);
        }
        public (double lat, double loNGT) newetnewlocation(Decimal Latitude, decimal Longitude)
        {
            var d1 = (double)Latitude * (Math.PI / 180.0);
            var num1 = (double)Longitude * (Math.PI / 180.0);
            //var d2 = lat * (Math.PI / 180.0);
            //var num2 = lang * (Math.PI / 180.0) - num1;
            //var d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) +
            //         Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);
            //var daa = 6376500.0 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));
            var new_latitude = (double)Latitude + (d1 / 200) * (45 / Math.PI);
            var new_longitude = (double)Longitude + (num1 / 200) * (45 / Math.PI) / Math.Cos((double)Latitude * Math.PI / 180);
            //var new_latitude = Latitude + d1;
            //var new_longitude = Longitude + num1;
            var tttt = newetnewlocation2((double)Latitude, (double)Longitude);

            return (tttt.lat, tttt.loNGT);
        }

        public (double lat, double loNGT) newetnewlocation2(double Latitude, double Longitude, int distanceinmeters = 160)
        {
            var earth = 6378.137;  //radius of the earth in kilometer

            var m = (1 / ((2 * Math.PI / 360) * earth)) / 1000;  //1 meter in degree

            var new_latitude = Latitude + (distanceinmeters * m);

            var new_longitude = Longitude + (distanceinmeters * m) / Math.Cos(Latitude * (Math.PI / 180));
            return (new_latitude, new_longitude);
        }
        // shaimaa refactoring 
        public locationDataMV getAlleventUserlocations(int pageNumber, int pageSize, UserDetails user, AppConfigrationVM AppConfigrationVM, string categories)
        {
            int distance = ((AppConfigrationVM.DistanceShowNearbyEventsOnMap_Min == null ? 0 : (int)AppConfigrationVM.DistanceShowNearbyEventsOnMap_Min) * 1000);

            int distancemax = ((AppConfigrationVM.DistanceShowNearbyEventsOnMap_Max == null ? 0 : (int)AppConfigrationVM.DistanceShowNearbyEventsOnMap_Max) * 1000);
            int userdistance = user.distanceFilter == false ? ((AppConfigrationVM.DistanceShowNearbyAccountsInFeed_Min == null ? 0
                : (int)AppConfigrationVM.DistanceShowNearbyAccountsInFeed_Min) * 1000) : 0;
            int userdistancemax = user.distanceFilter == false ? ((AppConfigrationVM.DistanceShowNearbyAccountsInFeed_Max == null ? 0
                : (int)AppConfigrationVM.DistanceShowNearbyAccountsInFeed_Max) * 1000) : (int)(user.Manualdistancecontrol * 1000);

            var id = user.PrimaryId;
            var userLat = user.lat == null ? (double)0 : Convert.ToDouble(user.lat);
            var userLong = user.lang == null ? (double)0 : Convert.ToDouble(user.lang);

            locationDataMV locationDataMV = new locationDataMV();
            List<EventlocationDataMV> list = new List<EventlocationDataMV>();
            List<peoplocationDataMV> peoplocationDataMV = new List<peoplocationDataMV>();
            var color = _authContext.EventColor.FirstOrDefault();

            var allRequest = this._authContext.Requestes.Where(m => m.status == 2 && (m.UserRequestId == id || m.UserId == id))
                           .Select(m => (id == m.UserId ? m.UserRequestId : m.UserId)).ToList();
           
            List<EventChatAttend> allblock = this._authContext.EventChatAttend.Include(q => q.EventData).Where(n => (n.UserattendId == id ?
                                           true : (!allRequest.Contains(n.EventData.UserId))) && n.EventData.IsActive == true
                                && (n.EventData.StopFrom != null ? (n.EventData.StopFrom.Value >= DateTime.Now.Date || n.EventData.StopTo.Value <= DateTime.Now.Date)
            : true) 
            && (n.EventData.EventTypeList.key == true ? (n.UserattendId == id && n.stutus != 1 && n.stutus != 2) : true)
            ).OrderByDescending(M => M.Id).ToList();

            if (categories != null)
            {
                List<string> deserializedCategories = JsonConvert.DeserializeObject<List<string>>(categories);

                if (deserializedCategories != null && deserializedCategories.Count() != 0)
                {
                    //List<category> listCategories = _authContext.category.AsNoTracking().ToList();
                    //_authContext.EventCategoryTrackers.AddRange(deserializedCategories.Select(q => new EventCategoryTracker()
                    //{
                    //    CategoryId = listCategories.FirstOrDefault(c => c.EntityId == q).Id,
                    //    UserId = user.PrimaryId,
                    //    Date = DateTime.Now,
                    //}).ToList());
                    //_authContext.SaveChanges();

                    allblock = allblock.Where(q => deserializedCategories.Contains(q.EventData.categorie == null ? null : q.EventData.categorie.EntityId)).ToList();
                }
            }

            var blockodEventIds = allblock.Where(m => (m.UserattendId == id) && m.stutus == 2).Select(m => m.EventDataid).ToList();
            var data = allblock.Where(m => !blockodEventIds.Contains(m.EventDataid)).Where(m => m.EventData.eventdateto.Value.Date >= DateTime.Now.Date).Select(m => m.EventData).Distinct().ToList();


            // data = data.Where(p =>  CalculateDistance(userLat, userLong, Convert.ToDouble(p.lat), Convert.ToDouble(p.lang)) <= distancemax && CalculateDistance(userLat, userLong, Convert.ToDouble(p.lat), Convert.ToDouble(p.lang)) >= distance).ToList();
            data = this.FilterByDistance(data, userLat, userLong, distance, distancemax).ToList();
            var allusers = this._authContext.LoggedinUser.Include(n => n.User.UserDetails).Where(p => p.User.UserDetails.lat != null && p.User.UserDetails.lang != null).ToList();
            var allClosedUsers = allusers.Where(p => CalculateDistance(userLat, userLong, Convert.ToDouble(p.User.UserDetails.lat), Convert.ToDouble(p.User.UserDetails.lang)) <= (user.Manualdistancecontrol == 0 ? Convert.ToDouble(userdistancemax)
            : Convert.ToDouble(user.Manualdistancecontrol * 1000))).Select(m => m.User.UserDetails).ToList();

            allClosedUsers = allClosedUsers.Where(m => m.allowmylocation == true && m.Gender != null).ToList();
            allClosedUsers = allClosedUsers.Where(p => (p.Filteringaccordingtoage == true ? birtdate(p.agefrom, p.ageto, (p.birthdate == null ? DateTime.Now.Date : p.birthdate.Value.Date)) : true)).ToList();

            allClosedUsers = allClosedUsers.Where(m => (m.ghostmode == true ? type(m.AppearanceTypes, user.Gender) : true)).ToList();
            allClosedUsers = allClosedUsers.Where(m => (m.ghostmode == true ? type(m.AppearanceTypes, m.Gender) : true)).ToList();
            
            foreach (EventData eventData in data)
            {
                eventData.lat = Math.Round(double.Parse(eventData.lat), 5).ToString();
                eventData.lang = Math.Round(double.Parse(eventData.lang), 5).ToString();
            }
            var Eventlocations = data.Where(m => m.eventdateto.Value.Date >= DateTime.Now.Date).Select(n => new { lang = Math.Round(double.Parse(n.lang), 5), lat = Math.Round(double.Parse(n.lat), 5) }).Distinct().ToList();
           

            // filter private events
            if (!string.IsNullOrEmpty(user.Code))
            {
                data = data.Where(e => (e.IsForWhiteLableOnly.HasValue && !e.IsForWhiteLableOnly.Value) || e.EventTypeListid != 6 ||
                       (_authContext.UserDetails.FirstOrDefault(u => u.PrimaryId == e.UserId).IsWhiteLabel.Value
                       && _authContext.UserDetails.FirstOrDefault(u => u.PrimaryId == e.UserId).Code == user.Code)).ToList();

            }
            else
            {
                data = data.Where(e => (e.IsForWhiteLableOnly.HasValue && !e.IsForWhiteLableOnly.Value) || e.EventTypeListid != 6).ToList();
            }
            var eventstypelist = data.Where(m => m.IsActive == true && (m.StopFrom != null ? (m.StopFrom.Value >= DateTime.Now.Date || m.StopTo.Value <= DateTime.Now.Date) : true)).ToList();
            var peapollocations = allClosedUsers.Select(n => new { n.lang, n.lat }).ToList().Distinct();
            //int i = 0;
            foreach (var location in Eventlocations)
            {
                //Fix Error Private event Not Comming !!!
                eventstypelist = data.Where(m => m.lang == location.lang.ToString() && m.lat == location.lat.ToString()).ToList();
                // && m.IsActive == true && (m.StopFrom != null ? (m.StopFrom.Value >= DateTime.Now.Date || m.StopTo.Value <= DateTime.Now.Date) : true)).ToList();
                ////TODO:my code
                //if (!string.IsNullOrEmpty(user.Code))
                //{
                //    eventstypelist = eventstypelist.Where(e => (e.IsForWhiteLableOnly.HasValue && !e.IsForWhiteLableOnly.Value) || e.EventTypeListid != 6 ||
                //           (_authContext.UserDetails.FirstOrDefault(u => u.PrimaryId == e.UserId).IsWhiteLabel.Value
                //           && _authContext.UserDetails.FirstOrDefault(u => u.PrimaryId == e.UserId).Code == user.Code)).ToList();

                //}
                //else
                //{
                //    eventstypelist = eventstypelist.Where(e => (e.IsForWhiteLableOnly.HasValue && !e.IsForWhiteLableOnly.Value) || e.EventTypeListid != 6).ToList();
                //}
                var types = eventstypelist.Select(m => m.EventTypeListid).Distinct().ToList();
                
                foreach (var item in types)
                {
                    EventlocationDataMV Eventlocation = new EventlocationDataMV();

                    // var events = eventstypelist.Where(m => m.EventTypeListid == item).ToList();
                    var events = eventstypelist.Where(m => m.EventTypeListid == item).ToList();

                    Eventlocation.lang = Convert.ToDecimal(location.lang);

                    Eventlocation.lat = Convert.ToDecimal(location.lat);
                    //if (i != 0)
                    //{
                    //    var NEWLOCTION = newetnewlocation2(Convert.ToDouble(location.lat), Convert.ToDouble(location.lang), i * 5);
                    //    Eventlocation.lang = Convert.ToDecimal(NEWLOCTION.loNGT);
                    //    Eventlocation.lat = Convert.ToDecimal(NEWLOCTION.lat);
                    //}
                    //i++;
                    //Eventlocation.Event_TypeId = events[0].EventTypeList.entityID;
                    //Eventlocation.Event_TypeColor = events[0].EventTypeList.color;
                    Eventlocation.Event_Type = events[0].EventTypeList.Name;
                    Eventlocation.EventTypeName = events[0].EventTypeList.Name.Contains("White") ? "Whitelabel" : events[0].EventTypeList.Name;
                    Eventlocation.EventData = events.Select(m => new EventDataMV
                    {
                        eventdate = m.eventdate.Value.ConvertDateTimeToString(),

                        eventdateto = m.eventdateto.Value.ConvertDateTimeToString(),
                        allday = Convert.ToBoolean(m.allday),
                        description = m.description,
                        timefrom = m.eventfrom == null ? "" : m.eventfrom.Value.ToString(),
                        timeto = m.eventto == null ? "" : m.eventto.Value.ToString(),
                        category = m.categorie == null ? "" : m.categorie.name,
                        categorieimage = _configuration["BaseUrl"] + (m.categorie == null ? "" : m.categorie.image),
                        //joined = GetEventattend(m.EntityId, eventattend),
                        Title = m.Title,
                        Image = (m.EventTypeListid != 3 ? _configuration["BaseUrl"] : "") + m.image,
                        UserImage = m.EventTypeListid == 5 || m.EventTypeListid == 6 ? _configuration["BaseUrl"] + m.User.UserImage : "",
                        EventTypeName = m.EventTypeList.Name.Contains("White") ? "Whitelabel" : m.EventTypeList.Name,
                        Id = m.EntityId,
                        categorieId = (m.categorie == null ? "" : m.categorie.EntityId),
                        totalnumbert = m.totalnumbert,
                        lang = m.lang,
                        lat = m.lat,
                        UseraddedId = m.User?.UserId,
                        //key = m.UserId == id ? 1 : (eventattend.Where(n => n.EventData.EntityId == m.EntityId).Where(b => b.UserattendId == id).Where(m => m.stutus != 1 && m.stutus != 2).FirstOrDefault() == null ? 2 : 3),

                        eventtypeid = m.EventTypeList.entityID,
                        eventtypecolor = m.EventTypeList.color,
                        eventtype = m.EventTypeList.Name,
                        //color = colorvalue(GetEventattend(m.EntityId, eventattend), m.totalnumbert)

                    }).ToList();
                    
                    decimal perc = 0;
                    if (data.Count() != 0)
                        perc = (events.Count() * 100 / data.Count());
                    //Eventlocation.color = color == null ? (perc < 34 ? "#0BBEA1" : (perc <67 ? "#e7b416" : "#cc3232"))
                    //    : (perc < 34 ? color.emptycolor : (perc  < 67 ? color.middlecolor : color.crowdedcolor));

                    Eventlocation.color = color == null ? events.Count() < 5 ? "#0BBEA1" : (events.Count() < 10 ? "#e7b416" : "#cc3232")
                         : events.Count() < color.emptynumber ? color.emptycolor : (events.Count() < color.middlenumber ? color.middlecolor : color.crowdedcolor);

                    Eventlocation.EventMarkerImage = events[0].EventTypeListid == 5 || events[0].EventTypeListid == 6 ? _configuration["BaseUrl"] + events[0].User.UserImage : _configuration["BaseUrl"] + "/images/594a2c50-6590-42bb-aa32-b1ca7bc1f1ce.jpeg";
                    Eventlocation.count = events.Count;
                    //i = events.Count + i;
                    list.Add(Eventlocation);
                }
            }
            foreach (var userLocation in peapollocations)
            {
                peoplocationDataMV Eventlocation = new peoplocationDataMV();

                var events = allClosedUsers.Where(m => m.lang == userLocation.lang && m.lat == userLocation.lat).Select(n => new { gender = (n.Gender == null ? "ss" : n.Gender), personalSpace = n.personalSpace }).ToList();
                Eventlocation.lang = Convert.ToDecimal(userLocation.lang);
                Eventlocation.lat = Convert.ToDecimal(userLocation.lat);
                //TODO: Workin Here !!!!!!!!
                if (events.Count() == 1)
                {
                    if (events.FirstOrDefault().personalSpace == true)
                    {
                        var NEWLOCTION = newetnewlocation2((double)Eventlocation.lat, (double)Eventlocation.lang);
                        Eventlocation.lang = Convert.ToDecimal(NEWLOCTION.loNGT);
                        Eventlocation.lat = Convert.ToDecimal(NEWLOCTION.lat);

                        //var disww = CalculateDistance(Convert.ToDouble(item.lat), Convert.ToDouble(item.lang), NEWLOCTION.lat, NEWLOCTION.loNGT);
                    }
                }
                var mal = events.Where(m => m.gender.ToString().ToLower() == ("male".ToLower())).ToList();
                var Femal = events.Where(m => m.gender.ToString().ToLower().Contains("Femal".ToLower())).ToList();
                var malFemal = events.Where(m => !(m.gender.ToString().ToLower().Contains("mal".ToLower())) && !(m.gender.ToString().ToLower().Contains("Femal".ToLower()))).ToList();

                Eventlocation.MalePercentage = mal.Count() == 0 ? 0 : ((mal.Count * 100 / events.Count()));
                Eventlocation.Femalepercentage = Femal.Count() == 0 ? 0 : ((Femal.Count * 100 / events.Count()));
                Eventlocation.otherpercentage = malFemal.Count() == 0 ? 0 : ((malFemal.Count * 100 / events.Count()));
                Eventlocation.totalUsers = events.Count();
                Eventlocation.color = color == null ? events.Count() < 5 ? "#0BBEA1" : (events.Count() < 10 ? "#e7b416" : "#cc3232")
                    : events.Count() < color.emptynumber ? color.emptycolor : (events.Count() < color.middlenumber ? color.middlecolor : color.crowdedcolor);

                peoplocationDataMV.Add(Eventlocation);
            }
            locationDataMV.EventlocationDataMV = list;
            //peoplocationDataMV = this.CompleteFakeUsers(peoplocationDataMV, userLat, userLong);
            locationDataMV.locationMV = peoplocationDataMV;
            return (locationDataMV);
        }

        private IEnumerable<EventData> FilterByDistance(List<EventData> data, double userLat, double userLong, int distance, int distancemax)
        {
            foreach (var item in data)
            {
                var distanceValue = CalculateDistance(userLat, userLong, Convert.ToDouble(item.lat), Convert.ToDouble(item.lang));
                if(distance <= distanceValue && distanceValue <= distancemax)
                {
                    yield return item;
                }                
            }
        }

        public locationDataMV getAlleventlocation(int pageNumber, int pageSize, int id, double myLat, double myLon, double dis, string Gender, UserDetails user, AppConfigrationVM AppConfigrationVM, string categories)
        {
            int distance = ((AppConfigrationVM.DistanceShowNearbyEventsOnMap_Min == null ? 0 : (int)AppConfigrationVM.DistanceShowNearbyEventsOnMap_Min) * 1000);

            int distancemax = ((AppConfigrationVM.DistanceShowNearbyEventsOnMap_Max == null ? 0 : (int)AppConfigrationVM.DistanceShowNearbyEventsOnMap_Max) * 1000);
            int userdistance = user.distanceFilter == false ? ((AppConfigrationVM.DistanceShowNearbyAccountsInFeed_Min == null ? 0
                : (int)AppConfigrationVM.DistanceShowNearbyAccountsInFeed_Min) * 1000) : 0;
            int userdistancemax = user.distanceFilter == false ? ((AppConfigrationVM.DistanceShowNearbyAccountsInFeed_Max == null ? 0
                : (int)AppConfigrationVM.DistanceShowNearbyAccountsInFeed_Max) * 1000) : (int)(user.Manualdistancecontrol * 1000);

            //var data = this._authContext.EventData.Include(m => m.categorie).ToList().Where(m => m.eventdateto.Value.Date>=DateTime.Now.Date).ToList();
            var allRequest = this._authContext.Requestes.Where(m => m.status == 2 && (m.UserRequestId == id || m.UserId == id)).Select(m => (id == m.UserId ? m.UserRequestId : m.UserId)).ToList();

            List<EventChatAttend> allblock = this._authContext.EventChatAttend.Include(q => q.EventData).Where(n => (n.UserattendId == id ? true : (!allRequest.Contains(n.EventData.UserId))) && n.EventData.IsActive == true && (n.EventData.StopFrom != null ? (n.EventData.StopFrom.Value >= DateTime.Now.Date || n.EventData.StopTo.Value <= DateTime.Now.Date)
            : true) && (n.EventData.EventTypeList.key == true ? (n.UserattendId == id && n.stutus != 1 && n.stutus != 2) : true)).OrderByDescending(M => M.Id).ToList();

            if (categories != null)
            {
                List<string> deserializedCategories = JsonConvert.DeserializeObject<List<string>>(categories);

                if (deserializedCategories != null && deserializedCategories.Count() != 0)
                {
                    List<category> listCategories = _authContext.category.AsNoTracking().ToList();
                    _authContext.EventCategoryTrackers.AddRange(deserializedCategories.Select(q => new EventCategoryTracker()
                    {
                        CategoryId = listCategories.FirstOrDefault(c => c.EntityId == q).Id,
                        UserId = user.PrimaryId,
                        Date = DateTime.Now,
                    }).ToList());
                    _authContext.SaveChanges();

                    allblock = allblock.Where(q => deserializedCategories.Contains(q.EventData.categorie == null ? null : q.EventData.categorie.EntityId)).ToList();
                }
            }


            var blockod = allblock.Where(m => (m.UserattendId == id) && m.stutus == 2).Select(m => m.EventDataid).ToList();

            var data = allblock.Where(m => !blockod.Contains(m.EventDataid)).Where(m => m.EventData.eventdateto.Value.Date >= DateTime.Now.Date).Select(m => m.EventData).Distinct().ToList();

            data = data.Where(p => CalculateDistance(myLat, myLon, Convert.ToDouble(p.lat), Convert.ToDouble(p.lang)) <= (distancemax) && CalculateDistance(myLat, myLon, Convert.ToDouble(p.lat), Convert.ToDouble(p.lang)) >= (distance)).AsEnumerable().ToList();
            //data = data.Where(n => !allrequest.Contains(n.UserId)).ToList();

            //var userdata = this._authContext.UserDetails.Where(p => CalculateDistance(myLat, myLon, Convert.ToDouble(p.lat), Convert.ToDouble(p.lang)) <= (dis)).AsEnumerable().ToList();
            var alluserr = this._authContext.LoggedinUser.Include(n => n.User.UserDetails).Where(p => p.User.UserDetails.lat != null && p.User.UserDetails.lang != null).ToList();
            var alluser = alluserr.Where(p => CalculateDistance(myLat, myLon, Convert.ToDouble(p.User.UserDetails.lat), Convert.ToDouble(p.User.UserDetails.lang)) <= (user.Manualdistancecontrol == 0 ? Convert.ToDouble(userdistancemax) : Convert.ToDouble(user.Manualdistancecontrol * 1000))).Select(m => m.User.UserDetails).ToList();

            alluser = alluser.Where(m => m.allowmylocation == true && m.Gender != null).ToList();
            alluser = alluser.Where(p => (p.Filteringaccordingtoage == true ? birtdate(p.agefrom, p.ageto, (p.birthdate == null ? DateTime.Now.Date : p.birthdate.Value.Date)) : true)).ToList();

            alluser = alluser.Where(m => (m.ghostmode == true ? type(m.AppearanceTypes, Gender) : true)).ToList();
            alluser = alluser.Where(m => (m.ghostmode == true ? type(m.AppearanceTypes, m.Gender) : true)).ToList();

            //new 
            //new { lang = Math.Round(double.Parse(n.lang), 4), lat = Math.Round(double.Parse(n.lat), 4) }

            //Old
            //new { n.lang, n.lat}

            var location = data.Where(m => m.eventdateto.Value.Date >= DateTime.Now.Date).Select(n => new { lang = Math.Round(double.Parse(n.lang), 5), lat = Math.Round(double.Parse(n.lat), 5) }).Distinct().ToList();

            foreach (EventData eventData in data)
            {
                eventData.lat = Math.Round(double.Parse(eventData.lat), 5).ToString();
                eventData.lang = Math.Round(double.Parse(eventData.lang), 5).ToString();
            }

            var peapollocation = alluser.Select(n => new { n.lang, n.lat }).ToList().Distinct();

            locationDataMV locationDataMV = new locationDataMV();
            List<EventlocationDataMV> list = new List<EventlocationDataMV>();
            List<peoplocationDataMV> peoplocationDataMV = new List<peoplocationDataMV>();

            var color = _authContext.EventColor.FirstOrDefault();
            foreach (var lidata in location)
            {
                //Fix Error Private event Not Comming !!!
                var eventstypelist = data.Where(m => m.lang == lidata.lang.ToString() && m.lat == lidata.lat.ToString()
                 && m.IsActive == true && (m.StopFrom != null ? (m.StopFrom.Value >= DateTime.Now.Date || m.StopTo.Value <= DateTime.Now.Date) : true)).ToList();
                ////TODO:my code
                if (!string.IsNullOrEmpty(user.Code))
                {
                    eventstypelist = eventstypelist.Where(e => (e.IsForWhiteLableOnly.HasValue && !e.IsForWhiteLableOnly.Value) || e.EventTypeListid != 6 ||
                           (_authContext.UserDetails.FirstOrDefault(u => u.PrimaryId == e.UserId).IsWhiteLabel.Value
                           && _authContext.UserDetails.FirstOrDefault(u => u.PrimaryId == e.UserId).Code == user.Code)).ToList();

                }
                else
                {
                    eventstypelist = eventstypelist.Where(e => (e.IsForWhiteLableOnly.HasValue && !e.IsForWhiteLableOnly.Value) || e.EventTypeListid != 6).ToList();
                }
                var types = eventstypelist.Select(m => m.EventTypeListid).Distinct().ToList();
                int i = 0;
                foreach (var item in types)
                {
                    EventlocationDataMV Eventlocation = new EventlocationDataMV();

                    // var events = eventstypelist.Where(m => m.EventTypeListid == item).ToList();
                    var events = eventstypelist.Where(m => m.EventTypeListid == item).ToList();

                    Eventlocation.lang = Convert.ToDecimal(lidata.lang);

                    Eventlocation.lat = Convert.ToDecimal(lidata.lat);
                    if (i != 0)
                    {
                        var NEWLOCTION = newetnewlocation2(Convert.ToDouble(lidata.lat), Convert.ToDouble(lidata.lang), i * 5);
                        Eventlocation.lang = Convert.ToDecimal(NEWLOCTION.loNGT);
                        Eventlocation.lat = Convert.ToDecimal(NEWLOCTION.lat);
                    }
                    i++;
                    //Eventlocation.Event_TypeId = events[0].EventTypeList.entityID;
                    //Eventlocation.Event_TypeColor = events[0].EventTypeList.color;
                    Eventlocation.Event_Type = events[0].EventTypeList.Name;
                    Eventlocation.EventTypeName = events[0].EventTypeList.Name.Contains("White") ? "Whitelabel" : events[0].EventTypeList.Name;
                    Eventlocation.EventData = events.Select(m => new EventDataMV
                    {
                        eventdate = m.eventdate.Value.ConvertDateTimeToString(),

                        eventdateto = m.eventdateto.Value.ConvertDateTimeToString(),
                        allday = Convert.ToBoolean(m.allday),
                        description = m.description,
                        timefrom = m.eventfrom == null ? "" : m.eventfrom.Value.ToString(),
                        timeto = m.eventto == null ? "" : m.eventto.Value.ToString(),
                        category = m.categorie == null ? "" : m.categorie.name,
                        categorieimage = _configuration["BaseUrl"] + (m.categorie == null ? "" : m.categorie.image),
                        //joined = GetEventattend(m.EntityId, eventattend),
                        Title = m.Title,
                        Image = (m.EventTypeListid != 3 ? _configuration["BaseUrl"] : "") + m.image,
                        UserImage = m.EventTypeListid == 5 || m.EventTypeListid == 6 ? _configuration["BaseUrl"]+m.User.UserImage:"",
                        EventTypeName = m.EventTypeList.Name.Contains("White") ? "Whitelabel" : m.EventTypeList.Name,
                        Id = m.EntityId,
                        categorieId = (m.categorie == null ? "" : m.categorie.EntityId),
                        totalnumbert = m.totalnumbert,
                        lang = m.lang,
                        lat = m.lat,
                        UseraddedId = m.User?.UserId,
                        //key = m.UserId == id ? 1 : (eventattend.Where(n => n.EventData.EntityId == m.EntityId).Where(b => b.UserattendId == id).Where(m => m.stutus != 1 && m.stutus != 2).FirstOrDefault() == null ? 2 : 3),

                        eventtypeid = m.EventTypeList.entityID,
                        eventtypecolor = m.EventTypeList.color,
                        eventtype = m.EventTypeList.Name,
                        //color = colorvalue(GetEventattend(m.EntityId, eventattend), m.totalnumbert)

                    }).ToList();
                    //Eventlocation.count = events.Count();
                    decimal perc = 0;
                    if (data.Count() != 0)
                        perc = (events.Count() * 100 / data.Count());
                    //Eventlocation.color = color == null ? (perc < 34 ? "#0BBEA1" : (perc <67 ? "#e7b416" : "#cc3232"))
                    //    : (perc < 34 ? color.emptycolor : (perc  < 67 ? color.middlecolor : color.crowdedcolor));

                    Eventlocation.color = color == null ? events.Count() < 5 ? "#0BBEA1" : (events.Count() < 10 ? "#e7b416" : "#cc3232")
                         : events.Count() < color.emptynumber ? color.emptycolor : (events.Count() < color.middlenumber ? color.middlecolor : color.crowdedcolor);

                    Eventlocation.EventMarkerImage = events[0].EventTypeListid == 5 || events[0].EventTypeListid == 6 ? _configuration["BaseUrl"] + events[0].User.UserImage : _configuration["BaseUrl"] + "/images/594a2c50-6590-42bb-aa32-b1ca7bc1f1ce.jpeg";
                    Eventlocation.count = events.Count;
                    list.Add(Eventlocation);
                }
            }
            //list= list.OrderByDescending(m => m.EventData.eventdate);
            foreach (var userLocation in peapollocation)
            {
                peoplocationDataMV Eventlocation = new peoplocationDataMV();

                var events = alluser.Where(m => m.lang == userLocation.lang && m.lat == userLocation.lat).Select(n => new { gender = (n.Gender == null ? "ss" : n.Gender), personalSpace = n.personalSpace }).ToList();
                Eventlocation.lang = Convert.ToDecimal(userLocation.lang);
                Eventlocation.lat = Convert.ToDecimal(userLocation.lat);
                //TODO: Workin Here !!!!!!!!
                if (events.Count() == 1)
                {
                    if (events.FirstOrDefault().personalSpace == true)
                    {
                        var NEWLOCTION = newetnewlocation2((double)Eventlocation.lat, (double)Eventlocation.lang);
                        Eventlocation.lang = Convert.ToDecimal(NEWLOCTION.loNGT);
                        Eventlocation.lat = Convert.ToDecimal(NEWLOCTION.lat);

                        //var disww = CalculateDistance(Convert.ToDouble(item.lat), Convert.ToDouble(item.lang), NEWLOCTION.lat, NEWLOCTION.loNGT);
                    }
                }
                var mal = events.Where(m => m.gender.ToString().ToLower() == ("male".ToLower())).ToList();
                var Femal = events.Where(m => m.gender.ToString().ToLower().Contains("Femal".ToLower())).ToList();
                var malFemal = events.Where(m => !(m.gender.ToString().ToLower().Contains("mal".ToLower())) && !(m.gender.ToString().ToLower().Contains("Femal".ToLower()))).ToList();

                Eventlocation.MalePercentage = mal.Count() == 0 ? 0 : ((mal.Count * 100 / events.Count()));
                Eventlocation.Femalepercentage = Femal.Count() == 0 ? 0 : ((Femal.Count * 100 / events.Count()));
                Eventlocation.otherpercentage = malFemal.Count() == 0 ? 0 : ((malFemal.Count * 100 / events.Count()));
                Eventlocation.totalUsers = events.Count();
                Eventlocation.color = color == null ? events.Count() < 5 ? "#0BBEA1" : (events.Count() < 10 ? "#e7b416" : "#cc3232")
                    : events.Count() < color.emptynumber ? color.emptycolor : (events.Count() < color.middlenumber ? color.middlecolor : color.crowdedcolor);

                peoplocationDataMV.Add(Eventlocation);
            }
            locationDataMV.EventlocationDataMV = list;            
            peoplocationDataMV = this.CompleteFakeUsers(peoplocationDataMV,myLat,myLon);
            locationDataMV.locationMV = peoplocationDataMV;
            return (locationDataMV);
        }

        private List<peoplocationDataMV> CompleteFakeUsers(List<peoplocationDataMV> locationMV, double myLat, double myLon)
        {
            var myLatStrings =  (Math.Truncate(1000000 * myLat) / 1000000).ToString().Split('.');
            var myLonStrings = (Math.Truncate(1000000 * myLon) / 1000000).ToString().Split('.'); 
            Random random = new Random();

            for (int i = 0; i < 5000; i++)
            {
                int lat = random.Next(int.Parse(myLatStrings[1]), 63030459); //18.51640014679267 - 18.630304598192915
                int lon = random.Next(int.Parse(myLonStrings[1]), 34119415); //-72.34119415283203 - -72.2244644165039

                peoplocationDataMV userLocation = new peoplocationDataMV();
                userLocation.lat = (decimal)Convert.ToDouble(myLatStrings[0]+'.' + lat);
                userLocation.lang = (decimal)Convert.ToDouble(myLonStrings[0] +'.'+ lon);
                userLocation.color = "#0BBEA1";
                locationMV.Add(userLocation);
            }
            return locationMV;
        }
      

        public async Task<(RecommendedEventViewModel, string)> RecommendedEvent(UserDetails userDeatil, string eventId)
        {
            if (!string.IsNullOrEmpty(eventId) && !string.IsNullOrWhiteSpace(eventId))
            {
                EventData eventToSkipe = await _authContext.EventData.AsNoTracking().FirstOrDefaultAsync(q => q.EntityId == eventId);
                if (eventToSkipe != null)
                {
                    bool skippedBefore = await _authContext.SkippedEvents.AnyAsync(q => q.UserId == userDeatil.PrimaryId && q.EventId == eventToSkipe.Id);
                    if (!skippedBefore)
                    {
                        _authContext.SkippedEvents.Add(new SkippedEvent() { UserId = userDeatil.PrimaryId, EventId = eventToSkipe.Id, Date = DateTime.Now });
                        await _authContext.SaveChangesAsync();
                    }
                }
            }

            List<int> skippedEvents = await _authContext.SkippedEvents.Where(q => q.UserId == userDeatil.PrimaryId).Select(q => q.EventId).ToListAsync();

            AppConfigration appConfigration = await _authContext.AppConfigrations.FirstOrDefaultAsync();

            int distanceMin = ((appConfigration.RecommendedEventArea_Min == null ? 0 : (int)appConfigration.RecommendedEventArea_Min));
            int distanceMax = ((appConfigration.RecommendedEventArea_Max == null ? 0 : (int)appConfigration.RecommendedEventArea_Max));

            IQueryable<EventData> eventData = _authContext.EventData.Include(q => q.EventChatAttend).Include(q => q.EventTypeList);
            //eventData = eventData.Where(q => q.eventdate.Value.Date >= DateTime.Now);
            eventData = eventData.Where(q => q.eventdateto.Value.Date >= DateTime.Now);
            eventData = eventData.Where(q => !skippedEvents.Contains(q.Id));
            eventData = eventData.Where(q => !q.EventChatAttend.Any(q => q.UserattendId == userDeatil.PrimaryId));

            List<EventData> events = await eventData.AsNoTracking().ToListAsync();

            events = events.Where(p => googleLocationService.CalculateDistance(Convert.ToDouble(userDeatil.lat), Convert.ToDouble(userDeatil.lang), Convert.ToDouble(p.lat), Convert.ToDouble(p.lang),'M') <= (distanceMax) && googleLocationService.CalculateDistance(Convert.ToDouble(userDeatil.lat), Convert.ToDouble(userDeatil.lang), Convert.ToDouble(p.lat), Convert.ToDouble(p.lang),'M') >= (distanceMin) && p.IsActive == true).ToList();

            EventVM nearbyEvent = events.Select(q => new EventVM()
            {
                Id = q.EntityId,
                Title = q.Title,
                description = q.description,
                eventdate = q.eventdate.Value.ToString("dd/MM/yyyy"),
                image = (q.EventTypeListid != 3 ? _configuration["BaseUrl"] : "") + q.image,
                joined = GetEventattend(q.EntityId, q.EventChatAttend.ToList()),
                totalnumbert = q.totalnumbert,
                eventtype = q.EventTypeList.Name,
                eventColor = q.EventTypeList.color,
                eventtypecolor = (q.eventtype == null && q.eventtype == Guid.Parse("265583AA-2511-4FD3-883E-6CAF1F8E4355")) ? "#0BBEA1" : "#00284c",
                DistanceBetweenLocationAndEvent = CalculateDistance(Convert.ToDouble(userDeatil.lat), Convert.ToDouble(userDeatil.lang), Convert.ToDouble(q.lat), Convert.ToDouble(q.lang))
            }).OrderBy(q => q.DistanceBetweenLocationAndEvent).FirstOrDefault();

            RecommendedEventViewModel recommendedEvent = new RecommendedEventViewModel();

            if (events.Count() == 0)
            {
                recommendedEvent = null;
            }

            if (events.Count() != 0)
            {
                recommendedEvent.EventId = nearbyEvent.Id;
                recommendedEvent.Title = nearbyEvent.Title;
                recommendedEvent.Description = nearbyEvent.description;
                recommendedEvent.Image = nearbyEvent.image;
                recommendedEvent.eventtype = nearbyEvent.eventtype;
                recommendedEvent.eventColor = nearbyEvent.eventColor;
                recommendedEvent.eventtypecolor = nearbyEvent.eventtypecolor;
                recommendedEvent.EventDate = nearbyEvent.eventdate;
                recommendedEvent.Attendees = nearbyEvent.joined;
                recommendedEvent.From = nearbyEvent.totalnumbert;
            }

            string message = events.Count() != 0 ? "Your data" : "See Map for Nearby Events";

            return (recommendedEvent, message);
        }

        public bool type(int gosttyp, string usertype)
        {
            bool Flag = true;

            if (gosttyp == 1)
            {
                Flag = false;
            }
            else if (gosttyp == 2 && usertype.ToLower() == "male".ToLower())
            {
                Flag = false;
            }
            else if (gosttyp == 3 && usertype.ToLower().Contains("femal".ToLower()))
            {
                Flag = false;
            }
            else if (gosttyp == 4 && usertype.ToLower().Contains("other".ToLower()))
            {
                Flag = false;
            }
            return Flag;
        }
        public bool type(ICollection<AppearanceTypes_UserDetails> gosttyps, string usertype)
        {
            var appearencetypesid = gosttyps.Select(x => x.AppearanceTypeID).ToList();
            bool Flag = true;

            if (appearencetypesid.Contains(1))
            {
                Flag = false;
            }
            else if (appearencetypesid.Contains(2) && usertype.ToLower() == "male".ToLower())
            {
                Flag = false;
            }
            else if (appearencetypesid.Contains(3) && usertype.ToLower().Contains("femal".ToLower()))
            {
                Flag = false;
            }
            else if (appearencetypesid.Contains(4) && usertype.ToLower().Contains("other".ToLower()))
            {
                Flag = false;
            }
            return Flag;
        }
        public bool birtdate(int from, int to, DateTime birth)
        {
            bool flag = false;
            int years = DateTime.Now.Date.Year - birth.Date.Year;
            if (years >= from && years <= to)
                flag = true;
            return flag;
        }
        public string colorvalue(int join, int alldata)
        {
            int perc = (join * 100 / alldata);
            var color = (perc < 34 ? "#0BBEA1" : (perc < 67 ? "#e7b416" : "#cc3232"));
            return color;
        }
        public double ToRadians(double degrees) => degrees * Math.PI / 180.0;
        public double distanceInMiles(double lon1d, double lat1d, double lon2d, double lat2d)
        {
            var lon1 = ToRadians(lon1d);
            var lat1 = ToRadians(lat1d);
            var lon2 = ToRadians(lon2d);
            var lat2 = ToRadians(lat2d);

            var deltaLon = lon2 - lon1;
            var c = Math.Acos(Math.Sin(lat1) * Math.Sin(lat2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Cos(deltaLon));
            var earthRadius = 3958.76;
            var distInMiles = earthRadius * c;

            return distInMiles;
        }
        
        //  Accurate (Tested) ✔
        public double CalculateDistance(double lat1, double lon1, double lat2, double lon2, char unit)
        {
            double rlat1 = Math.PI * lat1 / 180;
            double rlat2 = Math.PI * lat2 / 180;
            double theta = lon1 - lon2;
            double rtheta = Math.PI * theta / 180;
            double dist =
                Math.Sin(rlat1) * Math.Sin(rlat2) + Math.Cos(rlat1) *
                Math.Cos(rlat2) * Math.Cos(rtheta);
            dist = Math.Acos(dist);
            dist = dist * 180 / Math.PI;
            dist = dist * 60 * 1.1515;

            switch (unit)
            {
                case 'K': //Kilometers -> default
                    return dist * 1.609344;
                case 'N': //Nautical Miles 
                    return dist * 0.8684;
                case 'M': //Miles
                    return dist;
            }

            return dist;
        }

        public List<listoftags> GetINterestdata(int id)
        {
            var listoftags = _authContext.listoftags.Include(m => m.User).Include(m => m.Interests).Where(m => m.UserId == id).ToList();

            return (listoftags.ToList());
        }
        public int attendcount(string id, int intersid)
        {
            return _authContext.EventChatAttend.Where(m => m.EventData.EntityId == id).Count();
        }

        //public async Task updateeventattend(EventChatAttend code)
        //{
        //    _authContext.EventChatAttend.Update(code);
        //    _authContext.SaveChanges();
        //}

        public async Task updatecategory(category code)
        {
            _authContext.category.Update(code);
            _authContext.SaveChanges();
        }
        public async Task Insertcategory(category code)
        {
            code.EntityId = Guid.NewGuid().ToString();
            _authContext.category.Add(code);
            _authContext.SaveChanges();
        }


        public async Task deletecategory(string id)
        {
            _authContext.category.Remove(Getcategorybyid(id));
            _authContext.SaveChanges();
        }

        public IEnumerable<ValidationResult> _ValidationResult(EventDataadminMV model)
        {
            if (model.eventdate < DateTime.Now.Date || model.eventdate > DateTime.Now.Date.AddYears(1))
            {
                var Message = string.Format(localizer["EventdatemustNotinthepast"]);
                yield return new ValidationResult(Message, new[] { nameof(model.eventdate) });
            }
            if (model.eventdateto > model.eventdate.AddYears(1))
            {
                var Message = string.Format(localizer["Maximumallowedeventperiod"]);
                yield return new ValidationResult(Message, new[] { nameof(model.eventdateto) });
            }
            if (model.totalnumbert < 3)
            {
                var Message = string.Format(localizer["AttendeesNumbershouldnotbelessthan2"]);
                yield return new ValidationResult(Message, new[] { nameof(model.totalnumbert) });
            }
            if (model.eventdate > model.eventdateto)
            {
                var Message = string.Format(localizer["EventstartdatemustNotolderthanEventenddate"]);
                yield return new ValidationResult(Message, new[] { nameof(model.eventdate) });
            }
            if (model.eventfrom == null && model.allday == false)
            {
                var Message = string.Format(localizer["Required"]);
                yield return new ValidationResult(Message, new[] { nameof(model.eventfrom) });
            }
            if (model.eventdate == null && model.allday == false)
            {
                var Message = string.Format(localizer["Required"]);
                yield return new ValidationResult(Message, new[] { nameof(model.eventdate) });
            }
            if (model.eventfrom != null && model.eventdate != null && model.eventfrom >= model.eventto)
            {
                var Message = string.Format(localizer["EventstarttimemustNotolderthanEventendtime"]);
                yield return new ValidationResult(Message, new[] { nameof(model.eventfrom) });
            }
        }
        public async Task<bool> Createrang(EventData Obj)
        {
            try
            {
                var vm = _authContext.EventData.AddAsync(Obj);
                await _authContext.SaveChangesAsync();
                var a = await InsertEventChatAttend(new EventChatAttend
                {
                    Jointime = DateTime.Now.TimeOfDay,
                    EventDataid = Obj.Id,
                    UserattendId = httpContextAccessor.HttpContext.GetUser().User.UserDetails.PrimaryId,
                    JoinDate = DateTime.Now.Date,
                    ISAdmin = true
                });

                return true;
            }
            catch (Exception EX)
            {
                return false;
            }
        }
        public async Task<bool> Createrang(List<EventData> eventsData)
        {
            try
            {
                var CurentUserPrimaryID = httpContextAccessor.HttpContext.GetUser().User.UserDetails.PrimaryId;
                eventsData.ForEach(x =>
                {
                    var guid = Guid.NewGuid().ToString();
                    x.EventChatAttend = new List<EventChatAttend>() { new EventChatAttend { EntityId = guid, Jointime = DateTime.Now.TimeOfDay, UserattendId = CurentUserPrimaryID, JoinDate = DateTime.Now.Date, ISAdmin = true } };
                });
                await _authContext.EventData.AddRangeAsync(eventsData);
                await _authContext.SaveChangesAsync();

                await UpdateEventsAddressFromGoogle(eventsData);

                return true;
            }
            catch (Exception EX)
            {
                return false;
            }
        }
        public async Task<CommonResponse<List<int>>> Create(EventDataadminMV VM)
        {
            List<int> eventChatIds = new List<int>();
            try
            {
                var ObjectList = Converter(VM);
                await _authContext.EventData.AddRangeAsync(ObjectList);
                await _authContext.SaveChangesAsync();
                var Obj = ObjectList.FirstOrDefault();
                var createdEvents = _authContext.EventData.Where(e => e.EntityId == Obj.EntityId).ToList();
                           
                foreach (var obj in createdEvents)
                    {
                        try
                        {
                            var a = await InsertEventChatAttend(new EventChatAttend { Jointime = DateTime.Now.TimeOfDay, EventDataid = obj.Id, UserattendId = httpContextAccessor.HttpContext.GetUser().User.UserDetails.PrimaryId, JoinDate = DateTime.Now.Date, ISAdmin = true });
                            eventChatIds.Add(a.Id);
                        }
                        catch (Exception ex)
                        {

                            //throw;
                        }
                    }

                foreach (var obj in createdEvents)
                {
                    try
                    {
                        await UpdateEventAddressFromGoogle(obj);
                    }
                    catch (Exception ex)
                    {

                        //throw;
                    }
                }
                await _authContext.SaveChangesAsync();
                createdEvents = _authContext.EventData.Where(e => e.EntityId == Obj.EntityId).ToList();
                return CommonResponse<List<int>>.GetResult(200, true, localizer["SavedSuccessfully"],eventChatIds);
            }
            catch (Exception EX)
            {
                return CommonResponse<List<int>>.GetResult(406, false, EX.Message);
            }
        }


        public async Task<CommonResponse<EventDataadminMV>> Edit(EventDataadminMV VM)
        {
            try
            {
                var ObjectList = Converter(VM);
                foreach (var obj in ObjectList)
                {
                    await UpdateEventAddressFromGoogle(obj);
                    _authContext.Attach(obj);
                    _authContext.Entry(obj).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _authContext.SaveChanges();
                }
                return CommonResponse<EventDataadminMV>.GetResult(200, true, localizer["SavedSuccessfully"], VM);

            }
            catch (Exception ex)
            {
                return CommonResponse<EventDataadminMV>.GetResult(406, false, ex.Message);
            }
        }
        public async Task<CommonResponse<EventDataadminMV>> Remove(string ID)
        {
            var Obj = _authContext.EventData.FirstOrDefault(x => x.EntityId == ID);
            try

            {

                _authContext.EventData.Remove(Obj);
                await _authContext.SaveChangesAsync();
                return CommonResponse<EventDataadminMV>.GetResult(200, true, localizer["RemovedSuccessfully"]);
            }
            catch
            {
                var related = DependencyValidator<EventData>.GetDependenciesNames(Obj).ToList();
                var Message = string.Format(localizer["PlzDeleteDataRelatedWithObjForCompleteObjectRemove"],
                   Obj?.Title,
                    //localizer[Obj.GetType().BaseType.Name),
                    related.Select(x => localizer[x].Value).Aggregate((x, y) => x + "," + y));
                return CommonResponse<EventDataadminMV>.GetResult(406, false, Message);

            }
        }
        public async Task<CommonResponse<EventDataadminMV>> RemoveEventById(string id)
        {
            var Obj = _authContext.EventData.FirstOrDefault(x => x.Id == int.Parse(id));
            try

            {

                _authContext.EventData.Remove(Obj);
                await _authContext.SaveChangesAsync();
                return CommonResponse<EventDataadminMV>.GetResult(200, true, localizer["RemovedSuccessfully"]);
            }
            catch
            {
                var related = DependencyValidator<EventData>.GetDependenciesNames(Obj).ToList();
                var Message = string.Format(localizer["PlzDeleteDataRelatedWithObjForCompleteObjectRemove"],
                    Obj?.Title,
                    //localizer[Obj.GetType().BaseType.Name),
                    related.Select(x => localizer[x].Value).Aggregate((x, y) => x + "," + y));
                return CommonResponse<EventDataadminMV>.GetResult(406, false, Message);

            }
        }

        public async Task<CommonResponse<EventDataadminMV>> ToggleActiveConfigration(string ID, bool IsActive)
        {
            try
            {

                var EventData = _authContext.EventData.FirstOrDefault(x => x.EntityId == ID);
                EventData.IsActive = IsActive;
                await _authContext.SaveChangesAsync();
                return CommonResponse<EventDataadminMV>.GetResult(200, true, localizer["UpdatedSuccessfully"]);
            }
            catch
            {
                return CommonResponse<EventDataadminMV>.GetResult(406, false, "SomthingGoesWrong");

            }

        }

        public async Task<CommonResponse<EventDataadminMV>> ToggleActiveConfigrationByPrimaryKey(string ID, bool IsActive)
        {
            try
            {

                var EventData = _authContext.EventData.FirstOrDefault(x => x.Id == int.Parse(ID));
                EventData.IsActive = IsActive;
                await _authContext.SaveChangesAsync();
                return CommonResponse<EventDataadminMV>.GetResult(200, true, localizer["UpdatedSuccessfully"]);
            }
            catch
            {
                return CommonResponse<EventDataadminMV>.GetResult(406, false, "SomthingGoesWrong");

            }

        }
        public EventDataadminMV GetData(string ID)
        {
            try
            {
                var Obj = _authContext.EventData.FirstOrDefault(x => x.EntityId == ID);
                var vM = Converter(Obj);
                return vM;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public EventDataadminMV GetEventDataDetails(int Id)
        {
            try
            {
                var Obj = _authContext.EventData.FirstOrDefault(x => x.Id == Id);
                var vM = Converter(Obj);
                return vM;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public IEnumerable<EventDataadminMV> GetData()
        {
            var data = _authContext.EventData.OrderByDescending(x => x.Id).Select(Converter).ToList();
            return data;
        }
        public IEnumerable<EventDataadminMV> GetData(Expression<Func<EventData, bool>> predicate)
        {
            var data = _authContext.EventData.Where(predicate).OrderByDescending(x => x.Id).Select(Converter).ToList();
            return data;
        }

        public IQueryable<EventDataadminMV> GetData(PaginationFilter PaginationFilter,int? Search_EventTypeListID,int? Search_EventCategoryID, out int filteredCount)
        {
            var filterdData = _authContext.EventData.Where(x => (Search_EventTypeListID == null || x.EventTypeListid == Search_EventTypeListID)
                        && (Search_EventCategoryID == null || x.categorieId == Search_EventCategoryID)
                        && (string.IsNullOrEmpty(PaginationFilter.SearchValue) || x.Title.ToLower().Contains(PaginationFilter.SearchValue.ToLower())) 
                        && (x.eventdateto.Value.Date > DateTime.Now.Date || (x.eventdateto.Value.Date == DateTime.Now.Date && x.eventto > DateTime.Now.TimeOfDay) || (x.allday == true && x.eventdate.Value.Date == DateTime.Now.Date))
                        );
      
            IQueryable<EventDataadminMV> data = null;
            if (!string.IsNullOrEmpty(PaginationFilter.SortColumn) && !string.IsNullOrEmpty(PaginationFilter.SortColumnDirection))
            {
                 data = filterdData.Skip(PaginationFilter.PageNumber).Take(PaginationFilter.PageSize).OrderByDescending(x => x.eventdateto)                                          
                                          .Select(Converter).AsQueryable().OrderBy(PaginationFilter.SortColumn + " " + PaginationFilter.SortColumnDirection);
             
            }
            else
            {
                 data = filterdData.Skip(PaginationFilter.PageNumber).Take(PaginationFilter.PageSize).OrderByDescending(x => x.Id)                    
                      .Select(Converter).AsQueryable();
                
            }
            filteredCount = filterdData.Count();
            return data;
        }

        public IQueryable<EventDataadminMV> GetWhiteLableEvents(PaginationFilter PaginationFilter,int PrimaryId, int? Search_EventTypeListID, int? Search_EventCategoryID, out int filteredCount)
        {
            var filterdData = _authContext.EventData.Where(x => x.UserId == PrimaryId && (Search_EventTypeListID == null || x.EventTypeListid == Search_EventTypeListID)
                       && (Search_EventCategoryID == null || x.categorieId == Search_EventCategoryID));
            IQueryable<EventDataadminMV> data = null;
            if (!string.IsNullOrEmpty(PaginationFilter.SortColumn) && !string.IsNullOrEmpty(PaginationFilter.SortColumnDirection))
            {
                data = filterdData.Skip(PaginationFilter.PageNumber).Take(PaginationFilter.PageSize).OrderByDescending(x => x.CreatedDate)
                                         .Select(Converter).AsQueryable().OrderBy(PaginationFilter.SortColumn + " " + PaginationFilter.SortColumnDirection);

            }
            else
            {
                data = filterdData.Skip(PaginationFilter.PageNumber).Take(PaginationFilter.PageSize).OrderByDescending(x => x.CreatedDate)
                     .Select(Converter).AsQueryable();

            }
            filteredCount = filterdData.Count();
            return data;
        }


        IEnumerable<EventData> Converter(EventDataadminMV model)
        {
            if (model == null) yield return null;

            foreach (var (value, i) in model.eventdateList.Select((value,i) =>(value,i)))
            {
                var Obj = new EventData()
                {
                    EntityId = model.EntityId,
                    Id = model.Id,
                    checkout_details = model.checkout_detailsList[i],
                    UserId = model.userid,
                    allday = model.allday,
                    categorieId = model.categorieId,
                    description = model.description,
                    eventdate = value,
                    eventdateto = model.eventdatetoList[i],
                    eventfrom = model.eventfrom,
                    eventto = model.eventto,
                    EventTypeListid = model.EventTypeListid,
                    lat = model.lat,
                    lang = model.lang,
                    IsActive = model.IsActive,
                    status = model.status,
                    CreatedDate = DateTime.Now,
                    totalnumbert = model.totalnumbert,
                    image = model.Image,
                    Title = model.Title,
                    IsForWhiteLableOnly = model.EventTypeListid == 6 || model.EventTypeListid == 5 ? true : false,
                };
                yield return Obj;
            }            
            
        }
        EventDataadminMV Converter(EventData model)
        {
            if (model == null) return null;

            var Obj = new EventDataadminMV
            {
                EntityId = model.EntityId,
                Id = model.Id,
                checkout_details = model.checkout_details,
                EventAttendCount = model.EventChatAttend.Count,
                userid = Convert.ToInt32(model.UserId),
                allday = model.allday ?? false,
                categorieId = model.categorieId ?? 0,
                description = model.description,
                eventdate = model.eventdate ?? new DateTime(),
                eventdateto = model.eventdateto ?? new DateTime(),
                eventfrom = model.eventfrom ?? new TimeSpan(),
                eventto = model.eventto ?? new TimeSpan(),
                EventTypeListid = model.EventTypeListid,
                categoryName = model.categorie == null ? "" : model.categorie.name,
                EventTypeListName = model.EventTypeList?.Name,
                CreatedDate = model.CreatedDate.Value.ToString("HH:mm yyyy-MM-dd"),
                Attendees = model.Attendees == null ? 0 : model.Attendees.Value,
                Shars = model.Shars == null ? 0 : model.Shars.Value,
                Views = model.Views == null ? 0 : model.Views.Value,
                lat = model.lat,
                lang = model.lang,
                IsActive = model.IsActive ?? false,
                status = model.status,
                totalnumbert = model.totalnumbert,
                Image = model.image,
                Title = model.Title,
                MessageChatEventCount=_authContext.Messagedata.Include(m=>m.EventChatAttend).Where(m=>m.EventChatAttend.EventDataid== model.Id).Count(),
            };
            return Obj;
        }

        public List<EventChatAttend> getallattendevent(string id)
        {
            var curntuser = (httpContextAccessor.HttpContext.GetUser().User.UserDetails.PrimaryId);
            var data = this._authContext.EventChatAttend.Where(n => n.EventData.EntityId == id && n.stutus != 1 && n.stutus != 2 && n.stutus != 3).ToList();
            var d = data.Where(n => n.UserattendId != curntuser).ToList();
            d.ForEach(n => { n.UserNotreadcount += 1; });
            _authContext.EventChatAttend.UpdateRange(d);
            _authContext.SaveChanges();
            return (data);
        }


        #region public 

        public locationDataMV GetAllEventLocations(int pageNumber, int pageSize, double myLat, double myLon, AppConfigrationVM AppConfigrationVM, string categories)
        {
            int distance = ((AppConfigrationVM.DistanceShowNearbyEventsOnMap_Min == null ? 0 : (int)AppConfigrationVM.DistanceShowNearbyEventsOnMap_Min) * 1000);

            int distancemax = ((AppConfigrationVM.DistanceShowNearbyEventsOnMap_Max == null ? 0 : (int)AppConfigrationVM.DistanceShowNearbyEventsOnMap_Max) * 1000);

            var allrequest = this._authContext.Requestes.Where(m => m.status == 2).Select(m => m.UserId).ToList();

            List<EventChatAttend> allblock = this._authContext.EventChatAttend.Include(q => q.EventData).ThenInclude(q => q.categorie).Where(n => ((!allrequest.Contains(n.EventData.UserId))) && n.EventData.IsActive == true && (n.EventData.StopFrom != null ? (n.EventData.StopFrom.Value >= DateTime.Now.Date || n.EventData.StopTo.Value <= DateTime.Now.Date) : true) && (n.EventData.EventTypeList.key == true ? (n.stutus != 1 && n.stutus != 2) : true)).OrderByDescending(M => M.Id).ToList();

            if (categories != null)
            {
                List<string> deserializedCategories = JsonConvert.DeserializeObject<List<string>>(categories);

                if (deserializedCategories != null && deserializedCategories.Count() != 0)
                {
                    allblock = allblock.Where(q => deserializedCategories.Contains(q.EventData.categorie == null ? null : q.EventData.categorie.EntityId)).ToList();
                }
            }

            var blockod = allblock.Where(m => m.stutus == 2).Select(m => m.EventDataid).ToList();

            var data = allblock.Where(m => !blockod.Contains(m.EventDataid)).Where(m => m.EventData.eventdateto.Value.Date >= DateTime.Now.Date).Select(m => m.EventData).Distinct().ToList();

            data = data.Where(p => CalculateDistance(myLat, myLon, Convert.ToDouble(p.lat), Convert.ToDouble(p.lang)) <= (distancemax) && CalculateDistance(myLat, myLon, Convert.ToDouble(p.lat), Convert.ToDouble(p.lang)) >= (distance)).AsEnumerable().ToList();

            var eventattend = allblock.ToList();

            var alluserr = this._authContext.LoggedinUser.Include(n => n.User.UserDetails).Where(p => p.User.UserDetails.lat != null && p.User.UserDetails.lang != null).ToList();

            var alluser = alluserr.Where(p => CalculateDistance(myLat, myLon, Convert.ToDouble(p.User.UserDetails.lat), Convert.ToDouble(p.User.UserDetails.lang))
            <= distancemax).Select(m => m.User.UserDetails).ToList();

            alluser = alluser.Where(m => m.allowmylocation == true && m.Gender != null).ToList();
            alluser = alluser.Where(p => (p.Filteringaccordingtoage == true ? birtdate(p.agefrom, p.ageto, (p.birthdate == null ? DateTime.Now.Date : p.birthdate.Value.Date)) : true)).ToList();

            alluser = alluser.Where(m => (m.ghostmode == true ? type(m.AppearanceTypes, m.Gender) : true)).ToList();

            var location = data.Where(m => m.eventdateto.Value.Date >= DateTime.Now.Date).Select(n => new { lang = Math.Round(double.Parse(n.lang), 5), lat = Math.Round(double.Parse(n.lat), 5) }).Distinct().ToList();
            var peapollocation = alluser.Select(n => new { n.lang, n.lat }).ToList().Distinct();

            foreach (EventData eventData in data)
            {
                eventData.lat = Math.Round(double.Parse(eventData.lat), 5).ToString();
                eventData.lang = Math.Round(double.Parse(eventData.lang), 5).ToString();
            }

            locationDataMV locationDataMV = new locationDataMV();
            List<EventlocationDataMV> list = new List<EventlocationDataMV>();
            List<peoplocationDataMV> peoplocationDataMV = new List<peoplocationDataMV>();

            var color = _authContext.EventColor.FirstOrDefault();
            foreach (var lidata in location)
            {
                var eventstypelist = data.Where(m => m.lang == lidata.lang.ToString() && m.lat == lidata.lat.ToString() && m.IsActive == true && (m.StopFrom != null ? (m.StopFrom.Value >= DateTime.Now.Date || m.StopTo.Value <= DateTime.Now.Date) : true)).ToList();
                var types = eventstypelist.Select(m => m.EventTypeListid).Distinct().ToList();
                int i = 0;
                foreach (var item in types)
                {
                    EventlocationDataMV Eventlocation = new EventlocationDataMV();

                    // var events = eventstypelist.Where(m => m.EventTypeListid == item).ToList();
                    var events = eventstypelist.Where(m => m.EventTypeListid == item).ToList();

                    Eventlocation.lang = Convert.ToDecimal(lidata.lang);

                    Eventlocation.lat = Convert.ToDecimal(lidata.lat);
                    if (i != 0)
                    {
                        var NEWLOCTION = newetnewlocation2(Convert.ToDouble(lidata.lat), Convert.ToDouble(lidata.lang), i * 5);
                        Eventlocation.lang = Convert.ToDecimal(NEWLOCTION.loNGT);
                        Eventlocation.lat = Convert.ToDecimal(NEWLOCTION.lat);

                    }
                    i++;
                    //Eventlocation.Event_TypeId = events[0].EventTypeList.entityID;
                    //Eventlocation.Event_TypeColor = events[0].EventTypeList.color;
                    Eventlocation.Event_Type = events[0].EventTypeList.Name;
                    Eventlocation.EventData = events.Select(m => new EventDataMV
                    {
                        eventdate = m.eventdate.Value.ConvertDateTimeToString(),

                        eventdateto = m.eventdateto.Value.ConvertDateTimeToString(),
                        allday = Convert.ToBoolean(m.allday),
                        description = m.description,
                        timefrom = m.eventfrom == null ? "" : m.eventfrom.Value.ToString(),
                        timeto = m.eventto == null ? "" : m.eventto.Value.ToString(),
                        category = m.categorie == null ? "" : m.categorie.name,
                        categorieimage = _configuration["BaseUrl"] + (m.categorie == null ? "" : m.categorie.image),
                        //joined = GetEventattend(m.EntityId, eventattend),
                        Title = m.Title,
                        Image = (m.EventTypeListid != 3 ? _configuration["BaseUrl"] : "") + m.image,
                        Id = m.EntityId,
                        categorieId = (m.categorie == null ? "" : m.categorie.EntityId),
                        totalnumbert = m.totalnumbert,
                        lang = m.lang,
                        lat = m.lat,
                        UseraddedId = m.User?.UserId,
                        // key = (eventattend.Where(n => n.EventData.EntityId == m.EntityId).Where(m => m.stutus != 1 && m.stutus != 2).FirstOrDefault() == null ? 2 : 3),

                        eventtypeid = m.EventTypeList.entityID,
                        eventtypecolor = m.EventTypeList.color,
                        eventtype = m.EventTypeList.Name,
                        // color = colorvalue(GetEventattend(m.EntityId, eventattend), m.totalnumbert)


                    }).ToList();
                    Eventlocation.count = events.Count();
                    decimal perc = 0;
                    if (data.Count() != 0)
                        perc = (events.Count() * 100 / data.Count());
                    //Eventlocation.color = color == null ? (perc < 34 ? "#0BBEA1" : (perc <67 ? "#e7b416" : "#cc3232"))
                    //    : (perc < 34 ? color.emptycolor : (perc  < 67 ? color.middlecolor : color.crowdedcolor));

                    Eventlocation.color = color == null ? events.Count() < 5 ? "#0BBEA1" : (events.Count() < 10 ? "#e7b416" : "#cc3232")
                         : events.Count() < color.emptynumber ? color.emptycolor : (events.Count() < color.middlenumber ? color.middlecolor : color.crowdedcolor);

                    list.Add(Eventlocation);
                }
            }
            //list= list.OrderByDescending(m => m.EventData.eventdate);
            foreach (var item in peapollocation)
            {
                peoplocationDataMV Eventlocation = new peoplocationDataMV();

                var events = alluser.Where(m => m.lang == item.lang && m.lat == item.lat).Select(n => new { gender = (n.Gender == null ? "ss" : n.Gender), n.personalSpace }).ToList();
                Eventlocation.lang = Convert.ToDecimal(item.lang);
                Eventlocation.lat = Convert.ToDecimal(item.lat);
                if (events.Count() == 1)
                {
                    if (events.FirstOrDefault().personalSpace == true)
                    {
                        var NEWLOCTION = newetnewlocation2((double)Eventlocation.lat, (double)Eventlocation.lang);
                        Eventlocation.lang = Convert.ToDecimal(NEWLOCTION.loNGT);
                        Eventlocation.lat = Convert.ToDecimal(NEWLOCTION.lat);

                        //var disww = CalculateDistance(Convert.ToDouble(item.lat), Convert.ToDouble(item.lang), NEWLOCTION.lat, NEWLOCTION.loNGT);
                    }
                }
                var mal = events.Where(m => m.gender.ToString().ToLower() == ("male".ToLower())).ToList();
                var Femal = events.Where(m => m.gender.ToString().ToLower().Contains("Femal".ToLower())).ToList();
                var malFemal = events.Where(m => !(m.gender.ToString().ToLower().Contains("mal".ToLower())) && !(m.gender.ToString().ToLower().Contains("Femal".ToLower()))).ToList();


                Eventlocation.MalePercentage = mal.Count() == 0 ? 0 : ((mal.Count * 100 / events.Count()));
                Eventlocation.Femalepercentage = Femal.Count() == 0 ? 0 : ((Femal.Count * 100 / events.Count()));
                Eventlocation.otherpercentage = malFemal.Count() == 0 ? 0 : ((malFemal.Count * 100 / events.Count()));
                Eventlocation.totalUsers = events.Count();
                Eventlocation.color = color == null ? events.Count() < 5 ? "#0BBEA1" : (events.Count() < 10 ? "#e7b416" : "#cc3232")
                    : events.Count() < color.emptynumber ? color.emptycolor : (events.Count() < color.middlenumber ? color.middlecolor : color.crowdedcolor);

                peoplocationDataMV.Add(Eventlocation);
            }
            locationDataMV.EventlocationDataMV = list;
            locationDataMV.locationMV = peoplocationDataMV;
            return (locationDataMV);
        }

        public async Task<(List<EventVM> eventData, int totalRowCount)> GetAllEventLocations_2(double myLat, double myLon, AppConfigrationVM AppConfigrationVM, string categories, int pageNumber, int pageSize)
        {
            int distance = ((AppConfigrationVM.DistanceShowNearbyEvents_Min == null ? 0 : (int)AppConfigrationVM.DistanceShowNearbyEvents_Min) * 1000);

            int distancemax = ((AppConfigrationVM.DistanceShowNearbyEvents_Max == null ? 0 : (int)AppConfigrationVM.DistanceShowNearbyEvents_Max) * 1000);

            //var allrequest = await this._authContext.Requestes.Where(m => m.status == 2).Select(m => m.UserId).ToListAsync();

            IQueryable<EventData> eventDataList = _authContext.EventData.Include(q => q.EventChatAttend);

            if (categories != null)
            {
                List<string> deserializedCategories = JsonConvert.DeserializeObject<List<string>>(categories);

                if (deserializedCategories != null && deserializedCategories.Count() != 0)
                {
                    eventDataList = eventDataList.Where(q => deserializedCategories.Contains(q.categorie == null ? null : q.categorie.EntityId));
                }
            }

            List<EventData> eventDatas = await eventDataList.ToListAsync();

            List<EventChatAttend> eventChatAttends = eventDatas.SelectMany(q => q.EventChatAttend).ToList();

            List<EventData> data = eventDatas.Where(m => m.eventdateto.Value.Date >= DateTime.Now.Date && m.IsActive == true && (m.StopFrom != null ? (m.StopFrom.Value >= DateTime.Now.Date || m.StopTo.Value <= DateTime.Now.Date) : true) && (m.EventTypeList.key == true ? (m.EventChatAttend.Any(q => q.stutus != 1 && q.stutus != 2)) : true)).ToList();

            data = data.Where(p => CalculateDistance(myLat, myLon, Convert.ToDouble(p.lat), Convert.ToDouble(p.lang)) <= (distancemax) && CalculateDistance(myLat, myLon, Convert.ToDouble(p.lat), Convert.ToDouble(p.lang)) >= (distance) && p.IsActive == true).ToList();

            int totalRowCount = data.Count();

            locationDataMV locationDataMV = new locationDataMV();

            List<EventVM> list = new List<EventVM>();

            foreach (var m in data)
            {
                EventVM EventVM = new EventVM();
                EventVM.description = m.description;
                EventVM.categorie = m.categorie?.name;
                EventVM.categorieimage = _configuration["BaseUrl"] + m.categorie?.image;
                EventVM.lat = m.lat;
                EventVM.lang = m.lang;
                EventVM.DistanceBetweenLocationAndEvent = CalculateDistance(myLat, myLon, Convert.ToDouble(m.lat), Convert.ToDouble(m.lang));

                EventVM.Id = m.EntityId;
                EventVM.OrderByDes = m.Id;
                EventVM.eventdate = m.eventdate.Value.ConvertDateTimeToString();
                EventVM.eventtypeid = m.EventTypeList.entityID;
                EventVM.eventtypecolor = (m.eventtype == null && m.eventtype == Guid.Parse("265583AA-2511-4FD3-883E-6CAF1F8E4355")) ? "#0BBEA1" : "#00284c";
                EventVM.eventtype = m.EventTypeList.Name;
                EventVM.eventdateto = m.eventdateto.Value.ConvertDateTimeToString();
                EventVM.allday = Convert.ToBoolean(m.allday);
                EventVM.timefrom = m.eventfrom == null ? "" : m.eventfrom.Value.ToString(@"hh\:mm");
                EventVM.timeto = m.eventto == null ? "" : m.eventto.Value.ToString(@"hh\:mm");
                EventVM.Title = m.Title;
                //EventVM.joined = GetEventattend(m.EntityId, eventChatAttends);
                EventVM.image = (m.EventTypeListid != 3 ? _configuration["BaseUrl"] : "") + m.image;
                EventVM.totalnumbert = m.totalnumbert;
                //EventVM.key = (eventChatAttends.Where(n => n.EventData.EntityId == m.EntityId).Where(m => m.stutus != 1 && m.stutus != 2).FirstOrDefault() == null ? 2 : 3);
                //EventVM.color = colorvalue(GetEventattend(m.EntityId, eventChatAttends), m.totalnumbert);
                EventVM.eventColor = m.EventTypeList.color;
                list.Add(EventVM);
            }

            var dataList = list.OrderBy(x => x.DistanceBetweenLocationAndEvent).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            foreach (EventVM eventdata in dataList)
            {
                eventdata.joined = GetEventattend(data.FirstOrDefault(q => q.EntityId == eventdata.Id).EntityId, eventChatAttends);
                eventdata.key = (eventChatAttends.Where(n => n.EventData.EntityId == data.FirstOrDefault(q => q.EntityId == eventdata.Id).EntityId).Where(m => m.stutus != 1 && m.stutus != 2).FirstOrDefault() == null ? 2 : 3);
                eventdata.color = colorvalue(GetEventattend(data.FirstOrDefault(q => q.EntityId == eventdata.Id).EntityId, eventChatAttends), data.FirstOrDefault(q => q.EntityId == eventdata.Id).totalnumbert);
            }

            return (dataList, totalRowCount);

        }


        public async Task<List<EventData>> InsertEvents(List<EventData> eventsData)
        {            
            await _authContext.EventData.AddRangeAsync(eventsData);
            await _authContext.SaveChangesAsync();
            return eventsData;
        }


        public async Task<List<EventChatAttend>> InsertEventChatAttends(List<EventChatAttend> eventChatAttends)
        {
            await _authContext.EventChatAttend.AddRangeAsync(eventChatAttends);
            await _authContext.SaveChangesAsync();

            return (eventChatAttends);
        }



        public async void UpdateExistEvent(EventData existEventData, AddExternalEventDataModel newEventData, int eventType)
        {

            existEventData.Title = newEventData.Title;
            existEventData.eventtype = Guid.Parse("265583AA-2511-4FD3-883E-6CAF1F8E4355");
            existEventData.image = newEventData.image;
            existEventData.description = newEventData.description;
            existEventData.status = newEventData.status;
            existEventData.categorieId = ExtractEventCategory(newEventData.categorie.Trim());
            existEventData.EventTypeListid = eventType;
            existEventData.lang = newEventData.longitude;
            existEventData.lat = newEventData.latitude;
            existEventData.allday = newEventData.allday;
            existEventData.totalnumbert = newEventData.totalnumbert == 0 ? 1000 : newEventData.totalnumbert;
            existEventData.eventdate = newEventData.eventdate;
            existEventData.eventdateto = newEventData.eventdateto;
            existEventData.eventfrom = newEventData.timefrom;
            existEventData.eventto = newEventData.timeto;
            existEventData.CityID = newEventData.CityID;
            existEventData.CountryID = newEventData.CountryID;
            existEventData.StopTo = newEventData.StopTo;
            existEventData.StopFrom = newEventData.StopFrom;
            existEventData.checkout_details = newEventData.checkout_details;
            existEventData.showAttendees = newEventData.showAttendees;
            existEventData.IsActive = true;

        }


        public async Task UpdateEventAddressFromGoogle(EventData eventData)
        {
            try
            {
                AddressData AddressData = googleLocationService.GetAddressDataNotAsync(Convert.ToDouble(eventData.lat), Convert.ToDouble(eventData.lang));
                if (AddressData != null)
                {
                    var Country = await countryService.Create(new CountryVM()
                    {
                        GoogleName = AddressData.Country,
                        DisplayName = AddressData.Country
                    });
                    var City = await cityService.Create(new CityVM()
                    {
                        GoogleName = string.IsNullOrEmpty(AddressData.City) ? AddressData.State : AddressData.City,
                        DisplayName = string.IsNullOrEmpty(AddressData.City) ? AddressData.State : AddressData.City
                    });
                    eventData.CityID = City.Data.ID;
                    eventData.CountryID = Country.Data.ID;
                    //_authContext.Attach(eventData);
                    //_authContext.Entry(eventData).State = EntityState.Modified;
                    _authContext.EventData.Update(eventData);
                    _authContext.SaveChanges();
                }

            }
            catch (Exception ex)
            {
                //_authContext.SaveChanges();
            }
        }

        public async Task UpdateEventsAddressFromGoogle(List<EventData> eventsData)
        {
            try
            {
               // List<EventData> events = await GetExternaleventsByIds(eventsData.Select(q => q.Id).ToList());
                List<Country> countries = await _authContext.Countries.ToListAsync();
                List<City> cities = await _authContext.Cities.ToListAsync();

                foreach (var eventdata in eventsData)
                {
                    var AddressData = googleLocationService.GetAddressDataNotAsync(Convert.ToDouble(eventdata.lat), Convert.ToDouble(eventdata.lang));

                    if (AddressData != null)
                    {
                        var country = countries.FirstOrDefault(x => x.GoogleName == AddressData.Country);
                        if (country == null)
                        {
                            Country newCountry = newCountry = new Country()
                            {
                                DisplayName = AddressData.Country,
                                GoogleName = AddressData.Country

                            };
                            _authContext.Countries.Add(newCountry);
                            _authContext.SaveChanges();

                            eventdata.CountryID = newCountry.ID;
                        }
                        else
                        {
                            eventdata.CountryID = country.ID;
                        }

                        var city = cities.FirstOrDefault(x => x.GoogleName == AddressData.City);

                        if (city == null)
                        {
                            City newCity = new City()
                            {
                                DisplayName = AddressData.City,
                                GoogleName = AddressData.City
                            };
                            _authContext.Cities.Add(newCity);
                            _authContext.SaveChanges();

                            eventdata.CityID = newCity.ID;
                        }
                        else
                        {
                            eventdata.CityID = city.ID;
                        }

                    }
                    _authContext.Attach(eventdata);
                    _authContext.Entry(eventdata).State = EntityState.Modified;
                }
                int result = _authContext.SaveChanges();

            }
            catch (Exception ex)
            {
            }
        }



        public int ExtractEventCategory(string evntCategory)
        {

            if (ExternalCategory.CreativeAndArt.Contains(evntCategory))
            {
                return 18;
            }
            if (ExternalCategory.Music.Contains(evntCategory))
            {
                return 1;
            }
            if (ExternalCategory.TheatreAndFilm.Contains(evntCategory))
            {
                return 19;
            }
            if (ExternalCategory.Dance.Contains(evntCategory))
            {
                return 20;
            }
            if (ExternalCategory.Comedy.Contains(evntCategory))
            {
                return 21;
            }
            if (ExternalCategory.SportsAndFitness.Contains(evntCategory))
            {
                return 22;
            }
            if (ExternalCategory.Exhibition.Contains(evntCategory))
            {
                return 23;
            }
            if (ExternalCategory.ClassesAndWorkshops.Contains(evntCategory))
            {
                return 24;
            }
            if (ExternalCategory.WalksAndTours.Contains(evntCategory))
            {
                return 25;
            }
            if (ExternalCategory.FoodAndDrink.Contains(evntCategory))
            {
                return 9;
            }
            if (ExternalCategory.Community.Contains(evntCategory))
            {
                return 27;

            }
            else
            {
                return 16;
            }
        }

        private async Task<List<EventData>> GetExternaleventsByIds(List<int> eventsids)
        {
            List<EventData> data = await _authContext.EventData.Where(m => eventsids.Contains(m.Id) && m.IsActive == true && m.EventTypeListid == 3 && (m.StopFrom != null ? (m.StopFrom.Value >= DateTime.Now.Date || m.StopTo.Value <= DateTime.Now.Date) : true)).ToListAsync();

            return data;
        }
        #endregion

    }
}
