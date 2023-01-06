using System;
using System.ComponentModel.DataAnnotations;

namespace Social.Entity.Models
{
    public class SkippedEvent
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int EventId { get; set; }
        public DateTime Date { get; set; }
        public virtual UserDetails UserDetail { get; set; }
        public virtual EventData EventData { get; set; }
    }
}
