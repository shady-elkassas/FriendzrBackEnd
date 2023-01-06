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
    public class EventReportService : IEventReportService
    {
        private readonly AuthDBContext authDBContext;
        private readonly IStringLocalizer<SharedResource> localizer;
        private readonly IGlobalMethodsService globalMethodsService;
        private readonly IHttpContextAccessor httpContextAccessor;

        public EventReportService(AuthDBContext authDBContext, IStringLocalizer<SharedResource> _localizer,IGlobalMethodsService globalMethodsService, IHttpContextAccessor httpContextAccessor)
        {
            this.authDBContext = authDBContext;
            localizer = _localizer;
            this.globalMethodsService = globalMethodsService;
            this.httpContextAccessor = httpContextAccessor;
        }
        public async Task<CommonResponse<EventReportVM>> Create(EventReportVM VM)
        {
            try
            {
                var Obj = Converter(VM);
                await authDBContext.EventReports.AddAsync(Obj);
                await authDBContext.SaveChangesAsync();

                return CommonResponse<EventReportVM>.GetResult(200, true, localizer["SavedSuccessfully"]);
            }
            catch
            {
                return CommonResponse<EventReportVM>.GetResult(406, false, "ex");
            }
        }


        public async Task<CommonResponse<EventReportVM>> Edit(EventReportVM VM)
        {

            var Obj = Converter(VM);

            try
            {
                authDBContext.Attach(Obj);
                authDBContext.Entry(Obj).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                //authDBContext.EventReports.Update(Obj);
                await authDBContext.SaveChangesAsync();
                return CommonResponse<EventReportVM>.GetResult(200, true, localizer["SavedSuccessfully"]);
            }
            catch
            {
                return CommonResponse<EventReportVM>.GetResult(406, false, "ex");
            }
        }
        public async Task<CommonResponse<EventReportVM>> Remove(Guid ID)
        {
            var Obj = await authDBContext.EventReports.FindAsync(ID);
            try
            {
                authDBContext.EventReports.Remove(Obj);
                await authDBContext.SaveChangesAsync();
                return CommonResponse<EventReportVM>.GetResult(200, true, localizer["RemovedSuccessfully"]);



            }
            catch (Exception ex)
            {
                var related = DependencyValidator<EventReport>.GetDependenciesNames(Obj).ToList();
                var Message = string.Format(localizer["PlzDeleteDataRelatedWithObjForCompleteObjectRemove"],
                   Obj?.Message,
                    //localizer[Obj.GetType().BaseType.Name),
                    related.Select(x => localizer[x].Value).Aggregate((x, y) => x + "," + y));
                return CommonResponse<EventReportVM>.GetResult(406, false, Message);

            }
        }


        public async Task<EventReportVM> GetData(Guid ID)
        {
            try
            {
                var Obj = await authDBContext.EventReports.FindAsync(ID);
                var vM = Converter(Obj);
                return vM;
            }
            catch
            {
                return new EventReportVM();
            }
        }

        public IEnumerable<EventReportVM> GetData()
        {
            var data = authDBContext.EventReports.Include(x=>x.User).Include(x=>x.User.UserDetails).Select(Converter).ToList();
            return data;
        }

        public IEnumerable<EventReportVM> GetData(List<int>eventIds)
        {
            var data = authDBContext.EventReports.Where(e=>eventIds.Contains(e.EventDataID)).Include(x => x.User).Include(x => x.User.UserDetails).Select(Converter).ToList();
            return data;
        }
        public IEnumerable<EventReportVM> GetData(string EventID)
        {
            var data = authDBContext.EventReports.Where(x=>x.EventData.EntityId==EventID).Include(x=>x.User).Include(x=>x.User.UserDetails).Select(Converter).ToList();
            return data;
        }


        EventReport Converter(EventReportVM model)
        {
            if (model == null) return null;

            var ss = Guid.TryParse(model.ID, out Guid ID);
            Guid.TryParse(model.ReportReasonID, out Guid ReportReasonID);
           
            var Obj = new EventReport()
            {
                Message = model.Message,
                RegistrationDate = model.RegistrationDate,
                EventDataID = model.EventDataID,
                ReportReasonID = ReportReasonID,
                CreatedBy_UserID = httpContextAccessor.HttpContext.GetUser().User.Id,
                ID = ID,
            };
            return Obj;
        }
        EventReportVM Converter(EventReport model)
        {
            if (model == null) return null;
            var Obj = new EventReportVM
            {
                ID = model.ID.ToString(),
                Message = model.Message,
                CreatedBy_UserID = model.CreatedBy_UserID,
                ReportReasonID = model.ReportReasonID.ToString(),
                EventDataEntityID = model.EventData.EntityId,
                EventDataID = model.EventDataID,
                RegistrationDate = model.RegistrationDate,
                CreatedBy_UserName=model.User.DisplayedUserName,
                EventTitle = model.EventData.Title,
                Eventdescription = model.EventData.description,
                ReportReasonName=model.ReportReason.Name,
                EventImageUrl = model.EventData.image==null?"/assets/media/avatars/events.png":globalMethodsService.GetBaseDomain()+model.EventData.image,
            };
            return Obj;
        }
    }

}



