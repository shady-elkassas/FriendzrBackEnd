using System;
using System.Collections.Generic;
using System.Text;

namespace Social.Services.ModelView
{
  public  class WhatBestDescripsMeVM
    {
        public int ID { get; set; }
        public string EntityId { get; set; }  =Guid.NewGuid().ToString();
        public string name { get; set; }
        public bool IsSharedForAllUsers { get; set; }
    
        public string CreatedByUserID { get; set; }
        public DateTime RegistrationDate { get; set; } 
        public bool IsActive { get; set; }
    }
   
}
