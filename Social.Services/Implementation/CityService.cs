using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
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
    public class CityService : ICityService
    {
        private readonly AuthDBContext authDBContext;
        private readonly IStringLocalizer<SharedResource> localizer;
        private readonly IGlobalMethodsService globalMethodsService;
        private readonly IHttpContextAccessor httpContextAccessor;

        public CityService(AuthDBContext authDBContext, IStringLocalizer<SharedResource> _localizer, IGlobalMethodsService globalMethodsService, IHttpContextAccessor httpContextAccessor)
        {
            this.authDBContext = authDBContext;
            localizer = _localizer;
            this.globalMethodsService = globalMethodsService;
            this.httpContextAccessor = httpContextAccessor;
        }
        public async Task<CommonResponse<CityVM>> Create(CityVM VM)
        {
            try
            {
                var _City = await authDBContext.Cities.FirstOrDefaultAsync(x => x.GoogleName == VM.GoogleName);
                if (_City != null)
                {
                    VM.ID = _City.ID;
                    return CommonResponse<CityVM>.GetResult(200, true, localizer["SavedSuccessfully"], VM);
                }
            
                var Obj = Converter(VM);
                await authDBContext.Cities.AddAsync(Obj);
                await authDBContext.SaveChangesAsync();
                VM.ID = Obj.ID;
                return CommonResponse<CityVM>.GetResult(200, true, localizer["SavedSuccessfully"],VM);
            }
            catch (DbUpdateException e)
            when (e.InnerException?.InnerException is SqlException sqlEx && (sqlEx.Number == 2601 || sqlEx.Number == 2627))
            {
                var City = await authDBContext.Cities.FirstOrDefaultAsync(x => x.GoogleName == VM.GoogleName);
                if (City == null)
                {
                    return CommonResponse<CityVM>.GetResult(403, false, "ex");
                }
                else
                {
                    VM.ID = City.ID;
                    return CommonResponse<CityVM>.GetResult(200, true, localizer["SavedSuccessfully"], VM);
                }
            }
            catch (Exception ex)
            {
                return CommonResponse<CityVM>.GetResult(403, false, ex.Message);
            }
        }


        public async Task<CommonResponse<CityVM>> Edit(CityVM VM)
        {

            var Obj = Converter(VM);

            try
            {
                authDBContext.Attach(Obj);
                authDBContext.Entry(Obj).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                //authDBContext.Citys.Update(Obj);
                await authDBContext.SaveChangesAsync();
                return CommonResponse<CityVM>.GetResult(200, true, localizer["SavedSuccessfully"],VM);
            }
            catch
            {
                return CommonResponse<CityVM>.GetResult(403, false, "ex");
            }
        }
        public IEnumerable<ValidationResult> _ValidationResult(CityVM VM)
        {
            if (authDBContext.Countries.Any(x => x.ID != VM.ID && x.GoogleName.ToLower() == VM.GoogleName.ToLower()))
            {
                var Message = string.Format(localizer["AlreadyExist"], VM.GoogleName);
                yield return new ValidationResult(Message, new[] { nameof(VM.GoogleName) });
            }


        }
        public async Task<CommonResponse<CityVM>> Remove(int ID)
        {
            var Obj = await authDBContext.Cities.FindAsync(ID);
            try
            {
                authDBContext.Cities.Remove(Obj);
                await authDBContext.SaveChangesAsync();
                return CommonResponse<CityVM>.GetResult(200, true, localizer["RemovedSuccessfully"]);



            }
            catch 
            {
                var related = DependencyValidator<City>.GetDependenciesNames(Obj).ToList();
                var Message = string.Format(localizer["PlzDeleteDataRelatedWithObjForCompleteObjectRemove"],
                   Obj?.DisplayName,
                    //localizer[Obj.GetType().BaseType.Name),
                    related.Select(x => localizer[x].Value).Aggregate((x, y) => x + "," + y));
                return CommonResponse<CityVM>.GetResult(403, false, Message);

            }
        }


        public async Task<CityVM> GetData(int ID)
        {
            try
            {
                var Obj = await authDBContext.Cities.FindAsync(ID);
                var vM = Converter(Obj);
                return vM;
            }
            catch
            {
                return new CityVM();
            }
        }

        public IEnumerable<CityVM> GetData()
        {
            var data = authDBContext.Cities.Select(Converter).ToList();
            return data;
        }
        

        City Converter(CityVM model)
        {
            if (model == null) return null;

          
            var Obj = new City()
            {
                ID = model.ID,
                GoogleName = model.GoogleName,
                DisplayName = model.DisplayName,
             
            };

            return Obj;
        }
        CityVM Converter(City model)
        {
            if (model == null) return null;

            var Obj = new CityVM
            {
                ID = model.ID,
                GoogleName = model.GoogleName,
                DisplayName = model.DisplayName,

            };
            return Obj;
        }
    }

}



