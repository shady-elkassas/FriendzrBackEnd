using Social.Entity.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Social.Services.ModelView
{
public    class ChatGroupSubscribersVM
    {
        //public Guid ID { get; set; }
        public DateTime joinDate { get; set; }
        public DateTime? LeaveDateTime { get; set; }
        public DateTime? RemovedDateTime { get; set; }
        public DateTime? ClearChatDateTime { get; set; }
        public bool IsMuted { get; set; }
        public ChatGroupSubscriberStatus LeaveGroup { get; set; }
        public ChatGroupSubscriberType ChatGroupSubscriberType { get; set; }
        public bool isAdminGroup { get { return ChatGroupSubscriberType == ChatGroupSubscriberType.Admin; } }
        public string userId { get; set; }
        public string UserName { get; set; }
        public string image { get; set; }
    }
}
