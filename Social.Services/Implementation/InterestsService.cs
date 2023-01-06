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
    public class InterestsService : IInterestsService
    {
        private readonly AuthDBContext authDBContext;
        private readonly IStringLocalizer<SharedResource> localizer;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IUserService userService;

        public InterestsService(AuthDBContext authDBContext,
            IStringLocalizer<SharedResource> _localizer, IHttpContextAccessor httpContextAccessor, IUserService userService)
        {
            this.authDBContext = authDBContext;
            localizer = _localizer;
            this.httpContextAccessor = httpContextAccessor;
            this.userService = userService;
        }
        public async Task<CommonResponse<InterestsVM>> Create(InterestsVM VM)
        {
            try
            {
                VM.EntityId = Guid.NewGuid().ToString();
                VM.IsSharedForAllUsers = true;
                VM.IsActive = true;
                var Obj = Converter(VM);
                await authDBContext.Interests.AddAsync(Obj);
                await authDBContext.SaveChangesAsync();
                var listoftags = new listoftags()
                {
                    InterestsId = Obj.Id,
                    Tagsname = Obj.name,
                    UserId = httpContextAccessor.HttpContext.GetUser().User.UserDetails.PrimaryId
                };
                if(VM.IsSharedForAllUsers==false)
                userService.addlistoftags(listoftags);

                //VM.Id = Obj.EntityId;
                return CommonResponse<InterestsVM>.GetResult(200, true, localizer["SavedSuccessfully"], VM);
            }
            catch 
            {
                return CommonResponse<InterestsVM>.GetResult(406, false, "ex");
            }
        }


        public async Task<CommonResponse<InterestsVM>> Edit(InterestsVM VM)
        {
            VM.IsSharedForAllUsers = true;
            VM.IsActive = true;
            var Obj = Converter(VM);

            try
            {
               
                authDBContext.Attach(Obj);
                authDBContext.Entry(Obj).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                //authDBContext.Interestss.Update(Obj);
                 authDBContext.SaveChanges();
                return CommonResponse<InterestsVM>.GetResult(200, true, localizer["SavedSuccessfully"], VM);

            }
            catch (Exception ex)
            {
                return CommonResponse<InterestsVM>.GetResult(406, false, "ex");
            }
        }
        public async Task<CommonResponse<InterestsVM>> Remove(string ID)
        {
            var Obj =  authDBContext.Interests.FirstOrDefault(x=>x.EntityId==ID);
            try

            {
                authDBContext.listoftags.RemoveRange(Obj.listoftags);
                authDBContext.Interests.Remove(Obj);
                await authDBContext.SaveChangesAsync();
                return CommonResponse<InterestsVM>.GetResult(200, true, localizer["RemovedSuccessfully"]);



            }
            catch 
            {
                var related = DependencyValidator<Interests>.GetDependenciesNames(Obj).ToList();
                var Message = string.Format(localizer["PlzDeleteDataRelatedWithObjForCompleteObjectRemove"],
                   Obj?.name,
                    //localizer[Obj.GetType().BaseType.Name),
                    related.Select(x => localizer[x].Value).Aggregate((x, y) => x + "," + y));
                return CommonResponse<InterestsVM>.GetResult(406, false, Message);

            }
        }

        public async Task<CommonResponse<InterestsVM>> ToggleActiveConfigration(string ID, bool IsActive)
        {
            try
            {

                var Interests =  authDBContext.Interests.FirstOrDefault(x=>x.EntityId==ID);
                Interests.IsActive = IsActive;
                await authDBContext.SaveChangesAsync();
                return CommonResponse<InterestsVM>.GetResult(200, true, localizer["UpdatedSuccessfully"]);
            }
            catch
            {
                return CommonResponse<InterestsVM>.GetResult(406, false, "SomthingGoesWrong");

            }

        }
        public InterestsVM GetData(string ID)
        {
            try
            {
                var Obj =  authDBContext.Interests.AsNoTracking().FirstOrDefault(x=>x.EntityId==ID);
                var vM = Converter(Obj);
                return vM;
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public IEnumerable<InterestsVM> GetData()
        {
            var data = authDBContext.Interests.AsNoTracking().Select(Converter).ToList();
            return data;
        }


        Interests Converter(InterestsVM model)
        {
            if (model == null) return null;
            var Obj = new Interests()
            {
                EntityId = model.EntityId,
                Id = model.Id,
                RegistrationDate = model.RegistrationDate,
                IsActive = model.IsActive,
                IsSharedForAllUsers = model.IsSharedForAllUsers,
                CreatedByUserID = httpContextAccessor.HttpContext.GetUser().UserId,
                name = model.name,
            };
            return Obj;
        }
        InterestsVM Converter(Interests model)
        {
            if (model == null) return null;

            var Obj = new InterestsVM
            {
                EntityId = model.EntityId,
                Id = model.Id,
                RegistrationDate = model.RegistrationDate,
                IsActive = model.IsActive,
                IsSharedForAllUsers = model.IsSharedForAllUsers,
                CreatedByUserID = model.CreatedByUserID,
                name = model.name,
                //Id = model.Id

            };
            return Obj;
        }
    }

}



