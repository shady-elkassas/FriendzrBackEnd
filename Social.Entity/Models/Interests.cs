using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Social.Entity.Models
{
    public class Interests
    {
        public Interests()
        {
            listoftags = new HashSet<listoftags>();
            //eventattend = new HashSet<eventattend>();
            //RegistrationDate = DateTime.UtcNow;
            //IsSharedForAllUsers = true;
            //IsAcive = true;
        }
        public int Id { get; set; }
     
        public string EntityId { get; set; }
        public string name { get; set; }
        public bool IsSharedForAllUsers { get; set; }
        [ForeignKey("User")]
        public string    CreatedByUserID { get; set; }
        public DateTime   RegistrationDate { get; set; }

        public bool IsActive { get; set; }

        //public virtual ICollection<eventattend> eventattend { get; set; }
        public virtual ICollection<listoftags> listoftags { get; set; }
        public virtual User User { get; set; }
    }
}
