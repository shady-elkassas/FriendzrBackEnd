using System;
using System.Collections.Generic;
using System.Text;

namespace Social.Entity.Models
{
  public  class LinkAccount
    {
        public int Id { get; set; }
        public string EntityId { get; set; }
        public string LinkAccountname { get; set; }
        public int? UserId { get; set; }
        public string LinkAccounturl { get; set; }
        public virtual UserDetails User { get; set; }
    }
}
