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
    public class ReportReasonService : IReportReasonService
    {
        private readonly AuthDBContext authDBContext;
        private readonly IStringLocalizer<SharedResource> localizer;
        private readonly IHttpContextAccessor httpContextAccessor;

        public ReportReasonService(AuthDBContext authDBContext, IStringLocalizer<SharedResource> _localizer, IHttpContextAccessor httpContextAccessor)
        {
            this.authDBContext = authDBContext;
            localizer = _localizer;
            this.httpContextAccessor = httpContextAccessor;
        }
        public async Task<CommonResponse<ReportReasonVM>> Create(ReportReasonVM VM)
        {
            try
            {
                var Obj = Converter(VM);

                if (authDBContext.ReportReasons.Any(x => x.DisplayOrder == VM.DisplayOrder))//If Order duplicated we shift
                {
                 authDBContext.ReportReasons.Where(x => x.DisplayOrder >= VM.DisplayOrder).ToList()
                    .ForEach(x =>
                    {
                        x.DisplayOrder = x.DisplayOrder== null ? null : x.DisplayOrder + 1;
                    });
                }
                await authDBContext.ReportReasons.AddAsync(Obj);
                await authDBContext.SaveChangesAsync();

                return CommonResponse<ReportReasonVM>.GetResult(200, true, localizer["SavedSuccessfully"]);
            }
            catch
            {
                return CommonResponse<ReportReasonVM>.GetResult(406, false, "ex");
            }
        }


        public async Task<CommonResponse<ReportReasonVM>> Edit(ReportReasonVM VM)
        {

            var Obj = Converter(VM);

            try
            {
                if (authDBContext.ReportReasons.Any(x => x.ID!=Obj.ID&&x.DisplayOrder == VM.DisplayOrder))//If Order duplicated we shift
                {
                    authDBContext.ReportReasons.Where(x =>x.ID!=Obj.ID &&x.DisplayOrder >= VM.DisplayOrder).ToList()
                       .ForEach(x =>
                       {
                           x.DisplayOrder = x.DisplayOrder == null ? null : x.DisplayOrder + 1;
                       });
                }
                authDBContext.Attach(Obj);
                authDBContext.Entry(Obj).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                //authDBContext.ReportReasons.Update(Obj);
                await authDBContext.SaveChangesAsync();
                return CommonResponse<ReportReasonVM>.GetResult(200, true, localizer["SavedSuccessfully"]);
            }
            catch
            {
                return CommonResponse<ReportReasonVM>.GetResult(406, false, "ex");
            }
        }
        public async Task<CommonResponse<ReportReasonVM>> Remove(Guid ID)
        {
            var Obj = await authDBContext.ReportReasons.FindAsync(ID);
            try
            {
                authDBContext.ReportReasons.Remove(Obj);
                await authDBContext.SaveChangesAsync();
                return CommonResponse<ReportReasonVM>.GetResult(200, true, localizer["RemovedSuccessfully"]);



            }
            catch (Exception ex)
            {
                var related = DependencyValidator<ReportReason>.GetDependenciesNames(Obj).ToList();
                var Message = string.Format(localizer["PlzDeleteDataRelatedWithObjForCompleteObjectRemove"],
                   Obj?.Name,
                    //localizer[Obj.GetType().BaseType.Name),
                    related.Select(x => localizer[x].Value).Aggregate((x, y) => x + "," + y));
                return CommonResponse<ReportReasonVM>.GetResult(406, false, Message);

            }
        }
        public async Task<ReportReasonVM> GetData(Guid ID)
        {
            try
            {
                var Obj = await authDBContext.ReportReasons.FindAsync(ID);
                var vM = Converter(Obj);
                return vM;
            }
            catch
            {
                return new ReportReasonVM();
            }
        }
        public IEnumerable<ReportReasonVM> GetData()
        {
            var data = authDBContext.ReportReasons.Select(Converter).OrderBy(x => x.DisplayOrder == null ? (int.MaxValue - 1) : x.DisplayOrder).ThenBy(x => x.RegistrationDate).ToList();
            return data;
        }
        public async Task<CommonResponse<ReportReasonVM>> ToggleActiveConfigration(Guid ID, bool IsActive)
        {
            try
            {

                var reportReason = await authDBContext.ReportReasons.FindAsync(ID);
                reportReason.IsActive = IsActive;
                await authDBContext.SaveChangesAsync();
                return CommonResponse<ReportReasonVM>.GetResult(200, true, localizer["UpdatedSuccessfully"]);
            }
            catch
            {
                return CommonResponse<ReportReasonVM>.GetResult(406, false, "SomthingGoesWrong");

            }

        }
        public IEnumerable<ValidationResult> _ValidationResult(ReportReasonVM VM)
        {
            var ss = Guid.TryParse(VM.ID, out Guid ID);
            ID = ss ? ID : Guid.NewGuid();
            if (authDBContext.ReportReasons.Any(x => x.ID != ID && x.Name.ToLower() == VM.Name.ToLower()))
            {
                var Message = string.Format(localizer["AlreadyExist"], VM.Name);
                yield return new ValidationResult(Message, new[] { nameof(VM.Name) });
            }
            //else if(authDBContext.ReportReasons.Any(x => x.ID != ID && x.DisplayOrder == VM.DisplayOrder))
            //{
            //    var Message = string.Format(localizer["AlreadyExist"], VM.DisplayOrder);
            //    yield return new ValidationResult(Message, new[] { nameof(VM.DisplayOrder) });
            //}

        }
        ReportReason Converter(ReportReasonVM model)
        {
            if (model == null) return null;

            var ss = Guid.TryParse(model.ID, out Guid ID);
            ID = ss ? ID : Guid.NewGuid();
            var Obj = new ReportReason()
            {
                Name = model.Name,
                IsActive = model.IsActive,
                DisplayOrder = model.DisplayOrder,
                RegistrationDate = model.RegistrationDate,
                ID = ID,

            };
            return Obj;
        }
        ReportReasonVM Converter(ReportReason model)
        {
            if (model == null) return null;
            var Obj = new ReportReasonVM
            {
                ID = model.ID.ToString(),
                IsActive = model.IsActive,
                Name = model.Name,
                DisplayOrder = model.DisplayOrder,
                RegistrationDate = model.RegistrationDate

            };
            return Obj;
        }
    }

}



