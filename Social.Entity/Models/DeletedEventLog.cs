using System;
using System.Collections.Generic;
using System.Text;

namespace Social.Entity.Models
{
  public  class DeletedEventLog
    {
        public int ID { get; set; }
        public DateTime DateTime { get; set; }
        public string EventDataJson { get; set; }
        public string longitude { get; set; }
        public string latitude { get; set; }
        public int EventCategoryID { get; set; }
    }
}
