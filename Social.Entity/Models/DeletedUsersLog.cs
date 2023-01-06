using System;
using System.Collections.Generic;
using System.Text;

namespace Social.Entity.Models
{
  public  class DeletedUsersLog
    {
        public int ID { get; set; }
        public DateTime DateTime { get; set; }
        public string UserDetailsJson { get; set; }
        public string longitude { get; set; }
        public string latitude { get; set; }
        public string Gender { get; set; }
    }
}
