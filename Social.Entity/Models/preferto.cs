using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Social.Entity.Models
{
   public class preferto
    {
        public preferto()
        {
            Iprefertolist = new HashSet<Iprefertolist>();

        }
        public int Id { get; set; }

        public string EntityId { get; set; }
        public string name { get; set; }
      
        [ForeignKey("User")]
        public string CreatedByUserID { get; set; }
        public DateTime RegistrationDate { get; set; }

        public bool IsActive { get; set; }

        public virtual ICollection<Iprefertolist> Iprefertolist { get; set; }
        public virtual User User { get; set; }
    }
}
