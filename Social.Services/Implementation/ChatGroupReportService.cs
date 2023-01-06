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
    public class ChatGroupReportService : IChatGroupReportService
    {
        private readonly AuthDBContext authDBContext;
        private readonly IStringLocalizer<SharedResource> localizer;
        private readonly IGlobalMethodsService globalMethodsService;
        private readonly IHttpContextAccessor httpContextAccessor;

        public ChatGroupReportService(AuthDBContext authDBContext, IStringLocalizer<SharedResource> _localizer, IGlobalMethodsService globalMethodsService, IHttpContextAccessor httpContextAccessor)
        {
            this.authDBContext = authDBContext;
            localizer = _localizer;
            this.globalMethodsService = globalMethodsService;
            this.httpContextAccessor = httpContextAccessor;
        }
        public async Task<CommonResponse<ChatGroupReportVM>> Create(ChatGroupReportVM VM)
        {
            try
            {
                var Obj = Converter(VM);
                await authDBContext.ChatGroupReports.AddAsync(Obj);
                await authDBContext.SaveChangesAsync();

                return CommonResponse<ChatGroupReportVM>.GetResult(200, true, localizer["SavedSuccessfully"]);
            }
            catch
            {
                return CommonResponse<ChatGroupReportVM>.GetResult(406, false, "ex");
            }
        }


        public async Task<CommonResponse<ChatGroupReportVM>> Edit(ChatGroupReportVM VM)
        {

            var Obj = Converter(VM);

            try
            {
                authDBContext.Attach(Obj);
                authDBContext.Entry(Obj).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                //authDBContext.ChatGroupReports.Update(Obj);
                await authDBContext.SaveChangesAsync();
                return CommonResponse<ChatGroupReportVM>.GetResult(200, true, localizer["SavedSuccessfully"]);
            }
            catch
            {
                return CommonResponse<ChatGroupReportVM>.GetResult(406, false, "ex");
            }
        }
        public async Task<CommonResponse<ChatGroupReportVM>> Remove(Guid ID)
        {
            var Obj = await authDBContext.ChatGroupReports.FindAsync(ID);
            try
            {
                authDBContext.ChatGroupReports.Remove(Obj);
                await authDBContext.SaveChangesAsync();
                return CommonResponse<ChatGroupReportVM>.GetResult(200, true, localizer["RemovedSuccessfully"]);



            }
            catch (Exception ex)
            {
                var related = DependencyValidator<ChatGroupReport>.GetDependenciesNames(Obj).ToList();
                var Message = string.Format(localizer["PlzDeleteDataRelatedWithObjForCompleteObjectRemove"],
                   Obj?.Message,
                    //localizer[Obj.GetType().BaseType.Name),
                    related.Select(x => localizer[x].Value).Aggregate((x, y) => x + "," + y));
                return CommonResponse<ChatGroupReportVM>.GetResult(406, false, Message);

            }
        }


        public async Task<ChatGroupReportVM> GetData(Guid ID)
        {
            try
            {
                var Obj = await authDBContext.ChatGroupReports.FindAsync(ID);
                var vM = Converter(Obj);
                return vM;
            }
            catch
            {
                return new ChatGroupReportVM();
            }
        }

        public IEnumerable<ChatGroupReportVM> GetData()
        {
            var data = authDBContext.ChatGroupReports.Select(Converter).ToList();
            return data;
        }


        ChatGroupReport Converter(ChatGroupReportVM model)
        {
            if (model == null) return null;

            var ss = Guid.TryParse(model.ID, out Guid ID);
            Guid.TryParse(model.ReportReasonID, out Guid ReportReasonID);

            var Obj = new ChatGroupReport()
            {
                Message = model.Message,
                RegistrationDate = model.RegistrationDate,
                ChatGroupID = model.ChatGroupID,
                ReportReasonID = ReportReasonID,
                CreatedBy_UserID = httpContextAccessor.HttpContext.GetUser().User.Id,
                ID = ID,
            };
            return Obj;
        }
        ChatGroupReportVM Converter(ChatGroupReport model)
        {
            if (model == null) return null;
            var Obj = new ChatGroupReportVM
            {
                ID = model.ID.ToString(),
                Message = model.Message,
                CreatedBy_UserID = model.CreatedBy_UserID,
                ReportReasonID = model.ReportReasonID.ToString(),
                ChatGroupID = model.ChatGroupID,
                RegistrationDate = model.RegistrationDate,
                CreatedBy_UserName = model.User.UserDetails.userName,
                ChatGroupName = model.ChatGroup.Name,
             
                ReportReasonName = model.ReportReason.Name,
                ChatGroupImageUrl = globalMethodsService.GetBaseDomain() + model.ChatGroup.Image,
            };
            return Obj;
        }
    }

}



