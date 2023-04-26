using System;
using System.Collections.Generic;
using System.Text;

namespace Social.Entity.Models
{
  public  class EventChatAttend
    {
        public EventChatAttend()
        {
            Messagedata = new HashSet<Messagedata>();


        }
        public int Id { get; set; }
        public string EntityId { get; set; }
        public int? UserattendId { get; set; }
        public int stutus { get; set; }
        public bool ISAdmin { get; set; }
        public DateTime? JoinDate { get; set; }
        public TimeSpan? Jointime { get; set; }
        public DateTime? deletechatDate { get; set; }
        public TimeSpan? deletechattime { get; set; }
        public DateTime? leaveeventDate { get; set; }
        public TimeSpan? leaveeventtime { get; set; }
        public DateTime? leveeventchatDate { get; set; }
        public TimeSpan? leveeventchattime { get; set; }
        public bool delete { get; set; }
        public bool removefromevent { get; set; }
        public bool leave { get; set; }
        public bool leavechat { get; set; }
        public int EventDataid { get; set; }
       // public int TicketMasterEventDataid { get; set; }
        public string note { get; set; }
        public bool muit { get; set; }
        public bool isrecivedremindernotification { get; set; }
        public int UserNotreadcount { get; set; }
        public DateTime? deletedate { get; set; }
        public TimeSpan? delettime { get; set; }
        public virtual EventData EventData { get; set; }
        //public virtual EventDataTicketMaster EventDataTicketMaster { get; set; }
        public virtual UserDetails Userattend { get; set; }
        public virtual ICollection<Messagedata> Messagedata { get; set; }
    }
}
