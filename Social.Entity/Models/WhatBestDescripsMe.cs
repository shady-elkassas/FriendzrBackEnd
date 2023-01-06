using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Social.Entity.Models
{
   public class WhatBestDescripsMe
    {
        public WhatBestDescripsMe()
        {
            WhatBestDescripsMeList = new HashSet<WhatBestDescripsMeList>();
           
        }
        public int Id { get; set; }

        public string EntityId { get; set; }
        public string name { get; set; }
        public bool IsSharedForAllUsers { get; set; }
        [ForeignKey("User")]
        public string CreatedByUserID { get; set; }
        public DateTime RegistrationDate { get; set; }
        public bool IsActive { get; set; }
        public virtual ICollection<WhatBestDescripsMeList> WhatBestDescripsMeList { get; set; }
        public virtual User User { get; set; }
    }
}
