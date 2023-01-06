using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Social.Entity.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Social.Services.ModelView
{
   public class ChatGroupVM
    {
        public ChatGroupVM()
        {
            ChatGroupSubscribers =  new List<ChatGroupSubscribersVM>();
        }
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public bool IsMuted { get; set; }
        
        public int LeaveGroup { get; set; }
        public string ListOfUserIDs { get; set; }// هما اللي طالبين انها تستقبل ف سترنج :D
        [JsonIgnore]
        public IFormFile Image_File { get; set; }
        public DateTime RegistrationDateTime { get; set; }
        public List<ChatGroupSubscribersVM> ChatGroupSubscribers { get; set; }
    }
}
