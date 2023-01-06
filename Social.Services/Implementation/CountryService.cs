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
    public class CountryService : ICountryService
    {
        private readonly AuthDBContext authDBContext;
        private readonly IStringLocalizer<SharedResource> localizer;
        private readonly IGlobalMethodsService globalMethodsService;
        private readonly IHttpContextAccessor httpContextAccessor;

        public CountryService(AuthDBContext authDBContext, IStringLocalizer<SharedResource> _localizer, IGlobalMethodsService globalMethodsService, IHttpContextAccessor httpContextAccessor)
        {
            this.authDBContext = authDBContext;
            localizer = _localizer;
            this.globalMethodsService = globalMethodsService;
            this.httpContextAccessor = httpContextAccessor;
        }
        public async Task<CommonResponse<CountryVM>> Create(CountryVM VM)
        {
            try
            {
                var _Country = await authDBContext.Countries.FirstOrDefaultAsync(x => x.GoogleName == VM.GoogleName);
                if (_Country != null)
                {
                    VM.ID = _Country.ID;
                    return CommonResponse<CountryVM>.GetResult(200, true, localizer["SavedSuccessfully"], VM);
                }
            
                var Obj = Converter(VM);
                await authDBContext.Countries.AddAsync(Obj);
                await authDBContext.SaveChangesAsync();
                VM.ID = Obj.ID;
                return CommonResponse<CountryVM>.GetResult(200, true, localizer["SavedSuccessfully"],VM);
            }
            catch (DbUpdateException e)
            when (e.InnerException?.InnerException is SqlException sqlEx && (sqlEx.Number == 2601 || sqlEx.Number == 2627))
            {
                var Country = await authDBContext.Countries.FirstOrDefaultAsync(x => x.GoogleName == VM.GoogleName);
                if (Country == null)
                {
                    return CommonResponse<CountryVM>.GetResult(403, false, "ex");
                }
                else
                {
                    VM.ID = Country.ID;
                    return CommonResponse<CountryVM>.GetResult(200, true, localizer["SavedSuccessfully"], VM);
                }
            }
            catch
            {
                return CommonResponse<CountryVM>.GetResult(403, false, "ex");
            }
        }


        public async Task<CommonResponse<CountryVM>> Edit(CountryVM VM)
        {

            var Obj = Converter(VM);

            try
            {
                authDBContext.Attach(Obj);
                authDBContext.Entry(Obj).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                //authDBContext.Countrys.Update(Obj);
                await authDBContext.SaveChangesAsync();
                return CommonResponse<CountryVM>.GetResult(200, true, localizer["SavedSuccessfully"],VM);
            }
            catch
            {
                return CommonResponse<CountryVM>.GetResult(403, false, "ex");
            }
        }
       
        public async Task<CommonResponse<CountryVM>> Remove(int ID)
        {
            var Obj = await authDBContext.Countries.FindAsync(ID);
            try
            {
                authDBContext.Countries.Remove(Obj);
                await authDBContext.SaveChangesAsync();
                return CommonResponse<CountryVM>.GetResult(200, true, localizer["RemovedSuccessfully"]);



            }
            catch 
            {
                var related = DependencyValidator<Country>.GetDependenciesNames(Obj).ToList();
                var Message = string.Format(localizer["PlzDeleteDataRelatedWithObjForCompleteObjectRemove"],
                   Obj?.DisplayName,
                    //localizer[Obj.GetType().BaseType.Name),
                    related.Select(x => localizer[x].Value).Aggregate((x, y) => x + "," + y));
                return CommonResponse<CountryVM>.GetResult(403, false, Message);

            }
        }


        public async Task<CountryVM> GetData(int ID)
        {
            try
            {
                var Obj = await authDBContext.Countries.FindAsync(ID);
                var vM = Converter(Obj);
                return vM;
            }
            catch
            {
                return new CountryVM();
            }
        }
        public IEnumerable<ValidationResult> _ValidationResult(CountryVM VM)
        {
                   if (authDBContext.Countries.Any(x => x.ID != VM.ID && x.GoogleName.ToLower() == VM.GoogleName.ToLower()))
            {
                var Message = string.Format(localizer["AlreadyExist"], VM.GoogleName);
                yield return new ValidationResult(Message, new[] { nameof(VM.GoogleName) });
            }
          

        }
        public IEnumerable<CountryVM> GetData()
        {
            var data = authDBContext.Countries.Select(Converter).ToList();
            return data;
        }
        

        Country Converter(CountryVM model)
        {
            if (model == null) return null;

          
            var Obj = new Country()
            {
                ID = model.ID,
                GoogleName = model.GoogleName,
                DisplayName = model.DisplayName,
             
            };

            return Obj;
        }
        CountryVM Converter(Country model)
        {
            if (model == null) return null;

            var Obj = new CountryVM
            {
                ID = model.ID,
                GoogleName = model.GoogleName,
                DisplayName = model.DisplayName,

            };
            return Obj;
        }
    }

}



