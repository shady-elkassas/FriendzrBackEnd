using Social.Entity.ModelView;
using Social.Services.ModelView;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;

namespace Social.Services.Services
{
  public  interface IAppConfigrationService
    {


        Task<AppConfigrationVM> GetData(Guid ID);
 
       IEnumerable<AppConfigrationVM> GetData();
        //Task<CommonResponse<AppConfigrationVM>> Create(AppConfigrationVM VM);
        Task<CommonResponse<AppConfigrationVM>> Create(AppConfigrationVM_Required VM);
       
        Task<CommonResponse<AppConfigrationVM>> Edit(AppConfigrationVM_Required VM);
        Task<CommonResponse<AppConfigrationVM>> Edit(AppConfigrationVM VM);
        Task<CommonResponse<AppConfigrationVM>> Remove(Guid ID);
        IEnumerable<ValidationResult> _ValidationResult(AppConfigrationVM VM);
        IEnumerable<ValidationResult> _ValidationResult(AppConfigrationVM_Required VM);
        Task<CommonResponse<AppConfigrationVM>> ToggleActiveConfigration(Guid ID, bool IsActive);
    }
}
