using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Social.Entity.Models
{
  public  class EventTypeList
    {
        public EventTypeList()
        {
            EventData = new HashSet<EventData>();
        }
        public int ID { get; set; }
        public Guid entityID { get; set; }
        public string Name { get; set; }
        public string color { get; set; }
        public bool key { get; set; }
        //public bool IsActive { get; set; }
        public virtual ICollection<EventData> EventData { get; set; }
    }
}
