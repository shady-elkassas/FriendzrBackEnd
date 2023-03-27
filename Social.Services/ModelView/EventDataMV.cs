using Social.Entity.Models;
using System;
using System.Collections.Generic;

namespace Social.Entity.ModelView
{
    public class locationDataMV
    {
        public List<EventlocationDataMV> EventlocationDataMV { get; set; }
        public List<peoplocationDataMV> locationMV { get; set; }

    }

    public class EventlocationDataMV
    {
        public decimal lang { get; set; }
        public decimal lat { get; set; }
        public string Event_Type { get; set; }
        public string EventId { get; set; }
        //public Guid Event_TypeId { get; set; }
        //public string Event_TypeColor { get; set; }
        public int count { get; set; }
        public String color { get; set; }
        public string EventMarkerImage { get; set; }
        public List<EventDataMV> EventData { get; set; }
        public string EventTypeName { get; set; }

    }
    public class Eventjson
    {
        public List<EventData> model { get; set; }

    }
    public class peoplocationDataMV
    {
        public decimal lang { get; set; }
        public decimal lat { get; set; }

        // public int peoplecount { get; set; }
        public String color { get; set; }
        public decimal MalePercentage { get; set; }
        public decimal Femalepercentage { get; set; }
        public decimal otherpercentage { get; set; }
        public int totalUsers { get; set; }
    }
    public class peopleMV
    {
        public int count { get; set; }
        public string type { get; set; }
    }
    public class EventDataMV
    {

        public string Id { get; set; }
        public string EntityId { get; set; }
        public string EncryptedID { get; set; }
        // public int UserId { get; set; }
        public string UseraddedId { get; set; }
        public string categorieId { get; set; }
        public string Title { get; set; }
        public string description { get; set; }
        //   public string status { get; set; }
        public string Image { get; set; }
        //public string image { get; set; }
        public string category { get; set; }
        public string categorie { get; set; }
        // public string categorieid { get; set; }
        //public int joined { get; set; }
        //public int key { get; set; }
        public string categorieimage { get; set; }
        public string eventtype { get; set; }
        public Guid eventtypeid { get; set; }
        public string eventtypecolor { get; set; }
        public string lang { get; set; }
        public string lat { get; set; }
        public int totalnumbert { get; set; }
        public bool allday { get; set; }
        public string eventdate { get; set; }
        public string eventdateto { get; set; }
        public string timefrom { get; set; }
        public string timeto { get; set; }
        public string color { get; set; }
        public string timetext { get; set; }
        public string datetext { get; set; }
        public bool MyEvent { get; set; }
        public string UserImage { get; set; }
        public string EventTypeName { get; set; }

    }
    public class EventDataByLocationMV
    {

        public string Id { get; set; }
       
        public string UseraddedId { get; set; }
        public string categorieId { get; set; }
        public string Title { get; set; }
        //public string description { get; set; }
        public string Image { get; set; }
        public string category { get; set; }
       // public string eventtype { get; set; }
       // public Guid eventtypeid { get; set; }
       // public string eventtypecolor { get; set; }
        public string lang { get; set; }
        public string lat { get; set; }
        public int totalnumbert { get; set; }
        public bool allday { get; set; }
        public string eventdate { get; set; }
        //public string eventdateto { get; set; }
        //public string timefrom { get; set; }
        //public string timeto { get; set; }
        //public string color { get; set; }
        //public bool MyEvent { get; set; }
        //public string UserImage { get; set; }
        public string EventTypeName { get; set; }
        public bool IsFavorite { get; set; }

    }

    public class GetFavoriteEventsDto
    {
        public string Id { get; set; }
        public string description { get; set; }
        public string categorie { get; set; }
        public string categorieimage { get; set; }
        public string eventtypeid { get; set; }
        public bool EventHasExpired { get; set; }
        public string eventtypecolor { get; set; }
        public string eventtype { get; set; }
        public string lang { get; set; }
        public string lat { get; set; }
        public int OrderByDes { get; set; }
        public string eventdate { get; set; }
        public bool? showAttendees { get; set; }
        public string eventdateto { get; set; }
        public bool allday { get; set; }
        public string timefrom { get; set; }
        public string timeto { get; set; }
        public string Title { get; set; }
        public string checkout_details { get; set; }
        public string image { get; set; }
        public int joined { get; set; }
        public int totalnumbert { get; set; }
        public int key { get; set; }
        public List<AttendeesDto> Attendees { get; set; }

    }

    public class AttendeesDto
    {
        public string image { get; set; }
        public string UserName { get; set; }
        public string DisplayedUserName { get; set; }
        public string UserId { get; set; }
        public int stutus { get; set; }
        public string JoinDate { get; set; }
        public bool MyEvent { get; set; }
        public List<InterestsDto> interests { get; set; }

    }

    public class InterestsDto
    {
        public int InterestsId { get; set; }
        public string Name { get; set; }
    }
    public class EventDatalinka
    {

        public string Id { get; set; }

        public string Title { get; set; }

        public string Image { get; set; }

        public string categorie { get; set; }
        // public string categorieid { get; set; }
        public int joined { get; set; }
        public int key { get; set; }
        public string categorieimage { get; set; }
        public Guid? eventtypeid { get; set; }
        public string eventtypecolor { get; set; }
        public string eventtype { get; set; }
        public int totalnumbert { get; set; }

        public string eventdate { get; set; }
        public string eventdateto { get; set; }

        public bool MyEvent { get; set; }
    }
    public class muitchate
    {
        public bool muit { get; set; }
        public bool isevent { get; set; }
        public string id { get; set; }
    }
    public class deletechat
    {

        public bool isevent { get; set; }
        public string id { get; set; }
        public DateTime? DeleteDateTime { get; set; }
    }


}
