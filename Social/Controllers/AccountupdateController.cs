using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Social.Entity.DBContext;
using Social.Entity.Enums;
using Social.Entity.Models;
using Social.Entity.ModelView;
using Social.FireBase;
using Social.Sercices.Helpers;
using Social.Services;
using Social.Services.FireBase_Helper;
using Social.Services.Helpers;
using Social.Services.Implementation;
using Social.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Social.Controllers
{
    [ApiController]
    [EnableCors("CorsPolicy")]
    [Route("api/")]
    public class AccountupdateController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        private readonly IConfiguration _configuration;
        private readonly IGlobalMethodsService globalMethodsService;
        private readonly IUserService _userService;
        private readonly EmailHelper _emailHelper;
        private readonly IErrorLogService _errorLogService;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly IFrindRequest _FrindRequest;
        private readonly AuthDBContext authDBContext;
        private readonly IEventServ _Event;
        readonly string BaseUrlDomain;
        private readonly IAppConfigrationService appConfigrationService;
        private readonly IFirebaseManager _firebaseManager;
        private readonly IMessageServes _MessageServes;
        public AccountupdateController(IAppConfigrationService appConfigrationService, IEventServ _Event, AuthDBContext authDBContext,
            IFrindRequest FrindRequest, UserManager<User> userManager, IConfiguration configuration, IGlobalMethodsService globalMethodsService,
            IUserService userService, EmailHelper emailHelper, IErrorLogService errorLogService, IStringLocalizer<SharedResource> localizer,
            IFirebaseManager firebaseManager, IMessageServes MessageServes)
        {
            this.appConfigrationService = appConfigrationService;
            this._Event = _Event;
            this.authDBContext = authDBContext;
            this.userManager = userManager;
            _configuration = configuration;
            this.globalMethodsService = globalMethodsService;
            _userService = userService;
            _emailHelper = emailHelper;
            _errorLogService = errorLogService;
            _FrindRequest = FrindRequest;
            _localizer = localizer;
            BaseUrlDomain = globalMethodsService.GetBaseDomain();
            _firebaseManager = firebaseManager;
            _MessageServes = MessageServes;
        }

        
        [HttpPost]
        [Route("Account/update")]
        public async Task<IActionResult> Update([FromForm] IFormFile UserImags, [FromForm] updateUserModel model)
        {
            StringValues authorizationToken;
            HttpContext.Request.Headers.TryGetValue("Authorization", out authorizationToken);
            var appcon = appConfigrationService.GetData().FirstOrDefault();
            var loggedinUser = await this._userService.GetLoggedInUser(authorizationToken);
            try
            {
                if (loggedinUser == null)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized,
                    new ResponseModel<object>(StatusCodes.Status401Unauthorized, false,
                    _localizer["NotLogin"], null));
                }
                if(!string.IsNullOrEmpty(model.UniversityCode))
                {
                    if(!(await GetAllWhiteLableCods()).Contains(model.UniversityCode))
                    {
                        return StatusCode(StatusCodes.Status406NotAcceptable,
                       new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,
                       _localizer["CodeNotCorrect"], null));
                    }
                }
                var user = await userManager.FindByIdAsync(loggedinUser.UserId);
                Random random1 = new Random();
                var code1 = random1.Next(000000, 9) + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Second;

                if (model.Username != null)
                {
                    user.DisplayedUserName = model.Username;
                    if (loggedinUser.User.UserDetails.birthdate == null)
                    {


                        user.UserName = (model.Username + code1).Replace(" ", "-");
                    }
                }
                if (model.Email != null)
                {
                    if (userManager.Users.Any(x => x.Id != user.Id && ((x.Email != null && x.Email.ToUpper() == model.Email.ToLower()) || (x.UserDetails != null && x.UserDetails.Email != null && x.UserDetails.Email.ToUpper() == model.Email.ToLower()))))
                    {
                        return StatusCode(StatusCodes.Status406NotAcceptable,
                            new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                             _localizer["Email already Registered For Other Account"], null));
                    }
                    user.Email = model.Email;
                }

                string imageName = null;
                var userDeatils = this._userService.GetUserDetails(user.Id);
                var GetLinkAccount = this._userService.GetallLinkAccount((userDeatils.PrimaryId));
                var Getalllistoftags = this._userService.Getalllistoftags((userDeatils.PrimaryId));
                this._userService.deleteLinkAccount(GetLinkAccount);
                //this._userService.Deletelistoftags(Getalllistoftags);
                //if (UserImags == null && (userDeatils.UserImage == null || userDeatils.UserImage == ""))
                //{
                //    return StatusCode(StatusCodes.Status406NotAcceptable,
                //        new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                //         _localizer["profile picture is required"], null));
                //}
                if (model.listoftags == null)
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                        new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                         _localizer["tags is required"], null));
                }
                if (model?.Gender?.ToLower() == "other" && string.IsNullOrEmpty(model?.OtherGenderName))
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                        new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                         _localizer["Please enter your gender"], null));
                }
                if (model?.Gender?.ToLower() != "other")
                {
                    model.OtherGenderName = null;
                }
                if (model.listoftags[0] == "[]" || model.listoftags[0] == null || model.listoftags[0] == "\"\"")
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                        new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                         _localizer["tags is required"], null));
                }
                if ((appcon != null ? (appcon.UserBio_MaxLength != null) : false) && model.bio != null)
                {


                    if (appcon.UserBio_MaxLength < model.bio.Length || appcon.UserBio_MinLength > model.bio.Length)
                    {
                        return StatusCode(StatusCodes.Status406NotAcceptable,
                            new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                             _localizer["User Bio Min Length is  "] + appcon.UserBio_MinLength + _localizer["  User Bio max Length is  "] + appcon.UserBio_MaxLength, null));
                    }
                }
                var x = model.listoftags[0].ToString();
                List<string> deserializedObject = JsonConvert.DeserializeObject<List<string>>(x);
                if (appcon != null ? (appcon.UserTagM_MaxNumber != null) : false)
                {
                    if (appcon.UserTagM_MinNumber > deserializedObject.Count() || appcon.UserTagM_MaxNumber < deserializedObject.Count())
                    {
                        string message = _localizer["NumberOfInterests"];
                        message = message.Replace("#", "");
                        return StatusCode(StatusCodes.Status406NotAcceptable, new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,message, null));
                    }
                }
                var xIam = model.Iam[0].ToString();

                List<string> deserializedObjectIam = JsonConvert.DeserializeObject<List<string>>(xIam);
                if (appcon != null ? (appcon.UserIAM_MinLength != null) : false)
                {


                    if (appcon.UserIAM_MinLength > deserializedObjectIam.Count() || appcon.UserIAM_MaxLength < deserializedObjectIam.Count())
                    {
                        return StatusCode(StatusCodes.Status406NotAcceptable,
                            new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                             _localizer["I AM Min  is  "] + appcon.UserIAM_MinLength + _localizer["  I AM max  is  "] + appcon.UserIAM_MaxLength, null));
                    }
                }
                var xpreferto = model.preferto[0].ToString();
                List<string> deserializedObjectpreferto = JsonConvert.DeserializeObject<List<string>>(xpreferto);
                if (appcon != null ? (appcon.UserIPreferTo_MaxLength != null) : false)
                {


                    if (appcon.UserIPreferTo_MinLength > deserializedObjectpreferto.Count() || appcon.UserIPreferTo_MaxLength < deserializedObjectpreferto.Count())
                    {
                        return StatusCode(StatusCodes.Status406NotAcceptable,
                            new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                             _localizer["  I Prefer To Min is  "] + appcon.UserIPreferTo_MinLength + _localizer["    I Prefer To max is  "] + appcon.UserIPreferTo_MaxLength, null));
                    }
                }
                if (appcon != null ? (appcon.UserMinAge != null) : false)
                {

                    var years = DateTime.Now.Date.Year - model.birthdate.Year;
                    if (appcon.UserMinAge > years || appcon.UserMaxAge < years)
                    {
                        return StatusCode(StatusCodes.Status406NotAcceptable,
                            new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                             _localizer["User Min Age is  "] + appcon.UserMinAge + _localizer["  User max Age is  "] + appcon.UserMaxAge, null));
                    }
                }

                if (UserImags != null)
                {

                    globalMethodsService.DeleteFiles(userDeatils.UserImage, "");

                    var UniqName = await globalMethodsService.uploadFileAsync("/Images/Userprofile/", UserImags);
                    imageName = "/Images/Userprofile/" + UniqName;

                    userDeatils.UserImage = imageName;
                }

                if (model.ImageIsVerified != null)
                {
                    userDeatils.ImageIsVerified = model.ImageIsVerified;
                }
                userDeatils.Gender = model.Gender;
                userDeatils.bio = model.bio;
               
                //userDeatils.Email = model.Email;
                userDeatils.userName = model.Username;
                userDeatils.birthdate = Convert.ToDateTime(model.birthdate);
                //userDeatils.Facebook = model.Facebook;
                //userDeatils.tiktok = model.tiktok;
                userDeatils.OtherGenderName = model.OtherGenderName;
                // userDeatils.instagram = model.instagram;
                // userDeatils.snapchat = model.snapchat;
                userDeatils.allowmylocation = true;
                // userDeatils.pushnotification = true;
                userDeatils.whatAmILookingFor = model.whatAmILookingFor;
                userDeatils.personalSpace = model.personalSpace;
                userDeatils.Code = model.UniversityCode;
                if (model.listoftags != null)
                {
                    if (model.listoftags.Count() > 0 && model.listoftags[0] != "[]")
                    {
                        authDBContext.listoftags.RemoveRange(userDeatils.listoftags);

                        var t = deserializedObject[0];
                        foreach (var a in deserializedObject)
                        {
                            try
                            {
                                listoftags tage = new listoftags();
                                tage.InterestsId = _Event.getInterests(a).Id;
                                tage.UserId = userDeatils.PrimaryId;
                                this._userService.addlistoftags(tage);
                            }
                            catch
                            { continue; }
                        }

                    }
                }
                if (model.Iam != null)
                {
                    if (model.Iam.Count() > 0 && model.Iam[0] != "[]")
                    {
                        authDBContext.WhatBestDescripsMeList.RemoveRange(userDeatils.WhatBestDescripsMeList);

                        var t = deserializedObjectIam[0];
                        foreach (var a in deserializedObjectIam)
                        {
                            try
                            {
                                WhatBestDescripsMeList tage = new WhatBestDescripsMeList();
                                tage.WhatBestDescripsMeId = _Event.getWhatBestDescripsMe(a).Id;
                                tage.UserId = userDeatils.PrimaryId;
                                this._userService.addWhatBestDescripsMe(tage);
                            }
                            catch
                            { continue; }
                        }

                    }
                }
                if (model.preferto != null)
                {
                    if (model.preferto.Count() > 0 && model.preferto[0] != "[]")
                    {
                        authDBContext.Iprefertolist.RemoveRange(userDeatils.Iprefertolist);

                        foreach (var a in deserializedObjectpreferto)
                        {
                            try
                            {
                                Iprefertolist tage = new Iprefertolist();
                                tage.prefertoId = _Event.getpreferto(a).Id;
                                tage.UserId = userDeatils.PrimaryId;
                                this._userService.addIprefertolist(tage);
                            }
                            catch
                            { continue; }
                        }

                    }
                }

                if (userDeatils.birthdate != null && userDeatils.Gender != null && userDeatils.Iprefertolist != null && userDeatils.Iprefertolist.Count() != 0)
                {
                    userDeatils.ProfileCompleted = true;
                }

                this._userService.UpdateUserDetails(userDeatils);

                var result = await userManager.UpdateAsync(user);


                if (!result.Succeeded)
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                        new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,
                        result.Errors.FirstOrDefault().Description, null));
                }
                updateUserModelview modl = new updateUserModelview();
                modl.UserName = user.DisplayedUserName;
                modl.Gender = userDeatils.Gender;
                modl.OtherGenderName = userDeatils.OtherGenderName;
                modl.bio = userDeatils.bio;
                modl.lat = userDeatils.lat;
                modl.lang = userDeatils.lang;
                modl.birthdate = userDeatils.birthdate.Value.ConvertDateTimeToString();
                modl.Email = user.Email;
                modl.age = DateTime.Now.Date.Year - (userDeatils.birthdate == null ? DateTime.Now.Date.Year : userDeatils.birthdate.Value.Year);
                //modl.Facebook = userDeatils.Facebook;
                //modl.instagram = userDeatils.instagram;
                //  modl.tiktok = userDeatils.tiktok;
                //  modl.snapchat = userDeatils.snapchat;
                modl.pushnotification = userDeatils.pushnotification;
                modl.allowmylocation = userDeatils.allowmylocation;
                modl.whatAmILookingFor = userDeatils.whatAmILookingFor;
                modl.ghostmode = userDeatils.ghostmode;
                modl.Filteringaccordingtoage = userDeatils.Filteringaccordingtoage;
                modl.personalSpace = userDeatils.personalSpace;
                modl.agefrom = userDeatils.agefrom;
                modl.ageto = userDeatils.ageto;
                modl.distanceFilter = userDeatils.distanceFilter;
                modl.UserImage = BaseUrlDomain + user.UserDetails.UserImage;
                modl.UniversityCode = userDeatils.Code;
                var GetLinkAccount2 = this._userService.GetallLinkAccount((userDeatils.PrimaryId));
                var Getalllistoftags2 = this._userService.Getalllistoftags((userDeatils.PrimaryId));
                var WhatBestDescripsMeList = this._userService.GetallWhatBestDescripsMeList((userDeatils.PrimaryId));
                List<listoftagsmodel> list2 = new List<listoftagsmodel>();

                foreach (var item in Getalllistoftags2)
                {
                    listoftagsmodel LinkAccountmodel = new listoftagsmodel();
                    LinkAccountmodel.tagname = item.Interests.name;
                    LinkAccountmodel.tagID = item.Interests.EntityId;
                    list2.Add(LinkAccountmodel);
                }
                List<listoftagsmodel> WhatBestDescripsMeListdata = new List<listoftagsmodel>();

                foreach (var item in WhatBestDescripsMeList)
                {
                    listoftagsmodel LinkAccountmodel = new listoftagsmodel();
                    LinkAccountmodel.tagname = item.WhatBestDescripsMe.name;
                    LinkAccountmodel.tagID = item.WhatBestDescripsMe.EntityId;
                    WhatBestDescripsMeListdata.Add(LinkAccountmodel);
                }
                List<listoftagsmodel> preferto = new List<listoftagsmodel>();
                var prefertolist = this._userService.GetallIprefertolist((userDeatils.PrimaryId));
                foreach (var item in prefertolist)
                {
                    listoftagsmodel LinkAccountmodel = new listoftagsmodel();
                    LinkAccountmodel.tagname = item.preferto.name;
                    LinkAccountmodel.tagID = item.preferto.EntityId;
                    preferto.Add(LinkAccountmodel);
                }
                modl.prefertoList = preferto.Distinct().ToList();
                modl.listoftagsmodel = list2.Distinct().ToList();
                modl.IamList = WhatBestDescripsMeListdata.Distinct().ToList();
                modl.DisplayedUserName = user.UserName.ToString();
                if(!string.IsNullOrWhiteSpace(user.UserDetails.Code))
                {
                    await this.SubscribeInChatCommunity(user);
                }
                
                return StatusCode(StatusCodes.Status200OK,
                      new ResponseModel<object>(StatusCodes.Status200OK, true,
                     _localizer["updateprofiledata"], new
                     {
                         modl.UserName,
                         modl.Gender,
                         modl.OtherGenderName,
                         modl.bio,
                         modl.lat,
                         modl.lang,
                         modl.birthdate,
                         modl.Email,
                         modl.age,
                         modl.UserImage,
                         modl.prefertoList,
                         modl.listoftagsmodel,
                         modl.IamList,
                         modl.DisplayedUserName,
                         modl.UniversityCode
                     }));
            }
            catch (Exception ex)
            {
                if (ex.Message == "Not Accepted File Extention")
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                      new ResponseModel<object>(StatusCodes.Status406NotAcceptable, true,
                     ex.Message, null));
                }
                await _errorLogService.InsertErrorLog(new BWErrorLog(HttpContext.GetUser().UserId, "Account/update", ex));
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<object>(StatusCodes.Status500InternalServerError, false, ex.Message, null));
            }
        }
        /// <summary>
        /// Add User Images Used In User Profile. (JWT Token)
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Account/UserImages")]
        public async Task<IActionResult> UploadUserImages([FromForm] IFormFileCollection files)
        { 
            var userDetails = await GetUserDetails();
            if (userDetails == null)
            {
                return StatusCode(StatusCodes.Status401Unauthorized,
                    new ResponseModel<object>(StatusCodes.Status401Unauthorized, false,
                        _localizer["NotLogin"], null));
            }
            var userImages = new List <UserImage>();
            if (files != null && files.Count > 0 && files.Count <=5 )
            {

                foreach (var file in files)
                {
                    var fileName = await globalMethodsService.uploadFileAsync("/Images/Userprofile/", file);
                    var imageUrl = "/Images/Userprofile/" + fileName;
                    var userImage = new UserImage
                    {
                        ImageUrl = imageUrl,
                        UserDetailsId = userDetails.PrimaryId,
                        UserId = userDetails.UserId
                    };
                    userImages.Add(userImage);
                }

                var result =  _userService.AddUserImages(userImages);
                return result
                    ? StatusCode(StatusCodes.Status200OK,
                        new ResponseModel<object>(StatusCodes.Status200OK, true,
                            _localizer["UpdateUserImages"], true))
                    : StatusCode(StatusCodes.Status406NotAcceptable,
                        new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,
                            _localizer["FailUpdateUserImages"], false));
            }
            return StatusCode(StatusCodes.Status406NotAcceptable,
                new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,
                    _localizer["FailUpdateUserImages"], false));

        }
        /// <summary>
        /// Update User Images Used In User Profile. (JWT Token)
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Account/UpdateUserImages")]
        public async Task<IActionResult> UpdateUserImages([FromForm] IFormFileCollection files)
        {
            var userDetails = await GetUserDetails();
            if (userDetails == null)
            {
                return StatusCode(StatusCodes.Status401Unauthorized,
                    new ResponseModel<object>(StatusCodes.Status401Unauthorized, false,
                        _localizer["NotLogin"], null));
            }
            var userImages = new List<UserImage>();
            if (files != null && files.Count <= 5)
            {
                var oldUserImages = _userService.GetUserImages(userDetails.PrimaryId);
                if (oldUserImages.Any())
                {
                    _userService.DeleteUserImages(oldUserImages);
                    foreach (var image in oldUserImages)
                    {
                        globalMethodsService.DeleteFiles(image.ImageUrl, "");

                    }
                }

                if (files.Count == 0)
                {
                    return StatusCode(StatusCodes.Status200OK,
                        new ResponseModel<object>(StatusCodes.Status200OK, true,
                            _localizer["UpdateUserImages"], true));
                }
                foreach (var file in files)
                {
                    var fileName = await globalMethodsService.uploadFileAsync("/Images/Userprofile/", file);
                    var imageUrl = "/Images/Userprofile/" + fileName;
                    var userImage = new UserImage
                    {
                        ImageUrl = imageUrl,
                        UserDetailsId = userDetails.PrimaryId,
                        UserId = userDetails.UserId
                    };
                    userImages.Add(userImage);
                }

                var result = _userService.AddUserImages(userImages);
                return result
                    ? StatusCode(StatusCodes.Status200OK,
                        new ResponseModel<object>(StatusCodes.Status200OK, true,
                            _localizer["UpdateUserImages"], true))
                    : StatusCode(StatusCodes.Status406NotAcceptable,
                        new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,
                            _localizer["FailUpdateUserImages"], false));
            }
            return StatusCode(StatusCodes.Status406NotAcceptable,
                new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,
                    _localizer["FailUpdateUserImages"], false));

        }

        private async Task<UserDetails> GetUserDetails()
        {
            HttpContext.Request.Headers.TryGetValue("Authorization", out var authorizationToken);
            var loggedInUser = await _userService.GetLoggedInUser(authorizationToken);
            if (loggedInUser != null)
            {
                var user = await userManager.FindByIdAsync(loggedInUser.UserId);
                return _userService.GetUserDetails(user.Id);
            }

            return null;
        }
        private async Task SubscribeInChatCommunity(User user)
        {
            var whitelableUser = await authDBContext.UserDetails.FirstOrDefaultAsync(u => u.Code == user.UserDetails.Code && u.IsWhiteLabel.Value);
            var chatgroup = authDBContext.ChatGroups.FirstOrDefault(c => c.UserID == whitelableUser.UserId);
            var communityUserId = authDBContext.ChatGroupSubscribers.Include(c=>c.ChatGroup).Where(cg=>cg.ChatGroup.UserID== whitelableUser.UserId)
                .Select(cg=>cg.UserID).FirstOrDefault(u=>u==user.UserDetails.UserId);
            if (chatgroup != null && communityUserId == null)
            {
                
                var newSubscriber = new ChatGroupSubscribers
                {
                    JoinDateTime = DateTime.Now,
                    ChatGroupID = chatgroup.ID,
                    UserID = user.UserDetails.UserId,
                    LeaveGroup = ChatGroupSubscriberStatus.Joined,
                    IsAdminGroup = ChatGroupSubscriberType.Member,
                    IsMuted = false,
                    LeaveDateTime = null,
                    RemovedDateTime = null,
                };

                authDBContext.ChatGroupSubscribers.Add(newSubscriber);
                await authDBContext.SaveChangesAsync();
            }
        }

        [HttpPost]
        [Route("externaleventEvent/AddEventData")]
        public async Task<IActionResult> ADD([FromForm] IFormFile Eventimage, [FromForm] string categoryid, [FromForm] EventData model)
        {
            try
            {


                var dataconfig = appConfigrationService.GetData().FirstOrDefault();
                var loggedinUser = HttpContext.GetUser();


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
                    if (model.Title.Length >= dataconfig.EventTitle_MinLength && model.Title.Length >= dataconfig.EventTitle_MaxLength)
                    {
                        return StatusCode(StatusCodes.Status406NotAcceptable,
                            new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                                      _localizer["Event  Title Min Length is "] + dataconfig.EventTitle_MinLength + _localizer["Event Title Max Length is "] + dataconfig.EventTitle_MinLength, null));

                    }
                }
                if (dataconfig.EventDetailsDescription_MaxLength != null)
                {
                    if (model.Title.Length >= dataconfig.EventDetailsDescription_MinLength && model.Title.Length >= dataconfig.EventDetailsDescription_MaxLength)
                    {
                        return StatusCode(StatusCodes.Status406NotAcceptable,
                            new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                             _localizer["Event Details Description Min Length is "] + dataconfig.EventDetailsDescription_MinLength + _localizer["Event Details Description Max Length is "] + dataconfig.EventDetailsDescription_MaxLength, null));
                    }
                }
                if (model.description.Length > 150)
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                        new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                         _localizer["descriptionismustNotmorethan150characters."], null));
                }
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



                // var listdata = _FrindRequest.GetallFrendes(UserId.PrimaryId, null);


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


                model.UserId = loggedinUser.User.UserDetails.PrimaryId;
                model.categorieId = _Event.getcategory().Where(n => n.EntityId == categoryid).FirstOrDefault().Id;
                model.EntityId = Guid.NewGuid().ToString();
                model.EventTypeListid = 3;
                var cuserount = await _Event.InsertEvent(model);
                if (cuserount != null)
                {
                    //eventattend eventattend = new eventattend();
                    //eventattend.EventDataid = model.Id;
                    //eventattend.UserattendId = UserId.PrimaryId;
                    //eventattend.JoinDate = CreatDate;

                    var JoinTimeForMessage = (DateTime.Now.TimeOfDay);
                    var a = await _Event.InsertEventChatAttend(new EventChatAttend { Jointime = DateTime.Now.TimeOfDay, EventDataid = model.Id, UserattendId = loggedinUser.User.UserDetails.PrimaryId, JoinDate = DateTime.Now.Date, ISAdmin = true });

                    //await MessageServes.addeventmessage(new EventMessageDTO { EventChatAttendid = a.Id, eventjoin = false, Message = "", Messagetype = 1, EventId = model.EntityId, Messagesdate = CreatDate.Value, Messagestime = JoinTimeForMessage.Value }, loggedinUser.User.UserDetails);


                    //{
                    //    var allblock = this._authContext.Requestes;

                    //    var alluse = _userService.allusersaroundevent(Convert.ToDouble(model.lat), Convert.ToDouble(model.lang)).ToList();
                    //    foreach (var item in alluse)
                    //    {
                    //        try
                    //        {
                    //            int userid = UserId.PrimaryId;
                    //            if (item.PrimaryId != userid)
                    //            {

                    //                var blockod = allblock.Where(m => ((m.UserId == userid && m.UserRequestId == item.PrimaryId) || (m.UserId == item.PrimaryId && m.UserRequestId == userid)) && m.status == 2).ToList();
                    //                if (blockod != null)
                    //                {
                    //                    FireBaseData fireBaseInfo = new FireBaseData() { Title = model.Title, Body = "Check this hot event near you!", imageUrl = _configuration["BaseUrl"] + model.image, Action_code = model.EntityId, muit = false, Action = "Check_events_near_you" };
                    //                    var addnoti = MessageServes.getFireBaseData(item.PrimaryId, fireBaseInfo, CreatDate, Creattime);
                    //                    await MessageServes.addFireBaseDatamodel(addnoti);
                    //                    SendNotificationcs sendNotificationcs = new SendNotificationcs();
                    //                    if (item.FcmToken != null)
                    //                        await firebaseManager.SendNotification(item.FcmToken, fireBaseInfo);
                    //                    //await sendNotificationcs.SendMessageAsync(item.FcmToken, "Check_events_near_you", fireBaseInfo, _environment.WebRootPath);
                    //                }
                    //            }
                    //        }
                    //        catch
                    //        {
                    //            continue;
                    //        }
                    //    }
                    //}

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
                              eventtypeid = "",
                              eventtypecolor = "",
                              eventtype = "external",
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
                                  JoinDate = m.JoinDate == null ? DateTime.Now.Date.ConvertDateTimeToString() : m.JoinDate.Value.Date.ConvertDateTimeToString(),
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

        private async Task<List<string>> GetAllWhiteLableCods()
        {
            var data=authDBContext.UserDetails.ToList().Where(u => u.IsWhiteLabel.Value).Select(u => u.Code).ToList();
            return authDBContext.UserDetails.ToList().Where(u=>u.IsWhiteLabel.Value).Select(u => u.Code).ToList();
        }
        [HttpPost]
        public async Task SendUpdateProfileNotification()
        {            
            var users = authDBContext.UserDetails.Where(u => u.birthdate == null && u.User.EmailConfirmed
            && (EF.Functions.DateDiffDay(u.User.EmailConfirmedOn,DateTime.Today) == 1 || EF.Functions.DateDiffDay(u.User.EmailConfirmedOn, DateTime.Today) == 4)).ToList();
               await this.SendNotification(users);
        }

        private async Task SendNotification(List<UserDetails> users)
            {
            var body = "You're almost there!@Complete your profile to start connecting on Friendzr today";
             body = body.Replace("@", System.Environment.NewLine);
            foreach (var user in users)
            {
                FireBaseData fireBaseInfo = new FireBaseData()
                {
                    muit = false,
                    Title = "Friendzr",
                    imageUrl = _configuration["BaseUrl"] + user.UserImage,
                    Body = body,
                    Action_code = user.UserId,
                    Action = "Update_Profile"
                };
                try
                {
                    SendNotificationcs sendNotificationcs = new SendNotificationcs();
                    if (user.FcmToken != null)
                    {
                        bool result = await _firebaseManager.SendNotification(user.FcmToken, fireBaseInfo);

                        var messageSent = _MessageServes.getFireBaseData(user.PrimaryId, fireBaseInfo, DateTime.Today);
                        await _MessageServes.addFireBaseDatamodel(messageSent);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

        }
    }
}
