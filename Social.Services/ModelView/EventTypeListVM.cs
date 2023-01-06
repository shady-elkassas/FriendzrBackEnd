using Social.Services.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Social.Services.ModelView
{
   public class EventTypeListVM:IValidatableObject
    {
        public int ID { get; set; }
        public Guid EntityId { get; set; } = Guid.NewGuid();
        [Display(Name = "Name")]
        [Required(ErrorMessage = "Required")]
        public string Name { get; set; }
        [Display(Name = "Color")]
        [Required(ErrorMessage = "Required")]
        public string color { get; set; }
        [Display(Name = "Privtekey")]

        public bool Privtekey { get; set; }
        //public bool IsActive { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var repo = (IEventTypeListService)validationContext.GetService(typeof(IEventTypeListService));
            var validation = repo._ValidationResult(this);
            return validation;
        }

    }
}
