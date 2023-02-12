using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using Social.Entity.DBContext;
using Social.Entity.Enums;
using Social.Entity.Models;
using Social.Entity.ModelView;
using Social.FireBase;
using Social.Models;
using Social.Sercices.Helpers;
using Social.Services;
using Social.Services.Services;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Social.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    public class AuthenticatController : ControllerBase
    {
        readonly string BaseUrlDomain;
        private readonly IEventServ _Event;
        private readonly IUserService _userService;
        private readonly EmailHelper _emailHelper;
        private readonly IFrindRequest _FrindRequest;
        private readonly AuthDBContext authDBContext;
        private readonly IMessageServes MessageServes;
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> userManager;
        private readonly IErrorLogService _errorLogService;
        private readonly IGlobalMethodsService globalMethodsService;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly IAppConfigrationService appConfigrationService;
        public AuthenticatController(IMessageServes MessageServes, IEventServ Event, AuthDBContext authDBContext, IFrindRequest FrindRequest, UserManager<User> userManager,IConfiguration configuration, IGlobalMethodsService globalMethodsService, IUserService userService, EmailHelper emailHelper,IErrorLogService errorLogService, IStringLocalizer<SharedResource> localizer, IAppConfigrationService appConfigrationService)
        {
            this._Event = Event;
            _localizer = localizer;
            _userService = userService;
            _emailHelper = emailHelper;
            _FrindRequest = FrindRequest;
            this.userManager = userManager;
            _configuration = configuration;
            _errorLogService = errorLogService;
            this.authDBContext = authDBContext;
            this.MessageServes = MessageServes;
            this.globalMethodsService = globalMethodsService;
            this.appConfigrationService = appConfigrationService;
            BaseUrlDomain = globalMethodsService.GetBaseDomain();

        }

        [HttpPost]
        [Route("register")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> Register([FromForm] RegisterModel model)
        {
            try
            {
                var appcon = appConfigrationService.GetData().FirstOrDefault();
                Random random1 = new Random();
                var code1 = random1.Next(000000, 9) + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Second;
                Match matchMail = Emailvalidation(model);
                if (!matchMail.Success)
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                        new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,
                         _localizer["emailincorrect"], null));
                }
                bool matchMail2 = emailvali2(model);
                if (matchMail2)
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                        new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                         _localizer["emailincorrect"], null));
                }
                if (model.registertype == "0" || model.registertype == "" || model.registertype == null)
                {
                    if (model.UserName == "" || model.UserName == null)
                    {
                        return StatusCode(StatusCodes.Status406NotAcceptable,
                            new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                             _localizer["usernamefieldisemptyitismusthavevalue"], null));
                    }
                    if (model.Password == "" || model.Password == null)
                    {
                        return StatusCode(StatusCodes.Status406NotAcceptable,
                            new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                             _localizer["Passwordfieldisemptyitismusthavevalue"], null));
                    }
                    // no duplicate email
                    var emailExists = await userManager.FindByEmailAsync(model.Email);
                    if (emailExists != null)
                    {
                        return StatusCode(StatusCodes.Status406NotAcceptable,
                            new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                             _localizer["Email already in use."], null));
                    }

                    if (appcon != null ? (appcon.UserName_MaxLength != null) : false)
                    {


                        if (appcon.UserName_MaxLength < model.UserName.Length || appcon.UserName_MinLength > model.UserName.Length)
                        {
                            return StatusCode(StatusCodes.Status406NotAcceptable,
                                new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                                 _localizer["User Name Min Length is  "] + appcon.UserName_MinLength + _localizer["  User Name max Length is  "] + appcon.UserName_MaxLength, null));
                        }
                    }
                    if (appcon != null ? (appcon.Password_MaxLength != null) : false)
                    {


                        if (appcon.Password_MaxLength < model.Password.Length || appcon.Password_MinLength > model.Password.Length)
                        {
                            return StatusCode(StatusCodes.Status406NotAcceptable,
                                new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                                 _localizer["Password Min Length  is  "] + appcon.UserName_MinLength + _localizer[" Password Max Length  is  "] + appcon.UserName_MaxLength, null));
                        }
                    }
                    Regex regexpassword = new Regex(@"^(?=^.{6,}$)(?=.*\d)(?=.*[a-zA-Z])");

                    bool matchpassword = regexpassword.IsMatch(model.Password);
                    if (!matchpassword)
                    {
                        return StatusCode(StatusCodes.Status406NotAcceptable,
                            new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                             _localizer["PasswordContent"], null));
                    }
                    User user = new User()
                    {
                        Email = model.Email,
                        SecurityStamp = Guid.NewGuid().ToString(),
                        DisplayedUserName = model.UserName,
                        logintypevalue = model.registertype,
                        UserloginId = model.UserID,
                        PasswordHash = model.Password,
                        UserName = (model.UserName.Replace(" ", "-") + code1).Replace(" ", "-")

                    };

                    var result = await userManager.CreateAsync(user, model.Password);

                    if (!result.Succeeded)
                    {
                        if (result.Errors.FirstOrDefault().Description == "Passwords must be at least 8 characters.")
                        {
                            return StatusCode(StatusCodes.Status406NotAcceptable,
                            new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                             _localizer["passwordvalid"], null
                          ));
                        }
                        return StatusCode(StatusCodes.Status406NotAcceptable,
                             new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,
                             result.Errors.FirstOrDefault().Description, null
                            ));
                    }
                    var CreatedUser = await userManager.FindByEmailAsync(model.Email);
                    // details
                    var userDetails = new UserDetails
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = CreatedUser.Id,
                        Email = model.Email,
                        FcmToken = model.FcmToken,
                        userName = model.UserName,
                        userlogintypeid = model.UserID,
                        pasword = model.Password,

                        platform = model.platform,
                        allowmylocation = true,
                        distanceFilter = true,
                        Manualdistancecontrol = Convert.ToDecimal(appcon.DistanceShowNearbyAccountsInFeed_Max),
                        pushnotification = true,
                        Filteringaccordingtoage = true,
                        agefrom = (int)appcon.AgeFiltering_Min,
                        whatAmILookingFor = model.whatAmILookingFor,
                        ageto = (int)appcon.AgeFiltering_Max,
                        ProfileCompleted = false,
                        // UserImage = imageName != null ? "/Images/" + imageName : "/Images/WhatsApp Image 2021-06-29 at 10.14.15 PM.jpeg",
                    };
                    this._userService.InsertUserDetails(userDetails);

                    Random random = new Random();
                    var code = random.Next(1, 1000000).ToString("D6");
                    var token = await GenerateEmailConfirmationToken(user, Convert.ToInt32(code.ToString()));
                    return StatusCode(StatusCodes.Status200OK,
                          new ResponseModel<object>(StatusCodes.Status200OK, true,

                           _localizer["successfullyregistered"], new { token = token, code = code }));

                }
                // Register by Social Media
                else
                {

                    if (model.UserID != null)
                    {
                        var user = userManager.Users.FirstOrDefault(u => u.UserloginId == model.UserID || u.Email == model.Email);

                        model.Password = (model.Password == null || model.Password == "") ? model.UserID : model.Password;

                        if (user == null)
                        {
                            user = new User()
                            {
                                Email = model.Email,
                                SecurityStamp = Guid.NewGuid().ToString(),
                                UserName = (model.UserName == "" || model.UserName == null) ? ("UserName" + code1).Replace(" ", "-") : (model.UserName + code1).Replace(" ", "-"),
                                logintypevalue = model.registertype,
                                UserloginId = model.UserID,
                                PasswordHash = model.Password,
                                DisplayedUserName = (model.UserName == "" || model.UserName == null) ? ("User Name").Replace(" ", "-") : model.UserName,
                                EmailConfirmedOn = DateTime.Now,
                                EmailConfirmed = true
                            };

                            var result = await userManager.CreateAsync(user, model.Password);

                            if (!result.Succeeded)
                            {
                                if (result.Errors.FirstOrDefault().Description == "Passwords must be at least 8 characters.")
                                {
                                    return StatusCode(StatusCodes.Status406NotAcceptable,
                                    new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                                     _localizer["passwordvalid"], null
                                  ));
                                }
                                return StatusCode(StatusCodes.Status406NotAcceptable,
                                     new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,
                                     result.Errors.FirstOrDefault().Description, null
                                    ));
                            }
                            var CreatedUser = await userManager.FindByEmailAsync(model.Email);
                            // details
                            var userDetails = new UserDetails
                            {
                                Id = Guid.NewGuid().ToString(),
                                UserId = CreatedUser.Id,
                                Email = model.Email,
                                FcmToken = model.FcmToken,
                                userName = (model.UserName == "" || model.UserName == null) ? ("UserName" + code1).Replace(" ", "-") : model.UserName,

                                userlogintypeid = model.UserID,
                                pasword = model.Password,
                                platform = model.platform,
                                allowmylocation = true,
                                distanceFilter = true,
                                ImageIsVerified = false,
                                Filteringaccordingtoage = true,
                                agefrom = (int)appcon.AgeFiltering_Min,
                                whatAmILookingFor = model.whatAmILookingFor,
                                ageto = (int)appcon.AgeFiltering_Max,
                                Manualdistancecontrol = Convert.ToDecimal(appcon.DistanceShowNearbyAccountsInFeed_Max),
                                pushnotification = true,
                                ProfileCompleted = false,
                                // Manualdistancecontrol = 3
                                //UserImage = imageName != null ? "/Images/" + imageName : "/Images/WhatsApp Image 2021-06-29 at 10.14.15 PM.jpeg",
                            };
                            this._userService.InsertUserDetails(userDetails);



                        }

                        var token = "";
                        if (user != null)
                            if (user.EmailConfirmed)
                            {
                                return StatusCode(StatusCodes.Status406NotAcceptable,
                                 new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                              _localizer["UserAlreadyExist"], null));
                            }
                        //token = await GenerateAuthToken(user);
                        // for my test
                        Random random = new Random();
                        var code = random.Next(1, 1000000).ToString("D6");

                        token = await GenerateEmailConfirmationToken(user, Convert.ToInt32(code.ToString()));

                        var userDeatils = this._userService.GetUserDetails(user.Id);
                        var image = userDeatils.UserImage;
                        userDeatils.FcmToken = model.FcmToken;
                        userDeatils.Email = model.Email;
                        userDeatils.userlogintypeid = model.UserID;
                        user.EmailConfirmed = true;

                        user.UserloginId = model.UserID;
                        user.Email = model.Email;
                        await userManager.UpdateAsync(user);
                        this._userService.UpdateUserDetails(userDeatils);
                        SendNotificationcs sendNotificationcs = new SendNotificationcs();

                        return StatusCode(StatusCodes.Status200OK,
                      new ResponseModel<object>(StatusCodes.Status200OK, true,

                       _localizer["successfullyregistered"], new
                       {
                           token = token,
                           code = "",
                           NeedUpdate = userDeatils.birthdate == null ? 1 : 0,
                           UserName = user.DisplayedUserName,
                           DisplayedUserName = user.UserName,
                           email = user.Email,
                           UserID = user.UserloginId,
                           phoneNumber = user.PhoneNumber,
                           allowmylocation = userDeatils.allowmylocation,
                           MyAppearanceTypes = userDeatils.AppearanceTypes.Select(x => x.AppearanceTypeID).ToList(),
                           ImageIsVerified = userDeatils.ImageIsVerified,
                           ghostmode = userDeatils.ghostmode,
                           pushnotification = userDeatils.pushnotification,
                           language = userDeatils.language,
                           whatAmILookingFor = model.whatAmILookingFor,
                           userImage =string.IsNullOrEmpty(image) ? _configuration["DefaultImage"] : _configuration["BaseUrl"] + image,
                           Manualdistancecontrol = userDeatils.Manualdistancecontrol,
                           agefrom = userDeatils.agefrom,
                           ageto = userDeatils.ageto,


                       }));
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status406NotAcceptable,
                          new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                           _localizer["useridinvalid"], null));
                    }

                }
            }
            catch (Exception ex)
            {
                var log = new BWErrorLog
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = null,
                    Api = "Auth/Register",
                    ApiParams = "userNamr : " + model.UserName + "email  : " + model.Email +
                   "Pass : " + model.Password,
                    Exception = ex.ToString(),
                    ExMsg = ex.Message,
                    ExStackTrace = ex.StackTrace,
                    InnerException = ex.InnerException == null ? null : ex.InnerException.ToString(),
                    InnerExMsg = ex.InnerException == null ? null : ex.InnerException.Message,
                    InnerExStackTrace = ex.InnerException == null ? null : ex.InnerException.StackTrace,
                    CreatedOn = DateTime.Now
                };

                await this._errorLogService.InsertErrorLog(log);

                if (ex.InnerException != null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError,
                               new ResponseModel<object>(StatusCodes.Status500InternalServerError, false,
                               ex.InnerException.Message, null));
                }
                return StatusCode(StatusCodes.Status500InternalServerError,
                                new ResponseModel<object>(StatusCodes.Status500InternalServerError, false,
                                ex.Message, null));
            }

        }

        private static bool emailvali2(RegisterModel model)
        {
            Regex regexMail2 = new Regex(@"^([\u0600-\u06ff]\?)$");

            bool matchMail2 = Regex.IsMatch(model.Email, @"\p{IsArabic}");
            return matchMail2;
        }

        private static Match Emailvalidation(RegisterModel model)
        {
            Regex regexMail = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");

            Match matchMail = regexMail.Match(model.Email.ToString());
            return matchMail;
        }

        [HttpPost]
        [Route("CheckUserName")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> CheckUserName([FromForm] string UserName)
        {
            try
            {
                var UserNameEXists = userManager.Users.FirstOrDefault(u => u.UserName == UserName);
                if (UserNameEXists != null)
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                        new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                         _localizer["UserNameexists"], null));
                }
                else
                {
                    return StatusCode(StatusCodes.Status200OK,
                        new ResponseModel<object>(StatusCodes.Status200OK, true,

                        _localizer["User Name not exists"], null));
                }

            }
            catch (Exception ex)
            {
                var log = new BWErrorLog
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = null,
                    Api = "Auth/CheckUserName",
                    ApiParams = "fcmToken : " + UserName,
                    Exception = ex.ToString(),
                    ExMsg = ex.Message,
                    ExStackTrace = ex.StackTrace,
                    InnerException = ex.InnerException == null ? null : ex.InnerException.ToString(),
                    InnerExMsg = ex.InnerException == null ? null : ex.InnerException.Message,
                    InnerExStackTrace = ex.InnerException == null ? null : ex.InnerException.StackTrace,
                    CreatedOn = DateTime.Now
                };

                await this._errorLogService.InsertErrorLog(log);
                return StatusCode(StatusCodes.Status500InternalServerError,
                                 new ResponseModel<object>(StatusCodes.Status500InternalServerError, false,
                                 ex.Message, null));
            }
        }

        [HttpPost]
        [Route("login")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> Login([FromForm] LoginModel model)
        {
            //var txt = _localizer["Login"];
            //Random random1 = new Random();

            //Add Code instead of Numbers Only
            var code1 = Guid.NewGuid().ToString().Substring(9,4);//random1.Next(000000, 9) + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Second;

            //TODO: Fix Dublicate Code In Database 

            try
            {
                if (model.logintype == "0" || model.logintype == null || model.logintype == "")
                {
                    var user = userManager.Users.FirstOrDefault(u => u.Email == model.Email);

                    if (model.Password == "" || model.Password == null)
                    {
                        return StatusCode(StatusCodes.Status406NotAcceptable,
                            new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,
                             _localizer["Passwordfieldisemptyitismusthavevalue"], null));
                    }

                    if (user != null && await userManager.CheckPasswordAsync(user, model.Password))
                    {
                        if ((await userManager.IsInRoleAsync(user, StaticApplicationRoles.Admin.ToString())) || (await userManager.IsInRoleAsync(user, StaticApplicationRoles.SuperAdmin.ToString())))
                        {
                            return StatusCode(StatusCodes.Status406NotAcceptable,
                            new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                             _localizer["checkyourE-mailorpassword!"], null
                            ));
                        }
                        if (user.UserDetails.BanFrom != null)
                        {
                            if (user.UserDetails.BanFrom.Value <= DateTime.Now.Date && user.UserDetails.BanTo.Value >= DateTime.Now.Date)
                            {
                                return StatusCode(StatusCodes.Status406NotAcceptable,
                                    new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,
                                     _localizer["The user has been suspended "], null));
                            }
                        }
                        //checkconfirmation email if true login
                        //if false redirect to confrm mail
                        if (user.UserDetails.IsActive == false)
                        {
                            return StatusCode(StatusCodes.Status400BadRequest,
                          new ResponseModel<object>(StatusCodes.Status400BadRequest, false,
                           _localizer["Account temporary stopped contact with admin"], null));
                        }
                        if (!await userManager.IsEmailConfirmedAsync(user))
                        {

                            // generate code
                            Random random = new Random();
                            var code = random.Next(1, 1000000).ToString("D6");

                            // generate email confirmation token
                            var emailToken = await GenerateEmailConfirmationToken(user, Convert.ToInt32(code.ToString()));


                            return StatusCode(StatusCodes.Status307TemporaryRedirect,
                          new ResponseModel<object>(StatusCodes.Status307TemporaryRedirect, false,
                           _localizer["emailhasnotconfirmed"], null));
                        }


                        var token = await GenerateAuthToken(user);

                        var userDeatils = this._userService.GetUserDetails(user.Id);
                        var image = userDeatils.UserImage;
                        userDeatils.FcmToken = model.FcmToken ?? userDeatils.FcmToken;
                        this._userService.UpdateUserDetails(userDeatils);
                        SendNotificationcs sendNotificationcs = new SendNotificationcs();
                        //if(userDeatils.FcmToken != null)
                        //await sendNotificationcs.SendMessageAsync(userDeatils.FcmToken, "Welcome Message", fireBaseInfo, _environment.WebRootPath);
                        if (Request.Cookies["Usre_Culture"] == null)
                        {
                            CookieOptions option = new CookieOptions();
                            option.Expires = DateTime.Now.AddDays(15);
                            Response.Cookies.Append("Usre_Culture", userDeatils.language == "Fr" ? "fr-FR" : (userDeatils.language == "En" ? "en-US" : "ar-EG"), option);
                        }
                        var data = MessageServes.getFireBasecount(userDeatils.PrimaryId);
                        return StatusCode(StatusCodes.Status200OK,
                      new ResponseModel<object>(StatusCodes.Status200OK, true,

                       _localizer["DONE"], new
                       {
                           token = token,
                           code = "",
                           NeedUpdate = (userDeatils.IsWhiteLabel.Value || userDeatils.birthdate!=null)?0:1,
                           UserName = user.DisplayedUserName,
                           DisplayedUserName = user.UserName,
                           email = user.Email,
                           //UserID = user.UserloginId,
                           phoneNumber = user.PhoneNumber,
                           FrindRequestNumber = _FrindRequest.GetallRequestes(userDeatils.PrimaryId, RequestesType.RecivedOnly).Where(m => m.status == 0).Count(),

                           notificationcount = data,
                           Message_Count = MessageServes.messagelogincount(userDeatils.UserId),
                           userImage = string.IsNullOrEmpty(image) ? _configuration["DefaultImage"] : _configuration["BaseUrl"] + image,
                           Manualdistancecontrol = userDeatils.Manualdistancecontrol,
                           agefrom = userDeatils.agefrom,
                           ageto = userDeatils.ageto,
                           Filteringaccordingtoage = userDeatils.Filteringaccordingtoage,

                           allowmylocation = userDeatils.allowmylocation,
                           ImageIsVerified = userDeatils.ImageIsVerified,

                           MyAppearanceTypes = userDeatils.AppearanceTypes.Select(x => x.AppearanceTypeID).ToList(),

                           ghostmode = userDeatils.ghostmode,
                           pushnotification = userDeatils.pushnotification,
                           language = userDeatils.language,
                           isWhiteLable= userDeatils.IsWhiteLabel,
                           interests= userDeatils.listoftags?.Select(i=>i.EntityId).ToList(),
                       })); ;

                    }

                    return StatusCode(StatusCodes.Status406NotAcceptable,
                         new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                          _localizer["checkyourE-mailorpassword!"], null
                         ));
                }
                else
                {
                    return await registrationandlogin(model, code1);

                }
            }

            catch (Exception ex)
            {
                var log = new BWErrorLog
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = null,
                    Api = "Auth/Login",
                    ApiParams = "Email : " + model.Email + "Pass  : " + model.Password,
                    Exception = ex.ToString(),
                    ExMsg = ex.Message,
                    ExStackTrace = ex.StackTrace,
                    InnerException = ex.InnerException == null ? null : ex.InnerException.ToString(),
                    InnerExMsg = ex.InnerException == null ? null : ex.InnerException.Message,
                    InnerExStackTrace = ex.InnerException == null ? null : ex.InnerException.StackTrace,
                    CreatedOn = DateTime.Now
                };

                await this._errorLogService.InsertErrorLog(log);
                return StatusCode(StatusCodes.Status500InternalServerError,
                                 new ResponseModel<object>(StatusCodes.Status500InternalServerError, false,
                                 ex.Message, null));
            }
        }

        private async Task<IActionResult> registrationandlogin(LoginModel model, string code)
        {
            var appcon = appConfigrationService.GetData().FirstOrDefault();
            if (model.UserId != null)
            {
                var user = userManager.Users.FirstOrDefault(u => u.UserloginId == model.UserId || u.Email == model.Email);

                model.Password = (model.Password == null || model.Password == "") ? model.UserId : model.Password;

                if (user == null)
                {
                    user = new User()
                    {
                        Email = model.Email,
                        SecurityStamp = Guid.NewGuid().ToString(),
                        UserName = (model.UserName == "" || model.UserName == null) ? ("UserName_" + code).Replace(" ", "-") : (model.UserName + code).Replace(" ", "-"),
                        logintypevalue = model.logintype,
                        UserloginId = model.UserId,
                        PasswordHash = model.Password,
                        DisplayedUserName = (model.UserName == "" || model.UserName == null) ? ("UserName").Replace(" ", "-") : model.UserName,
                        EmailConfirmedOn = DateTime.Now,
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(user, model.Password);

                    if (!result.Succeeded)
                    {
                        if (result.Errors.FirstOrDefault().Description == "Passwords must be at least 8 characters.")
                        {
                            return StatusCode(StatusCodes.Status406NotAcceptable,
                            new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                             _localizer["passwordvalid"], null
                          ));
                        }
                        return StatusCode(StatusCodes.Status406NotAcceptable,
                             new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,
                             result.Errors.FirstOrDefault().Description, null
                            ));
                    }
                    var CreatedUser = await userManager.FindByEmailAsync(model.Email);
                    // details
                    var userDetails = new UserDetails
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = CreatedUser.Id,
                        Email = model.Email,
                        FcmToken = model.FcmToken,
                        userName = (model.UserName == "" || model.UserName == null) ? ("UserName" + code).Replace(" ", "-") : model.UserName,
                        Filteringaccordingtoage = true,
                        agefrom = (int)appcon.AgeFiltering_Min,
                        whatAmILookingFor = model.whatAmILookingFor,
                        ageto = (int)appcon.AgeFiltering_Max,
                        userlogintypeid = model.UserId,
                        pasword = model.Password,
                        platform = model.platform,
                        allowmylocation = true,
                        distanceFilter = true,
                        pushnotification = true,
                        ProfileCompleted = false,
                        Manualdistancecontrol = Convert.ToDecimal(appcon.DistanceShowNearbyAccountsInFeed_Max)
                        // UserImage = imageName != null ? "/Images/" + imageName : "/Images/WhatsApp Image 2021-06-29 at 10.14.15 PM.jpeg",
                    };
                    this._userService.InsertUserDetails(userDetails);


                }

                var token = "";
                if (user != null)
                {
                    token = await GenerateAuthToken(user);
                    if (user.UserDetails.BanFrom != null)
                    {
                        if (user.UserDetails.BanFrom.Value <= DateTime.Now.Date && user.UserDetails.BanTo.Value >= DateTime.Now.Date)
                        {
                            return StatusCode(StatusCodes.Status406NotAcceptable,
                                new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,
                                 _localizer["The user has been suspended "], null));
                        }
                    }
                }

                var userDeatils = this._userService.GetUserDetails(user.Id);
                if (user.UserDetails.IsActive == false)
                {
                    return StatusCode(StatusCodes.Status400BadRequest,
                  new ResponseModel<object>(StatusCodes.Status400BadRequest, false,
                   _localizer["Account temporary stopped contact with admin"], null));
                }
                //if (userDeatils == null)
                //{
                //    userDeatils = new UserDetails
                //    {
                //        Id = Guid.NewGuid().ToString(),
                //        UserId = user.Id,
                //        Email = model.Email,
                //        FcmToken = model.FcmToken,
                //        userName = (model.UserName == "" || model.UserName == null) ? ("UserName" + code1).Replace(" ", "-") : model.UserName,
                //        Filteringaccordingtoage = true,
                //        agefrom = 14,

                //        ageto = 85,
                //        userlogintypeid = model.UserId,
                //        pasword = model.Password,
                //        platform = model.platform,
                //        allowmylocation = true,
                //        pushnotification = true,
                //        Manualdistancecontrol = Convert.ToDecimal("0.25")
                //        // UserImage = imageName != null ? "/Images/" + imageName : "/Images/WhatsApp Image 2021-06-29 at 10.14.15 PM.jpeg",
                //    };
                //}
                var image = userDeatils?.UserImage;
                userDeatils.FcmToken = model.FcmToken;
                //userDeatils.Email = model.Email;
                //   userDeatils.userlogintypeid = model.UserId;
                user.EmailConfirmed = true;

                //user.UserloginId = model.UserId;
                //  user.Email = model.Email;
                await userManager.UpdateAsync(user);
                this._userService.UpdateUserDetails(userDeatils);
                SendNotificationcs sendNotificationcs = new SendNotificationcs();
                var data = MessageServes.getFireBasecount(userDeatils.PrimaryId);
                return StatusCode(StatusCodes.Status200OK,
              new ResponseModel<object>(StatusCodes.Status200OK, true,

               _localizer["successfullyregistered"], new
               {
                   token = token,
                   code = "",
                   NeedUpdate = userDeatils.birthdate == null ? 1 : 0,
                   UserName = user.DisplayedUserName,
                   DisplayedUserName = user.UserName,
                   email = user.Email,
                   //UserID = user.UserloginId,
                   phoneNumber = user.PhoneNumber,
                   allowmylocation = userDeatils.allowmylocation,
                   MyAppearanceTypes = userDeatils.AppearanceTypes.Select(x => x.AppearanceTypeID).ToList(),
                   ghostmode = userDeatils.ghostmode,
                   pushnotification = userDeatils.pushnotification,
                   language = userDeatils.language,
                  
                   Iam = user.UserDetails.WhatBestDescripsMeList.Select(q => q.WhatBestDescripsMe.EntityId).ToList(),
                   PreferToList = user.UserDetails.Iprefertolist.Select(q => q.preferto.EntityId ).ToList(),
                   Interests = user.UserDetails.listoftags.Select(q => q.Interests.EntityId).ToList(),

                   userImage = _configuration["BaseUrl"] + image,
                   Manualdistancecontrol = userDeatils.Manualdistancecontrol,
                   agefrom = userDeatils.agefrom,
                   ageto = userDeatils.ageto,
                   Filteringaccordingtoage = userDeatils.Filteringaccordingtoage,
                   FrindRequestNumber = _FrindRequest.GetallRequestes(userDeatils.PrimaryId, RequestesType.RecivedOnly).Where(m => m.status == 0).Count(),
                   Message_Count = MessageServes.messagelogincount(userDeatils.UserId),
                   notificationcount = data,

               }));
            }
            else
            {
                return StatusCode(StatusCodes.Status406NotAcceptable,
                  new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                   _localizer["useridinvalid"], null));
            }
        }

        [HttpPost]
        [Route("UpdateFcmToken")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> UpdateFcmToken([FromForm] string fcmToken)
        {
            try
            {
                StringValues authorizationToken;
                HttpContext.Request.Headers.TryGetValue("Authorization", out authorizationToken);

                var user = await this._userService.GetLoggedInUser(authorizationToken);
                if (user != null)
                {
                    var userDeatils = this._userService.GetUserDetails(user.UserId);
                    userDeatils.FcmToken = fcmToken;
                    this._userService.UpdateUserDetails(userDeatils);
                }
                return StatusCode(StatusCodes.Status401Unauthorized,
                       new ResponseModel<object>(StatusCodes.Status401Unauthorized, false,

                       _localizer["Pleasere-login"],
                       null));

            }
            catch (Exception ex)
            {
                var log = new BWErrorLog
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = null,
                    Api = "Auth/UpdateFcmToken",
                    ApiParams = "fcmToken : " + fcmToken,
                    Exception = ex.ToString(),
                    ExMsg = ex.Message,
                    ExStackTrace = ex.StackTrace,
                    InnerException = ex.InnerException == null ? null : ex.InnerException.ToString(),
                    InnerExMsg = ex.InnerException == null ? null : ex.InnerException.Message,
                    InnerExStackTrace = ex.InnerException == null ? null : ex.InnerException.StackTrace,
                    CreatedOn = DateTime.Now
                };

                await this._errorLogService.InsertErrorLog(log);
                return StatusCode(StatusCodes.Status500InternalServerError,
                                 new ResponseModel<object>(StatusCodes.Status500InternalServerError, false,
                                 ex.Message, new
                                 {
                                     token = "",
                                     code = ""
                                 }));
            }
        }

        [HttpPost]
        [Route("forgetpass")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> ForgetPassword([FromForm] string email)
        {
            try
            {

                var user = await userManager.FindByEmailAsync(email);

                if (user == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound,
                    new ResponseModel<object>(StatusCodes.Status404NotFound, false,

                     _localizer["emailincorrect"], null));
                }

                // generate code
                Random random = new Random();
                var code = random.Next(1, 1000000).ToString("D6");


                // checkconfirmation email if true  generate pass token
                // if false redirect to confrm mail
                if (!await userManager.IsEmailConfirmedAsync(user))
                {

                    // generate email confirmation token
                    var emailToken = await GenerateEmailConfirmationToken(user, Convert.ToInt32(code.ToString()));

                    return StatusCode(StatusCodes.Status307TemporaryRedirect,
                      new ResponseModel<object>(StatusCodes.Status307TemporaryRedirect, false,

                       _localizer["emailhasnotconfirmed"], null));
                }

                //generate pass reset token 
                var token = await GeneratePassResetToken(user, Convert.ToInt32(code.ToString()));

                return StatusCode(StatusCodes.Status200OK,
                      new ResponseModel<object>(StatusCodes.Status200OK, true,

                       _localizer["codesentsuccessfully"], new { token = token, code = code }));

            }
            catch (Exception ex)
            {
                var log = new BWErrorLog
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = null,
                    Api = "Auth/forgetPass",
                    ApiParams = "email : " + email,
                    Exception = ex.ToString(),
                    ExMsg = ex.Message,
                    ExStackTrace = ex.StackTrace,
                    InnerException = ex.InnerException == null ? null : ex.InnerException.ToString(),
                    InnerExMsg = ex.InnerException == null ? null : ex.InnerException.Message,
                    InnerExStackTrace = ex.InnerException == null ? null : ex.InnerException.StackTrace,
                    CreatedOn = DateTime.Now
                };

                await this._errorLogService.InsertErrorLog(log);

                return StatusCode(StatusCodes.Status500InternalServerError,
                                new ResponseModel<object>(StatusCodes.Status500InternalServerError, false,
                                ex.Message, null));
            }
        }

        [HttpPost]
        [Route("ForgotPassword")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> ForgotPassword([FromForm] string Email)
        {
            try
            {

                var user = await userManager.FindByEmailAsync(Email);

                if (user == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound,
                    new ResponseModel<object>(StatusCodes.Status404NotFound, false,

                     _localizer["emailincorrect"], null));
                }

                // generate code
                Random random = new Random();
                var code = random.Next(1, 1000000).ToString("D6");


                // checkconfirmation email if true  generate pass token
                // if false redirect to confrm mail
                if (!await userManager.IsEmailConfirmedAsync(user))
                {

                    // generate email confirmation token
                    var emailToken = await GenerateEmailConfirmationToken(user, Convert.ToInt32(code.ToString()));

                    return StatusCode(StatusCodes.Status307TemporaryRedirect,
                      new ResponseModel<object>(StatusCodes.Status307TemporaryRedirect, false,

                       _localizer["emailhasnotconfirmed"], null));
                }

                //generate pass reset token 
                var token = await GeneratePassResetToken(user, Convert.ToInt32(code.ToString()));

                return StatusCode(StatusCodes.Status200OK,
                      new ResponseModel<object>(StatusCodes.Status200OK, true,

                       _localizer["codesentsuccessfully"], new { token = token, code = code }));

            }
            catch (Exception ex)
            {
                var log = new BWErrorLog
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = null,
                    Api = "Auth/forgetPass",
                    ApiParams = "email : " + Email,
                    Exception = ex.ToString(),
                    ExMsg = ex.Message,
                    ExStackTrace = ex.StackTrace,
                    InnerException = ex.InnerException == null ? null : ex.InnerException.ToString(),
                    InnerExMsg = ex.InnerException == null ? null : ex.InnerException.Message,
                    InnerExStackTrace = ex.InnerException == null ? null : ex.InnerException.StackTrace,
                    CreatedOn = DateTime.Now
                };

                await this._errorLogService.InsertErrorLog(log);

                return StatusCode(StatusCodes.Status500InternalServerError,
                                new ResponseModel<object>(StatusCodes.Status500InternalServerError, false,
                                ex.Message, null));
            }

            //OldCode
            //// Find the user by email
            ////var user = await userManager.FindByEmailAsync(Email);
            //var user = userManager.Users.FirstOrDefault(x => x.Email.ToLower() == Email.ToLower());
            //// If the user is found AND Email is confirmed
            //if (user != null && await userManager.IsEmailConfirmedAsync(user))
            //{
            //    // Generate the reset password token
            //    var token = await userManager.GeneratePasswordResetTokenAsync(user);
            //    // Build the password reset link
            //    var passwordResetLink = Url.Action("ResetPassword", "UserAccount", new { Area = "User", email = Email, token = token }, Request.Scheme);
            //    await _emailHelper.SendEmail(user.Email, passwordResetLink, "Reset Password", "", user.DisplayedUserName);
            //    return StatusCode(StatusCodes.Status200OK,
            //          new ResponseModel<object>(StatusCodes.Status200OK, true,
            //         _localizer["Mail Sent Please Check Your Mail inbox"], null));
            //}
            //else
            //{
            //    return StatusCode(StatusCodes.Status200OK,
            //          new ResponseModel<object>(StatusCodes.Status200OK, true,
            //         _localizer["Not Confirmed Or Not Exist Email"], null));
            //}
        }

        [HttpPost]
        [Route("ConfirmEmailinweb")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> ConfirmEmailinweb([FromForm] int code, [FromForm] string email)
        {
            try
            {
                var exist = this._userService.GetUserCodeByEmail(email);
                if (exist != null)
                {
                    // check code 
                    if (exist.Code != code)
                    {
                        return StatusCode(StatusCodes.Status200OK,
                      new ResponseModel<object>(StatusCodes.Status200OK, false,

                       _localizer["codeincorrect"], null));


                    }

                    // check expiration 
                    var time = exist.CreatedOn.AddDays(6);

                    if (time > DateTime.Now)
                    {
                        // valid 
                        var user = await userManager.FindByEmailAsync(exist.Email);
                        if (user != null)
                        {
                            var result = await userManager.ConfirmEmailAsync(user, exist.Token);
                            this._userService.DeleteUserCode(exist);
                            var userDeatils = this._userService.GetUserDetails(user.Id);
                            var image = userDeatils.UserImage;

                            user.EmailConfirmedOn = DateTime.Now;
                            await userManager.UpdateAsync(user);

                            var token = await GenerateAuthToken(user);
                            {
                                return StatusCode(StatusCodes.Status200OK,
                                    new ResponseModel<object>(StatusCodes.Status200OK, true,

                                     _localizer["mailconfirmed"], new
                                     {
                                         token = token,
                                         code = "",
                                         UserName = user.DisplayedUserName.Replace(" ", "-"),
                                         email = user.Email,
                                         phoneNumber = user.PhoneNumber,
                                         userImage = _configuration["BaseUrl"] + image
                                     }));
                            }
                        }
                        return StatusCode(StatusCodes.Status200OK,
                                   new ResponseModel<object>(StatusCodes.Status200OK, false,

                                    _localizer["userdoesnotexist"], null
                                   ));

                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status200OK,
                                   new ResponseModel<object>(StatusCodes.Status200OK, false,

                                    _localizer["codeinvalid"], null
                                   ));

                    }
                }
                return StatusCode(StatusCodes.Status200OK,
                                  new ResponseModel<object>(StatusCodes.Status200OK, false,

                                   _localizer["emailincorrect"], null
                                  ));
            }
            catch (Exception ex)
            {

                var log = new BWErrorLog
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = null,
                    Api = "Auth/ConfirmEmaile",
                    ApiParams = "code : " + code,
                    Exception = ex.ToString(),
                    ExMsg = ex.Message,
                    ExStackTrace = ex.StackTrace,
                    InnerException = ex.InnerException == null ? null : ex.InnerException.ToString(),
                    InnerExMsg = ex.InnerException == null ? null : ex.InnerException.Message,
                    InnerExStackTrace = ex.InnerException == null ? null : ex.InnerException.StackTrace,
                    CreatedOn = DateTime.Now
                };

                await this._errorLogService.InsertErrorLog(log);
                return StatusCode(StatusCodes.Status500InternalServerError,
                                 new ResponseModel<object>(StatusCodes.Status500InternalServerError, false,
                                 ex.Message, null));
            }
        }

        [HttpPost]
        [Route("ChangePasswordweb")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> ChangePasswordweb([FromForm] string newPassword, [FromForm] string Email)
        {

            try
            {

                // get user
                //  var user = await userManager.FindByIdAsync(loggedinUser.UserId);
                var user = userManager.Users.FirstOrDefault(u => u.Email == Email);

                if (user == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound,
                    new ResponseModel<object>(StatusCodes.Status404NotFound, false,

                     _localizer["userdoesnotexist"], null));
                }

                if (string.IsNullOrEmpty(newPassword))
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                        new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,

                         _localizer["emailincorrect"], null));

                // check if old pass is correct
                if (user != null)
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
                        var userDeatils = this._userService.GetUserDetails(user.Id);
                        userDeatils.pasword = newPassword;
                        this._userService.UpdateUserDetails(userDeatils);
                        return StatusCode(StatusCodes.Status200OK,
                         new ResponseModel<object>(StatusCodes.Status200OK, true,

                          _localizer["Password changed successfully"], null));

                    }

                    return StatusCode(StatusCodes.Status404NotFound,
                   new ResponseModel<object>(StatusCodes.Status404NotFound, false,

                    _localizer["emailincorrect"], null));

                }

                return StatusCode(StatusCodes.Status404NotFound,
                   new ResponseModel<object>(StatusCodes.Status404NotFound, false,

                    _localizer["userdoesnotexist"], null));

            }
            catch (Exception ex)
            {
                var log = new BWErrorLog
                {
                    Id = Guid.NewGuid().ToString(),
                    // UserId = User == null ? null : User.,
                    Api = "Account/ChangePass",
                    ApiParams = " , NewPass : " + newPassword,
                    Exception = ex.ToString(),
                    ExMsg = ex.Message,
                    ExStackTrace = ex.StackTrace,
                    InnerException = ex.InnerException == null ? null : ex.InnerException.ToString(),
                    InnerExMsg = ex.InnerException == null ? null : ex.InnerException.Message,
                    InnerExStackTrace = ex.InnerException == null ? null : ex.InnerException.StackTrace,
                    CreatedOn = DateTime.Now
                };

                await this._errorLogService.InsertErrorLog(log);

                return StatusCode(StatusCodes.Status500InternalServerError,
                                new ResponseModel<object>(StatusCodes.Status500InternalServerError, false,
                                ex.Message, null));
            }
        }

        private async Task<string> GenerateEmailConfirmationToken(User user, int code)
        {
            // generate token
            var newToken = await userManager.GenerateEmailConfirmationTokenAsync(user);

            if (!string.IsNullOrEmpty(user.Email))
            {
                var exist = this._userService.GetUserCodeByEmail(user.Email);
                if (exist != null)
                {
                    exist.Token = newToken;
                    exist.Code = code;
                    exist.CreatedOn = DateTime.Now;
                    this._userService.UpdateUserCode(exist);
                }
                else
                {
                    // save into db with that email
                    var userCode = new UserCodeCheck
                    {
                        Id = Guid.NewGuid().ToString(),
                        Email = user.Email,
                        Token = newToken,
                        Code = code,
                        CreatedOn = DateTime.Now
                    };
                    this._userService.InsertUserCode(userCode);
                }
            }
            else
            {
                // save into db with that email
                var userCode = new UserCodeCheck
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = user.Email,
                    Token = newToken,
                    Code = code,
                    CreatedOn = DateTime.Now
                };
                this._userService.InsertUserCode(userCode);
            }


            //TODO send it to that email with info (expire in 5 min)
            await SendEmailconfirm(user.Email, "Reset Password", code, user.DisplayedUserName);

            return newToken;
        }

        private async Task<string> GeneratePassResetToken(User user, int code)
        {
            // generate token
            var newToken = await userManager.GeneratePasswordResetTokenAsync(user);

            // generate code
            //Random random = new Random();
            //var code = random.Next(000000, 999999);

            // update user code in db

            var exist = this._userService.GetUserCodeByEmail(user.Email);

            if (exist != null)
            {
                exist.Token = newToken;
                exist.Code = code;
                exist.CreatedOn = DateTime.Now;
                this._userService.UpdateUserCode(exist);
            }
            else
            {
                // save into db with that email
                var userCode = new UserCodeCheck
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = user.Email,
                    Token = newToken,
                    Code = code,
                    CreatedOn = DateTime.Now
                };
                this._userService.InsertUserCode(userCode);
            }

            //TODO send it to that email with info (expire in 5 min)
            await SendEmailchangepassword(user.PasswordHash, user.Email, "Reset Password", code);

            return newToken;
        }
        private async Task ConfirmEmail(int code, string email)
        {

            try
            {
                var exist = this._userService.GetUserCodeByEmail(email);
                if (exist != null)
                {
                    // check code 
                    if (exist.Code == code)
                    {
                        // check expiration 
                        var time = exist.CreatedOn.AddDays(6);

                        if (time > DateTime.Now)
                        {
                            // valid 
                            var user = await userManager.FindByEmailAsync(exist.Email);
                            if (user != null)
                            {
                                var result = await userManager.ConfirmEmailAsync(user, exist.Token);
                                this._userService.DeleteUserCode(exist);
                                var userDeatils = this._userService.GetUserDetails(user.Id);
                                var image = userDeatils.UserImage;

                                user.EmailConfirmedOn = DateTime.Now;
                                user.EmailConfirmed = true;
                                await userManager.UpdateAsync(user);
                               // ViewBag.Message = "Email Confirmed";
                               // return View();

                            }
                           // ViewBag.Message = "User Not Exist";

                            //return View();



                        }
                        //else
                        //{
                        //    ViewBag.Message = "Expired Code";

                        //    return View();


                        //}
                    }
                    //ViewBag.Message = "Invalid Code";

                    //return View();



                }
                //else
                //{
                //    ViewBag.Message = "Not Exist";
                //    return View();

                //}
            }
            catch (Exception ex)
            {

                var log = new BWErrorLog
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = null,
                    Api = "Auth/ConfirmEmaile",
                    ApiParams = "code : " + code,
                    Exception = ex.ToString(),
                    ExMsg = ex.Message,
                    ExStackTrace = ex.StackTrace,
                    InnerException = ex.InnerException == null ? null : ex.InnerException.ToString(),
                    InnerExMsg = ex.InnerException == null ? null : ex.InnerException.Message,
                    InnerExStackTrace = ex.InnerException == null ? null : ex.InnerException.StackTrace,
                    CreatedOn = DateTime.Now
                };
               // ViewBag.Message = "Somthing Goes Wrong";
               // return View();

                //await this._errorLogService.InsertErrorLog(log);
                //return StatusCode(StatusCodes.Status500InternalServerError,
                //                 new ResponseModel<object>(StatusCodes.Status500InternalServerError, false,
                //                 ex.Message, null));
            }
        }
        private async Task SendEmailconfirm(string email, string subject, int code, string username)
        {
            var emailModel = new EmailModel(email, // To  
                subject, // Subject  
                "Your Confirmation Code Is : " + code +
                ", Note : this code is valid for 5 day !", // Message  
                false // IsBodyHTML  
            );
            // _emailHelper.SendEmail(emailModel);
            var redirectUrl =_configuration.GetValue<string>("DeepLinkOfConfirmedEmail") ;
            await this.ConfirmEmail(code, email);
            await _emailHelper.SendEmailregistration(email, subject, "Your Confirmation Code Is : " + code +
                  ", Note : this code is valid for 5 day !", code, /*(globalMethodsService.GetBaseDomain() +*/ redirectUrl/*)*/, username);
        }

        private async Task SendEmailchangepassword(string phone, string email, string subject, int code)
        {
            var emailModel = new EmailModel(email, // To  
                subject, // Subject  
                "Your Confirmation Code Is : " + code +
                ", Note : this code is valid for 5 day !", // Message  
                false // IsBodyHTML  
            );
            var user = userManager.Users.FirstOrDefault(x => x.Email.ToLower() == email.ToLower());
            // _emailHelper.SendEmail(emailModel);
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var passwordResetLink = Url.Action("ResetPassword", "UserAccount", new { Area = "User", email = email, token = token }, Request.Scheme);

            await _emailHelper.SendEmailchangepassword(phone, email, subject, "Your Confirmation Code Is : " + code +", Note : this code is valid for 5 day !", code, passwordResetLink , user.DisplayedUserName);
        }

        private async Task<string> GenerateAuthToken(User user)
        {
            var userRoles = await userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddMonths(2),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            var existLoggedinUser = await this._userService.GetLoggedInUser(user.Id, 3, 2);
            if (existLoggedinUser != null)
            {
                // update token
                existLoggedinUser.Token = new JwtSecurityTokenHandler().WriteToken(token);
                existLoggedinUser.CreatedOn = DateTime.Now;
                existLoggedinUser.ExpiredOn = token.ValidTo;

                this._userService.UpdateLoggedInUser(existLoggedinUser);
            }
            else
            {
                // save loggedin user
                var loggedinUser = new LoggedinUser
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    CreatedOn = DateTime.Now,
                    ExpiredOn = token.ValidTo,
                    UserId = user.Id,
                    ProjectId = 3, // Bait Waten ProjectId,
                    PlatformId = 2 // mobile
                };
                this._userService.InsertLoggedInUser(loggedinUser);
            }

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
