using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Social.Entity.Models
{
   public class ReportReason
    {
        public ReportReason()
        {
            EventReports = new List<EventReport>();
            UserReports = new List<UserReport>();
            ChatGroupReports = new List<ChatGroupReport>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ID { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public int? DisplayOrder { get; set; }
        public DateTime RegistrationDate { get; set; }
        public virtual ICollection<UserReport> UserReports { get; set; }
        public virtual ICollection<EventReport> EventReports { get; set; }
        public virtual ICollection<ChatGroupReport> ChatGroupReports { get; set; }
    
    }
}
