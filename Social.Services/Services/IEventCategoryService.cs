using Social.Entity.ModelView;
using Social.Services.ModelView;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;

namespace Social.Services.Services
{

    public interface IEventCategoryService
    {

        EventCategoryVM GetData(string ID);
        IEnumerable<EventCategoryVM> GetData();
        Task<CommonResponse<EventCategoryVM>> Create(EventCategoryVM VM);
        Task<CommonResponse<EventCategoryVM>> Edit(EventCategoryVM VM);
        Task<CommonResponse<EventCategoryVM>> Remove(string ID);
        Task<CommonResponse<EventCategoryVM>> ToggleActiveConfigration(string ID, bool IsActive);
        IEnumerable<ValidationResult> _ValidationResult(EventCategoryVM VM);


    }
}
