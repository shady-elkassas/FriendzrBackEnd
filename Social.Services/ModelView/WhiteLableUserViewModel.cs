using System;
using System.Collections.Generic;
using System.Text;

namespace Social.Services.ModelView
{
    public class WhiteLableUserViewModel
    {
        public string ID { get; set; }
        public string Email { get; set; }
        public string DisplayedUserName { get; set; }
        public string UserName { get; set; }
        public string Image { get; set; }
        public string Password { get; set; }
        public string RegistrationDate { get; set; }
        public bool IsActive { get; set; }
        public string Code { get; set; }

    }
}
