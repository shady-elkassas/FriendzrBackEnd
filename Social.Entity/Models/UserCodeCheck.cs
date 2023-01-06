using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Social.Entity.Models
{
    public class UserCodeCheck
    {
        [Key]
        public string Id { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public int Code { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
