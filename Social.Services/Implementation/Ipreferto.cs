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

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Services.Implementation
{
   public class Ipreferto : IIpreferto
    {
        private readonly AuthDBContext authDBContext;
        private readonly IStringLocalizer<SharedResource> localizer;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IUserService userService;

        public Ipreferto(AuthDBContext authDBContext, IStringLocalizer<SharedResource> _localizer, IHttpContextAccessor httpContextAccessor, IUserService userService)
        {
            this.authDBContext = authDBContext;
            localizer = _localizer;
            this.httpContextAccessor = httpContextAccessor;
            this.userService = userService;
        }
        public async Task<CommonResponse<prefertoVM>> Create(prefertoVM VM)
        {
            try
            {
                VM.IsActive = true;
                var Obj = Converter(VM);
                await authDBContext.preferto.AddAsync(Obj);
                await authDBContext.SaveChangesAsync();
                var preferto = new Iprefertolist()
                {
                    prefertoId = Obj.Id,
                    Tagsname = Obj.name,
                    UserId = httpContextAccessor.HttpContext.GetUser().User.UserDetails.PrimaryId
                };
               
                return CommonResponse<prefertoVM>.GetResult(200, true, localizer["SavedSuccessfully"], VM);
            }
            catch
            {
                return CommonResponse<prefertoVM>.GetResult(406, false, "ex");
            }
        }


        public async Task<CommonResponse<prefertoVM>> Edit(prefertoVM VM)
        {


         
            VM.IsActive = true;
            var Obj = Converter(VM);

            try
            {

                authDBContext.Attach(Obj);
                authDBContext.Entry(Obj).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                //authDBContext.Interestss.Update(Obj);
                authDBContext.SaveChanges();
                return CommonResponse<prefertoVM>.GetResult(200, true, localizer["SavedSuccessfully"], VM);

            }
            catch (Exception ex)
            {
                return CommonResponse<prefertoVM>.GetResult(406, false, "ex");
            }
        }
        public async Task<CommonResponse<prefertoVM>> Remove(string ID)
        {
            var Obj = authDBContext.preferto.FirstOrDefault(x => x.Id ==Convert.ToInt32( ID));
            try

            {
                authDBContext.Iprefertolist.RemoveRange(Obj.Iprefertolist);
                authDBContext.preferto.Remove(Obj);
                await authDBContext.SaveChangesAsync();
                return CommonResponse<prefertoVM>.GetResult(200, true, localizer["RemovedSuccessfully"]);



            }
            catch
            {
                var related = DependencyValidator<preferto>.GetDependenciesNames(Obj).ToList();
                var Message = string.Format(localizer["PlzDeleteDataRelatedWithObjForCompleteObjectRemove"],
                   Obj?.name,
                    //localizer[Obj.GetType().BaseType.Name),
                    related.Select(x => localizer[x].Value).Aggregate((x, y) => x + "," + y));
                return CommonResponse<prefertoVM>.GetResult(406, false, Message);

            }
        }

        public async Task<CommonResponse<prefertoVM>> ToggleActiveConfigration(string ID, bool IsActive)
        {
            try
            {

                var preferto = authDBContext.preferto.FirstOrDefault(x => x.EntityId == ID);
                preferto.IsActive = IsActive;
                await authDBContext.SaveChangesAsync();
                return CommonResponse<prefertoVM>.GetResult(200, true, localizer["UpdatedSuccessfully"]);
            }
            catch
            {
                return CommonResponse<prefertoVM>.GetResult(406, false, "SomthingGoesWrong");

            }

        }
        public prefertoVM GetData(string ID)
        {
            try
            {
                var Obj = authDBContext.preferto.AsNoTracking().FirstOrDefault(x => x.Id == Convert.ToInt32(ID));
                var vM = Converter(Obj);
                return vM;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public IEnumerable<prefertoVM> GetData()
        {
            var data = authDBContext.preferto.AsNoTracking().Select(Converter).ToList();
            return data;
        }


        preferto Converter(prefertoVM model)
        {
            if (model == null) return null;
            var Obj = new preferto()
            {
                EntityId = Guid.NewGuid().ToString(),
            Id = model.ID,
                RegistrationDate = model.RegistrationDate,
                IsActive = model.IsActive,
                
                CreatedByUserID = httpContextAccessor.HttpContext.GetUser().UserId,
                name = model.name,
            };
            return Obj;
        }
        prefertoVM Converter(preferto model)
        {
            if (model == null) return null;

            var Obj = new prefertoVM
            {
                EntityId = model.EntityId,
                ID = model.Id,
                RegistrationDate = model.RegistrationDate,
                IsActive = model.IsActive,
                
                CreatedByUserID = model.CreatedByUserID,
                name = model.name,
                //Id = model.Id

            };
            return Obj;
        }
    }
}
