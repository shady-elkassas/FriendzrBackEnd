using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Social.Entity.DBContext;
using Social.Entity.ModelView;
using Social.Services;
using Social.Services.Attributes;
using Social.Services.FireBase_Helper;
using Social.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Social.Areas.Admin.Controllers
{
    [Area("Admin")]
    [ServiceFilter(typeof(AuthorizeUser))]

    public class UsersAdminNotificationController : Controller
    {
        private readonly IUserService userService;
        private readonly ICountryService countryService;
        private readonly IMessageServes messageServes;
        private readonly IGlobalMethodsService globalMethodsService;
        private readonly IFirebaseManager firebaseManager;
        private readonly ICityService cityService;
        private readonly AuthDBContext authDBContext;
        private readonly IStringLocalizer<SharedResource> localizer;
        private readonly IConfiguration _configuration;
        public UsersAdminNotificationController(IConfiguration configuration,
            IUserService userService,
            ICountryService countryService,
            IMessageServes MessageServes,
            IGlobalMethodsService globalMethodsService,
            IFirebaseManager firebaseManager,
            ICityService cityService,
            AuthDBContext authDBContext, IStringLocalizer<SharedResource> _localizer)
        {
            this.userService = userService;
            this.countryService = countryService;
            messageServes = MessageServes;
            this.globalMethodsService = globalMethodsService;
            this.firebaseManager = firebaseManager;
            this.cityService = cityService;
            this.authDBContext = authDBContext;
            localizer = _localizer;
            _configuration = configuration;
        }
        public IActionResult Index()
        {

            return View();
        }
        public IActionResult AdminNotification()
        {
            return View();
        }

        public IActionResult Search()
        {
            ViewBag.Countries = (countryService.GetData()).Select(x => new SelectListItem { Value = x.ID.ToString(), Text = x.DisplayName });
            ViewBag.Cities = (cityService.GetData()).Select(x => new SelectListItem { Value = x.ID.ToString(), Text = x.DisplayName });
            return View();
        }
        public IActionResult UserDetails(string UserID)
        {
            var user = authDBContext.Users.Find(UserID);
            if (user == null)
                return NotFound();
            return View(user);
        }
        [HttpPost]
        public IActionResult AllowAdsForAll(bool AllowAds)
        {
           var allusers= authDBContext.UserDetails.Where(x => x.AllowAds == !AllowAds);
            allusers.ToList().ForEach(x => x.AllowAds = AllowAds);
            authDBContext.UpdateRange(allusers);
            authDBContext.SaveChanges();
           
            var Result = CommonResponse<object>.GetResult(200, true, localizer["UpdatedSuccessfully"]);
            return Ok(JObject.FromObject(Result, new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));
        } [HttpPost]
        public IActionResult changeAllowAdsStatus(string ID, bool AllowAds)
        {

            var UserDetails = userService.GetUserDetails(ID);
            UserDetails.AllowAds = AllowAds;
            userService.UpdateUserDetails(UserDetails);
           
            var Result = CommonResponse<object>.GetResult(200, true, localizer["UpdatedSuccessfully"]);
            return Ok(JObject.FromObject(Result, new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));
        }  
        [HttpPost]
        public async Task<IActionResult> changeStatus(string ID, bool IsActive)
        {
            var UserDetails = userService.GetUserDetails(ID);
            UserDetails.IsActive = IsActive;
            userService.UpdateUserDetails(UserDetails);
            if (IsActive == false)
            {
                var getloggedinuser = await userService.GetLoggedInUsers(UserDetails.Id);
                foreach (var item in getloggedinuser)
                {
                    await userService.DeleteLoggedInUser(item);
                }
            }
            var Result = CommonResponse<object>.GetResult(200, true, localizer["UpdatedSuccessfully"]);
            return Ok(JObject.FromObject(Result, new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));
        }
        [HttpPost]
        public IActionResult BanUser(string ID, DateTime BanFrom, DateTime BanTo)
        {
            var UserDetails = userService.GetUserDetails(ID);
            UserDetails.BanFrom = BanFrom;
            UserDetails.BanTo = BanTo;
            userService.UpdateUserDetails(UserDetails);
            var Result = CommonResponse<object>.GetResult(200, true, localizer["UpdatedSuccessfully"]);
            return Ok(JObject.FromObject(Result, new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));
        }
        [HttpPost]
        public IActionResult RemoveBanUser(string ID, DateTime StopFrom, DateTime StopTo)
        {
            var UserDetails = userService.GetUserDetails(ID);
            UserDetails.BanFrom = StopFrom;
            UserDetails.BanTo = StopTo;
            userService.UpdateUserDetails(UserDetails);
            var Result = CommonResponse<object>.GetResult(200, true, localizer["UpdatedSuccessfully"]);
            return Ok(JObject.FromObject(Result, new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));
        }
        [HttpPost]
        public async Task<IActionResult> RemoveObj(string ID)
        {

         var   data = authDBContext.FireBaseDatamodel.Where(x => x.id == ID).FirstOrDefault();
            authDBContext.FireBaseDatamodel.Remove(data);
            authDBContext.SaveChanges();
            var Result = CommonResponse<object>.GetResult(200, true, localizer["UpdatedSuccessfully"]);
            return Ok(JObject.FromObject(Result, new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));
        }
        public IActionResult GetAll()
        {
            var Result = new
            {
                data = userService.getallUserDetails().Where(b => b.User.Email != "Owner@Owner.com").OrderByDescending(m => m.User.RegistrationDate).Select(x => new
                {
                    UserName = x.User.DisplayedUserName,
                    ID = x.UserId,
                    Email = x.User.Email,
                    UserImage = x.UserImage,
                    Gender = x.Gender,
                    AllowAds=x.AllowAds,
                    IsActive = x.IsActive,
                    RegistrationDate = x.User.RegistrationDate.ToShortDateString(),

                })
            };
            return Ok(JObject.FromObject(Result, new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));



        }
        [HttpPost]
        public async Task<IActionResult> SendNotification(List<string> Tokens,List<int> UserPrimaryIds, string NotificationBody, string NotificationTitle, IFormFile NotificationImage)
        {
            if (NotificationImage != null && !NotificationImage.ContentType.ToLower().Contains("image"))
            {
                return Ok(JObject.FromObject(CommonResponse<object>.GetResult(403, false,localizer["OnlyAllowImages"]), new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));
            }
            var imageUrl =   await globalMethodsService.uploadFileAsync("/Images/AdminNotification/", NotificationImage);
            var FirebaseDataModel = new FireBaseData()
            {
                Title = NotificationTitle/* localizer["AppNotification"]*/,
                Body = NotificationBody,
                imageUrl = string.IsNullOrEmpty(imageUrl) ? null : globalMethodsService.GetBaseDomain() + "/Images/AdminNotification/" + imageUrl
            };
            await firebaseManager.SendNotification(Tokens.Where(x => string.IsNullOrEmpty(x) == false).ToList(), FirebaseDataModel);
            var FirebaseDataModels = UserPrimaryIds.Select(x => messageServes.getFireBaseData(x, FirebaseDataModel,null,null,true)).ToList() ;
            await messageServes.addFireBaseDatamodel(FirebaseDataModels);
            var Result = CommonResponse<object>.GetResult(200, true, localizer["SuccessfulSent"]);
            return Ok(JObject.FromObject(Result, new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));
        }
        [HttpPost]
        public async Task<IActionResult> ResentNotifications(List<string> IDs)
        {
            foreach (var item in authDBContext.FireBaseDatamodel.Where(x => IDs.Contains(x.id)))
            {
                var FibaseDataMoal = new FireBaseData
                {
                    Title = item.Title,
                    Body = item.Body,
                    imageUrl = item.imageUrl,
                    Action = item.Action,
                };
                var Token = item.User.FcmToken;
                await firebaseManager.SendNotification(Token, FibaseDataMoal);

            }
            var Result = CommonResponse<object>.GetResult(200, true, localizer["SuccessfulSent"]);
            return Ok(JObject.FromObject(Result, new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));
        }
        public IActionResult SearchInUsers(int? CountryID, int? CityID)
        {
            var Result = new
            {
                data = userService.getallUserDetails().Where(b => b.User.Email != "Owner@Owner.com").Where(x => (CountryID == null || x.CountryID == CountryID) && (CityID == null || x.CityID == CityID)).OrderByDescending(m => m.User.RegistrationDate).Select(x => new
                {
                    UserName = x.User.DisplayedUserName,
                    ID = x.UserId,
                    PrimaryID=x.PrimaryId,
                    FcmToken = x.FcmToken,
                    CityName = x.City == null ? "" : x.City.DisplayName,
                    CountryName = x.Country == null ? "" : x.Country.DisplayName,
                    Email = x.User.Email,
                    IsActive = x.IsActive,
                    RegistrationDate = x.User.RegistrationDate.ToShortDateString(),
                })
            };
            return Ok(JObject.FromObject(Result, new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));
        }
        public IActionResult GetAllNotificationSentByAdmin()
        {
            var Result = new
            {
                data = authDBContext.FireBaseDatamodel.Where(x => x.IsCreatedByAdmin == true).OrderByDescending(x=>x.CreatedAt).ToList().Select(x => new {
                    Body = x.Body,
                    Title = x.Title,
                    ImageURl = x.imageUrl,
                    NotifiyedUserName = x.User.User.DisplayedUserName,
                    NotifiyedUserEmail = x.User.User.Email,
                    NotifiyedUserImageURl = x.User.UserImage == null ? "/assets/media/avatars/blank.png" : globalMethodsService.GetBaseDomain() + x.User.UserImage,
                    Date = x.CreatedAt.ToString("dddd, dd MMMM yyyy HH:mm:ss"),
                    ID=x.id
                })
            };
            return Ok(JObject.FromObject(Result, new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));
        }
    }
}
