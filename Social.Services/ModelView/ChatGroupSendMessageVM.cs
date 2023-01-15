﻿using Microsoft.AspNetCore.Http;
using Social.Entity.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Social.Services.ModelView
{
    public class ChatGroupSendMessageVM
    {

        public Guid ChatGroupID { set; get; }
        public string Message { set; get; }
        public Messagetype MessageType { get; set; }
        public bool linkable { get; set; }
        public String EventLINKid { get; set; }
        public DateTime? MessagesDateTime { get; set; }
        [JsonIgnore]
        public IFormFile Attach_File { set; get; }
        public string Attach { set; get; }

    }
}