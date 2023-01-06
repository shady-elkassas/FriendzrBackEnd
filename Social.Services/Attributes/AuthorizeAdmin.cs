using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Primitives;
using Social.Entity.Enums;
using Social.Entity.Models;
using Social.Entity.ModelView;
using Social.Services.Helpers;
using Social.Services.Services;
using System;

namespace Social.Services.Attributes
{
    public class AuthorizeAdmin : IAuthorizationFilter
    {
        private readonly IUserService _userService;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly UserManager<User> userManager;

        public AuthorizeAdmin(IUserService userService, IStringLocalizer<SharedResource> localizer, UserManager<User> userManager)
        {
            _userService = userService;
            _localizer = localizer;
            this.userManager = userManager;
        }

        public bool AllowMultiple => throw new NotImplementedException();

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            context.HttpContext.Request.Headers.TryGetValue("Authorization", out StringValues authorizationToken);

            if (authorizationToken.Count == 0)
                authorizationToken = context.HttpContext.Request.Cookies["Authorization"];
            var loggedinUser =  _userService.GetLoggedInUser(authorizationToken).Result;
            if (loggedinUser == null)
            {

                RedirectRequest(context);

            }
           //else if (loggedinUser.User.UserDetails.birthdate == null&&userManager.IsInRoleAsync(loggedinUser.User, StaticApplicationRoles.Admin.ToString()).Result==false&&userManager.IsInRoleAsync(loggedinUser.User, StaticApplicationRoles.SuperAdmin.ToString()).Result==false)
           // {
           //     if (context.HttpContext.Request.IsAjaxRequest() || (context.HttpContext.Request?.Path.Value?.ToLower().Contains("api") ?? false))
           //     {
           //         var resObj = new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false, _localizer["Please you have to complete your profile."], null);
           //         var res = new ObjectResult(resObj);
           //         res.StatusCode = StatusCodes.Status406NotAcceptable;
           //         context.Result = res;
           //     }

           // }
            else
                context.HttpContext.Items["User"] = loggedinUser;
        }
        void RedirectRequest(AuthorizationFilterContext context)
        {

            PathString path = context.HttpContext.Request.Path;

            string[] splitedUrl = path.ToString().Substring(1).Split('/');


            if (context.HttpContext.Request.IsAjaxRequest() || (context.HttpContext.Request?.Path.Value?.ToLower().Contains("api") ?? false))
            {
                var resObj = new ResponseModel<object>(StatusCodes.Status401Unauthorized, false, _localizer["401eror"], null);
                var res = new ObjectResult(resObj);
                res.StatusCode = StatusCodes.Status401Unauthorized;
                context.Result = res;
            }
            else
            {
                context.Result = new RedirectToRouteResult("AdminArea", new RouteValueDictionary
                {
                   { "controller","Account"},
                   { "action","Login"},
                  
                   {"returnUrl",context.HttpContext.Request.Path.ToString()},

                });
            }
        }

    }
}
