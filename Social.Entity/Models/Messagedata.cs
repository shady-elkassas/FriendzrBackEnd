using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Social.Entity.Models
{
   public class Messagedata
    {
        public Messagedata()
        {
        }
        public string Id { get; set; }
        public string UserMessagessId { get; set; }
        [ForeignKey("ChatGroup")]
        public Guid? ChatGroupID { get; set; }
        public int Messagetype { get; set; }
        public int UserId { get; set; }
        public int? EventChatAttendId { get; set; }
        public string Messages { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string LocationName { get; set; }
        public string LocationStartTime { get; set; }
        public string LocationEndTime { get; set; }
        public string LocationPeriod { get; set; }
        public bool? IsLiveLocation { get; set; }

        public string MessagesAttached { get; set; }
        public bool linkable { get; set; }
        public int? EventDataid { get; set; }
        public int? TicketMasterEventDataid { get; set; }
        public DateTime Messagesdate { get; set; }
        public TimeSpan Messagestime { get; set; }
        public virtual ChatGroup ChatGroup { get; set; }
        public virtual UserMessages UserMessagess { get; set; }
        public virtual UserDetails User { get; set; }
        public virtual EventData EventData { get; set; }
        public virtual EventChatAttend EventChatAttend { get; set; }
     
    }
}
