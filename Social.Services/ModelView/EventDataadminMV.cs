using Microsoft.AspNetCore.Http;
using Social.Services.Implementation;
using Social.Services.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace Social.Services.ModelView
{
    public class EventDataadminMV : IValidatableObject
    {

        public int Id { get; set; }
        public string EntityId { get; set; } = Guid.NewGuid().ToString();
       
        public string UseraddedId { get; set; }
        [Required(ErrorMessage = "category is required")]
        [Display(Name = "Category")]

        public int categorieId { get; set; }
        [Required(ErrorMessage = "Title is required")]
        [Display(Name = "EventTilte")]

        public string Title { get; set; }
        [Display(Name ="EventType")]
        [Required(ErrorMessage = "EventType is required")]

        public int? EventTypeListid { get; set; }
        [Display(Name = "EventType")]

        public string EventTypeListName { get; set; }
        [Display(Name = "IsActive")]
        public bool IsActive { get; set; } = true;
        [Required(ErrorMessage = "description is required")]
        public string description { get; set; }
        public string checkout_details { get; set; }
        public string status { get; set; }
        [Required(ErrorMessage = "Image is required")]
        public IFormFile ImageFile { get; set; }
        public string Image { get; set; }

        [Display(Name = "CreatedDate")]
        public string CreatedDate { get; set; }
        [Display(Name = "EventCategory")]

        public string categoryName { get; set; }
        public int joined { get; set; }
        public int key { get; set; }
        public string categorieimage { get; set; }
        [Required(ErrorMessage = "location is required")]

        public string lang { get; set; }
        public string lat { get; set; }
        [Required(ErrorMessage = "total numbert is required")]
        public int totalnumbert { get; set; }
        public bool allday { get; set; } =false;
        //[Required(ErrorMessage = "event date is required")]
        public DateTime eventdate { get; set; } = DateTime.Now;
        [JsonExtensionData]
        public List<DateTime> eventdateList { get; set; }
        //[Required(ErrorMessage = "event date is required")]
        public DateTime eventdateto { get; set; } = DateTime.Now;
        [JsonExtensionData]
        public List<DateTime> eventdatetoList { get; set; }
        public TimeSpan? eventfrom { get; set; } = DateTime.Now.TimeOfDay;
        public TimeSpan? eventto { get; set; } = DateTime.Now.TimeOfDay;
        public string color { get; set; }
        public string timetext { get; set; }
        public string datetext { get; set; }
        public bool MyEvent { get; set; }
        public int userid { get; set; }
        public int EventAttendCount { get; set; } = 0;
        public int Attendees { get; set; } = 0;
        public int Views { get; set; } = 0;
        public int Shars { get; set; } = 0;
        public string SharedLink { get; set; }
        [Display(Name = "Total Attendees")]
        public  string TotalAttendees { get => string.Format("{0}/{1}", Attendees, totalnumbert); }
        [Display(Name = "Messages number")]
        public int MessageChatEventCount { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var repo = (IEventServ)validationContext.GetService(typeof(IEventServ));
            var validation = repo._ValidationResult(this);
            return validation;
        }
    }
    public class eventjson
    {
        public string checkout_details { get; set; }
        public string eventdate { get; set; }
        public string eventdateto { get; set; }
        public bool allday { get; set; }
        public string description { get; set; }
        public TimeSpan timefrom { get; set; }
        public TimeSpan timeto { get; set; }

        public string title { get; set; }
        public string image{ get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }

        
    }
}
