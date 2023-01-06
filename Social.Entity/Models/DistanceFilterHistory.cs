using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Social.Entity.Models
{
  public  class DistanceFilterHistory
    {
        public int ID { get; set; }
        [ForeignKey("User")]
        public string UserID { get; set; }
        public DateTime RegistrationDate { get; set; }
        public decimal destance { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int Day { get; set; }
        public virtual User User { get; set; }
    }
}
