using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Social.Entity.Models
{
    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole()
        {

        }
        public ApplicationRole(string RoleName) : base(RoleName)
        {

        }
        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; } = new List<ApplicationUserRole>();
        //public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }

    }
    //public class ApplicationUserRole : IdentityUserRole<string>
    //{
    //    [ForeignKey("UserId")]
    //    public virtual User User { get; set; }
    //    [ForeignKey("RoleId")]
    //    public virtual ApplicationRole Role { get; set; }
    //}

}
