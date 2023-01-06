using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Social.Entity.Models
{
public   class UserReport
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ID { get; set; }
        [ForeignKey("ReportedUser")]
        public string    UserID { get; set; }
        [ForeignKey("CreatedBy_User")]
        public string    CreatedBy_UserID { get; set; }
        public string    Message { get; set; }
        [ForeignKey("ReportReason")]
        public Guid ReportReasonID { get; set; }
       
        public DateTime? RegistrationDate { get; set; }
        public virtual User ReportedUser { get; set; }
        
        public virtual User CreatedBy_User { get; set; }

        public virtual ReportReason ReportReason { get; set; }

    }
}
