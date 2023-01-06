using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Social.Entity.Models
{
  public  class AppConfigration
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ID { get; set; }
        public string Name { get; set; }
        public int? UserName_MaxLength { get; set; }
        public int? UserName_MinLength { get; set; }
        public int? UserIAM_MaxLength { get; set; }
        public int? UserIAM_MinLength { get; set; }
        public int? UserIPreferTo_MaxLength { get; set; }
        public int? UserIPreferTo_MinLength { get; set; }
        public int? Password_MaxLength { get; set; }
        public int? Password_MinLength { get; set; }
        public int? Password_MinNumbers { get; set; }
        public int? Password_MaxNumbers { get; set; }
        public int? Password_MaxSpecialCharacters { get; set; }
        public int? Password_MinSpecialCharacters { get; set; }
        public int? UserMinAge { get; set; }
        public int? UserMaxAge { get; set; }
        public int? UserBio_MaxLength { get; set; }
        public int? UserBio_MinLength { get; set; }
        public int? EventDetailsDescription_MinLength { get; set; }
        public int? EventDetailsDescription_MaxLength { get; set; }
        public int? EventTitle_MinLength { get; set; }
        public int? EventTitle_MaxLength { get; set; }
        public double? EventTimeValidation_MinLength { get; set; }
        public double? EventTimeValidation_MaxLength { get; set; } 
        public int? EventCreationLimitNumber_MinLength { get; set; }
        public int? EventCreationLimitNumber_MaxLength { get; set; }
        public int? UserTagM_MaxNumber { get; set; }
        public int? UserTagM_MinNumber { get; set; }
        public int? AgeFiltering_Min { get; set; }
        public int? AgeFiltering_Max { get; set; }   
        public int? DistanceFiltering_Min { get; set; }
        public int? DistanceFiltering_Max { get; set; }
        public int? DistanceShowNearbyAccountsInFeed_Min { get; set; }
      
        public int? DistanceShowNearbyAccountsInFeed_Max { get; set; }
     
        public int? DistanceShowNearbyEvents_Min { get; set; }
   
        public int? DistanceShowNearbyEvents_Max { get; set; }
    
        public int? DistanceShowNearbyEventsOnMap_Min { get; set; }

        public int? DistanceShowNearbyEventsOnMap_Max { get; set; }
        public int? RecommendedPeopleArea_Min { get; set; }
        public int? RecommendedPeopleArea_Max { get; set; }
        public int? RecommendedEventArea_Min { get; set; }
        public int? RecommendedEventArea_Max { get; set; }
        public int? WhiteLableCodeLength { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime RegistrationDate { get; set; }
        public bool IsActive { get; set; }

        [Required]
        public string UserID { get; set; }
        public virtual User User { get; set; }

    }
}
