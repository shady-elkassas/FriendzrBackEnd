using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Social.Entity.DBContext;
using Social.Entity.Models;
using Social.Entity.ModelView;
using Social.Services.Helpers;
using Social.Services.ModelView;
using Social.Services.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Services.ModelView
{
    public class EventReportVM
    {
        public string ID { get; set; }
        public int EventDataID { get; set; }
        public string EventDataEntityID { get; set; }
        public string CreatedBy_UserID { get; set; }
        public string Message { get; set; }
        public string ReportReasonID { get; set; }
        public DateTime? RegistrationDate { get; set; } = DateTime.UtcNow;
        public string CreatedBy_UserName { get; set; }
        public string ReportReasonName { get; set; }
        
        public string EventTitle { get; set; }
        public string Eventdescription { get; set; }
        public string EventImageUrl { get; set; }
        //public string RegistrationDateStr { get { return RegistrationDate?.ToString("dd MMM yyyy")??""; } }
        public string RegistrationDateStr { get { return RegistrationDate?.ToString("dd MMM yyyy ,hh:mm tt") ?? ""; } }
    }

}



