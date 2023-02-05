using System;
using System.Collections.Generic;
using System.Text;

namespace Social.Entity.ModelView
{
   public class Feeds
    {
       public string Gender { get; set; }
        public string userId { get; set; }
        public string lang { get; set; }
        public string lat { get; set; }
        public string DisplayedUserName { get; set; }
        public string UserName { get; set; }
        public string email { get; set; }
        public string image { get; set; }
        public bool? ImageIsVerified { get; set; }
        public int key { get; set; }
        public decimal InterestMatchPercent { get; set; }

    }
}
