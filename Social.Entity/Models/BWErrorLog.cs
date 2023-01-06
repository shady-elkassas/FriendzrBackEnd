
using System;
using System.ComponentModel.DataAnnotations;

namespace Social.Entity.Models
{
    public class BWErrorLog
    {
        [Key]
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Api { get; set; }
        public string Method { get; set; }
        public string ApiParams { get; set; }
        public string Exception { get; set; }
        public string ExMsg { get; set; }
        public string ExStackTrace { get; set; }
        public string InnerException { get; set; }
        public string InnerExMsg { get; set; }
        public string InnerExStackTrace { get; set; }
        public DateTime CreatedOn { get; set; }

        public BWErrorLog()
        { }
        public BWErrorLog(string userId , string api ,Exception ex)
        {
            Id = Guid.NewGuid().ToString();
            UserId = userId;
            Api = api;
            Exception = ex.ToString();
            ExMsg = ex.Message;
            ExStackTrace = ex.StackTrace;
            InnerException = ex.InnerException?.ToString();
            InnerExMsg = ex.InnerException?.Message;
            InnerExStackTrace = ex.InnerException?.StackTrace;
            CreatedOn = DateTime.Now;
        }
    }
}
