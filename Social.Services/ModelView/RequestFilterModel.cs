using Social.Entity.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Social.Services.ModelView
{
    public class RequestFilterModel
    {
        public int Id { get; set; }
        public string EntityId { get; set; }
        public int? UserId { get; set; }
        public int? UserRequestId { get; set; }

        public string CityName { get; set; }

        public  UserDetails User { get; set; }
    }
}
