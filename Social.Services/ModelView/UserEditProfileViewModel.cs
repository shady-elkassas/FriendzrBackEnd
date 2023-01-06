using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Social.Services.ModelView
{
    public class UserEditProfileViewModel
    {
        [Required]
        public IFormFile ProfilImage { get; set; }
    }
}
