using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using Social.Entity.Enums;
using Social.Entity.Models;
using Social.FireBase;
using Social.Models;
using Social.Services;
using Social.Services.Services;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Social.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public AccountController(UserManager<User> userManager, IHttpContextAccessor httpContextAccessor, IUserService userService, IConfiguration configuration, IStringLocalizer<SharedResource> localizer)
        {
            this.userManager = userManager;
            this.httpContextAccessor = httpContextAccessor;
            this._userService = userService;
            this._configuration = configuration;
            this._localizer = localizer;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
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

                    var user = userManager.Users.FirstOrDefault(u => u.Email == model.Email);

                    var isAdmin = (await userManager.IsInRoleAsync(user, StaticApplicationRoles.Admin.ToString()));

                    var isSuperAdmin = (await userManager.IsInRoleAsync(user, StaticApplicationRoles.SuperAdmin.ToString()));

                    var isWhiteLable = (await userManager.IsInRoleAsync(user, StaticApplicationRoles.WhiteLable.ToString()));

                    //if (isWhiteLable)
                    //{
                    //    return Redirect("/WhiteLable/Account/Login");
                    //}

                    if ((isAdmin == true || isSuperAdmin == true) && (await userManager.CheckPasswordAsync(user, model.Password)))
                    {
                        //checkconfirmation email if true login
                        //if false redirect to confrm mail
                        var token = await GenerateAuthToken(user);
                        var userDeatils = this._userService.GetUserDetails(user.Id);
                        var image = userDeatils.UserImage;
                        //userDeatils.FcmToken = model.FcmToken;
                        //this._userService.UpdateUserDetails(userDeatils);
                        SendNotificationcs sendNotificationcs = new SendNotificationcs();
                        //if(userDeatils.FcmToken != null)
                        //await sendNotificationcs.SendMessageAsync(userDeatils.FcmToken, "Welcome Message", fireBaseInfo, _environment.WebRootPath);
                        CookieOptions option = new CookieOptions();
                        if (Request.Cookies["Usre_Culture"] == null)
                        {
                            option.Expires = DateTime.Now.AddDays(15);
                            Response.Cookies.Append("Usre_Culture", userDeatils.language == "Fr" ? "en-US" : (userDeatils.language == "En" ? "en-US" : "ar-EG"), option);
                        }
                        Response.Cookies.Append("Authorization", $"Bearer {token}", option);
                        
                        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)) return LocalRedirect(returnUrl);
                        else return Redirect("/admin/home/index");


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
            httpContextAccessor.HttpContext.Response.Cookies.Delete("Authorization");
            return RedirectToAction("Login", "Account");
        }
    }
}
