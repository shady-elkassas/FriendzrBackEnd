using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Social.Entity.Models
{
    public class RegisterModel
    {
        //[Required(ErrorMessage = "Username is required")]
        public string UserName { get; set; }
       // [Required(ErrorMessage = "Email is required")]

        public string Email { get; set; }
       /// [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        public int platform { get; set; }
        public string FcmToken { get; set; }
       // public string SocialUser { get; set; }
        public string registertype { get; set; }
        public string whatAmILookingFor { get; set; }
        public string UserID { get; set; }
        public string UserImage { get; set; }

        
    }
}
