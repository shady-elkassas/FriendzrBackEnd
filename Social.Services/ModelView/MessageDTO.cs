using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Social.Entity.ModelView
{
    public class MessageDTO
    {
        public string UserId { set; get; }
        public string Message { set; get; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string LocationName { get; set; }
        public DateTime? LocationStartTime { get; set; }
        public string LocationPeriod { get; set; }
        public bool? IsLiveLocation { get; set; }
        public int Messagetype { get; set; }
        public bool linkable { get; set; }
        public String EventLINKid { get; set; }
        public string Messagesdate { get; set; }
        public TimeSpan Messagestime { get; set; }
        public List<IFormFile> Attach { set; get; }
    }
    public class MessageVIEWDTO
    {
        public string Id { set; get; }
        public string UserId { set; get; }
        public string Message { set; get; }
        public int Messagetype { set; get; }
       
        public string Attach { set; get; }
    }
    public class EventMessageDTO
    {
        public bool eventjoin { get; set; }
        public int Messagetype { get; set; }
        public int EventChatAttendid { get; set; }
        public string EventId { set; get; }
        public bool linkable { get; set; }
        public String EventLINKid { get; set; }
        public string Message { set; get; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string LocationName { get; set; }
        public DateTime? LocationStartTime { get; set; }
        public string LocationPeriod { get; set; }
        public bool? IsLiveLocation { get; set; }

        public DateTime Messagesdate { get; set; }
        public TimeSpan Messagestime { get; set; }
        public List<IFormFile> Attach { set; get; }
    }
}
