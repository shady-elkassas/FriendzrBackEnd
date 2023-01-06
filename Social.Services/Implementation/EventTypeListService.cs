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
    public class EventTypeListService : IEventTypeListService
    {
        private readonly AuthDBContext authDBContext;
        private readonly IStringLocalizer<SharedResource> localizer;
        private readonly IHttpContextAccessor httpContextAccessor;

        public EventTypeListService(AuthDBContext authDBContext, IStringLocalizer<SharedResource> _localizer, IHttpContextAccessor httpContextAccessor)
        {
            this.authDBContext = authDBContext;
            localizer = _localizer;
            this.httpContextAccessor = httpContextAccessor;
        }
        public async Task<CommonResponse<EventTypeListVM>> Create(EventTypeListVM VM)
        {
            try
            {
                var Obj = Converter(VM);


                await authDBContext.EventTypeList.AddAsync(Obj);
                await authDBContext.SaveChangesAsync();

                return CommonResponse<EventTypeListVM>.GetResult(200, true, localizer["SavedSuccessfully"]);
            }
            catch
            {
                return CommonResponse<EventTypeListVM>.GetResult(406, false, "ex");
            }
        }


        public async Task<CommonResponse<EventTypeListVM>> Edit(EventTypeListVM VM)
        {

            var Obj = Converter(VM);

            try
            {

                authDBContext.Attach(Obj);
                authDBContext.Entry(Obj).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                //authDBContext.EventTypeLists.Update(Obj);
                await authDBContext.SaveChangesAsync();
                return CommonResponse<EventTypeListVM>.GetResult(200, true, localizer["SavedSuccessfully"]);
            }
            catch
            {
                return CommonResponse<EventTypeListVM>.GetResult(406, false, "ex");
            }
        }
        public async Task<CommonResponse<EventTypeListVM>> Remove(Guid ID)
        {
            var Obj = await authDBContext.EventTypeList.FirstOrDefaultAsync(x => x.entityID == ID);
            try
            {
                authDBContext.EventTypeList.Remove(Obj);
                await authDBContext.SaveChangesAsync();
                return CommonResponse<EventTypeListVM>.GetResult(200, true, localizer["RemovedSuccessfully"]);
            }
            catch (Exception ex)
            {
                var related = DependencyValidator<EventTypeList>.GetDependenciesNames(Obj).ToList();
                var Message = string.Format(localizer["PlzDeleteDataRelatedWithObjForCompleteObjectRemove"],
                   Obj?.Name,
                    //localizer[Obj.GetType().BaseType.Name),
                    related.Select(x => localizer[x].Value).Aggregate((x, y) => x + "," + y));
                return CommonResponse<EventTypeListVM>.GetResult(406, false, Message);

            }
        }
        public async Task<EventTypeListVM> GetData(Guid ID)
        {
            try
            {
                var Obj = await authDBContext.EventTypeList.FirstOrDefaultAsync(x => x.entityID == ID);
                var vM = Converter(Obj);
                return vM;
            }
            catch
            {
                return new EventTypeListVM();
            }
        }

        public async Task<int>GetEventTypeListId(string name)
        {
            var item = await authDBContext.EventTypeList.FirstOrDefaultAsync(e => e.Name == name);
            return item.ID;
        }
        public bool IsPrivteKey(Guid ID)
        {
           
                return authDBContext.EventTypeList.Any(x => x.entityID == ID && x.key == true);
            
           
        }
        public IEnumerable<EventTypeListVM> GetData()
        {
            var data = authDBContext.EventTypeList.OrderByDescending(x => x.ID).Select(Converter).ToList();
            return data;
        }

        public async Task<CommonResponse<EventTypeListVM>> ToggleActiveConfigration(Guid ID, bool IsActive)
        {
            try
            {

                var EventTypeList = await authDBContext.EventTypeList.FindAsync(ID);
                //EventTypeList.IsActive = IsActive;
                await authDBContext.SaveChangesAsync();
                return CommonResponse<EventTypeListVM>.GetResult(200, true, localizer["UpdatedSuccessfully"]);
            }
            catch
            {
                return CommonResponse<EventTypeListVM>.GetResult(406, false, "SomthingGoesWrong");

            }

        }
        public IEnumerable<ValidationResult> _ValidationResult(EventTypeListVM VM)
        {

            if (authDBContext.EventTypeList.Any(x => x.ID != VM.ID && x.Name.ToLower() == VM.Name.ToLower()))
            {
                var Message = string.Format(localizer["AlreadyExist"], VM.Name);
                yield return new ValidationResult(Message, new[] { nameof(VM.Name) });
            }
            //else if(authDBContext.EventTypeLists.Any(x => x.ID != ID && x.DisplayOrder == VM.DisplayOrder))
            //{
            //    var Message = string.Format(localizer["AlreadyExist"], VM.DisplayOrder);
            //    yield return new ValidationResult(Message, new[] { nameof(VM.DisplayOrder) });
            //}

        }
        EventTypeList Converter(EventTypeListVM model)
        {
            if (model == null) return null;

            var Obj = new EventTypeList()
            {
                Name = model.Name,
                //IsActive = model.IsActive,
                ID = model.ID,
                color = model.color,
                key = model.Privtekey,
                
                entityID = model.EntityId,

            };
            return Obj;
        }
        EventTypeListVM Converter(EventTypeList model)
        {
            if (model == null) return null;
            var Obj = new EventTypeListVM
            {
                ID = model.ID ,
                color=model.color,
                EntityId=model.entityID,
                Privtekey=model.key,
                //IsActive = model.IsActive,
                Name = model.Name,
            };
            return Obj;
        }
    }

}



