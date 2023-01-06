using Social.Services.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Social.Services.ModelView
{
  public  class CountryVM:IValidatableObject
    {
        public int ID { get; set; }
        [Display(Name = "GoogleName")]
        [Required(ErrorMessage = "Required")]
        public string GoogleName { get; set; }
        [Display(Name = "DisplayName")]
        [Required(ErrorMessage = "Required")]
        public string DisplayName { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var repo = (ICountryService)validationContext.GetService(typeof(ICountryService));
            var validation = repo._ValidationResult(this);
            return validation;
        }
    }
}
