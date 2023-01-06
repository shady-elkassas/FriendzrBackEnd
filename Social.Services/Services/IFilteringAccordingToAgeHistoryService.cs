using Social.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Services.Services
{
    public interface IFilteringAccordingToAgeHistoryService
    {
        Task Create(User CurrentUser,int AgeFrom,int AgeTo);
        IQueryable<FilteringAccordingToAgeHistory> GetAll(int Year);

    }
}
