using Social.Services.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Social.Services.ModelView
{
    public class AppConfigrationVM : IValidatableObject
    {
        public string ID { get; set; }
        public string androidFrSecretKey { get; set; }
        public string androidFrAccessKey { get; set; }
        [Display(Name = "IsActive")]
        public bool IsActive { get; set; }
        [Display(Name = "ConfigrationName")]
        [Required(ErrorMessage ="Required")]
        public string Name { get; set; }
        [RegularExpression(@"[0-9]*$", ErrorMessage = "InvalidNumber")]
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")]
        [Display(Name = "UserName_MaxLength")]
        public int? UserName_MaxLength { get; set; }
        [RegularExpression(@"[0-9]*$", ErrorMessage = "InvalidNumber")]
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")]
        [Display(Name = "UserName_MinLength")]
        public int? UserName_MinLength { get; set; }
        [RegularExpression(@"[0-9]*$", ErrorMessage = "InvalidNumber")]
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")]
        [Display(Name = "UserIAM_MaxLength")]
        public int? UserIAM_MaxLength { get; set; }
        [RegularExpression(@"[0-9]*$", ErrorMessage = "InvalidNumber")]
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")]
        [Display(Name = "UserIAM_MinLength")]
        public int? UserIAM_MinLength { get; set; }
        [RegularExpression(@"[0-9]*$", ErrorMessage = "InvalidNumber")]
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")]
        [Display(Name = "UserIPreferTo_MaxLength")]
        public int? UserIPreferTo_MaxLength { get; set; }
        [RegularExpression(@"[0-9]*$", ErrorMessage = "InvalidNumber")]
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")]
        [Display(Name = "UserIPreferTo_MinLength")]
        public int? UserIPreferTo_MinLength { get; set; }
      
        [RegularExpression(@"[0-9]*$", ErrorMessage = "InvalidNumber")]
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")]
        [Display(Name = "Password_MaxLength")]
        public int? Password_MaxLength { get; set; }
        [Display(Name = "Password_MinLength")]
        [RegularExpression(@"[0-9]*$", ErrorMessage = "InvalidNumber")]
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")]
        public int? Password_MinLength { get; set; }
        [RegularExpression(@"[0-9]*$", ErrorMessage = "InvalidNumber")]
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")]
        [Display(Name = "Password_MinNumbers")]
        public int? Password_MinNumbers { get; set; }
        [Display(Name = "Password_MaxNumbers")]
        [RegularExpression(@"[0-9]*$", ErrorMessage = "InvalidNumber")]
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")]
        public int? Password_MaxNumbers { get; set; }
        [RegularExpression(@"[0-9]*$", ErrorMessage = "InvalidNumber")]
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")]
        [Display(Name = "Password_MaxSpecialCharacters")]
        public int? Password_MaxSpecialCharacters { get; set; }
        [RegularExpression(@"[0-9]*$", ErrorMessage = "InvalidNumber")]
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")]
        [Display(Name = "Password_MinSpecialCharacters")]
        public int? Password_MinSpecialCharacters { get; set; }
        [RegularExpression(@"[0-9]*$", ErrorMessage = "InvalidNumber")]
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")]
        [Display(Name = "UserMinAge")]
        public int? UserMinAge { get; set; }
        [RegularExpression(@"[0-9]*$", ErrorMessage = "InvalidNumber")]
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")]
        [Display(Name = "UserMaxAge")]
        public int? UserMaxAge { get; set; }
        [RegularExpression(@"[0-9]*$", ErrorMessage = "InvalidNumber")]
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")]
        [Display(Name = "UserBio_MaxLength")]
        public int? UserBio_MaxLength { get; set; }
        [RegularExpression(@"[0-9]*$", ErrorMessage = "InvalidNumber")]
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")]
        [Display(Name = "UserBio_MinLength")]
        public int? UserBio_MinLength { get; set; }
        [RegularExpression(@"[0-9]*$", ErrorMessage = "InvalidNumber")]
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")]
        [Display(Name = "EventDetailsDescription_MinLength")]
        public int? EventDetailsDescription_MinLength { get; set; }
        [RegularExpression(@"[0-9]*$", ErrorMessage = "InvalidNumber")]
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")]
        [Display(Name = "EventDetailsDescription_MaxLength")]
        public int? EventDetailsDescription_MaxLength { get; set; }   
        [RegularExpression(@"[0-9]*$", ErrorMessage = "InvalidNumber")]
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")]
        [Display(Name = "EventTitle_MinLength")]
        public int? EventTitle_MinLength { get; set; }
        [RegularExpression(@"[0-9]*$", ErrorMessage = "InvalidNumber")]
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")]
        [Display(Name = "EventTitle_MaxLength")]
        public int? EventTitle_MaxLength { get; set; }

        
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")]
        [Display(Name = "EventTimeValidation_MinLength")]
        public double? EventTimeValidation_MinLength { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")]
        [Display(Name = "EventTimeValidation_MaxLength")]
        public double? EventTimeValidation_MaxLength { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "InvalidNumber")]
        [Display(Name = "EventCreationLimitNumber_MinLength")]
        public int? EventCreationLimitNumber_MinLength { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "InvalidNumber")]
        [Display(Name = "EventCreationLimitNumber_MaxLength")]
        public int? EventCreationLimitNumber_MaxLength { get; set; }

        [RegularExpression(@"[0-9]*$", ErrorMessage = "InvalidNumber")]
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")]
        [Display(Name = "UserTagM_MaxNumber")]
        public int? UserTagM_MaxNumber { get; set; }
        [RegularExpression(@"[0-9]*$", ErrorMessage = "InvalidNumber")]
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")]
        [Display(Name = "UserTagM_MinNumber")]
        public int? UserTagM_MinNumber { get; set; }
        [RegularExpression(@"[0-9]*$", ErrorMessage = "InvalidNumber")]
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")]
        [Display(Name = "AgeFiltering_Min")]
        public int? AgeFiltering_Min { get; set; }
        [Display(Name = "AgeFiltering_Max")]
        [RegularExpression(@"[0-9]*$", ErrorMessage = "InvalidNumber")]
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")]
        public int? AgeFiltering_Max { get; set; }
        [Display(Name = "DistanceFiltering_Min")]
        [RegularExpression(@"[0-9]*$", ErrorMessage = "InvalidNumber")]
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")] 
        public int? DistanceFiltering_Min { get; set; }
        [Display(Name = "DistanceFiltering_Max")]
        [RegularExpression(@"[0-9]*$", ErrorMessage = "InvalidNumber")]
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")] 
        public int? DistanceFiltering_Max { get; set; }
        [Display(Name = "DistanceShowNearbyAccountsInFeed_Min")]
        [RegularExpression(@"[0-9]*$", ErrorMessage = "InvalidNumber")]
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")] 
        public int? DistanceShowNearbyAccountsInFeed_Min { get; set; }
        [Display(Name = "DistanceShowNearbyAccountsInFeed_Max")]
        [RegularExpression(@"[0-9]*$", ErrorMessage = "InvalidNumber")]
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")] 
        public int? DistanceShowNearbyAccountsInFeed_Max { get; set; }  
        [Display(Name = "DistanceShowNearbyEvents_Min")]
        [RegularExpression(@"[0-9]*$", ErrorMessage = "InvalidNumber")]
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")]
        public int? DistanceShowNearbyEvents_Min { get; set; }
        [Display(Name = "DistanceShowNearbyEvents_Max")]
        [RegularExpression(@"[0-9]*$", ErrorMessage = "InvalidNumber")]
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")] 
        public int? DistanceShowNearbyEvents_Max { get; set; }
        [Display(Name = "DistanceShowNearbyEventsOnMap_Min")]
        [RegularExpression(@"[0-9]*$", ErrorMessage = "InvalidNumber")]
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")] 
        public int? DistanceShowNearbyEventsOnMap_Min { get; set; }
        [Display(Name = "DistanceShowNearbyEventsOnMap_Max")]
        [RegularExpression(@"[0-9]*$", ErrorMessage = "InvalidNumber")]
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")] 
        public int? DistanceShowNearbyEventsOnMap_Max { get; set; }

        [Display(Name = "RecommendedPeopleArea_Min")]
        [RegularExpression(@"[0-9]*$", ErrorMessage = "InvalidNumber")]
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")]
        public int? RecommendedPeopleArea_Min { get; set; }

        [Display(Name = "RecommendedPeopleArea_Max")]
        [RegularExpression(@"[0-9]*$", ErrorMessage = "InvalidNumber")]
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")]
        public int? RecommendedPeopleArea_Max { get; set; }

        [Display(Name = "RecommendedEventArea_Min")]
        [RegularExpression(@"[0-9]*$", ErrorMessage = "InvalidNumber")]
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")]
        public int? RecommendedEventArea_Min { get; set; }

        [Display(Name = "RecommendedEventArea_Max")]
        [RegularExpression(@"[0-9]*$", ErrorMessage = "InvalidNumber")]
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")]
        public int? RecommendedEventArea_Max { get; set; }

        [Display(Name = "WhiteLableCodeLength")]
        [RegularExpression(@"[4-9]*$", ErrorMessage = "InvalidNumber")]
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")]
        public int? WhiteLableCodeLength { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var repo = (IAppConfigrationService)validationContext.GetService(typeof(IAppConfigrationService));
            var validation = repo._ValidationResult(this);
            return validation;
        }
    }
    public class AppConfigrationVM_Required : IValidatableObject
    {
        public string ID { get; set; }
        [Display(Name = "IsActive")]
        public bool IsActive { get; set; }
        [Display(Name = "ConfigrationName")]
        [Required(ErrorMessage = "Required")]
        public string Name { get; set; }
        [RegularExpression(@"[0-9]*$", ErrorMessage = "InvalidNumber")]
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")]
        [Display(Name = "UserName_MaxLength")]
        [Required(ErrorMessage = "Required")]
        public int? UserName_MaxLength { get; set; }
        [RegularExpression(@"[0-9]*$", ErrorMessage = "InvalidNumber")]
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")]
        [Display(Name = "UserIAM_MaxLength")]
        [Required(ErrorMessage = "Required")]
        public int? UserIAM_MaxLength { get; set; }
        [RegularExpression(@"[0-9]*$", ErrorMessage = "InvalidNumber")]
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")]
        [Display(Name = "UserIAM_MinLength")]
        [Required(ErrorMessage = "Required")]
        public int? UserIAM_MinLength { get; set; }
        [RegularExpression(@"[0-9]*$", ErrorMessage = "InvalidNumber")]
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")]
        [Display(Name = "UserIPreferTo_MaxLength")]
        [Required(ErrorMessage = "Required")]
        public int? UserIPreferTo_MaxLength { get; set; }
        [RegularExpression(@"[0-9]*$", ErrorMessage = "InvalidNumber")]
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")]
        [Display(Name = "UserIPreferTo_MinLength")]
        [Required(ErrorMessage = "Required")]
        public int? UserIPreferTo_MinLength { get; set; }
        [Required(ErrorMessage = "Required")]

        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")]
        [Display(Name = "EventTimeValidation_MinLength")]
        public double? EventTimeValidation_MinLength { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")]
        [Required(ErrorMessage = "Required")]

        [Display(Name = "EventTimeValidation_MaxLength")]
        public double? EventTimeValidation_MaxLength { get; set; }
        [Required(ErrorMessage = "Required")]

        [Range(0, int.MaxValue, ErrorMessage = "InvalidNumber")]
        [Display(Name = "EventCreationLimitNumber_MinLength")]
        public int? EventCreationLimitNumber_MinLength { get; set; }
        [Required(ErrorMessage = "Required")]

        [Range(0, int.MaxValue, ErrorMessage = "InvalidNumber")]
        [Display(Name = "EventCreationLimitNumber_MaxLength")]
        public int? EventCreationLimitNumber_MaxLength { get; set; }
        [Required(ErrorMessage = "Required")]

        [Display(Name = "UserName_MinLength")]
        [RegularExpression(@"[0-9]*$", ErrorMessage = "InvalidNumber")]
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")]
        public int? UserName_MinLength { get; set; }
        [RegularExpression(@"[0-9]*$", ErrorMessage = "InvalidNumber")]
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")]
        [Display(Name = "Password_MaxLength")]
        [Required(ErrorMessage = "Required")]
        public int? Password_MaxLength { get; set; }
        [Display(Name = "Password_MinLength")]
        [RegularExpression(@"[0-9]*$", ErrorMessage = "InvalidNumber")]
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")]
        [Required(ErrorMessage = "Required")]
        public int? Password_MinLength { get; set; }
        [RegularExpression(@"[0-9]*$", ErrorMessage = "InvalidNumber")]
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")]
        [Display(Name = "Password_MinNumbers")]
        [Required(ErrorMessage = "Required")]
        public int? Password_MinNumbers { get; set; }
        [Display(Name = "Password_MaxNumbers")]
        [RegularExpression(@"[0-9]*$", ErrorMessage = "InvalidNumber")]
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")]
        [Required(ErrorMessage = "Required")]
        public int? Password_MaxNumbers { get; set; }
        [RegularExpression(@"[0-9]*$", ErrorMessage = "InvalidNumber")]
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")]
        [Display(Name = "Password_MaxSpecialCharacters")]
        [Required(ErrorMessage = "Required")]
        public int? Password_MaxSpecialCharacters { get; set; }
        [RegularExpression(@"[0-9]*$", ErrorMessage = "InvalidNumber")]
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")]
        [Display(Name = "Password_MinSpecialCharacters")]
        [Required(ErrorMessage = "Required")]
        public int? Password_MinSpecialCharacters { get; set; }
        [RegularExpression(@"[0-9]*$", ErrorMessage = "InvalidNumber")]
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")]
        [Display(Name = "UserMinAge")]
        [Required(ErrorMessage = "Required")]
        public int? UserMinAge { get; set; }
        [RegularExpression(@"[0-9]*$", ErrorMessage = "InvalidNumber")]
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")]
        [Display(Name = "UserMaxAge")]
        [Required(ErrorMessage = "Required")]
        public int? UserMaxAge { get; set; }
        [RegularExpression(@"[0-9]*$", ErrorMessage = "InvalidNumber")]
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")]
        [Display(Name = "UserBio_MaxLength")]
        [Required(ErrorMessage = "Required")]
        public int? UserBio_MaxLength { get; set; }
        [RegularExpression(@"[0-9]*$", ErrorMessage = "InvalidNumber")]
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")]
        [Display(Name = "UserBio_MinLength")]
        [Required(ErrorMessage = "Required")]
        public int? UserBio_MinLength { get; set; }
        [RegularExpression(@"[0-9]*$", ErrorMessage = "InvalidNumber")]
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")]
        [Display(Name = "EventDetailsDescription_MinLength")]
        public int? EventDetailsDescription_MinLength { get; set; }
        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"[0-9]*$", ErrorMessage = "InvalidNumber")]
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")]
        [Display(Name = "EventDetailsDescription_MaxLength")]

        public int? EventDetailsDescription_MaxLength { get; set; }  
        [RegularExpression(@"[0-9]*$", ErrorMessage = "InvalidNumber")]
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")]
        [Display(Name = "EventTitle_MinLength")]
        public int? EventTitle_MinLength { get; set; }
        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"[0-9]*$", ErrorMessage = "InvalidNumber")]
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")]
        [Display(Name = "EventTitle_MaxLength")]

        public int? EventTitle_MaxLength { get; set; }
        [RegularExpression(@"[0-9]*$", ErrorMessage = "InvalidNumber")]
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")]
        [Display(Name = "UserTagM_MaxNumber")]
        [Required(ErrorMessage = "Required")]
        public int? UserTagM_MaxNumber { get; set; }
        [RegularExpression(@"[0-9]*$", ErrorMessage = "InvalidNumber")]
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")]
        [Display(Name = "UserTagM_MinNumber")]
        [Required(ErrorMessage = "Required")]
        public int? UserTagM_MinNumber { get; set; }
        [RegularExpression(@"[0-9]*$", ErrorMessage = "InvalidNumber")]
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")]
        [Display(Name = "AgeFiltering_Min")]
        [Required(ErrorMessage = "Required")]
        public int? AgeFiltering_Min { get; set; }
        [Display(Name = "AgeFiltering_Max")]
        [RegularExpression(@"[0-9]*$", ErrorMessage = "InvalidNumber")]
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")]
        [Required(ErrorMessage = "Required")]
        public int? AgeFiltering_Max { get; set; }
        [Display(Name = "DistanceFiltering_Min")]
        [RegularExpression(@"[0-9]*$", ErrorMessage = "InvalidNumber")]
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")]
        [Required(ErrorMessage = "Required")]
        public int? DistanceFiltering_Min { get; set; }
        [Display(Name = "DistanceFiltering_Max")]
        [RegularExpression(@"[0-9]*$", ErrorMessage = "InvalidNumber")]
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")]
        [Required(ErrorMessage = "Required")]
        public int? DistanceFiltering_Max { get; set; }
        [Display(Name = "DistanceShowNearbyAccountsInFeed_Min")]
        [RegularExpression(@"[0-9]*$", ErrorMessage = "InvalidNumber")]
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")]
        [Required(ErrorMessage = "Required")]
        public int? DistanceShowNearbyAccountsInFeed_Min { get; set; }
        [Display(Name = "DistanceShowNearbyAccountsInFeed_Max")]
        [RegularExpression(@"[0-9]*$", ErrorMessage = "InvalidNumber")]
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")]
        [Required(ErrorMessage = "Required")]
        public int? DistanceShowNearbyAccountsInFeed_Max { get; set; }
        [Display(Name = "DistanceShowNearbyEvents_Min")]
        [RegularExpression(@"[0-9]*$", ErrorMessage = "InvalidNumber")]
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")]
        [Required(ErrorMessage = "Required")]
        public int? DistanceShowNearbyEvents_Min { get; set; }
        [Display(Name = "DistanceShowNearbyEvents_Max")]
        [RegularExpression(@"[0-9]*$", ErrorMessage = "InvalidNumber")]
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")]
        [Required(ErrorMessage = "Required")]
        public int? DistanceShowNearbyEvents_Max { get; set; }
        [Display(Name = "DistanceShowNearbyEventsOnMap_Min")]
        [RegularExpression(@"[0-9]*$", ErrorMessage = "InvalidNumber")]
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")]
        [Required(ErrorMessage = "Required")]
        public int? DistanceShowNearbyEventsOnMap_Min { get; set; }
        [Display(Name = "DistanceShowNearbyEventsOnMap_Max")]
        [RegularExpression(@"[0-9]*$", ErrorMessage = "InvalidNumber")]
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")]
        [Required(ErrorMessage = "Required")]
        public int? DistanceShowNearbyEventsOnMap_Max { get; set; }


        [Display(Name = "RecommendedPeopleArea_Min")]
        [RegularExpression(@"[0-9]*$", ErrorMessage = "InvalidNumber")]
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")]
        [Required(ErrorMessage = "Required")]
        public int? RecommendedPeopleArea_Min { get; set; }

        [Display(Name = "RecommendedPeopleArea_Max")]
        [RegularExpression(@"[0-9]*$", ErrorMessage = "InvalidNumber")]
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")]
        [Required(ErrorMessage = "Required")]
        public int? RecommendedPeopleArea_Max { get; set; }

        [Display(Name = "RecommendedEventArea_Min")]
        [RegularExpression(@"[0-9]*$", ErrorMessage = "InvalidNumber")]
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")]
        [Required(ErrorMessage = "Required")]
        public int? RecommendedEventArea_Min { get; set; }

        [Display(Name = "RecommendedEventArea_Max")]
        [RegularExpression(@"[0-9]*$", ErrorMessage = "InvalidNumber")]
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")]
        [Required(ErrorMessage = "Required")]
        public int? RecommendedEventArea_Max { get; set; }

        [Display(Name = "WhiteLableCodeLength")]
        [RegularExpression(@"[0-9]*$", ErrorMessage = "InvalidNumber")]
        [Range(0, double.MaxValue, ErrorMessage = "InvalidNumber")]
        public int? WhiteLableCodeLength { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var repo = (IAppConfigrationService)validationContext.GetService(typeof(IAppConfigrationService));
            var validation = repo._ValidationResult(this);
            return validation;
        }
    }
}
