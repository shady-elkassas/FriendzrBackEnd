using System;
using System.Collections.Generic;
using System.Text;

namespace Social.Entity.Models
{
   public class category
    {
        public category()
        {
            
            EventData = new HashSet<EventData>();
            EventCategoryTrackers = new HashSet<EventCategoryTracker>();
        }
        public int Id { get; set; }
        public string EntityId { get; set; }
        public string name { get; set; }
        public string image { get; set; }
       
        public DateTime RegistrationDate { get; set; }
        public bool IsActive { get; set; }
        public virtual ICollection<EventData> EventData { get; set; }
        public virtual ICollection<EventCategoryTracker> EventCategoryTrackers { get; set; }

    }
}
