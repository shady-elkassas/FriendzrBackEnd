using Social.Entity.ModelView;
using Social.Services.ModelView;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;

namespace Social.Services.Services
{

    public interface IEventReportService
    {
        Task<EventReportVM> GetData(Guid ID);
        IEnumerable<EventReportVM> GetData();
        IEnumerable<EventReportVM> GetData(string EventOD);
        Task<CommonResponse<EventReportVM>> Create(EventReportVM VM);
        Task<CommonResponse<EventReportVM>> Edit(EventReportVM VM);
        Task<CommonResponse<EventReportVM>> Remove(Guid ID);
        IEnumerable<EventReportVM> GetData(List<int> eventIds);


    }
}
