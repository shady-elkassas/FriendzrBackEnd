using Social.Entity.ModelView;
using Social.Services.ModelView;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Social.Services.Services
{
    public interface IWhiteLableUserService
    {
        Task<CommonResponse<Entity.Models.User>> CreatetWhiteLableUser(AddEditWhiteLableUserViewModel whiteLableUser);
        Task<AddEditWhiteLableUserViewModel> ViewWhiteLableUserDetails(string whiteLableUserId);
        Task<CommonResponse<bool>> EditWhiteLableUser(AddEditWhiteLableUserViewModel editWhiteLableUser);
        Task<CommonResponse<bool>> ToggleSuspendWhiteLableUser(string whiteLableUserId);
        Task<IQueryable<WhiteLableUserViewModel>> GetWhiteLableUsers();
        Task<CommonResponse<bool>> DeleteWhiteLableUser(string whiteLableUserId);

    }
}
