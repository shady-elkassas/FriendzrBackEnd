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
using System.Threading.Tasks;
using ClosedXML.Excel;
using System.IO;
using Microsoft.AspNetCore.Http;
using Social.Entity.Models;
using System.Text.Json;
using Newtonsoft.Json;

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
        private readonly IUserService _userService;

        public HomeController(AuthDBContext authDBContext,
            IUserReportService userReportService,
            IDistanceFilterHistoryService distanceFilterHistoryService,
            IFilteringAccordingToAgeHistoryService filteringAccordingToAgeHistoryService,
            IEventReportService eventReportService, IAppConfigrationService appConfigrationService, IEventServ _event, IUserService userService)
        {
            _authDBContext = authDBContext;
            _userReportService = userReportService;
            _distanceFilterHistoryService = distanceFilterHistoryService;
            _filteringAccordingToAgeHistoryService = filteringAccordingToAgeHistoryService;
            _eventReportService = eventReportService;
            _appConfigrationService = appConfigrationService;
            _Event = _event;
            _userService = userService;
        }
        public IActionResult Index()
        {
            var DbF = Microsoft.EntityFrameworkCore.EF.Functions;
            var DateTimeNow = DateTime.UtcNow.Date;
            var loggedinUser = HttpContext.GetUser();
            var userDeatils = loggedinUser.User.UserDetails;
            
            var actualUsers= _authDBContext.Users.Include(u=>u.UserDetails).Where(x => x.Email.ToLower().Contains("@owner") == false && !x.UserDetails.IsWhiteLabel.Value && x.UserDetails.Code== userDeatils.Code);
            var Users = _authDBContext.Users.Where(x => x.Email.ToLower().Contains("@owner") == false && !x.UserDetails.IsWhiteLabel.Value);

            UserStatistics UserStatistics = new UserStatistics()
            {
                CurrenUsers_Count = Users.Count(),
                ConfirmedMailUsers_Count = Users.Where(x => x.EmailConfirmed).Count(),
                Updated = actualUsers.Count(),
                DeletedUsers_Count = _authDBContext.EventChatAttend.Where(c => c.stutus != 1).Count(),
                UnConfirmedMailUsers_Count = Users.Where(x => !x.EmailConfirmed).Count(),
                Male_Count = actualUsers.Where(x => x.UserDetails.Gender == "male").Count(),
                Female_Count = actualUsers.Where(x => x.UserDetails.Gender == "female").Count(),
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

            var allEvents = _authDBContext.EventData.Where(n => n.UserId == userDeatils.PrimaryId && n.IsActive == true
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

        [HttpGet]
        public IActionResult GetUsersStatistics(int ID)
        {
            var loggedinUser = HttpContext.GetUser();
            var userDeatils = loggedinUser.User.UserDetails;
            var choosedEvent = _authDBContext.EventData.Where(n => n.UserId == userDeatils.PrimaryId && n.Id == ID && n.IsActive == true && (n.EventTypeListid == 5
                                       || n.EventTypeListid == 6)).ToList();
            var allEventAttends = _authDBContext.EventChatAttend.Where(e => choosedEvent.Select(a => a.Id).Contains(e.EventDataid));
            var AttendedUsers = allEventAttends.Where(c => c.stutus != 2).Select(e => e.Userattend);
            var actualUsers = AttendedUsers.Where(x => x.Email.ToLower().Contains("@owner") == false && !x.IsWhiteLabel.Value);

            var Users = _authDBContext.Users.Where(x => x.Email.ToLower().Contains("@owner") == false && !x.UserDetails.IsWhiteLabel.Value);

            UserStatistics UserStatistics = new UserStatistics()
            {
                Updated = actualUsers.Count(),
                DeletedUsers_Count = _authDBContext.EventChatAttend.Where(c => c.stutus != 1).Count(),
                UnConfirmedMailUsers_Count = Users.Where(x => !x.EmailConfirmed).Count(),
                Male_Count = actualUsers.Where(x => x.Gender == "male").Count(),
                Female_Count = actualUsers.Where(x => x.Gender == "female").Count(),
            };
            
            var data = JObject.FromObject(UserStatistics, new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() });

            return Ok(data);

        }
        public  IActionResult UsersInEachInterest(int Id)
        {
            var loggedinUser = HttpContext.GetUser();
            var userDeatils = loggedinUser.User.UserDetails;
            var choosedEvent = _authDBContext.EventData.Where(n => n.UserId == userDeatils.PrimaryId && n.Id == Id && n.IsActive == true && (n.EventTypeListid == 5
                                       || n.EventTypeListid == 6)).ToList();
            var allEventAttends = _authDBContext.EventChatAttend.Where(e => choosedEvent.Select(a => a.Id).Contains(e.EventDataid));
            var AttendedUsers = allEventAttends.Where(c => c.stutus != 2).Select(e => e.Userattend);
            var actualUsers = AttendedUsers.Where(x => x.Email.ToLower().Contains("@owner") == false && !x.IsWhiteLabel.Value).ToList();
            var userIds = actualUsers.Select(u => u.PrimaryId).ToList();
            var listofTages =  _authDBContext.listoftags.Where(l => userIds.Contains(l.UserId.Value)).ToList();
            var interestsCommon =  _authDBContext.Interests.Where(i => listofTages.Select(l=>l.InterestsId).Distinct().Contains(i.Id)).ToList();
            var interests = interestsCommon.Select(q => new { InterestName = q.name, NumOfUsers = listofTages.Where(l=>l.InterestsId== q.Id).ToList().Count });
            //var interests = await _authDBContext.Interests.Include(q => q.listoftags).Where(i=>i.listoftags.
            //Where(li=>userIds.Contains(li.UserId.Value)).Any()).Select(q => new { InterestName = q.name, NumOfUsers = q.listoftags.Count() }).ToListAsync();

            var series = new List<object>()
            {
                new {name="NumberOfUsers",data=interests.Select(q=>q.NumOfUsers)},
            };

            return Ok(new { Interests = interests.Select(x => x.InterestName), series = series });
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
            var loggedinUser = HttpContext.GetUser();
            var userDeatils = loggedinUser.User.UserDetails;
            var EventData = _Event.GetEventbyid(EventID);
            var messageCount = _authDBContext.Messagedata.Include(m => m.EventChatAttend).Where(m => m.EventChatAttend.EventDataid == EventData.Id);

            var allEventAttends = _authDBContext.EventChatAttend.Where(e => e.EventDataid == EventData.Id);
            var attendedUsers = allEventAttends.Where(c => c.stutus != 2 && c.UserattendId!= userDeatils.PrimaryId).Select(e => e.Userattend);
            var actualUsers = attendedUsers.Where(x => x.Email.ToLower().Contains("@owner") == false && !x.IsWhiteLabel.Value);
            ViewBag.eventUsers = new UserStatistics()
            {
                Updated = actualUsers.Count(),
                Male_Count = actualUsers.Where(x => x.Gender == "male").Count(),
                Female_Count = actualUsers.Where(x => x.Gender == "female").Count(),
            };
            EventData.Messagedata = messageCount.ToList();
            return View(EventData);
        }

        public async Task<IActionResult> ExportWhitelabelStatisticsAsExcel()
        {
            string authorizationToken;
            HttpContext.Request.Cookies.TryGetValue("Authorization", out authorizationToken);

            var loggedinUser = this._userService.GetLoggedInUser(authorizationToken).Result;

            if (loggedinUser == null)
            {
                throw new Exception($"No User Found:{ StatusCodes.Status404NotFound}");
            }
            using (XLWorkbook workbook = new XLWorkbook())
            {
                var totalEventsData = this.GetTotalEventsQueryAsync(loggedinUser);
                this.GetTotalEventInExcel(workbook, totalEventsData);
                var statisticsEvent = this.EventStatisticsQuery(loggedinUser);
                this.GetEventStatisticsInExcel(workbook, statisticsEvent);
                var categories = this.EventInEachCategoryQuery(loggedinUser);
                this.GetEventInEachCategoryInExcel(workbook, categories);
                var eventAttendQuery = this.EventAttendQuery(loggedinUser);
                this.GetEventAttendInExcel(workbook, eventAttendQuery);
                var blockedUsers = await this.BlockedUsersQuery(loggedinUser);
                this.GetBlockedUsersInExcel(workbook, blockedUsers);
                var totalreport= TotalReportsQuery(loggedinUser);
                this.GetTotalReportsInExcel(workbook,totalreport);

                var reports = TotalReportsPerEventQuery(loggedinUser);
                this.GetTotalReportsPerEventInExcel(workbook,reports);


                using (MemoryStream stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    byte[] content = stream.ToArray();

                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "All Statistics.xlsx");
                }
            }
        }
        
        private IEnumerable<dynamic> GetTotalEventsQueryAsync(LoggedinUser loggedinUser)
        {
            var allEvents = _authDBContext.EventData.Where(n => n.IsActive == true && n.UserId == loggedinUser.User.UserDetails.PrimaryId && (n.EventTypeListid == 5
                            || n.EventTypeListid == 6)).ToList();
            var eventsIds = allEvents.Select(e => e.Id).ToList();
            var allEventAttends = _authDBContext.EventChatAttend.Where(a => eventsIds.Contains(a.EventDataid)).ToList();
            allEventAttends = allEventAttends.Where(c => c.stutus != 2 && c.Userattend.Email.ToLower().Contains("@owner") == false
             && !c.Userattend.IsWhiteLabel.Value).ToList();
            var averageOfParticibated = Math.Round(Convert.ToDouble(allEventAttends.GroupBy(x => x.EventDataid).Select(x => x.Count()).Sum()) / allEvents.Count(), 2);
            var averageOfParticibatedInExisteEvent = Math.Round(Convert.ToDouble(allEventAttends.Where(q => q.EventData.eventdateto < DateTime.Now)
                .GroupBy(x => x.EventDataid).Select(x => x.Count()).Sum()) / allEvents.Count(), 2);

            var COUNTLIST = allEventAttends.GroupBy(x => x.EventDataid).Select(x => new { COUNT = x.Count(), totalnumbert = x.FirstOrDefault().EventData.totalnumbert }).ToList();

            var LAWcapacity = COUNTLIST.Where(N => (N.COUNT * 100 / N.totalnumbert) <= 40).Count();
            var mediumcapacity = COUNTLIST.Where(N => (N.COUNT * 100 / N.totalnumbert) > 40 && (N.COUNT * 100 / N.totalnumbert) <= 75).Count();
            var highcapacity = COUNTLIST.Where(N => (N.COUNT * 100 / N.totalnumbert) > 75 && (N.COUNT * 100 / N.totalnumbert) <= 100).Count();

            var deletedEventsJsonData = _authDBContext.DeletedEventLogs.Select(e => e.EventDataJson).ToList();
            var deletedEventJobject = new List<JObject>();
            foreach (var deletedEvent in deletedEventsJsonData)
            {
                var data = (JObject)JsonConvert.DeserializeObject(deletedEvent);
                deletedEventJobject.Add(data);
            }
            var UserIds = new List<int>();
            foreach (var item in deletedEventJobject)
            {
                var userId = item.SelectToken("UserID").Value<int>();
                UserIds.Add(userId);
            }
            var eventsInfo = new
            {
                AllEventsCount = allEvents.Count(),
                NumberOfExisteEvent = allEvents.Where(q => q.eventdateto.Value.Date > DateTime.Now.Date || (q.eventdateto.Value.Date == DateTime.Now.Date && q.eventto > DateTime.Now.TimeOfDay) || (q.allday == true && q.eventdate.Value.Date == DateTime.Now.Date)).Count(),
                NumberOfFinishedEvent = allEvents.Where(q => q.eventdateto.Value.Date < DateTime.Now.Date || (q.eventdateto.Value.Date == DateTime.Now.Date && q.eventto <= DateTime.Now.TimeOfDay) || (q.allday == true && q.eventdate.Value.Date <= DateTime.Now.Date)).Count(),
                NumberOfPrivateEvent = allEvents.Where(q => q.EventTypeListid == 6).Count(),
                NumberOfPublicEvent = allEvents.Where(q => q.EventTypeListid == 5).Count(),
                AverageOfParticibated = averageOfParticibated,
                //Eventsafter3months = allEvents.Where(x => EF.Functions.DateDiffMonth(x.eventdate, DateTime.Now) >= 3).Count(),
                NumberOfDeletedEvents = UserIds.Where(u => u == loggedinUser.PrimaryId).Count(),
                averageOfParticibatedInExisteEvent = averageOfParticibatedInExisteEvent,
                LOWcapacity = LAWcapacity,
                Mediumcapacity = mediumcapacity,
                Highcapacity = highcapacity,
                Attendes = allEventAttends.Count(),
            };
            yield return eventsInfo;
        }

        private IEnumerable<dynamic> EventStatisticsQuery(LoggedinUser loggedinUser)
        {
            int year = DateTime.Now.Year;
            DateTime firstDay = new DateTime(year, 1, 1);
            DateTime lastDay = new DateTime(year, 12, 31);
            var monthes = Enumerable.Range(1, 12).Select(i => new { index = i, Month = DateTimeFormatInfo.CurrentInfo.GetAbbreviatedMonthName(i) });
            var allEventsInYear = _authDBContext.EventData.Where(n => n.IsActive == true && n.UserId == loggedinUser.User.UserDetails.PrimaryId && (n.EventTypeListid == 5
                          || n.EventTypeListid == 6)&& EF.Functions.DateDiffDay(firstDay, n.CreatedDate) >= 0 && EF.Functions.DateDiffDay(n.CreatedDate, lastDay) >= 0);

            var AllEventPerMonth = allEventsInYear.GroupBy(x => x.CreatedDate.Value.Date.Month).Select(x => new
            {
                Month = x.Key,
                Count = x.Count()
            }).OrderBy(x => x.Month).ToList();
            var publicEventPerMonth = allEventsInYear.Where(x => x.EventTypeListid == 5).GroupBy(x => x.CreatedDate.Value.Date.Month).Select(x => new
            {
                Month = x.Key,
                Count = x.Count()
            }).OrderBy(x => x.Month).ToList();
            var WhiteLableEventPerMonth = allEventsInYear.Where(x => x.EventTypeListid == 6).GroupBy(x => x.CreatedDate.Value.Date.Month).Select(x => new
            {
                Month = x.Key,
                Count = x.Count()
            }).OrderBy(x => x.Month).ToList();

            var eventStatistics = monthes.Select(x => new
            {
                Month = x.Month,
                AllEventPerMonth = AllEventPerMonth.FirstOrDefault(xx => x.index == xx.Month)?.Count ?? 0,        
                PublicEventPerMonth = publicEventPerMonth.FirstOrDefault(xx => x.index == xx.Month)?.Count ?? 0,
                WhiteLableEventPerMonth = WhiteLableEventPerMonth.FirstOrDefault(xx => x.index == xx.Month)?.Count ?? 0,
            }).ToList();

            return eventStatistics;
        }

        private IEnumerable<dynamic> EventInEachCategoryQuery(LoggedinUser loggedinUser)
        {
            var allEvents = _authDBContext.EventData.Where(n => n.IsActive == true && n.UserId == loggedinUser.User.UserDetails.PrimaryId && (n.EventTypeListid == 5
                              || n.EventTypeListid == 6)).ToList();
            var categorieIds = allEvents.Select(e => e.categorieId).Distinct().ToList();
            var categories = _authDBContext.category.Where(c => categorieIds.Contains(c.Id)).Select(x => new { CategoryName = x.name, CatId = x.Id });
            foreach (var category in categories)
            {
                var numofevents = allEvents.Where(e => e.categorieId== category.CatId).Count();
                yield return new
                {
                    CategoryName = category.CategoryName,
                    NumOfCreatedEvent = numofevents
                };
            }          
            
        }

        private ExportStatisticsByGenderAndAgeViewModel EventAttendQuery(LoggedinUser loggedinUser)
        {
            var allEvents = _authDBContext.EventData.Where(n => n.IsActive == true && n.UserId == loggedinUser.User.UserDetails.PrimaryId && (n.EventTypeListid == 5
                            || n.EventTypeListid == 6)).ToList();
            var eventsIds = allEvents.Select(e => e.Id).ToList();
            var monthes = Enumerable.Range(1, 12).Select(i => new { index = i, Month = DateTimeFormatInfo.CurrentInfo.GetAbbreviatedMonthName(i) });

            List<EventTracker> eventTrackers = _authDBContext.EventTrackers.Include(q => q.User).Include(q => q.Event).Where(q => eventsIds.Contains(q.EventId) && q.ActionType == EventActionType.Attend.ToString()
            && q.User.Email.ToLower().Contains("@owner") == false && q.Date.Date.Year == DateTime.Now.Year && q.User.listoftags.Any() &&
            q.User.birthdate != null && q.User.User.EmailConfirmed == true && (q.User.ProfileCompleted != null && q.User.ProfileCompleted == true)).ToList();

            ExportStatisticsByGenderAndAgeViewModel eventTrackersPerMonth = new ExportStatisticsByGenderAndAgeViewModel();

            var groupedEventTracker = eventTrackers.ToList().GroupBy(q => new { q.EventId, q.UserId }).Select(q => new EventTracker() { Id = q.FirstOrDefault().Id, UserId = q.FirstOrDefault().UserId, EventId = q.FirstOrDefault().EventId, Date = q.FirstOrDefault().Date, ActionType = q.FirstOrDefault().ActionType }).ToList();

            var eventTrackerbyMonth = groupedEventTracker.GroupBy(x => new { Month = x.Date.Month, GenderType = eventTrackers.ToList().FirstOrDefault(q => q.UserId == x.UserId).User.Gender }).Select(x => new { Month = x.Key.Month, Count = x.Count(), Gender = x.Key.GenderType, Birthdate = eventTrackers.FirstOrDefault(q => q.UserId == x.FirstOrDefault().UserId).User.birthdate }).OrderBy(x => x.Month).ToList();

            var From18To24 = groupedEventTracker.Where(q => GetAge(eventTrackers.FirstOrDefault(u => u.UserId == q.UserId).User.birthdate.Value) >= 18 && GetAge(eventTrackers.FirstOrDefault(u => u.UserId == q.UserId).User.birthdate.Value) <= 25).GroupBy(x => new { Month = x.Date.Month }).Select(x => new { Month = x.Key.Month, Count = x.Count(), Birthdate = eventTrackers.FirstOrDefault(u => u.UserId == x.FirstOrDefault().UserId).User.birthdate.Value }).OrderBy(x => x.Month).ToList();
            var From25To34 = groupedEventTracker.Where(q => GetAge(eventTrackers.FirstOrDefault(u => u.UserId == q.UserId).User.birthdate.Value) >= 26 && GetAge(eventTrackers.FirstOrDefault(u => u.UserId == q.UserId).User.birthdate.Value) <= 34).GroupBy(x => new { Month = x.Date.Month }).Select(x => new { Month = x.Key.Month, Count = x.Count(), Birthdate = eventTrackers.FirstOrDefault(u => u.UserId == x.FirstOrDefault().UserId).User.birthdate.Value }).OrderBy(x => x.Month).ToList();
            var From35To44 = groupedEventTracker.Where(q => GetAge(eventTrackers.FirstOrDefault(u => u.UserId == q.UserId).User.birthdate.Value) >= 35 && GetAge(eventTrackers.FirstOrDefault(u => u.UserId == q.UserId).User.birthdate.Value) <= 44).GroupBy(x => new { Month = x.Date.Month }).Select(x => new { Month = x.Key.Month, Count = x.Count(), Birthdate = eventTrackers.FirstOrDefault(u => u.UserId == x.FirstOrDefault().UserId).User.birthdate.Value }).OrderBy(x => x.Month).ToList();
            var From45To54 = groupedEventTracker.Where(q => GetAge(eventTrackers.FirstOrDefault(u => u.UserId == q.UserId).User.birthdate.Value) >= 45 && GetAge(eventTrackers.FirstOrDefault(u => u.UserId == q.UserId).User.birthdate.Value) <= 54).GroupBy(x => new { Month = x.Date.Month }).Select(x => new { Month = x.Key.Month, Count = x.Count(), Birthdate = eventTrackers.FirstOrDefault(u => u.UserId == x.FirstOrDefault().UserId).User.birthdate.Value }).OrderBy(x => x.Month).ToList();
            var From55To64 = groupedEventTracker.Where(q => GetAge(eventTrackers.FirstOrDefault(u => u.UserId == q.UserId).User.birthdate.Value) >= 55 && GetAge(eventTrackers.FirstOrDefault(u => u.UserId == q.UserId).User.birthdate.Value) <= 64).GroupBy(x => new { Month = x.Date.Month }).Select(x => new { Month = x.Key.Month, Count = x.Count(), Birthdate = eventTrackers.FirstOrDefault(u => u.UserId == x.FirstOrDefault().UserId).User.birthdate.Value }).OrderBy(x => x.Month).ToList();
            var MoreThan65 = groupedEventTracker.Where(q => GetAge(eventTrackers.FirstOrDefault(u => u.UserId == q.UserId).User.birthdate.Value) >= 65).GroupBy(x => new { Month = x.Date.Month }).Select(x => new { Month = x.Key.Month, Count = x.Count(), Birthdate = eventTrackers.FirstOrDefault(u => u.UserId == x.FirstOrDefault().UserId).User.birthdate.Value }).OrderBy(x => x.Month).ToList();

            eventTrackersPerMonth.StatisticsByGender = monthes.Select(m => new StatisticsByGenderViewModel()
            {
                Month = m.Month,
                Male = eventTrackerbyMonth.Where(n => m.index == n.Month).FirstOrDefault(q => q.Gender == "male")?.Count ?? 0,
                Female = eventTrackerbyMonth.Where(n => m.index == n.Month).FirstOrDefault(q => q.Gender == "female")?.Count ?? 0,
                Other = eventTrackerbyMonth.Where(n => m.index == n.Month).FirstOrDefault(q => q.Gender == "other")?.Count ?? 0,
            }).ToList();

            eventTrackersPerMonth.StatisticsByAge = monthes.Select(m => new StatisticsByAgeViewModel()
            {
                Month = m.Month,
                From18To24 = From18To24.Where(n => m.index == n.Month).FirstOrDefault()?.Count ?? 0,
                From25To34 = From25To34.Where(n => m.index == n.Month).FirstOrDefault()?.Count ?? 0,
                From35To44 = From35To44.Where(n => m.index == n.Month).FirstOrDefault()?.Count ?? 0,
                From45To54 = From45To54.Where(n => m.index == n.Month).FirstOrDefault()?.Count ?? 0,
                From55To64 = From55To64.Where(n => m.index == n.Month).FirstOrDefault()?.Count ?? 0,
                MoreThan65 = MoreThan65.Where(n => m.index == n.Month).FirstOrDefault()?.Count ?? 0
            }).ToList();

            return eventTrackersPerMonth;
        }

        private  async Task<StatisticsByGenderAndAgeViewModel> BlockedUsersQuery(LoggedinUser loggedinUser)
        {
            List<Requestes> requestes = await _authDBContext.Requestes.Include(q => q.User).ThenInclude(q => q.User).Where(q => q.User.Email.ToLower().Contains("@owner") == false
            && q.User.birthdate != null && q.User.Email != "dev@dev.com" && q.User.User.EmailConfirmed == true 
            && (q.User.ProfileCompleted != null && q.User.ProfileCompleted == true) && q.User.listoftags.Any()).AsNoTracking().ToListAsync();
            List<Requestes> blockedUsers = requestes.Where(q => q.UserblockId == loggedinUser.User.UserDetails.PrimaryId &&
            //!q.User.Email.ToLower().Contains("@owner") && q.User.birthdate != null && q.User != null && q.User.User.EmailConfirmed == true && (q.User.ProfileCompleted != null && q.User.ProfileCompleted == true)
             q.status == 2).ToList();

            StatisticsByGenderAndAgeViewModel blockedUsersStatictes = new StatisticsByGenderAndAgeViewModel()
            {
                All = blockedUsers.Count(),
                Male = blockedUsers.Where(q => q.User.Gender == "male").Count(),
                Female = blockedUsers.Where(q => q.User.Gender == "female").Count(),
                Other = blockedUsers.Where(q => q.User.Gender == "other").Count(),
                From18To25 = blockedUsers.Where(q => GetAge(q.User.birthdate.Value) >= 18 && GetAge(q.User.birthdate.Value) <= 25).Count(),
                From25To34 = blockedUsers.Where(q => GetAge(q.User.birthdate.Value) >= 26 && GetAge(q.User.birthdate.Value) <= 34).Count(),
                From35To44 = blockedUsers.Where(q => GetAge(q.User.birthdate.Value) >= 35 && GetAge(q.User.birthdate.Value) <= 44).Count(),
                From45To54 = blockedUsers.Where(q => GetAge(q.User.birthdate.Value) >= 45 && GetAge(q.User.birthdate.Value) <= 54).Count(),
                From55To64 = blockedUsers.Where(q => GetAge(q.User.birthdate.Value) >= 55 && GetAge(q.User.birthdate.Value) <= 64).Count(),
                From65AndMore = blockedUsers.Where(q => GetAge(q.User.birthdate.Value) >= 65).Count(),
            };

            return blockedUsersStatictes;
        }

        private dynamic TotalReportsQuery(LoggedinUser loggedinUser)
        {
            var allEvents = _authDBContext.EventData.Where(n => n.IsActive == true && n.UserId == loggedinUser.User.UserDetails.PrimaryId && (n.EventTypeListid == 5
                            || n.EventTypeListid == 6)).ToList();
            var eventsIds = allEvents.Select(e => e.Id).ToList();
            var totalReports = _authDBContext.EventReports.Where(r=>eventsIds.Contains(r.EventDataID)).ToList().Count();

            return new { totalEvents = allEvents.Count, totalReports = totalReports };
        }

        private IEnumerable<dynamic> TotalReportsPerEventQuery(LoggedinUser loggedinUser)
        {
            var allEvents = _authDBContext.EventData.Where(n => n.IsActive == true && n.UserId == loggedinUser.User.UserDetails.PrimaryId && (n.EventTypeListid == 5
                            || n.EventTypeListid == 6)).ToList();            
            foreach (var item in allEvents)
            {
                var reportscount= _authDBContext.EventReports.Where(r=>r.EventDataID== item.Id).Count();
                yield return new
                {
                    EventName = item.Title,
                    TotalReports = reportscount
                };
            }
            
        }

        private void GetTotalEventInExcel(XLWorkbook workbook, IEnumerable<dynamic> eventsInfos)
        {
            var eventsInfo = eventsInfos.Select(i => i).FirstOrDefault();
            IXLWorksheet totalEventStatisticsWorksheet = workbook.Worksheets.Add("Total Events");
            totalEventStatisticsWorksheet.Range(totalEventStatisticsWorksheet.Cell(1, 2), totalEventStatisticsWorksheet.Cell(1, 16)).Merge().Value = "Total Events";
            totalEventStatisticsWorksheet.Range(totalEventStatisticsWorksheet.Cell(1, 2), totalEventStatisticsWorksheet.Cell(1, 16)).Style.Fill.SetBackgroundColor(XLColor.Gray);
            totalEventStatisticsWorksheet.Range(totalEventStatisticsWorksheet.Cell(1, 2), totalEventStatisticsWorksheet.Cell(1, 16)).Style.Font.SetBold().Font.FontSize = 14;
            totalEventStatisticsWorksheet.Cell(2, 2).Value = "Total number of events created";
            totalEventStatisticsWorksheet.Cell(2, 3).Value = "Number of existing events";
            totalEventStatisticsWorksheet.Cell(2, 4).Value = "Number Of Finished Events";
            totalEventStatisticsWorksheet.Cell(2, 5).Value = "Number Of Private Events";
            totalEventStatisticsWorksheet.Cell(2, 6).Value = "Number Of Public Events";
            totalEventStatisticsWorksheet.Cell(2, 7).Value = "Number Of Deleted Events";
            totalEventStatisticsWorksheet.Cell(2, 8).Value = "Percentage of events participants";
            totalEventStatisticsWorksheet.Cell(2, 9).Value = "Percentage of Existe events participants";
            //totalEventStatisticsWorksheet.Cell(2, 10).Value = "Created Events After 3 Months";
            //totalEventStatisticsWorksheet.Cell(2, 11).Value = "Low Capacity Events";
            //totalEventStatisticsWorksheet.Cell(2, 12).Value = "Medium Capacity Events";
            //totalEventStatisticsWorksheet.Cell(2, 13).Value = "High Capacity Events";
            totalEventStatisticsWorksheet.Cell(2, 13).Value = "Total Events Attendance";


            totalEventStatisticsWorksheet.Cell(3, 2).Value = eventsInfo.AllEventsCount;
            totalEventStatisticsWorksheet.Cell(3, 3).Value = eventsInfo.NumberOfExisteEvent;
            totalEventStatisticsWorksheet.Cell(3, 4).Value = eventsInfo.NumberOfFinishedEvent;
            totalEventStatisticsWorksheet.Cell(3, 5).Value = eventsInfo.NumberOfPrivateEvent;
            totalEventStatisticsWorksheet.Cell(3, 6).Value = eventsInfo.NumberOfPublicEvent;
            totalEventStatisticsWorksheet.Cell(3, 7).Value = eventsInfo.NumberOfDeletedEvents;
            totalEventStatisticsWorksheet.Cell(3, 8).Value = eventsInfo.AverageOfParticibated;
            totalEventStatisticsWorksheet.Cell(3, 9).Value = eventsInfo.averageOfParticibatedInExisteEvent;
           // totalEventStatisticsWorksheet.Cell(3, 10).Value = eventsInfo.Eventsafter3months;
            //totalEventStatisticsWorksheet.Cell(3, 11).Value = eventsInfo.LOWcapacity;
            //totalEventStatisticsWorksheet.Cell(3, 12).Value = eventsInfo.Mediumcapacity;
            //totalEventStatisticsWorksheet.Cell(3, 13).Value = eventsInfo.Highcapacity;
            totalEventStatisticsWorksheet.Cell(3, 13).Value = eventsInfo.Attendes;
        }
        private void GetEventStatisticsInExcel(XLWorkbook workbook, IEnumerable<dynamic> eventStatistics)
        {
            IXLWorksheet eventWorksheet = workbook.Worksheets.Add("Event Statistics");

            int eventStatisticsCurrentRow = 2;
            eventWorksheet.Range(eventWorksheet.Cell(1, 2), eventWorksheet.Cell(1, 8)).Merge().Value = "Event Statistics";
            eventWorksheet.Range(eventWorksheet.Cell(1, 2), eventWorksheet.Cell(1, 8)).Style.Fill.SetBackgroundColor(XLColor.Gray);
            eventWorksheet.Range(eventWorksheet.Cell(1, 2), eventWorksheet.Cell(1, 8)).Style.Font.SetBold().Font.FontSize = 14;
            eventWorksheet.Cell(eventStatisticsCurrentRow, 2).Value = "Month";
            eventWorksheet.Cell(eventStatisticsCurrentRow, 3).Value = "All Events";
            eventWorksheet.Cell(eventStatisticsCurrentRow, 4).Value = "Private Events";
            //eventWorksheet.Cell(eventStatisticsCurrentRow, 5).Value = "Friendzr Events";
            //eventWorksheet.Cell(eventStatisticsCurrentRow, 6).Value = "External Events";
            //eventWorksheet.Cell(eventStatisticsCurrentRow, 7).Value = "Admin External Events";
            eventWorksheet.Cell(eventStatisticsCurrentRow, 5).Value = "Public Events";

            foreach (var monthStatistic in eventStatistics)
            {
                eventStatisticsCurrentRow++;
                eventWorksheet.Cell(eventStatisticsCurrentRow, 2).Value = monthStatistic.Month;
                eventWorksheet.Cell(eventStatisticsCurrentRow, 3).Value = monthStatistic.AllEventPerMonth;
                eventWorksheet.Cell(eventStatisticsCurrentRow, 4).Value = monthStatistic.WhiteLableEventPerMonth;
                //eventWorksheet.Cell(eventStatisticsCurrentRow, 5).Value = monthStatistic.FriendzrEventPerMonth;
                //eventWorksheet.Cell(eventStatisticsCurrentRow, 6).Value = monthStatistic.ExternalEventPerMonth;
                //eventWorksheet.Cell(eventStatisticsCurrentRow, 7).Value = monthStatistic.AdminExternalEventPerMonth;
                eventWorksheet.Cell(eventStatisticsCurrentRow, 5).Value = monthStatistic.PublicEventPerMonth;
            }
        }
        private void GetEventInEachCategoryInExcel(XLWorkbook workbook, IEnumerable<dynamic> categories)
        {
            IXLWorksheet eventsInEachCategoryWorksheet = workbook.Worksheets.Add("Events In Each Category");

            int eventsInEachCategoryCurrentColumn = 2;
            eventsInEachCategoryWorksheet.Range(eventsInEachCategoryWorksheet.Cell(1, 2), eventsInEachCategoryWorksheet.Cell(1, categories.Count() + 1)).Merge().Value = "Events In Each Category";
            eventsInEachCategoryWorksheet.Range(eventsInEachCategoryWorksheet.Cell(1, 2), eventsInEachCategoryWorksheet.Cell(1, categories.Count() + 1)).Style.Fill.SetBackgroundColor(XLColor.Gray);
            eventsInEachCategoryWorksheet.Range(eventsInEachCategoryWorksheet.Cell(1, 2), eventsInEachCategoryWorksheet.Cell(1, categories.Count() + 1)).Style.Font.SetBold().Font.FontSize = 14;
            foreach (var category in categories)
            {
                eventsInEachCategoryWorksheet.Cell(2, eventsInEachCategoryCurrentColumn).Value = category.CategoryName;
                eventsInEachCategoryCurrentColumn++;
            }

            eventsInEachCategoryCurrentColumn = 2;
            foreach (var category in categories)
            {
                eventsInEachCategoryWorksheet.Cell(3, eventsInEachCategoryCurrentColumn).Value = category.NumOfCreatedEvent;
                eventsInEachCategoryCurrentColumn++;
            }

        }
        private void GetEventAttendInExcel(XLWorkbook workbook, ExportStatisticsByGenderAndAgeViewModel data)
        {
            IXLWorksheet newWorksheet = workbook.Worksheets.Add("Event Attend");            

            int currentRowByAge = 2;
            newWorksheet.Range(newWorksheet.Cell(1, 2), newWorksheet.Cell(1, 8)).Merge().Value = "Event Attend By Age";
            newWorksheet.Range(newWorksheet.Cell(1, 2), newWorksheet.Cell(1, 8)).Style.Fill.SetBackgroundColor(XLColor.Gray);
            newWorksheet.Range(newWorksheet.Cell(1, 2), newWorksheet.Cell(1, 8)).Style.Font.SetBold().Font.FontSize = 14;
            newWorksheet.Cell(currentRowByAge, 2).Value = "Month";
            newWorksheet.Cell(currentRowByAge, 3).Value = "From 18 To 24";
            newWorksheet.Cell(currentRowByAge, 4).Value = "From 25 To 34";
            newWorksheet.Cell(currentRowByAge, 5).Value = "From 35 To 44";
            newWorksheet.Cell(currentRowByAge, 6).Value = "From 45 To 54";
            newWorksheet.Cell(currentRowByAge, 7).Value = "From 55 To 64";
            newWorksheet.Cell(currentRowByAge, 8).Value = "More Than 65";

            foreach (StatisticsByAgeViewModel monthStatistic in data.StatisticsByAge)
            {
                currentRowByAge++;
                newWorksheet.Cell(currentRowByAge, 2).Value = monthStatistic.Month;
                newWorksheet.Cell(currentRowByAge, 3).Value = monthStatistic.From18To24;
                newWorksheet.Cell(currentRowByAge, 4).Value = monthStatistic.From25To34;
                newWorksheet.Cell(currentRowByAge, 5).Value = monthStatistic.From35To44;
                newWorksheet.Cell(currentRowByAge, 6).Value = monthStatistic.From45To54;
                newWorksheet.Cell(currentRowByAge, 7).Value = monthStatistic.From55To64;
                newWorksheet.Cell(currentRowByAge, 8).Value = monthStatistic.MoreThan65;
            }

            int currentRowByGender = 2;
            newWorksheet.Range(newWorksheet.Cell(1, 11), newWorksheet.Cell(1, 14)).Merge().Value = "Event Attend By Gender";
            newWorksheet.Range(newWorksheet.Cell(1, 11), newWorksheet.Cell(1, 14)).Style.Fill.SetBackgroundColor(XLColor.Gray);
            newWorksheet.Range(newWorksheet.Cell(1, 11), newWorksheet.Cell(1, 14)).Style.Font.SetBold().Font.FontSize = 14;
            newWorksheet.Cell(currentRowByGender, 11).Value = "Month";
            newWorksheet.Cell(currentRowByGender, 12).Value = "Male";
            newWorksheet.Cell(currentRowByGender, 13).Value = "Female";
            newWorksheet.Cell(currentRowByGender, 14).Value = "Other";


            foreach (StatisticsByGenderViewModel monthStatistic in data.StatisticsByGender)
            {
                currentRowByGender++;
                newWorksheet.Cell(currentRowByGender, 11).Value = monthStatistic.Month;
                newWorksheet.Cell(currentRowByGender, 12).Value = monthStatistic.Male;
                newWorksheet.Cell(currentRowByGender, 13).Value = monthStatistic.Female;
                newWorksheet.Cell(currentRowByGender, 14).Value = monthStatistic.Other;

            }

        }
        private void GetBlockedUsersInExcel(XLWorkbook workbook, StatisticsByGenderAndAgeViewModel data)
        {
            IXLWorksheet newWorksheet = workbook.Worksheets.Add("Blocked Users");

            int currentRowByAge = 2;
            newWorksheet.Range(newWorksheet.Cell(1, 2), newWorksheet.Cell(1, 7)).Merge().Value = "Blocked Users By Age";
            newWorksheet.Range(newWorksheet.Cell(1, 2), newWorksheet.Cell(1, 7)).Style.Fill.SetBackgroundColor(XLColor.Gray);
            newWorksheet.Range(newWorksheet.Cell(1, 2), newWorksheet.Cell(1, 7)).Style.Font.SetBold().Font.FontSize = 14;
            newWorksheet.Cell(currentRowByAge, 2).Value = "From 18 To 24";
            newWorksheet.Cell(currentRowByAge, 3).Value = "From 25 To 34";
            newWorksheet.Cell(currentRowByAge, 4).Value = "From 35 To 44";
            newWorksheet.Cell(currentRowByAge, 5).Value = "From 45 To 54";
            newWorksheet.Cell(currentRowByAge, 6).Value = "From 55 To 64";
            newWorksheet.Cell(currentRowByAge, 7).Value = "More Than 65";


            newWorksheet.Cell(3, 2).Value = data.From18To25;
            newWorksheet.Cell(3, 3).Value = data.From25To34;
            newWorksheet.Cell(3, 4).Value = data.From35To44;
            newWorksheet.Cell(3, 5).Value = data.From45To54;
            newWorksheet.Cell(3, 6).Value = data.From55To64;
            newWorksheet.Cell(3, 7).Value = data.From65AndMore;


            int currentRowByGender = 2;
            newWorksheet.Range(newWorksheet.Cell(1, 10), newWorksheet.Cell(1, 13)).Merge().Value = "Blocked Users By Gender";
            newWorksheet.Range(newWorksheet.Cell(1, 10), newWorksheet.Cell(1, 13)).Style.Fill.SetBackgroundColor(XLColor.Gray);
            newWorksheet.Range(newWorksheet.Cell(1, 10), newWorksheet.Cell(1, 13)).Style.Font.SetBold().Font.FontSize = 14;
            newWorksheet.Cell(currentRowByGender, 10).Value = "Male";
            newWorksheet.Cell(currentRowByGender, 11).Value = "Female";
            newWorksheet.Cell(currentRowByGender, 12).Value = "Other";
            newWorksheet.Cell(currentRowByGender, 13).Value = "All";



            newWorksheet.Cell(3, 10).Value = data.Male;
            newWorksheet.Cell(3, 11).Value = data.Female;
            newWorksheet.Cell(3, 12).Value = data.Other;
            newWorksheet.Cell(3, 13).Value = data.All;
        }

        private void GetTotalReportsInExcel(XLWorkbook workbook, dynamic eventsInfo)
        {
            
            IXLWorksheet totalEventStatisticsWorksheet = workbook.Worksheets.Add("Total Reports");
            totalEventStatisticsWorksheet.Range(totalEventStatisticsWorksheet.Cell(1, 2), totalEventStatisticsWorksheet.Cell(1, 16)).Merge().Value = "Total Reports";
            totalEventStatisticsWorksheet.Range(totalEventStatisticsWorksheet.Cell(1, 2), totalEventStatisticsWorksheet.Cell(1, 16)).Style.Fill.SetBackgroundColor(XLColor.Gray);
            totalEventStatisticsWorksheet.Range(totalEventStatisticsWorksheet.Cell(1, 2), totalEventStatisticsWorksheet.Cell(1, 16)).Style.Font.SetBold().Font.FontSize = 14;
            totalEventStatisticsWorksheet.Cell(2, 2).Value = "Total number of events";
            totalEventStatisticsWorksheet.Cell(2, 3).Value = "Total number of Reports";

            totalEventStatisticsWorksheet.Cell(3, 2).Value = eventsInfo.totalEvents;
            totalEventStatisticsWorksheet.Cell(3, 3).Value = eventsInfo.totalReports;          
        }

        private void GetTotalReportsPerEventInExcel(XLWorkbook workbook, IEnumerable<dynamic> data)
        {
            IXLWorksheet usersInEachinterestWorksheet = workbook.Worksheets.Add("Total Reports Per Event");

            int usersInEachinterestCurrentRow = 3;
            usersInEachinterestWorksheet.Range(usersInEachinterestWorksheet.Cell(1, 2), usersInEachinterestWorksheet.Cell(1, 3)).Merge().Value = "Total Reports Per Event";
            usersInEachinterestWorksheet.Range(usersInEachinterestWorksheet.Cell(1, 2), usersInEachinterestWorksheet.Cell(1, 3)).Style.Fill.SetBackgroundColor(XLColor.Gray);
            usersInEachinterestWorksheet.Range(usersInEachinterestWorksheet.Cell(1, 2), usersInEachinterestWorksheet.Cell(1, 3)).Style.Font.SetBold().Font.FontSize = 14;
            usersInEachinterestWorksheet.Cell(2, 2).Value = "Event Name";
            usersInEachinterestWorksheet.Cell(2, 3).Value = "Number Of Reports";

            foreach (var interest in data)
            {
                usersInEachinterestWorksheet.Cell(usersInEachinterestCurrentRow, 2).Value = interest.EventName;
                usersInEachinterestCurrentRow++;
            }

            usersInEachinterestCurrentRow = 3;
            foreach (var interest in data)
            {
                usersInEachinterestWorksheet.Cell(usersInEachinterestCurrentRow, 3).Value = interest.TotalReports;
                usersInEachinterestCurrentRow++;
            }
        }
        private int GetAge(DateTime dob)
        {
            int age = 0;
            age = DateTime.Now.Subtract(dob).Days;
            age = age / 365;
            return age;
        }

    }
}
