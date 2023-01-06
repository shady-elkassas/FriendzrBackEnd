using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Social.Entity.Models
{
   public class ChatGroup
    {
        public ChatGroup()
        {
            Subscribers = new HashSet<ChatGroupSubscribers>();
            Messagedatas = new HashSet<Messagedata>();
            ChatGroupReports = new HashSet<ChatGroupReport>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        [ForeignKey("User")]
        public string UserID { get; set; }
        public bool IsActive { get; set; }
        public DateTime  RegistrationDateTime { get; set; }
        public virtual User  User { get; set; }
        public virtual ICollection<ChatGroupSubscribers> Subscribers { get; set; }
        public virtual ICollection<Messagedata> Messagedatas { get; set; }
        public virtual ICollection<ChatGroupReport> ChatGroupReports { get; set; }
    }
}
