using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Social.Entity.DBContext;
using Social.Entity.Enums;
using Social.Entity.Models;
using Social.Entity.ModelView;
using Social.Services.ModelView;
using Social.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace Social.Services.Implementation
{
    public class WhiteLableUserService : IWhiteLableUserService
    {

        public AuthDBContext _dBContext;
        private readonly UserManager<User> _userManager;
        private readonly IAppConfigrationService _appConfigrationService;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly IGlobalMethodsService _globalMethodsService;
        public WhiteLableUserService(AuthDBContext dBContext, IAppConfigrationService appConfigrationService, UserManager<User> userManager, IGlobalMethodsService globalMethodsService , IStringLocalizer<SharedResource> stringLocalizer)
        {
            _dBContext = dBContext;
            _userManager = userManager;
            _localizer = stringLocalizer;
            _appConfigrationService = appConfigrationService;
            _globalMethodsService = globalMethodsService;
        }


        public async Task<CommonResponse<User>> CreatetWhiteLableUser(AddEditWhiteLableUserViewModel whiteLableUser)
        {
            try
            {                
                if(await this.IsEmailExist(whiteLableUser.Email,null))
                {
                    return CommonResponse<User>.GetResult(406, false, "Email Already Exist");
                }
                if(!await IsCodeLengthCorrect(whiteLableUser.Code))
                    return CommonResponse<User>.GetResult(406,false , $"Code Length Not Correct");
                User newWhiteLableUser = new User()
                {
                    Email = whiteLableUser.Email,
                    DisplayedUserName = whiteLableUser.DisplayedUserName,
                    UserName = whiteLableUser.UserName,
                    RegistrationDate = DateTime.UtcNow,
                    EmailConfirmed=true,
                    EmailConfirmedOn= DateTime.UtcNow,
                };

                newWhiteLableUser.UserDetails = new UserDetails
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = newWhiteLableUser.Id,
                    Email = whiteLableUser.Email,
                    userName = newWhiteLableUser.UserName,
                    pasword = whiteLableUser.Password,
                    platform = 1,
                    allowmylocation = true,
                    pushnotification = true,
                    Filteringaccordingtoage = true,
                    IsActive = true,
                    agefrom = 14,
                    ageto = 85,
                    IsWhiteLabel = true,
                    Code= CheckCode(whiteLableUser.Code)
                };

                if (whiteLableUser.Image != null)
                {
                    var UniqName = await _globalMethodsService.uploadFileAsync("/Images/Userprofile/", whiteLableUser.Image);

                    newWhiteLableUser.UserDetails.UserImage = $"/Images/Userprofile/{UniqName}";
                }

                IdentityResult userResult = await _userManager.CreateAsync(newWhiteLableUser, whiteLableUser.Password);
                IdentityResult addRoleResult = null;
                if (userResult.Succeeded)
                {
                    addRoleResult = await _userManager.AddToRoleAsync(newWhiteLableUser, StaticApplicationRoles.WhiteLable.ToString());
                }

                if (addRoleResult.Succeeded && userResult.Succeeded)
                {

                    return CommonResponse<User>.GetResult(200, true, _localizer["SavedSuccessfully"], newWhiteLableUser);
                }

                return CommonResponse<User>.GetResult(400, true, _localizer["NotSaved"], newWhiteLableUser);



            }
            catch (Exception ex)
            {
                return CommonResponse<User>.GetResult(406, false,ex.Message);
            }

        }

      

        public async Task<IQueryable<WhiteLableUserViewModel>> GetWhiteLableUsers()
        {
            try
            {
                var users =  _dBContext.ApplicationUsers.Include(q => q.UserDetails).Include(q => q.UserRoles).ThenInclude(q => q.Role).Where(q => q.UserRoles.Any(q => q.Role.Name == StaticApplicationRoles.WhiteLable.ToString()));

                var whiteLableUsers = users.Select(q => new WhiteLableUserViewModel()
                {
                    ID = q.Id,
                    UserName = q.UserName,
                    DisplayedUserName = q.DisplayedUserName,
                    Email = q.Email,
                    RegistrationDate = q.RegistrationDate.ToString("HH:mm dd/MM/yyyy"),
                    Password = q.UserDetails.pasword,
                    Image = q.UserDetails.UserImage == null ? "/assets/media/avatars/blank.png" : _globalMethodsService.GetBaseDomain() + q.UserDetails.UserImage,
                    IsActive = q.UserDetails.IsActive,
                    Code = q.UserDetails.Code,

                });

                return whiteLableUsers;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<AddEditWhiteLableUserViewModel> ViewWhiteLableUserDetails(string whiteLableUserId)
        {
            try
            {
                User user = await _dBContext.ApplicationUsers.Include(q => q.UserDetails).FirstOrDefaultAsync(q => q.Id == whiteLableUserId);

                AddEditWhiteLableUserViewModel whiteLableUser = new AddEditWhiteLableUserViewModel()
                {
                    ID = user.Id,
                    UserName = user.UserName,
                    DisplayedUserName = user.DisplayedUserName,
                    Email = user.Email,
                    //RegistrationDate = user.RegistrationDate.ToString("HH:mm yyyy-MM-dd"),
                    Password = user.UserDetails.pasword,
                    //Image = user.UserDetails.UserImage == null ? "/assets/media/avatars/blank.png" : _globalMethodsService.GetBaseDomain() + user.UserDetails.UserImage,
                    //IsActive = user.UserDetails.IsActive
                    Code= user.UserDetails.Code,
                };

                return whiteLableUser;
            }
            catch (Exception ex)
            {
                var exp = ex;

                throw ex;
            }
        }


        public async Task<CommonResponse<bool>> EditWhiteLableUser(AddEditWhiteLableUserViewModel editWhiteLableUser)
        {
            try
            {
                if (!await IsCodeLengthCorrect(editWhiteLableUser.Code))
                    return CommonResponse<bool>.GetResult(406, false, $"Code Length Not Correct");
                User user = await _dBContext.Users.Include(q => q.UserDetails).FirstOrDefaultAsync(q => q.Id == editWhiteLableUser.ID);

                if (user != null)
                {
                    user.Email = editWhiteLableUser.Email;
                    user.UserName = editWhiteLableUser.UserName;
                    user.DisplayedUserName = editWhiteLableUser.DisplayedUserName;
                    user.Email = editWhiteLableUser.Email;
                    user.UserDetails.Code= CheckCode(editWhiteLableUser.Code, user.UserDetails.PrimaryId);
                    if (editWhiteLableUser.Image != null)
                    {
                        var UniqName = await _globalMethodsService.uploadFileAsync("/Images/Userprofile/", editWhiteLableUser.Image);

                        user.UserDetails.UserImage = $"/Images/Userprofile/{UniqName}";
                    }

                    IdentityResult result = await _userManager.UpdateAsync(user);

                    if (result.Succeeded)
                    {
                        return CommonResponse<bool>.GetResult(200, true, _localizer["SavedSuccessfully"], true);
                    }

                }

                return CommonResponse<bool>.GetResult(400, true, _localizer["NotSaved"], false);

            }
            catch (Exception ex)
            {
                return CommonResponse<bool>.GetResult(406, false, ex.Message);
            }
        }


        public async Task<CommonResponse<bool>> DeleteWhiteLableUser(string whiteLableUserId)
        {
            try
            {
                using(TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled)) {
                    User user = await _dBContext.Users.FirstOrDefaultAsync(q => q.Id == whiteLableUserId);

                    UserDetails userDetail = await _dBContext.UserDetails.FirstOrDefaultAsync(q => q.UserId == whiteLableUserId);

                    if (user != null)
                    {
                        _dBContext.Remove(user);
                        await _userManager.RemoveFromRoleAsync(user, StaticApplicationRoles.WhiteLable.ToString());
                    }

                    if (userDetail != null)
                    {
                        _dBContext.Remove(userDetail);
                    }

                    int result = await _dBContext.SaveChangesAsync();

                    scope.Complete();

                    if (result > 0)
                    {
                        return CommonResponse<bool>.GetResult(200, true, _localizer["SavedSuccessfully"], true);
                    }

                    return CommonResponse<bool>.GetResult(400, true, _localizer["NotSaved"], true);
                }
                   
            }
            catch (Exception ex)
            {
                var exp = ex;
                return CommonResponse<bool>.GetResult(406, false, ex.Message);
            }
        }


        public async Task<CommonResponse<bool>> ToggleSuspendWhiteLableUser(string whiteLableUserId)
        {
            try
            {
                UserDetails userDetail = await _dBContext.UserDetails.FirstOrDefaultAsync(q => q.UserId == whiteLableUserId);

                if (userDetail != null)
                {
                    userDetail.IsActive = !userDetail.IsActive;
                }

                int result = await _dBContext.SaveChangesAsync();

                if (result > 0)
                {
                    return CommonResponse<bool>.GetResult(200, true, _localizer["SavedSuccessfully"], true);
                }

                return CommonResponse<bool>.GetResult(400, true, _localizer["NotSaved"], true);
            }
            catch (Exception ex)
            {
                return CommonResponse<bool>.GetResult(406, false, ex.Message);
            }
        }

           private async Task<bool> IsEmailExist(string email, string? userId)
           {
              var isExist = await _dBContext.Users.AnyAsync(u => u.Email == email &&(userId == null || u.Id != userId));
             return isExist;
           }

        private async Task<bool> IsCodeLengthCorrect(string code)
        {
            var codeLength = 5;            
            var appconfig = _appConfigrationService.GetData().FirstOrDefault();
            codeLength = (appconfig == null || appconfig.WhiteLableCodeLength == 0) ? codeLength : appconfig.WhiteLableCodeLength.Value;
            if (code.Length != codeLength)
                return false;
            return true;
        }
        private string CheckCode(string code, int uerId = 0)
        {
            var WhitelableCodes = (_dBContext.UserDetails.Where(u => u.IsWhiteLabel.Value && u.PrimaryId!= uerId)).Select(u => u.Code).ToList();
            if(WhitelableCodes.Contains(code))
            {
                throw new Exception("Unversity Code Already Exist");
            }
            return code;
        }
    }
}
