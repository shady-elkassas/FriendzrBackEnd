using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Social.Entity.DBContext;
using Social.Entity.Enums;
using Social.Entity.Models;
using Social.Services.Attributes;
using Social.Services.ModelView;
using Social.Services.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace Social.Areas.Admin.Controllers
{
    [Area("Admin")]
    [ServiceFilter(typeof(AuthorizeUser))]
    public class HomeController : Controller
    {
        private readonly AuthDBContext authDBContext;
        private readonly IUserReportService userReportService;
        private readonly IEventReportService eventReportService;
        private readonly IAppConfigrationService appConfigrationService;
        private readonly IDistanceFilterHistoryService distanceFilterHistoryService;
        private readonly IFilteringAccordingToAgeHistoryService filteringAccordingToAgeHistoryService;
        public HomeController(AuthDBContext authDBContext, IUserReportService userReportService, IDistanceFilterHistoryService distanceFilterHistoryService, IFilteringAccordingToAgeHistoryService filteringAccordingToAgeHistoryService, IEventReportService eventReportService, IAppConfigrationService appConfigrationService)
        {
            this.authDBContext = authDBContext;
            this.userReportService = userReportService;
            this.eventReportService = eventReportService;
            this.appConfigrationService = appConfigrationService;
            this.distanceFilterHistoryService = distanceFilterHistoryService;
            this.filteringAccordingToAgeHistoryService = filteringAccordingToAgeHistoryService;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var DateTimeNow = DateTime.UtcNow.Date;
            var DbF = Microsoft.EntityFrameworkCore.EF.Functions;
            var allrequestes = await authDBContext.Requestes.Include(q => q.User).ThenInclude(q => q.User).Where(q => q.User.Email.ToLower().Contains("@owner") == false && q.User.birthdate != null && q.User.listoftags.Any() && q.User.User.EmailConfirmed == true && (q.User.ProfileCompleted != null && q.User.ProfileCompleted == true)).ToListAsync();
            List<LoggedinUser> loggedinUser = await authDBContext.LoggedinUser.Include(q => q.User).ThenInclude(q => q.UserDetails).Where(q => q.User.Email.ToLower().Contains("@owner") == false && q.User.UserDetails.birthdate != null && q.User.Email != "dev@dev.com" && q.User.EmailConfirmed == true && (q.User.UserDetails.ProfileCompleted != null && q.User.UserDetails.ProfileCompleted == true) && q.User.UserDetails.birthdate != null && q.User.UserDetails.listoftags.Any()).ToListAsync();
            var UserDetails = authDBContext.UserDetails.Where(q => q.birthdate != null && q.Email.ToLower().Contains("@owner") == false && q.Email != "dev@dev.com" && q.listoftags.Any() && q.User.EmailConfirmed == true && (q.ProfileCompleted != null && q.ProfileCompleted == true));
            var UseresWithBirthDate = UserDetails.Select(x => DbF.DateDiffYear(x.birthdate, DateTimeNow));
            var averageAge = Math.Round(Convert.ToDouble(UseresWithBirthDate.Sum(x => x ?? 0)) / UseresWithBirthDate.Count(), 2);
            var users = await authDBContext.Users.Include(q => q.UserDetails).Where(q => q.Email.ToLower().Contains("@owner") == false && q.Email != "dev@dev.com" && q.UserDetails != null).ToListAsync();
            var usersEnableGhostMode = UserDetails.Where(x => x.ghostmode == true);
            var UsersBirthDate = UserDetails.Where(x => x.birthdate != null).Select(x => x.birthdate);
            var TotalUserUsedAgeFiltring = authDBContext.FilteringAccordingToAgeHistory.Select(x => x.UserID).Distinct().Count();

            List<DeletedUser> deletedUsers = await authDBContext.DeletedUsers.ToListAsync();

            int deletedUsersCount = deletedUsers.Where(q => !users.Select(q => q.Email).Contains(q.Email)).GroupBy(q => q.Email).Count();

            var MaxAgeFiltringRangeUsed = authDBContext.FilteringAccordingToAgeHistory.Select(x => new { x.AgeFrom, x.AgeTo, x.UserID }).ToList().GroupBy(x => new { x.AgeFrom, x.AgeTo }).Select(x => new { Count = x.Count(), Range = x.Key.AgeFrom + " - " + x.Key.AgeTo }).OrderByDescending(x => x.Count).FirstOrDefault();

            UserStatistics UserStatistics = new UserStatistics()
            {
                CurrenUsers_Count = users.Count(),
                TotalUserUsedAgeFiltring = TotalUserUsedAgeFiltring,
                MostAgeFiltirngRangeUsed = MaxAgeFiltringRangeUsed?.Range ?? "0 - 0",
                MostAgeFiltirngRangeUsed_Rate = Math.Round(MaxAgeFiltringRangeUsed?.Count ?? 0 / ((double)TotalUserUsedAgeFiltring), 2),
                deactivatespushnotifications_Count = users.Where(x => x.UserDetails.pushnotification == false).Count(),
                UseresAverageAge = averageAge,
                TotalInAppearanceTypes = authDBContext.AppearanceTypes_UserDetails.Count(),
                AppearenceEveryOneInGhostMode_Count = authDBContext.AppearanceTypes_UserDetails.Where(x => x.AppearanceTypeID == 1).Count(),
                AppearenceMaleInGhostMode_Count = authDBContext.AppearanceTypes_UserDetails.Where(x => x.AppearanceTypeID == 2).Count(),
                AppearenceFemaleInGhostMode_Count = authDBContext.AppearanceTypes_UserDetails.Where(x => x.AppearanceTypeID == 3).Count(),
                AppearenceOtherGenderInGhostMode_Count = authDBContext.AppearanceTypes_UserDetails.Where(x => x.AppearanceTypeID == 4).Count(),
                RequestesCount = allrequestes.Count(),
                UserWithLessThan18Age_Count = UsersBirthDate.Count(x => DbF.DateDiffYear(x, DateTimeNow) < 18),
                UsersWith18_24Age_Count = UsersBirthDate.Count(x => DbF.DateDiffYear(x, DateTimeNow) >= 18 && DbF.DateDiffYear(x, DateTimeNow) <= 24),
                UsersWith25_34Age_Count = UsersBirthDate.Count(x => DbF.DateDiffYear(x, DateTimeNow) >= 25 && DbF.DateDiffYear(x, DateTimeNow) <= 34),
                UsersWith35_54Age_Count = UsersBirthDate.Count(x => DbF.DateDiffYear(x, DateTimeNow) >= 35 && DbF.DateDiffYear(x, DateTimeNow) <= 54),
                UsersWithMoreThan55Age_Count = UsersBirthDate.Count(x => DbF.DateDiffYear(x, DateTimeNow) >= 55),
                BlockRequestesCount = allrequestes.Where(x => x.status == 3).Count(),
                AcceptedRequestesCount = allrequestes.Where(x => x.status == 2).Count(),
                PendindingRequestesCount = allrequestes.Where(x => x.status == 1).Count(),
                UseresStillUseAppAfter3Months_Count = UserDetails.Where(x => DbF.DateDiffMonth(x.User.RegistrationDate, x.User.LoggedinUser.Max(q => q.ExpiredOn).Date) >= 3).Count(),
                ActiveUsers_Count = loggedinUser.Where(q => q.ExpiredOn > DateTime.Now).Count(),
                UseresEnableGhostMode_Count = usersEnableGhostMode.Count(),
                ConfirmedMailUsers_Count = users.Where(x => x.EmailConfirmed).Count(),
                DeletedUsers_Count = deletedUsersCount, //authDBContext.DeletedUsersLogs.Count(),
                UnConfirmedMailUsers_Count = users.Where(x => !x.EmailConfirmed).Count(),
                Male_Count = UserDetails.Where(x => x.Gender == "male").Count(),
                Female_Count = UserDetails.Where(x => x.Gender == "female").Count(),
                NeedUpdate = ((users.Count()) - (UserDetails.Where(n => n.birthdate != null && n.listoftags.Any()).Count())),
                Updated = UserDetails.Where(n => n.birthdate != null && n.listoftags.Any()).Count(),
                personalspace = UserDetails.Where(n => n.personalSpace == true).Count(),
                DeletedProfiles_Count = deletedUsersCount,
                PushNotificationsEnabled_Count = UserDetails.Where(n => n.pushnotification == true).Count(),
            };
            ViewBag.ListOFCities = authDBContext.Cities.Select(x => new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.DisplayName,
                Selected = x.GoogleName.ToLower() == "london"
            }).ToList();
            ViewBag.Cateories = authDBContext.category.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.name,
            }).ToList();
            return View(UserStatistics);
        }

        [Authorize]
        public async Task<IActionResult> AppStatistics()
        {
            var DateTimeNow = DateTime.UtcNow.Date;
            var DbF = Microsoft.EntityFrameworkCore.EF.Functions;
            var allrequestes = await authDBContext.Requestes.Include(q => q.User).ThenInclude(q => q.User).Where(q => q.User.Email.ToLower().Contains("@owner") == false && q.User.birthdate != null && q.User.listoftags.Any() && q.User.User.EmailConfirmed == true && (q.User.ProfileCompleted != null && q.User.ProfileCompleted == true)).ToListAsync();
            List<LoggedinUser> loggedinUser = await authDBContext.LoggedinUser.Include(q => q.User).ThenInclude(q => q.UserDetails).Where(q => q.User.Email.ToLower().Contains("@owner") == false && q.User.UserDetails.birthdate != null && q.User.Email != "dev@dev.com" && q.User.EmailConfirmed == true && (q.User.UserDetails.ProfileCompleted != null && q.User.UserDetails.ProfileCompleted == true) && q.User.UserDetails.birthdate != null && q.User.UserDetails.listoftags.Any()).ToListAsync();
            var UserDetails = authDBContext.UserDetails.Where(q => q.birthdate != null && q.Email.ToLower().Contains("@owner") == false && q.Email != "dev@dev.com" && q.listoftags.Any() && q.User.EmailConfirmed == true && (q.ProfileCompleted != null && q.ProfileCompleted == true));
            var UseresWithBirthDate = UserDetails.Select(x => DbF.DateDiffYear(x.birthdate, DateTimeNow));
            var averageAge = Math.Round(Convert.ToDouble(UseresWithBirthDate.Sum(x => x ?? 0)) / UseresWithBirthDate.Count(), 2);
            var users = await authDBContext.Users.Include(q => q.UserDetails).Where(q => q.Email.ToLower().Contains("@owner") == false && q.Email != "dev@dev.com" && q.UserDetails != null).ToListAsync();
            var usersEnableGhostMode = UserDetails.Where(x => x.ghostmode == true);
            var UsersBirthDate = UserDetails.Where(x => x.birthdate != null).Select(x => x.birthdate);
            var TotalUserUsedAgeFiltring = authDBContext.FilteringAccordingToAgeHistory.Select(x => x.UserID).Distinct().Count();

            List<DeletedUser> deletedUsers = await authDBContext.DeletedUsers.ToListAsync();

            int deletedUsersCount = deletedUsers.Where(q => !users.Select(q => q.Email).Contains(q.Email)).GroupBy(q => q.Email).Count();

            var MaxAgeFiltringRangeUsed = authDBContext.FilteringAccordingToAgeHistory.Select(x => new { x.AgeFrom, x.AgeTo, x.UserID }).ToList().GroupBy(x => new { x.AgeFrom, x.AgeTo }).Select(x => new { Count = x.Count(), Range = x.Key.AgeFrom + " - " + x.Key.AgeTo }).OrderByDescending(x => x.Count).FirstOrDefault();

            UserStatistics UserStatistics = new UserStatistics()
            {
                CurrenUsers_Count = users.Count(),
                TotalUserUsedAgeFiltring = TotalUserUsedAgeFiltring,
                MostAgeFiltirngRangeUsed = MaxAgeFiltringRangeUsed?.Range ?? "0 - 0",
                MostAgeFiltirngRangeUsed_Rate = Math.Round(MaxAgeFiltringRangeUsed?.Count ?? 0 / ((double)TotalUserUsedAgeFiltring), 2),
                deactivatespushnotifications_Count = users.Where(x => x.UserDetails.pushnotification == false).Count(),
                UseresAverageAge = averageAge,
                TotalInAppearanceTypes = authDBContext.AppearanceTypes_UserDetails.Count(),
                AppearenceEveryOneInGhostMode_Count = authDBContext.AppearanceTypes_UserDetails.Where(x => x.AppearanceTypeID == 1).Count(),
                AppearenceMaleInGhostMode_Count = authDBContext.AppearanceTypes_UserDetails.Where(x => x.AppearanceTypeID == 2).Count(),
                AppearenceFemaleInGhostMode_Count = authDBContext.AppearanceTypes_UserDetails.Where(x => x.AppearanceTypeID == 3).Count(),
                AppearenceOtherGenderInGhostMode_Count = authDBContext.AppearanceTypes_UserDetails.Where(x => x.AppearanceTypeID == 4).Count(),
                RequestesCount = allrequestes.Count(),
                UserWithLessThan18Age_Count = UsersBirthDate.Count(x => DbF.DateDiffYear(x, DateTimeNow) < 18),
                UsersWith18_24Age_Count = UsersBirthDate.Count(x => DbF.DateDiffYear(x, DateTimeNow) >= 18 && DbF.DateDiffYear(x, DateTimeNow) <= 24),
                UsersWith25_34Age_Count = UsersBirthDate.Count(x => DbF.DateDiffYear(x, DateTimeNow) >= 25 && DbF.DateDiffYear(x, DateTimeNow) <= 34),
                UsersWith35_54Age_Count = UsersBirthDate.Count(x => DbF.DateDiffYear(x, DateTimeNow) >= 35 && DbF.DateDiffYear(x, DateTimeNow) <= 54),
                UsersWithMoreThan55Age_Count = UsersBirthDate.Count(x => DbF.DateDiffYear(x, DateTimeNow) >= 55),
                BlockRequestesCount = allrequestes.Where(x => x.status == 3).Count(),
                AcceptedRequestesCount = allrequestes.Where(x => x.status == 2).Count(),
                PendindingRequestesCount = allrequestes.Where(x => x.status == 1).Count(),
                UseresStillUseAppAfter3Months_Count = UserDetails.Where(x => DbF.DateDiffMonth(x.User.RegistrationDate, x.User.LoggedinUser.Max(q => q.ExpiredOn).Date) >= 3).Count(),
                ActiveUsers_Count = loggedinUser.Where(q => q.ExpiredOn > DateTime.Now).Count(),
                UseresEnableGhostMode_Count = usersEnableGhostMode.Count(),
                ConfirmedMailUsers_Count = users.Where(x => x.EmailConfirmed).Count(),
                DeletedUsers_Count = deletedUsersCount, //authDBContext.DeletedUsersLogs.Count(),
                UnConfirmedMailUsers_Count = users.Where(x => !x.EmailConfirmed).Count(),
                Male_Count = UserDetails.Where(x => x.Gender == "male").Count(),
                Female_Count = UserDetails.Where(x => x.Gender == "female").Count(),
                NeedUpdate = ((users.Count()) - (UserDetails.Where(n => n.birthdate != null && n.listoftags.Any()).Count())),
                Updated = UserDetails.Where(n => n.birthdate != null && n.listoftags.Any()).Count(),
                personalspace = UserDetails.Where(n => n.personalSpace == true).Count(),
                DeletedProfiles_Count = deletedUsersCount,
                PushNotificationsEnabled_Count = UserDetails.Where(n => n.pushnotification == true).Count(),
            };
            return View(UserStatistics);
        }

        public async Task<IActionResult> GetInfoAboutUsers()
        {
            try
            {
                var DateTimeNow = DateTime.UtcNow.Date;
                var DbF = Microsoft.EntityFrameworkCore.EF.Functions;
                var allrequestes = await authDBContext.Requestes.Include(q => q.User).ThenInclude(q => q.User).Where(q => q.User.Email.ToLower().Contains("@owner") == false && q.User.birthdate != null && q.User.listoftags.Any() && q.User.User.EmailConfirmed == true && (q.User.ProfileCompleted != null && q.User.ProfileCompleted == true)).ToListAsync();
                List<LoggedinUser> loggedinUser = await authDBContext.LoggedinUser.Include(q => q.User).ThenInclude(q => q.UserDetails).Where(q => q.User.Email.ToLower().Contains("@owner") == false && q.User.UserDetails.birthdate != null && q.User.Email != "dev@dev.com" && q.User.EmailConfirmed == true && (q.User.UserDetails.ProfileCompleted != null && q.User.UserDetails.ProfileCompleted == true) && q.User.UserDetails.birthdate != null && q.User.UserDetails.listoftags.Any()).ToListAsync();
                var UserDetails = authDBContext.UserDetails.Where(q => q.birthdate != null && q.Email.ToLower().Contains("@owner") == false && q.Email != "dev@dev.com" && q.listoftags.Any() && q.User.EmailConfirmed == true && (q.ProfileCompleted != null && q.ProfileCompleted == true));
                var UseresWithBirthDate = UserDetails.Select(x => DbF.DateDiffYear(x.birthdate, DateTimeNow));
                var averageAge = Math.Round(Convert.ToDouble(UseresWithBirthDate.Sum(x => x ?? 0)) / UseresWithBirthDate.Count(), 2);
                var users = await authDBContext.Users.Include(q => q.UserDetails).Where(q => q.Email.ToLower().Contains("@owner") == false && q.Email != "dev@dev.com" && q.UserDetails != null).ToListAsync();
                var usersEnableGhostMode = UserDetails.Where(x => x.ghostmode == true);
                var UsersBirthDate = UserDetails.Where(x => x.birthdate != null).Select(x => x.birthdate);
                var TotalUserUsedAgeFiltring = authDBContext.FilteringAccordingToAgeHistory.Select(x => x.UserID).Distinct().Count();

                List<DeletedUser> deletedUsers = await authDBContext.DeletedUsers.ToListAsync();

                int deletedUsersCount = deletedUsers.Where(q => !users.Select(q => q.Email).Contains(q.Email)).GroupBy(q => q.Email).Count();

                var MaxAgeFiltringRangeUsed = authDBContext.FilteringAccordingToAgeHistory.Select(x => new { x.AgeFrom, x.AgeTo, x.UserID }).ToList().GroupBy(x => new { x.AgeFrom, x.AgeTo }).Select(x => new { Count = x.Count(), Range = x.Key.AgeFrom + " - " + x.Key.AgeTo }).OrderByDescending(x => x.Count).FirstOrDefault();

                UserStatistics UserStatistics = new UserStatistics()
                {
                    CurrenUsers_Count = users.Count(),
                    TotalUserUsedAgeFiltring = TotalUserUsedAgeFiltring,
                    MostAgeFiltirngRangeUsed = MaxAgeFiltringRangeUsed?.Range ?? "0 - 0",
                    MostAgeFiltirngRangeUsed_Rate = Math.Round(MaxAgeFiltringRangeUsed?.Count ?? 0 / ((double)TotalUserUsedAgeFiltring), 2),
                    deactivatespushnotifications_Count = users.Where(x => x.UserDetails.pushnotification == false).Count(),
                    UseresAverageAge = averageAge,
                    TotalInAppearanceTypes = authDBContext.AppearanceTypes_UserDetails.Count(),
                    AppearenceEveryOneInGhostMode_Count = authDBContext.AppearanceTypes_UserDetails.Where(x => x.AppearanceTypeID == 1).Count(),
                    AppearenceMaleInGhostMode_Count = authDBContext.AppearanceTypes_UserDetails.Where(x => x.AppearanceTypeID == 2).Count(),
                    AppearenceFemaleInGhostMode_Count = authDBContext.AppearanceTypes_UserDetails.Where(x => x.AppearanceTypeID == 3).Count(),
                    AppearenceOtherGenderInGhostMode_Count = authDBContext.AppearanceTypes_UserDetails.Where(x => x.AppearanceTypeID == 4).Count(),
                    RequestesCount = allrequestes.Count(),
                    UserWithLessThan18Age_Count = UsersBirthDate.Count(x => DbF.DateDiffYear(x, DateTimeNow) < 18),
                    UsersWith18_24Age_Count = UsersBirthDate.Count(x => DbF.DateDiffYear(x, DateTimeNow) >= 18 && DbF.DateDiffYear(x, DateTimeNow) <= 24),
                    UsersWith25_34Age_Count = UsersBirthDate.Count(x => DbF.DateDiffYear(x, DateTimeNow) >= 25 && DbF.DateDiffYear(x, DateTimeNow) <= 34),
                    UsersWith35_54Age_Count = UsersBirthDate.Count(x => DbF.DateDiffYear(x, DateTimeNow) >= 35 && DbF.DateDiffYear(x, DateTimeNow) <= 54),
                    UsersWithMoreThan55Age_Count = UsersBirthDate.Count(x => DbF.DateDiffYear(x, DateTimeNow) >= 55),
                    BlockRequestesCount = allrequestes.Where(x => x.status == 3).Count(),
                    AcceptedRequestesCount = allrequestes.Where(x => x.status == 2).Count(),
                    PendindingRequestesCount = allrequestes.Where(x => x.status == 1).Count(),
                    UseresStillUseAppAfter3Months_Count = UserDetails.Where(x => DbF.DateDiffMonth(x.User.RegistrationDate, x.User.LoggedinUser.Max(q => q.ExpiredOn).Date) >= 3).Count(),
                    ActiveUsers_Count = loggedinUser.Where(q => q.ExpiredOn > DateTime.Now).Count(),
                    UseresEnableGhostMode_Count = usersEnableGhostMode.Count(),
                    ConfirmedMailUsers_Count = users.Where(x => x.EmailConfirmed).Count(),
                    DeletedUsers_Count = deletedUsersCount, //authDBContext.DeletedUsersLogs.Count(),
                    UnConfirmedMailUsers_Count = users.Where(x => !x.EmailConfirmed).Count(),
                    Male_Count = UserDetails.Where(x => x.Gender == "male").Count(),
                    Female_Count = UserDetails.Where(x => x.Gender == "female").Count(),
                    NeedUpdate = ((users.Count()) - (UserDetails.Where(n => n.birthdate != null && n.listoftags.Any()).Count())),
                    Updated = UserDetails.Where(n => n.birthdate != null && n.listoftags.Any()).Count(),
                    personalspace = UserDetails.Where(n => n.personalSpace == true).Count(),
                    DeletedProfiles_Count = deletedUsersCount,
                    PushNotificationsEnabled_Count = UserDetails.Where(n => n.pushnotification == true).Count(),
                };
                return Ok(JObject.FromObject(new { UsersInfo = UserStatistics }, new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));

            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        public IActionResult GetInfoAboutEvents()
        {

            var DbF = Microsoft.EntityFrameworkCore.EF.Functions;
            var DateTimeNow = DateTime.UtcNow.Date;

            var allEvents = authDBContext.EventData.Where(n => n.IsActive == true).ToList();

            var allEventAttends = authDBContext.EventChatAttend.ToList();

            var sharedevent = authDBContext.EventTrackers.Where(m => m.ActionType == EventActionType.Share.ToString()).Select(m => m.EventId).Distinct();
            var averageOfParticibated = Math.Round(Convert.ToDouble(allEventAttends.GroupBy(x => x.EventDataid).Select(x => x.Count()).Sum()) / allEvents.Count(), 2);
            var averageOfParticibatedInExisteEvent = Math.Round(Convert.ToDouble(allEventAttends.Where(q => q.EventData.eventdateto < DateTime.Now).GroupBy(x => x.EventDataid).Select(x => x.Count()).Sum()) / allEvents.Count(), 2);
            try
            { var LAWcapacityK = allEventAttends.GroupBy(x => x.EventDataid).Count(); }
            catch (Exception EX)
            {
                var T = EX;
            }
            var COUNTLIST = allEventAttends.GroupBy(x => x.EventDataid).Select(x => new { COUNT = x.Count(), totalnumbert = x.FirstOrDefault().EventData.totalnumbert }).ToList();

            var LAWcapacity = COUNTLIST.Where(N => (N.COUNT * 100 / N.totalnumbert) <= 40).Count();
            var mediumcapacity = COUNTLIST.Where(N => (N.COUNT * 100 / N.totalnumbert) > 40 && (N.COUNT * 100 / N.totalnumbert) <= 75).Count();
            var highcapacity = COUNTLIST.Where(N => (N.COUNT * 100 / N.totalnumbert) > 75 && (N.COUNT * 100 / N.totalnumbert) <= 100).Count();

            var eventsinfo = new
            {
                allEventsCount = allEvents.Count(),
                NumberOfExisteEvent = allEvents.Where(q => q.eventdateto.Value.Date > DateTime.Now.Date || (q.eventdateto.Value.Date == DateTime.Now.Date && q.eventto > DateTime.Now.TimeOfDay) || (q.allday == true && q.eventdate.Value.Date == DateTime.Now.Date)).Count(),
                NumberOfFinishedEvent = allEvents.Where(q => q.eventdateto.Value.Date < DateTime.Now.Date || (q.eventdateto.Value.Date == DateTime.Now.Date && q.eventto <= DateTime.Now.TimeOfDay) || (q.allday == true && q.eventdate.Value.Date <= DateTime.Now.Date)).Count(),
                NumberOfPrivateEvent = allEvents.Where(q => q.EventTypeListid == 1).Count(),
                NumberOfFriendzrEvent = allEvents.Where(q => q.EventTypeListid == 2).Count(),
                NumberOfExternalEvent = allEvents.Where(q => q.EventTypeListid == 3).Count(),
                NumberOfadminExternalEvent = allEvents.Where(q => q.EventTypeListid == 4).Count(),
                NumberOfWhiteLableEvent = allEvents.Where(q => q.EventTypeListid == 5).Count(),
                averageOfParticibated = averageOfParticibated,
                averageOfParticibatedInExisteEvent = averageOfParticibatedInExisteEvent,
                eventsafter3months = allEvents.Where(x => DbF.DateDiffMonth(x.eventdate, DateTimeNow) >= 3).Count(),
                NumberOfDeletedEvents = authDBContext.DeletedEventLogs.Count(),
                NumberOfUseresHowCreatedEvents = allEvents.Where(q => !q.User.Email.ToLower().Contains("@owner") && q.User.birthdate != null && q.User.User.EmailConfirmed == true && (q.User.ProfileCompleted != null && q.User.ProfileCompleted == true)).GroupBy(x => x.UserId).Select(x => x.Key).Count(),
                LOWcapacity = LAWcapacity,
                mediumcapacity = mediumcapacity,
                highcapacity = highcapacity,
                sharedevent = sharedevent.Count(),
                attendes = allEventAttends.Where(v => v.EventData.EventTypeListid == 3 && v.ISAdmin != true).Count(),
            };
            return Ok(JObject.FromObject(new { EventsInfo = eventsinfo }, new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));

        }

        //Not Used !!!!!
        public IActionResult UserStatictesPerMonth(int? Year)
        {
            //var AdminUsers=authDBContext.Users.Where(z=>z.is)
            Year = Year ?? DateTime.Now.Year;
            var allUsers = authDBContext.Users.Where(q => q.UserDetails.birthdate != null && q.Email.ToLower().Contains("@owner") == false && q.Email != "dev@dev.com" && q.UserDetails.listoftags.Any() && q.EmailConfirmed == true && (q.UserDetails.ProfileCompleted != null && q.UserDetails.ProfileCompleted == true)).Select(x => new
            {
                RegstrationDate = x.RegistrationDate,
                UserName = x.UserName,
                Gender = x.UserDetails.Gender
            }).AsEnumerable().Where(x => x.RegstrationDate.Year == Year).GroupBy(x => x.RegstrationDate.Date.Month).Select(x => new { Month = x.Key, Count = x.Count() });
            var DeactiveUsers = authDBContext.DeletedUsersLogs.Select(x => new
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
            var allUsersInYear = authDBContext.Users.Where(x => x.Email.ToLower().Contains("@owner") == false && x.Email != "dev@dev.com").Where(x => DbF.DateDiffDay(firstDay, x.RegistrationDate) >= 0 && DbF.DateDiffDay(x.RegistrationDate, lastDay) >= 0);
            var allRequestesInYear = authDBContext.Requestes.Where(x => x.User.listoftags.Any() && x.User.User.EmailConfirmed == true && (x.User.ProfileCompleted != null && x.User.ProfileCompleted == true) && DbF.DateDiffDay(firstDay, x.regestdata) >= 0 && DbF.DateDiffDay(x.regestdata, lastDay) >= 0);
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
            //var GostModeUseresPerMonth = allUsersInYear.Where(x => x.UserDetails != null && x.UserDetails.birthdate != null && x.UserDetails.listoftags.Any() && x.EmailConfirmed == true && (x.UserDetails.ProfileCompleted != null && x.UserDetails.ProfileCompleted == true) && x.UserDetails.ghostmode == true).GroupBy(x => x.RegistrationDate.Date.Month).Select(x => new
            //{
            //    Month = x.Key,
            //    Count = x.Count()
            //}).OrderBy(x => x.Month).ToList();
            //var GostModeMenUseresPerMonth = allUsersInYear.Where(x => x.UserDetails != null && x.UserDetails.birthdate != null && x.UserDetails.listoftags.Any() && x.EmailConfirmed == true && (x.UserDetails.ProfileCompleted != null && x.UserDetails.ProfileCompleted == true) && x.UserDetails.ghostmode == true && (x.UserDetails.Gender != null && x.UserDetails.Gender == "male")).GroupBy(x => x.RegistrationDate.Date.Month).Select(x => new
            //{
            //    Month = x.Key,
            //    Count = x.Count()
            //}).OrderBy(x => x.Month).ToList();
            //var GostModeWomenUseresPerMonth = allUsersInYear.Where(x => x.UserDetails != null && x.UserDetails.birthdate != null && x.UserDetails.listoftags.Any() && x.EmailConfirmed == true && (x.UserDetails.ProfileCompleted != null && x.UserDetails.ProfileCompleted == true) && x.UserDetails.ghostmode == true && (x.UserDetails.Gender != null && x.UserDetails.Gender == "female")).GroupBy(x => x.RegistrationDate.Date.Month).Select(x => new
            //{
            //    Month = x.Key,
            //    Count = x.Count()
            //}).OrderBy(x => x.Month).ToList();
            //var GostModeOtherUseresPerMonth = allUsersInYear.Where(x => x.UserDetails != null && x.UserDetails.birthdate != null && x.UserDetails.listoftags.Any() && x.EmailConfirmed == true && (x.UserDetails.ProfileCompleted != null && x.UserDetails.ProfileCompleted == true) && x.UserDetails.ghostmode == true && (x.UserDetails.Gender != null && x.UserDetails.Gender == "other")).GroupBy(x => x.RegistrationDate.Date.Month).Select(x => new
            //{
            //    Month = x.Key,
            //    Count = x.Count()
            //}).OrderBy(x => x.Month).ToList();
            //var GostModeEveryoneUseresPerMonth = allUsersInYear.Where(x => x.UserDetails != null && x.UserDetails.birthdate != null && x.UserDetails.listoftags.Any() && x.EmailConfirmed == true && (x.UserDetails.ProfileCompleted != null && x.UserDetails.ProfileCompleted == true) && x.UserDetails.ghostmode == true).GroupBy(x => x.RegistrationDate.Date.Month).Select(x => new
            //{
            //    Month = x.Key,
            //    Count = x.Count()
            //}).OrderBy(x => x.Month).ToList();
            var disablePushNotficationPerMonth = allUsersInYear.Where(x => x.UserDetails != null && x.UserDetails.birthdate != null && x.UserDetails.listoftags.Any() && x.EmailConfirmed == true && (x.UserDetails.ProfileCompleted != null && x.UserDetails.ProfileCompleted == true) && x.UserDetails.pushnotification == false).GroupBy(x => x.RegistrationDate.Date.Month).Select(x => new
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

            var DeactiveUsers = authDBContext.DeletedUsers.Where(q => !allUsersInYear.Select(q => q.Email).Contains(q.Email)).ToList().GroupBy(q => q.Email).Where(x => DbF.DateDiffDay(firstDay, x.FirstOrDefault().Date) >= 0 && DbF.DateDiffDay(x.FirstOrDefault().Date, lastDay) >= 0).GroupBy(x => x.FirstOrDefault().Date.Date.Month).Select(x => new { Month = x.Key, Count = x.Count() }).OrderBy(x => x.Month);

            var Monthes = Enumerable.Range(1, 12).Select(i => new { indx = i, Month = DateTimeFormatInfo.CurrentInfo.GetAbbreviatedMonthName(i) });
            var Results = Monthes.Select(x => new
            {
                Months = x.Month,
                registered = ActiveUseresPerMonth.FirstOrDefault(xx => x.indx == xx.Month)?.Count ?? 0,
                DeactiveUsers = DeactiveUsers.FirstOrDefault(xx => x.indx == xx.Month)?.Count ?? 0,
                //NotActiveUseresPerMonth = NotActiveUseresPerMonth.FirstOrDefault(xx => x.indx == xx.Month)?.Count ?? 0,
                ConfirmedMailUseresPerMonth = ConfirmedMailUseresPerMonth.FirstOrDefault(xx => x.indx == xx.Month)?.Count ?? 0,
                NotConfirmedMailUseresPerMonth = NotConfirmedMailUseresPerMonth.FirstOrDefault(xx => x.indx == xx.Month)?.Count ?? 0,
                //GostModeUseresPerMonth = GostModeUseresPerMonth.FirstOrDefault(xx => x.indx == xx.Month)?.Count ?? 0,
                //GostModeMenUseresPerMonth = GostModeUseresPerMonth.FirstOrDefault(xx => x.indx == xx.Month)?.Count ?? 0,
                //GostModeWomenUseresPerMonth = GostModeUseresPerMonth.FirstOrDefault(xx => x.indx == xx.Month)?.Count ?? 0,
                //GostModeOtherUseresPerMonth = GostModeUseresPerMonth.FirstOrDefault(xx => x.indx == xx.Month)?.Count ?? 0,
                //GostModeEveryoneUseresPerMonth = GostModeUseresPerMonth.FirstOrDefault(xx => x.indx == xx.Month)?.Count ?? 0,
                disablePushNotficationPerMonth = disablePushNotficationPerMonth.FirstOrDefault(xx => x.indx == xx.Month)?.Count ?? 0,
                NumberOfRequestesPerMonth = NumberOfRequestesPerMonth.FirstOrDefault(xx => x.indx == xx.Month)?.Count ?? 0,
                NumberOfBlockRequestesPerMonth = NumberOfBlockRequestesPerMonth.FirstOrDefault(xx => x.indx == xx.Month)?.Count ?? 0,
            }).ToList();
            var series = new List<object>()
            {
                new {name="Registered",data=Results.Select(x=>x.registered)},
                new {name="Deleted",data=Results.Select(x=>x.DeactiveUsers)},
                //new {name="Not Active Useres",data=Results.Select(x=>x.NotActiveUseresPerMonth)},
                new {name="Verified email",data=Results.Select(x=>x.ConfirmedMailUseresPerMonth)},
                new {name="Unverified email",data=Results.Select(x=>x.NotConfirmedMailUseresPerMonth)},
                //new {name="Private mode",data=Results.Select(x=>x.GostModeUseresPerMonth)},
                //new {name="Hide From Men",data=Results.Select(x=>x.GostModeMenUseresPerMonth)},
                //new {name="Hide From Women",data=Results.Select(x=>x.GostModeWomenUseresPerMonth)},
                //new {name="Hide From Other",data=Results.Select(x=>x.GostModeOtherUseresPerMonth)},
                //new {name="Hide From Everyone",data=Results.Select(x=>x.GostModeEveryoneUseresPerMonth)},
                new {name="Disable Push Notification",data=Results.Select(x=>x.disablePushNotficationPerMonth)},
                new {name="Connection requests",data=Results.Select(x=>x.NumberOfRequestesPerMonth)},
                new {name="Blocks",data=Results.Select(x=>x.NumberOfBlockRequestesPerMonth)},
            };
            return Ok(new { Monthes = Monthes, series = series });
        }

        public IActionResult EventStatictesPerMonth(int? Year)
        {
            var DbF = Microsoft.EntityFrameworkCore.EF.Functions;
            Year = Year ?? DateTime.Now.Year;
            var DateTimeNow = DateTime.UtcNow.Date;
            DateTime firstDay = new DateTime((int)Year, 1, 1);
            DateTime lastDay = new DateTime((int)Year, 12, 31);
            var allEventsInYear = authDBContext.EventData.Where(x => DbF.DateDiffDay(firstDay, x.CreatedDate) >= 0 && DbF.DateDiffDay(x.CreatedDate, lastDay) >= 0);

            var AllEventPerMonth = allEventsInYear.GroupBy(x => x.CreatedDate.Value.Date.Month).Select(x => new
            {
                Month = x.Key,
                Count = x.Count()
            }).OrderBy(x => x.Month).ToList();

            var PrivateEventPerMonth = allEventsInYear.Where(x => x.EventTypeListid == 1).GroupBy(x => x.CreatedDate.Value.Date.Month).Select(x => new
            {
                Month = x.Key,
                Count = x.Count()
            }).OrderBy(x => x.Month).ToList();
            var FriendzrEventPerMonth = allEventsInYear.Where(x => x.EventTypeListid == 2).GroupBy(x => x.CreatedDate.Value.Date.Month).Select(x => new
            {
                Month = x.Key,
                Count = x.Count()
            }).OrderBy(x => x.Month).ToList();
            var ExternalEventPerMonth = allEventsInYear.Where(x => x.EventTypeListid == 3).GroupBy(x => x.CreatedDate.Value.Date.Month).Select(x => new
            {
                Month = x.Key,
                Count = x.Count()
            }).OrderBy(x => x.Month).ToList();
            var AdminExternalEventPerMonth = allEventsInYear.Where(x => x.EventTypeListid == 4).GroupBy(x => x.CreatedDate.Value.Date.Month).Select(x => new
            {
                Month = x.Key,
                Count = x.Count()
            }).OrderBy(x => x.Month).ToList();
            var WhiteLableEventPerMonth = allEventsInYear.Where(x => x.EventTypeListid == 5).GroupBy(x => x.CreatedDate.Value.Date.Month).Select(x => new
            {
                Month = x.Key,
                Count = x.Count()
            }).OrderBy(x => x.Month).ToList();

            var Monthes = Enumerable.Range(1, 12).Select(i => new { indx = i, Month = DateTimeFormatInfo.CurrentInfo.GetAbbreviatedMonthName(i) });
            var Results = Monthes.Select(x => new
            {
                Months = x.Month,
                AllEventPerMonth = AllEventPerMonth.FirstOrDefault(xx => x.indx == xx.Month)?.Count ?? 0,
                PrivateEventPerMonth = PrivateEventPerMonth.FirstOrDefault(xx => x.indx == xx.Month)?.Count ?? 0,
                FriendzrEventPerMonth = FriendzrEventPerMonth.FirstOrDefault(xx => x.indx == xx.Month)?.Count ?? 0,
                ExternalEventPerMonth = ExternalEventPerMonth.FirstOrDefault(xx => x.indx == xx.Month)?.Count ?? 0,
                AdminExternalEventPerMonth = AdminExternalEventPerMonth.FirstOrDefault(xx => x.indx == xx.Month)?.Count ?? 0,
                WhiteLableEventPerMonth = WhiteLableEventPerMonth.FirstOrDefault(xx => x.indx == xx.Month)?.Count ?? 0,
            }).ToList();
            var series = new List<object>()
            {
                new {name="All Events",data=Results.Select(x=>x.AllEventPerMonth)},
                new {name="Private Events",data=Results.Select(x=>x.PrivateEventPerMonth)},
                new {name="Friendzr Events",data=Results.Select(x=>x.FriendzrEventPerMonth)},
                new {name="External Events",data=Results.Select(x=>x.ExternalEventPerMonth)},
                new {name="Admin External Events",data=Results.Select(x=>x.AdminExternalEventPerMonth)},
                new {name="WhiteLable Events",data=Results.Select(x=>x.WhiteLableEventPerMonth)},
            };
            return Ok(new { Monthes = Monthes, series = series });
        }

        public IActionResult EventsInEachCategory(int? Year)
        {
            var Categories = authDBContext.category.Select(x => new { CategoryName = x.name, NumOfCreatedEvent = x.EventData.Count });
            var series = new List<object>()
            {
                new {name="NumberOfCreatedEvents",data=Categories.Select(x=>x.NumOfCreatedEvent)},
            };
            return Ok(new { categories = Categories.Select(x => x.CategoryName), series = series });
        }

        public IActionResult FilterByEventCategory()
        {
            var categories = authDBContext.category.Select(x => new { CategoryName = x.name, NumOfFilter = x.EventCategoryTrackers.Count });

            var series = new List<object>()
            {
                new {name="NumberOfFilter",data = categories.Select(x=>x.NumOfFilter)},
            };

            return Ok(new { categories = categories.Select(x => x.CategoryName), series = series });
        }

        public async Task<IActionResult> UsersInEachInterest()
        {
            var interests = await authDBContext.Interests.Include(q => q.listoftags).Select(q => new { InterestName = q.name, NumOfUsers = q.listoftags.Count() }).ToListAsync();

            var series = new List<object>()
            {
                new {name="NumberOfUsers",data=interests.Select(q=>q.NumOfUsers)},
            };

            return Ok(new { Interests = interests.Select(x => x.InterestName), series = series });
        }

        public IActionResult AppStatictesPerMonth(int Year)
        {
            var distancelist = distanceFilterHistoryService.GetAll(Year).ToList();
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

        #region Link Clicks Statistics

        public async Task<IActionResult> LinkClickAgeAndGenderStatistics([FromQuery] int year, [FromQuery] string by, [FromQuery] string linkType)
        {
            try
            {
                var monthes = Enumerable.Range(1, 12).Select(i => new { index = i, Month = DateTimeFormatInfo.CurrentInfo.GetAbbreviatedMonthName(i) });

                List<UserLinkClick> userLinkClicks = await authDBContext.UserLinkClicks.Include(q => q.userDetails).Where(q => q.Type == linkType && q.userDetails.User.Email.ToLower().Contains("@owner") == false && q.Date.Date.Year == (new DateTime(Convert.ToInt32(year.ToString()), 1, 1)).Date.Year && q.userDetails.birthdate != null && q.userDetails.listoftags.Any() && q.userDetails.User.EmailConfirmed == true && (q.userDetails.ProfileCompleted != null && q.userDetails.ProfileCompleted == true)).ToListAsync();

                var userLinkClicksbyMonthMale = userLinkClicks.Where(q => q.userDetails.Gender == "male").GroupBy(x => new { Month = x.Date.Month }).Select(x => new { Month = x.Key.Month, Count = x.Count(), Birthdate = x.FirstOrDefault().userDetails.birthdate }).OrderBy(x => x.Month).ToList();
                var userLinkClicksbyMonthFemale = userLinkClicks.Where(q => q.userDetails.Gender == "female").GroupBy(x => new { Month = x.Date.Month }).Select(x => new { Month = x.Key.Month, Count = x.Count(), Birthdate = x.FirstOrDefault().userDetails.birthdate }).OrderBy(x => x.Month).ToList();
                var userLinkClicksbyMonthOther = userLinkClicks.Where(q => q.userDetails.Gender == "other").GroupBy(x => new { Month = x.Date.Month }).Select(x => new { Month = x.Key.Month, Count = x.Count(), Birthdate = x.FirstOrDefault().userDetails.birthdate }).OrderBy(x => x.Month).ToList();

                var From18To24 = userLinkClicks.Where(q => GetAge(q.userDetails.birthdate.Value) >= 18 && GetAge(q.userDetails.birthdate.Value) <= 25).GroupBy(x => new { Month = x.Date.Month }).Select(x => new { Month = x.Key.Month, Count = x.Count(), Birthdate = x.FirstOrDefault().userDetails.birthdate }).OrderBy(x => x.Month).ToList();
                var From25To34 = userLinkClicks.Where(q => GetAge(q.userDetails.birthdate.Value) >= 26 && GetAge(q.userDetails.birthdate.Value) <= 34).GroupBy(x => new { Month = x.Date.Month }).Select(x => new { Month = x.Key.Month, Count = x.Count(), Birthdate = x.FirstOrDefault().userDetails.birthdate }).OrderBy(x => x.Month).ToList();
                var From35To44 = userLinkClicks.Where(q => GetAge(q.userDetails.birthdate.Value) >= 35 && GetAge(q.userDetails.birthdate.Value) <= 44).GroupBy(x => new { Month = x.Date.Month }).Select(x => new { Month = x.Key.Month, Count = x.Count(), Birthdate = x.FirstOrDefault().userDetails.birthdate }).OrderBy(x => x.Month).ToList();
                var From45To54 = userLinkClicks.Where(q => GetAge(q.userDetails.birthdate.Value) >= 45 && GetAge(q.userDetails.birthdate.Value) <= 54).GroupBy(x => new { Month = x.Date.Month }).Select(x => new { Month = x.Key.Month, Count = x.Count(), Birthdate = x.FirstOrDefault().userDetails.birthdate }).OrderBy(x => x.Month).ToList();
                var From55To64 = userLinkClicks.Where(q => GetAge(q.userDetails.birthdate.Value) >= 55 && GetAge(q.userDetails.birthdate.Value) <= 64).GroupBy(x => new { Month = x.Date.Month }).Select(x => new { Month = x.Key.Month, Count = x.Count(), Birthdate = x.FirstOrDefault().userDetails.birthdate }).OrderBy(x => x.Month).ToList();
                var MoreThan65 = userLinkClicks.Where(q => GetAge(q.userDetails.birthdate.Value) >= 65).GroupBy(x => new { Month = x.Date.Month }).Select(x => new { Month = x.Key.Month, Count = x.Count(), Birthdate = x.FirstOrDefault().userDetails.birthdate }).OrderBy(x => x.Month).ToList();

                var results = monthes.Select(m => new
                {
                    Months = m.Month,
                    Male = userLinkClicksbyMonthMale.Where(n => m.index == n.Month).FirstOrDefault()?.Count ?? 0,
                    Female = userLinkClicksbyMonthFemale.Where(n => m.index == n.Month).FirstOrDefault()?.Count ?? 0,
                    Other = userLinkClicksbyMonthOther.Where(n => m.index == n.Month).FirstOrDefault()?.Count ?? 0,
                    From18To24 = From18To24.Where(n => m.index == n.Month).FirstOrDefault()?.Count ?? 0,
                    From25To34 = From25To34.Where(n => m.index == n.Month).FirstOrDefault()?.Count ?? 0,
                    From35To44 = From35To44.Where(n => m.index == n.Month).FirstOrDefault()?.Count ?? 0,
                    From45To54 = From45To54.Where(n => m.index == n.Month).FirstOrDefault()?.Count ?? 0,
                    From55To64 = From55To64.Where(n => m.index == n.Month).FirstOrDefault()?.Count ?? 0,
                    MoreThan65 = MoreThan65.Where(n => m.index == n.Month).FirstOrDefault()?.Count ?? 0,
                }).ToList();

                var series = new List<object>();

                if (by == "SeeByGender")
                {
                    series = new List<object>()
                    {
                        new {name="Male",data=results.Select(q => q.Male)},
                        new {name="Female",data=results.Select(q => q.Female)},
                        new {name="Other",data=results.Select(q => q.Other)},
                    };
                }
                else
                {
                    series = new List<object>()
                    {
                        new {name="From 18 to 24",data=results.Select(q => q.From18To24)},
                        new {name="From 25 to 34",data=results.Select(q => q.From25To34)},
                        new {name="From 35 to 44",data=results.Select(q => q.From35To44)},
                        new {name="From 45 to 54",data=results.Select(q => q.From45To54)},
                        new {name="From 55 to 64",data=results.Select(q => q.From55To64)},
                        new {name="More than 65" ,data=results.Select(q => q.MoreThan65)},
                    };

                }

                return Ok(new { Monthes = monthes, series = series });
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        #endregion

        #region Event Statistics

        public async Task<IActionResult> NumberOfSharedEventStatistics([FromQuery] int year, [FromQuery] string by)
        {
            try
            {
                List<EventTracker> eventTrackers = await authDBContext.EventTrackers.Include(q => q.User).Include(q => q.Event).Where(q => q.ActionType == EventActionType.Share.ToString() && q.User.Email.ToLower().Contains("@owner") == false && q.Date.Date.Year == (new DateTime(Convert.ToInt32(year.ToString()), 1, 1)).Date.Year && q.User.listoftags.Any() && q.User.birthdate != null && q.User.User.EmailConfirmed == true && (q.User.ProfileCompleted != null && q.User.ProfileCompleted == true)).ToListAsync();

                var groupedEventTracker = eventTrackers.GroupBy(q => new { q.EventId, q.UserId }).Select(q => new EventTracker() { Id = q.FirstOrDefault().Id, UserId = q.FirstOrDefault().UserId, EventId = q.FirstOrDefault().EventId, Date = q.FirstOrDefault().Date, ActionType = q.FirstOrDefault().ActionType }).ToList();

                var eventTrackerbyMonth = groupedEventTracker.GroupBy(x => new { Month = x.Date.Month, GenderType = eventTrackers.FirstOrDefault(q => q.UserId == x.UserId).User.Gender }).Select(x => new { Month = x.Key.Month, Count = x.Count(), Gender = x.Key.GenderType, Birthdate = eventTrackers.FirstOrDefault(q => q.UserId == x.FirstOrDefault().UserId).User.birthdate }).OrderBy(x => x.Month).ToList();

                var From18To24 = groupedEventTracker.Where(q => GetAge(eventTrackers.FirstOrDefault(u => u.UserId == q.UserId).User.birthdate.Value) >= 18 && GetAge(eventTrackers.FirstOrDefault(u => u.UserId == q.UserId).User.birthdate.Value) <= 25).GroupBy(x => new { Month = x.Date.Month }).Select(x => new { Month = x.Key.Month, Count = x.Count(), Birthdate = eventTrackers.FirstOrDefault(u => u.UserId == x.FirstOrDefault().UserId).User.birthdate.Value }).OrderBy(x => x.Month).ToList();
                var From25To34 = groupedEventTracker.Where(q => GetAge(eventTrackers.FirstOrDefault(u => u.UserId == q.UserId).User.birthdate.Value) >= 26 && GetAge(eventTrackers.FirstOrDefault(u => u.UserId == q.UserId).User.birthdate.Value) <= 34).GroupBy(x => new { Month = x.Date.Month }).Select(x => new { Month = x.Key.Month, Count = x.Count(), Birthdate = eventTrackers.FirstOrDefault(u => u.UserId == x.FirstOrDefault().UserId).User.birthdate.Value }).OrderBy(x => x.Month).ToList();
                var From35To44 = groupedEventTracker.Where(q => GetAge(eventTrackers.FirstOrDefault(u => u.UserId == q.UserId).User.birthdate.Value) >= 35 && GetAge(eventTrackers.FirstOrDefault(u => u.UserId == q.UserId).User.birthdate.Value) <= 44).GroupBy(x => new { Month = x.Date.Month }).Select(x => new { Month = x.Key.Month, Count = x.Count(), Birthdate = eventTrackers.FirstOrDefault(u => u.UserId == x.FirstOrDefault().UserId).User.birthdate.Value }).OrderBy(x => x.Month).ToList();
                var From45To54 = groupedEventTracker.Where(q => GetAge(eventTrackers.FirstOrDefault(u => u.UserId == q.UserId).User.birthdate.Value) >= 45 && GetAge(eventTrackers.FirstOrDefault(u => u.UserId == q.UserId).User.birthdate.Value) <= 54).GroupBy(x => new { Month = x.Date.Month }).Select(x => new { Month = x.Key.Month, Count = x.Count(), Birthdate = eventTrackers.FirstOrDefault(u => u.UserId == x.FirstOrDefault().UserId).User.birthdate.Value }).OrderBy(x => x.Month).ToList();
                var From55To64 = groupedEventTracker.Where(q => GetAge(eventTrackers.FirstOrDefault(u => u.UserId == q.UserId).User.birthdate.Value) >= 55 && GetAge(eventTrackers.FirstOrDefault(u => u.UserId == q.UserId).User.birthdate.Value) <= 64).GroupBy(x => new { Month = x.Date.Month }).Select(x => new { Month = x.Key.Month, Count = x.Count(), Birthdate = eventTrackers.FirstOrDefault(u => u.UserId == x.FirstOrDefault().UserId).User.birthdate.Value }).OrderBy(x => x.Month).ToList();
                var MoreThan65 = groupedEventTracker.Where(q => GetAge(eventTrackers.FirstOrDefault(u => u.UserId == q.UserId).User.birthdate.Value) >= 65).GroupBy(x => new { Month = x.Date.Month }).Select(x => new { Month = x.Key.Month, Count = x.Count(), Birthdate = eventTrackers.FirstOrDefault(u => u.UserId == x.FirstOrDefault().UserId).User.birthdate.Value }).OrderBy(x => x.Month).ToList();

                var Monthes = Enumerable.Range(1, 12).Select(i => new { index = i, Month = DateTimeFormatInfo.CurrentInfo.GetAbbreviatedMonthName(i) });

                var results = Monthes.Select(m => new
                {
                    Male = eventTrackerbyMonth.Where(n => m.index == n.Month).FirstOrDefault(q => q.Gender == "male")?.Count ?? 0,
                    Female = eventTrackerbyMonth.Where(n => m.index == n.Month).FirstOrDefault(q => q.Gender == "female")?.Count ?? 0,
                    Other = eventTrackerbyMonth.Where(n => m.index == n.Month).FirstOrDefault(q => q.Gender == "other")?.Count ?? 0,
                    From18To24 = From18To24.Where(n => m.index == n.Month).FirstOrDefault()?.Count ?? 0,
                    From25To34 = From25To34.Where(n => m.index == n.Month).FirstOrDefault()?.Count ?? 0,
                    From35To44 = From35To44.Where(n => m.index == n.Month).FirstOrDefault()?.Count ?? 0,
                    From45To54 = From45To54.Where(n => m.index == n.Month).FirstOrDefault()?.Count ?? 0,
                    From55To64 = From55To64.Where(n => m.index == n.Month).FirstOrDefault()?.Count ?? 0,
                    MoreThan65 = MoreThan65.Where(n => m.index == n.Month).FirstOrDefault()?.Count ?? 0,
                }).ToList();

                var series = new List<object>();

                if (by == "SeeByGender")
                {
                    series = new List<object>()
                    {
                        new {name="Male",data=results.Select(q => q.Male)},
                        new {name="Female",data=results.Select(q => q.Female)},
                        new {name="Other",data=results.Select(q => q.Other)},
                    };
                }
                else
                {
                    series = new List<object>()
                    {
                        new {name="From 18 to 24",data=results.Select(q => q.From18To24)},
                        new {name="From 25 to 34",data=results.Select(q => q.From25To34)},
                        new {name="From 35 to 44",data=results.Select(q => q.From35To44)},
                        new {name="From 45 to 54",data=results.Select(q => q.From45To54)},
                        new {name="From 55 to 64",data=results.Select(q => q.From55To64)},
                        new {name="More than 65" ,data=results.Select(q => q.MoreThan65)},
                    };

                }

                return Ok(new { Monthes = Monthes, series = series });
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<IActionResult> NumberOfViewedEventStatistics([FromQuery] int year, [FromQuery] string by)
        {
            try
            {
                List<EventTracker> eventTrackers = await authDBContext.EventTrackers.Include(q => q.User).Include(q => q.Event).Where(q => q.ActionType == EventActionType.View.ToString() && q.User.Email.ToLower().Contains("@owner") == false && q.Date.Date.Year == (new DateTime(Convert.ToInt32(year.ToString()), 1, 1)).Date.Year && q.User.User.EmailConfirmed == true && q.User.listoftags.Any() && q.User.birthdate != null && (q.User.ProfileCompleted != null && q.User.ProfileCompleted == true)).ToListAsync();

                var groupedEventTracker = eventTrackers.GroupBy(q => new { q.EventId, q.UserId }).Select(q => new EventTracker() { Id = q.FirstOrDefault().Id, UserId = q.FirstOrDefault().UserId, EventId = q.FirstOrDefault().EventId, Date = q.FirstOrDefault().Date, ActionType = q.FirstOrDefault().ActionType }).ToList();

                var eventTrackerbyMonth = groupedEventTracker.GroupBy(x => new { Month = x.Date.Month, GenderType = eventTrackers.FirstOrDefault(q => q.UserId == x.UserId).User.Gender }).Select(x => new { Month = x.Key.Month, Count = x.Count(), Gender = x.Key.GenderType, Birthdate = eventTrackers.FirstOrDefault(q => q.UserId == x.FirstOrDefault().UserId).User.birthdate }).OrderBy(x => x.Month).ToList();

                var From18To24 = groupedEventTracker.Where(q => GetAge(eventTrackers.FirstOrDefault(u => u.UserId == q.UserId).User.birthdate.Value) >= 18 && GetAge(eventTrackers.FirstOrDefault(u => u.UserId == q.UserId).User.birthdate.Value) <= 25).GroupBy(x => new { Month = x.Date.Month }).Select(x => new { Month = x.Key.Month, Count = x.Count(), Birthdate = eventTrackers.FirstOrDefault(u => u.UserId == x.FirstOrDefault().UserId).User.birthdate.Value }).OrderBy(x => x.Month).ToList();
                var From25To34 = groupedEventTracker.Where(q => GetAge(eventTrackers.FirstOrDefault(u => u.UserId == q.UserId).User.birthdate.Value) >= 26 && GetAge(eventTrackers.FirstOrDefault(u => u.UserId == q.UserId).User.birthdate.Value) <= 34).GroupBy(x => new { Month = x.Date.Month }).Select(x => new { Month = x.Key.Month, Count = x.Count(), Birthdate = eventTrackers.FirstOrDefault(u => u.UserId == x.FirstOrDefault().UserId).User.birthdate.Value }).OrderBy(x => x.Month).ToList();
                var From35To44 = groupedEventTracker.Where(q => GetAge(eventTrackers.FirstOrDefault(u => u.UserId == q.UserId).User.birthdate.Value) >= 35 && GetAge(eventTrackers.FirstOrDefault(u => u.UserId == q.UserId).User.birthdate.Value) <= 44).GroupBy(x => new { Month = x.Date.Month }).Select(x => new { Month = x.Key.Month, Count = x.Count(), Birthdate = eventTrackers.FirstOrDefault(u => u.UserId == x.FirstOrDefault().UserId).User.birthdate.Value }).OrderBy(x => x.Month).ToList();
                var From45To54 = groupedEventTracker.Where(q => GetAge(eventTrackers.FirstOrDefault(u => u.UserId == q.UserId).User.birthdate.Value) >= 45 && GetAge(eventTrackers.FirstOrDefault(u => u.UserId == q.UserId).User.birthdate.Value) <= 54).GroupBy(x => new { Month = x.Date.Month }).Select(x => new { Month = x.Key.Month, Count = x.Count(), Birthdate = eventTrackers.FirstOrDefault(u => u.UserId == x.FirstOrDefault().UserId).User.birthdate.Value }).OrderBy(x => x.Month).ToList();
                var From55To64 = groupedEventTracker.Where(q => GetAge(eventTrackers.FirstOrDefault(u => u.UserId == q.UserId).User.birthdate.Value) >= 55 && GetAge(eventTrackers.FirstOrDefault(u => u.UserId == q.UserId).User.birthdate.Value) <= 64).GroupBy(x => new { Month = x.Date.Month }).Select(x => new { Month = x.Key.Month, Count = x.Count(), Birthdate = eventTrackers.FirstOrDefault(u => u.UserId == x.FirstOrDefault().UserId).User.birthdate.Value }).OrderBy(x => x.Month).ToList();
                var MoreThan65 = groupedEventTracker.Where(q => GetAge(eventTrackers.FirstOrDefault(u => u.UserId == q.UserId).User.birthdate.Value) >= 65).GroupBy(x => new { Month = x.Date.Month }).Select(x => new { Month = x.Key.Month, Count = x.Count(), Birthdate = eventTrackers.FirstOrDefault(u => u.UserId == x.FirstOrDefault().UserId).User.birthdate.Value }).OrderBy(x => x.Month).ToList();

                var Monthes = Enumerable.Range(1, 12).Select(i => new { index = i, Month = DateTimeFormatInfo.CurrentInfo.GetAbbreviatedMonthName(i) });

                var results = Monthes.Select(m => new
                {
                    Male = eventTrackerbyMonth.Where(n => m.index == n.Month).FirstOrDefault(q => q.Gender == "male")?.Count ?? 0,
                    Female = eventTrackerbyMonth.Where(n => m.index == n.Month).FirstOrDefault(q => q.Gender == "female")?.Count ?? 0,
                    Other = eventTrackerbyMonth.Where(n => m.index == n.Month).FirstOrDefault(q => q.Gender == "other")?.Count ?? 0,
                    From18To24 = From18To24.Where(n => m.index == n.Month).FirstOrDefault()?.Count ?? 0,
                    From25To34 = From25To34.Where(n => m.index == n.Month).FirstOrDefault()?.Count ?? 0,
                    From35To44 = From35To44.Where(n => m.index == n.Month).FirstOrDefault()?.Count ?? 0,
                    From45To54 = From45To54.Where(n => m.index == n.Month).FirstOrDefault()?.Count ?? 0,
                    From55To64 = From55To64.Where(n => m.index == n.Month).FirstOrDefault()?.Count ?? 0,
                    MoreThan65 = MoreThan65.Where(n => m.index == n.Month).FirstOrDefault()?.Count ?? 0,
                }).ToList();

                var series = new List<object>();

                if (by == "SeeByGender")
                {
                    series = new List<object>()
                    {
                        new {name="Male",data=results.Select(q => q.Male)},
                        new {name="Female",data=results.Select(q => q.Female)},
                        new {name="Other",data=results.Select(q => q.Other)},
                    };
                }
                else
                {
                    series = new List<object>()
                    {
                        new {name="From 18 to 24",data=results.Select(q => q.From18To24)},
                        new {name="From 25 to 34",data=results.Select(q => q.From25To34)},
                        new {name="From 35 to 44",data=results.Select(q => q.From35To44)},
                        new {name="From 45 to 54",data=results.Select(q => q.From45To54)},
                        new {name="From 55 to 64",data=results.Select(q => q.From55To64)},
                        new {name="More than 65" ,data=results.Select(q => q.MoreThan65)},
                    };

                }

                return Ok(new { Monthes = Monthes, series = series });
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<IActionResult> NumberOfAttendedEventStatistics([FromQuery] int year, [FromQuery] string by)
        {
            try
            {
                List<EventTracker> eventTrackers = await authDBContext.EventTrackers.Include(q => q.User).Include(q => q.Event).Where(q => q.ActionType == EventActionType.Attend.ToString() && !q.User.Email.ToLower().Contains("@owner") && q.Date.Date.Year == (new DateTime(Convert.ToInt32(year.ToString()), 1, 1)).Date.Year && q.User.birthdate != null && q.User.User.EmailConfirmed == true && (q.User.ProfileCompleted != null && q.User.ProfileCompleted == true)).ToListAsync();

                var groupedEventTracker = eventTrackers.GroupBy(q => new { q.EventId, q.UserId }).Select(q => new EventTracker() { Id = q.FirstOrDefault().Id, UserId = q.FirstOrDefault().UserId, EventId = q.FirstOrDefault().EventId, Date = q.FirstOrDefault().Date, ActionType = q.FirstOrDefault().ActionType }).ToList();

                var eventTrackerbyMonth = groupedEventTracker.GroupBy(x => new { Month = x.Date.Month, GenderType = eventTrackers.FirstOrDefault(q => q.UserId == x.UserId).User.Gender }).Select(x => new { Month = x.Key.Month, Count = x.Count(), Gender = x.Key.GenderType, Birthdate = eventTrackers.FirstOrDefault(q => q.UserId == x.FirstOrDefault().UserId).User.birthdate }).OrderBy(x => x.Month).ToList();

                var From18To24 = groupedEventTracker.Where(q => GetAge(eventTrackers.FirstOrDefault(u => u.UserId == q.UserId).User.birthdate.Value) >= 18 && GetAge(eventTrackers.FirstOrDefault(u => u.UserId == q.UserId).User.birthdate.Value) <= 25).GroupBy(x => new { Month = x.Date.Month }).Select(x => new { Month = x.Key.Month, Count = x.Count(), Birthdate = eventTrackers.FirstOrDefault(u => u.UserId == x.FirstOrDefault().UserId).User.birthdate.Value }).OrderBy(x => x.Month).ToList();
                var From25To34 = groupedEventTracker.Where(q => GetAge(eventTrackers.FirstOrDefault(u => u.UserId == q.UserId).User.birthdate.Value) >= 26 && GetAge(eventTrackers.FirstOrDefault(u => u.UserId == q.UserId).User.birthdate.Value) <= 34).GroupBy(x => new { Month = x.Date.Month }).Select(x => new { Month = x.Key.Month, Count = x.Count(), Birthdate = eventTrackers.FirstOrDefault(u => u.UserId == x.FirstOrDefault().UserId).User.birthdate.Value }).OrderBy(x => x.Month).ToList();
                var From35To44 = groupedEventTracker.Where(q => GetAge(eventTrackers.FirstOrDefault(u => u.UserId == q.UserId).User.birthdate.Value) >= 35 && GetAge(eventTrackers.FirstOrDefault(u => u.UserId == q.UserId).User.birthdate.Value) <= 44).GroupBy(x => new { Month = x.Date.Month }).Select(x => new { Month = x.Key.Month, Count = x.Count(), Birthdate = eventTrackers.FirstOrDefault(u => u.UserId == x.FirstOrDefault().UserId).User.birthdate.Value }).OrderBy(x => x.Month).ToList();
                var From45To54 = groupedEventTracker.Where(q => GetAge(eventTrackers.FirstOrDefault(u => u.UserId == q.UserId).User.birthdate.Value) >= 45 && GetAge(eventTrackers.FirstOrDefault(u => u.UserId == q.UserId).User.birthdate.Value) <= 54).GroupBy(x => new { Month = x.Date.Month }).Select(x => new { Month = x.Key.Month, Count = x.Count(), Birthdate = eventTrackers.FirstOrDefault(u => u.UserId == x.FirstOrDefault().UserId).User.birthdate.Value }).OrderBy(x => x.Month).ToList();
                var From55To64 = groupedEventTracker.Where(q => GetAge(eventTrackers.FirstOrDefault(u => u.UserId == q.UserId).User.birthdate.Value) >= 55 && GetAge(eventTrackers.FirstOrDefault(u => u.UserId == q.UserId).User.birthdate.Value) <= 64).GroupBy(x => new { Month = x.Date.Month }).Select(x => new { Month = x.Key.Month, Count = x.Count(), Birthdate = eventTrackers.FirstOrDefault(u => u.UserId == x.FirstOrDefault().UserId).User.birthdate.Value }).OrderBy(x => x.Month).ToList();
                var MoreThan65 = groupedEventTracker.Where(q => GetAge(eventTrackers.FirstOrDefault(u => u.UserId == q.UserId).User.birthdate.Value) >= 65).GroupBy(x => new { Month = x.Date.Month }).Select(x => new { Month = x.Key.Month, Count = x.Count(), Birthdate = eventTrackers.FirstOrDefault(u => u.UserId == x.FirstOrDefault().UserId).User.birthdate.Value }).OrderBy(x => x.Month).ToList();

                var Monthes = Enumerable.Range(1, 12).Select(i => new { index = i, Month = DateTimeFormatInfo.CurrentInfo.GetAbbreviatedMonthName(i) });

                var results = Monthes.Select(m => new
                {
                    Male = eventTrackerbyMonth.Where(n => m.index == n.Month).FirstOrDefault(q => q.Gender == "male")?.Count ?? 0,
                    Female = eventTrackerbyMonth.Where(n => m.index == n.Month).FirstOrDefault(q => q.Gender == "female")?.Count ?? 0,
                    Other = eventTrackerbyMonth.Where(n => m.index == n.Month).FirstOrDefault(q => q.Gender == "other")?.Count ?? 0,
                    From18To24 = From18To24.Where(n => m.index == n.Month).FirstOrDefault()?.Count ?? 0,
                    From25To34 = From25To34.Where(n => m.index == n.Month).FirstOrDefault()?.Count ?? 0,
                    From35To44 = From35To44.Where(n => m.index == n.Month).FirstOrDefault()?.Count ?? 0,
                    From45To54 = From45To54.Where(n => m.index == n.Month).FirstOrDefault()?.Count ?? 0,
                    From55To64 = From55To64.Where(n => m.index == n.Month).FirstOrDefault()?.Count ?? 0,
                    MoreThan65 = MoreThan65.Where(n => m.index == n.Month).FirstOrDefault()?.Count ?? 0,
                }).ToList();

                var series = new List<object>();

                if (by == "SeeByGender")
                {
                    series = new List<object>()
                    {
                        new {name="Male",data=results.Select(q => q.Male)},
                        new {name="Female",data=results.Select(q => q.Female)},
                        new {name="Other",data=results.Select(q => q.Other)},
                    };
                }
                else
                {
                    series = new List<object>()
                    {
                        new {name="From 18 to 24",data=results.Select(q => q.From18To24)},
                        new {name="From 25 to 34",data=results.Select(q => q.From25To34)},
                        new {name="From 35 to 44",data=results.Select(q => q.From35To44)},
                        new {name="From 45 to 54",data=results.Select(q => q.From45To54)},
                        new {name="From 55 to 64",data=results.Select(q => q.From55To64)},
                        new {name="More than 65" ,data=results.Select(q => q.MoreThan65)},
                    };

                }

                return Ok(new { Monthes = Monthes, series = series });
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<IActionResult> EventStatistics()
        {
            List<EventData> events = await authDBContext.EventData.AsNoTracking().ToListAsync();

            EventStatisticViewModel numberOfEventsCreatedByFriendzrStatistics = new EventStatisticViewModel()
            {
                All = events.Count(),
                Friendzr = events.Where(q => q.EventTypeListid == 1).Count()
            };
            return Ok(numberOfEventsCreatedByFriendzrStatistics);
        }

        #endregion

        #region App Age And Gender Statistics

        public async Task<IActionResult> FinishedRegistrationUserStatictes()
        {
            List<UserDetails> users = await authDBContext.UserDetails
                .Where(q => q.birthdate != null 
                            && q.listoftags.Any() 
                            && q.Email.ToLower().Contains("@owner") == false 
                            && q.Email != "dev@dev.com" 
                            && q.User.EmailConfirmed == true 
                            && (q.ProfileCompleted != null && q.ProfileCompleted == true))
                .ToListAsync();

           var usersOver18 = users.Where(q => GetAge(q.birthdate.Value) >= 18).ToList();

            StatisticsByGenderAndAgeViewModel finishedRegistrationUserStatictes = new StatisticsByGenderAndAgeViewModel()
            {
                All = users.Count(),
                Male = users.Where(q => q.Gender == "male").Count(),
                Female = users.Where(q => q.Gender == "female").Count(),
                Other = users.Where(q => q.Gender == "other").Count(),
                From18To25 = usersOver18.Where(q => GetAge(q.birthdate.Value) >= 18 && GetAge(q.birthdate.Value) <= 25).Count(),
                From25To34 = usersOver18.Where(q => GetAge(q.birthdate.Value) >= 26 && GetAge(q.birthdate.Value) <= 34).Count(),
                From35To44 = usersOver18.Where(q => GetAge(q.birthdate.Value) >= 35 && GetAge(q.birthdate.Value) <= 44).Count(),
                From45To54 = usersOver18.Where(q => GetAge(q.birthdate.Value) >= 45 && GetAge(q.birthdate.Value) <= 54).Count(),
                From55To64 = usersOver18.Where(q => GetAge(q.birthdate.Value) >= 55 && GetAge(q.birthdate.Value) <= 64).Count(),
                From65AndMore = usersOver18.Where(q => GetAge(q.birthdate.Value) >= 65).Count(),               
            };
            return Ok(finishedRegistrationUserStatictes);
        }

        public async Task<IActionResult> GenderOfUsersStatictes()
        {
            List<UserDetails> users = await authDBContext.UserDetails
                .Where(q => q.birthdate != null 
                            && q.listoftags.Any() 
                            && q.Email.ToLower().Contains("@owner") == false
                            && q.Email != "dev@dev.com" 
                            && q.User.EmailConfirmed == true
                            && (q.ProfileCompleted != null
                            && q.ProfileCompleted == true))
                .ToListAsync();
          //  users =  users.Where(q => GetAge(q.birthdate.Value) >= 18).ToList();
            StatisticsByGenderAndAgeViewModel genderOfUserStatictes = new StatisticsByGenderAndAgeViewModel()
            {
                All = users.Count(),
                Male = users.Where(q => q.Gender == "male").Count(),
                Female = users.Where(q => q.Gender == "female").Count(),
                Other = users.Where(q => q.Gender == "other").Count(),
            };
            return Ok(genderOfUserStatictes);
        }

        public async Task<IActionResult> UsersWhoEnabledPersonalSpaceStatictes()
        {
            List<UserDetails> users = await authDBContext.UserDetails.Where(q => q.birthdate != null && q.listoftags.Any() && q.Email.ToLower().Contains("@owner") == false && q.Email != "dev@dev.com" && q.User.EmailConfirmed == true && (q.ProfileCompleted != null && q.ProfileCompleted == true)).ToListAsync();
            users = users.Where(q => GetAge(q.birthdate.Value) >= 18).ToList();

            StatisticsByGenderAndAgeViewModel usersWhoEnabledPersonalSpaceStatictes = new StatisticsByGenderAndAgeViewModel()
            {
                All = users.Where(q => q.personalSpace).Count(),
                Male = users.Where(q => q.personalSpace && q.Gender == "male").Count(),
                Female = users.Where(q => q.personalSpace && q.Gender == "female").Count(),
                Other = users.Where(q => q.personalSpace && q.Gender == "other").Count(),
                From18To25 = users.Where(q => q.personalSpace && GetAge(q.birthdate.Value) >= 18 && GetAge(q.birthdate.Value) <= 25).Count(),
                From25To34 = users.Where(q => q.personalSpace && GetAge(q.birthdate.Value) >= 26 && GetAge(q.birthdate.Value) <= 34).Count(),
                From35To44 = users.Where(q => q.personalSpace && GetAge(q.birthdate.Value) >= 35 && GetAge(q.birthdate.Value) <= 44).Count(),
                From45To54 = users.Where(q => q.personalSpace && GetAge(q.birthdate.Value) >= 45 && GetAge(q.birthdate.Value) <= 54).Count(),
                From55To64 = users.Where(q => q.personalSpace && GetAge(q.birthdate.Value) >= 55 && GetAge(q.birthdate.Value) <= 64).Count(),
                From65AndMore = users.Where(q => q.personalSpace && GetAge(q.birthdate.Value) >= 65).Count(),
            };
            return Ok(usersWhoEnabledPersonalSpaceStatictes);
        }

        public async Task<IActionResult> NumberOfConnectionRequestsSentStatictes()
        {
            List<Requestes> users = await authDBContext.Requestes.Include(q => q.User).ThenInclude(q => q.User).Where(q => q.User.Email.ToLower().Contains("@owner") == false && q.User.birthdate != null && q.User.birthdate != null && q.User.User.EmailConfirmed == true && (q.User.ProfileCompleted != null && q.User.ProfileCompleted == true)).ToListAsync();
            users = users.Where(q => GetAge(q.User.birthdate.Value) >= 18).ToList();
            StatisticsByGenderAndAgeViewModel numberOfConnectionRequestsSentStatictes = new StatisticsByGenderAndAgeViewModel()
            {
                All = users.Count(),
                Male = users.Where(q => q.User.Gender == "male").Count(),
                Female = users.Where(q => q.User.Gender == "female").Count(),
                Other = users.Where(q => q.User.Gender == "other").Count(),
                From18To25 = users.Where(q => GetAge(q.User.birthdate.Value) >= 18 && GetAge(q.User.birthdate.Value) <= 25).Count(),
                From25To34 = users.Where(q => GetAge(q.User.birthdate.Value) >= 26 && GetAge(q.User.birthdate.Value) <= 34).Count(),
                From35To44 = users.Where(q => GetAge(q.User.birthdate.Value) >= 35 && GetAge(q.User.birthdate.Value) <= 44).Count(),
                From45To54 = users.Where(q => GetAge(q.User.birthdate.Value) >= 45 && GetAge(q.User.birthdate.Value) <= 54).Count(),
                From55To64 = users.Where(q => GetAge(q.User.birthdate.Value) >= 55 && GetAge(q.User.birthdate.Value) <= 64).Count(),
                From65AndMore = users.Where(q => GetAge(q.User.birthdate.Value) >= 65).Count(),
            };
            return Ok(numberOfConnectionRequestsSentStatictes);
        }

        public async Task<IActionResult> NumberOfConnectionRequestsAcceptedStatictes()
        {
            List<Requestes> requestes = await authDBContext.Requestes.Include(q => q.User).ThenInclude(q => q.User).Where(q => !q.User.Email.ToLower().Contains("@owner") && q.User.birthdate != null && q.User.User.EmailConfirmed == true && (q.User.ProfileCompleted != null && q.User.ProfileCompleted == true) && q.status == 1).ToListAsync();
            requestes = requestes.Where(q => GetAge(q.User.birthdate.Value) >= 18).ToList();
            StatisticsByGenderAndAgeViewModel numberOfConnectionRequestsAcceptedStatictes = new StatisticsByGenderAndAgeViewModel()
            {
                All = requestes.Count(),
                Male = requestes.Where(q => q.User.Gender == "male").Count(),
                Female = requestes.Where(q => q.User.Gender == "female").Count(),
                Other = requestes.Where(q => q.User.Gender == "other").Count(),
                From18To25 = requestes.Where(q => GetAge(q.User.birthdate.Value) >= 18 && GetAge(q.User.birthdate.Value) <= 25).Count(),
                From25To34 = requestes.Where(q => GetAge(q.User.birthdate.Value) >= 26 && GetAge(q.User.birthdate.Value) <= 34).Count(),
                From35To44 = requestes.Where(q => GetAge(q.User.birthdate.Value) >= 35 && GetAge(q.User.birthdate.Value) <= 44).Count(),
                From45To54 = requestes.Where(q => GetAge(q.User.birthdate.Value) >= 45 && GetAge(q.User.birthdate.Value) <= 54).Count(),
                From55To64 = requestes.Where(q => GetAge(q.User.birthdate.Value) >= 55 && GetAge(q.User.birthdate.Value) <= 64).Count(),
                From65AndMore = requestes.Where(q => GetAge(q.User.birthdate.Value) >= 65).Count(),
            };
            return Ok(numberOfConnectionRequestsAcceptedStatictes);
        }

        public async Task<IActionResult> BlockedUsersStatictes()
        {
            List<Requestes> requestes = await authDBContext.Requestes.Include(q => q.User).ThenInclude(q => q.User).Where(q => !q.User.Email.ToLower()
            .Contains("@owner") && q.User.birthdate != null && q.User.User.EmailConfirmed == true 
            && (q.User.ProfileCompleted != null && q.User.ProfileCompleted == true) && q.status == 2).ToListAsync();
            requestes = requestes.Where(q => GetAge(q.User.birthdate.Value) >= 18).ToList();

            StatisticsByGenderAndAgeViewModel blockedUsersStatictes = new StatisticsByGenderAndAgeViewModel()
            {
                All = requestes.Count(),
                Male = requestes.Where(q => q.User.Gender == "male").Count(),
                Female = requestes.Where(q => q.User.Gender == "female").Count(),
                Other = requestes.Where(q => q.User.Gender == "other").Count(),
                From18To25 = requestes.Where(q => GetAge(q.User.birthdate.Value) >= 18 && GetAge(q.User.birthdate.Value) <= 25).Count(),
                From25To34 = requestes.Where(q => GetAge(q.User.birthdate.Value) >= 26 && GetAge(q.User.birthdate.Value) <= 34).Count(),
                From35To44 = requestes.Where(q => GetAge(q.User.birthdate.Value) >= 35 && GetAge(q.User.birthdate.Value) <= 44).Count(),
                From45To54 = requestes.Where(q => GetAge(q.User.birthdate.Value) >= 45 && GetAge(q.User.birthdate.Value) <= 54).Count(),
                From55To64 = requestes.Where(q => GetAge(q.User.birthdate.Value) >= 55 && GetAge(q.User.birthdate.Value) <= 64).Count(),
                From65AndMore = requestes.Where(q => GetAge(q.User.birthdate.Value) >= 65).Count(),
            };
            return Ok(blockedUsersStatictes);
        }

        public async Task<IActionResult> ActiveUsersStatictes()
        {
            List<LoggedinUser> activeUsers = await authDBContext.LoggedinUser.Include(q => q.User).ThenInclude(q => q.UserDetails)
                .Where(q => q.User.Email.ToLower().Contains("@owner") == false && q.User.UserDetails.birthdate != null 
                && q.User.Email != "dev@dev.com" && q.User.EmailConfirmed == true && (q.User.UserDetails.ProfileCompleted != null
                && q.User.UserDetails.ProfileCompleted == true) && q.User.UserDetails.birthdate != null && q.User.UserDetails.listoftags.Any() 
                && q.ExpiredOn > DateTime.Now).ToListAsync();
            activeUsers = activeUsers.Where(q => GetAge(q.User.UserDetails.birthdate.Value) >= 18).ToList();
            StatisticsByGenderAndAgeViewModel ActiveUsersStatictes = new StatisticsByGenderAndAgeViewModel()
            {
                All = activeUsers.Count(),
                Male = activeUsers.Where(q => q.User.UserDetails.Gender == "male").Count(),
                Female = activeUsers.Where(q => q.User.UserDetails.Gender == "female").Count(),
                Other = activeUsers.Where(q => q.User.UserDetails.Gender == "other").Count(),
                From18To25 = activeUsers.Where(q => GetAge(q.User.UserDetails.birthdate.Value) >= 18 && GetAge(q.User.UserDetails.birthdate.Value) <= 25).Count(),
                From25To34 = activeUsers.Where(q => GetAge(q.User.UserDetails.birthdate.Value) >= 26 && GetAge(q.User.UserDetails.birthdate.Value) <= 34).Count(),
                From35To44 = activeUsers.Where(q => GetAge(q.User.UserDetails.birthdate.Value) >= 35 && GetAge(q.User.UserDetails.birthdate.Value) <= 44).Count(),
                From45To54 = activeUsers.Where(q => GetAge(q.User.UserDetails.birthdate.Value) >= 45 && GetAge(q.User.UserDetails.birthdate.Value) <= 54).Count(),
                From55To64 = activeUsers.Where(q => GetAge(q.User.UserDetails.birthdate.Value) >= 55 && GetAge(q.User.UserDetails.birthdate.Value) <= 64).Count(),
                From65AndMore = activeUsers.Where(q => GetAge(q.User.UserDetails.birthdate.Value) >= 65).Count(),
            };
            return Ok(ActiveUsersStatictes);
        }

        public async Task<IActionResult> PercentageOfUsersWhoCreatedEventsStatictes()
        {
            List<EventData> events = await authDBContext.EventData.Include(q => q.User).Where(q => !q.User.Email.ToLower().Contains("@owner") 
            && q.User.birthdate != null  && q.User.User.EmailConfirmed == true && (q.User.ProfileCompleted != null && q.User.ProfileCompleted == true)).ToListAsync();
            events = events.Where(e => GetAge(e.User.birthdate.Value) >= 18).ToList();
            var usersWhoCreatedEvents = events.GroupBy(x => x.UserId).ToList();

            StatisticsByGenderAndAgeViewModel percentageOfUsersWhoCreatedEventsStatictes = new StatisticsByGenderAndAgeViewModel()
            {
                All = usersWhoCreatedEvents.Count(),
                Male = usersWhoCreatedEvents.Where(q => q.FirstOrDefault().User.Gender == "male").Count(),
                Female = usersWhoCreatedEvents.Where(q => q.FirstOrDefault().User.Gender == "female").Count(),
                Other = usersWhoCreatedEvents.Where(q => q.FirstOrDefault().User.Gender == "other").Count(),
                From18To25 = usersWhoCreatedEvents.Where(q => GetAge(q.FirstOrDefault().User.birthdate.Value) >= 18 && GetAge(q.FirstOrDefault().User.birthdate.Value) <= 25).Count(),
                From25To34 = usersWhoCreatedEvents.Where(q => GetAge(q.FirstOrDefault().User.birthdate.Value) >= 26 && GetAge(q.FirstOrDefault().User.birthdate.Value) <= 34).Count(),
                From35To44 = usersWhoCreatedEvents.Where(q => GetAge(q.FirstOrDefault().User.birthdate.Value) >= 35 && GetAge(q.FirstOrDefault().User.birthdate.Value) <= 44).Count(),
                From45To54 = usersWhoCreatedEvents.Where(q => GetAge(q.FirstOrDefault().User.birthdate.Value) >= 45 && GetAge(q.FirstOrDefault().User.birthdate.Value) <= 54).Count(),
                From55To64 = usersWhoCreatedEvents.Where(q => GetAge(q.FirstOrDefault().User.birthdate.Value) >= 55 && GetAge(q.FirstOrDefault().User.birthdate.Value) <= 64).Count(),
                From65AndMore = usersWhoCreatedEvents.Where(q => GetAge(q.FirstOrDefault().User.birthdate.Value) >= 65).Count(),
            };
            return Ok(percentageOfUsersWhoCreatedEventsStatictes);
        }

        #endregion

        #region Statistics Export To Excel


        public async Task<IActionResult> ExportAllEmailsAsExcel()
        {
            List<User> allUsers = await authDBContext.Users.Include(q => q.UserDetails).Where(q => q.Email.ToLower().Contains("@owner") == false && q.Email != "dev@dev.com").ToListAsync();

            List<User> finishedRegistrationUsers = allUsers.Where(q => q.EmailConfirmed && (q.UserDetails != null && (q.UserDetails.ProfileCompleted != null || q.UserDetails.ProfileCompleted == true))).ToList();
            List<User> verifiedUsers = allUsers.Where(q => q.EmailConfirmed && (q.UserDetails != null && (q.UserDetails.ProfileCompleted == null || q.UserDetails.ProfileCompleted == false))).ToList();
            List<User> notVerifiedUsers = allUsers.Where(q => !q.EmailConfirmed && (q.UserDetails != null && (q.UserDetails.ProfileCompleted == null || q.UserDetails.ProfileCompleted == false))).ToList();


            using (XLWorkbook workbook = new XLWorkbook())
            {
                IXLWorksheet worksheet = workbook.Worksheets.Add("All users’ Emails");


                int currentRowR1 = 2;
                worksheet.Range(worksheet.Cell(1, 2), worksheet.Cell(1, 7)).Merge().Value = "Finished Registration Users";
                worksheet.Range(worksheet.Cell(1, 2), worksheet.Cell(1, 7)).Style.Fill.SetBackgroundColor(XLColor.Gray);
                worksheet.Range(worksheet.Cell(1, 2), worksheet.Cell(1, 7)).Style.Font.SetBold().Font.FontSize = 14;

                worksheet.Cell(currentRowR1, 2).Value = "Displayed UserName";
                worksheet.Cell(currentRowR1, 3).Value = "Email";
                worksheet.Cell(currentRowR1, 4).Value = "Email Confirmed";
                worksheet.Cell(currentRowR1, 5).Value = "Profile Completed";
                worksheet.Cell(currentRowR1, 6).Value = "User Name";
                worksheet.Cell(currentRowR1, 7).Value = "Registration Date";

                try
                {
                    foreach (User user in finishedRegistrationUsers)
                    {
                        currentRowR1++;
                        worksheet.Cell(currentRowR1, 2).Value = user.DisplayedUserName;
                        worksheet.Cell(currentRowR1, 3).Value = user.Email;
                        worksheet.Cell(currentRowR1, 4).Value = user.EmailConfirmed ? "Confirmed" : "Not Confirmed";
                        worksheet.Cell(currentRowR1, 5).Value = (user.UserDetails.ProfileCompleted == null || user.UserDetails.ProfileCompleted.Value == false) ? "Not Completed" : "Completed";
                        worksheet.Cell(currentRowR1, 6).Value = user.UserName;
                        worksheet.Cell(currentRowR1, 7).Value = user.RegistrationDate.ToString("dd/MM/yyyy").Trim();

                    }
                }
                catch (Exception ex)
                {

                    throw;
                }

                int currentRowR2 = 2;
                worksheet.Range(worksheet.Cell(1, 10), worksheet.Cell(1, 15)).Merge().Value = "Verified Users";
                worksheet.Range(worksheet.Cell(1, 10), worksheet.Cell(1, 15)).Style.Fill.SetBackgroundColor(XLColor.Gray);
                worksheet.Range(worksheet.Cell(1, 10), worksheet.Cell(1, 15)).Style.Font.SetBold().Font.FontSize = 14;

                worksheet.Cell(currentRowR2, 10).Value = "Displayed UserName";
                worksheet.Cell(currentRowR2, 11).Value = "Email";
                worksheet.Cell(currentRowR2, 12).Value = "Email Confirmed";
                worksheet.Cell(currentRowR2, 13).Value = "Profile Completed";
                worksheet.Cell(currentRowR2, 14).Value = "User Name";
                worksheet.Cell(currentRowR2, 15).Value = "Registration Date";

                foreach (User user in verifiedUsers)
                {
                    currentRowR2++;
                    worksheet.Cell(currentRowR2, 10).Value = user.DisplayedUserName;
                    worksheet.Cell(currentRowR2, 11).Value = user.Email;
                    worksheet.Cell(currentRowR2, 12).Value = user.EmailConfirmed ? "Confirmed" : "Not Confirmed";
                    worksheet.Cell(currentRowR2, 13).Value = (user.UserDetails.ProfileCompleted == null || user.UserDetails.ProfileCompleted.Value == false) ? "Not Completed" : "Completed";
                    worksheet.Cell(currentRowR2, 14).Value = user.UserName;
                    worksheet.Cell(currentRowR2, 15).Value = user.RegistrationDate.ToString("dd/MM/yyyy").Trim();
                }


                int currentRowR3 = 2;
                worksheet.Range(worksheet.Cell(1, 18), worksheet.Cell(1, 23)).Merge().Value = "Not Verified Users";
                worksheet.Range(worksheet.Cell(1, 18), worksheet.Cell(1, 23)).Style.Fill.SetBackgroundColor(XLColor.Gray);
                worksheet.Range(worksheet.Cell(1, 18), worksheet.Cell(1, 23)).Style.Font.SetBold().Font.FontSize = 14;

                worksheet.Cell(currentRowR3, 18).Value = "Displayed UserName";
                worksheet.Cell(currentRowR3, 19).Value = "Email";
                worksheet.Cell(currentRowR3, 20).Value = "Email Confirmed";
                worksheet.Cell(currentRowR3, 21).Value = "Profile Completed";
                worksheet.Cell(currentRowR3, 22).Value = "User Name";
                worksheet.Cell(currentRowR3, 23).Value = "Registration Date";

                foreach (User user in notVerifiedUsers)
                {
                    currentRowR3++;
                    worksheet.Cell(currentRowR3, 18).Value = user.DisplayedUserName;
                    worksheet.Cell(currentRowR3, 19).Value = user.Email;
                    worksheet.Cell(currentRowR3, 20).Value = user.EmailConfirmed ? "Confirmed" : "Not Confirmed";
                    worksheet.Cell(currentRowR3, 21).Value = (user.UserDetails.ProfileCompleted == null || user.UserDetails.ProfileCompleted.Value == false) ? "Not Completed" : "Completed";
                    worksheet.Cell(currentRowR3, 22).Value = user.UserName;
                    worksheet.Cell(currentRowR3, 23).Value = user.RegistrationDate.ToString("dd/MM/yyyy").Trim();

                }


                using (MemoryStream stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    byte[] content = stream.ToArray();

                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "All users Emails.xlsx");
                }
            }
        }


        public async Task<IActionResult> ExportUsersLocationsAsExcel()
        {
            List<User> notFinishedRegistrationUsers = await authDBContext.Users.Include(q => q.UserDetails).Where(q => !q.Email.ToLower().Contains("@owner") && q.UserDetails != null && q.UserDetails.lang != null && q.UserDetails.lat != null).ToListAsync();

            using (XLWorkbook workbook = new XLWorkbook())
            {
                IXLWorksheet worksheet = workbook.Worksheets.Add("Users Locations");
                int currentRow = 2;
                worksheet.Range(worksheet.Cell(1, 2), worksheet.Cell(1, 6)).Merge().Value = "Users’ Locations";
                worksheet.Range(worksheet.Cell(1, 2), worksheet.Cell(1, 6)).Style.Fill.SetBackgroundColor(XLColor.Gray);
                worksheet.Range(worksheet.Cell(1, 2), worksheet.Cell(1, 6)).Style.Font.SetBold().Font.FontSize = 14;
                worksheet.Cell(currentRow, 2).Value = "Displayed UserName";
                worksheet.Cell(currentRow, 3).Value = "Email";
                worksheet.Cell(currentRow, 4).Value = "Latitude";
                worksheet.Cell(currentRow, 5).Value = "Longitude";
                worksheet.Cell(currentRow, 6).Value = "Zip Code";

                foreach (User user in notFinishedRegistrationUsers)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 2).Value = user.DisplayedUserName;
                    worksheet.Cell(currentRow, 3).Value = user.Email;
                    worksheet.Cell(currentRow, 4).Value = user.UserDetails.lat;
                    worksheet.Cell(currentRow, 5).Value = user.UserDetails.lang;
                    worksheet.Cell(currentRow, 6).Value = user.UserDetails.ZipCode;

                }

                using (MemoryStream stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    byte[] content = stream.ToArray();

                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Users Locations.xlsx");
                }
            }
        }


        public async Task<IActionResult> ExportAllStatisticsAsExcel()
        {
            try
            {
                using (XLWorkbook workbook = new XLWorkbook())
                {
                    var DbF = Microsoft.EntityFrameworkCore.EF.Functions;
                    var DateTimeNow = DateTime.UtcNow.Date;
                    int year = DateTime.Now.Year;
                    DateTime firstDay = new DateTime(year, 1, 1);
                    DateTime lastDay = new DateTime(year, 12, 31);
                    var monthes = Enumerable.Range(1, 12).Select(i => new { index = i, Month = DateTimeFormatInfo.CurrentInfo.GetAbbreviatedMonthName(i) });

                    List<User> users = await authDBContext.Users.Include(q => q.UserDetails).Where(q => q.Email.ToLower().Contains("@owner") == false && q.Email != "dev@dev.com" && q.UserDetails != null).AsNoTracking().ToListAsync();
                    List<Requestes> requestes = await authDBContext.Requestes.Include(q => q.User).ThenInclude(q => q.User).Where(q => q.User.Email.ToLower().Contains("@owner") == false && q.User.birthdate != null && q.User.Email != "dev@dev.com" && q.User.User.EmailConfirmed == true && (q.User.ProfileCompleted != null && q.User.ProfileCompleted == true) && q.User.listoftags.Any()).AsNoTracking().ToListAsync();
                    List<LoggedinUser> LoggedinUsers = await authDBContext.LoggedinUser.Include(q => q.User).ThenInclude(q => q.UserDetails).Where(q => q.User.Email.ToLower().Contains("@owner") == false && q.User.UserDetails.birthdate != null && q.User.Email != "dev@dev.com" && q.User.EmailConfirmed == true && (q.User.UserDetails.ProfileCompleted != null && q.User.UserDetails.ProfileCompleted == true) && q.User.UserDetails.birthdate != null && q.User.UserDetails.listoftags.Any()).ToListAsync(); List<EventData> events = await authDBContext.EventData.Include(q => q.User).Where(q => q.User.Email.ToLower().Contains("@owner") == false && q.User.User.EmailConfirmed == true && q.User.birthdate != null && q.User.listoftags.Any() && (q.User.ProfileCompleted != null && q.User.ProfileCompleted == true)).AsNoTracking().ToListAsync();
                    //List<EventTracker> eventTrackers = await authDBContext.EventTrackers.Include(q => q.User).Include(q => q.Event).Where(q => q.User.Email.ToLower().Contains("@owner") == false && q.Date.Date.Year == DateTime.Now.Year && q.User.listoftags.Any() && q.User.birthdate != null && q.User.User.EmailConfirmed == true && (q.User.ProfileCompleted != null && q.User.ProfileCompleted == true)).AsNoTracking().ToListAsync();
                    List<UserLinkClick> userLinkClicks = await authDBContext.UserLinkClicks.Include(q => q.userDetails).Where(q => q.userDetails.User.Email.ToLower().Contains("@owner") == false && q.Date.Date.Year == DateTime.Now.Year && q.userDetails.listoftags.Any() && q.userDetails.birthdate != null && q.userDetails.User.EmailConfirmed == true && (q.userDetails.ProfileCompleted != null && q.userDetails.ProfileCompleted == true)).AsNoTracking().ToListAsync();

                    #region GetInfoAboutUsers Shared Code !!

                    var UserDetails = authDBContext.UserDetails.Where(x => x.birthdate != null && x.Email.ToLower().Contains("@owner") == false && x.Email != "dev@dev.com" && (x.ProfileCompleted != null && x.ProfileCompleted == true) && x.listoftags.Any());
                    var UseresWithBirthDate = UserDetails.Select(x => DbF.DateDiffYear(x.birthdate, DateTimeNow));
                    var averageAge = Math.Round(Convert.ToDouble(UseresWithBirthDate.Sum(x => x ?? 0)) / UseresWithBirthDate.Count(), 2);
                    var usersEnableGhostMode = UserDetails.Where(x => x.ghostmode == true);
                    var UsersBirthDate = UserDetails.Where(x => x.birthdate != null).Select(x => x.birthdate);
                    var TotalUserUsedAgeFiltring = authDBContext.FilteringAccordingToAgeHistory.Select(x => x.UserID).Distinct().Count();

                    List<DeletedUser> deletedUsers = await authDBContext.DeletedUsers.ToListAsync();

                    int deletedUsersCount = deletedUsers.Where(q => !users.Select(q => q.Email).Contains(q.Email)).GroupBy(q => q.Email).Count();

                    var MaxAgeFiltringRangeUsed = authDBContext.FilteringAccordingToAgeHistory.Select(x => new { x.AgeFrom, x.AgeTo, x.UserID }).ToList().GroupBy(x => new { x.AgeFrom, x.AgeTo }).Select(x => new { Count = x.Count(), Range = x.Key.AgeFrom + " - " + x.Key.AgeTo }).OrderByDescending(x => x.Count).FirstOrDefault();

                    UserStatistics userStatistics = new UserStatistics()
                    {
                        CurrenUsers_Count = users.Count(),
                        TotalUserUsedAgeFiltring = TotalUserUsedAgeFiltring,
                        MostAgeFiltirngRangeUsed = MaxAgeFiltringRangeUsed?.Range ?? "0 - 0",
                        MostAgeFiltirngRangeUsed_Rate = Math.Round(MaxAgeFiltringRangeUsed?.Count ?? 0 / ((double)TotalUserUsedAgeFiltring), 2),
                        deactivatespushnotifications_Count = users.Where(x => x.UserDetails.pushnotification == false).Count(),
                        UseresAverageAge = averageAge,
                        TotalInAppearanceTypes = authDBContext.AppearanceTypes_UserDetails.Count(),
                        AppearenceEveryOneInGhostMode_Count = authDBContext.AppearanceTypes_UserDetails.Where(x => x.AppearanceTypeID == 1).Count(),
                        AppearenceMaleInGhostMode_Count = authDBContext.AppearanceTypes_UserDetails.Where(x => x.AppearanceTypeID == 2).Count(),
                        AppearenceFemaleInGhostMode_Count = authDBContext.AppearanceTypes_UserDetails.Where(x => x.AppearanceTypeID == 3).Count(),
                        AppearenceOtherGenderInGhostMode_Count = authDBContext.AppearanceTypes_UserDetails.Where(x => x.AppearanceTypeID == 4).Count(),
                        RequestesCount = requestes.Count(),
                        UserWithLessThan18Age_Count = UsersBirthDate.Count(x => DbF.DateDiffYear(x, DateTimeNow) < 18),
                        UsersWith18_24Age_Count = UsersBirthDate.Count(x => DbF.DateDiffYear(x, DateTimeNow) >= 18 && DbF.DateDiffYear(x, DateTimeNow) <= 24),
                        UsersWith25_34Age_Count = UsersBirthDate.Count(x => DbF.DateDiffYear(x, DateTimeNow) >= 25 && DbF.DateDiffYear(x, DateTimeNow) <= 34),
                        UsersWith35_54Age_Count = UsersBirthDate.Count(x => DbF.DateDiffYear(x, DateTimeNow) >= 35 && DbF.DateDiffYear(x, DateTimeNow) <= 54),
                        UsersWithMoreThan55Age_Count = UsersBirthDate.Count(x => DbF.DateDiffYear(x, DateTimeNow) >= 55),
                        BlockRequestesCount = requestes.Where(x => x.status == 3).Count(),
                        AcceptedRequestesCount = requestes.Where(x => x.status == 2).Count(),
                        PendindingRequestesCount = requestes.Where(x => x.status == 1).Count(),
                        UseresStillUseAppAfter3Months_Count = UserDetails.Where(x => DbF.DateDiffMonth(x.User.RegistrationDate, x.User.LoggedinUser.Max(q => q.ExpiredOn).Date) >= 3).Count(),
                        ActiveUsers_Count = LoggedinUsers.Where(q => q.ExpiredOn > DateTime.Now).Count(),
                        UseresEnableGhostMode_Count = usersEnableGhostMode.Count(),
                        ConfirmedMailUsers_Count = users.Where(x => x.EmailConfirmed).Count(),
                        DeletedUsers_Count = deletedUsersCount, //authDBContext.DeletedUsersLogs.Count(),
                        UnConfirmedMailUsers_Count = users.Where(x => !x.EmailConfirmed).Count(),
                        Male_Count = UserDetails.Where(x => x.Gender == "male").Count(),
                        Female_Count = UserDetails.Where(x => x.Gender == "female").Count(),
                        NeedUpdate = ((users.Count()) - (UserDetails.Where(n => n.birthdate != null && n.listoftags.Any()).Count())),
                        Updated = UserDetails.Where(n => n.birthdate != null && n.listoftags.Any()).Count(),
                        personalspace = UserDetails.Where(n => n.personalSpace == true).Count(),
                        DeletedProfiles_Count = deletedUsersCount,
                        PushNotificationsEnabled_Count = UserDetails.Where(n => n.pushnotification == true).Count(),
                    };
                    #endregion

                    #region  Export Link Click Statistics (9)

                    AddNewWorkbookForStatisticsPerMonthAndDay(workbook, ExportLinkClickStatistics(userLinkClicks, LinkClickTypeEnum.Share), "Share Clicks Number", "Share Link By Age", "Share Link By Gender");
                    AddNewWorkbookForStatisticsPerMonthAndDay(workbook, ExportLinkClickStatistics(userLinkClicks, LinkClickTypeEnum.Help), "Help Clicks Number", "Help Link By Age", "Help Link By Gender");
                    AddNewWorkbookForStatisticsPerMonthAndDay(workbook, ExportLinkClickStatistics(userLinkClicks, LinkClickTypeEnum.TipsAndGuidance), "T&G Clicks Number", "Tips & Guidance Link By Age", "Tips & Guidance Link By Gender");
                    AddNewWorkbookForStatisticsPerMonthAndDay(workbook, ExportLinkClickStatistics(userLinkClicks, LinkClickTypeEnum.AboutUs), "About Us Clicks Number", "About Us Link By Age", "About Us Link By Gender");
                    AddNewWorkbookForStatisticsPerMonthAndDay(workbook, ExportLinkClickStatistics(userLinkClicks, LinkClickTypeEnum.TermsAndConditions), "T&C Clicks Number", "Terms & Conditions Link By Age", "Terms & Conditions Link By Gender");
                    AddNewWorkbookForStatisticsPerMonthAndDay(workbook, ExportLinkClickStatistics(userLinkClicks, LinkClickTypeEnum.PrivacyPolicy), "Privacy Policy Clicks Number", "Privacy Policy Link By Age", "Privacy Policy Link By Gender");
                    AddNewWorkbookForStatisticsPerMonthAndDay(workbook, ExportLinkClickStatistics(userLinkClicks, LinkClickTypeEnum.SkipTutorial), "Skip Tutorial Clicks Number", "Skip Tutorial Link By Age", "Skip Tutorial Link By Gender");
                    AddNewWorkbookForStatisticsPerMonthAndDay(workbook, ExportLinkClickStatistics(userLinkClicks, LinkClickTypeEnum.SupportRequest), "Support Request Clicks Number", "Support Request Link By Age", "Support Request Link By Gender");
                    AddNewWorkbookForStatisticsPerMonthAndDay(workbook, ExportLinkClickStatistics(userLinkClicks, LinkClickTypeEnum.SortByInterestMatch), "Feeds Interest Match Number", "Feeds Interest Match By Age", "Feeds Interest Match By Gender");

                    #endregion

                    #region User Home Statistics (1)

                    IXLWorksheet userHomeStatisticsWorksheet = workbook.Worksheets.Add("Users Home Statistics");

                    userHomeStatisticsWorksheet.Range(userHomeStatisticsWorksheet.Cell(1, 2), userHomeStatisticsWorksheet.Cell(1, 15)).Merge().Value = "User Home Statistics";
                    userHomeStatisticsWorksheet.Range(userHomeStatisticsWorksheet.Cell(1, 2), userHomeStatisticsWorksheet.Cell(1, 15)).Style.Fill.SetBackgroundColor(XLColor.Gray);
                    userHomeStatisticsWorksheet.Range(userHomeStatisticsWorksheet.Cell(1, 2), userHomeStatisticsWorksheet.Cell(1, 15)).Style.Font.SetBold().Font.FontSize = 14;
                    userHomeStatisticsWorksheet.Cell(2, 2).Value = "Registered users";
                    userHomeStatisticsWorksheet.Cell(2, 3).Value = "Deleted Accounts";
                    userHomeStatisticsWorksheet.Cell(2, 4).Value = "Inactive users";
                    userHomeStatisticsWorksheet.Cell(2, 5).Value = "Verified emails";
                    userHomeStatisticsWorksheet.Cell(2, 6).Value = "Unverified emails";
                    userHomeStatisticsWorksheet.Cell(2, 7).Value = "Users still active after 3 months";
                    userHomeStatisticsWorksheet.Cell(2, 8).Value = "Users with private mode enabled";
                    userHomeStatisticsWorksheet.Cell(2, 9).Value = "Users who have disabled push notifications";
                    userHomeStatisticsWorksheet.Cell(2, 10).Value = "Average age of users";
                    userHomeStatisticsWorksheet.Cell(2, 11).Value = "Number of friend requests";
                    userHomeStatisticsWorksheet.Cell(2, 12).Value = "Number of blocks";
                    userHomeStatisticsWorksheet.Cell(2, 13).Value = "Completed  profile data";
                    userHomeStatisticsWorksheet.Cell(2, 14).Value = "Not completed  profile data";
                    userHomeStatisticsWorksheet.Cell(2, 15).Value = "Users with personal space enabled";

                    userHomeStatisticsWorksheet.Cell(3, 2).Value = userStatistics.TotalUsers_Count;
                    userHomeStatisticsWorksheet.Cell(3, 3).Value = userStatistics.DeletedUsers_Count;
                    userHomeStatisticsWorksheet.Cell(3, 4).Value = userStatistics.NotactiveUsers_Count;
                    userHomeStatisticsWorksheet.Cell(3, 5).Value = userStatistics.ConfirmedMailUsers_Count;
                    userHomeStatisticsWorksheet.Cell(3, 6).Value = userStatistics.UnConfirmedMailUsers_Count;
                    userHomeStatisticsWorksheet.Cell(3, 7).Value = userStatistics.UseresStillUseAppAfter3Months_Count;
                    userHomeStatisticsWorksheet.Cell(3, 8).Value = userStatistics.UseresEnableGhostMode_Count;
                    userHomeStatisticsWorksheet.Cell(3, 9).Value = userStatistics.deactivatespushnotifications_Count;
                    userHomeStatisticsWorksheet.Cell(3, 10).Value = userStatistics.UseresAverageAge;
                    userHomeStatisticsWorksheet.Cell(3, 11).Value = userStatistics.RequestesCount;
                    userHomeStatisticsWorksheet.Cell(3, 12).Value = userStatistics.BlockRequestesCount;
                    userHomeStatisticsWorksheet.Cell(3, 13).Value = userStatistics.Updated;
                    userHomeStatisticsWorksheet.Cell(3, 14).Value = userStatistics.NeedUpdate;
                    userHomeStatisticsWorksheet.Cell(3, 15).Value = userStatistics.personalspace;

                    #endregion 

                    #region Total Event Statistics (2)

                    var allEvents = authDBContext.EventData.Where(n => n.IsActive == true).ToList();
                    var allEventAttends = authDBContext.EventChatAttend.ToList();
                    var sharedevent = authDBContext.EventTrackers.Where(m => m.ActionType == EventActionType.Share.ToString()).Select(m => m.EventId).Distinct();
                    var averageOfParticibated = Math.Round(Convert.ToDouble(allEventAttends.GroupBy(x => x.EventDataid).Select(x => x.Count()).Sum()) / allEvents.Count(), 2);
                    var averageOfParticibatedInExisteEvent = Math.Round(Convert.ToDouble(allEventAttends.Where(q => q.EventData.eventdateto < DateTime.Now).GroupBy(x => x.EventDataid).Select(x => x.Count()).Sum()) / allEvents.Count(), 2);

                    try
                    { var LAWcapacityK = allEventAttends.GroupBy(x => x.EventDataid).Count(); }
                    catch (Exception EX)
                    {
                        var T = EX;
                    }
                    var COUNTLIST = allEventAttends.GroupBy(x => x.EventDataid).Select(x => new { COUNT = x.Count(), totalnumbert = x.FirstOrDefault().EventData.totalnumbert }).ToList();

                    var LAWcapacityVV = COUNTLIST.Select(N => (N.COUNT * 100 / N.totalnumbert)).ToList();
                    var LAWcapacity = COUNTLIST.Where(N => (N.COUNT * 100 / N.totalnumbert) <= 40).Count();
                    var mediumcapacity = COUNTLIST.Where(N => (N.COUNT * 100 / N.totalnumbert) > 40 && (N.COUNT * 100 / N.totalnumbert) <= 75).Count();
                    var highcapacity = COUNTLIST.Where(N => (N.COUNT * 100 / N.totalnumbert) > 75 && (N.COUNT * 100 / N.totalnumbert) <= 100).Count();
                    var eventsInfo = new
                    {
                        AllEventsCount = allEvents.Count(),
                        NumberOfExisteEvent = allEvents.Where(q => q.eventdateto.Value.Date > DateTime.Now.Date || (q.eventdateto.Value.Date == DateTime.Now.Date && q.eventto > DateTime.Now.TimeOfDay) || (q.allday == true && q.eventdate.Value.Date == DateTime.Now.Date)).Count(),
                        NumberOfFinishedEvent = allEvents.Where(q => q.eventdateto.Value.Date < DateTime.Now.Date || (q.eventdateto.Value.Date == DateTime.Now.Date && q.eventto <= DateTime.Now.TimeOfDay) || (q.allday == true && q.eventdate.Value.Date <= DateTime.Now.Date)).Count(),
                        NumberOfPrivateEvent = allEvents.Where(q => q.EventTypeListid == 1).Count(),
                        NumberOfFriendzrEvent = allEvents.Where(q => q.EventTypeListid == 2).Count(),
                        NumberOfExternalEvent = allEvents.Where(q => q.EventTypeListid == 3).Count(),
                        NumberOfadminExternalEvent = allEvents.Where(q => q.EventTypeListid == 4).Count(),
                        NumberOfWhiteLableEvent = allEvents.Where(q => q.EventTypeListid == 5).Count(),
                        AverageOfParticibated = averageOfParticibated,
                        Eventsafter3months = allEvents.Where(x => DbF.DateDiffMonth(x.eventdate, DateTimeNow) >= 3).Count(),
                        NumberOfDeletedEvents = authDBContext.DeletedEventLogs.Count(),
                        NumberOfUseresHowCreatedEvents = allEvents.GroupBy(x => x.UserId).Select(x => x.Key).Count(),
                        averageOfParticibatedInExisteEvent = averageOfParticibatedInExisteEvent,
                        LOWcapacity = LAWcapacity,
                        Mediumcapacity = mediumcapacity,
                        Highcapacity = highcapacity,
                        Sharedevent = sharedevent.Count(),
                        Attendes = allEventAttends.Where(v => v.EventData.EventTypeListid == 3 && v.ISAdmin != true).Count(),
                    };

                    IXLWorksheet totalEventStatisticsWorksheet = workbook.Worksheets.Add("Total Event Home Statistics");

                    totalEventStatisticsWorksheet.Range(totalEventStatisticsWorksheet.Cell(1, 2), totalEventStatisticsWorksheet.Cell(1, 16)).Merge().Value = "Total Event Home Statistics";
                    totalEventStatisticsWorksheet.Range(totalEventStatisticsWorksheet.Cell(1, 2), totalEventStatisticsWorksheet.Cell(1, 16)).Style.Fill.SetBackgroundColor(XLColor.Gray);
                    totalEventStatisticsWorksheet.Range(totalEventStatisticsWorksheet.Cell(1, 2), totalEventStatisticsWorksheet.Cell(1, 16)).Style.Font.SetBold().Font.FontSize = 14;
                    totalEventStatisticsWorksheet.Cell(2, 2).Value = "Total number of events created";
                    totalEventStatisticsWorksheet.Cell(2, 3).Value = "Number of existing events";
                    totalEventStatisticsWorksheet.Cell(2, 4).Value = "Number Of Finished Events";
                    totalEventStatisticsWorksheet.Cell(2, 5).Value = "Number Of Private Events";
                    totalEventStatisticsWorksheet.Cell(2, 6).Value = "Number Of Friendzr Events";
                    totalEventStatisticsWorksheet.Cell(2, 7).Value = "Number of external events";
                    totalEventStatisticsWorksheet.Cell(2, 8).Value = "Number Of Admin External Events";
                    totalEventStatisticsWorksheet.Cell(2, 9).Value = "Number Of WhiteLable Events";
                    totalEventStatisticsWorksheet.Cell(2, 10).Value = "Number Of Deleted Events";
                    totalEventStatisticsWorksheet.Cell(2, 11).Value = "Users Who Have Created an Event";
                    totalEventStatisticsWorksheet.Cell(2, 12).Value = "Percentage of events participants";
                    totalEventStatisticsWorksheet.Cell(2, 13).Value = "Percentage of Existe events participants";
                    totalEventStatisticsWorksheet.Cell(2, 14).Value = "Created Events After 3 Months";
                    //totalEventStatisticsWorksheet.Cell(2, 15).Value = "Low Capacity Events";
                    //totalEventStatisticsWorksheet.Cell(2, 16).Value = "Medium Capacity Events";
                    //totalEventStatisticsWorksheet.Cell(2, 17).Value = "High Capacity Events";
                    totalEventStatisticsWorksheet.Cell(2, 15).Value = "Number Of Shared Events";
                    totalEventStatisticsWorksheet.Cell(2, 16).Value = "Total External Events Attendance";

                    totalEventStatisticsWorksheet.Cell(3, 2).Value = eventsInfo.AllEventsCount;
                    totalEventStatisticsWorksheet.Cell(3, 3).Value = eventsInfo.NumberOfExisteEvent;
                    totalEventStatisticsWorksheet.Cell(3, 4).Value = eventsInfo.NumberOfFinishedEvent;
                    totalEventStatisticsWorksheet.Cell(3, 5).Value = eventsInfo.NumberOfPrivateEvent;
                    totalEventStatisticsWorksheet.Cell(3, 6).Value = eventsInfo.NumberOfFriendzrEvent;
                    totalEventStatisticsWorksheet.Cell(3, 7).Value = eventsInfo.NumberOfExternalEvent;
                    totalEventStatisticsWorksheet.Cell(3, 8).Value = eventsInfo.NumberOfadminExternalEvent;
                    totalEventStatisticsWorksheet.Cell(3, 9).Value = eventsInfo.NumberOfWhiteLableEvent;
                    totalEventStatisticsWorksheet.Cell(3, 10).Value = eventsInfo.NumberOfDeletedEvents;
                    totalEventStatisticsWorksheet.Cell(3, 11).Value = eventsInfo.NumberOfUseresHowCreatedEvents;
                    totalEventStatisticsWorksheet.Cell(3, 12).Value = eventsInfo.AverageOfParticibated;
                    totalEventStatisticsWorksheet.Cell(3, 13).Value = eventsInfo.averageOfParticibatedInExisteEvent;
                    totalEventStatisticsWorksheet.Cell(3, 14).Value = eventsInfo.Eventsafter3months;
                    //totalEventStatisticsWorksheet.Cell(3, 15).Value = eventsInfo.LOWcapacity;
                    //totalEventStatisticsWorksheet.Cell(3, 16).Value = eventsInfo.Mediumcapacity;
                    //totalEventStatisticsWorksheet.Cell(3, 17).Value = eventsInfo.Highcapacity;
                    totalEventStatisticsWorksheet.Cell(3, 15).Value = eventsInfo.Sharedevent;
                    totalEventStatisticsWorksheet.Cell(3, 16).Value = eventsInfo.Attendes;

                    #endregion

                    #region App Statistics (3)

                    var distancelist = distanceFilterHistoryService.GetAll(DateTime.Now.Year).ToList();
                    var alldistanceFilterHistory = distancelist.GroupBy(x => x.Month).Select(x => new { Month = x.Key, Count = x.Count() }).OrderBy(x => x.Month).ToList();

                    int distsum = distancelist.Count() == 0 ? 1 : distancelist.Count();
                    var distance = distancelist.GroupBy(x => x.Month).Select(x => new { Month = x.Key, Count = (int)(x.Select(m => m.destance).Sum() / distsum) }).OrderBy(x => x.Month).ToList();

                    var appStatistics = monthes.Select(x => new
                    {
                        Month = x.Month,
                        DistanceFilter = alldistanceFilterHistory.FirstOrDefault(xx => x.index == xx.Month)?.Count ?? 0,
                        AverageRangeOfDistance = distance.FirstOrDefault(xx => x.index == xx.Month)?.Count ?? 0
                    }).ToList();

                    IXLWorksheet appStatisticsWorksheet = workbook.Worksheets.Add("App Statistics");

                    int appStatisticsCurrentRow = 2;
                    appStatisticsWorksheet.Range(appStatisticsWorksheet.Cell(1, 2), appStatisticsWorksheet.Cell(1, 4)).Merge().Value = "App Statistics";
                    appStatisticsWorksheet.Range(appStatisticsWorksheet.Cell(1, 2), appStatisticsWorksheet.Cell(1, 4)).Style.Fill.SetBackgroundColor(XLColor.Gray);
                    appStatisticsWorksheet.Range(appStatisticsWorksheet.Cell(1, 2), appStatisticsWorksheet.Cell(1, 4)).Style.Font.SetBold().Font.FontSize = 14;
                    appStatisticsWorksheet.Cell(appStatisticsCurrentRow, 2).Value = "Category";
                    appStatisticsWorksheet.Cell(appStatisticsCurrentRow, 3).Value = "Distance Filter";
                    appStatisticsWorksheet.Cell(appStatisticsCurrentRow, 4).Value = "Average range of distance";

                    foreach (var monthStatistic in appStatistics)
                    {
                        appStatisticsCurrentRow++;
                        appStatisticsWorksheet.Cell(appStatisticsCurrentRow, 2).Value = monthStatistic.Month;
                        appStatisticsWorksheet.Cell(appStatisticsCurrentRow, 3).Value = monthStatistic.DistanceFilter;
                        appStatisticsWorksheet.Cell(appStatisticsCurrentRow, 4).Value = monthStatistic.AverageRangeOfDistance;
                    }

                    #endregion

                    #region Users Statistics Chart (4) per Month and per Day

                    var allUsersInYear = authDBContext.Users.Include(u=>u.UserDetails).Where(x => x.Email.ToLower().Contains("@owner") == false && x.Email != "dev@dev.com").Where(x => DbF.DateDiffDay(firstDay, x.RegistrationDate) >= 0 && DbF.DateDiffDay(x.RegistrationDate, lastDay) >= 0);
                    var allRequestesInYear = authDBContext.Requestes.Where(x => x.User.listoftags.Any() && x.User.User.EmailConfirmed == true && (x.User.ProfileCompleted != null && x.User.ProfileCompleted == true) && DbF.DateDiffDay(firstDay, x.regestdata) >= 0 && DbF.DateDiffDay(x.regestdata, lastDay) >= 0);
                    var registeredUsers = allUsersInYear.GroupBy(x => x.RegistrationDate.Date.Month).Select(x => new
                    {
                        Month = x.Key,
                        Count = x.Count()
                    }).OrderBy(x => x.Month).ToList();
                    // Active Users
                    var activeUsers = LoggedinUsers.Where(u=>u.User.UserDetails.IsActive && u.ExpiredOn > DateTimeNow).GroupBy(x => x.ExpiredOn.Date.Month).Select(x => new
                    {
                        Month = x.Key,
                        Count = x.Count()
                    }).OrderBy(x => x.Month).ToList();
                    var ConfirmedMailUseresPerMonth = allUsersInYear.Where(x => x.EmailConfirmed == true).GroupBy(x => x.RegistrationDate.Date.Month).Select(x => new
                    {
                        Month = x.Key,
                        Count = x.Count()
                    }).OrderBy(x => x.Month).ToList();
                    //var GostModeUseresPerMonth = allUsersInYear.Where(x => x.UserDetails != null && x.UserDetails.birthdate != null && x.UserDetails.listoftags.Any() && x.EmailConfirmed == true && (x.UserDetails.ProfileCompleted != null && x.UserDetails.ProfileCompleted == true) && x.UserDetails.ghostmode == true).GroupBy(x => x.RegistrationDate.Date.Month).Select(x => new
                    //{
                    //    Month = x.Key,
                    //    Count = x.Count()
                    //}).OrderBy(x => x.Month).ToList();
                    //var GostModeMenUseresPerMonth = allUsersInYear.Where(x => x.UserDetails != null && x.UserDetails.birthdate != null && x.UserDetails.listoftags.Any() && x.EmailConfirmed == true && (x.UserDetails.ProfileCompleted != null && x.UserDetails.ProfileCompleted == true) && x.UserDetails.ghostmode == true && (x.UserDetails.Gender != null && x.UserDetails.Gender == "male")).GroupBy(x => x.RegistrationDate.Date.Month).Select(x => new
                    //{
                    //    Month = x.Key,
                    //    Count = x.Count()
                    //}).OrderBy(x => x.Month).ToList();
                    //var GostModeWomenUseresPerMonth = allUsersInYear.Where(x => x.UserDetails != null && x.UserDetails.birthdate != null && x.UserDetails.listoftags.Any() && x.EmailConfirmed == true && (x.UserDetails.ProfileCompleted != null && x.UserDetails.ProfileCompleted == true) && x.UserDetails.ghostmode == true && (x.UserDetails.Gender != null && x.UserDetails.Gender == "female")).GroupBy(x => x.RegistrationDate.Date.Month).Select(x => new
                    //{
                    //    Month = x.Key,
                    //    Count = x.Count()
                    //}).OrderBy(x => x.Month).ToList();
                    //var GostModeOtherUseresPerMonth = allUsersInYear.Where(x => x.UserDetails != null && x.UserDetails.birthdate != null && x.UserDetails.listoftags.Any() && x.EmailConfirmed == true && (x.UserDetails.ProfileCompleted != null && x.UserDetails.ProfileCompleted == true) && x.UserDetails.ghostmode == true && (x.UserDetails.Gender != null && x.UserDetails.Gender == "other")).GroupBy(x => x.RegistrationDate.Date.Month).Select(x => new
                    //{
                    //    Month = x.Key,
                    //    Count = x.Count()
                    //}).OrderBy(x => x.Month).ToList();
                    //var GostModeEveryoneUseresPerMonth = allUsersInYear.Where(x => x.UserDetails != null && x.UserDetails.birthdate != null && x.UserDetails.listoftags.Any() && x.EmailConfirmed == true && (x.UserDetails.ProfileCompleted != null && x.UserDetails.ProfileCompleted == true) && x.UserDetails.ghostmode == true).GroupBy(x => x.RegistrationDate.Date.Month).Select(x => new
                    //{
                    //    Month = x.Key,
                    //    Count = x.Count()
                    //}).OrderBy(x => x.Month).ToList();
                    var disablePushNotficationPerMonth = allUsersInYear.Where(x => x.UserDetails != null && x.UserDetails.birthdate != null && x.UserDetails.listoftags.Any() && x.EmailConfirmed == true && (x.UserDetails.ProfileCompleted != null && x.UserDetails.ProfileCompleted == true) && x.UserDetails.pushnotification == false).GroupBy(x => x.RegistrationDate.Date.Month).Select(x => new
                    {
                        Month = x.Key,
                        Count = x.Count()
                    }).OrderBy(x => x.Month).ToList();
                    var NotConfirmedMailUseresPerMonth = allUsersInYear.Where(x => x.EmailConfirmed == false).GroupBy(x => x.RegistrationDate.Date.Month).Select(x => new
                    {
                        Month = x.Key,
                        Count = x.Count()
                    }).OrderBy(x => x.Month).ToList();
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

                    var DeactiveUsers = authDBContext.DeletedUsers.Where(q => !allUsersInYear.Select(q => q.Email).Contains(q.Email)).ToList().GroupBy(q => q.Email).Where(x => DbF.DateDiffDay(firstDay, x.FirstOrDefault().Date) >= 0 && DbF.DateDiffDay(x.FirstOrDefault().Date, lastDay) >= 0).GroupBy(x => x.FirstOrDefault().Date.Date.Month).Select(x => new { Month = x.Key, Count = x.Count() }).OrderBy(x => x.Month);

                    var usersStatistics = monthes.Select(x => new
                    {
                        Month = x.Month,
                        Registered = registeredUsers.FirstOrDefault(xx => x.index == xx.Month)?.Count ?? 0,
                        DeactiveUsers = DeactiveUsers.FirstOrDefault(xx => x.index == xx.Month)?.Count ?? 0,
                        ConfirmedMailUseresPerMonth = ConfirmedMailUseresPerMonth.FirstOrDefault(xx => x.index == xx.Month)?.Count ?? 0,
                        NotConfirmedMailUseresPerMonth = NotConfirmedMailUseresPerMonth.FirstOrDefault(xx => x.index == xx.Month)?.Count ?? 0,
                        //GostModeUseresPerMonth = GostModeUseresPerMonth.FirstOrDefault(xx => x.index == xx.Month)?.Count ?? 0,
                        //GostModeMenUseresPerMonth = GostModeUseresPerMonth.FirstOrDefault(xx => x.index == xx.Month)?.Count ?? 0,
                        //GostModeWomenUseresPerMonth = GostModeUseresPerMonth.FirstOrDefault(xx => x.index == xx.Month)?.Count ?? 0,
                        //GostModeOtherUseresPerMonth = GostModeUseresPerMonth.FirstOrDefault(xx => x.index == xx.Month)?.Count ?? 0,
                        //GostModeEveryoneUseresPerMonth = GostModeUseresPerMonth.FirstOrDefault(xx => x.index == xx.Month)?.Count ?? 0,
                        disablePushNotficationPerMonth = disablePushNotficationPerMonth.FirstOrDefault(xx => x.index == xx.Month)?.Count ?? 0,
                        NumberOfRequestesPerMonth = NumberOfRequestesPerMonth.FirstOrDefault(xx => x.index == xx.Month)?.Count ?? 0,
                        NumberOfBlockRequestesPerMonth = NumberOfBlockRequestesPerMonth.FirstOrDefault(xx => x.index == xx.Month)?.Count ?? 0,
                        ActiveUsers = activeUsers.FirstOrDefault(u => x.index == u.Month)?.Count ?? 0
                    }).ToList();

                    IXLWorksheet usersStatisticsWorksheet = workbook.Worksheets.Add("Users Statistics");
                    // per Month
                    int usersStatisticsCurrentRow = 2;
                    usersStatisticsWorksheet.Range(usersStatisticsWorksheet.Cell(1, 2), usersStatisticsWorksheet.Cell(1, 10)).Merge().Value = "Users Statistics Per Month";
                    usersStatisticsWorksheet.Range(usersStatisticsWorksheet.Cell(1, 2), usersStatisticsWorksheet.Cell(1, 10)).Style.Fill.SetBackgroundColor(XLColor.Gray);
                    usersStatisticsWorksheet.Range(usersStatisticsWorksheet.Cell(1, 2), usersStatisticsWorksheet.Cell(1, 10)).Style.Font.SetBold().Font.FontSize = 14;
                    usersStatisticsWorksheet.Cell(usersStatisticsCurrentRow, 2).Value = "Category";
                    usersStatisticsWorksheet.Cell(usersStatisticsCurrentRow, 3).Value = "Registered";
                    usersStatisticsWorksheet.Cell(usersStatisticsCurrentRow, 4).Value = "Deleted";
                    usersStatisticsWorksheet.Cell(usersStatisticsCurrentRow, 5).Value = "Verified email";
                    usersStatisticsWorksheet.Cell(usersStatisticsCurrentRow, 6).Value = "Unverified email";
                    //usersStatisticsWorksheet.Cell(usersStatisticsCurrentRow, 7).Value = "Private mode";
                    //usersStatisticsWorksheet.Cell(usersStatisticsCurrentRow, 8).Value = "Hide From Men";
                    //usersStatisticsWorksheet.Cell(usersStatisticsCurrentRow, 9).Value = "Hide From Women";
                    //usersStatisticsWorksheet.Cell(usersStatisticsCurrentRow, 10).Value = "Hide From Other";
                    //usersStatisticsWorksheet.Cell(usersStatisticsCurrentRow, 11).Value = "Hide From Everyone";
                    usersStatisticsWorksheet.Cell(usersStatisticsCurrentRow, 7).Value = "Disable Push Notification";
                    usersStatisticsWorksheet.Cell(usersStatisticsCurrentRow, 8).Value = "Connection requests";
                    usersStatisticsWorksheet.Cell(usersStatisticsCurrentRow, 9).Value = "Blocks";
                    usersStatisticsWorksheet.Cell(usersStatisticsCurrentRow, 10).Value = "Active Users";

                    foreach (var monthStatistic in usersStatistics)
                    {
                        usersStatisticsCurrentRow++;
                        usersStatisticsWorksheet.Cell(usersStatisticsCurrentRow, 2).Value = monthStatistic.Month;
                        usersStatisticsWorksheet.Cell(usersStatisticsCurrentRow, 3).Value = monthStatistic.Registered;
                        usersStatisticsWorksheet.Cell(usersStatisticsCurrentRow, 4).Value = monthStatistic.DeactiveUsers;
                        usersStatisticsWorksheet.Cell(usersStatisticsCurrentRow, 5).Value = monthStatistic.ConfirmedMailUseresPerMonth;
                        usersStatisticsWorksheet.Cell(usersStatisticsCurrentRow, 6).Value = monthStatistic.NotConfirmedMailUseresPerMonth;
                        //usersStatisticsWorksheet.Cell(usersStatisticsCurrentRow, 7).Value = monthStatistic.GostModeUseresPerMonth;
                        //usersStatisticsWorksheet.Cell(usersStatisticsCurrentRow, 8).Value = monthStatistic.GostModeMenUseresPerMonth;
                        //usersStatisticsWorksheet.Cell(usersStatisticsCurrentRow, 9).Value = monthStatistic.GostModeWomenUseresPerMonth;
                        //usersStatisticsWorksheet.Cell(usersStatisticsCurrentRow, 10).Value = monthStatistic.GostModeOtherUseresPerMonth;
                        //usersStatisticsWorksheet.Cell(usersStatisticsCurrentRow, 11).Value = monthStatistic.GostModeEveryoneUseresPerMonth;
                        usersStatisticsWorksheet.Cell(usersStatisticsCurrentRow, 7).Value = monthStatistic.disablePushNotficationPerMonth;
                        usersStatisticsWorksheet.Cell(usersStatisticsCurrentRow, 8).Value = monthStatistic.NumberOfRequestesPerMonth;
                        usersStatisticsWorksheet.Cell(usersStatisticsCurrentRow, 9).Value = monthStatistic.NumberOfBlockRequestesPerMonth;
                        usersStatisticsWorksheet.Cell(usersStatisticsCurrentRow, 10).Value = monthStatistic.ActiveUsers;
                    }
                    #region Export per day
                      usersStatisticsCurrentRow = 2;
                        usersStatisticsWorksheet.Range(usersStatisticsWorksheet.Cell(1, 12), usersStatisticsWorksheet.Cell(1, 15)).Merge().Value = "Users Statistics Per Day";
                        usersStatisticsWorksheet.Range(usersStatisticsWorksheet.Cell(1, 12), usersStatisticsWorksheet.Cell(1, 15)).Style.Fill.SetBackgroundColor(XLColor.Gray);
                        usersStatisticsWorksheet.Range(usersStatisticsWorksheet.Cell(1, 12), usersStatisticsWorksheet.Cell(1, 15)).Style.Font.SetBold().Font.FontSize = 14;
                        usersStatisticsWorksheet.Cell(usersStatisticsCurrentRow, 12).Value = "Category";
                        usersStatisticsWorksheet.Cell(usersStatisticsCurrentRow, 13).Value = "Registered";
                        usersStatisticsWorksheet.Cell(usersStatisticsCurrentRow, 14).Value = "Deleted";                        
                        usersStatisticsWorksheet.Cell(usersStatisticsCurrentRow, 15).Value = "Active Users";
                    
                 
                    foreach (var month in monthes)
                    {
                        var daysOfMonth = Enumerable.Range(1, DateTime.DaysInMonth(DateTime.Now.Year, month.index)).Select(d => new DateTime(DateTime.Now.Year, month.index, d));
                        var selectedUsers = daysOfMonth.Select(d => new 
                        {
                            Date = d.Date.ToString("dd/MM/yyyy").Trim(),
                            Registered = allUsersInYear.Where(n => d.Day == n.RegistrationDate.Day && n.RegistrationDate.Month == month.index).Count(),
                            DeactiveUsers = authDBContext.DeletedUsers.Where(q => !allUsersInYear.Select(q => q.Email).Contains(q.Email)).ToList().Where(u=>u.Date.Day== d.Day && u.Date.Month== month.index).Count(),
                          
                            ActiveUsers = allUsersInYear.Where(u => (u.UserDetails.IsActive && u.RegistrationDate.Day == d.Day && u.RegistrationDate.Month == month.index) ).Count()

                        }).ToList();
                        foreach (var user in selectedUsers)
                        {
                            usersStatisticsCurrentRow++;
                            usersStatisticsWorksheet.Cell(usersStatisticsCurrentRow, 12).Value = user.Date;
                            usersStatisticsWorksheet.Cell(usersStatisticsCurrentRow, 13).Value = user.Registered;
                            usersStatisticsWorksheet.Cell(usersStatisticsCurrentRow, 14).Value = user.DeactiveUsers;                            
                            usersStatisticsWorksheet.Cell(usersStatisticsCurrentRow, 15).Value = user.ActiveUsers;
                        }
                    }
                        
                    #endregion
                    #endregion

                    #region Event Statistics (5)

                    var allEventsInYear = authDBContext.EventData.Where(x => DbF.DateDiffDay(firstDay, x.CreatedDate) >= 0 && DbF.DateDiffDay(x.CreatedDate, lastDay) >= 0);

                    var AllEventPerMonth = allEventsInYear.GroupBy(x => x.CreatedDate.Value.Date.Month).Select(x => new
                    {
                        Month = x.Key,
                        Count = x.Count()
                    }).OrderBy(x => x.Month).ToList();

                    var PrivateEventPerMonth = allEventsInYear.Where(x => x.EventTypeListid == 1).GroupBy(x => x.CreatedDate.Value.Date.Month).Select(x => new
                    {
                        Month = x.Key,
                        Count = x.Count()
                    }).OrderBy(x => x.Month).ToList();
                    var FriendzrEventPerMonth = allEventsInYear.Where(x => x.EventTypeListid == 2).GroupBy(x => x.CreatedDate.Value.Date.Month).Select(x => new
                    {
                        Month = x.Key,
                        Count = x.Count()
                    }).OrderBy(x => x.Month).ToList();
                    var ExternalEventPerMonth = allEventsInYear.Where(x => x.EventTypeListid == 3).GroupBy(x => x.CreatedDate.Value.Date.Month).Select(x => new
                    {
                        Month = x.Key,
                        Count = x.Count()
                    }).OrderBy(x => x.Month).ToList();
                    var AdminExternalEventPerMonth = allEventsInYear.Where(x => x.EventTypeListid == 4).GroupBy(x => x.CreatedDate.Value.Date.Month).Select(x => new
                    {
                        Month = x.Key,
                        Count = x.Count()
                    }).OrderBy(x => x.Month).ToList();
                    var WhiteLableEventPerMonth = allEventsInYear.Where(x => x.EventTypeListid == 5).GroupBy(x => x.CreatedDate.Value.Date.Month).Select(x => new
                    {
                        Month = x.Key,
                        Count = x.Count()
                    }).OrderBy(x => x.Month).ToList();

                    var eventStatistics = monthes.Select(x => new
                    {
                        Month = x.Month,
                        AllEventPerMonth = AllEventPerMonth.FirstOrDefault(xx => x.index == xx.Month)?.Count ?? 0,
                        PrivateEventPerMonth = PrivateEventPerMonth.FirstOrDefault(xx => x.index == xx.Month)?.Count ?? 0,
                        FriendzrEventPerMonth = FriendzrEventPerMonth.FirstOrDefault(xx => x.index == xx.Month)?.Count ?? 0,
                        ExternalEventPerMonth = ExternalEventPerMonth.FirstOrDefault(xx => x.index == xx.Month)?.Count ?? 0,
                        AdminExternalEventPerMonth = AdminExternalEventPerMonth.FirstOrDefault(xx => x.index == xx.Month)?.Count ?? 0,
                        WhiteLableEventPerMonth = WhiteLableEventPerMonth.FirstOrDefault(xx => x.index == xx.Month)?.Count ?? 0,
                    }).ToList();

                    IXLWorksheet eventWorksheet = workbook.Worksheets.Add("Event Statistics");

                    int eventStatisticsCurrentRow = 2;
                    eventWorksheet.Range(eventWorksheet.Cell(1, 2), eventWorksheet.Cell(1, 8)).Merge().Value = "Event Statistics";
                    eventWorksheet.Range(eventWorksheet.Cell(1, 2), eventWorksheet.Cell(1, 8)).Style.Fill.SetBackgroundColor(XLColor.Gray);
                    eventWorksheet.Range(eventWorksheet.Cell(1, 2), eventWorksheet.Cell(1, 8)).Style.Font.SetBold().Font.FontSize = 14;
                    eventWorksheet.Cell(eventStatisticsCurrentRow, 2).Value = "Category";
                    eventWorksheet.Cell(eventStatisticsCurrentRow, 3).Value = "All Events";
                    eventWorksheet.Cell(eventStatisticsCurrentRow, 4).Value = "Private Events";
                    eventWorksheet.Cell(eventStatisticsCurrentRow, 5).Value = "Friendzr Events";
                    eventWorksheet.Cell(eventStatisticsCurrentRow, 6).Value = "External Events";
                    eventWorksheet.Cell(eventStatisticsCurrentRow, 7).Value = "Admin External Events";
                    eventWorksheet.Cell(eventStatisticsCurrentRow, 8).Value = "WhiteLable Events";

                    foreach (var monthStatistic in eventStatistics)
                    {
                        eventStatisticsCurrentRow++;
                        eventWorksheet.Cell(eventStatisticsCurrentRow, 2).Value = monthStatistic.Month;
                        eventWorksheet.Cell(eventStatisticsCurrentRow, 3).Value = monthStatistic.AllEventPerMonth;
                        eventWorksheet.Cell(eventStatisticsCurrentRow, 4).Value = monthStatistic.PrivateEventPerMonth;
                        eventWorksheet.Cell(eventStatisticsCurrentRow, 5).Value = monthStatistic.FriendzrEventPerMonth;
                        eventWorksheet.Cell(eventStatisticsCurrentRow, 6).Value = monthStatistic.ExternalEventPerMonth;
                        eventWorksheet.Cell(eventStatisticsCurrentRow, 7).Value = monthStatistic.AdminExternalEventPerMonth;
                        eventWorksheet.Cell(eventStatisticsCurrentRow, 8).Value = monthStatistic.WhiteLableEventPerMonth;
                    }

                    #endregion

                    #region Events In Each Category (6)

                    var categories = authDBContext.category.Select(x => new { CategoryName = x.name, NumOfCreatedEvent = x.EventData.Count });

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

                    #endregion

                    #region Filter In Each Category (7)

                    var listCategories = authDBContext.category.AsNoTracking().Select(x => new { CategoryName = x.name, NumOfFilter = x.EventCategoryTrackers.Count });

                    IXLWorksheet filterInEachCategoryWorksheet = workbook.Worksheets.Add("Filter In Each Category");

                    int filterInEachCategoryCurrentColumn = 2;
                    filterInEachCategoryWorksheet.Range(filterInEachCategoryWorksheet.Cell(1, 2), filterInEachCategoryWorksheet.Cell(1, listCategories.Count() + 1)).Merge().Value = "Filter In Each Category";
                    filterInEachCategoryWorksheet.Range(filterInEachCategoryWorksheet.Cell(1, 2), filterInEachCategoryWorksheet.Cell(1, listCategories.Count() + 1)).Style.Fill.SetBackgroundColor(XLColor.Gray);
                    filterInEachCategoryWorksheet.Range(filterInEachCategoryWorksheet.Cell(1, 2), filterInEachCategoryWorksheet.Cell(1, listCategories.Count() + 1)).Style.Font.SetBold().Font.FontSize = 14;

                    foreach (var category in listCategories)
                    {
                        filterInEachCategoryWorksheet.Cell(2, filterInEachCategoryCurrentColumn).Value = category.CategoryName;
                        filterInEachCategoryCurrentColumn++;
                    }

                    filterInEachCategoryCurrentColumn = 2;
                    foreach (var category in listCategories)
                    {
                        filterInEachCategoryWorksheet.Cell(3, filterInEachCategoryCurrentColumn).Value = category.NumOfFilter;
                        filterInEachCategoryCurrentColumn++;
                    }

                    #endregion

                    #region Users In Each Interest (8)

                    var interests = await authDBContext.Interests.Include(q => q.listoftags).Select(q => new { InterestName = q.name, NumOfUsers = q.listoftags.Count() }).ToListAsync();

                    var series = new List<object>()
                    {
                        new {name="NumOfUsers",data=interests.Select(q=>q.NumOfUsers)},
                    };

                    IXLWorksheet usersInEachinterestWorksheet = workbook.Worksheets.Add("Users In Each interest");

                    int usersInEachinterestCurrentRow = 3;
                    usersInEachinterestWorksheet.Range(usersInEachinterestWorksheet.Cell(1, 2), usersInEachinterestWorksheet.Cell(1, 3)).Merge().Value = "Users In Each interest";
                    usersInEachinterestWorksheet.Range(usersInEachinterestWorksheet.Cell(1, 2), usersInEachinterestWorksheet.Cell(1, 3)).Style.Fill.SetBackgroundColor(XLColor.Gray);
                    usersInEachinterestWorksheet.Range(usersInEachinterestWorksheet.Cell(1, 2), usersInEachinterestWorksheet.Cell(1, 3)).Style.Font.SetBold().Font.FontSize = 14;
                    usersInEachinterestWorksheet.Cell(2, 2).Value = "Interest";
                    usersInEachinterestWorksheet.Cell(2, 3).Value = "Number Of Users";

                    foreach (var interest in interests)
                    {
                        usersInEachinterestWorksheet.Cell(usersInEachinterestCurrentRow, 2).Value = interest.InterestName;
                        usersInEachinterestCurrentRow++;
                    }

                    usersInEachinterestCurrentRow = 3;
                    foreach (var interest in interests)
                    {
                        usersInEachinterestWorksheet.Cell(usersInEachinterestCurrentRow, 3).Value = interest.NumOfUsers;
                        usersInEachinterestCurrentRow++;
                    }

                    #endregion

                    #region  Export Event Share Attend & View Statistics (10)

                    AddNewWorkbookForStatisticsPerMonthAndDay(workbook, ExportEventShareAttendViewStatistics(EventActionType.Share.ToString()), "Event Share", "Event Share By Age", "Event Share By Gender");
                    AddNewWorkbookForStatisticsPerMonthAndDay(workbook, ExportEventShareAttendViewStatistics(EventActionType.View.ToString()), "Event View", "Event View By Age", "Event View By Gender");
                    AddNewWorkbookForStatisticsPerMonthAndDay(workbook, ExportEventShareAttendViewStatistics(EventActionType.Attend.ToString()), "Event Attend", "Event Attend By Age", "Event Attend By Gender");

                    #endregion

                    #region  User Statistics (11 , 12 , 13 , 14 , 15 , 16 , 17 , 18)

                    #region  Finished Registration (11)

                    List<User> finishedRegistration = users.Where(q => q.EmailConfirmed == true && (q.UserDetails.ProfileCompleted != null && q.UserDetails.ProfileCompleted == true)).ToList();

                    StatisticsByGenderAndAgeViewModel finishedRegistrationUserStatictes = new StatisticsByGenderAndAgeViewModel()
                    {
                        All = finishedRegistration.Count(),
                        Male = finishedRegistration.Where(q => q.UserDetails.Gender == "male").Count(),
                        Female = finishedRegistration.Where(q => q.UserDetails.Gender == "female").Count(),
                        Other = finishedRegistration.Where(q => q.UserDetails.Gender == "other").Count(),
                        From18To25 = finishedRegistration.Where(q => GetAge(q.UserDetails.birthdate.Value) >= 18 && GetAge(q.UserDetails.birthdate.Value) <= 25).Count(),
                        From25To34 = finishedRegistration.Where(q => GetAge(q.UserDetails.birthdate.Value) >= 26 && GetAge(q.UserDetails.birthdate.Value) <= 34).Count(),
                        From35To44 = finishedRegistration.Where(q => GetAge(q.UserDetails.birthdate.Value) >= 35 && GetAge(q.UserDetails.birthdate.Value) <= 44).Count(),
                        From45To54 = finishedRegistration.Where(q => GetAge(q.UserDetails.birthdate.Value) >= 45 && GetAge(q.UserDetails.birthdate.Value) <= 54).Count(),
                        From55To64 = finishedRegistration.Where(q => GetAge(q.UserDetails.birthdate.Value) >= 55 && GetAge(q.UserDetails.birthdate.Value) <= 64).Count(),
                        From65AndMore = finishedRegistration.Where(q => GetAge(q.UserDetails.birthdate.Value) >= 65).Count(),
                    };

                    AddNewWorkbook(workbook, finishedRegistrationUserStatictes, "Finished registration", "Finished registration By Age", "Finished registration By Gender");

                    #endregion

                    #region  Gender Of Users (12)

                    List<User> genderOfUsers = users.Where(q => q.EmailConfirmed == true && (q.UserDetails.ProfileCompleted != null && q.UserDetails.ProfileCompleted == true)).ToList();

                    StatisticsByGenderAndAgeViewModel genderOfUserStatictes = new StatisticsByGenderAndAgeViewModel()
                    {
                        All = genderOfUsers.Count(),
                        Male = genderOfUsers.Where(q => q.UserDetails.Gender == "male").Count(),
                        Female = genderOfUsers.Where(q => q.UserDetails.Gender == "female").Count(),
                        Other = genderOfUsers.Where(q => q.UserDetails.Gender == "other").Count(),
                    };

                    AddNewWorkbookForGenderOfUsers(workbook, genderOfUserStatictes, "Gender of users", "Gender of users");

                    #endregion

                    #region  Users With Personal Space (13)

                    List<User> usersWithPersonalSpace = users.Where(q => q.EmailConfirmed == true && (q.UserDetails.ProfileCompleted != null && q.UserDetails.ProfileCompleted == true) && q.UserDetails.personalSpace).ToList();

                    StatisticsByGenderAndAgeViewModel usersWhoEnabledPersonalSpaceStatictes = new StatisticsByGenderAndAgeViewModel()
                    {
                        All = usersWithPersonalSpace.Count(),
                        Male = usersWithPersonalSpace.Where(q => q.UserDetails.Gender == "male").Count(),
                        Female = usersWithPersonalSpace.Where(q => q.UserDetails.Gender == "female").Count(),
                        Other = usersWithPersonalSpace.Where(q => q.UserDetails.Gender == "other").Count(),
                        From18To25 = usersWithPersonalSpace.Where(q => GetAge(q.UserDetails.birthdate.Value) >= 18 && GetAge(q.UserDetails.birthdate.Value) <= 25).Count(),
                        From25To34 = usersWithPersonalSpace.Where(q => GetAge(q.UserDetails.birthdate.Value) >= 26 && GetAge(q.UserDetails.birthdate.Value) <= 34).Count(),
                        From35To44 = usersWithPersonalSpace.Where(q => GetAge(q.UserDetails.birthdate.Value) >= 35 && GetAge(q.UserDetails.birthdate.Value) <= 44).Count(),
                        From45To54 = usersWithPersonalSpace.Where(q => GetAge(q.UserDetails.birthdate.Value) >= 45 && GetAge(q.UserDetails.birthdate.Value) <= 54).Count(),
                        From55To64 = usersWithPersonalSpace.Where(q => GetAge(q.UserDetails.birthdate.Value) >= 55 && GetAge(q.UserDetails.birthdate.Value) <= 64).Count(),
                        From65AndMore = usersWithPersonalSpace.Where(q => GetAge(q.UserDetails.birthdate.Value) >= 65).Count(),
                    };

                    AddNewWorkbook(workbook, usersWhoEnabledPersonalSpaceStatictes, "Users with personal space", "Users who enabled personal space By Age", "Users who enabled personal space By Gender");
                    #endregion

                    #region  Number Of Connection Requests Sent (14)

                    List<Requestes> numberOfConnectionRequests = requestes.Where(q => !q.User.Email.ToLower().Contains("@owner") && q.User != null && q.User.User.EmailConfirmed == true && (q.User.ProfileCompleted != null && q.User.ProfileCompleted == true)).ToList();

                    StatisticsByGenderAndAgeViewModel numberOfConnectionRequestsSentStatictes = new StatisticsByGenderAndAgeViewModel()
                    {
                        All = numberOfConnectionRequests.Count(),
                        Male = numberOfConnectionRequests.Where(q => q.User.Gender == "male").Count(),
                        Female = numberOfConnectionRequests.Where(q => q.User.Gender == "female").Count(),
                        Other = numberOfConnectionRequests.Where(q => q.User.Gender == "other").Count(),
                        From18To25 = numberOfConnectionRequests.Where(q => GetAge(q.User.birthdate.Value) >= 18 && GetAge(q.User.birthdate.Value) <= 25).Count(),
                        From25To34 = numberOfConnectionRequests.Where(q => GetAge(q.User.birthdate.Value) >= 26 && GetAge(q.User.birthdate.Value) <= 34).Count(),
                        From35To44 = numberOfConnectionRequests.Where(q => GetAge(q.User.birthdate.Value) >= 35 && GetAge(q.User.birthdate.Value) <= 44).Count(),
                        From45To54 = numberOfConnectionRequests.Where(q => GetAge(q.User.birthdate.Value) >= 45 && GetAge(q.User.birthdate.Value) <= 54).Count(),
                        From55To64 = numberOfConnectionRequests.Where(q => GetAge(q.User.birthdate.Value) >= 55 && GetAge(q.User.birthdate.Value) <= 64).Count(),
                        From65AndMore = numberOfConnectionRequests.Where(q => GetAge(q.User.birthdate.Value) >= 65).Count(),
                    };

                    AddNewWorkbook(workbook, numberOfConnectionRequestsSentStatictes, "Connection Requests Sent", "Connection Requests Sent By Age", "Connection Requests By Gender");

                    #endregion

                    #region  Number Of Connection Requests Accepted (15)

                    List<Requestes> numberOfConnectionRequestAccepted = requestes.Where(q => !q.User.Email.ToLower().Contains("@owner") && q.User != null && q.User.User.EmailConfirmed == true && (q.User.ProfileCompleted != null && q.User.ProfileCompleted == true) && q.status == 1).ToList();

                    StatisticsByGenderAndAgeViewModel numberOfConnectionRequestsAcceptedStatictes = new StatisticsByGenderAndAgeViewModel()
                    {
                        All = numberOfConnectionRequestAccepted.Count(),
                        Male = numberOfConnectionRequestAccepted.Where(q => q.User.Gender == "male").Count(),
                        Female = numberOfConnectionRequestAccepted.Where(q => q.User.Gender == "female").Count(),
                        Other = numberOfConnectionRequestAccepted.Where(q => q.User.Gender == "other").Count(),
                        From18To25 = numberOfConnectionRequestAccepted.Where(q => GetAge(q.User.birthdate.Value) >= 18 && GetAge(q.User.birthdate.Value) <= 25).Count(),
                        From25To34 = numberOfConnectionRequestAccepted.Where(q => GetAge(q.User.birthdate.Value) >= 26 && GetAge(q.User.birthdate.Value) <= 34).Count(),
                        From35To44 = numberOfConnectionRequestAccepted.Where(q => GetAge(q.User.birthdate.Value) >= 35 && GetAge(q.User.birthdate.Value) <= 44).Count(),
                        From45To54 = numberOfConnectionRequestAccepted.Where(q => GetAge(q.User.birthdate.Value) >= 45 && GetAge(q.User.birthdate.Value) <= 54).Count(),
                        From55To64 = numberOfConnectionRequestAccepted.Where(q => GetAge(q.User.birthdate.Value) >= 55 && GetAge(q.User.birthdate.Value) <= 64).Count(),
                        From65AndMore = numberOfConnectionRequestAccepted.Where(q => GetAge(q.User.birthdate.Value) >= 65).Count(),
                    };

                    AddNewWorkbook(workbook, numberOfConnectionRequestsAcceptedStatictes, "Accepted Connection Requests", "Accepted Connection Requests By Age", "Accepted Connection Requests By Gender");

                    #endregion

                    #region  Blocked Users (16)

                    List<Requestes> blockedUsers = requestes.Where(q => !q.User.Email.ToLower().Contains("@owner") && q.User.birthdate != null && q.User != null && q.User.User.EmailConfirmed == true && (q.User.ProfileCompleted != null && q.User.ProfileCompleted == true) && q.status == 2).ToList();

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

                    AddNewWorkbook(workbook, blockedUsersStatictes, "Blocked Users", "Blocked Users By Age", "Blocked Users By Gender");

                    #endregion

                    #region  Active Users (17)

                    StatisticsByGenderAndAgeViewModel ActiveUsersStatictes = new StatisticsByGenderAndAgeViewModel()
                    {
                        All = LoggedinUsers.Where(q => q.ExpiredOn > DateTime.Now).Count(),
                        Male = LoggedinUsers.Where(q => q.ExpiredOn > DateTime.Now && q.User.UserDetails.Gender == "male").Count(),
                        Female = LoggedinUsers.Where(q => q.ExpiredOn > DateTime.Now && q.User.UserDetails.Gender == "female").Count(),
                        Other = LoggedinUsers.Where(q => q.ExpiredOn > DateTime.Now && q.User.UserDetails.Gender == "other").Count(),
                        From18To25 = LoggedinUsers.Where(q => q.ExpiredOn > DateTime.Now && GetAge(q.User.UserDetails.birthdate.Value) >= 18 && GetAge(q.User.UserDetails.birthdate.Value) <= 25).Count(),
                        From25To34 = LoggedinUsers.Where(q => q.ExpiredOn > DateTime.Now && GetAge(q.User.UserDetails.birthdate.Value) >= 26 && GetAge(q.User.UserDetails.birthdate.Value) <= 34).Count(),
                        From35To44 = LoggedinUsers.Where(q => q.ExpiredOn > DateTime.Now && GetAge(q.User.UserDetails.birthdate.Value) >= 35 && GetAge(q.User.UserDetails.birthdate.Value) <= 44).Count(),
                        From45To54 = LoggedinUsers.Where(q => q.ExpiredOn > DateTime.Now && GetAge(q.User.UserDetails.birthdate.Value) >= 45 && GetAge(q.User.UserDetails.birthdate.Value) <= 54).Count(),
                        From55To64 = LoggedinUsers.Where(q => q.ExpiredOn > DateTime.Now && GetAge(q.User.UserDetails.birthdate.Value) >= 55 && GetAge(q.User.UserDetails.birthdate.Value) <= 64).Count(),
                        From65AndMore = LoggedinUsers.Where(q => q.ExpiredOn > DateTime.Now && GetAge(q.User.UserDetails.birthdate.Value) >= 65).Count(),
                    };

                    AddNewWorkbook(workbook, ActiveUsersStatictes, "Active Users", "Active Users By Age", "Active Users By Gender");

                    #endregion

                    #region  Users Who Created Events (18)

                    var usersWhoCreatedEvents = events.GroupBy(x => x.UserId).ToList();

                    StatisticsByGenderAndAgeViewModel percentageOfUsersWhoCreatedEventsStatictes = new StatisticsByGenderAndAgeViewModel()
                    {
                        All = usersWhoCreatedEvents.Count(),
                        Male = usersWhoCreatedEvents.Where(q => q.FirstOrDefault().User.Gender == "male").Count(),
                        Female = usersWhoCreatedEvents.Where(q => q.FirstOrDefault().User.Gender == "female").Count(),
                        Other = usersWhoCreatedEvents.Where(q => q.FirstOrDefault().User.Gender == "other").Count(),
                        From18To25 = usersWhoCreatedEvents.Where(q => GetAge(q.FirstOrDefault().User.birthdate.Value) >= 18 && GetAge(q.FirstOrDefault().User.birthdate.Value) <= 25).Count(),
                        From25To34 = usersWhoCreatedEvents.Where(q => GetAge(q.FirstOrDefault().User.birthdate.Value) >= 26 && GetAge(q.FirstOrDefault().User.birthdate.Value) <= 34).Count(),
                        From35To44 = usersWhoCreatedEvents.Where(q => GetAge(q.FirstOrDefault().User.birthdate.Value) >= 35 && GetAge(q.FirstOrDefault().User.birthdate.Value) <= 44).Count(),
                        From45To54 = usersWhoCreatedEvents.Where(q => GetAge(q.FirstOrDefault().User.birthdate.Value) >= 45 && GetAge(q.FirstOrDefault().User.birthdate.Value) <= 54).Count(),
                        From55To64 = usersWhoCreatedEvents.Where(q => GetAge(q.FirstOrDefault().User.birthdate.Value) >= 55 && GetAge(q.FirstOrDefault().User.birthdate.Value) <= 64).Count(),
                        From65AndMore = usersWhoCreatedEvents.Where(q => GetAge(q.FirstOrDefault().User.birthdate.Value) >= 65).Count(),
                    };

                    AddNewWorkbook(workbook, percentageOfUsersWhoCreatedEventsStatictes, "Users Who Created Events", "Users Who Created Events By Age", "Users Who Created Events By Gender");

                    #endregion

                    #endregion

                    #region Event Statictis By Rate Chart (19)

                    List<EventData> allEventList = await authDBContext.EventData.AsNoTracking().ToListAsync();

                    EventStatisticViewModel numberOfEventsCreatedByFriendzrStatistics = new EventStatisticViewModel()
                    {
                        All = allEventList.Count(),
                        Friendzr = allEventList.Where(q => q.EventTypeListid == 1).Count()
                    };

                    IXLWorksheet eventStatictisByRateWorksheet = workbook.Worksheets.Add("Friendzr Events");

                    eventStatictisByRateWorksheet.Range(eventStatictisByRateWorksheet.Cell(1, 2), eventStatictisByRateWorksheet.Cell(1, 3)).Merge().Value = "Friendzr Events";
                    eventStatictisByRateWorksheet.Range(eventStatictisByRateWorksheet.Cell(1, 2), eventStatictisByRateWorksheet.Cell(1, 3)).Style.Fill.SetBackgroundColor(XLColor.Gray);
                    eventStatictisByRateWorksheet.Range(eventStatictisByRateWorksheet.Cell(1, 2), eventStatictisByRateWorksheet.Cell(1, 3)).Style.Font.SetBold().Font.FontSize = 14;
                    eventStatictisByRateWorksheet.Cell(2, 2).Value = "Friendzr Events Count";
                    eventStatictisByRateWorksheet.Cell(3, 2).Value = "Friendzr Event Percentage";

                    eventStatictisByRateWorksheet.Cell(2, 3).Value = $"Friendzr Events are:{numberOfEventsCreatedByFriendzrStatistics.Friendzr} from :{numberOfEventsCreatedByFriendzrStatistics.All} Event ";
                    eventStatictisByRateWorksheet.Cell(3, 3).Value = $"{numberOfEventsCreatedByFriendzrStatistics.Rate} %";

                    #endregion

                    #region Age Statistics By Rate Chart (20)

                    IXLWorksheet ageWorksheet = workbook.Worksheets.Add("Age Statistics");

                    ageWorksheet.Range(ageWorksheet.Cell(1, 2), ageWorksheet.Cell(1, 5)).Merge().Value = "Age Statistics";
                    ageWorksheet.Range(ageWorksheet.Cell(1, 2), ageWorksheet.Cell(1, 5)).Style.Fill.SetBackgroundColor(XLColor.Gray);
                    ageWorksheet.Range(ageWorksheet.Cell(1, 2), ageWorksheet.Cell(1, 5)).Style.Font.SetBold().Font.FontSize = 14;
                    ageWorksheet.Cell(2, 2).Value = "Age less Than 18 Is:";
                    ageWorksheet.Cell(2, 4).Value = "Percentage:";
                    ageWorksheet.Cell(3, 2).Value = "Age from 18 - 24 Is:";
                    ageWorksheet.Cell(3, 4).Value = "Percentage:";
                    ageWorksheet.Cell(4, 2).Value = "Age from 25 - 34 Is:";
                    ageWorksheet.Cell(4, 4).Value = "Percentage:";
                    ageWorksheet.Cell(5, 2).Value = "Age from 35 - 54 Is:";
                    ageWorksheet.Cell(5, 4).Value = "Percentage:";
                    ageWorksheet.Cell(6, 2).Value = "Age more than 55 Is:";
                    ageWorksheet.Cell(6, 4).Value = "Percentage:";

                    ageWorksheet.Cell(2, 3).Value = userStatistics.UserWithLessThan18Age_Count;
                    ageWorksheet.Cell(2, 5).Value = $"{userStatistics.UserWithLessThan18Age_Rate} %";
                    ageWorksheet.Cell(3, 3).Value = userStatistics.UsersWith18_24Age_Count;
                    ageWorksheet.Cell(3, 5).Value = $"{userStatistics.UsersWith18_24Age_Rate} %";
                    ageWorksheet.Cell(4, 3).Value = userStatistics.UsersWith25_34Age_Count;
                    ageWorksheet.Cell(4, 5).Value = $"{userStatistics.UsersWith25_34Age_Rate} %";
                    ageWorksheet.Cell(5, 3).Value = userStatistics.UsersWith35_54Age_Count;
                    ageWorksheet.Cell(5, 5).Value = $"{userStatistics.UsersWith35_54Age_Rate} %";
                    ageWorksheet.Cell(6, 3).Value = userStatistics.UsersWithMoreThan55Age_Count;
                    ageWorksheet.Cell(6, 5).Value = $"{userStatistics.UsersWithMoreThan55Age_Rate} %";

                    #endregion

                    #region Private Mode Statistics By Rate Chart (21)

                    IXLWorksheet privateModeWorksheet = workbook.Worksheets.Add("Private Mode Statistics");

                    privateModeWorksheet.Range(privateModeWorksheet.Cell(1, 2), privateModeWorksheet.Cell(1, 5)).Merge().Value = "Private Mode Statistics";
                    privateModeWorksheet.Range(privateModeWorksheet.Cell(1, 2), privateModeWorksheet.Cell(1, 5)).Style.Fill.SetBackgroundColor(XLColor.Gray);
                    privateModeWorksheet.Range(privateModeWorksheet.Cell(1, 2), privateModeWorksheet.Cell(1, 5)).Style.Font.SetBold().Font.FontSize = 14;
                    privateModeWorksheet.Cell(2, 2).Value = "EveryOne:";
                    privateModeWorksheet.Cell(2, 4).Value = "Percentage:";
                    privateModeWorksheet.Cell(3, 2).Value = "OtherGender:";
                    privateModeWorksheet.Cell(3, 4).Value = "Percentage:";
                    privateModeWorksheet.Cell(4, 2).Value = "Female:";
                    privateModeWorksheet.Cell(4, 4).Value = "Percentage:";
                    privateModeWorksheet.Cell(5, 2).Value = "Male:";
                    privateModeWorksheet.Cell(5, 4).Value = "Percentage:";

                    privateModeWorksheet.Cell(2, 3).Value = userStatistics.AppearenceEveryOneInGhostMode_Count;
                    privateModeWorksheet.Cell(2, 5).Value = $"{userStatistics.AppearenceEveryOneInGhostMode_Rate} %";
                    privateModeWorksheet.Cell(3, 3).Value = userStatistics.AppearenceOtherGenderInGhostMode_Count;
                    privateModeWorksheet.Cell(3, 5).Value = $"{userStatistics.AppearenceOtherGenderInGhostMode_Rate} %";
                    privateModeWorksheet.Cell(4, 3).Value = userStatistics.AppearenceFemaleInGhostMode_Count;
                    privateModeWorksheet.Cell(4, 5).Value = $"{userStatistics.AppearenceFemaleInGhostMode_Rate} %";
                    privateModeWorksheet.Cell(5, 3).Value = userStatistics.AppearenceMaleInGhostMode_Count;
                    privateModeWorksheet.Cell(5, 5).Value = $"{userStatistics.AppearenceMaleInGhostMode_Rate} %";

                    #endregion

                    #region Users Statistics By Rate Chart (22)

                    IXLWorksheet usersStatisticsByRateWorksheet = workbook.Worksheets.Add("Users Statistics By Rate");

                    usersStatisticsByRateWorksheet.Range(usersStatisticsByRateWorksheet.Cell(1, 2), usersStatisticsByRateWorksheet.Cell(1, 5)).Merge().Value = "Users Statistics By Rate";
                    usersStatisticsByRateWorksheet.Range(usersStatisticsByRateWorksheet.Cell(1, 2), usersStatisticsByRateWorksheet.Cell(1, 5)).Style.Fill.SetBackgroundColor(XLColor.Gray);
                    usersStatisticsByRateWorksheet.Range(usersStatisticsByRateWorksheet.Cell(1, 2), usersStatisticsByRateWorksheet.Cell(1, 5)).Style.Font.SetBold().Font.FontSize = 14;
                    usersStatisticsByRateWorksheet.Cell(2, 2).Value = "Verified Users:";
                    usersStatisticsByRateWorksheet.Cell(2, 4).Value = "Percentage:";
                    usersStatisticsByRateWorksheet.Cell(3, 2).Value = "UnVerified Users:";
                    usersStatisticsByRateWorksheet.Cell(3, 4).Value = "Percentage:";
                    usersStatisticsByRateWorksheet.Cell(4, 2).Value = "Deleted profiles:";
                    usersStatisticsByRateWorksheet.Cell(4, 4).Value = "Percentage:";
                    usersStatisticsByRateWorksheet.Cell(5, 2).Value = "Users With Personal Space Enabled:";
                    usersStatisticsByRateWorksheet.Cell(5, 4).Value = "Percentage:";
                    usersStatisticsByRateWorksheet.Cell(6, 2).Value = "Push Notifications Enabled:";
                    usersStatisticsByRateWorksheet.Cell(6, 4).Value = "Percentage:";
                    usersStatisticsByRateWorksheet.Cell(7, 2).Value = "Users With Private Mode Enabled:";
                    usersStatisticsByRateWorksheet.Cell(8, 2).Value = "Max Age Filtering 18 - 85 user:";
                    usersStatisticsByRateWorksheet.Cell(9, 2).Value = "Active Users :";

                    usersStatisticsByRateWorksheet.Cell(2, 3).Value = $"Verified Users {userStatistics.ConfirmedMailUsers_Count} From {userStatistics.CurrenUsers_Count} User";
                    usersStatisticsByRateWorksheet.Cell(2, 5).Value = $"{userStatistics.UsersVertified_Rate} %";
                    usersStatisticsByRateWorksheet.Cell(3, 3).Value = $"UnVerified Users {userStatistics.UnConfirmedMailUsers_Count} From {userStatistics.CurrenUsers_Count} User";
                    usersStatisticsByRateWorksheet.Cell(3, 5).Value = $"{userStatistics.UsersUnVertified_Rate} %";
                    usersStatisticsByRateWorksheet.Cell(4, 3).Value = $"Deleted profiles :{userStatistics.DeletedProfiles_Count} User";
                    usersStatisticsByRateWorksheet.Cell(4, 5).Value = $"{userStatistics.DeletedProfiles_Rate} %";
                    usersStatisticsByRateWorksheet.Cell(5, 3).Value = $"Users With Personal Space Enabled From {userStatistics.Updated} User";
                    usersStatisticsByRateWorksheet.Cell(5, 5).Value = $"{userStatistics.UsersWithPersonalSpaceEnabled_Rate} %";
                    usersStatisticsByRateWorksheet.Cell(6, 3).Value = $"Push Notifications Enabled From {userStatistics.Updated} User";
                    usersStatisticsByRateWorksheet.Cell(6, 5).Value = $"{userStatistics.UsersWithPushNotificationsEnabled_Rate} %";
                    usersStatisticsByRateWorksheet.Cell(7, 3).Value = $"{userStatistics.UseresEnableGhostModeRate_Rate} %";
                    usersStatisticsByRateWorksheet.Cell(8, 3).Value = $"{userStatistics.MostAgeFiltirngRangeUsed_Rate} %";
                    usersStatisticsByRateWorksheet.Cell(9, 3).Value = $"{userStatistics.ActiveUsers_Rate} %";


                    #endregion

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
            catch (Exception ex)
            {
                throw ex;
            }

        }

        #endregion

        #region private Methods

        private int GetAge(DateTime dob)
        {
            int age = 0;
            age = DateTime.Now.Subtract(dob).Days;
            age = age / 365;
            return age;
        }

        private ExportStatisticsByGenderAndAgeViewModel ExportLinkClickStatistics(List<UserLinkClick> userLinkClicks, LinkClickTypeEnum type)
        {
            var monthes = Enumerable.Range(1, 12).Select(i => new { index = i, Month = DateTimeFormatInfo.CurrentInfo.GetAbbreviatedMonthName(i) });

            ExportStatisticsByGenderAndAgeViewModel linkClicksPerMonth = new ExportStatisticsByGenderAndAgeViewModel();

            #region Export Per Month

            var userLinkClicksbyMonthMale = userLinkClicks.Where(q => q.Type == type.ToString() && q.userDetails.Gender == "male").GroupBy(x => new { Month = x.Date.Month }).Select(x => new { Month = x.Key.Month, Count = x.Count(), Birthdate = x.FirstOrDefault().userDetails.birthdate }).OrderBy(x => x.Month).ToList();
            var userLinkClicksbyMonthFemale = userLinkClicks.Where(q => q.Type == type.ToString() && q.userDetails.Gender == "female").GroupBy(x => new { Month = x.Date.Month }).Select(x => new { Month = x.Key.Month, Count = x.Count(), Birthdate = x.FirstOrDefault().userDetails.birthdate }).OrderBy(x => x.Month).ToList();
            var userLinkClicksbyMonthOther = userLinkClicks.Where(q => q.Type == type.ToString() && q.userDetails.Gender == "other").GroupBy(x => new { Month = x.Date.Month }).Select(x => new { Month = x.Key.Month, Count = x.Count(), Birthdate = x.FirstOrDefault().userDetails.birthdate }).OrderBy(x => x.Month).ToList();

            var From18To24 = userLinkClicks.Where(q => q.Type == type.ToString() && GetAge(q.userDetails.birthdate.Value) >= 18 && GetAge(q.userDetails.birthdate.Value) <= 25).GroupBy(x => new { Month = x.Date.Month }).Select(x => new { Month = x.Key.Month, Count = x.Count(), Birthdate = x.FirstOrDefault().userDetails.birthdate }).OrderBy(x => x.Month).ToList();
            var From25To34 = userLinkClicks.Where(q => q.Type == type.ToString() && GetAge(q.userDetails.birthdate.Value) >= 26 && GetAge(q.userDetails.birthdate.Value) <= 34).GroupBy(x => new { Month = x.Date.Month }).Select(x => new { Month = x.Key.Month, Count = x.Count(), Birthdate = x.FirstOrDefault().userDetails.birthdate }).OrderBy(x => x.Month).ToList();
            var From35To44 = userLinkClicks.Where(q => q.Type == type.ToString() && GetAge(q.userDetails.birthdate.Value) >= 35 && GetAge(q.userDetails.birthdate.Value) <= 44).GroupBy(x => new { Month = x.Date.Month }).Select(x => new { Month = x.Key.Month, Count = x.Count(), Birthdate = x.FirstOrDefault().userDetails.birthdate }).OrderBy(x => x.Month).ToList();
            var From45To54 = userLinkClicks.Where(q => q.Type == type.ToString() && GetAge(q.userDetails.birthdate.Value) >= 45 && GetAge(q.userDetails.birthdate.Value) <= 54).GroupBy(x => new { Month = x.Date.Month }).Select(x => new { Month = x.Key.Month, Count = x.Count(), Birthdate = x.FirstOrDefault().userDetails.birthdate }).OrderBy(x => x.Month).ToList();
            var From55To64 = userLinkClicks.Where(q => q.Type == type.ToString() && GetAge(q.userDetails.birthdate.Value) >= 55 && GetAge(q.userDetails.birthdate.Value) <= 64).GroupBy(x => new { Month = x.Date.Month }).Select(x => new { Month = x.Key.Month, Count = x.Count(), Birthdate = x.FirstOrDefault().userDetails.birthdate }).OrderBy(x => x.Month).ToList();
            var MoreThan65 = userLinkClicks.Where(q => q.Type == type.ToString() && GetAge(q.userDetails.birthdate.Value) >= 65).GroupBy(x => new { Month = x.Date.Month }).Select(x => new { Month = x.Key.Month, Count = x.Count(), Birthdate = x.FirstOrDefault().userDetails.birthdate }).OrderBy(x => x.Month).ToList();

            linkClicksPerMonth.StatisticsByGender = monthes.Select(m => new StatisticsByGenderViewModel()
            {
                Month = m.Month,
                Male = userLinkClicksbyMonthMale.Where(n => m.index == n.Month).FirstOrDefault()?.Count ?? 0,
                Female = userLinkClicksbyMonthFemale.Where(n => m.index == n.Month).FirstOrDefault()?.Count ?? 0,
                Other = userLinkClicksbyMonthOther.Where(n => m.index == n.Month).FirstOrDefault()?.Count ?? 0,
            }).ToList();


            linkClicksPerMonth.StatisticsByAge = monthes.Select(m => new StatisticsByAgeViewModel()
            {
                Month = m.Month,
                From18To24 = From18To24.Where(n => m.index == n.Month).FirstOrDefault()?.Count ?? 0,
                From25To34 = From25To34.Where(n => m.index == n.Month).FirstOrDefault()?.Count ?? 0,
                From35To44 = From35To44.Where(n => m.index == n.Month).FirstOrDefault()?.Count ?? 0,
                From45To54 = From45To54.Where(n => m.index == n.Month).FirstOrDefault()?.Count ?? 0,
                From55To64 = From55To64.Where(n => m.index == n.Month).FirstOrDefault()?.Count ?? 0,
                MoreThan65 = MoreThan65.Where(n => m.index == n.Month).FirstOrDefault()?.Count ?? 0
            }).ToList();

            #endregion

            #region Export Per Day

            var userLinkClicksbyDayMale = userLinkClicks.Where(q => q.Type == type.ToString() && q.userDetails.Gender == "male").GroupBy(x => new { Day = x.Date.Day }).Select(x => new { Day = x.Key.Day, Count = x.Count(), Birthdate = x.FirstOrDefault().userDetails.birthdate, Month = x.FirstOrDefault().Date.Month }).OrderBy(x => x.Day).ToList();
            var userLinkClicksbyDayFemale = userLinkClicks.Where(q => q.Type == type.ToString() && q.userDetails.Gender == "female").GroupBy(x => new { Day = x.Date.Day }).Select(x => new { Day = x.Key.Day, Count = x.Count(), Birthdate = x.FirstOrDefault().userDetails.birthdate, Month = x.FirstOrDefault().Date.Month }).OrderBy(x => x.Day).ToList();
            var userLinkClicksbyDayOther = userLinkClicks.Where(q => q.Type == type.ToString() && q.userDetails.Gender == "other").GroupBy(x => new { Day = x.Date.Day }).Select(x => new { Day = x.Key.Day, Count = x.Count(), Birthdate = x.FirstOrDefault().userDetails.birthdate, Month = x.FirstOrDefault().Date.Month }).OrderBy(x => x.Day).ToList();

            var From18To24PerDay = userLinkClicks.Where(q => q.Type == type.ToString() && GetAge(q.userDetails.birthdate.Value) >= 18 && GetAge(q.userDetails.birthdate.Value) <= 25).GroupBy(x => new { Day = x.Date.Day }).Select(x => new { Day = x.Key.Day, Count = x.Count(), Birthdate = x.FirstOrDefault().userDetails.birthdate, Month = x.FirstOrDefault().Date.Month }).OrderBy(x => x.Day).ToList();
            var From25To34PerDay = userLinkClicks.Where(q => q.Type == type.ToString() && GetAge(q.userDetails.birthdate.Value) >= 26 && GetAge(q.userDetails.birthdate.Value) <= 34).GroupBy(x => new { Day = x.Date.Day }).Select(x => new { Day = x.Key.Day, Count = x.Count(), Birthdate = x.FirstOrDefault().userDetails.birthdate, Month = x.FirstOrDefault().Date.Month }).OrderBy(x => x.Day).ToList();
            var From35To44PerDay = userLinkClicks.Where(q => q.Type == type.ToString() && GetAge(q.userDetails.birthdate.Value) >= 35 && GetAge(q.userDetails.birthdate.Value) <= 44).GroupBy(x => new { Day = x.Date.Day }).Select(x => new { Day = x.Key.Day, Count = x.Count(), Birthdate = x.FirstOrDefault().userDetails.birthdate, Month = x.FirstOrDefault().Date.Month }).OrderBy(x => x.Day).ToList();
            var From45To54PerDay = userLinkClicks.Where(q => q.Type == type.ToString() && GetAge(q.userDetails.birthdate.Value) >= 45 && GetAge(q.userDetails.birthdate.Value) <= 54).GroupBy(x => new { Day = x.Date.Day }).Select(x => new { Day = x.Key.Day, Count = x.Count(), Birthdate = x.FirstOrDefault().userDetails.birthdate, Month = x.FirstOrDefault().Date.Month }).OrderBy(x => x.Day).ToList();
            var From55To64PerDay = userLinkClicks.Where(q => q.Type == type.ToString() && GetAge(q.userDetails.birthdate.Value) >= 55 && GetAge(q.userDetails.birthdate.Value) <= 64).GroupBy(x => new { Day = x.Date.Day }).Select(x => new { Day = x.Key.Day, Count = x.Count(), Birthdate = x.FirstOrDefault().userDetails.birthdate, Month = x.FirstOrDefault().Date.Month }).OrderBy(x => x.Day).ToList();
            var MoreThan65PerDay = userLinkClicks.Where(q => q.Type == type.ToString() && GetAge(q.userDetails.birthdate.Value) >= 65).GroupBy(x => new { Day = x.Date.Day }).Select(x => new { Day = x.Key.Day, Count = x.Count(), Birthdate = x.FirstOrDefault().userDetails.birthdate, Month = x.FirstOrDefault().Date.Month }).OrderBy(x => x.Day).ToList();

            foreach (var month in monthes)
            {
                List<StatisticsByGenderViewModel> statisticsByGendersPerDay = new List<StatisticsByGenderViewModel>();
                List<StatisticsByAgeViewModel> StatisticsByAgePerDay = new List<StatisticsByAgeViewModel>();

                var daysOfMonth = Enumerable.Range(1, DateTime.DaysInMonth(DateTime.Now.Year, month.index)).Select(d => new DateTime(DateTime.Now.Year, month.index, d));

                statisticsByGendersPerDay = daysOfMonth.Select(d => new StatisticsByGenderViewModel()
                {
                    Month = month.Month,
                    Day = d.DayOfWeek.ToString(),
                    Date = d.Date.ToString("dd/MM/yyyy").Trim(),
                    Male = userLinkClicksbyDayMale.Where(n => d.Day == n.Day && n.Month == month.index).FirstOrDefault()?.Count ?? 0,
                    Female = userLinkClicksbyDayFemale.Where(n => d.Day == n.Day && n.Month == month.index).FirstOrDefault()?.Count ?? 0,
                    Other = userLinkClicksbyDayOther.Where(n => d.Day == n.Day && n.Month == month.index).FirstOrDefault()?.Count ?? 0,
                }).ToList();

                StatisticsByAgePerDay = daysOfMonth.Select(d => new StatisticsByAgeViewModel()
                {
                    Month = month.Month,
                    Day = d.DayOfWeek.ToString(),
                    Date = d.Date.ToString("dd/MM/yyyy").Trim(),
                    From18To24 = From18To24PerDay.Where(n => d.Day == n.Day && n.Month == month.index).FirstOrDefault()?.Count ?? 0,
                    From25To34 = From25To34PerDay.Where(n => d.Day == n.Day && n.Month == month.index).FirstOrDefault()?.Count ?? 0,
                    From35To44 = From35To44PerDay.Where(n => d.Day == n.Day && n.Month == month.index).FirstOrDefault()?.Count ?? 0,
                    From45To54 = From45To54PerDay.Where(n => d.Day == n.Day && n.Month == month.index).FirstOrDefault()?.Count ?? 0,
                    From55To64 = From55To64PerDay.Where(n => d.Day == n.Day && n.Month == month.index).FirstOrDefault()?.Count ?? 0,
                    MoreThan65 = MoreThan65PerDay.Where(n => d.Day == n.Day && n.Month == month.index).FirstOrDefault()?.Count ?? 0
                }).ToList();

                linkClicksPerMonth.StatisticsByGenderPerDay.AddRange(statisticsByGendersPerDay);
                linkClicksPerMonth.StatisticsByAgePerDay.AddRange(StatisticsByAgePerDay);

            }

            #endregion

            return linkClicksPerMonth;
        }

        private ExportStatisticsByGenderAndAgeViewModel ExportEventShareAttendViewStatistics(string type)
        {
            var monthes = Enumerable.Range(1, 12).Select(i => new { index = i, Month = DateTimeFormatInfo.CurrentInfo.GetAbbreviatedMonthName(i) });

            List<EventTracker> eventTrackers = authDBContext.EventTrackers.Include(q => q.User).Include(q => q.Event).Where(q => q.ActionType == type && q.User.Email.ToLower().Contains("@owner") == false && q.Date.Date.Year == DateTime.Now.Year && q.User.listoftags.Any() && q.User.birthdate != null && q.User.User.EmailConfirmed == true && (q.User.ProfileCompleted != null && q.User.ProfileCompleted == true)).ToList();

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

        private void AddNewWorkbookForStatisticsPerMonthAndDay(XLWorkbook workbook, ExportStatisticsByGenderAndAgeViewModel data, string sheetName, string ageHeadTitle, string genderHeadTitle)
        {

            IXLWorksheet newWorksheet = workbook.Worksheets.Add(sheetName);

            #region Export Per Month

            int currentRowByAge = 2;
            newWorksheet.Range(newWorksheet.Cell(1, 2), newWorksheet.Cell(1, 8)).Merge().Value = ageHeadTitle;
            newWorksheet.Range(newWorksheet.Cell(1, 2), newWorksheet.Cell(1, 8)).Style.Fill.SetBackgroundColor(XLColor.Gray);
            newWorksheet.Range(newWorksheet.Cell(1, 2), newWorksheet.Cell(1, 8)).Style.Font.SetBold().Font.FontSize = 14;
            newWorksheet.Cell(currentRowByAge, 2).Value = "Category";
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
            newWorksheet.Range(newWorksheet.Cell(1, 11), newWorksheet.Cell(1, 14)).Merge().Value = genderHeadTitle;
            newWorksheet.Range(newWorksheet.Cell(1, 11), newWorksheet.Cell(1, 14)).Style.Fill.SetBackgroundColor(XLColor.Gray);
            newWorksheet.Range(newWorksheet.Cell(1, 11), newWorksheet.Cell(1, 14)).Style.Font.SetBold().Font.FontSize = 14;
            newWorksheet.Cell(currentRowByGender, 11).Value = "Category";
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

            #endregion

            #region Export Per Day

            if (data.StatisticsByGenderPerDay.Count() != 0 && data.StatisticsByAgePerDay.Count() != 0)
            {

                int currentRowByAgePerDay = 2;
                newWorksheet.Range(newWorksheet.Cell(1, 18), newWorksheet.Cell(1, 24)).Merge().Value = $"{ageHeadTitle} Per Day";
                newWorksheet.Range(newWorksheet.Cell(1, 18), newWorksheet.Cell(1, 24)).Style.Fill.SetBackgroundColor(XLColor.Gray);
                newWorksheet.Range(newWorksheet.Cell(1, 18), newWorksheet.Cell(1, 24)).Style.Font.SetBold().Font.FontSize = 14;
                newWorksheet.Cell(currentRowByAgePerDay, 18).Value = "Category";
                newWorksheet.Cell(currentRowByAgePerDay, 19).Value = "From 18 To 24";
                newWorksheet.Cell(currentRowByAgePerDay, 20).Value = "From 25 To 34";
                newWorksheet.Cell(currentRowByAgePerDay, 21).Value = "From 35 To 44";
                newWorksheet.Cell(currentRowByAgePerDay, 22).Value = "From 45 To 54";
                newWorksheet.Cell(currentRowByAgePerDay, 23).Value = "From 55 To 64";
                newWorksheet.Cell(currentRowByAgePerDay, 24).Value = "More Than 65";

                foreach (StatisticsByAgeViewModel monthStatistic in data.StatisticsByAgePerDay)
                {
                    currentRowByAgePerDay++;
                    newWorksheet.Cell(currentRowByAgePerDay, 18).Value = monthStatistic.Date;
                    newWorksheet.Cell(currentRowByAgePerDay, 19).Value = monthStatistic.From18To24;
                    newWorksheet.Cell(currentRowByAgePerDay, 20).Value = monthStatistic.From25To34;
                    newWorksheet.Cell(currentRowByAgePerDay, 21).Value = monthStatistic.From35To44;
                    newWorksheet.Cell(currentRowByAgePerDay, 22).Value = monthStatistic.From45To54;
                    newWorksheet.Cell(currentRowByAgePerDay, 23).Value = monthStatistic.From55To64;
                    newWorksheet.Cell(currentRowByAgePerDay, 24).Value = monthStatistic.MoreThan65;
                }

                int currentRowByGenderPerDay = 2;
                newWorksheet.Range(newWorksheet.Cell(1, 27), newWorksheet.Cell(1, 30)).Merge().Value = $"{genderHeadTitle} Per Day";
                newWorksheet.Range(newWorksheet.Cell(1, 27), newWorksheet.Cell(1, 30)).Style.Fill.SetBackgroundColor(XLColor.Gray);
                newWorksheet.Range(newWorksheet.Cell(1, 27), newWorksheet.Cell(1, 30)).Style.Font.SetBold().Font.FontSize = 14;
                newWorksheet.Cell(currentRowByGenderPerDay, 27).Value = "Category";
                newWorksheet.Cell(currentRowByGenderPerDay, 28).Value = "Male";
                newWorksheet.Cell(currentRowByGenderPerDay, 29).Value = "Female";
                newWorksheet.Cell(currentRowByGenderPerDay, 30).Value = "Other";

                foreach (StatisticsByGenderViewModel monthStatistic in data.StatisticsByGenderPerDay)
                {
                    currentRowByGenderPerDay++;
                    newWorksheet.Cell(currentRowByGenderPerDay, 27).Value = monthStatistic.Date;
                    newWorksheet.Cell(currentRowByGenderPerDay, 28).Value = monthStatistic.Male;
                    newWorksheet.Cell(currentRowByGenderPerDay, 29).Value = monthStatistic.Female;
                    newWorksheet.Cell(currentRowByGenderPerDay, 30).Value = monthStatistic.Other;

                }
            }

            #endregion

        }

        private void AddNewWorkbook(XLWorkbook workbook, StatisticsByGenderAndAgeViewModel data, string sheetName, string ageHeadTitle, string genderHeadTitle)
        {

            IXLWorksheet newWorksheet = workbook.Worksheets.Add(sheetName);

            int currentRowByAge = 2;
            newWorksheet.Range(newWorksheet.Cell(1, 2), newWorksheet.Cell(1, 7)).Merge().Value = ageHeadTitle;
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
            newWorksheet.Range(newWorksheet.Cell(1, 10), newWorksheet.Cell(1, 13)).Merge().Value = genderHeadTitle;
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

        private void AddNewWorkbookForGenderOfUsers(XLWorkbook workbook, StatisticsByGenderAndAgeViewModel data, string sheetName, string genderHeadTitle)
        {

            IXLWorksheet newWorksheet = workbook.Worksheets.Add(sheetName);

            int currentRowByGender = 2;
            newWorksheet.Range(newWorksheet.Cell(1, 2), newWorksheet.Cell(1, 5)).Merge().Value = genderHeadTitle;
            newWorksheet.Range(newWorksheet.Cell(1, 2), newWorksheet.Cell(1, 5)).Style.Fill.SetBackgroundColor(XLColor.Gray);
            newWorksheet.Range(newWorksheet.Cell(1, 2), newWorksheet.Cell(1, 5)).Style.Font.SetBold().Font.FontSize = 14;
            newWorksheet.Cell(currentRowByGender, 2).Value = "Male";
            newWorksheet.Cell(currentRowByGender, 3).Value = "Female";
            newWorksheet.Cell(currentRowByGender, 4).Value = "Other";
            newWorksheet.Cell(currentRowByGender, 5).Value = "All";

            newWorksheet.Cell(3, 2).Value = data.Male;
            newWorksheet.Cell(3, 3).Value = data.Female;
            newWorksheet.Cell(3, 4).Value = data.Other;
            newWorksheet.Cell(3, 5).Value = data.All;

        }

        #endregion


        public async Task<IActionResult> Exportsatisticperview(int year)
        {

            var monthes = Enumerable.Range(1, 12).Select(i => new { index = i, Month = DateTimeFormatInfo.CurrentInfo.GetAbbreviatedMonthName(i) });

            List<EventTracker> eventTrackers = authDBContext.EventTrackers.Include(q => q.User).Include(q=> q.User.City).Include(q => q.Event)
                .Where(q => q.ActionType == "view" && q.User.Email.ToLower().Contains("@owner") == false 
                && q.Date.Date.Year == year && q.User.listoftags.Any() 
                
                
                && q.User.birthdate != null && q.User.User.EmailConfirmed == true &&
                (q.User.ProfileCompleted != null && q.User.ProfileCompleted == true)).ToList();


            List<EventTracker> groupedData = eventTrackers.GroupBy(q => q.UserId).Select(q => new EventTracker() {
                Id = q.First().Id,
                EventId = q.First().EventId,
                UserId = q.First().UserId,
                User = q.First().User,
                Event = q.First().Event,
                Date = q.First().Date
            }).Where(x=> x.User.Requestesfor.Count() > 0 && !string.IsNullOrEmpty(x.User.userName)).ToList();


            ExportStatisticsByGenderAndAgeViewModel eventTrackersPerMonth = new ExportStatisticsByGenderAndAgeViewModel();

            using (XLWorkbook workbook = new XLWorkbook())
            {
                IXLWorksheet worksheet = workbook.Worksheets.Add("Events View Statistics");


                int currentRowR1 = 2;
                worksheet.Range(worksheet.Cell(1, 2), worksheet.Cell(1, 7)).Merge().Value = "Events View Statistics";
                worksheet.Range(worksheet.Cell(1, 2), worksheet.Cell(1, 7)).Style.Fill.SetBackgroundColor(XLColor.Gray);
                worksheet.Range(worksheet.Cell(1, 2), worksheet.Cell(1, 7)).Style.Font.SetBold().Font.FontSize = 14;

                worksheet.Cell(currentRowR1, 2).Value = "User Name";
                worksheet.Cell(currentRowR1, 3).Value = "number of requests";
                worksheet.Cell(currentRowR1, 4).Value = "location";
                worksheet.Cell(currentRowR1, 5).Value = "email";
                worksheet.Cell(currentRowR1, 6).Value = "age";
                worksheet.Cell(currentRowR1, 7).Value = "gender";
                worksheet.Cell(currentRowR1, 8).Value = "Registration Date";
                

                try
                {
                    foreach (var month in groupedData)
                    {
                        currentRowR1++;
                        worksheet.Cell(currentRowR1, 2).Value = month.User.userName;
                        worksheet.Cell(currentRowR1, 3).Value = month.User.Requestesfor.Count();
                        worksheet.Cell(currentRowR1, 4).Value = month.User.City== null ? "" : month.User.City.DisplayName;
                        worksheet.Cell(currentRowR1, 5).Value = month.User.Email;
                        worksheet.Cell(currentRowR1, 6).Value =DateTime.Now.Year - month.User.birthdate.Value.Year;
                        worksheet.Cell(currentRowR1, 7).Value = month.User.Gender;
                        worksheet.Cell(currentRowR1, 8).Value = month.User.Requestesfor.FirstOrDefault(x=> x.UserRequestId == month.User.PrimaryId).regestdata.ToString("ddd/MM/yyyy");

                    }
                }
                catch (Exception ex)
                {

                    throw;
                }




                using (MemoryStream stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    byte[] content = stream.ToArray();

                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Filter Result.xlsx");
                }
            }
        }


        public async Task<IActionResult> Exportsatisticrequestsent()
        {
            try
            {
                List<Requestes> requestes = await authDBContext.Requestes.Include(q => q.User).ThenInclude(q => q.User).Where(q => q.User.Email.ToLower().Contains("@owner") == false && q.User.birthdate != null && q.User.Email != "dev@dev.com" && q.User.User.EmailConfirmed == true && (q.User.ProfileCompleted != null && q.User.ProfileCompleted == true) && q.User.listoftags.Any()).ToListAsync();

                List<Requestes> numberOfConnectionRequests = requestes.Where(q => !q.User.Email.ToLower().Contains("@owner") && q.User != null && q.User.User.EmailConfirmed == true && (q.User.ProfileCompleted != null && q.User.ProfileCompleted == true)).ToList();

                List<Requestes> reqts = await authDBContext.Requestes.Where(x => x.Id != 0).ToListAsync();

                List<RequestFilterModel> groupedData = numberOfConnectionRequests.GroupBy(q => q.UserId).Select(q => new RequestFilterModel()
                {
                    Id = q.First().Id,
                    EntityId = q.First().EntityId,
                    CityName = q.FirstOrDefault().User.City == null ? "" : q.FirstOrDefault().User.City.DisplayName,
                    //UserRequest = q.First().UserRequest,
                    UserId = q.First().UserId,
                    User = q.First().User,
                    birthdate = q.First().User.birthdate.Value,
                    regestrationdate= q.First().regestdata
                  

                }).Where(x => x.User.Requestesfor.Count() > 0 && !string.IsNullOrEmpty(x.User.userName)).ToList();



                using (XLWorkbook workbook = new XLWorkbook())
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Add("Number of Connection Request");


                    int currentRowR1 = 2;
                    worksheet.Range(worksheet.Cell(1, 2), worksheet.Cell(1, 7)).Merge().Value = "Number of Connection Request";
                    worksheet.Range(worksheet.Cell(1, 2), worksheet.Cell(1, 7)).Style.Fill.SetBackgroundColor(XLColor.Gray);
                    worksheet.Range(worksheet.Cell(1, 2), worksheet.Cell(1, 7)).Style.Font.SetBold().Font.FontSize = 14;

                    worksheet.Cell(currentRowR1, 2).Value = "User Name";
                    worksheet.Cell(currentRowR1, 3).Value = "number of requests";
                    worksheet.Cell(currentRowR1, 4).Value = "location";
                    worksheet.Cell(currentRowR1, 5).Value = "email";
                    worksheet.Cell(currentRowR1, 6).Value = "age";
                    worksheet.Cell(currentRowR1, 7).Value = "gender";
                    worksheet.Cell(currentRowR1, 8).Value = "Register Date";

                    


                    try
                    {
                        foreach (var month in groupedData)
                        {
                            currentRowR1++;
                            worksheet.Cell(currentRowR1, 2).Value = month.User.userName;
                            worksheet.Cell(currentRowR1, 3).Value = month.User.Requestesfor.Count();
                            worksheet.Cell(currentRowR1, 4).Value = month.CityName;
                            worksheet.Cell(currentRowR1, 5).Value = month.User.Email;
                            worksheet.Cell(currentRowR1, 6).Value = month.Age;
                            worksheet.Cell(currentRowR1, 7).Value = month.User.Gender;
                            worksheet.Cell(currentRowR1, 8).Value = month.regestrationdate.ToString("dd/MM/yyyy");

                        }
                    }
                    catch (Exception ex)
                    {

                        throw;
                    }




                    using (MemoryStream stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        byte[] content = stream.ToArray();

                        return File(
                            content,
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            "Filter Result.xlsx");
                    }
                }
            }
             
             catch (Exception ex)
            {

                throw;
            }
        }
    }

}
