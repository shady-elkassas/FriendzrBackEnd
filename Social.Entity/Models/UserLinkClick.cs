using System;
using System.ComponentModel.DataAnnotations;

namespace Social.Entity.Models
{
    public class UserLinkClick
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public virtual UserDetails userDetails { get; set; }
    }
}

