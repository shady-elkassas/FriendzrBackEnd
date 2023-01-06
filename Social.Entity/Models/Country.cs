using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Social.Entity.Models
{
    public class Country
    {
        public Country()
        {
            UserDetails = new HashSet<UserDetails>();
            EventDatas = new HashSet<EventData>();
        }
        public int ID { get; set; }
        [Required]
        //[StringLength(200)]
        public string GoogleName { get; set; }
        public string DisplayName { get; set; }
        public virtual ICollection<UserDetails> UserDetails { get; set; }
        public virtual ICollection<EventData> EventDatas { get; set; }
    }
}
