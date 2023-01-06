using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Social.Entity.Models
{
 public   class User_FCMToken
    {
        public int ID { get; set; }
        [ForeignKey("User")]
        [Required]

        public string UserID { get; set; }
        public string Token { get; set; }

        public virtual User User { get; set; }
    }
}
