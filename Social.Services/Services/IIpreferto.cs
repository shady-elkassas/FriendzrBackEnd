using Social.Entity.ModelView;
using Social.Services.ModelView;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Social.Services.Services
{
  public  interface IIpreferto
    {
        prefertoVM GetData(string ID);
        IEnumerable<prefertoVM> GetData();
        Task<CommonResponse<prefertoVM>> Create(prefertoVM VM);
        Task<CommonResponse<prefertoVM>> Edit(prefertoVM VM);
        Task<CommonResponse<prefertoVM>> Remove(string ID);
        Task<CommonResponse<prefertoVM>> ToggleActiveConfigration(string ID, bool IsActive);

    }
}
