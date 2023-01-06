using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Social.Entity.Models
{
 public   class ChatGroupReport
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ID { get; set; }
        [ForeignKey("ChatGroup")]
        public Guid ChatGroupID { get; set; }
        [ForeignKey("User")]

        public string CreatedBy_UserID { get; set; }
        public string Message { get; set; }
        [ForeignKey("ReportReason")]
        public Guid ReportReasonID { get; set; }

        public DateTime? RegistrationDate { get; set; }
        public virtual ChatGroup ChatGroup { get; set; }
        public virtual ReportReason ReportReason { get; set; }
        public virtual User User { get; set; }
    }
}
