using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Social.Entity.DBContext;
using Social.Entity.Models;
using Social.Entity.ModelView;
using Social.Services.Helpers;
using Social.Services.ModelView;
using Social.Services.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Services.Implementation
{
    public class UserReportService : IUserReportService
    {
        private readonly AuthDBContext authDBContext;
        private readonly IStringLocalizer<SharedResource> localizer;
        private readonly IGlobalMethodsService globalMethodsService;
        private readonly IHttpContextAccessor httpContextAccessor;

        public UserReportService(AuthDBContext authDBContext, IStringLocalizer<SharedResource> _localizer,IGlobalMethodsService globalMethodsService, IHttpContextAccessor httpContextAccessor)
        {
            this.authDBContext = authDBContext;
            localizer = _localizer;
            this.globalMethodsService = globalMethodsService;
            this.httpContextAccessor = httpContextAccessor;
        }
        public async Task<CommonResponse<UserReportVM>> Create(UserReportVM VM)
        {
            try
            {
                var Obj = Converter(VM);
                await authDBContext.UserReports.AddAsync(Obj);
                await authDBContext.SaveChangesAsync();

                return CommonResponse<UserReportVM>.GetResult(200, true, localizer["SavedSuccessfully"]);
            }
            catch(Exception ex)
            {
                return CommonResponse<UserReportVM>.GetResult(403, false, "ex");
            }
        }


        public async Task<CommonResponse<UserReportVM>> Edit(UserReportVM VM)
        {

            var Obj = Converter(VM);

            try
            {
                authDBContext.Attach(Obj);
                authDBContext.Entry(Obj).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                //authDBContext.UserReports.Update(Obj);
                await authDBContext.SaveChangesAsync();
                return CommonResponse<UserReportVM>.GetResult(200, true, localizer["SavedSuccessfully"]);
            }
            catch
            {
                return CommonResponse<UserReportVM>.GetResult(403, false, "ex");
            }
        }
        public async Task<CommonResponse<UserReportVM>> Remove(Guid ID)
        {
            var Obj = await authDBContext.UserReports.FindAsync(ID);
            try
            {
                authDBContext.UserReports.Remove(Obj);
                await authDBContext.SaveChangesAsync();
                return CommonResponse<UserReportVM>.GetResult(200, true, localizer["RemovedSuccessfully"]);



            }
            catch (Exception ex)
            {
                var related = DependencyValidator<UserReport>.GetDependenciesNames(Obj).ToList();
                var Message = string.Format(localizer["PlzDeleteDataRelatedWithObjForCompleteObjectRemove"],
                   Obj?.Message,
                    //localizer[Obj.GetType().BaseType.Name),
                    related.Select(x => localizer[x].Value).Aggregate((x, y) => x + "," + y));
                return CommonResponse<UserReportVM>.GetResult(403, false, Message);

            }
        }


        public async Task<UserReportVM> GetData(Guid ID)
        {
            try
            {
                var Obj = await authDBContext.UserReports.FindAsync(ID);
                var vM = Converter(Obj);
                return vM;
            }
            catch
            {
                return new UserReportVM();
            }
        }

        public IEnumerable<UserReportVM> GetData()
        {
            var data = authDBContext.UserReports.Select(Converter).ToList();
            return data;
        }
        public IEnumerable<UserReportVM> GetAllReportsInUser(string UserID)
        {
            var data = authDBContext.UserReports.Where(x=>x.UserID==UserID).Select(Converter).ToList();
            return data;
        }

        UserReport Converter(UserReportVM model)
        {
            if (model == null) return null;

            var ss = Guid.TryParse(model.ID, out Guid ID);
            ID = ss ? ID : Guid.NewGuid();
            Guid.TryParse(model.ReportReasonID, out Guid ReportReasonID);

            var Obj = new UserReport()
            {
                Message = model.Message,
                RegistrationDate = model.RegistrationDate,
                ReportReasonID = ReportReasonID,
                UserID= model.UserID,
                CreatedBy_UserID = httpContextAccessor.HttpContext.GetUser().User.UserDetails.UserId,
                ID = ID,
            };

            return Obj;
        }
        UserReportVM Converter(UserReport model)
        {
            if (model == null) return null;

            var Obj = new UserReportVM
            {
                ID = model.ID.ToString(),
                Message = model.Message,
               CreatedBy_UserID = model.CreatedBy_UserID,
               ReportReasonID = model.ReportReasonID.ToString(),
               UserID = model.UserID,
               
                RegistrationDate = model.RegistrationDate,
                CreatedBy_UserName=model.CreatedBy_User.DisplayedUserName,
                ReportedUserImageUrl=model.ReportedUser.UserDetails.UserImage==null? "/assets/media/avatars/blank.png" : globalMethodsService.GetBaseDomain()+model.ReportedUser.UserDetails.UserImage,
                CreatedBy_ImageUrl=model.CreatedBy_User.UserDetails.UserImage==null? "/assets/media/avatars/blank.png" : globalMethodsService.GetBaseDomain()+model.ReportedUser.UserDetails.UserImage,
                ReportReasonName=model.ReportReason.Name,
                ReportedUserID=model.ReportedUser.Id,
                ReportedUserName=model.ReportedUser.DisplayedUserName,
                ReportedUserEmail = model.ReportedUser.UserDetails.Email,
                CreatedBy_Email = model.CreatedBy_User.UserDetails.Email

            };
            return Obj;
        }
    }

}



