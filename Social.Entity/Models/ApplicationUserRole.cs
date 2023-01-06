using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Social.Entity.Models
{
    public class ApplicationUserRole : IdentityUserRole<string>
    {
        public virtual User User { get; set; }
        public virtual ApplicationRole Role { get; set; }
    }
}
