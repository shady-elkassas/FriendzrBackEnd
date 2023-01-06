using Social.Entity.ModelView;
using Social.Services.ModelView;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;

namespace Social.Services.Services
{
    public interface IEventTypeListService
    {

        Task<EventTypeListVM> GetData(Guid EntityID);
        IEnumerable<EventTypeListVM> GetData();
        bool IsPrivteKey(Guid ID);
        Task<CommonResponse<EventTypeListVM>> Create(EventTypeListVM VM);
        Task<CommonResponse<EventTypeListVM>> Edit(EventTypeListVM VM);
        Task<CommonResponse<EventTypeListVM>> Remove(Guid EntityID);
        IEnumerable<ValidationResult> _ValidationResult(EventTypeListVM VM);
        Task<CommonResponse<EventTypeListVM>> ToggleActiveConfigration(Guid ID, bool IsActive);
        Task<int> GetEventTypeListId(string name);
    }
}
