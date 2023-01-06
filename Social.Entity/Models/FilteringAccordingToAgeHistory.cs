using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Social.Entity.Models
{
    public class FilteringAccordingToAgeHistory
    {
        public int ID { get; set; }
        [ForeignKey("User")]
        public string UserID { get; set; }
        public DateTime RegistrationDate { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int Day { get; set; }
        public int AgeFrom { get; set; }
        public int AgeTo { get; set; }
        public virtual User User { get; set; }
    }
}
