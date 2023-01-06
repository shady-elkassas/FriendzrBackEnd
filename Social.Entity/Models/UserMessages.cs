using System;
using System.Collections.Generic;
using System.Text;

namespace Social.Entity.Models
{
   public class UserMessages
    {
        public UserMessages()
        {
            Messagedata = new HashSet<Messagedata>();
        }
        public string Id { get; set; }
        public int UserId { get; set; }
        public int ToUserId { get; set; }
        public int UserNotreadcount { get; set; }
        public int ToUserNotreadcount { get; set; }
        public DateTime startedin { get; set; }
        public string muit { get; set; }
        public string Tomuit { get; set; }
        public TimeSpan? deleteTime { get; set; }
        public TimeSpan? UserdeleteTime { get; set; }
        public string delete { get; set; }
        public string Todelete { get; set; }
        public DateTime? deletedate { get; set; }
        public DateTime? Userdeletedate { get; set; }
        public virtual UserDetails User { get; set; }
        public virtual UserDetails ToUser { get; set; }

        public virtual ICollection<Messagedata> Messagedata { get; set; }
    }
}
