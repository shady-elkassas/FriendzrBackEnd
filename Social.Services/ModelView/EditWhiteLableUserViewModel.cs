using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Social.Services.ModelView
{
    public class EditWhiteLableUserViewModel
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string DisplayedUserName { get; set; }
        public string UserName { get; set; }
        public IFormFile Image { get; set; }
    }
}
