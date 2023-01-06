using Social.Entity.ModelView;
using Social.Services.ModelView;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;

namespace Social.Services.Services
{

    public interface IInterestsService
    {
        InterestsVM GetData(string ID);
        IEnumerable<InterestsVM> GetData();
        Task<CommonResponse<InterestsVM>> Create(InterestsVM VM);
        Task<CommonResponse<InterestsVM>> Edit(InterestsVM VM);
        Task<CommonResponse<InterestsVM>> Remove(string ID);
        Task<CommonResponse<InterestsVM>> ToggleActiveConfigration(string ID, bool IsActive);

    }
}
