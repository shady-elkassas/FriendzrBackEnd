using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Social.Services.ModelView;
using Social.Entity.DBContext;
using Social.Services.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Social.Services.Attributes;
using Social.Entity.Enums;
using Social.Services.Helpers;

namespace Social.Areas.WhiteLable.Controllers
{
    [Area("Whitelabel")]
    [ServiceFilter(typeof(AuthorizeWhiteLable))]
    public class HomeController : Controller
    {
        private readonly AuthDBContext _authDBContext;        
        private readonly IUserReportService _userReportService;
        private readonly IEventReportService _eventReportService;
        private readonly IAppConfigrationService _appConfigrationService;
        private readonly IDistanceFilterHistoryService _distanceFilterHistoryService;
        private readonly IFilteringAccordingToAgeHistoryService _filteringAccordingToAgeHistoryService;
        private readonly IEventServ _Event;

        public HomeController(AuthDBContext authDBContext,
            IUserReportService userReportService,
            IDistanceFilterHistoryService distanceFilterHistoryService,
            IFilteringAccordingToAgeHistoryService filteringAccordingToAgeHistoryService,
            IEventReportService eventReportService, IAppConfigrationService appConfigrationService, IEventServ _event)
        {
            _authDBContext = authDBContext;
            _userReportService = userReportService;
            _distanceFilterHistoryService = distanceFilterHistoryService;
            _filteringAccordingToAgeHistoryService = filteringAccordingToAgeHistoryService;
            _eventReportService = eventReportService;
            _appConfigrationService = appConfigrationService;
            _Event = _event;
        }
        public IActionResult Index()
        {
            var DbF = Microsoft.EntityFrameworkCore.EF.Functions;
            var DateTimeNow = DateTime.UtcNow.Date;
            var loggedinUser = HttpContext.GetUser();
            var userDeatils = loggedinUser.User.UserDetails;
            var allEvents = _authDBContext.EventData.Where(n => n.UserId == userDeatils.PrimaryId && n.IsActive == true && (n.EventTypeListid == 5
                                     || n.EventTypeListid == 6)).ToList();           
            //var messages = _authDBContext.Messagedata.Where(m => allEvents.Select(a => a.Id).ToList().Contains(m.EventDataid.Value));
            var allEventAttends = _authDBContext.EventChatAttend.Where(e => allEvents.Select(a => a.Id).Contains(e.EventDataid));            
            var AttendedUsers= allEventAttends.Where(c=>c.stutus !=2).Select(e=>e.Userattend);
            var actualUsers = AttendedUsers.Where(x => x.Email.ToLower().Contains("@owner") == false && !x.IsWhiteLabel.Value);

            var Users = _authDBContext.Users.Where(x => x.Email.ToLower().Contains("@owner") == false && !x.UserDetails.IsWhiteLabel.Value);

            UserStatistics UserStatistics = new UserStatistics()
            {
                //CurrenUsers_Count = Users.Count(),
                //ConfirmedMailUsers_Count = Users.Where(x => x.EmailConfirmed).Count(),
                Updated= actualUsers.Count(),
                DeletedUsers_Count = _authDBContext.EventChatAttend.Where(c => c.stutus != 1).Count(),
                UnConfirmedMailUsers_Count = Users.Where(x => !x.EmailConfirmed).Count(),
                Male_Count = actualUsers.Where(x => x.Gender == "male").Count(),
                Female_Count = actualUsers.Where(x => x.Gender == "female").Count(),
            };
            ViewBag.ListOFCities = _authDBContext.Cities.Select(x => new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.DisplayName,
                Selected = x.GoogleName.ToLower() == "london"
            }).ToList();
            ViewBag.Cateories = _authDBContext.category.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.name,
            }).ToList();
            var eventDataadminMV = new EventDataadminMV();           
            ViewBag.EventDataadminMV = eventDataadminMV;
            return View(UserStatistics);
        }
        public IActionResult AppStatistics()
        {
            var DbF = Microsoft.EntityFrameworkCore.EF.Functions;
            var DateTimeNow = DateTime.UtcNow.Date;
            var Users = _authDBContext.Users.Where(x => x.Email.ToLower().Contains("@owner") == false);
            UserStatistics UserStatistics = new UserStatistics()
            {
                CurrenUsers_Count = Users.Count(),
                ConfirmedMailUsers_Count = Users.Where(x => x.EmailConfirmed).Count(),
                DeletedUsers_Count = _authDBContext.DeletedUsersLogs.Count(),
                UnConfirmedMailUsers_Count = Users.Where(x => !x.EmailConfirmed).Count(),
                Male_Count = Users.Where(x => x.UserDetails.Gender == "male").Count(),
                Female_Count = Users.Where(x => x.UserDetails.Gender == "female").Count(),
            };
            return View(UserStatistics);
        }
        public IActionResult GetInfoAboutUsers()
        {
            var DbF = Microsoft.EntityFrameworkCore.EF.Functions;
            var DateTimeNow = DateTime.UtcNow.Date;
            var allrequestes = _authDBContext.Requestes;
            var UserDetails = _authDBContext.UserDetails.Where(x => x.birthdate != null).Where(x => x.Email.ToLower().Contains("@owner") == false);
            var UseresWithBirthDate = UserDetails.Select(x => DbF.DateDiffYear(x.birthdate, DateTimeNow));
            var tt = Math.Round(Convert.ToDouble(UseresWithBirthDate.Sum(x => x ?? 0)) / UseresWithBirthDate.Count(), 2);
            var users = _authDBContext.Users.Where(x => x.Email.ToLower().Contains("@owner") == false && x.UserDetails.birthdate != null);
            var usersEnableGhostMode = UserDetails.Where(x => x.ghostmode == true);
            var UsersBirthDate = UserDetails.Where(x => x.birthdate != null).Select(x => x.birthdate);
            var TotalUserUsedAgeFiltring = _authDBContext.FilteringAccordingToAgeHistory.Select(x => x.UserID).Distinct().Count();

            var MaxAgeFiltringRangeUsed = _authDBContext.FilteringAccordingToAgeHistory.Select(x => new { x.AgeFrom, x.AgeTo, x.UserID }).ToList().GroupBy(x => new { x.AgeFrom, x.AgeTo }).Select(x => new { Count = x.Count(), Range = x.Key.AgeFrom + " - " + x.Key.AgeTo }).OrderByDescending(x => x.Count).FirstOrDefault();
            UserStatistics UserStatistics = new UserStatistics()
            {
                CurrenUsers_Count = users.Count(),
                TotalUserUsedAgeFiltring = TotalUserUsedAgeFiltring,
                MostAgeFiltirngRangeUsed = MaxAgeFiltringRangeUsed?.Range ?? "0 - 0",
                MostAgeFiltirngRangeUsed_Rate = Math.Round(MaxAgeFiltringRangeUsed?.Count ?? 0 / ((double)TotalUserUsedAgeFiltring), 2),
                deactivatespushnotifications_Count = users.Where(x => x.UserDetails.pushnotification == false).Count(),
                UseresAverageAge = tt,
                TotalInAppearanceTypes = _authDBContext.AppearanceTypes_UserDetails.Count(),
                AppearenceEveryOneInGhostMode_Count = _authDBContext.AppearanceTypes_UserDetails.Where(x => x.AppearanceTypeID == 1).Count(),
                AppearenceMaleInGhostMode_Count = _authDBContext.AppearanceTypes_UserDetails.Where(x => x.AppearanceTypeID == 2).Count(),
                AppearenceFemaleInGhostMode_Count = _authDBContext.AppearanceTypes_UserDetails.Where(x => x.AppearanceTypeID == 3).Count(),
                AppearenceOtherGenderInGhostMode_Count = _authDBContext.AppearanceTypes_UserDetails.Where(x => x.AppearanceTypeID == 4).Count(),
                RequestesCount = allrequestes.Count(),
                UserWithLessThan18Age_Count = UsersBirthDate.Count(x => DbF.DateDiffYear(x, DateTimeNow) < 18),
                UsersWith18_24Age_Count = UsersBirthDate.Count(x => DbF.DateDiffYear(x, DateTimeNow) >= 18 && DbF.DateDiffYear(x, DateTimeNow) <= 24),
                UsersWith25_34Age_Count = UsersBirthDate.Count(x => DbF.DateDiffYear(x, DateTimeNow) >= 25 && DbF.DateDiffYear(x, DateTimeNow) <= 34),
                UsersWith35_54Age_Count = UsersBirthDate.Count(x => DbF.DateDiffYear(x, DateTimeNow) >= 35 && DbF.DateDiffYear(x, DateTimeNow) <= 54),
                UsersWithMoreThan55Age_Count = UsersBirthDate.Count(x => DbF.DateDiffYear(x, DateTimeNow) >= 55),
                BlockRequestesCount = allrequestes.Where(x => x.status == 3).Count(),
                AcceptedRequestesCount = allrequestes.Where(x => x.status == 2).Count(),
                PendindingRequestesCount = allrequestes.Where(x => x.status == 1).Count(),
                UseresStillUseAppAfter3Months_Count = UserDetails.Where(x => DbF.DateDiffMonth(x.User.RegistrationDate, x.LastFeedRequestDate.Date) >= 3).Count(),
                //NotactiveUsers_Count = UserDetails.Where(x => DbF.DateDiffDay(x.LastFeedRequestDate.Date, DateTimeNow) > 7).Count(),
                UseresEnableGhostMode_Count = usersEnableGhostMode.Count(),
                ConfirmedMailUsers_Count = users.Where(x => x.EmailConfirmed).Count(),
                DeletedUsers_Count = _authDBContext.DeletedUsersLogs.Count(),
                UnConfirmedMailUsers_Count = users.Where(x => !x.EmailConfirmed).Count(),
                Male_Count = users.Where(x => x.UserDetails.Gender == "male").Count(),
                Female_Count = users.Where(x => x.UserDetails.Gender == "female").Count(),
                NeedUpdate = UserDetails.Where(n => n.birthdate == null).Count(),
                Updated = UserDetails.Where(n => n.birthdate != null).Count(),
                personalspace = UserDetails.Where(n => n.personalSpace == true).Count(),
            };
            return Ok(JObject.FromObject(new { UsersInfo = UserStatistics }, new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));

        }
        public IActionResult GetInfoAboutEvents()
        {
            var loggedinUser = HttpContext.GetUser();

            var userDeatils = loggedinUser.User.UserDetails;
            var DbF = Microsoft.EntityFrameworkCore.EF.Functions;
            var DateTimeNow = DateTime.UtcNow.Date;

            var allEvents = _authDBContext.EventData.Where(n =>n.UserId== userDeatils.PrimaryId&& n.IsActive == true
                                    && (n.EventTypeListid == 5 || n.EventTypeListid == 6)).ToList();

            var allEventAttends = _authDBContext.EventChatAttend.Where(e => allEvents.Select(a => a.Id).Contains(e.EventDataid));

             allEventAttends = allEventAttends.Where(c => c.stutus != 2 && c.Userattend.Email.ToLower().Contains("@owner") == false
                               && !c.Userattend.IsWhiteLabel.Value);         

            //var sharedevent = _authDBContext.EventTrackers.Where(m => m.ActionType == EventActionType.Share.ToString()).Select(m => m.EventId).Distinct();
            var averageOfParticibated = Math.Round(Convert.ToDouble(allEventAttends.GroupBy(x => x.EventDataid).Select(x => x.Count()).Sum()) / allEvents.Count(), 2);
            var averageOfParticibatedInExisteEvent = Math.Round(Convert.ToDouble(allEventAttends.Where(q => q.EventData.eventdateto < DateTime.Now).GroupBy(x => x.EventDataid).Select(x => x.Count()).Sum()) / allEvents.Count(), 2);
            try
            { var LAWcapacityK = allEventAttends.GroupBy(x => x.EventDataid).Count(); }
            catch (Exception EX)
            {
                var T = EX;
            }
            //comment
            //var COUNTLIST = allEventAttends.GroupBy(x => x.EventDataid).Select(x => new { COUNT = x.Count(), totalnumbert =100 /*x.FirstOrDefault().EventData.totalnumbert*/ }).ToList();

            //var LAWcapacity = COUNTLIST.Where(N => (N.COUNT * 100 / N.totalnumbert) <= 40).Count();
            //var mediumcapacity = COUNTLIST.Where(N => (N.COUNT * 100 / N.totalnumbert) > 40 && (N.COUNT * 100 / N.totalnumbert) <= 75).Count();
            //var highcapacity = COUNTLIST.Where(N => (N.COUNT * 100 / N.totalnumbert) > 75 && (N.COUNT * 100 / N.totalnumbert) <= 100).Count();
            //
            var eventsinfo = new
            {
                allEventsCount = allEvents.Count(),
                NumberOfExisteEvent = allEvents.Where(q => q.eventdateto.Value.Date > DateTime.Now.Date || (q.eventdateto.Value.Date == DateTime.Now.Date && q.eventto > DateTime.Now.TimeOfDay) || (q.allday == true && q.eventdate.Value.Date == DateTime.Now.Date)).Count(),
                NumberOfFinishedEvent = allEvents.Where(q => q.eventdateto.Value.Date < DateTime.Now.Date || (q.eventdateto.Value.Date == DateTime.Now.Date && q.eventto <= DateTime.Now.TimeOfDay) || (q.allday == true && q.eventdate.Value.Date < DateTime.Now.Date)).Count(),
                NumberOfPrivateEvent = allEvents.Where(q => q.EventTypeListid == 6).Count(),
                //NumberOfFriendzrEvent = allEvents.Where(q => q.EventTypeListid == 2).Count(),
                //NumberOfExternalEvent = allEvents.Where(q => q.EventTypeListid == 3).Count(),
                //NumberOfadminExternalEvent = allEvents.Where(q => q.EventTypeListid == 4).Count(),
                //NumberOfWhiteLableEvent = allEvents.Where(q => q.EventTypeListid == 5).Count(),
                averageOfParticibated = averageOfParticibated,
                averageOfParticibatedInExisteEvent = averageOfParticibatedInExisteEvent,

                eventsafter3months = allEvents.Where(x => DbF.DateDiffMonth(x.eventdate, DateTimeNow) >= 3).Count(),
                NumberOfDeletedEvents = _authDBContext.DeletedEventLogs.Count(),
                NumberOfUseresWhoCreatedEvents = allEvents.Where(q => !q.User.Email.ToLower().Contains("@owner") && q.User.birthdate != null && q.User.User.EmailConfirmed == true && (q.User.ProfileCompleted != null && q.User.ProfileCompleted == true)).GroupBy(x => x.UserId).Select(x => x.Key).Count(),
                //LOWcapacity = LAWcapacity,
                //mediumcapacity = mediumcapacity,
                //highcapacity = highcapacity,
                //sharedevent = sharedevent.Count(),



                attendes = allEventAttends.Count(),
            };
            return Ok(JObject.FromObject(new { EventsInfo = eventsinfo }, new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));

        }
        public IActionResult UserStatictesPerMonth(int? Year)
        {
            //var AdminUsers=_authDBContext.Users.Where(z=>z.is)
            Year = Year ?? DateTime.Now.Year;
            var allUsers = _authDBContext.Users.Where(x => x.Email.ToLower().Contains("@owner") == false).Select(x => new
            {
                RegstrationDate = x.RegistrationDate,
                UserName = x.UserName,
                Gender = x.UserDetails.Gender
            }).AsEnumerable().Where(x => x.RegstrationDate.Year == Year).GroupBy(x => x.RegstrationDate.Date.Month).Select(x => new { Month = x.Key, Count = x.Count() });
            var DeactiveUsers = _authDBContext.DeletedUsersLogs.Select(x => new
            {
                RegstrationDate = x.DateTime,
                UserName = "",
                Gender = x.Gender
            }).AsEnumerable().Where(x => x.RegstrationDate.Year == Year).GroupBy(x => x.RegstrationDate.Date.Month).Select(x => new { Month = x.Key, Count = x.Count() });
            var Monthes = Enumerable.Range(1, 12).Select(i => new { indx = i, Month = DateTimeFormatInfo.CurrentInfo.GetAbbreviatedMonthName(i) });
            var res = Monthes.Select(x => new
            {
                Months = x.Month,
                ActiveUsers = allUsers.FirstOrDefault(xx => x.indx == xx.Month)?.Count ?? 0,
                DeactiveUsers = DeactiveUsers.FirstOrDefault(xx => x.indx == xx.Month)?.Count ?? 0,
            }).ToList();
            return Ok(new { UserStatictes = res });
        }
        public IActionResult allUserStatictesPerMonth(int? Year)
        {
            var DbF = Microsoft.EntityFrameworkCore.EF.Functions;
            Year = Year ?? DateTime.Now.Year;
            var DateTimeNow = DateTime.UtcNow.Date;
            DateTime firstDay = new DateTime((int)Year, 1, 1);
            DateTime lastDay = new DateTime((int)Year, 12, 31);
            var allUsersInYear = _authDBContext.Users.Where(x => x.Email.ToLower().Contains("@owner") == false).Where(x => DbF.DateDiffDay(firstDay, x.RegistrationDate) >= 0 && DbF.DateDiffDay(x.RegistrationDate, lastDay) >= 0);
            var allRequestesInYear = _authDBContext.Requestes.Where(x => DbF.DateDiffDay(firstDay, x.regestdata) >= 0 && DbF.DateDiffDay(x.regestdata, lastDay) >= 0);
            var ActiveUseresPerMonth = allUsersInYear.GroupBy(x => x.RegistrationDate.Date.Month).Select(x => new
            {
                Month = x.Key,
                Count = x.Count()
            }).OrderBy(x => x.Month).ToList();
            var ConfirmedMailUseresPerMonth = allUsersInYear.Where(x => x.EmailConfirmed == true).GroupBy(x => x.RegistrationDate.Date.Month).Select(x => new
            {
                Month = x.Key,
                Count = x.Count()
            }).OrderBy(x => x.Month).ToList();
            var GostModeUseresPerMonth = allUsersInYear.Where(x => x.UserDetails.ghostmode == true).GroupBy(x => x.RegistrationDate.Date.Month).Select(x => new
            {
                Month = x.Key,
                Count = x.Count()
            }).OrderBy(x => x.Month).ToList();
            var disablePushNotficationPerMonth = allUsersInYear.Where(x => x.UserDetails.pushnotification == false).GroupBy(x => x.RegistrationDate.Date.Month).Select(x => new
            {
                Month = x.Key,
                Count = x.Count()
            }).OrderBy(x => x.Month).ToList();
            var NotConfirmedMailUseresPerMonth = allUsersInYear.Where(x => x.EmailConfirmed == false).GroupBy(x => x.RegistrationDate.Date.Month).Select(x => new
            {
                Month = x.Key,
                Count = x.Count()
            }).OrderBy(x => x.Month).ToList();
            //var NotActiveUseresPerMonth = allUsersInYear.Where(x => DbF.DateDiffDay( x.UserDetails.LastFeedRequestDate.Date,DateTimeNow) >7).GroupBy(x => x.UserDetails.LastFeedRequestDate.Date.Month).Select(x => new
            //{
            //    Month = x.Key,
            //    Count = x.Count()
            //}).OrderBy(x => x.Month).ToList();
            var NumberOfRequestesPerMonth = allRequestesInYear.Where(x => x.status != 3 /*Block*/).GroupBy(x => x.regestdata.Date.Month).Select(x => new
            {
                Month = x.Key,
                Count = x.Count()
            }).OrderBy(x => x.Month).ToList();
            var NumberOfBlockRequestesPerMonth = allRequestesInYear.Where(x => x.status == 3 /*Block*/).GroupBy(x => x.regestdata.Date.Month).Select(x => new
            {
                Month = x.Key,
                Count = x.Count()
            }).OrderBy(x => x.Month).ToList();
            var DeactiveUsers = _authDBContext.DeletedUsersLogs.Where(x => DbF.DateDiffDay(firstDay, x.DateTime) >= 0 && DbF.DateDiffDay(x.DateTime, lastDay) >= 0).GroupBy(x => x.DateTime.Date.Month).Select(x => new { Month = x.Key, Count = x.Count() }).OrderBy(x => x.Month);
            var Monthes = Enumerable.Range(1, 12).Select(i => new { indx = i, Month = DateTimeFormatInfo.CurrentInfo.GetAbbreviatedMonthName(i) });
            var Results = Monthes.Select(x => new
            {
                Months = x.Month,
                registered = ActiveUseresPerMonth.FirstOrDefault(xx => x.indx == xx.Month)?.Count ?? 0,
                DeactiveUsers = DeactiveUsers.FirstOrDefault(xx => x.indx == xx.Month)?.Count ?? 0,
                //NotActiveUseresPerMonth = NotActiveUseresPerMonth.FirstOrDefault(xx => x.indx == xx.Month)?.Count ?? 0,
                ConfirmedMailUseresPerMonth = ConfirmedMailUseresPerMonth.FirstOrDefault(xx => x.indx == xx.Month)?.Count ?? 0,
                NotConfirmedMailUseresPerMonth = NotConfirmedMailUseresPerMonth.FirstOrDefault(xx => x.indx == xx.Month)?.Count ?? 0,
                GostModeUseresPerMonth = GostModeUseresPerMonth.FirstOrDefault(xx => x.indx == xx.Month)?.Count ?? 0,
                disablePushNotficationPerMonth = disablePushNotficationPerMonth.FirstOrDefault(xx => x.indx == xx.Month)?.Count ?? 0,
                NumberOfRequestesPerMonth = NumberOfRequestesPerMonth.FirstOrDefault(xx => x.indx == xx.Month)?.Count ?? 0,
                NumberOfBlockRequestesPerMonth = NumberOfBlockRequestesPerMonth.FirstOrDefault(xx => x.indx == xx.Month)?.Count ?? 0,
            }).ToList();
            var series = new List<object>()
            {
                new {name="Registered",data=Results.Select(x=>x.registered)},
                new {name="Deleted",data=Results.Select(x=>x.DeactiveUsers)},
                //new {name="Not Active Useres",data=Results.Select(x=>x.NotActiveUseresPerMonth)},
                new {name="Confirmed Email",data=Results.Select(x=>x.ConfirmedMailUseresPerMonth)},
                new {name="Not confirmed Email",data=Results.Select(x=>x.NotConfirmedMailUseresPerMonth)},
                new {name="Ghost Mode",data=Results.Select(x=>x.GostModeUseresPerMonth)},
                new {name="Disable Push Notification",data=Results.Select(x=>x.disablePushNotficationPerMonth)},
                new {name="all Requestes",data=Results.Select(x=>x.NumberOfRequestesPerMonth)},
                new {name="Blocks",data=Results.Select(x=>x.NumberOfBlockRequestesPerMonth)},
            };
            return Ok(new { Monthes = Monthes, series = series });
        }
        public IActionResult EventsInEachCategory(int? Year)
        {

            var Categories = _authDBContext.category.Select(x => new { CategoryName = x.name, NumOfCreatedEvent = x.EventData.Count });
            var series = new List<object>()
            {
                new {name="NumberOfCreatedEvents",data=Categories.Select(x=>x.NumOfCreatedEvent)},

            };
            return Ok(new { categories = Categories.Select(x => x.CategoryName), series = series });
        }

        public IActionResult AppStatictesPerMonth(int Year)
        {
            var distancelist = _distanceFilterHistoryService.GetAll(Year).ToList();
            var alldistanceFilterHistory = distancelist.GroupBy(x => x.Month).Select(x => new { Month = x.Key, Count = x.Count() }).OrderBy(x => x.Month).ToList();
            //var allfilteringAccordingToAgeHistory = filteringAccordingToAgeHistoryService.GetAll(Year).GroupBy(x => x.Month).Select(x => new { Month = x.Key, Count = x.Count() }).OrderBy(x => x.Month).ToList();

            int distsum = distancelist.Count() == 0 ? 1 : distancelist.Count();
            var distance = distancelist.GroupBy(x => x.Month).Select(x => new { Month = x.Key, Count = (int)(x.Select(m => m.destance).Sum() / distsum) }).OrderBy(x => x.Month).ToList();

            var Monthes = Enumerable.Range(1, 12).Select(i => new { indx = i, Month = DateTimeFormatInfo.CurrentInfo.GetAbbreviatedMonthName(i) });
            var Results = Monthes.Select(x => new
            {
                Months = x.Month,
                distanceFilterHistory = alldistanceFilterHistory.FirstOrDefault(xx => x.indx == xx.Month)?.Count ?? 0,
                // filteringAccordingToAgeHistory = allfilteringAccordingToAgeHistory.FirstOrDefault(xx => x.indx == xx.Month)?.Count ?? 0,
                data = distance.FirstOrDefault(xx => x.indx == xx.Month)?.Count ?? 0
            }).ToList();

            var series = new List<object>()
            {
                new {name="distance Filter",data=Results.Select(x=>x.distanceFilterHistory)},
               // new {name="filteringAccordingToAge",data=Results.Select(x=>x.filteringAccordingToAgeHistory)},
                 new {name="average range of distance",data=Results.Select(x=>x.data)},
            };
            return Ok(new { Monthes = Monthes, series = series });
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
    }
}
