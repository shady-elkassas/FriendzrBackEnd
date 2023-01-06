using Social.Services.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Social.Services.ModelView
{
  public  class EventCategoryVM : IValidatableObject
    {
        public int Id { get; set; }
        public string EntityId { get; set; } = Guid.NewGuid().ToString();
        [Display(Name = "Name")]

        public string name { get; set; }
        public string CreatedByUserID { get; set; }
        public DateTime RegistrationDate { get; set; }
        [Display(Name = "IsActive")]
        public bool IsActive { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var repo = (IEventCategoryService)validationContext.GetService(typeof(IEventCategoryService));
            var validation = repo._ValidationResult(this);
            return validation;
        }
    }
}
