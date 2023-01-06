using System;
using System.Collections.Generic;
using System.Text;

namespace Social.Entity.Models
{
   public class FireBaseDatamodel
    {

        public string id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string imageUrl { get; set; }
        public int? Messagetype { get; set; }
        public int? userid { get; set; }
        public string Action_code { get; set; }
        public bool? read { get; set; }
        public bool muit { get; set; }
        public bool IsCreatedByAdmin { get; set; }
        //public int Action_Id2 { get; set; }
        public string Action { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual UserDetails User { get; set; }
    }
}
