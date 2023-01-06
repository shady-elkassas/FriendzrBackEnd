using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Social.Entity.DBContext;
using Social.Entity.Enums;
using Social.Entity.Models;
using Social.Entity.ModelView;
using Social.FireBase;
using Social.Models;
using Social.Sercices.Helpers;
using Social.Services;
using Social.Services.Attributes;
using Social.Services.Helpers;
using Social.Services.ModelView;
using Social.Services.Services;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Social.Areas.WhiteLable.Controllers
{
    [Area("Whitelabel")]
    public class AccountController : Controller
    {
        private readonly EmailHelper _emailHelper;
        private readonly AuthDBContext _dBContext;
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IGlobalMethodsService _globalMethodsService;
        private readonly IStringLocalizer<SharedResource> _localizer;
        public AccountController(UserManager<User> userManager, IHttpContextAccessor httpContextAccessor, IUserService userService, IConfiguration configuration, IStringLocalizer<SharedResource> localizer, EmailHelper emailHelper, IGlobalMethodsService globalMethodsService, AuthDBContext dBContext)
        {
            _localizer = localizer;
            _dBContext = dBContext;
            _userManager = userManager;
            _userService = userService;
            _emailHelper = emailHelper;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _globalMethodsService = globalMethodsService;
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult ForgetPassword()
        {
            return View();
        }

        [ServiceFilter(typeof(AuthorizeWhiteLable))]
        public IActionResult UserProfile()
        {
            return View();
        }

        [HttpPost]
        [ServiceFilter(typeof(AuthorizeWhiteLable))]
        public async Task<IActionResult> UpdateProfileImage(UserEditProfileViewModel userEditProfile)
        {
            var result = new CommonResponse<bool>();

            _httpContextAccessor.HttpContext.Request.Headers.TryGetValue("Authorization", out StringValues authorizationToken);

            if (authorizationToken.Count == 0)
            {
                authorizationToken = _httpContextAccessor.HttpContext.Request.Cookies["Authorization"];
            }
            var loggedinUser = _userService.GetLoggedInUser(authorizationToken).Result;

            if (loggedinUser != null && userEditProfile.ProfilImage != null)
            {
               UserDetails userDetail =  await _dBContext.UserDetails.Where(q => q.UserId == loggedinUser.UserId).FirstOrDefaultAsync();

                string oldUserImage = userDetail.UserImage;

                var imageName = await _globalMethodsService.uploadFileAsync("/Images/Userprofile/", userEditProfile.ProfilImage);

                userDetail.UserImage = "/Images/Userprofile/" + imageName;

               int savedReslt = await _dBContext.SaveChangesAsync();

               if (savedReslt > 0)
               {
                    result.Status = true;
                    result.Message = "Image Updated";
                }

               if (oldUserImage != null)
               {
                   _globalMethodsService.DeleteFiles(null, oldUserImage);
               }
            }
            return Ok(JObject.FromObject(result, new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));
        }

        [HttpPost]
        public async Task<IActionResult> SendEmailForgetPassword(LoginModel2 model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user == null)
                {
                    ModelState.AddModelError("", _localizer["checkyourE-mailorpassword!"]);
                }

                // generate code
                Random random = new Random();
                var code = random.Next(1, 1000000).ToString("D6");

                //generate pass reset token 
                var token = await GeneratePassResetToken(user, Convert.ToInt32(code.ToString()));

                return Redirect("/Whitelabel/Account/Login");

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", _localizer["Check your Email !"]);
                return View(model);
            }
        }

        private async Task<string> GenerateAuthToken(User user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var authSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(1000),
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

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel2 model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                Random random1 = new Random();
                var code1 = random1.Next(000000, 9) + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Second;
                try
                {

                    User user = _userManager.Users.FirstOrDefault(u => u.Email == model.Email);

                    bool isAdmin = (await _userManager.IsInRoleAsync(user, StaticApplicationRoles.Admin.ToString()));

                    bool isSuperAdmin = (await _userManager.IsInRoleAsync(user, StaticApplicationRoles.SuperAdmin.ToString()));

                    bool isWhiteLable = (await _userManager.IsInRoleAsync(user, StaticApplicationRoles.WhiteLable.ToString()));

                    //if (isAdmin || isSuperAdmin)
                    //{
                    //    return Redirect("/Admin/Account/Login");
                    //}

                    if (isWhiteLable == true && (await _userManager.CheckPasswordAsync(user, model.Password)))
                    {
                        //checkconfirmation email if true login
                        //if false redirect to confrm mail
                        var token = await GenerateAuthToken(user);
                        var userDeatils = this._userService.GetUserDetails(user.Id);
                        var image = userDeatils.UserImage;

                        SendNotificationcs sendNotificationcs = new SendNotificationcs();

                        CookieOptions option = new CookieOptions();
                        if (Request.Cookies["Usre_Culture"] == null)
                        {
                            option.Expires = DateTime.Now.AddDays(15);
                            Response.Cookies.Append("Usre_Culture", userDeatils.language == "Fr" ? "en-US" : (userDeatils.language == "En" ? "en-US" : "ar-EG"), option);
                        }                        
                        Response.Cookies.Append("Authorization", $"Bearer {token}", option);
                        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)) return LocalRedirect(returnUrl);
                        else return Redirect("/Whitelabel/home/Index");


                    }
                    ModelState.AddModelError("", _localizer["checkyourE-mailorpassword!"]);
                    return View(model);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", _localizer["checkyourE-mailorpassword!"]);
                    return View(model);
                }
            }
            return View(model);

        }
        [HttpPost]
        public IActionResult LogOut()
        {
            _httpContextAccessor.HttpContext.Response.Cookies.Delete("Authorization");
            return RedirectToAction("Login", "Account");
        }

        private async Task<string> GeneratePassResetToken(User user, int code)
        {
            // generate token
            var newToken = await _userManager.GeneratePasswordResetTokenAsync(user);

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

        private async Task SendEmailchangepassword(string phone, string email, string subject, int code)
        {
            var emailModel = new EmailModel(email, // To  
                subject, // Subject  
                "Your Confirmation Code Is : " + code +
                ", Note : this code is valid for 5 day !", // Message  
                false // IsBodyHTML  
            );
            var user = _userManager.Users.FirstOrDefault(x => x.Email.ToLower() == email.ToLower());
            // _emailHelper.SendEmail(emailModel);
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var passwordResetLink = Url.Action("ResetPassword", "UserAccount", new { Area = "User", email = email, token = token }, Request.Scheme);

            await _emailHelper.SendEmailchangepassword(phone, email, subject, "Your Confirmation Code Is : " + code + ", Note : this code is valid for 5 day !", code, passwordResetLink, user.DisplayedUserName);
        }

    }
}
