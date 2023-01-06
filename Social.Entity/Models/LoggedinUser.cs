using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Social.Entity.Models
{
    public class LoggedinUser
    {
        [Key]
        public int PrimaryId { get; set; }

        public string Id { get; set; }
        public string Token { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ExpiredOn { get; set; }
        public string UserId { get; set; }
        public int? ProjectId { get; set; }
        public int? PlatformId { get; set; }

        public virtual User User { get; set; }
       
    }
}
