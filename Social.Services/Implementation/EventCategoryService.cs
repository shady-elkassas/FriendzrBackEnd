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
    public class EventCategoryService : IEventCategoryService
    {
        private readonly AuthDBContext authDBContext;
        private readonly IStringLocalizer<SharedResource> localizer;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IUserService userService;

        public EventCategoryService(AuthDBContext authDBContext,
            IStringLocalizer<SharedResource> _localizer, IHttpContextAccessor httpContextAccessor, IUserService userService)
        {
            this.authDBContext = authDBContext;
            localizer = _localizer;
            this.httpContextAccessor = httpContextAccessor;
            this.userService = userService;
        }
        public async Task<CommonResponse<EventCategoryVM>> Create(EventCategoryVM VM)
        {
            try
            {
                VM.EntityId = Guid.NewGuid().ToString();
                VM.IsActive = true;
                var Obj = Converter(VM);
                await authDBContext.category.AddAsync(Obj);
                await authDBContext.SaveChangesAsync();
             
                return CommonResponse<EventCategoryVM>.GetResult(200, true, localizer["SavedSuccessfully"], VM);
            }
            catch
            {
                return CommonResponse<EventCategoryVM>.GetResult(406, false, "ex");
            }
        }


        public async Task<CommonResponse<EventCategoryVM>> Edit(EventCategoryVM VM)
        {
            VM.IsActive = true;
            var Obj = Converter(VM);
            try
            {

                authDBContext.Attach(Obj);
                authDBContext.Entry(Obj).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                //authDBContext.EventCategorys.Update(Obj);
                authDBContext.SaveChanges();
                return CommonResponse<EventCategoryVM>.GetResult(200, true, localizer["SavedSuccessfully"], VM);

            }
            catch (Exception ex)
            {
                return CommonResponse<EventCategoryVM>.GetResult(406, false, "ex");
            }
        }
        public async Task<CommonResponse<EventCategoryVM>> Remove(string ID)
        {
            var Obj = authDBContext.category.FirstOrDefault(x => x.EntityId == ID);
            try

            {
                authDBContext.category.Remove(Obj);
                await authDBContext.SaveChangesAsync();
                return CommonResponse<EventCategoryVM>.GetResult(200, true, localizer["RemovedSuccessfully"]);
            }
            catch
            {
                var related = DependencyValidator<category>.GetDependenciesNames(Obj).ToList();
                var Message = string.Format(localizer["PlzDeleteDataRelatedWithObjForCompleteObjectRemove"],
                   Obj?.name,
                    //localizer[Obj.GetType().BaseType.Name),
                    related.Select(x => localizer[x].Value).Aggregate((x, y) => x + "," + y));
                return CommonResponse<EventCategoryVM>.GetResult(406, false, Message);

            }
        }

        public async Task<CommonResponse<EventCategoryVM>> ToggleActiveConfigration(string ID, bool IsActive)
        {
            try
            {

                var EventCategory = authDBContext.category.FirstOrDefault(x => x.EntityId == ID);
                EventCategory.IsActive = IsActive;
                await authDBContext.SaveChangesAsync();
                return CommonResponse<EventCategoryVM>.GetResult(200, true, localizer["UpdatedSuccessfully"]);
            }
            catch
            {
                return CommonResponse<EventCategoryVM>.GetResult(406, false, "SomthingGoesWrong");

            }

        }
        public EventCategoryVM GetData(string ID)
        {
            try
            {
                var Obj = authDBContext.category.FirstOrDefault(x => x.EntityId == ID);
                var vM = Converter(Obj);
                return vM;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public IEnumerable<ValidationResult> _ValidationResult(EventCategoryVM VM)
        {
            if (authDBContext.category.Any(x => x.Id != VM.Id && x.name.ToLower() == VM.name.ToLower()))
            {
                var Message = string.Format(localizer["AlreadyExist"], VM.name);
                yield return new ValidationResult(Message, new[] { nameof(VM.name) });
            }


        }
        public IEnumerable<EventCategoryVM> GetData()
        {
            var data = authDBContext.category.Select(Converter).ToList();
            return data;
        }


        category Converter(EventCategoryVM model)
        {
            if (model == null) return null;
            var Obj = new category()
            {
                EntityId = model.EntityId,
                Id = model.Id,
                RegistrationDate = model.RegistrationDate,
                IsActive = model.IsActive,
                //CreatedByUserID = httpContextAccessor.HttpContext.GetUser().UserId,
                name = model.name,
            };
            return Obj;
        }
        EventCategoryVM Converter(category model)
        {
            if (model == null) return null;

            var Obj = new EventCategoryVM
            {
                EntityId = model.EntityId,
                Id = model.Id,
                RegistrationDate = model.RegistrationDate,
                IsActive = model.IsActive,
                
                name = model.name,
            };
            return Obj;
        }
    }

}



