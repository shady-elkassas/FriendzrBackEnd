using CRM.Services.Wrappers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Social.Entity.DBContext;
using Social.Entity.Enums;
using Social.Entity.Models;
using Social.Entity.ModelView;
using Social.FireBase;
using Social.Services;
using Social.Services.Attributes;
using Social.Services.FireBase_Helper;
using Social.Services.Helpers;
using Social.Services.Implementation;
using Social.Services.ModelView;
using Social.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Social.Entity.Migrations;

namespace Social.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(AuthorizeUser))]
    public class FrindRequestController : ControllerBase
    {
        public AuthDBContext _authContext;
        private readonly IUserService _userService;
        private readonly IFrindRequest _FrindRequest;
        private readonly IMessageServes MessageServes;
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _environment;
        private readonly IFirebaseManager firebaseManager;
        private readonly IErrorLogService _errorLogService;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly IAppConfigrationService appConfigrationService;
        public FrindRequestController(IAppConfigrationService appConfigrationService, IMessageServes MessageServes, UserManager<User> userManager, IStringLocalizer<SharedResource> localizer, IHostingEnvironment environment, IFirebaseManager firebaseManager, IFrindRequest FrindRequest, IConfiguration configuration, IUserService userService, IErrorLogService errorLogService, AuthDBContext authContext)
        {
            _localizer = localizer;
            _userService = userService;
            _environment = environment;
            _authContext = authContext;
            _FrindRequest = FrindRequest;
            _configuration = configuration;
            this.MessageServes = MessageServes;
            _errorLogService = errorLogService;
            this.firebaseManager = firebaseManager;
            this.appConfigrationService = appConfigrationService;
        }


        /// <summary>
        ///    Feeds Api
        /// </summary> 
        [HttpPost]
        [Route("AllUsers")]
        [Consumes("application/x-www-form-urlencoded")]
        [ServiceFilter(typeof(AuthorizeUser))]
        public async Task<IActionResult> AllUsers([FromForm] int pageNumber, [FromForm] int pageSize, [FromForm] double degree, [FromForm] string userlang, [FromForm] string userlat, [FromForm] bool sortByInterestMatch)
        {
            try
            {
                var userDeatils = HttpContext.GetUser().User.UserDetails;
                userDeatils.LastFeedRequestDate = DateTime.UtcNow; //Last Active Time
                this._userService.UpdateUserDetails(userDeatils);
                if (userDeatils.allowmylocation == false)
                {
                    return StatusCode(StatusCodes.Status400BadRequest,
                    new ResponseModel<object>(StatusCodes.Status400BadRequest, false,
                     _localizer["please allow your location"], null));
                }
                if (userlang != "" && userlang != null && userlat != "" && userlat != null)
                {
                    userDeatils.lang = userlang;
                    userDeatils.lat = userlat;
                    this._userService.UpdateUserDetails(userDeatils);
                }
               // var allReq = _authContext.Requestes.ToList();
                var lat = userDeatils.lat is null ? 0 : Convert.ToDouble(userDeatils.lat);
                var lang = userDeatils.lang is null ? 0 : Convert.ToDouble(userDeatils.lang);
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
                if (userDeatils.lang is null || userDeatils.lang == "0")
                {
                    return StatusCode(StatusCodes.Status200OK, new ResponseModel<object>(StatusCodes.Status200OK, true, "not data Found", new
                    {
                        pageNumber = 1,
                        pageSize = pageSize,
                        totalPages = 0,
                        totalRecords = 0,
                        data = new List<Feeds>()
                    }));
                }
                var appcon = appConfigrationService.GetData().FirstOrDefault();

                if (sortByInterestMatch)
                {
                    UserLinkClick userLinkClick = new UserLinkClick() { UserId = userDeatils.PrimaryId, Date = DateTime.Now, Type = LinkClickTypeEnum.SortByInterestMatch.ToString() };

                    _authContext.UserLinkClicks.Add(userLinkClick);

                    await _authContext.SaveChangesAsync();
                } 

                (List<UserDetails> userDetails, List<int> currentUserInterests) userDetailsList = (degree is 0) ?
                    _userService.allusers(lat, lang, userDeatils.Gender, userDeatils, appcon, sortByInterestMatch) :
                    _userService.allusersdirection(lat, lang, userDeatils.Gender, userDeatils, degree, appcon, sortByInterestMatch);

                var listUsersdata = userDetailsList.userDetails.Where(m => m.PrimaryId != userDeatils.PrimaryId).ToList();
                var validFilter = new PaginationFilter(pageNumber, pageSize);
                var  listdata = listUsersdata.Skip((validFilter.PageNumber - 1) * validFilter.PageSize).Take(validFilter.PageSize).ToList();

                var data = listdata.Select(q => new Feeds
                {
                    Gender = q.Gender,
                    userId = q.UserId,
                    lang = q.lang,
                    lat = q.lat,
                    ImageIsVerified = q.ImageIsVerified ?? false,
                    DisplayedUserName = q.User.UserName,
                    UserName = q.User.DisplayedUserName,
                    email = q.User.Email,
                    image = string.IsNullOrEmpty(q.UserImage) ? _configuration["DefaultImage"] : _configuration["BaseUrl"] + q.UserImage,
                    key = _FrindRequest.GetallkeyForFeed(userDeatils.PrimaryId, q.PrimaryId),
                    InterestMatchPercent = Math.Round(((q.listoftags.Select(q => q.InterestsId).Intersect(userDetailsList.currentUserInterests).Count() / Convert.ToDecimal(userDetailsList.currentUserInterests.Count())) * 100), 0)
                }).Where(k => k.key != 4 && k.key != 5).ToList();
                int rowCount = listUsersdata.Count();
                var pagedLands = data.Skip((validFilter.PageNumber - 1) * validFilter.PageSize).Take(validFilter.PageSize).ToList();
                var pagedModel = new PagedResponse<List<Feeds>>(pagedLands, validFilter.PageNumber, pagedLands.Count(), data.Count());

                //var dataObj = (degree is 0) ? pagedModel.Data : pagedLands;

                var totalPages = ((double)rowCount / (double)validFilter.PageSize);

                int roundedTotalPages = Convert.ToInt32(Math.Ceiling(totalPages));

                return StatusCode(StatusCodes.Status200OK, new ResponseModel<object>(StatusCodes.Status200OK, true, "Your data ", new
                {
                    //TODO: Abdelrahman (Fix Pagination)
                    //pageNumber = pagedModel.PageNumber,
                    //pageSize = pagedModel.PageSize,
                    //totalPages = pagedModel.TotalPages,
                    //totalRecords = pagedModel.TotalRecords,

                    pageNumber = pagedModel.PageNumber,
                    pageSize = pagedModel.PageSize,
                    totalPages = roundedTotalPages,
                    totalRecords = rowCount,
                    data = pagedModel.Data
                }));
            }
            catch (Exception ex)
            {
                await _errorLogService.InsertErrorLog(new BWErrorLog(HttpContext.GetUser().UserId, "FrindRequest/AllUsers", ex));
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<object>(StatusCodes.Status500InternalServerError, false, ex.Message, null));
            }
        }


        [HttpPost]
        [Route("Allrequest")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> Allrequest([FromForm] int pageNumber, [FromForm] int pageSize, [FromForm] string search, [FromForm] RequestesType requestesType)
        {
            try
            {
                IQueryable<Requestes> listdata;

                var userDeatils = HttpContext.GetUser().User.UserDetails;

                listdata = _FrindRequest.GetallRequestes(userDeatils.PrimaryId, requestesType, search);

                var validFilter = new PaginationFilter(pageNumber, pageSize);

                var pagedLands = listdata.OrderBy(m => m.status).ThenByDescending(m => m.regestdata).Skip((validFilter.PageNumber - 1) * validFilter.PageSize).Take(validFilter.PageSize).ToList();

                var pagedModel = new PagedResponse<List<Requestes>>(pagedLands, validFilter.PageNumber,

                    pagedLands.Count(), listdata.Count());

                var allReq = _authContext.Requestes.ToList();

                List<int> currentUserInterests = userDeatils.listoftags.Select(q => q.InterestsId).ToList();

                return StatusCode(StatusCodes.Status200OK,
                      new ResponseModel<object>(StatusCodes.Status200OK, true,
                      "Your data ", new
                      {
                          pageNumber = pagedModel.PageNumber,
                          pageSize = pagedModel.PageSize,
                          totalPages = pagedModel.TotalPages,
                          totalRecords = pagedModel.TotalRecords,
                          data = pagedModel.Data.Select(m => new
                          {
                              IsSentRequest = userDeatils.PrimaryId == m.UserId,
                              regestdata = m.regestdata.ToString(@"dd-MM-yyyy HH\:mm"),
                              message = m.Message ?? string.Empty,
                              userId = userDeatils.PrimaryId == m.UserId ? m.UserRequest.UserId : m.User.UserId,
                              ImageIsVerified = m.User?.ImageIsVerified ?? false,
                              lang = userDeatils.PrimaryId == m.UserId ? m.UserRequest.lang : m.User.lang,
                              lat = userDeatils.PrimaryId == m.UserId ? m.UserRequest.lat : m.User.lat,
                              UserName = userDeatils.PrimaryId == m.UserId ? m.UserRequest.User.DisplayedUserName : m.User.User.DisplayedUserName,
                              DisplayedUserName = userDeatils.PrimaryId == m.UserId ? m.UserRequest.User.UserName : m.User.User.UserName,
                              Email = userDeatils.PrimaryId == m.UserId ? m.UserRequest.User.Email : m.User.User.Email,
                              image = userDeatils.PrimaryId == m.UserId 
                                  ? string.IsNullOrEmpty(m.UserRequest.UserImage) 
                                      ? _configuration["DefaultImage"] : _configuration["BaseUrl"] + m.UserRequest.UserImage 
                                  : string.IsNullOrEmpty(m.User.UserImage)
                                      ? _configuration["DefaultImage"] : _configuration["BaseUrl"] + m.User.UserImage,
                              key = _FrindRequest.Getallkey(userDeatils.PrimaryId, (userDeatils.PrimaryId == m.UserId ? m.UserRequestId : m.UserId) ?? 0, allReq),
                              InterestMatchPercent = m.User.PrimaryId == userDeatils.PrimaryId ?
                              Math.Round(((m.UserRequest.listoftags.Select(q => q.InterestsId).Intersect(currentUserInterests).Count() / Convert.ToDecimal(currentUserInterests.Count())) * 100), 0) : Math.Round(((m.User.listoftags.Select(q => q.InterestsId).Intersect(currentUserInterests).Count() / Convert.ToDecimal(currentUserInterests.Count())) * 100), 0)
                          }).ToList()
                      }));


            }
            catch (Exception ex)
            {
                await _errorLogService.InsertErrorLog(new BWErrorLog(HttpContext.GetUser().UserId, "FrindRequest/Allrequest", ex));
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<object>(StatusCodes.Status500InternalServerError, false, ex.Message, null));
            }
        }


        [HttpPost]
        [Route("AllFriendes")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> AllFriendes([FromForm] int pageNumber, [FromForm] int pageSize, [FromForm] string search)
        {
            try
            {                             
                var userDeatils = HttpContext.GetUser().User.UserDetails;
                var validFilter = new PaginationFilter(pageNumber, pageSize);
                var allReq = _authContext.Requestes.ToList();
                List<int> currentUserInterests = userDeatils.listoftags.Select(q => q.InterestsId).ToList();
               
                if (!string.IsNullOrEmpty(userDeatils.Code) && userDeatils.IsWhiteLabel.HasValue && userDeatils.IsWhiteLabel.Value)
                {
                    var allFriend = _authContext.UserDetails
                        .Where(u => u.Code == userDeatils.Code && u.PrimaryId != userDeatils.PrimaryId
                    && (string.IsNullOrWhiteSpace(search) || u.User.DisplayedUserName.ToLower().Contains(search.ToLower()))).ToList();
                   
                    var pagedLandsFriend = allFriend.Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                      .Take(validFilter.PageSize).ToList();
                    var pagedModelFriends = new PagedResponse<List<UserDetails>>(pagedLandsFriend, validFilter.PageNumber,
                                   pagedLandsFriend.Count(), allFriend.Count());
                    return StatusCode(StatusCodes.Status200OK,
                          new ResponseModel<object>(StatusCodes.Status200OK, true,
                          "Your data ", new
                          {
                              pageNumber = pagedModelFriends.PageNumber,
                              pageSize = pagedModelFriends.PageSize,
                              totalPages = pagedModelFriends.TotalPages,
                              totalRecords = pagedModelFriends.TotalRecords,
                              data = pagedModelFriends.Data.Select(m => new
                              {
                                  userId = m.UserId,
                                  lang = m.lang,
                                  lat = m.lat,
                                  regestdata = m.User.RegistrationDate.ConvertDateTimeToString(),
                                  userName = m.userName,
                                  DisplayedUserName = m.User.DisplayedUserName,
                                  Email = m.User.Email,
                                  ImageIsVerified = m.ImageIsVerified ?? false,
                                  image =string.IsNullOrEmpty(m.UserImage) ? _configuration["DefaultImage"] : _configuration["BaseUrl"] + m.UserImage,
                                  key = 3,// by default friends because they are in the same community                                  
                              }).ToList()
                          }));

                }
                else { 
                        var requestFriends = _FrindRequest.GetallFrendes(userDeatils.PrimaryId, search);
                        var pagedLands = requestFriends.Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                               .Take(validFilter.PageSize).ToList();
                        var pagedModel = new PagedResponse<List<Requestes>>(pagedLands, validFilter.PageNumber,
                                       pagedLands.Count(), requestFriends.Count());
                        return StatusCode(StatusCodes.Status200OK,
                              new ResponseModel<object>(StatusCodes.Status200OK, true,
                          "Your data ", new
                          {
                              pageNumber = pagedModel.PageNumber,
                              pageSize = pagedModel.PageSize,
                              totalPages = pagedModel.TotalPages,
                              totalRecords = pagedModel.TotalRecords,
                              data = pagedModel.Data.Select(m => new
                              {
                                  userId = m.UserId != userDeatils.PrimaryId ? m.User.UserId : m.UserRequest.UserId,
                                  lang = m.UserId != userDeatils.PrimaryId ? m.User.lang : m.UserRequest.lang,
                                  lat = m.UserId != userDeatils.PrimaryId ? m.User.lat : m.UserRequest.lat,
                                  regestdata = m.regestdata.Date.ConvertDateTimeToString(),
                                  ImageIsVerified = m.UserId != userDeatils.PrimaryId ? m.User.ImageIsVerified ?? false : m.UserRequest.ImageIsVerified ?? false,
                                  userName = m.UserId != userDeatils.PrimaryId ? m.User.User.DisplayedUserName : m.UserRequest.User.DisplayedUserName,
                                  DisplayedUserName = m.UserId != userDeatils.PrimaryId ? m.User.User.UserName : m.UserRequest.User.UserName,
                                  Email = m.UserId != userDeatils.PrimaryId ? m.User.User.Email : m.UserRequest.User.Email,
                                  image = _configuration["BaseUrl"] + (m.UserId != userDeatils.PrimaryId ? m.User.UserImage : m.UserRequest.UserImage),
                                  key = _FrindRequest.Getallkey(userDeatils.PrimaryId, m.UserId == userDeatils.PrimaryId ? m.UserRequest.PrimaryId : m.User.PrimaryId, allReq),
                                  InterestMatchPercent = m.User.PrimaryId == userDeatils.PrimaryId ? Math.Round(((m.UserRequest.listoftags.Select(q => q.InterestsId).Intersect(currentUserInterests).Count() / Convert.ToDecimal(currentUserInterests.Count())) * 100), 0) : Math.Round(((m.User.listoftags.Select(q => q.InterestsId).Intersect(currentUserInterests).Count() / Convert.ToDecimal(currentUserInterests.Count())) * 100), 0)
                              }).ToList()
                          }));


                }               
                
            }
            catch (Exception ex)
            {
                await _errorLogService.InsertErrorLog(new BWErrorLog(HttpContext.GetUser().UserId, "FrindRequest/AllFriendes", ex));
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<object>(StatusCodes.Status500InternalServerError, false, ex.Message, null));
            }
        }


        [HttpPost]
        [Route("AllBlocked")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> AllBlocked([FromForm] int pageNumber, [FromForm] int pageSize, [FromForm] string search)
        {
            try
            {
                var userDeatils = HttpContext.GetUser().User.UserDetails;
                var listdata = _FrindRequest.GetallBlock(userDeatils.PrimaryId, search);

                var allReq = _authContext.Requestes.ToList();
                var validFilter = new PaginationFilter(pageNumber, pageSize);
                var pagedLands = listdata.Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                .Take(validFilter.PageSize).ToList();
                var pagedModel = new PagedResponse<List<Requestes>>(pagedLands, validFilter.PageNumber,
            pagedLands.Count(), listdata.Count());
                return StatusCode(StatusCodes.Status200OK,
                      new ResponseModel<object>(StatusCodes.Status200OK, true,
                      "Your data ", new
                      {
                          pageNumber = pagedModel.PageNumber,
                          pageSize = pagedModel.PageSize,
                          totalPages = pagedModel.TotalPages,
                          totalRecords = pagedModel.TotalRecords,
                          data = pagedModel.Data.Select(m => new
                          {
                              blockDate = m.blockDate == null ? "" : m.blockDate.Value.Date.ConvertDateTimeToString(),
                              userId = m.UserId != userDeatils.PrimaryId ? m.User.UserId : m.UserRequest.UserId,
                              ImageIsVerified = m.User?.ImageIsVerified ?? false,
                              lang = m.UserId != userDeatils.PrimaryId ? m.User.lang : m.UserRequest.lang,
                              lat = m.UserId != userDeatils.PrimaryId ? m.User.lat : m.UserRequest.lat,
                              userName = m.UserId != userDeatils.PrimaryId ? m.User.User.DisplayedUserName : m.UserRequest.User.DisplayedUserName,
                              DisplayedUserName = m.UserId != userDeatils.PrimaryId ? m.User.User.UserName : m.UserRequest.User.UserName,
                              Email = m.UserId != userDeatils.PrimaryId ? m.User.User.Email : m.UserRequest.User.Email,
                              image = m.UserId != userDeatils.PrimaryId
                                  ? string.IsNullOrEmpty(m.User.UserImage)
                                      ? _configuration["DefaultImage"] : _configuration["BaseUrl"] + m.User.UserImage 
                                  : string.IsNullOrEmpty(m.UserRequest.UserImage)
                                      ? _configuration["DefaultImage"] : _configuration["BaseUrl"] + m.UserRequest.UserImage,
                              key = _FrindRequest.Getallkey(userDeatils.PrimaryId, m.UserId == userDeatils.PrimaryId ? m.UserRequest.PrimaryId : m.User.PrimaryId, allReq)
                          }).ToList().OrderByDescending(d=>d.blockDate)
                      }));

            }
            catch (Exception ex)
            {
                await _errorLogService.InsertErrorLog(new BWErrorLog(HttpContext.GetUser().UserId, "FrindRequest/AllBlocked", ex));
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<object>(StatusCodes.Status500InternalServerError, false, ex.Message, null));
            }
        }


        [HttpPost]
        [Route("Userprofil")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> requestprofil([FromForm] string userid)
        {
            try
            {
                var userDeatils = HttpContext.GetUser().User.UserDetails;
                var meDeatils = _userService.GetUserDetails(userid);
                var GetLinkAccount = _userService.GetallLinkAccount((meDeatils.PrimaryId));
                var Getalllistoftags = _userService.Getalllistoftags((meDeatils.PrimaryId));
                var userImages = _userService.GetUserImages(meDeatils.PrimaryId);

                var allReq = _authContext.Requestes.ToList();
                updateUserModelview updateUserModel = new updateUserModelview();

                updateUserModel.Gender = meDeatils.Gender;
                updateUserModel.OtherGenderName = meDeatils.OtherGenderName;
                updateUserModel.bio = meDeatils.bio;
                updateUserModel.ImageIsVerified = meDeatils.ImageIsVerified ?? false;
                updateUserModel.Email = meDeatils.User.Email;
                updateUserModel.birthdate = meDeatils.birthdate == null ? DateTime.Now.Date.ConvertDateTimeToString() : meDeatils.birthdate.Value.ConvertDateTimeToString();
                updateUserModel.age = meDeatils.birthdate == null ? 0 : (GetAge(meDeatils.birthdate.Value));

                updateUserModel.UserImage = (meDeatils.UserImage == null || meDeatils.UserImage == "") ? "" : (_configuration["BaseUrl"] + meDeatils.UserImage);
                updateUserModel.UserImages = userImages.Any() ? userImages.Select(a => _configuration["BaseUrl"] + a.ImageUrl).ToList() : new List<string>();

                updateUserModel.UserName = meDeatils.User.DisplayedUserName;
                updateUserModel.DisplayedUserName = meDeatils.User.UserName;
                updateUserModel.Userid = userid;
                if (userDeatils.IsWhiteLabel.HasValue && userDeatils.IsWhiteLabel.Value && userDeatils.Code == meDeatils.Code)
                {
                    var key =
                      _FrindRequest.Getallkey(userDeatils.PrimaryId, meDeatils.PrimaryId, allReq);
                    if(key==0)
                    {
                        updateUserModel.key = 3;
                    }
                    else
                    {
                        updateUserModel.key = key;
                    }
                  
                }
                else
                { 
                    updateUserModel.key =
                      _FrindRequest.Getallkey(userDeatils.PrimaryId, meDeatils.PrimaryId, allReq);
                }

                List<listoftagsmodel> list2 = new List<listoftagsmodel>();
                foreach (var item in Getalllistoftags)
                {
                    listoftagsmodel LinkAccountmodel = new listoftagsmodel();
                    LinkAccountmodel.tagname = item.Interests.name;
                    LinkAccountmodel.tagID = item.Interests.EntityId;
                    list2.Add(LinkAccountmodel);
                }
                List<listoftagsmodel> WhatBestDescripsMeListdata = new List<listoftagsmodel>();
                var WhatBestDescripsMeList = this._userService.GetallWhatBestDescripsMeList((meDeatils.PrimaryId));
                foreach (var item in WhatBestDescripsMeList)
                {
                    listoftagsmodel LinkAccountmodel = new listoftagsmodel();
                    LinkAccountmodel.tagname = item.WhatBestDescripsMe.name;
                    LinkAccountmodel.tagID = item.WhatBestDescripsMe.EntityId;
                    WhatBestDescripsMeListdata.Add(LinkAccountmodel);
                }
                List<listoftagsmodel> preferto = new List<listoftagsmodel>();
                var prefertolist = this._userService.GetallIprefertolist((meDeatils.PrimaryId));
                foreach (var item in prefertolist)
                {
                    listoftagsmodel LinkAccountmodel = new listoftagsmodel();
                    LinkAccountmodel.tagname = item.preferto.name;
                    LinkAccountmodel.tagID = item.preferto.EntityId;
                    preferto.Add(LinkAccountmodel);
                }
                updateUserModel.prefertoList = preferto.Distinct().ToList();
                updateUserModel.IamList = WhatBestDescripsMeListdata.Distinct().ToList();
                updateUserModel.listoftagsmodel = list2;
                return StatusCode(StatusCodes.Status200OK,
                      new ResponseModel<object>(StatusCodes.Status200OK, true,
                      "Your data ", updateUserModel));
            }
            catch (Exception ex)
            {
                await _errorLogService.InsertErrorLog(new BWErrorLog(HttpContext.GetUser().UserId, "FrindRequest/requestprofil", ex));
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<object>(StatusCodes.Status500InternalServerError, false, ex.Message, null));
            }
        }


        [HttpPost]
        [Route("RequestFriendStatus")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> RequestFriendStatus([FromForm] string userid, [FromForm] int key, [FromForm] DateTime? Requestdate, [FromForm] bool isNotFriend , [FromForm] string message)
        {
            try
            {
                var meDeatils = HttpContext.GetUser().User.UserDetails;
                var Deatils = this._userService.GetUserDetails(userid);
                Requestes Request = _FrindRequest.GetReque(meDeatils.PrimaryId, Deatils.PrimaryId);
                Requestes Request2 = _FrindRequest.GetReque(Deatils.PrimaryId, meDeatils.PrimaryId);
                if (key == 1)
                {

                    if (Requestdate == null)
                    {
                        return StatusCode(StatusCodes.Status406NotAcceptable,
                            new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                             _localizer["Request date is Required"], null));
                    }
                    if (Request == null && Request2 == null)
                    {
                        var Requestes = new Requestes
                        {
                            UserId = meDeatils.PrimaryId,
                            UserRequestId = Deatils.PrimaryId,
                            status = 0,
                            regestdata = Convert.ToDateTime(Requestdate),
                            Message = message
                        };
                        await _FrindRequest.addrequest(Requestes);
                        // FireBaseData fireBaseInfo = new FireBaseData() { Title = "Friend Request ", Body = meDeatils.userName + "  " + " sent Friend Request " };
                        FireBaseData fireBaseInfo = new FireBaseData()
                        {
                            muit = false,
                            Title = "Friendzr",
                            imageUrl = _configuration["BaseUrl"] + meDeatils.UserImage,
                            Body = "New friend request!",
                            Action_code = meDeatils.UserId,
                            Action = "Friend_Request"
                        };
                        try
                        {
                            SendNotificationcs sendNotificationcs = new SendNotificationcs();
                            if (Deatils.FcmToken != null)
                                await firebaseManager.SendNotification(Deatils.FcmToken, fireBaseInfo);
                            //await sendNotificationcs.SendMessageAsync(Deatils.FcmToken, " Request", fireBaseInfo, _environment.WebRootPath);

                            var addnoti = MessageServes.getFireBaseData(Deatils.PrimaryId, fireBaseInfo, Requestdate);
                            await MessageServes.addFireBaseDatamodel(addnoti);
                        }
                        catch
                        { }
                    }
                    return StatusCode(StatusCodes.Status200OK,
                           new ResponseModel<object>(StatusCodes.Status200OK, true,
                           "Your request has been sent", null));
                }
                else if (key == 2)
                {
                    Requestes Requestes = _FrindRequest.GetReque(meDeatils.PrimaryId, Deatils.PrimaryId);
                    var messageFriendRequest = Requestes?.Message;
                    if (Requestes != null)
                    {
                        if (Requestes.status == 0)
                        {
                            Requestes.status = 1;
                            Requestes.AcceptingDate = DateTime.Now;
                            Requestes.Message = null;
                            await _FrindRequest.updaterequest(Requestes);
                            FireBaseData fireBaseInfo = new FireBaseData()
                            {
                                muit = false,
                                Title = meDeatils.User.DisplayedUserName,
                                imageUrl = _configuration["BaseUrl"] + meDeatils.UserImage,
                                Body = " Accepted your friend request ",
                                Action_code = meDeatils.UserId,
                                Action = "Accept_Friend_Request"
                            };
                            UserMessages UserMessages = new UserMessages();

                            UserMessages.UserId = meDeatils.PrimaryId;
                            UserMessages.ToUserId = Deatils.PrimaryId;
                            UserMessages.startedin = DateTime.Now.Date;
                            string userMessageId = string.Empty;
                            var Messaghistory = MessageServes.getUserMessages(Deatils.PrimaryId, meDeatils.PrimaryId);
                            if (Messaghistory != null)
                            {
                                userMessageId = Messaghistory.Id;
                            }
                            if (Messaghistory == null)
                            {
                                 userMessageId = await MessageServes.addUserMessages(UserMessages, false);
                                
                            }
                            if (!string.IsNullOrEmpty(messageFriendRequest))
                            {
                                var messageData = new Messagedata
                                {
                                    Messagesdate = Requestes.regestdata.Date,
                                    Messagestime = Requestes.regestdata.TimeOfDay,
                                    linkable = false,
                                    EventDataid = null,
                                    UserMessagessId = userMessageId,
                                    UserId = Deatils.PrimaryId,
                                    Messages = messageFriendRequest,
                                    Messagetype = 1
                                };
                                 MessageServes.getUserMessages(Deatils.PrimaryId, meDeatils.PrimaryId, true);

                                var messageViewDto = await MessageServes.addMessagedata(messageData);
                            }
                            SendNotificationcs sendNotificationcs = new SendNotificationcs();
                            try
                            {
                                if (Deatils.FcmToken != null)
                                    // await sendNotificationcs.SendNotification(Deatils.FcmToken, " Request", fireBaseInfo, _environment.WebRootPath);
                                    await firebaseManager.SendNotification(Deatils.FcmToken, fireBaseInfo);
                                var addnoti = MessageServes.getFireBaseData(Deatils.PrimaryId, fireBaseInfo, Requestdate);
                                await MessageServes.addFireBaseDatamodel(addnoti);

                            }
                            catch { }
                            return StatusCode(StatusCodes.Status200OK,
                                 new ResponseModel<object>(StatusCodes.Status200OK, true,
                                 " successful Accept Request ", null));
                        }
                        else
                        {


                            return StatusCode(StatusCodes.Status200OK,
                              new ResponseModel<object>(StatusCodes.Status200OK, true,
                              " You has no Request from this Account", null));
                        }
                    }
                    else
                    {


                        return StatusCode(StatusCodes.Status200OK,
                          new ResponseModel<object>(StatusCodes.Status200OK, true,
                          " You has no Request from this Account", null));
                    }
                }
                else if (key == 3)
                {
                    Requestes Requestes = _FrindRequest.GetReque(meDeatils.PrimaryId, Deatils.PrimaryId);
                    var isWhitelable = meDeatils.IsWhiteLabel.HasValue && meDeatils.IsWhiteLabel.Value && (meDeatils.Code == Deatils.Code);
                    if (Requestes != null && !isNotFriend)
                    {
                        if(Requestes.status == 2)
                        {
                            return StatusCode(StatusCodes.Status406NotAcceptable,
                           new ResponseModel<object>(StatusCodes.Status200OK, true,
                           "The User Blocked Before", null));
                        }
                        Requestes.status = 2;
                            Requestes.UserblockId = meDeatils.PrimaryId;
                            Requestes.blockDate
                                = DateTime.Now.Date;
                            await _FrindRequest.updaterequest(Requestes);
                        return StatusCode(StatusCodes.Status200OK,
                           new ResponseModel<object>(StatusCodes.Status200OK, true,
                           " successful Blocked the User ", null));

                    }
                    // block un friend users
                    else if (Requestes == null && isNotFriend)
                    {
                        var request = new Requestes()
                        {
                            regestdata = DateTime.Now,
                            blockDate = DateTime.Now,
                            status = 2,                      
                           UserblockId = meDeatils.PrimaryId, 
                           UserRequestId = Deatils.PrimaryId,
                           UserId= meDeatils.PrimaryId,

                        };
                       
                        await _FrindRequest.addrequest(request);
                        return StatusCode(StatusCodes.Status200OK,
                           new ResponseModel<object>(StatusCodes.Status200OK, true,
                           " successful Blocked the User that is not Friend ", null));
                    }
                    // block whitelable user                   
                    else if (Requestes == null && isWhitelable)
                    {
                        var request = new Requestes()
                        {
                            regestdata = DateTime.Now,
                            blockDate = DateTime.Now,
                            status = 2,
                            UserblockId = meDeatils.PrimaryId,
                            UserRequestId = Deatils.PrimaryId,
                            UserId = meDeatils.PrimaryId,
                        };

                        await _FrindRequest.addrequest(request);
                        return StatusCode(StatusCodes.Status200OK,
                           new ResponseModel<object>(StatusCodes.Status200OK, true,
                           " successful Blocked the User that is in the Community ", null));
                    }
                    else
                    {


                        return StatusCode(StatusCodes.Status406NotAcceptable,
                          new ResponseModel<object>(StatusCodes.Status200OK, true,
                          " Please try again", null));
                    }

                }
                else if (key == 4)
                {
                    Requestes Requestes = _FrindRequest.GetReque(meDeatils.PrimaryId, Deatils.PrimaryId);
                    if (Requestes != null)
                    {
                        if (Requestes.status == 2)
                        {
                            await _FrindRequest.deleterequest(Requestes);

                            return StatusCode(StatusCodes.Status200OK,
                              new ResponseModel<object>(StatusCodes.Status200OK, true,
                              " successful unblock Request ", null));
                        }
                        else
                        {


                            return StatusCode(StatusCodes.Status200OK,
                              new ResponseModel<object>(StatusCodes.Status200OK, true,
                              " You are not blocked this Account ", null));
                        }
                    }
                    else
                    {


                        return StatusCode(StatusCodes.Status200OK,
                          new ResponseModel<object>(StatusCodes.Status200OK, true,
                          " You are not friend ", null));
                    }

                }
                else if (key == 5)
                {
                    Requestes Requestes = _FrindRequest.GetReque(meDeatils.PrimaryId, Deatils.PrimaryId);
                    if (Requestes != null)
                    {
                        await _FrindRequest.deleterequest(Requestes);

                        return StatusCode(StatusCodes.Status200OK,
                          new ResponseModel<object>(StatusCodes.Status200OK, true,
                          " successful unfriend  Request ", null));
                    }
                    else
                    {


                        return StatusCode(StatusCodes.Status200OK,
                          new ResponseModel<object>(StatusCodes.Status200OK, true,
                          "you unfriend this account successfully ", null));
                    }
                }
                else if (key == 6)
                {
                    Requestes Requestes = _FrindRequest.GetReque(meDeatils.PrimaryId, Deatils.PrimaryId);
                    if (Requestes != null)
                    {



                        await _FrindRequest.deleterequest(Requestes);
                        //FireBaseData fireBaseInfo = new FireBaseData()
                        //{
                        //    muit = false,
                        //    Title = "Friend request cancelled",
                        //    imageUrl = _configuration["BaseUrl"] + meDeatils.UserImage,
                        //    Body = "Friend request cancelled " + " from   " + meDeatils.User.DisplayedUserName,
                        //    Action_code = meDeatils.UserId,
                        //    Action = "Friend_request_cancelled"
                        //};
                        //    try
                        //    {
                        //        SendNotificationcs sendNotificationcs = new SendNotificationcs();
                        //        if (Deatils.FcmToken != null)
                        //            await firebaseManager.SendNotification(Deatils.FcmToken, fireBaseInfo);
                        //        //await sendNotificationcs.SendMessageAsync(Deatils.FcmToken, " Request", fireBaseInfo, _environment.WebRootPath);

                        //        //var addnoti = MessageServes.getFireBaseData(Deatils.PrimaryId, fireBaseInfo);
                        //        //await MessageServes.addFireBaseDatamodel(addnoti);
                        //    }
                        //    catch
                        //    { }
                        return StatusCode(StatusCodes.Status200OK,
                          new ResponseModel<object>(StatusCodes.Status200OK, true,
                         " you cancelled this account request", null));
                    }
                    else
                    {


                        return StatusCode(StatusCodes.Status200OK,
                          new ResponseModel<object>(StatusCodes.Status200OK, true,
                          " You are not friend ", null));
                    }
                }
                else
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                      new ResponseModel<object>(StatusCodes.Status406NotAcceptable, true,
                      " Key not found ", null));
                }
            }
            catch (Exception ex)
            {
                await _errorLogService.InsertErrorLog(new BWErrorLog(HttpContext.GetUser().UserId, "FrindRequest/RequestFriendStatus", ex));
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<object>(StatusCodes.Status500InternalServerError, false, ex.Message, null));
            }
        }


        [HttpPost]
        [Route("RequestFriend")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> UserFrind([FromForm] string userid)
        {
            try
            {
                var meDeatils = HttpContext.GetUser().User.UserDetails;
                var Deatils = this._userService.GetUserDetails(userid);
                Requestes Requestes = new Requestes
                {
                    UserId = meDeatils.PrimaryId,
                    UserRequestId = Deatils.PrimaryId,
                    status = 0,
                    regestdata = DateTime.Now.Date
                };
                await _FrindRequest.addrequest(Requestes);
                FireBaseData fireBaseInfo = new FireBaseData() 
                    { muit = false, Title = "Friend Request ",
                        imageUrl = _configuration["BaseUrl"] + meDeatils.UserImage,
                        Body = meDeatils.userName + "  " + " sent Friend Request ",
                        Action_code = Deatils.UserId, Action = "Accept_Friend_Request"

                    };
                SendNotificationcs sendNotificationcs = new SendNotificationcs();
                try
                {
                    if (Deatils.FcmToken != null)
                        await firebaseManager.SendNotification(Deatils.FcmToken, fireBaseInfo);
                    // await sendNotificationcs.SendMessageAsync(Deatils.FcmToken, " Request", fireBaseInfo, _environment.WebRootPath);
                }
                catch
                { }
                var addnoti = MessageServes.getFireBaseData(Deatils.PrimaryId, fireBaseInfo);
                await MessageServes.addFireBaseDatamodel(addnoti);
                return StatusCode(StatusCodes.Status200OK,
                      new ResponseModel<object>(StatusCodes.Status200OK, true,
                      "Your request has been sent", null));
            }
            catch (Exception ex)
            {
                await _errorLogService.InsertErrorLog(new BWErrorLog(HttpContext.GetUser().UserId, "FrindRequest/UserFrind", ex));
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<object>(StatusCodes.Status500InternalServerError, false, ex.Message, null));
            }
        }


        [HttpPost]
        [Route("AcceptRequestFriend")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> AcceptUserFrind([FromForm] string userid)
        {
            try
            {
                var meDeatils = HttpContext.GetUser().User.UserDetails;
                var Deatils = this._userService.GetUserDetails(userid);
                Requestes Requestes = _FrindRequest.GetReque(meDeatils.PrimaryId, Deatils.PrimaryId);
                Requestes.status = 1;
                Requestes.AcceptingDate = DateTime.Now;
                await _FrindRequest.updaterequest(Requestes);

                return StatusCode(StatusCodes.Status200OK,
                      new ResponseModel<object>(StatusCodes.Status200OK, true,
                      " successful Accept Request ", null));
            }
            catch (Exception ex)
            {
                await _errorLogService.InsertErrorLog(new BWErrorLog(HttpContext.GetUser().UserId, "FrindRequest/AcceptUserFrind", ex));
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<object>(StatusCodes.Status500InternalServerError, false, ex.Message, null));
            }
        }


        [HttpPost]
        [Route("blockRequestFriend")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> blockRequestFriend([FromForm] string userid)
        {
            try
            {
                var meDeatils = HttpContext.GetUser().User.UserDetails;
                var Deatils = this._userService.GetUserDetails(userid);
                Requestes Requestes = _FrindRequest.GetReque(meDeatils.PrimaryId, Deatils.PrimaryId);
                Requestes.status = 2;
                Requestes.UserblockId = meDeatils.PrimaryId;
                await _FrindRequest.updaterequest(Requestes);
                return StatusCode(StatusCodes.Status200OK, new ResponseModel<object>(StatusCodes.Status200OK, true, "successful block Request ", null));
            }
            catch (Exception ex)
            {
                await _errorLogService.InsertErrorLog(new BWErrorLog(HttpContext.GetUser().UserId, "FrindRequest/BlockRequestFriend", ex));
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<object>(StatusCodes.Status500InternalServerError, false, ex.Message, null));
            }
        }


        [HttpGet]
        [Route("RecommendedPeople")]
        public async Task<IActionResult> RecommendedPeople([FromQuery] string userId)
        {
            try
            {
                var userDetails = HttpContext.GetUser().User.UserDetails;               

                var (people, message) = await _userService.RecommendedPeopleFix(userDetails, userId);

                return StatusCode(StatusCodes.Status200OK, new ResponseModel<RecommendedPeopleViewModel>(StatusCodes.Status200OK, true, message, people));
            }
            catch (Exception ex)
            {
                await _errorLogService.InsertErrorLog(new BWErrorLog(HttpContext.GetUser().UserId, "FrindRequest/RecommendedPeople", ex));
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<RecommendedPeopleViewModel>(StatusCodes.Status500InternalServerError, false, ex.Message, null));
            }
        }


        [HttpGet]
        [Route("RecentlyConnected")]
        public async Task<IActionResult> RecentlyConnected([FromQuery] int pageNumber =1, [FromQuery] int pageSize = 10)
        {
            try
            {
                UserDetails userDeatil = HttpContext.GetUser().User.UserDetails;

                (List<RecentlyConnectedViewModel> recentlyConnected, string message , int totalRowCount) response = await _userService.RecentlyConnected(userDeatil, pageNumber , pageSize);

                var totalPages = ((double)response.totalRowCount / (double)pageSize);
                int roundedTotalPages = Convert.ToInt32(Math.Ceiling(totalPages));

                return StatusCode(StatusCodes.Status200OK, new ResponseModel<object>(StatusCodes.Status200OK, true, response.message, new
                {
                    pageNumber = pageNumber,
                    pageSize = pageSize,
                    totalPages = roundedTotalPages,
                    totalRecords = response.totalRowCount,
                    data = response.recentlyConnected
                }));
            }
            catch (Exception ex)
            {
                await _errorLogService.InsertErrorLog(new BWErrorLog(HttpContext.GetUser().UserId, "FrindRequest/RecentlyConnected", ex));
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<List<RecentlyConnectedViewModel>>(StatusCodes.Status500InternalServerError, false, ex.Message, null));
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

        

    }
}
