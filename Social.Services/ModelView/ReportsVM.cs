using System;
using System.Collections.Generic;
using System.Text;

namespace Social.Services.ModelView
{
public    class ReportsVM
    {
        public string ID { get; set; }
        public ReportType ReportType { get; set; }
        public Guid ReportReasonID { get; set; }
        public string Message { get; set; }
    }

    public enum ReportType
    {
        ChatGroup=1,
        Event=2,
        User=3
    }
}
