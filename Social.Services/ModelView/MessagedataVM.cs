using Social.Entity.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Social.Entity.ModelView;
namespace Social.Entity.ModelView
{
   public class MessagedataVM
    {
        public string Id { get; set; }
        public bool currentuserMessage { get; set; }
        public string UserMessagessId { get; set; }
        public int Messagetype { get; set; }

        public bool linkable { get; set; }
        public String EventLINKid { get; set; }
        public string Username { get; set; }
        public Guid? eventtypeid { get; set; }
        public string eventtypecolor { get; set; }
        public string eventtype { get; set; }
        public string DisplayedUserName { get; set; }
        public string Userimage { get; set; }
        public string UserId { get; set; }
        public string Messages { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string EventChatID { get; set; }
        public string Messagesdate { get; set; }
        public string Messagestime { get; set; }
        public List<MessageAttachedVM> MessageAttachedVM { get; set; }
        public EventDatalinka EventData { get; set; }
        public bool IsWhitelabel { get; set; }
        public int Key { get; set; }
    }
    //public class EventDatavm
    //{
    //    public int Id { get; set; }
    //    public string EntityId { get; set; }
    //    public int? UserId { get; set; }
    //    public int? categorieId { get; set; }
    //    public string Title { get; set; }
    //    public string description { get; set; }
    //    public string status { get; set; }
    //    public string image { get; set; }
    //    public string category { get; set; }
    //    public string lang { get; set; }
    //    public string lat { get; set; }
    //    public int totalnumbert { get; set; }
    //    public bool? allday { get; set; }
    //    public string eventdate { get; set; }
    //    public string eventdateto { get; set; }
    //    public string eventfrom { get; set; }
    //    public string eventto { get; set; }
    //}
    public class MessageAttachedVM
    {

    
        public string attached { get; set; }
      


    }
}
