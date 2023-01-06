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
    public class AppConfigrationService : IAppConfigrationService
    {
        private readonly AuthDBContext authDBContext;
        private readonly IStringLocalizer<SharedResource> localizer;
        private readonly IHttpContextAccessor httpContextAccessor;

        public AppConfigrationService(AuthDBContext authDBContext, IStringLocalizer<SharedResource> _localizer, IHttpContextAccessor httpContextAccessor)
        {
            this.authDBContext = authDBContext;
            localizer = _localizer;
            this.httpContextAccessor = httpContextAccessor;
        }
        //public async Task<CommonResponse<AppConfigrationVM>> Create(AppConfigrationVM VM)
        //{
        //    try
        //    {
        //        var Obj = Converter(VM);
        //        await authDBContext.AppConfigrations.AddAsync(Obj);
        //        await authDBContext.SaveChangesAsync();

        //        return CommonResponse<AppConfigrationVM>.GetResult(200, true, localizer["SavedSuccessfully"]);
        //    }
        //    catch
        //    {
        //        return CommonResponse<AppConfigrationVM>.GetResult(403, false, "ex");
        //    }
        //}  
        public async Task<CommonResponse<AppConfigrationVM>> Create(AppConfigrationVM_Required VM)
        {
            try
            {
                var Obj = Converter(VM);
                await authDBContext.AppConfigrations.AddAsync(Obj);
                await authDBContext.SaveChangesAsync();

                return CommonResponse<AppConfigrationVM>.GetResult(200, true, localizer["SavedSuccessfully"]);
            }
            catch
            {
                return CommonResponse<AppConfigrationVM>.GetResult(403, false, "ex");
            }
        }


        public async Task<CommonResponse<AppConfigrationVM>> Edit(AppConfigrationVM VM)
        {
            var ss = Guid.TryParse(VM.ID, out Guid ID);
            ID = ss ? ID : Guid.NewGuid();
            var old = await authDBContext.AppConfigrations.FindAsync(ID);
            var Obj = Converter(VM, old);

            try
            {
                authDBContext.Attach(Obj);
                authDBContext.Entry(Obj).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                //authDBContext.AppConfigrations.Update(Obj);
                await authDBContext.SaveChangesAsync();
                return CommonResponse<AppConfigrationVM>.GetResult(200, true, localizer["SavedSuccessfully"]);
            }
            catch
            {
                return CommonResponse<AppConfigrationVM>.GetResult(403, false, "ex");
            }
        }
        public async Task<CommonResponse<AppConfigrationVM>> Edit(AppConfigrationVM_Required VM)
        {
            var ss = Guid.TryParse(VM.ID, out Guid ID);
            ID = ss ? ID : Guid.NewGuid();
            var old = await authDBContext.AppConfigrations.FindAsync(ID);
            var Obj = Converter(VM, old);
            var allolduserdata = authDBContext.UserDetails;
            var olduserdata = allolduserdata.Where(n => n.Manualdistancecontrol > VM.DistanceShowNearbyAccountsInFeed_Max || n.Manualdistancecontrol < VM.DistanceShowNearbyAccountsInFeed_Min);
            var olduseagerdata = allolduserdata.Where(n => n.ageto > VM.AgeFiltering_Max || n.agefrom < VM.AgeFiltering_Min);

            await olduserdata.ForEachAsync(m =>
            {
                m.Manualdistancecontrol = (decimal)VM.DistanceShowNearbyAccountsInFeed_Max;
            });
            await olduseagerdata.ForEachAsync(m =>
            {
                m.ageto =(int) VM.AgeFiltering_Max;
                m.agefrom = (int)VM.AgeFiltering_Min;
            });
            authDBContext.UserDetails.UpdateRange(olduserdata);
            authDBContext.UserDetails.UpdateRange(olduseagerdata);
            try
            {
                authDBContext.Attach(Obj);
                authDBContext.Entry(Obj).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                await authDBContext.SaveChangesAsync();
                return CommonResponse<AppConfigrationVM>.GetResult(200, true, localizer["SavedSuccessfully"]);
            }
            catch (Exception ex)
            {
                return CommonResponse<AppConfigrationVM>.GetResult(403, false, "ex");
            }
        }
        public async Task<CommonResponse<AppConfigrationVM>> Remove(Guid ID)
        {
            var Obj = await authDBContext.AppConfigrations.FindAsync(ID);
            try
            {
                authDBContext.AppConfigrations.Remove(Obj);
                await authDBContext.SaveChangesAsync();
                return CommonResponse<AppConfigrationVM>.GetResult(200, true, localizer["RemovedSuccessfully"]);



            }
            catch
            {
                var related = DependencyValidator<AppConfigration>.GetDependenciesNames(Obj).ToList();
                var Message = string.Format(localizer["PlzDeleteDataRelatedWithObjForCompleteObjectRemove"],
                   Obj?.Name,
                    //localizer[Obj.GetType().BaseType.Name),
                    related.Select(x => localizer[x].Value).Aggregate((x, y) => x + "," + y));
                return CommonResponse<AppConfigrationVM>.GetResult(403, false, Message);

            }
        }


        public async Task<AppConfigrationVM> GetData(Guid ID)
        {
            try
            {
                var Obj = await authDBContext.AppConfigrations.FindAsync(ID);
                var vM = Converter(Obj);
                return vM;
            }
            catch
            {
                return new AppConfigrationVM();
            }
        }

        public IEnumerable<AppConfigrationVM> GetData()
        {
            var data = authDBContext.AppConfigrations.Select(Converter).ToList();
          
            return data;
        }
        public async Task<CommonResponse<AppConfigrationVM>> ToggleActiveConfigration(Guid ID, bool IsActive)
        {
            try
            {

                var allConfig = await authDBContext.AppConfigrations.ToListAsync();
                allConfig.ForEach(x =>
                {
                    if (x.ID == ID)
                        x.IsActive = IsActive;
                    else
                        x.IsActive = false;
                });
                await authDBContext.SaveChangesAsync();
                return CommonResponse<AppConfigrationVM>.GetResult(200, true, localizer["UpdatedSuccessfully"]);
            }
            catch
            {
                return CommonResponse<AppConfigrationVM>.GetResult(403, false, "SomthingGoesWrong");

            }

        }
        public IEnumerable<ValidationResult> _ValidationResult(AppConfigrationVM_Required VM)
        {
            var ss = Guid.TryParse(VM.ID, out Guid ID);
            ID = ss ? ID : Guid.NewGuid();
            if (VM.AgeFiltering_Max < VM.AgeFiltering_Min)
            {
                var Message = string.Format(localizer["NumberMoreThan"], VM.AgeFiltering_Min);
                yield return new ValidationResult(Message, new[] { nameof(VM.AgeFiltering_Max) });
            }

            if (VM.Password_MaxSpecialCharacters < VM.Password_MinSpecialCharacters)
            {
                var Message = string.Format(localizer["NumberMoreThan"], VM.Password_MinSpecialCharacters);
                yield return new ValidationResult(Message, new[] { nameof(VM.Password_MaxSpecialCharacters) });
            }

            if (VM.Password_MaxNumbers < VM.Password_MinNumbers)
            {
                var Message = string.Format(localizer["NumberMoreThan"], VM.Password_MinNumbers);
                yield return new ValidationResult(Message, new[] { nameof(VM.Password_MaxNumbers) });
            }

            if (VM.Password_MaxLength < VM.Password_MinLength)
            {
                var Message = string.Format(localizer["NumberMoreThan"], VM.Password_MinLength);
                yield return new ValidationResult(Message, new[] { nameof(VM.Password_MaxLength) });
            }

            if (VM.UserBio_MaxLength < VM.UserBio_MinLength)
            {
                var Message = string.Format(localizer["NumberMoreThan"], VM.UserBio_MinLength);
                yield return new ValidationResult(Message, new[] { nameof(VM.UserBio_MaxLength) });
            }

            if (VM.UserMaxAge < VM.UserMinAge)
            {
                var Message = string.Format(localizer["NumberMoreThan"], VM.UserMinAge);
                yield return new ValidationResult(Message, new[] { nameof(VM.UserMaxAge) });
            }

            if (VM.UserName_MaxLength < VM.UserName_MinLength)
            {
                var Message = string.Format(localizer["NumberMoreThan"], VM.UserName_MinLength);
                yield return new ValidationResult(Message, new[] { nameof(VM.UserName_MaxLength) });
            }

            if (VM.UserTagM_MaxNumber < VM.UserTagM_MinNumber)
            {
                var Message = string.Format(localizer["NumberMoreThan"], VM.UserTagM_MinNumber);
                yield return new ValidationResult(Message, new[] { nameof(VM.UserTagM_MaxNumber) });
            }

            if (VM.EventDetailsDescription_MaxLength < VM.EventDetailsDescription_MinLength)
            {
                var Message = string.Format(localizer["NumberMoreThan"], VM.EventDetailsDescription_MinLength);
                yield return new ValidationResult(Message, new[] { nameof(VM.EventDetailsDescription_MaxLength) });
            }
            if (VM.EventTitle_MaxLength < VM.EventTitle_MinLength)
            {
                var Message = string.Format(localizer["NumberMoreThan"], VM.EventTitle_MinLength);
                yield return new ValidationResult(Message, new[] { nameof(VM.EventTitle_MaxLength) });
            }

            if (VM.DistanceShowNearbyEventsOnMap_Max < VM.DistanceShowNearbyEventsOnMap_Min)
            {
                var Message = string.Format(localizer["NumberMoreThan"], VM.DistanceShowNearbyEventsOnMap_Min);
                yield return new ValidationResult(Message, new[] { nameof(VM.DistanceShowNearbyEventsOnMap_Max) });
            }

            if (VM.DistanceShowNearbyEvents_Max < VM.DistanceShowNearbyEvents_Min)
            {
                var Message = string.Format(localizer["NumberMoreThan"], VM.DistanceShowNearbyEvents_Min);
                yield return new ValidationResult(Message, new[] { nameof(VM.DistanceShowNearbyEvents_Max) });
            }

            if (VM.DistanceShowNearbyAccountsInFeed_Max < VM.DistanceShowNearbyAccountsInFeed_Min)
            {
                var Message = string.Format(localizer["NumberMoreThan"], VM.DistanceShowNearbyAccountsInFeed_Min);
                yield return new ValidationResult(Message, new[] { nameof(VM.DistanceShowNearbyAccountsInFeed_Max) });
            }

            if (VM.DistanceFiltering_Max < VM.DistanceFiltering_Min)
            {
                var Message = string.Format(localizer["NumberMoreThan"], VM.DistanceFiltering_Min);
                yield return new ValidationResult(Message, new[] { nameof(VM.DistanceFiltering_Max) });
            }
            if (VM.UserIAM_MaxLength < VM.UserIAM_MinLength)
            {
                var Message = string.Format(localizer["NumberMoreThan"], VM.UserIAM_MinLength);
                yield return new ValidationResult(Message, new[] { nameof(VM.UserIAM_MaxLength) });
            }
            if (VM.UserIPreferTo_MaxLength < VM.UserIPreferTo_MinLength)
            {
                var Message = string.Format(localizer["NumberMoreThan"], VM.UserIPreferTo_MinLength);
                yield return new ValidationResult(Message, new[] { nameof(VM.UserIPreferTo_MaxLength) });
            }

            if (VM.RecommendedPeopleArea_Max < VM.RecommendedPeopleArea_Min)
            {
                var Message = string.Format(localizer["NumberMoreThan"], VM.RecommendedPeopleArea_Min);
                yield return new ValidationResult(Message, new[] { nameof(VM.RecommendedPeopleArea_Max) });
            }
            if (VM.RecommendedEventArea_Max < VM.RecommendedEventArea_Min)
            {
                var Message = string.Format(localizer["NumberMoreThan"], VM.RecommendedEventArea_Min);
                yield return new ValidationResult(Message, new[] { nameof(VM.RecommendedEventArea_Max) });
            }


            if (authDBContext.AppConfigrations.Any(x => x.ID != ID && x.Name.ToLower() == VM.Name.ToLower()))
            {
                var Message = string.Format(localizer["AlreadyExist"], VM.Name);
                yield return new ValidationResult(Message, new[] { nameof(VM.Name) });
            }

        }
        public IEnumerable<ValidationResult> _ValidationResult(AppConfigrationVM VM)
        {
            var ss = Guid.TryParse(VM.ID, out Guid ID);
            ID = ss ? ID : Guid.NewGuid();
            if (VM.AgeFiltering_Max < VM.AgeFiltering_Min)
            {
                var Message = string.Format(localizer["NumberMoreThan"], VM.AgeFiltering_Min);
                yield return new ValidationResult(Message, new[] { nameof(VM.AgeFiltering_Max) });
            }

            if (VM.Password_MaxSpecialCharacters < VM.Password_MinSpecialCharacters)
            {
                var Message = string.Format(localizer["NumberMoreThan"], VM.Password_MinSpecialCharacters);
                yield return new ValidationResult(Message, new[] { nameof(VM.Password_MaxSpecialCharacters) });
            }

            if (VM.Password_MaxNumbers < VM.Password_MinNumbers)
            {
                var Message = string.Format(localizer["NumberMoreThan"], VM.Password_MinNumbers);
                yield return new ValidationResult(Message, new[] { nameof(VM.Password_MaxNumbers) });
            }

            if (VM.Password_MaxLength < VM.Password_MinLength)
            {
                var Message = string.Format(localizer["NumberMoreThan"], VM.Password_MinLength);
                yield return new ValidationResult(Message, new[] { nameof(VM.Password_MaxLength) });
            }

            if (VM.UserBio_MaxLength < VM.UserBio_MinLength)
            {
                var Message = string.Format(localizer["NumberMoreThan"], VM.UserBio_MinLength);
                yield return new ValidationResult(Message, new[] { nameof(VM.UserBio_MaxLength) });
            }

            if (VM.UserMaxAge < VM.UserMinAge)
            {
                var Message = string.Format(localizer["NumberMoreThan"], VM.UserMinAge);
                yield return new ValidationResult(Message, new[] { nameof(VM.UserMaxAge) });
            }

            if (VM.UserName_MaxLength < VM.UserName_MinLength)
            {
                var Message = string.Format(localizer["NumberMoreThan"], VM.UserName_MinLength);
                yield return new ValidationResult(Message, new[] { nameof(VM.UserName_MaxLength) });
            }
            if (VM.UserIAM_MaxLength < VM.UserIAM_MinLength)
            {
                var Message = string.Format(localizer["NumberMoreThan"], VM.UserIAM_MinLength);
                yield return new ValidationResult(Message, new[] { nameof(VM.UserIAM_MaxLength) });
            }
            if (VM.UserIPreferTo_MaxLength < VM.UserIPreferTo_MinLength)
            {
                var Message = string.Format(localizer["NumberMoreThan"], VM.UserIPreferTo_MinLength);
                yield return new ValidationResult(Message, new[] { nameof(VM.UserIPreferTo_MaxLength) });
            }

            if (VM.UserTagM_MaxNumber < VM.UserTagM_MinNumber)
            {
                var Message = string.Format(localizer["NumberMoreThan"], VM.UserTagM_MinNumber);
                yield return new ValidationResult(Message, new[] { nameof(VM.UserTagM_MaxNumber) });
            }

            if (VM.EventDetailsDescription_MaxLength < VM.EventDetailsDescription_MinLength)
            {
                var Message = string.Format(localizer["NumberMoreThan"], VM.EventDetailsDescription_MinLength);
                yield return new ValidationResult(Message, new[] { nameof(VM.EventDetailsDescription_MaxLength) });
            }
            if (VM.EventTitle_MaxLength < VM.EventTitle_MinLength)
            {
                var Message = string.Format(localizer["NumberMoreThan"], VM.EventTitle_MinLength);
                yield return new ValidationResult(Message, new[] { nameof(VM.EventTitle_MaxLength) });
            }

            if (VM.DistanceShowNearbyEventsOnMap_Max < VM.DistanceShowNearbyEventsOnMap_Min)
            {
                var Message = string.Format(localizer["NumberMoreThan"], VM.DistanceShowNearbyEventsOnMap_Min);
                yield return new ValidationResult(Message, new[] { nameof(VM.DistanceShowNearbyEventsOnMap_Max) });
            }

            if (VM.DistanceShowNearbyEvents_Max < VM.DistanceShowNearbyEvents_Min)
            {
                var Message = string.Format(localizer["NumberMoreThan"], VM.DistanceShowNearbyEvents_Min);
                yield return new ValidationResult(Message, new[] { nameof(VM.DistanceShowNearbyEvents_Max) });
            }

            if (VM.DistanceShowNearbyAccountsInFeed_Max < VM.DistanceShowNearbyAccountsInFeed_Min)
            {
                var Message = string.Format(localizer["NumberMoreThan"], VM.DistanceShowNearbyAccountsInFeed_Min);
                yield return new ValidationResult(Message, new[] { nameof(VM.DistanceShowNearbyAccountsInFeed_Max) });
            }

            if (VM.DistanceFiltering_Max < VM.DistanceFiltering_Min)
            {
                var Message = string.Format(localizer["NumberMoreThan"], VM.DistanceFiltering_Min);
                yield return new ValidationResult(Message, new[] { nameof(VM.DistanceFiltering_Max) });
            }

            if (VM.RecommendedPeopleArea_Max < VM.RecommendedPeopleArea_Min)
            {
                var Message = string.Format(localizer["NumberMoreThan"], VM.RecommendedPeopleArea_Min);
                yield return new ValidationResult(Message, new[] { nameof(VM.RecommendedPeopleArea_Max) });
            }
            if (VM.RecommendedEventArea_Max < VM.RecommendedEventArea_Min)
            {
                var Message = string.Format(localizer["NumberMoreThan"], VM.RecommendedEventArea_Min);
                yield return new ValidationResult(Message, new[] { nameof(VM.RecommendedEventArea_Max) });
            }

            if (authDBContext.AppConfigrations.Any(x => x.ID != ID && x.Name.ToLower() == VM.Name.ToLower()))
            {
                var Message = string.Format(localizer["AlreadyExist"], VM.Name);
                yield return new ValidationResult(Message, new[] { nameof(VM.Name) });
            }

        }
        AppConfigration Converter(AppConfigrationVM model)
        {
            var ss = Guid.TryParse(model.ID, out Guid ID);
            ID = ss ? ID : Guid.NewGuid();
            var Obj = new AppConfigration()
            {
                Name = model.Name,
                AgeFiltering_Max = model.AgeFiltering_Max,
                AgeFiltering_Min = model.AgeFiltering_Min,
                DistanceFiltering_Max = model.DistanceFiltering_Max,
                DistanceFiltering_Min = model.DistanceFiltering_Min,
                EventDetailsDescription_MaxLength = model.EventDetailsDescription_MaxLength,
                EventDetailsDescription_MinLength = model.EventDetailsDescription_MinLength,
                EventTitle_MaxLength = model.EventTitle_MaxLength,
                EventTitle_MinLength = model.EventTitle_MinLength,
                Password_MaxNumbers = model.Password_MaxNumbers,
                Password_MinNumbers = model.Password_MinNumbers,
                Password_MinSpecialCharacters = model.Password_MinSpecialCharacters,
                Password_MaxSpecialCharacters = model.Password_MaxSpecialCharacters,
                Password_MaxLength = model.Password_MaxLength,
                Password_MinLength = model.Password_MinLength,
                UserBio_MaxLength = model.UserBio_MaxLength,
                UserBio_MinLength = model.UserBio_MinLength,
                UserMaxAge = model.UserMaxAge,
                UserMinAge = model.UserMinAge,
                UserName_MaxLength = model.UserName_MaxLength,
                UserName_MinLength = model.UserName_MinLength,
                UserTagM_MinNumber = model.UserTagM_MinNumber,
                UserTagM_MaxNumber = model.UserTagM_MaxNumber,
                RegistrationDate = DateTime.UtcNow,
                //IsActive = model.IsActive,
                DistanceShowNearbyAccountsInFeed_Max = model.DistanceShowNearbyAccountsInFeed_Max,
                DistanceShowNearbyAccountsInFeed_Min = model.DistanceShowNearbyAccountsInFeed_Min,
                DistanceShowNearbyEventsOnMap_Max = model.DistanceShowNearbyEventsOnMap_Max,
                DistanceShowNearbyEventsOnMap_Min = model.DistanceShowNearbyEventsOnMap_Min,
                DistanceShowNearbyEvents_Max = model.DistanceShowNearbyEvents_Max,
                DistanceShowNearbyEvents_Min = model.DistanceShowNearbyEvents_Min,

                RecommendedPeopleArea_Min = model.RecommendedPeopleArea_Min,
                RecommendedPeopleArea_Max = model.RecommendedPeopleArea_Max,
                RecommendedEventArea_Min = model.RecommendedEventArea_Min,
                RecommendedEventArea_Max = model.RecommendedEventArea_Max,
                WhiteLableCodeLength= model.WhiteLableCodeLength,
                UserID = httpContextAccessor.HttpContext.GetUser().UserId,
                ID = ID,

            };
            return Obj;
        }
        AppConfigration Converter(AppConfigrationVM_Required model)
        {
            var ss = Guid.TryParse(model.ID, out Guid ID);
            ID = ss ? ID : Guid.NewGuid();
            var Obj = new AppConfigration()
            {
                Name = model.Name,
                AgeFiltering_Max = model.AgeFiltering_Max,
                AgeFiltering_Min = model.AgeFiltering_Min,
                DistanceFiltering_Max = model.DistanceFiltering_Max,
                DistanceFiltering_Min = model.DistanceFiltering_Min,
                EventDetailsDescription_MaxLength = model.EventDetailsDescription_MaxLength,
                EventDetailsDescription_MinLength = model.EventDetailsDescription_MinLength,
                EventTitle_MaxLength = model.EventTitle_MaxLength,
                EventTitle_MinLength = model.EventTitle_MinLength,
                Password_MaxNumbers = model.Password_MaxNumbers,
                Password_MinNumbers = model.Password_MinNumbers,
                Password_MinSpecialCharacters = model.Password_MinSpecialCharacters,
                Password_MaxSpecialCharacters = model.Password_MaxSpecialCharacters,
                Password_MaxLength = model.Password_MaxLength,
                Password_MinLength = model.Password_MinLength,
                UserBio_MaxLength = model.UserBio_MaxLength,
                UserBio_MinLength = model.UserBio_MinLength,
                UserMaxAge = model.UserMaxAge,
                UserMinAge = model.UserMinAge,
                UserName_MaxLength = model.UserName_MaxLength,
                UserName_MinLength = model.UserName_MinLength,
                UserIAM_MaxLength = model.UserIAM_MaxLength,
                UserIAM_MinLength = model.UserIAM_MinLength,
                UserIPreferTo_MaxLength = model.UserIPreferTo_MaxLength,
                UserIPreferTo_MinLength = model.UserIPreferTo_MinLength,
                EventCreationLimitNumber_MaxLength = model.EventCreationLimitNumber_MaxLength,
                EventCreationLimitNumber_MinLength = model.EventCreationLimitNumber_MinLength,
                EventTimeValidation_MaxLength = model.EventTimeValidation_MaxLength,
                EventTimeValidation_MinLength = model.EventTimeValidation_MinLength,

                UserTagM_MinNumber = model.UserTagM_MinNumber,
                UserTagM_MaxNumber = model.UserTagM_MaxNumber,
                RegistrationDate = DateTime.UtcNow,
                //IsActive = model.IsActive,
                DistanceShowNearbyAccountsInFeed_Max = model.DistanceShowNearbyAccountsInFeed_Max,
                DistanceShowNearbyAccountsInFeed_Min = model.DistanceShowNearbyAccountsInFeed_Min,
                DistanceShowNearbyEventsOnMap_Max = model.DistanceShowNearbyEventsOnMap_Max,
                DistanceShowNearbyEventsOnMap_Min = model.DistanceShowNearbyEventsOnMap_Min,
                DistanceShowNearbyEvents_Max = model.DistanceShowNearbyEvents_Max,
                DistanceShowNearbyEvents_Min = model.DistanceShowNearbyEvents_Min,

                RecommendedPeopleArea_Min = model.RecommendedPeopleArea_Min,
                RecommendedPeopleArea_Max = model.RecommendedPeopleArea_Max,
                RecommendedEventArea_Min = model.RecommendedEventArea_Min,
                RecommendedEventArea_Max = model.RecommendedEventArea_Max,
                WhiteLableCodeLength = model.WhiteLableCodeLength,
                UserID = httpContextAccessor.HttpContext.GetUser().UserId,
                ID = ID,

            };
            return Obj;
        }
        AppConfigration Converter(AppConfigrationVM model, AppConfigration Obj)
        {
            Obj.Name = model.Name ?? Obj.Name;
            Obj.AgeFiltering_Max = model.AgeFiltering_Max ?? Obj.AgeFiltering_Max;
            Obj.AgeFiltering_Min = model.AgeFiltering_Min ?? Obj.AgeFiltering_Min;
            Obj.DistanceFiltering_Max = model.DistanceFiltering_Max ?? Obj.DistanceFiltering_Max;
            Obj.DistanceFiltering_Min = model.DistanceFiltering_Min ?? Obj.DistanceFiltering_Min;
            Obj.EventDetailsDescription_MaxLength = model.EventDetailsDescription_MaxLength ?? Obj.EventDetailsDescription_MaxLength;
            Obj.EventDetailsDescription_MinLength = model.EventDetailsDescription_MinLength ?? Obj.EventDetailsDescription_MinLength;
            Obj.EventTitle_MaxLength = model.EventTitle_MaxLength ?? Obj.EventTitle_MaxLength;
            Obj.EventTitle_MinLength = model.EventTitle_MinLength ?? Obj.EventTitle_MinLength;
            Obj.Password_MaxNumbers = model.Password_MaxNumbers ?? Obj.Password_MaxNumbers;
            Obj.Password_MinNumbers = model.Password_MinNumbers ?? Obj.Password_MinNumbers;
            Obj.Password_MinSpecialCharacters = model.Password_MinSpecialCharacters ?? Obj.Password_MinSpecialCharacters;
            Obj.Password_MaxSpecialCharacters = model.Password_MaxSpecialCharacters ?? Obj.Password_MaxSpecialCharacters;
            Obj.Password_MaxLength = model.Password_MaxLength ?? Obj.Password_MaxLength;
            Obj.Password_MinLength = model.Password_MinLength ?? Obj.Password_MinLength;
            Obj.UserBio_MaxLength = model.UserBio_MaxLength ?? Obj.UserBio_MaxLength;
            Obj.UserBio_MinLength = model.UserBio_MinLength ?? Obj.UserBio_MinLength;
            Obj.UserMaxAge = model.UserMaxAge ?? Obj.UserMaxAge;
            Obj.UserMinAge = model.UserMinAge ?? Obj.UserMinAge;
            Obj.UserName_MaxLength = model.UserName_MaxLength ?? Obj.UserName_MaxLength;
            Obj.UserName_MinLength = model.UserName_MinLength ?? Obj.UserName_MinLength;
            Obj.UserIPreferTo_MaxLength = model.UserIPreferTo_MaxLength ?? Obj.UserIPreferTo_MaxLength;
            Obj.UserIPreferTo_MinLength = model.UserIPreferTo_MinLength ?? Obj.UserIPreferTo_MinLength;
            Obj.UserIAM_MaxLength = model.UserIAM_MaxLength ?? Obj.UserIAM_MaxLength;
            Obj.UserIAM_MinLength = model.UserIAM_MinLength ?? Obj.UserIAM_MinLength;

            Obj.UserTagM_MaxNumber = model.UserTagM_MaxNumber ?? Obj.UserTagM_MaxNumber;
            Obj.UserTagM_MinNumber = model.UserTagM_MinNumber ?? Obj.UserTagM_MinNumber;
            Obj.RegistrationDate = DateTime.UtcNow;
            Obj.DistanceShowNearbyAccountsInFeed_Max = model.DistanceShowNearbyAccountsInFeed_Max ?? Obj.DistanceShowNearbyAccountsInFeed_Max;
            Obj.DistanceShowNearbyAccountsInFeed_Min = model.DistanceShowNearbyAccountsInFeed_Min ?? Obj.DistanceShowNearbyAccountsInFeed_Min;
            Obj.DistanceShowNearbyEventsOnMap_Max = model.DistanceShowNearbyEventsOnMap_Max ?? Obj.DistanceShowNearbyEventsOnMap_Max;
            Obj.DistanceShowNearbyEventsOnMap_Min = model.DistanceShowNearbyEventsOnMap_Min ?? Obj.DistanceShowNearbyEventsOnMap_Min;
            Obj.DistanceShowNearbyEvents_Max = model.DistanceShowNearbyEvents_Max ?? Obj.DistanceShowNearbyEvents_Max;
            Obj.DistanceShowNearbyEvents_Min = model.DistanceShowNearbyEvents_Min ?? Obj.DistanceShowNearbyEvents_Min;
            Obj.UserID = httpContextAccessor.HttpContext.GetUser().UserId;

            Obj.EventTimeValidation_MaxLength = model.EventTimeValidation_MaxLength ?? Obj.EventTimeValidation_MaxLength;
            Obj.EventTimeValidation_MinLength = model.EventTimeValidation_MinLength ?? Obj.EventTimeValidation_MinLength;
            Obj.EventCreationLimitNumber_MaxLength = model.EventCreationLimitNumber_MaxLength ?? Obj.EventCreationLimitNumber_MaxLength;
            Obj.EventCreationLimitNumber_MinLength = model.EventCreationLimitNumber_MinLength ?? Obj.EventCreationLimitNumber_MinLength;

            Obj.RecommendedPeopleArea_Min = model.RecommendedPeopleArea_Min ?? Obj.RecommendedPeopleArea_Min;
            Obj.RecommendedPeopleArea_Max = model.RecommendedPeopleArea_Max ?? Obj.RecommendedPeopleArea_Max;
            Obj.RecommendedEventArea_Min = model.RecommendedEventArea_Min ?? Obj.RecommendedEventArea_Min;
            Obj.RecommendedEventArea_Max = model.RecommendedEventArea_Max ?? Obj.RecommendedEventArea_Max;
            Obj.WhiteLableCodeLength= model.WhiteLableCodeLength ?? Obj.WhiteLableCodeLength;
            return Obj;
        }
        AppConfigration Converter(AppConfigrationVM_Required model, AppConfigration Obj)
        {
            Obj.Name = model.Name ?? Obj.Name;
            Obj.AgeFiltering_Max = model.AgeFiltering_Max ?? Obj.AgeFiltering_Max;
            Obj.AgeFiltering_Min = model.AgeFiltering_Min ?? Obj.AgeFiltering_Min;
            Obj.DistanceFiltering_Max = model.DistanceFiltering_Max ?? Obj.DistanceFiltering_Max;
            Obj.DistanceFiltering_Min = model.DistanceFiltering_Min ?? Obj.DistanceFiltering_Min;
            Obj.EventDetailsDescription_MaxLength = model.EventDetailsDescription_MaxLength ?? Obj.EventDetailsDescription_MaxLength;
            Obj.EventDetailsDescription_MinLength = model.EventDetailsDescription_MinLength ?? Obj.EventDetailsDescription_MinLength;
            Obj.EventTitle_MaxLength = model.EventTitle_MaxLength ?? Obj.EventTitle_MaxLength;
            Obj.EventTitle_MinLength = model.EventTitle_MinLength ?? Obj.EventTitle_MinLength;
            Obj.Password_MaxNumbers = model.Password_MaxNumbers ?? Obj.Password_MaxNumbers;
            Obj.Password_MinNumbers = model.Password_MinNumbers ?? Obj.Password_MinNumbers;
            Obj.Password_MinSpecialCharacters = model.Password_MinSpecialCharacters ?? Obj.Password_MinSpecialCharacters;
            Obj.Password_MaxSpecialCharacters = model.Password_MaxSpecialCharacters ?? Obj.Password_MaxSpecialCharacters;
            Obj.Password_MaxLength = model.Password_MaxLength ?? Obj.Password_MaxLength;
            Obj.Password_MinLength = model.Password_MinLength ?? Obj.Password_MinLength;
            Obj.UserBio_MaxLength = model.UserBio_MaxLength ?? Obj.UserBio_MaxLength;
            Obj.UserBio_MinLength = model.UserBio_MinLength ?? Obj.UserBio_MinLength;
            Obj.UserMaxAge = model.UserMaxAge ?? Obj.UserMaxAge;
            Obj.UserMinAge = model.UserMinAge ?? Obj.UserMinAge;

            Obj.UserName_MaxLength = model.UserName_MaxLength ?? Obj.UserName_MaxLength;
            Obj.UserName_MinLength = model.UserName_MinLength ?? Obj.UserName_MinLength;

            Obj.UserIPreferTo_MaxLength = model.UserIPreferTo_MaxLength ?? Obj.UserIPreferTo_MaxLength;
            Obj.UserIPreferTo_MinLength = model.UserIPreferTo_MinLength ?? Obj.UserIPreferTo_MinLength;
            Obj.UserIAM_MaxLength = model.UserIAM_MaxLength ?? Obj.UserIAM_MaxLength;
            Obj.UserIAM_MinLength = model.UserIAM_MinLength ?? Obj.UserIAM_MinLength;


            Obj.UserTagM_MaxNumber = model.UserTagM_MaxNumber ?? Obj.UserTagM_MaxNumber;
            Obj.UserTagM_MinNumber = model.UserTagM_MinNumber ?? Obj.UserTagM_MinNumber;
            Obj.RegistrationDate = DateTime.UtcNow;
            Obj.DistanceShowNearbyAccountsInFeed_Max = model.DistanceShowNearbyAccountsInFeed_Max ?? Obj.DistanceShowNearbyAccountsInFeed_Max;
            Obj.DistanceShowNearbyAccountsInFeed_Min = model.DistanceShowNearbyAccountsInFeed_Min ?? Obj.DistanceShowNearbyAccountsInFeed_Min;
            Obj.DistanceShowNearbyEventsOnMap_Max = model.DistanceShowNearbyEventsOnMap_Max ?? Obj.DistanceShowNearbyEventsOnMap_Max;
            Obj.DistanceShowNearbyEventsOnMap_Min = model.DistanceShowNearbyEventsOnMap_Min ?? Obj.DistanceShowNearbyEventsOnMap_Min;
            Obj.DistanceShowNearbyEvents_Max = model.DistanceShowNearbyEvents_Max ?? Obj.DistanceShowNearbyEvents_Max;
            Obj.DistanceShowNearbyEvents_Min = model.DistanceShowNearbyEvents_Min ?? Obj.DistanceShowNearbyEvents_Min;
            Obj.UserID = httpContextAccessor.HttpContext.GetUser().UserId;
            Obj.EventTimeValidation_MaxLength = model.EventTimeValidation_MaxLength ?? Obj.EventTimeValidation_MaxLength;
            Obj.EventTimeValidation_MinLength = model.EventTimeValidation_MinLength ?? Obj.EventTimeValidation_MinLength;
            Obj.EventCreationLimitNumber_MaxLength = model.EventCreationLimitNumber_MaxLength ?? Obj.EventCreationLimitNumber_MaxLength;
            Obj.EventCreationLimitNumber_MinLength = model.EventCreationLimitNumber_MinLength ?? Obj.EventCreationLimitNumber_MinLength;

            Obj.RecommendedPeopleArea_Min = model.RecommendedPeopleArea_Min ?? Obj.RecommendedPeopleArea_Min;
            Obj.RecommendedPeopleArea_Max = model.RecommendedPeopleArea_Max ?? Obj.RecommendedPeopleArea_Max;
            Obj.RecommendedEventArea_Min = model.RecommendedEventArea_Min ?? Obj.RecommendedEventArea_Min;
            Obj.RecommendedEventArea_Max = model.RecommendedEventArea_Max ?? Obj.RecommendedEventArea_Max;
            Obj.WhiteLableCodeLength= model.WhiteLableCodeLength ?? Obj.WhiteLableCodeLength;

            return Obj;
        }
        AppConfigrationVM Converter(AppConfigration model)
        {
            var Obj = new AppConfigrationVM
            {
                ID = model.ID.ToString(),
                Name = model.Name,
                androidFrSecretKey= "FlZMii1ih+JXCAbki0OOAvcggqdS2O0MSWj6B0wS",
                androidFrAccessKey= "AKIA5SBX6UH4UW7E6TGW",
                UserIAM_MaxLength = model.UserIAM_MaxLength,
                UserIAM_MinLength = model.UserIAM_MinLength,
                UserIPreferTo_MaxLength = model.UserIPreferTo_MaxLength,
                UserIPreferTo_MinLength = model.UserIPreferTo_MinLength,
                AgeFiltering_Max = model.AgeFiltering_Max,
                AgeFiltering_Min = model.AgeFiltering_Min,
                DistanceFiltering_Max = model.DistanceFiltering_Max,
                DistanceFiltering_Min = model.DistanceFiltering_Min,
                EventDetailsDescription_MaxLength = model.EventDetailsDescription_MaxLength,
                EventDetailsDescription_MinLength = model.EventDetailsDescription_MinLength,
                EventTitle_MaxLength = model.EventTitle_MaxLength,
                EventTitle_MinLength = model.EventTitle_MinLength,
                Password_MaxNumbers = model.Password_MaxNumbers,
                Password_MinNumbers = model.Password_MinNumbers,
                Password_MinSpecialCharacters = model.Password_MinSpecialCharacters,
                Password_MaxSpecialCharacters = model.Password_MaxSpecialCharacters,
                Password_MaxLength = model.Password_MaxLength,
                Password_MinLength = model.Password_MinLength,
                UserBio_MaxLength = model.UserBio_MaxLength,
                UserBio_MinLength = model.UserBio_MinLength,
                UserMaxAge = model.UserMaxAge,
                UserMinAge = model.UserMinAge,
                UserName_MaxLength = model.UserName_MaxLength,
                UserName_MinLength = model.UserName_MinLength,
                UserTagM_MinNumber = model.UserTagM_MinNumber,
                UserTagM_MaxNumber = model.UserTagM_MaxNumber,
                IsActive = model.IsActive,
                DistanceShowNearbyAccountsInFeed_Max = model.DistanceShowNearbyAccountsInFeed_Max,
                DistanceShowNearbyAccountsInFeed_Min = model.DistanceShowNearbyAccountsInFeed_Min,
                DistanceShowNearbyEventsOnMap_Max = model.DistanceShowNearbyEventsOnMap_Max,
                DistanceShowNearbyEventsOnMap_Min = model.DistanceShowNearbyEventsOnMap_Min,
                DistanceShowNearbyEvents_Max = model.DistanceShowNearbyEvents_Max,
                DistanceShowNearbyEvents_Min = model.DistanceShowNearbyEvents_Min,
                EventCreationLimitNumber_MinLength = model.EventCreationLimitNumber_MinLength,
                EventCreationLimitNumber_MaxLength = model.EventCreationLimitNumber_MaxLength,
                EventTimeValidation_MinLength = model.EventTimeValidation_MinLength,
                EventTimeValidation_MaxLength = model.EventTimeValidation_MaxLength,

                RecommendedPeopleArea_Min = model.RecommendedPeopleArea_Min,
                RecommendedPeopleArea_Max = model.RecommendedPeopleArea_Max,
                RecommendedEventArea_Min = model.RecommendedEventArea_Min,
                RecommendedEventArea_Max = model.RecommendedEventArea_Max,
                WhiteLableCodeLength=model.WhiteLableCodeLength,
            };
            return Obj;
        }


    }

}



