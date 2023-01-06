using Social.Entity.ModelView;
using Social.Services.ModelView;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;

namespace Social.Services.Services
{
    public interface IReportReasonService
    {
        Task<ReportReasonVM> GetData(Guid ID);
        IEnumerable<ReportReasonVM> GetData();
        Task<CommonResponse<ReportReasonVM>> Create(ReportReasonVM VM);
        Task<CommonResponse<ReportReasonVM>> Edit(ReportReasonVM VM);
        Task<CommonResponse<ReportReasonVM>> Remove(Guid ID);
        IEnumerable<ValidationResult> _ValidationResult(ReportReasonVM VM);
        Task<CommonResponse<ReportReasonVM>> ToggleActiveConfigration(Guid ID, bool IsActive);
    }
}
