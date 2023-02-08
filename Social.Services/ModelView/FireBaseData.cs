
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Social.Entity.ModelView
{
    public class FireBaseData
    {

        public string id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string imageUrl { get; set; }
        public int? Messagetype { get; set; }
        public string Action_code { get; set; }

        public string messageId { get; set; }
        public string senderId { get; set; }
        public string senderImage { get; set; }
        public string senderDisplayName { get; set; }
        public string  messsageImageURL { get; set; }
        public string userimage { get; set; }
        public string date { get; set; }
        public string time { get; set; }
        public string name { get; set; }
        public bool muit { get; set; }
        //public int Action_Id2 { get; set; }
        public string Action { get; set; }
        public string CreatedAt { get; set; }
        public string NotificationDate { get; set; }

        public bool isAdmin { get; set; }
        public Guid? eventtypeid { get; set; }
        public string eventtypecolor { get; set; }
        public string eventtype { get; set; }


        public string messsageLinkEvenId { get; set; }

        public string messsageLinkEvenTitle { get; set; }

        public string messsageLinkEvenImage { get; set; }

        public string messsageLinkEvencategorie { get; set; }
        // public string categorieid { get; set; }
        public int messsageLinkEvenjoined { get; set; }
        public int messsageLinkEvenkey { get; set; }
        public string messsageLinkEvencategorieimage { get; set; }

        public int messsageLinkEventotalnumbert { get; set; }

        public string messsageLinkEveneventdate { get; set; }
        public string messsageLinkEveneventdateto { get; set; }

        public bool messsageLinkEvenMyEvent { get; set; }

        public bool IsWhitelabel { get; set; }


    }

    public class FireBaseNotification
    {

        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("Body")]
        public string Body { get; set; }
        [JsonProperty("Messagetype")]
        public int? Messagetype { get; set; }
        [JsonProperty("Action_code")]
        public string Action_code { get; set; }
        [JsonProperty("Action")]
        public string Action { get; set; }
        [JsonProperty("image")]
        public string ImageUrl { get; set; }








    }
}
