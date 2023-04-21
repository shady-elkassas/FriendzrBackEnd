using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Social.Entity.Models
{
 public   class EventReport
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ID { get; set; }
        [ForeignKey("EventData")]
        public int EventDataID { get; set; }
        //public int TicketMasterEventDataID { get; set; }
        [ForeignKey("User")]

        public string CreatedBy_UserID { get; set; }
        public string Message { get; set; }
        [ForeignKey("ReportReason")]
        public Guid ReportReasonID { get; set; }
     
        public DateTime? RegistrationDate { get; set; }
        public virtual EventData EventData { get; set; }
        //public virtual EventDataTicketMaster EventDataTicketMaster { get; set; }
        public virtual ReportReason ReportReason { get; set; }
        public virtual User User { get; set; }
    }
}
