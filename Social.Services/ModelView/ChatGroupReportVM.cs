using System;
using System.Collections.Generic;
using System.Text;

namespace Social.Services.ModelView
{
 public   class ChatGroupReportVM
    {
        public string ID { get; set; }
        public Guid ChatGroupID { get; set; }
        public string CreatedBy_UserID { get; set; }
        public string Message { get; set; }
        public string ReportReasonID { get; set; }
        public DateTime? RegistrationDate { get; set; } = DateTime.UtcNow;
        public string CreatedBy_UserName { get; set; }
        public string ReportReasonName { get; set; }

        public string ChatGroupName { get; set; }
        public string ChatGroupImageUrl { get; set; }
        //public string RegistrationDateStr { get { return RegistrationDate?.ToString("dd MMM yyyy")??""; } }
        public string RegistrationDateStr { get { return RegistrationDate?.ToString("dd MMM yyyy ,hh:mm tt") ?? ""; } }
    }
}
