using Social.Entity.ModelView;
using Social.Services.ModelView;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;

namespace Social.Services.Services
{
    public interface ICountryService
    {

        Task<CountryVM> GetData(int ID);
        IEnumerable<CountryVM> GetData();
        Task<CommonResponse<CountryVM>> Create(CountryVM VM);
        Task<CommonResponse<CountryVM>> Edit(CountryVM VM);
        Task<CommonResponse<CountryVM>> Remove(int ID);
        IEnumerable<ValidationResult> _ValidationResult(CountryVM VM);
    }
}
