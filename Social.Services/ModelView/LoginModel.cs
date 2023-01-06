using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Social.Models
{
    public class LoginModel
    {
        //[Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
        public int platform { get; set; }
        // [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        public string FcmToken { get; set; }
        public string logintype { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string whatAmILookingFor { get; set; }
    }
    public class LoginModel2
    {
        [Required(ErrorMessage = "Emailisrequired")]
        [DataType(DataType.EmailAddress, ErrorMessage = "NotValidEmail")]


        public string Email { get; set; }
        public int platform { get; set; }
        // [Required(ErrorMessage = "Password is required")]
        [Required(ErrorMessage = "PasswordIsRequired")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string FcmToken { get; set; }
        public string logintype { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public bool RememberMe { get; set; }
    }
    public enum Languages
    {
        Arabic = 1,
        English = 2,
        French = 2,
    }
}
