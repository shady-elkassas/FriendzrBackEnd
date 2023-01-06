using System;
using System.Collections.Generic;
using System.Text;

namespace Social.Services.ModelView
{
  public  class UserReportVM
    {
        public string ID { get; set; }

        public string UserID { get; set; }
        public string CreatedBy_UserID { get; set; }
        public string Message { get; set; }
      
        public string ReportReasonID { get; set; }

        public DateTime? RegistrationDate { get; set; } = DateTime.UtcNow;
        public string CreatedBy_UserName { get; set; }
        public string CreatedBy_Email { get; set; }
        public string CreatedBy_ImageUrl { get; set; }
        public string ReportedUserEmail { get; set; }
        public string ReportedUserName { get; set; }
        public string ReportReasonName { get; set; }
        public string ReportedUserImageUrl { get; set; }
        public string ReportedUserID{ get; set; }

        //public string RegistrationDateStr { get { return RegistrationDate?.ToString("dd MMM yyyy") ?? ""; } }
        public string RegistrationDateStr { get { return RegistrationDate?.ToString("dd MMM yyyy ,hh:mm tt") ?? ""; } }

    }
}
