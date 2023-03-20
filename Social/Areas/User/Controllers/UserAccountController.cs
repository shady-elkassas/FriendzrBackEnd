using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Social.Entity.Models;
using Social.Sercices.Helpers;
using Social.Services.ModelView;
using Social.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Threading.Tasks;

namespace Social.Areas.UserArea.Controllers
{
    [Area("User")]
    public class UserAccountController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly IUserService _userService;
        private readonly EmailHelper _emailHelper;

        public UserAccountController(UserManager<User> userManager,IUserService userService, EmailHelper emailHelper)
        {
            this.userManager = userManager;
            this._userService = userService;
            _emailHelper = emailHelper;
        }
        [HttpGet]
        [AllowAnonymous]

        //[Route("Uer/ResetPassword")]
        public async Task<IActionResult> ConfirmEmail(int code, string email)
        {
            await _emailHelper.SendWelcomeEmail(email);


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
                               ViewBag.Message = "Email Confirmed";
                              await _emailHelper.SendWelcomeEmail(email);
                            return Redirect("https://friendzr.onelink.me/59hw/bo9x5q4r");

                            }
                            ViewBag.Message = "User Not Exist";

                            return View();



                        }
                        else
                        {
                            ViewBag.Message = "Expired Code";

                            return View();


                        }
                    }
                    ViewBag.Message = "Invalid Code";

                    return View();



                }
                else
                {
                    ViewBag.Message = "Not Exist";
                    return View();

                }
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
                ViewBag.Message = "Somthing Goes Wrong";
                return View();

                //await this._errorLogService.InsertErrorLog(log);
                //return StatusCode(StatusCodes.Status500InternalServerError,
                //                 new ResponseModel<object>(StatusCodes.Status500InternalServerError, false,
                //                 ex.Message, null));
            }
        
        }
        public async Task<IActionResult> DELETEEmail(int code, string email)
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
                                var userDeatils = this._userService.DeleteUser_StoredProcedure(user.UserDetails);
                                
                                ViewBag.Message = "Account deleted";
                                return View();

                            }
                            ViewBag.Message = "User Not Exist";

                            return View();



                        }
                        else
                        {
                            ViewBag.Message = "Expired Code";

                            return View();


                        }
                    }
                    ViewBag.Message = "Invalid Code";

                    return View();



                }
                else
                {
                    ViewBag.Message = "Not Exist";
                    return View();

                }
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
                ViewBag.Message = "Somthing Goes Wrong";
                return View();

                //await this._errorLogService.InsertErrorLog(log);
                //return StatusCode(StatusCodes.Status500InternalServerError,
                //                 new ResponseModel<object>(StatusCodes.Status500InternalServerError, false,
                //                 ex.Message, null));
            }
        }

        [HttpGet]
        [AllowAnonymous]
        //[Route("Uer/ResetPassword")]
        public IActionResult ResetPassword(string token, string email)
        {
            // If password reset token or email is null, most likely the
            // user tried to tamper the password reset link
            if (token == null || email == null)
            {
                ModelState.AddModelError("", "Invalid password reset token");
            }
            return View(new ResetPasswordViewModel { Token=token,Email=email});
        }
        [AllowAnonymous]

        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]

        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Find the user by email
                //var user = await userManager.FindByEmailAsync(model.Email);
                var user =  userManager.Users.FirstOrDefault(x=>x.Email.ToLower().Trim()==model.Email.ToLower().Trim());

                if (user != null)
                {
                    // reset the user password
                    var result = await userManager.ResetPasswordAsync(user, model.Token, model.Password);
                    if (result.Succeeded)
                    {
                        return View("ResetPasswordConfirmation");
                    }
                    // Display validation errors. For example, password reset token already
                    // used to change the password or password complexity rules not met
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }

                // To avoid account enumeration and brute force attacks, don't
                // reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation");
            }
            // Display validation errors if model state is not valid
            return View(model);
        }

    }
}
