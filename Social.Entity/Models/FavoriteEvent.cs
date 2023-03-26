using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;

namespace Social.Entity.Models
{
    public class FavoriteEvent
    {
        public Guid Id { get; set; }
        public int UserDetailsId { get; set; }
        public string EventEntityId { get; set; }
      
    }
}
