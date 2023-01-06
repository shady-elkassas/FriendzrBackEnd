using System;
using System.Collections.Generic;
using System.Text;

namespace Social.Entity.Models
{
    public class EventCategoryTracker
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public int UserId { get; set; }
        public DateTime Date { get; set; }
        public virtual category Category { get; set; }
        public virtual UserDetails User { get; set; }
    }
}
