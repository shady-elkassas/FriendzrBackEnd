using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Social.Entity.ModelView
{
  public  class eventattendMV
    {

        public int Id { get; set; }
        public string EntityId { get; set; }
        public string UserattendId { get; set; }
        public int stutus { get; set; }
        public DateTime? Messagesdate { get; set; }

        [Required(ErrorMessage = "Join Date  is required")]
        public DateTime? JoinDate { get; set; }
        [Required(ErrorMessage = "Join time  is required")]
        public TimeSpan? Jointime { get; set; }
        public string EventDataid { get; set; }
        public string note { get; set; }
    }
    public class eventclickout
    {

        public int Id { get; set; }
        public string EntityId { get; set; }
        public string UserattendId { get; set; }
        public int stutus { get; set; }
        public string EventDataid { get; set; }
        public string note { get; set; }

        [Required(ErrorMessage = "Action Date  is required")]
        public DateTime? ActionDate { get; set; }
        [Required(ErrorMessage = "Action time  is required")]
        public TimeSpan? Actiontime { get; set; }
    }
}
