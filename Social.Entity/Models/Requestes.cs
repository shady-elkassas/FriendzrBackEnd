using System;
using System.Collections.Generic;
using System.Text;

namespace Social.Entity.Models
{
    public class Requestes
    {
        public int Id { get; set; }
        public string EntityId { get; set; }
        public int?UserId { get; set; }
        public int? UserRequestId { get; set; }
        public int? UserblockId { get; set; }
        public DateTime? blockDate { get; set; }
        public int status { get; set; }
        public DateTime regestdata { get; set; }
        public DateTime? AcceptingDate { get; set; }
        public virtual UserDetails User { get; set; }
        public virtual UserDetails UserRequest { get; set; }
        public virtual UserDetails Userblock { get; set; }
    }
}
