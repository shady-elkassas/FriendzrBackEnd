using Social.Entity.DBContext;
using Social.Entity.Models;
using Social.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Social.Services.Implementation
{
    public class FilteringAccordingToAgeHistoryService : IFilteringAccordingToAgeHistoryService
    {
        private readonly AuthDBContext authDBContext;
        private readonly IGlobalMethodsService globalMethodsService;

        public FilteringAccordingToAgeHistoryService(AuthDBContext authDBContext, IGlobalMethodsService globalMethodsService)
        {
            this.authDBContext = authDBContext;
            this.globalMethodsService = globalMethodsService;
        }
        public async Task Create(User CurrentUser,int AgeFrom,int AgeTo)
        {
            try
            {
                await authDBContext.FilteringAccordingToAgeHistory.AddAsync(new FilteringAccordingToAgeHistory { AgeFrom=AgeFrom,AgeTo=AgeTo,UserID = CurrentUser.Id, Month = DateTime.Now.Month, Year = DateTime.Now.Year, Day = DateTime.Now.Day, RegistrationDate = DateTime.Now });
                await authDBContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {

            }
        }
        public IQueryable<FilteringAccordingToAgeHistory> GetAll(int Year)
        {
            //var StartDate = new DateTime(Year, 1, 1);
            //var EndDate = StartDate.AddYears(1);
            //var DbF = Microsoft.EntityFrameworkCore.EF.Functions;
            //var result = authDBContext.FilteringAccordingToAgeHistory.Where(x => DbF.DateDiffDay(StartDate, x.RegistrationDate) > 0 && DbF.DateDiffDay(x.RegistrationDate, EndDate) > 0);
            var result = authDBContext.FilteringAccordingToAgeHistory.Where(x => x.Year==Year);
            return result;
        }
    }
}
