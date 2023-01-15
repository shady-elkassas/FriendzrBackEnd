using System;
using System.Collections.Generic;
using System.Text;

namespace Social.Entity.Models
{
    public class UserImage
    {
        public int Id { get; set; }
        public int? UserDetailsId { get; set; }
        public string UserId { get; set; }
        public string ImageUrl { get; set; }
        public virtual UserDetails UserDetails { get; set; }
    }
}
