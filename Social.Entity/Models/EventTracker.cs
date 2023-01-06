using System;
namespace Social.Entity.Models
{
    public class EventTracker
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public int UserId { get; set; }
        public string ActionType { get; set; }
        public DateTime Date { get; set; }
        public virtual EventData Event { get; set; }
        public virtual UserDetails User { get; set; }
    }
}

