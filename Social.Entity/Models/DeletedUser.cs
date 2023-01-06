using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Social.Entity.Models
{
    public class DeletedUser
    {
        [Key]
        public int Id { get; set; }
        public string IdentityUserId { get; set; }
        public string UserDetail { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public DateTime Date { get; set; }
    }
}
