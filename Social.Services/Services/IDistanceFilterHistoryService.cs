using Social.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Services.Services
{
  public  interface IDistanceFilterHistoryService
    {
        Task Create(User CurrentUser,decimal distance);
        IQueryable<DistanceFilterHistory> GetAll(int Year);
        IQueryable<UserDetails> manialdistancav(int Year);


    }
}
