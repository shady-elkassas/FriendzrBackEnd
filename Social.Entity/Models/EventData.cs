using System;
using System.Collections.Generic;
using System.Text;

namespace Social.Entity.Models
{
  public  class EventData
    {
        public EventData()
        {
            //eventattend = new HashSet<eventattend>();
            //EventChat= new HashSet<EventChat>();
            EventChatAttend = new HashSet<EventChatAttend>();
            EventReports = new HashSet<EventReport>();
            Messagedata =  new HashSet<Messagedata>();
            EventTrackers = new HashSet<EventTracker>();
            SkippedEvents = new HashSet<SkippedEvent>();
        }
        public int Id { get; set; }
        public string EntityId { get; set; }
        public int? UserId { get; set; }
        public int? categorieId { get; set; }
        public string SubCategoriesIds { get; set; }
        public string Title { get; set; }
        public string description { get; set; }
        public string status { get; set; }
        public string image { get; set; }
        //public string category { get; set; }
        public string lang { get; set; }
        public string checkout_details { get; set; }
        public Guid? eventtype { get; set; }
        public string lat { get; set; }
        public int totalnumbert { get; set; }
       public int? EventTypeListid { get; set; }
        public bool? allday { get; set; }
        public bool? IsActive { get; set; }
        public bool? showAttendees { get; set; }
        public bool? IsForWhiteLableOnly { get; set; }
        public DateTime? StopFrom { get; set; }
        public DateTime? StopTo { get; set; }
        public DateTime? eventdate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? eventdateto { get; set; }
        public TimeSpan? eventfrom { get; set; }
        public TimeSpan? eventto { get; set; }
        public int? CityID { get; set; }
        public int? CountryID { get; set; }
        public int? Views { get; set; }
        public int? Shars { get; set; }
        public int? Attendees { get; set; }
        public virtual City City { get; set; }
        public virtual Country Country { get; set; }
        public virtual UserDetails User { get; set; }
        public virtual category categorie { get; set; }
        public virtual ICollection<Messagedata> Messagedata { get; set; }
        public virtual ICollection<EventChatAttend> EventChatAttend { get; set; }
        public virtual ICollection<EventReport> EventReports { get; set; }
        public virtual EventTypeList EventTypeList { get; set; }
        public virtual ICollection<EventTracker> EventTrackers { get; set; }
        public virtual ICollection<SkippedEvent> SkippedEvents { get; set; }
    }
}
