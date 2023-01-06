using Social.Entity.ModelView;
using Social.Services.ModelView;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;

namespace Social.Services.Services
{
    public interface ICityService
    {

        Task<CityVM> GetData(int ID);
        IEnumerable<CityVM> GetData();
        Task<CommonResponse<CityVM>> Create(CityVM VM);
        Task<CommonResponse<CityVM>> Edit(CityVM VM);
        Task<CommonResponse<CityVM>> Remove(int ID);
        IEnumerable<ValidationResult> _ValidationResult(CityVM VM);
    }
}
