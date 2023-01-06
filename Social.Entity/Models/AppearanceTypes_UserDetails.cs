using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Social.Entity.Models
{
  public  class AppearanceTypes_UserDetails
    {
        public int ID { get; set; }
        public int AppearanceTypeID { get; set; }
        [ForeignKey("UserDetails")]
        public int UserDetailsID { get; set; }
        public virtual UserDetails UserDetails { get; set; }
    }
}
