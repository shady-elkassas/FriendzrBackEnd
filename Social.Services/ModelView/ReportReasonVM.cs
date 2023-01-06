using Social.Services.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Social.Services.ModelView
{
  public  class ReportReasonVM : IValidatableObject
    {
        public string ID { get; set; }
        [Display(Name = "ReasonName")]
        [Required(ErrorMessage = "Required")]
        public string Name { get; set; }
        [Display(Name = "IsActive")]
        public bool IsActive { get; set; } = true;
        [Display(Name = "DisplayOrder")]
        [Range(1, int.MaxValue, ErrorMessage = "InvalidNumber")]
        [RegularExpression(@"[0-9]*$", ErrorMessage = "InvalidNumber")]
        public int? DisplayOrder { get; set; } 
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var repo = (IReportReasonService)validationContext.GetService(typeof(IReportReasonService));
            var validation = repo._ValidationResult(this);
            return validation;
        }
    }
}
