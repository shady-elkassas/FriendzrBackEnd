using Social.Entity.ModelView;
using Social.Services.ModelView;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Social.Services.Services
{
  public  interface IWhatBestDescripsMe
    {
        WhatBestDescripsMeVM GetData(string ID);
        IEnumerable<WhatBestDescripsMeVM> GetData();
        Task<CommonResponse<WhatBestDescripsMeVM>> Create(WhatBestDescripsMeVM VM);
        Task<CommonResponse<WhatBestDescripsMeVM>> Edit(WhatBestDescripsMeVM VM);
        Task<CommonResponse<WhatBestDescripsMeVM>> Remove(string ID);
        Task<CommonResponse<WhatBestDescripsMeVM>> ToggleActiveConfigration(string ID, bool IsActive);

    }
}
