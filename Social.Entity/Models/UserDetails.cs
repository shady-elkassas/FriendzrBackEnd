using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Social.Entity.Models
{
    public partial class UserDetails
    {
       
        public  UserDetails()
        {
            listoftags = new HashSet<listoftags>();
            LinkAccount = new HashSet<LinkAccount>();
            EventData = new HashSet<EventData>();
            //eventattend  = new HashSet<eventattend>();
            Requestesfor = new HashSet<Requestes>();
            Requestesto = new HashSet<Requestes>();
            Requestesblock= new HashSet<Requestes>();
            UserMessagesto = new HashSet<UserMessages>();
            UserMessages = new HashSet<UserMessages>();
            Messagedata= new HashSet<Messagedata>();
            AppearanceTypes = new HashSet<AppearanceTypes_UserDetails>();

            FireBaseDatamodel = new HashSet<FireBaseDatamodel>();
            Iprefertolist = new HashSet<Iprefertolist>();
            WhatBestDescripsMeList = new HashSet<WhatBestDescripsMeList>();
            UserLinkClicks = new HashSet<UserLinkClick>();
            EventTrackers = new HashSet<EventTracker>();
            EventCategoryTrackers = new HashSet<EventCategoryTracker>();
            SkippedUsers = new HashSet<SkippedUser>();
            UserSkippedUsers = new HashSet<SkippedUser>();
            SkippedEvents = new HashSet<SkippedEvent>();
            UserImages = new HashSet<UserImage>();
        }

        [Key]
        public int PrimaryId { get; set; }
        public string Id { get; set; }
        public string UserId { get; set; }
        public string ManagerId { get; set; }
        public string userName { get; set; }
        public string phone { get; set; }
        public string Email { get; set; }
        public string UserImage { get; set; }
        public bool AllowAds { get; set; }
        public string FcmToken { get; set; }
        public string pasword { get; set; }
        public string lang { get; set; }
        public string lat { get; set; }
        public int logintype { get; set; }
        public int platform { get; set; }
        public string userlogintypeid { get; set; }
        public string bio { get; set; }
        public string Facebook { get; set; }
        public string instagram { get; set; }
        //public string snapchat { get; set; }
        //public string tiktok { get; set; }
        public bool pushnotification { get; set; }
        public bool allowmylocation { get; set; } = true;
        public string whatAmILookingFor { get; set; }
        public decimal Manualdistancecontrol { get; set; }
        public int ageto { get; set; }
        public int agefrom { get; set; }
        public bool Filteringaccordingtoage { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Code { get; set; }
        public bool distanceFilter { get; set; }
        //public int AppearanceTypes { get; set; }
        public bool personalSpace { get; set; }
        public bool ghostmode { get; set; }
        public string language { get; set; }
        public string Gender { get; set; }
        public string OtherGenderName { get; set; }
        public int? CityID { get; set; }
        public int? CountryID { get; set; }
        public string ZipCode { get; set; }
        public DateTime? birthdate { get; set; }
        public DateTime? BanFrom { get; set; }
        public DateTime? BanTo { get; set; }
        public DateTime? LastUpdateLocation { get; set; }
        public DateTime LastFeedRequestDate { get; set; }
        public bool IsActive { get; set; } = true;
        public bool? ProfileCompleted { get; set; }
        public bool? IsWhiteLabel { get; set; }
        public bool? ImageIsVerified { get; set; }
        public virtual User User { get; set; }
        public virtual City City { get; set; }
        public virtual Country Country { get; set; }
        public virtual ICollection<FireBaseDatamodel> FireBaseDatamodel { get; set; }
        public virtual ICollection<Messagedata> Messagedata { get; set; }
        //public virtual ICollection<eventattend> eventattend { get; set; }
        public virtual ICollection<LinkAccount> LinkAccount { get; set; }
        public virtual ICollection<listoftags> listoftags { get; set; }
        public virtual ICollection<WhatBestDescripsMeList> WhatBestDescripsMeList { get; set; }
        public virtual ICollection<Iprefertolist> Iprefertolist { get; set; }
        public virtual ICollection<EventData> EventData { get; set; }
        public virtual ICollection<Requestes> Requestesfor { get; set; }
        public virtual ICollection<Requestes> Requestesto { get; set; }
        public virtual ICollection<UserMessages> UserMessagesto { get; set; }
        public virtual ICollection<UserMessages> UserMessages { get; set; }
        public virtual ICollection<Requestes> Requestesblock { get; set; }
        public virtual ICollection<AppearanceTypes_UserDetails> AppearanceTypes { get; set; }
        public virtual ICollection<UserLinkClick> UserLinkClicks { get; set; }
        public virtual ICollection<EventTracker> EventTrackers { get; set; }
        public virtual ICollection<EventCategoryTracker> EventCategoryTrackers { get; set; }
        public virtual ICollection<SkippedUser> SkippedUsers { get; set; }
        public virtual ICollection<SkippedUser> UserSkippedUsers { get; set; }
        public virtual ICollection<SkippedEvent> SkippedEvents { get; set; }
        public virtual ICollection<UserImage> UserImages { get; set; }
    }
}
