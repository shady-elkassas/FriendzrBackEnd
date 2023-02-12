using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Social.Entity.DBContext;
using Social.Entity.Enums;
using Social.Entity.Models;
using Social.Entity.ModelView;
using Social.FireBase;
using Social.Sercices.Helpers;
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

namespace Social.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(AuthorizeAdmin))]
    public class AccountController : ControllerBase
    {
        //private readonly HttpContext _httpContext;
        readonly string BaseUrlDomain;
        private readonly IEventServ _Event;
        private readonly EmailHelper _emailHelper;
        private readonly IUserService userService;
        private readonly IFrindRequest _FrindRequest;
        private readonly AuthDBContext authDBContext;
        private readonly IMessageServes messageServes;
        private readonly UserManager<User> userManager;
        private readonly IFirebaseManager firebaseManager;
        private readonly IErrorLogService _errorLogService;
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly IGlobalMethodsService globalMethodsService;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly IAppConfigrationService appConfigrationService;
        private readonly IDistanceFilterHistoryService distanceFilterHistoryService;
        private readonly IFilteringAccordingToAgeHistoryService filteringAccordingToAgeHistoryService;
        public AccountController(IGlobalMethodsService globalMethodsService, IFilteringAccordingToAgeHistoryService filteringAccordingToAgeHistoryService, IDistanceFilterHistoryService distanceFilterHistoryService, IAppConfigrationService appConfigrationService, UserManager<User> userManager, RoleManager<ApplicationRole> roleManager, IFirebaseManager firebaseManager, IStringLocalizer<SharedResource> localizer, AuthDBContext authDBContext, IMessageServes messageServes, IEventServ Event, IUserService userService, EmailHelper emailHelper, IErrorLogService errorLogService, IFrindRequest _FrindRequest)
        {
            this._Event = Event;
            _localizer = localizer;
            _emailHelper = emailHelper;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.userService = userService;
            this._FrindRequest = _FrindRequest;
            this.authDBContext = authDBContext;
            _errorLogService = errorLogService;
            this.messageServes = messageServes;
            this.firebaseManager = firebaseManager;
            this.globalMethodsService = globalMethodsService;
            BaseUrlDomain = globalMethodsService.GetBaseDomain();
            this.appConfigrationService = appConfigrationService;
            this.distanceFilterHistoryService = distanceFilterHistoryService;
            this.filteringAccordingToAgeHistoryService = filteringAccordingToAgeHistoryService;
        }

        [HttpGet]
        [Route("test")]
        public async Task<IActionResult> test()
        {
            try
            {

                return StatusCode(StatusCodes.Status200OK,
                      new ResponseModel<object>(StatusCodes.Status200OK, true,
                       _localizer["updateprofiledata"], null));

            }
            catch (Exception ex)
            {
                await _errorLogService.InsertErrorLog(new BWErrorLog(HttpContext.GetUser().UserId, "Account/Logout", ex));
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<object>(StatusCodes.Status500InternalServerError, false, ex.Message, null));

            }
        }

        [HttpPost]
        [Route("LinkClick")]
        public async Task<IActionResult> LinkClick([FromForm] string key)
        {
            try
            {
                LoggedinUser loggedinUser = HttpContext.GetUser();
                return StatusCode(StatusCodes.Status200OK,
                     new ResponseModel<object>(StatusCodes.Status200OK, true,
                      _localizer["Click Saved"], await userService.LinkClicks(loggedinUser, key)));

            }
            catch (Exception ex)
            {
                await _errorLogService.InsertErrorLog(new BWErrorLog(HttpContext.GetUser().UserId, "Account/Logout", ex));
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<object>(StatusCodes.Status500InternalServerError, false, ex.Message, null));

            }
        }

        [HttpPost]
        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var loggedinUser = HttpContext.GetUser();
                // delete token
                await this.userService.DeleteLoggedInUser(loggedinUser);
                var userDeatils = this.userService.GetUserDetails(loggedinUser.UserId);
                userDeatils.FcmToken = null;
                this.userService.UpdateUserDetails(userDeatils);
                return StatusCode(StatusCodes.Status200OK,
                      new ResponseModel<object>(StatusCodes.Status200OK, true,
                       _localizer["You logged out successfully"], null));

            }
            catch (Exception ex)
            {
                await _errorLogService.InsertErrorLog(new BWErrorLog(HttpContext.GetUser().UserId, "Account/Logout", ex));
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<object>(StatusCodes.Status500InternalServerError, false, ex.Message, null));

            }
        }

        [HttpPost]
        [Route("updatelocation")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> Updatelocation([FromForm] string lang, [FromForm] string lat)
        {
            try
            {

                var loggedinUser = HttpContext.GetUser();

                if (loggedinUser == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound,
                        new ResponseModel<object>(StatusCodes.Status404NotFound, false,
                            _localizer["401eror"], null));
                }


                // get user
                var user = await userManager.FindByIdAsync(loggedinUser.UserId);
               
                var userDeatils = this.userService.GetUserDetails(user.Id);
                //var GetLinkAccount = this.userService.GetallLinkAccount((userDeatils.PrimaryId));
                //var Getalllistoftags = this.userService.Getalllistoftags((userDeatils.PrimaryId));
                userDeatils.lang = lang;
                userDeatils.lat = lat;
                this.userService.UpdateUserDetails(userDeatils);
                await userService.UpdateUserAddressFromGoogle(userDeatils, Convert.ToDouble(lat), Convert.ToDouble(lang));
                var itemEVENT = this.userService.allEventDataaroundevent(userDeatils.PrimaryId, Convert.ToDouble(lat), Convert.ToDouble(lang));
                var alloldnotification = authDBContext.FireBaseDatamodel;

                try
                {
                    var usernotification = alloldnotification.FirstOrDefault(m => m.userid == userDeatils.PrimaryId && m.Action_code == itemEVENT.EntityId);
                    if (usernotification == null)
                    {
                        FireBaseData fireBaseInfo = new FireBaseData() { Title = itemEVENT.Title, Body = "Check this hot event near you!", imageUrl = ((itemEVENT.EventTypeListid == 3 ? "" : BaseUrlDomain) + itemEVENT.image), Action_code = itemEVENT.EntityId, muit = false, Action = "Check_events_near_you" };
                        var addnoti = messageServes.getFireBaseData(userDeatils.PrimaryId, fireBaseInfo);
                        await messageServes.addFireBaseDatamodel(addnoti);
                        SendNotificationcs sendNotificationcs = new SendNotificationcs();
                        if (userDeatils.FcmToken != null)
                            await firebaseManager.SendNotification(userDeatils.FcmToken, fireBaseInfo);
                    }
                }
                catch
                {

                }

                updateUserModelviewprofile modl = new updateUserModelviewprofile();
                modl.UserName = user.DisplayedUserName;
                modl.DisplayedUserName = user.UserName;
                modl.Gender = userDeatils.Gender;
                modl.bio = userDeatils.bio;
                modl.birthdate = userDeatils.birthdate == null ? "" : userDeatils.birthdate.Value.ConvertDateTimeToString();
                modl.Email = user.Email;
                modl.lat = userDeatils.lat;
                modl.lang = userDeatils.lang;
                modl.Manualdistancecontrol = userDeatils.Manualdistancecontrol;
                modl.agefrom = userDeatils.agefrom;
                modl.ageto = userDeatils.ageto;
                modl.Filteringaccordingtoage = userDeatils.Filteringaccordingtoage;
                modl.personalSpace = userDeatils.personalSpace;
                modl.allowmylocation = userDeatils.allowmylocation;
                modl.ImageIsVerified = userDeatils.ImageIsVerified ?? false;
                modl.whatAmILookingFor = userDeatils.whatAmILookingFor;
                modl.MyAppearanceTypes = userDeatils.AppearanceTypes.Select(x => x.AppearanceTypeID).ToList();
                modl.ghostmode = userDeatils.ghostmode;
                modl.NeedUpdate = userDeatils.birthdate == null ? 1 : 0;
                modl.pushnotification = userDeatils.pushnotification;
                modl.language = userDeatils.language;
                modl.UserImage = string.IsNullOrEmpty(user.UserDetails.UserImage) ? "https://www.friendzsocialmedia.com/Images/Userprofile/person_default_a353371c-fcc2-43c3-ab55-d02229fba815.png" : BaseUrlDomain + user.UserDetails.UserImage;
                var GetLinkAccount2 = this.userService.GetallLinkAccount((userDeatils.PrimaryId));
                var Getalllistoftags2 = this.userService.Getalllistoftags((userDeatils.PrimaryId));

                List<listoftagsmodel> list2 = new List<listoftagsmodel>();
                foreach (var item in Getalllistoftags2)
                {
                    listoftagsmodel LinkAccountmodel = new listoftagsmodel();
                    LinkAccountmodel.tagname = item.Interests.name;
                    LinkAccountmodel.tagID = item.Interests.EntityId;
                    list2.Add(LinkAccountmodel);
                }
                List<listoftagsmodel> WhatBestDescripsMeListdata = new List<listoftagsmodel>();
                var WhatBestDescripsMeList = this.userService.GetallWhatBestDescripsMeList((userDeatils.PrimaryId));
                foreach (var item in WhatBestDescripsMeList)
                {
                    listoftagsmodel LinkAccountmodel = new listoftagsmodel();
                    LinkAccountmodel.tagname = item.WhatBestDescripsMe.name;
                    LinkAccountmodel.tagID = item.WhatBestDescripsMe.EntityId;
                    WhatBestDescripsMeListdata.Add(LinkAccountmodel);
                }
                List<listoftagsmodel> preferto = new List<listoftagsmodel>();
                var prefertolist = this.userService.GetallIprefertolist((userDeatils.PrimaryId));
                foreach (var item in prefertolist)
                {
                    listoftagsmodel LinkAccountmodel = new listoftagsmodel();
                    LinkAccountmodel.tagname = item.preferto.name;
                    LinkAccountmodel.tagID = item.preferto.EntityId;
                    preferto.Add(LinkAccountmodel);
                }
                modl.prefertoList = preferto.Distinct().ToList();
                userDeatils.Facebook = userDeatils.Facebook;

                userDeatils.instagram = userDeatils.instagram;
                var data = messageServes.getFireBasecount(userDeatils.PrimaryId);
                modl.FrindRequestNumber = _FrindRequest.GetallRequestes(userDeatils.PrimaryId, RequestesType.RecivedOnly).Where(m => m.status == 0).Count();
                modl.Message_Count = messageServes.messagelogincount(userDeatils.UserId);
                modl.notificationcount = data;
                modl.listoftagsmodel = list2;
                modl.IamList = WhatBestDescripsMeListdata;
                return StatusCode(StatusCodes.Status200OK,
                      new ResponseModel<object>(StatusCodes.Status200OK, true,
                       _localizer["updateprofiledata"].Value, modl));

            }
            catch (Exception ex)
            {
                await _errorLogService.InsertErrorLog(new BWErrorLog(HttpContext.GetUser().UserId, "Account/updatelocation", ex));
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<object>(StatusCodes.Status500InternalServerError, false, ex.Message, null));

            }
        }


        //[HttpPost]
        //[Route("updatelocation")]
        //[Consumes("application/x-www-form-urlencoded")]
        //public async Task<IActionResult> Updatelocation([FromForm] string lang, [FromForm] string lat)
        //{
        //    try
        //    {

        //        var loggedinUser = HttpContext.GetUser();

        //        // get user
        //        var user = await userManager.FindByIdAsync(loggedinUser.UserId);
        //        if (loggedinUser == null)
        //        {
        //            return StatusCode(StatusCodes.Status404NotFound,
        //            new ResponseModel<object>(StatusCodes.Status404NotFound, false,
        //             _localizer["401eror"], null));
        //        }

        //        var userDeatils = this._userService.GetUserDetails(user.Id);
        //        var GetLinkAccount = this._userService.GetallLinkAccount((userDeatils.PrimaryId));
        //        var Getalllistoftags = this._userService.Getalllistoftags((userDeatils.PrimaryId));
        //        userDeatils.lang = lang;
        //        userDeatils.lat = lat;
        //        this._userService.UpdateUserDetails(userDeatils);


        //        updateUserModelview modl = new updateUserModelview();
        //        modl.UserName = user.DisplayedUserName;
        //        modl.DisplayedUserName = user.UserName;
        //        modl.Gender = userDeatils.Gender;
        //        modl.bio = userDeatils.bio;
        //        modl.birthdate = userDeatils.birthdate == null ? "" : userDeatils.birthdate.Value.ConvertDateTimeToString();
        //        modl.Email = user.Email;
        //        modl.lat = userDeatils.lat;
        //        modl.lang = userDeatils.lang;
        //        modl.Manualdistancecontrol = userDeatils.Manualdistancecontrol;
        //        modl.agefrom = userDeatils.agefrom;
        //        modl.ageto = userDeatils.ageto;
        //        modl.Filteringaccordingtoage = userDeatils.Filteringaccordingtoage;
        //        modl.allowmylocation = userDeatils.allowmylocation;
        //        modl.MyAppearanceTypes = userDeatils.AppearanceTypes.Select(x => x.AppearanceTypeID).ToList();
        //        modl.ghostmode = userDeatils.ghostmode;

        //        modl.pushnotification = userDeatils.pushnotification;
        //        modl.language = userDeatils.language;
        //        modl.UserImage = _configuration["BaseUrl"] + user.UserDetails.UserImage;
        //        var GetLinkAccount2 = this._userService.GetallLinkAccount((userDeatils.PrimaryId));
        //        var Getalllistoftags2 = this._userService.Getalllistoftags((userDeatils.PrimaryId));

        //        List<listoftagsmodel> list2 = new List<listoftagsmodel>();
        //        foreach (var item in Getalllistoftags2)
        //        {
        //            listoftagsmodel LinkAccountmodel = new listoftagsmodel();
        //            LinkAccountmodel.tagname = item.Interests.name;
        //            LinkAccountmodel.tagID = item.Interests.EntityId;
        //            list2.Add(LinkAccountmodel);
        //        }
        //        userDeatils.Facebook = userDeatils.Facebook;
        //        userDeatils.tiktok = userDeatils.tiktok;
        //        userDeatils.instagram = userDeatils.instagram;
        //        userDeatils.snapchat = userDeatils.snapchat;
        //        modl.listoftagsmodel = list2;
        //        return StatusCode(StatusCodes.Status200OK,
        //              new ResponseModel<object>(StatusCodes.Status200OK, true,
        //               _localizer["updateprofiledata"].Value, modl));

        //    }
        //    catch (Exception ex)
        //    {
        //        await _errorLogService.InsertErrorLog(new BWErrorLog(HttpContext.GetUser().UserId, "Account/updatelocation", ex));
        //        return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<object>(StatusCodes.Status500InternalServerError, false, ex.Message, null));
        //    }
        //}

        [HttpPost]
        [Route("updatSetting")]
        //[Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> updatSetting([FromForm] UpdateSettingsVM model)
        {
            try
            {
                var appcon = appConfigrationService.GetData().FirstOrDefault();

                var loggedinUser = HttpContext.GetUser();
                // get user
                var user = await userManager.FindByIdAsync(loggedinUser.UserId);
                if (loggedinUser == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound,
                    new ResponseModel<object>(StatusCodes.Status404NotFound, false,
                     _localizer["401eror"], null));
                }
                if (model.Filteringaccordingtoage == true)
                {
                    if (appcon != null ? (appcon.AgeFiltering_Max != null) : false)
                    {


                        if (appcon.AgeFiltering_Min > model.agefrom || appcon.AgeFiltering_Max < model.ageto)
                        {
                            return StatusCode(StatusCodes.Status406NotAcceptable,
                                new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                                 _localizer["Age Filtering Min  "] + appcon.AgeFiltering_Min + _localizer["  Age Filtering Max is  "] + appcon.AgeFiltering_Max, null));
                        }
                    }
                }
                if (model.distanceFilter == true)
                {

                    if (appcon != null ? (appcon.DistanceFiltering_Max != null) : false)
                    {


                        if (appcon.DistanceFiltering_Min > model.Manualdistancecontrol || appcon.DistanceFiltering_Max < model.Manualdistancecontrol)
                        {
                            return StatusCode(StatusCodes.Status406NotAcceptable,
                                new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                                 _localizer["User Distance Filtering Min  is  "] + appcon.DistanceFiltering_Min + _localizer["  User Distance Filtering Max is  "] + appcon.DistanceFiltering_Max, null));
                        }
                    }
                    await distanceFilterHistoryService.Create(user, Convert.ToDecimal(model.Manualdistancecontrol));

                }
                if (model.Filteringaccordingtoage == true)
                {

                    await filteringAccordingToAgeHistoryService.Create(user, model.agefrom ?? 0, model.ageto ?? 0);
                }
                var userDeatils = this.userService.GetUserDetails(user.Id);
                var GetLinkAccount = this.userService.GetallLinkAccount((userDeatils.PrimaryId));
                var Getalllistoftags = this.userService.Getalllistoftags((userDeatils.PrimaryId));
                userDeatils.pushnotification = model.pushnotification == null ? userDeatils.pushnotification : Convert.ToBoolean(model.pushnotification);
                userDeatils.allowmylocation = model.allowmylocation == null ? userDeatils.allowmylocation : Convert.ToBoolean(model.allowmylocation);
                userDeatils.ghostmode = model.ghostmode == null ? userDeatils.ghostmode : Convert.ToBoolean(model.ghostmode);
                if (model.ghostmode == false)
                {
                    authDBContext.AppearanceTypes_UserDetails.RemoveRange(userDeatils.AppearanceTypes);
                }
                if (string.IsNullOrEmpty(model.MyAppearanceTypes) == false && (model.ghostmode != null && model.ghostmode != false) && model.MyAppearanceTypes != null)
                {
                    authDBContext.AppearanceTypes_UserDetails.RemoveRange(userDeatils.AppearanceTypes);
                    List<int> MyAppearanceTypes = new List<int>();
                    try
                    {
                        MyAppearanceTypes = JsonConvert.DeserializeObject<List<int>>(model.MyAppearanceTypes);
                        MyAppearanceTypes = MyAppearanceTypes.Distinct().ToList();
                    }
                    catch
                    {
                        return StatusCode(StatusCodes.Status404NotFound,
                   new ResponseModel<object>(StatusCodes.Status404NotFound, false,
                    $"{model.MyAppearanceTypes}  invalid AppearanceTypes values ", null));
                    }
                    userDeatils.AppearanceTypes = MyAppearanceTypes?.Select(x => new AppearanceTypes_UserDetails
                    {
                        AppearanceTypeID = x,
                    }).ToList();
                }

                //if (model.ghostmode != null) //Mohamed mobile 21-.012022
                //{
                //    if (model.ghostmode == false)
                //    {
                //    authDBContext.AppearanceTypes_UserDetails.RemoveRange(userDeatils.AppearanceTypes);
                //        userDeatils.AppearanceTypes = new List<AppearanceTypes_UserDetails>() { new AppearanceTypes_UserDetails() { AppearanceTypeID = (int)AppearanceTypes.hiddenForall } };
                //    }
                //    else if(model.ghostmode==true)
                //    {
                //        authDBContext.AppearanceTypes_UserDetails.RemoveRange(userDeatils.AppearanceTypes);

                //        List<int> MyAppearanceTypes = new List<int>();
                //        try
                //        {
                //            MyAppearanceTypes = JsonConvert.DeserializeObject<List<int>>(model.MyAppearanceTypes);
                //        }
                //        catch
                //        {
                //            return StatusCode(StatusCodes.Status404NotFound,
                //       new ResponseModel<object>(StatusCodes.Status404NotFound, false,
                //        $"{model.MyAppearanceTypes}  invalid AppearanceTypes values ", null));
                //        }
                //        userDeatils.AppearanceTypes = MyAppearanceTypes?.Select(x => new AppearanceTypes_UserDetails
                //        {
                //            AppearanceTypeID = x,
                //        }).ToList();
                //        if (userDeatils.AppearanceTypes?.Count == 0)
                //            userDeatils.AppearanceTypes = new List<AppearanceTypes_UserDetails>() { new AppearanceTypes_UserDetails() { AppearanceTypeID = (int)AppearanceTypes.hiddenForall } };

                //    }
                //}

                userDeatils.language = (model.language == "" || model.language == null) ? userDeatils.language : model.language;

                userDeatils.agefrom = model.agefrom == null ? userDeatils.agefrom : Convert.ToInt32(model.agefrom);
                userDeatils.ageto = model.ageto == null ? userDeatils.ageto : Convert.ToInt32(model.ageto);
                userDeatils.Filteringaccordingtoage = model.Filteringaccordingtoage == null ? userDeatils.Filteringaccordingtoage : Convert.ToBoolean(model.Filteringaccordingtoage);
                userDeatils.distanceFilter = model.distanceFilter ?? userDeatils.distanceFilter;
                userDeatils.Manualdistancecontrol = userDeatils.distanceFilter == false ? Convert.ToDecimal(appcon.DistanceShowNearbyAccountsInFeed_Max) : (model.Manualdistancecontrol == null ? (userDeatils.Manualdistancecontrol) : (Convert.ToDecimal(model.Manualdistancecontrol)));
                userDeatils.personalSpace = model.personalSpace == null ? userDeatils.personalSpace : Convert.ToBoolean(model.personalSpace);
                userDeatils.whatAmILookingFor = model.whatAmILookingFor;
                this.userService.UpdateUserDetails(userDeatils);
                var testvalue = _localizer["updateprofiledata"].Value;

                return StatusCode(StatusCodes.Status200OK,
                      new ResponseModel<object>(StatusCodes.Status200OK, true,
                        _localizer["updateprofiledata"].Value, new
                        {
                            language = userDeatils.language,
                            MyAppearanceTypes = userDeatils.AppearanceTypes.Select(x => x.AppearanceTypeID).ToList(),
                            ghostmode = userDeatils.ghostmode,
                            pushnotification = userDeatils.pushnotification,
                            allowmylocation = userDeatils.allowmylocation,
                            whatAmILookingFor = userDeatils.whatAmILookingFor,
                            Manualdistancecontrol = userDeatils.Manualdistancecontrol,
                            agefrom = userDeatils.agefrom,
                            ageto = userDeatils.ageto,
                            distanceFilter = userDeatils.distanceFilter,
                            Filteringaccordingtoage = userDeatils.Filteringaccordingtoage,
                            userDeatils.personalSpace
                        }));

            }
            catch (Exception ex)
            {
                await _errorLogService.InsertErrorLog(new BWErrorLog(HttpContext.GetUser().UserId, "Account/updatesetting", ex));
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<object>(StatusCodes.Status500InternalServerError, false, ex.Message, null));
            }
        }

        [HttpPost]
        [Route("UserSetting")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> UserSetting()
        {
            try
            {
                var userDeatils = HttpContext.GetUser().User.UserDetails;

                return StatusCode(StatusCodes.Status200OK,
                      new ResponseModel<object>(StatusCodes.Status200OK, true,
                      "Your data  !", new
                      {
                          language = userDeatils.language,

                          MyAppearanceTypes = userDeatils.AppearanceTypes.Select(x => x.AppearanceTypeID).Distinct().ToList(),


                          userDeatils.ghostmode,
                          userDeatils.distanceFilter,
                          userDeatils.pushnotification,
                          userDeatils.allowmylocation,
                          userDeatils.whatAmILookingFor,
                          userDeatils.Manualdistancecontrol,
                          userDeatils.agefrom,
                          userDeatils.ageto,
                          userDeatils.Filteringaccordingtoage,
                          userDeatils.personalSpace
                      }));

            }
            catch (Exception ex)
            {
                await _errorLogService.InsertErrorLog(new BWErrorLog(HttpContext.GetUser().UserId, "Account/usersetting", ex));
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<object>(StatusCodes.Status500InternalServerError, false, ex.Message, null));

            }
        }

        [HttpPost]
        [Route("GetAllValidatConfig")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> GetAllValidatConfig()
        {
            try
            {
                var userDeatils = HttpContext.GetUser().User.UserDetails;

                return StatusCode(StatusCodes.Status200OK,
                      new ResponseModel<object>(StatusCodes.Status200OK, true,
                      "Your data  !", appConfigrationService.GetData().FirstOrDefault()));

            }
            catch (Exception ex)
            {
                await _errorLogService.InsertErrorLog(new BWErrorLog(HttpContext.GetUser().UserId, "Account/GetAllValidatConfig", ex));
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<object>(StatusCodes.Status500InternalServerError, false, ex.Message, null));

            }
        }

        [HttpPost]
        [Route("DeleteAccount")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> DeleteUser()
        {
            try
            {

                var loggedinUser = HttpContext.GetUser();
                var userDeatils = loggedinUser.User.UserDetails;

                await userService.DeleteUser_StoredProcedure(userDeatils);
                //await _userService.DeleteUser(loggedinUser, userDeatils.PrimaryId);
                //await userManager.DeleteAsync(loggedinUser.User);


                return StatusCode(StatusCodes.Status200OK,
                          new ResponseModel<object>(StatusCodes.Status200OK, true,
                          _localizer["AccounthasbeenDeleted"], null));
            }
            catch (Exception ex)
            {
                await _errorLogService.InsertErrorLog(new BWErrorLog(HttpContext.GetUser().UserId, "Account/deleteaccount", ex));
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<object>(StatusCodes.Status500InternalServerError, false, ex.Message, null));

            }
        }

        [HttpPost]
        [Route("UpdateLinkAccount")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> UpdateLinkAccount([FromForm] LinkAccount model)
        {
            try
            {

                var loggedinUser = HttpContext.GetUser();

                var userDeatils = HttpContext.GetUser().User.UserDetails;

                var GetLinkAccount = this.userService.GetLinkAccount(Convert.ToInt32(model.Id));
                GetLinkAccount.LinkAccountname = model.LinkAccountname;
                GetLinkAccount.LinkAccounturl = model.LinkAccounturl;

                this.userService.UpdateLinkAccount(GetLinkAccount);

                return StatusCode(StatusCodes.Status200OK,
                      new ResponseModel<object>(StatusCodes.Status200OK, true,
                      _localizer["updateprofiledata"], model));

            }
            catch (Exception ex)
            {
                await _errorLogService.InsertErrorLog(new BWErrorLog(HttpContext.GetUser().UserId, "Account/Update", ex));
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<object>(StatusCodes.Status500InternalServerError, false, ex.Message, null));

            }
        }

        int GetAge(DateTime birthDate)
        {
            DateTime n = DateTime.Now; // To avoid a race condition around midnight
            int age = n.Year - birthDate.Year;

            if (n.Month < birthDate.Month || (n.Month == birthDate.Month && n.Day < birthDate.Day))
                age--;

            return age;
        }

        [HttpPost]
        [Route("getprofildata")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> getprofildata()
        {
            try
            {
                var userDeatils = HttpContext.GetUser().User.UserDetails;
                var user = HttpContext.GetUser().User;
                var userImages = userService.GetUserImages(userDeatils.PrimaryId);
                var GetLinkAccount = this.userService.GetallLinkAccount((userDeatils.PrimaryId));
                var Getalllistoftags = this.userService.Getalllistoftags((userDeatils.PrimaryId));

                updateUserModelviewprofile updateUserModel = new updateUserModelviewprofile();
                updateUserModel.age = userDeatils.birthdate == null ? 0 : (GetAge(userDeatils.birthdate.Value));
                updateUserModel.UserImages = userImages.Any() ? userImages.Select(a=> BaseUrlDomain + a.ImageUrl).ToList() : new List<string>();
                updateUserModel.bio = userDeatils.bio;
                updateUserModel.distanceFilter = userDeatils.distanceFilter;
                updateUserModel.Email = userDeatils.User.Email;
                updateUserModel.birthdate = userDeatils.birthdate == null ? "" : userDeatils.birthdate.Value.ConvertDateTimeToString();
                updateUserModel.UserImage = (userDeatils.UserImage == null || userDeatils.UserImage == "") ? "https://www.friendzsocialmedia.com/Images/Userprofile/person_default_a353371c-fcc2-43c3-ab55-d02229fba815.png" : (BaseUrlDomain + userDeatils.UserImage);
                updateUserModel.lat = userDeatils.lat;
                updateUserModel.lang = userDeatils.lang;
                updateUserModel.UserName = user.DisplayedUserName;
                updateUserModel.DisplayedUserName = user.UserName;

                List<listoftagsmodel> list2 = new List<listoftagsmodel>();
                foreach (var item in Getalllistoftags)
                {
                    listoftagsmodel LinkAccountmodel = new listoftagsmodel();
                    LinkAccountmodel.tagname = item.Interests.name;
                    LinkAccountmodel.tagID = item.Interests.EntityId;
                    list2.Add(LinkAccountmodel);
                }
                List<listoftagsmodel> WhatBestDescripsMeListdata = new List<listoftagsmodel>();
                var WhatBestDescripsMeList = this.userService.GetallWhatBestDescripsMeList((userDeatils.PrimaryId));
                foreach (var item in WhatBestDescripsMeList)
                {
                    listoftagsmodel LinkAccountmodel = new listoftagsmodel();
                    LinkAccountmodel.tagname = item.WhatBestDescripsMe.name;
                    LinkAccountmodel.tagID = item.WhatBestDescripsMe.EntityId;
                    WhatBestDescripsMeListdata.Add(LinkAccountmodel);
                }
                List<listoftagsmodel> preferto = new List<listoftagsmodel>();
                var prefertolist = this.userService.GetallIprefertolist((userDeatils.PrimaryId));
                foreach (var item in prefertolist)
                {
                    listoftagsmodel LinkAccountmodel = new listoftagsmodel();
                    LinkAccountmodel.tagname = item.preferto.name;
                    LinkAccountmodel.tagID = item.preferto.EntityId;
                    preferto.Add(LinkAccountmodel);
                }
                updateUserModel.prefertoList = preferto.Distinct().ToList();

                updateUserModel.Gender = userDeatils.Gender;
                updateUserModel.OtherGenderName = userDeatils.OtherGenderName;
                updateUserModel.ImageIsVerified = userDeatils.ImageIsVerified ?? false;
                updateUserModel.listoftagsmodel = list2;
                updateUserModel.IamList = WhatBestDescripsMeListdata;
                updateUserModel.allowmylocation = userDeatils.allowmylocation;
                updateUserModel.whatAmILookingFor = userDeatils.whatAmILookingFor;
                updateUserModel.MyAppearanceTypes = userDeatils.AppearanceTypes.Select(x => x.AppearanceTypeID).ToList();
                updateUserModel.ghostmode = userDeatils.ghostmode;
                updateUserModel.pushnotification = userDeatils.pushnotification;
                updateUserModel.language = userDeatils.language;
                updateUserModel.NeedUpdate = userDeatils.birthdate == null ? 1 : 0;
                updateUserModel.Manualdistancecontrol = userDeatils.Manualdistancecontrol;
                updateUserModel.agefrom = userDeatils.agefrom;
                var data = messageServes.getFireBasecount(userDeatils.PrimaryId);
                updateUserModel.FrindRequestNumber = _FrindRequest.GetallRequestes(userDeatils.PrimaryId, RequestesType.RecivedOnly).Where(m => m.status == 0).Count();
                updateUserModel.Message_Count = messageServes.messagelogincount(userDeatils.UserId);
                updateUserModel.notificationcount = data;
                updateUserModel.ageto = userDeatils.ageto;
                updateUserModel.Filteringaccordingtoage = userDeatils.Filteringaccordingtoage;
                updateUserModel.personalSpace = userDeatils.personalSpace;
                updateUserModel.UniversityCode = userDeatils.Code;
                return StatusCode(StatusCodes.Status200OK,
                      new ResponseModel<object>(StatusCodes.Status200OK, true,
                      "Your data ", updateUserModel));


            }
            catch (Exception ex)
            {
                await _errorLogService.InsertErrorLog(new BWErrorLog(HttpContext.GetUser().UserId, "Account/getprofile", ex));
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<object>(StatusCodes.Status500InternalServerError, false, ex.Message, null));

            }
        }

        [HttpPost]
        [Route("SetLanguage")]
        public IActionResult SetLanguage([FromForm] string culture)
        {
            Response.Cookies.Append(
               "Usre_Culture",
                culture,
            new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );
            Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
        new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
        );
            //var currentLanguage = _httpContext.Features.Get<IRequestCultureFeature>().RequestCulture.Culture.Name;

            return StatusCode(StatusCodes.Status200OK,
                     new ResponseModel<object>(StatusCodes.Status200OK, true,
                     " Language set", culture));
        }

        [HttpPost]
        [Route("changepass")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> ChangePassword([FromForm] string oldPassword, [FromForm] string newPassword)
        {
            try
            {
                var loggedinUser = HttpContext.GetUser();

                var userDeatils = HttpContext.GetUser().User.UserDetails;

                var user = HttpContext.GetUser().User;
                if (string.IsNullOrEmpty(oldPassword))
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                        new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,
                        _localizer["Pleaseenterthecurrentpassword!"], null));

                if (string.IsNullOrEmpty(newPassword))
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                        new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,
                       _localizer["Pleaseenterthenewpassword!"], null));

                // check if old pass is correct
                if (user != null && await userManager.CheckPasswordAsync(user, oldPassword))
                {
                    //generate pass reset token 
                    var token = await userManager.GeneratePasswordResetTokenAsync(user);

                    if (!string.IsNullOrEmpty(token))
                    {
                        // reset pass
                        var result = await userManager.ResetPasswordAsync(user, token, newPassword);
                        if (!result.Succeeded)
                        {
                            return StatusCode(StatusCodes.Status406NotAcceptable,
                                new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,
                                result.Errors.FirstOrDefault().Description, null));
                        }

                        return StatusCode(StatusCodes.Status200OK,
                         new ResponseModel<object>(StatusCodes.Status200OK, true,
                        _localizer["Passwordchangedsuccessfully"], null));
                    }


                }

                return StatusCode(StatusCodes.Status406NotAcceptable,
                     new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,
                     _localizer["old password is wrong"], null));



            }
            catch (Exception ex)
            {
                await _errorLogService.InsertErrorLog(new BWErrorLog(HttpContext.GetUser().UserId, "Account/changepassword", ex));
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<object>(StatusCodes.Status500InternalServerError, false, ex.Message, null));

            }
        }

    }

}

