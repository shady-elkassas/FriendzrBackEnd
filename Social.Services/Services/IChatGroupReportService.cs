using Social.Entity.ModelView;
using Social.Services.ModelView;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;

namespace Social.Services.Services
{

    public interface IChatGroupReportService
    {
        Task<ChatGroupReportVM> GetData(Guid ID);
        IEnumerable<ChatGroupReportVM> GetData();
        Task<CommonResponse<ChatGroupReportVM>> Create(ChatGroupReportVM VM);
        Task<CommonResponse<ChatGroupReportVM>> Edit(ChatGroupReportVM VM);
        Task<CommonResponse<ChatGroupReportVM>> Remove(Guid ID);

    }
}
