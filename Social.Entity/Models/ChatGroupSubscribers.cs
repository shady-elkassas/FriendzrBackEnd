using Social.Entity.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Social.Entity.Models
{
 public   class ChatGroupSubscribers
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ID { get; set; }
        public DateTime JoinDateTime { get; set; }
        public DateTime? LeaveDateTime { get; set; }
        public DateTime? RemovedDateTime { get; set; }
        public DateTime? ClearChatDateTime { get; set; }
        public ChatGroupSubscriberStatus LeaveGroup { get; set; }
        public ChatGroupSubscriberType IsAdminGroup { get; set; }
        [ForeignKey("ChatGroup")]
        public Guid ChatGroupID { get; set; }
        [ForeignKey("User")]
        public string UserID { get; set; }
        public bool IsMuted { get; set; }
        public int UserNotreadcount { get; set; }
        public virtual  ChatGroup ChatGroup { get; set; }
        public virtual  User User { get; set; }
    }
}
