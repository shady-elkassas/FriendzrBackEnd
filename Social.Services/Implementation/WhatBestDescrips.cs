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
   public class WhatBestDescrips: IWhatBestDescripsMe
    {
        private readonly AuthDBContext authDBContext;
        private readonly IStringLocalizer<SharedResource> localizer;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IUserService userService;

        public WhatBestDescrips(AuthDBContext authDBContext, IStringLocalizer<SharedResource> _localizer, IHttpContextAccessor httpContextAccessor, IUserService userService)
        {
            this.authDBContext = authDBContext;
            localizer = _localizer;
            this.httpContextAccessor = httpContextAccessor;
            this.userService = userService;
        }
        public async Task<CommonResponse<WhatBestDescripsMeVM>> Create(WhatBestDescripsMeVM VM)
        {
            try
            {
                VM.IsActive = true;
                var Obj = Converter(VM);
                await authDBContext.WhatBestDescripsMe.AddAsync(Obj);
                await authDBContext.SaveChangesAsync();
                var WhatBestDescripsMe = new WhatBestDescripsMeList()
                {
                    WhatBestDescripsMeId = Obj.Id,
                    Tagsname = Obj.name,
                    UserId = httpContextAccessor.HttpContext.GetUser().User.UserDetails.PrimaryId
                };
                if (VM.IsSharedForAllUsers == false)
                    userService.addWhatBestDescripsMe(WhatBestDescripsMe);

                //VM.Id = Obj.EntityId;
                return CommonResponse<WhatBestDescripsMeVM>.GetResult(200, true, localizer["SavedSuccessfully"], VM);
            }
            catch
            {
                return CommonResponse<WhatBestDescripsMeVM>.GetResult(406, false, "ex");
            }
        }


        public async Task<CommonResponse<WhatBestDescripsMeVM>> Edit(WhatBestDescripsMeVM VM)
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
                return CommonResponse<WhatBestDescripsMeVM>.GetResult(200, true, localizer["SavedSuccessfully"], VM);

            }
            catch (Exception ex)
            {
                return CommonResponse<WhatBestDescripsMeVM>.GetResult(406, false, "ex");
            }
        }
        public async Task<CommonResponse<WhatBestDescripsMeVM>> Remove(string ID)
        {
            var Obj = authDBContext.WhatBestDescripsMe.FirstOrDefault(x => x.Id ==Convert.ToInt32( ID));
            try

            {
                authDBContext.WhatBestDescripsMeList.RemoveRange(Obj.WhatBestDescripsMeList);
                authDBContext.WhatBestDescripsMe.Remove(Obj);
                await authDBContext.SaveChangesAsync();
                return CommonResponse<WhatBestDescripsMeVM>.GetResult(200, true, localizer["RemovedSuccessfully"]);



            }
            catch
            {
                var related = DependencyValidator<WhatBestDescripsMe>.GetDependenciesNames(Obj).ToList();
                var Message = string.Format(localizer["PlzDeleteDataRelatedWithObjForCompleteObjectRemove"],
                   Obj?.name,
                    //localizer[Obj.GetType().BaseType.Name),
                    related.Select(x => localizer[x].Value).Aggregate((x, y) => x + "," + y));
                return CommonResponse<WhatBestDescripsMeVM>.GetResult(406, false, Message);

            }
        }

        public async Task<CommonResponse<WhatBestDescripsMeVM>> ToggleActiveConfigration(string ID, bool IsActive)
        {
            try
            {

                var WhatBestDescripsMe = authDBContext.WhatBestDescripsMe.FirstOrDefault(x => x.EntityId == ID);
                WhatBestDescripsMe.IsActive = IsActive;
                await authDBContext.SaveChangesAsync();
                return CommonResponse<WhatBestDescripsMeVM>.GetResult(200, true, localizer["UpdatedSuccessfully"]);
            }
            catch
            {
                return CommonResponse<WhatBestDescripsMeVM>.GetResult(406, false, "SomthingGoesWrong");

            }

        }
        public WhatBestDescripsMeVM GetData(string ID)
        {
            try
            {
                var Obj = authDBContext.WhatBestDescripsMe.AsNoTracking().FirstOrDefault(x => x.Id ==Convert.ToInt32( ID));
                var vM = Converter(Obj);
                return vM;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public IEnumerable<WhatBestDescripsMeVM> GetData()
        {
            var data = authDBContext.WhatBestDescripsMe.AsNoTracking().Select(Converter).ToList();
            return data;
        }


        WhatBestDescripsMe Converter(WhatBestDescripsMeVM model)
        {
            if (model == null) return null;
            var Obj = new WhatBestDescripsMe()
            {
                EntityId =   Guid.NewGuid().ToString(),
            Id = model.ID,
                RegistrationDate = model.RegistrationDate,
                IsActive = model.IsActive,
                IsSharedForAllUsers = model.IsSharedForAllUsers,
                CreatedByUserID = httpContextAccessor.HttpContext.GetUser().UserId,
                name = model.name,
            };
            return Obj;
        }
        WhatBestDescripsMeVM Converter(WhatBestDescripsMe model)
        {
            if (model == null) return null;

            var Obj = new WhatBestDescripsMeVM
            {
                EntityId = model.EntityId,
                ID = model.Id,
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
