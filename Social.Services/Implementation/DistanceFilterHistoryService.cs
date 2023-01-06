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
    public class DistanceFilterHistoryService : IDistanceFilterHistoryService
    {
        private readonly AuthDBContext authDBContext;
        private readonly IGlobalMethodsService globalMethodsService;

        public DistanceFilterHistoryService(AuthDBContext authDBContext, IGlobalMethodsService globalMethodsService)
        {
            this.authDBContext = authDBContext;
            this.globalMethodsService = globalMethodsService;
        }
        public async Task Create(User CurrentUser, decimal distance)
        {
            try
            {

                await authDBContext.DistanceFilterHistory.AddAsync(new DistanceFilterHistory {destance=distance, UserID = CurrentUser.Id,Month=DateTime.Now.Month,Year=DateTime.Now.Year,Day=DateTime.Now.Day, RegistrationDate = DateTime.Now });
                await authDBContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {

            }
        }

        public IQueryable<DistanceFilterHistory> GetAll(int Year)
        {
            //var StartDate = new DateTime(Year, 1, 1);
            //var EndDate = StartDate.AddYears(1);
            //var DbF = Microsoft.EntityFrameworkCore.EF.Functions;
            //var result = authDBContext.DistanceFilterHistory.Where(x => DbF.DateDiffDay(StartDate, x.RegistrationDate) > 0 && DbF.DateDiffDay(x.RegistrationDate, EndDate) > 0);
            var result = authDBContext.DistanceFilterHistory.Where(x => x.Year == Year);

            return result;
        }
        public IQueryable<UserDetails> manialdistancav(int Year)
        {
            //var StartDate = new DateTime(Year, 1, 1);
            //var EndDate = StartDate.AddYears(1);
            //var DbF = Microsoft.EntityFrameworkCore.EF.Functions;
            //var result = authDBContext.DistanceFilterHistory.Where(x => DbF.DateDiffDay(StartDate, x.RegistrationDate) > 0 && DbF.DateDiffDay(x.RegistrationDate, EndDate) > 0);
            var result = authDBContext.UserDetails.Where(x => x.User.RegistrationDate.Year == Year);
            
            //var a = result.Select(m => m.Manualdistancecontrol).ToList();
            return result;
        }
    }
}
