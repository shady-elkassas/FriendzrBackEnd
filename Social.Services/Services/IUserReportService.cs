using Social.Entity.ModelView;
using Social.Services.ModelView;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;

namespace Social.Services.Services
{
    public interface IUserReportService
    {
        Task<UserReportVM> GetData(Guid ID);
        IEnumerable<UserReportVM> GetData();
        IEnumerable<UserReportVM> GetAllReportsInUser(string UserID);
        Task<CommonResponse<UserReportVM>> Create(UserReportVM VM);
        Task<CommonResponse<UserReportVM>> Edit(UserReportVM VM);
        Task<CommonResponse<UserReportVM>> Remove(Guid ID);
    }
}
