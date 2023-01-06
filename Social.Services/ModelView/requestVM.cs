using System;
using System.Collections.Generic;
using System.Text;

namespace Social.Entity.ModelView
{
   public class requestVM
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int UserRequestId { get; set; }
        public int status { get; set; }

        public DateTime regestdata { get; set; }
    }
}
