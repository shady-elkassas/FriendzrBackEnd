using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Social.Entity.Models
{
    public class User : IdentityUser
    {
        public User()
        {
            AppConfigrations = new List<AppConfigration>();
            UserReports_CreatedBy = new List<UserReport>();
            UserReports_Reported = new List<UserReport>();
            Interests = new List<Interests>();
            EventReports = new List<EventReport>();
            ChatGroupSubscribers = new HashSet<ChatGroupSubscribers>();
            ChatGroups = new HashSet<ChatGroup>();
            ChatGroupReports = new HashSet<ChatGroupReport>();
            DistanceFilterHistorys = new HashSet<DistanceFilterHistory>();
            FilteringAccordingToAgeHistorys = new HashSet<FilteringAccordingToAgeHistory>();
        }
        public string UserloginId { get; set; }
        public  string logintypevalue { get; set; }
        public string DisplayedUserName { get; set; }
        public DateTime EmailConfirmedOn { get; set; }
        public DateTime RegistrationDate { get; set; }
        public virtual UserDetails UserDetails { get; set; }
        public virtual List<LoggedinUser> LoggedinUser { get; set; }
        public virtual ICollection<AppConfigration> AppConfigrations { get; set; }
        public virtual ICollection<UserReport> UserReports_CreatedBy { get; set; }
        public virtual ICollection<UserReport> UserReports_Reported { get; set; }
        public virtual ICollection<Interests> Interests { get; set; }
        public virtual ICollection<EventReport> EventReports { get; set; }
        public virtual ICollection<ChatGroup> ChatGroups { get; set; }
        public virtual ICollection<ChatGroupReport> ChatGroupReports { get; set; }
        public virtual ICollection<ChatGroupSubscribers> ChatGroupSubscribers { get; set; }
        public virtual ICollection<DistanceFilterHistory> DistanceFilterHistorys { get; set; }
        public virtual ICollection<FilteringAccordingToAgeHistory> FilteringAccordingToAgeHistorys { get; set; }
        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; } = new List<ApplicationUserRole>();

        //public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }
    }
}
