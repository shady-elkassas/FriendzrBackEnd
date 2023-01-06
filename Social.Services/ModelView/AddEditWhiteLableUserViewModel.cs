using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Social.Services.ModelView
{
    public class AddEditWhiteLableUserViewModel
    {
        public string ID { get; set; }
        public string Email { get; set; }
        public string DisplayedUserName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public IFormFile Image { get; set; }
        public string Code { get; set; }

    }
}
