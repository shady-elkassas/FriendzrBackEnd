using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Social.Entity.Models
{
    public class UserModel
    {
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Phone Number is required")]
        public string PhoneNumber { get; set; }
        public string UserImage { get; set; }

    }
    public class updateUserModel
    {
        public string Username { get; set; }
        public string DisplayedUserName { get; set; }

        public string Email { get; set; }
        public string UserImage { get; set; }

        [Required(ErrorMessage = "bio  is required")]
        public string bio { get; set; }
        public string Gender { get; set; }
        public string OtherGenderName { get; set; }
        public DateTime birthdate { get; set; }
        public List<LinkAccount> LinkAccount { get; set; }
        public bool personalSpace { get; set; }
        public bool? ImageIsVerified { get; set; }
        public string Facebook { get; set; }
        public string instagram { get; set; }
        public string snapchat { get; set; }
        public string tiktok { get; set; }
        public string whatAmILookingFor { get; set; }
        public List<string> listoftags { get; set; }
        public List<string> Iam { get; set; }
        public List<string> preferto { get; set; }
        public string UniversityCode { get; set; }
    }
    public class updateUserModelview
    {
        public int age { get; set; }
        public int NeedUpdate { get; set; }
        //public string Facebook { get; set; }
        //public string instagram { get; set; }
        //public string snapchat { get; set; }
        public string whatAmILookingFor { get; set; }
        public string tiktok { get; set; }
        public string UserName { get; set; }

        public string DisplayedUserName { get; set; }
        public string Userid { get; set; }
        public int key { get; set; }
        public string Email { get; set; }
        public string UserImage { get; set; }
        public List<string> UserImages { get; set; }
        public string bio { get; set; }
        public string lang { get; set; }
        public string language { get; set; }
        public bool pushnotification { get; set; }
        public bool ghostmode { get; set; }
        public bool allowmylocation { get; set; }
        public decimal Manualdistancecontrol { get; set; }
        public int ageto { get; set; }
        public int agefrom { get; set; }
        public bool Filteringaccordingtoage { get; set; }
        public bool distanceFilter { get; set; }

        public bool personalSpace { get; set; }
        public List<int> MyAppearanceTypes { get; set; }
        public string lat { get; set; }
        public string Gender { get; set; }
        public string OtherGenderName { get; set; }
      
        public String birthdate { get; set; }
        public List<LinkAccountmodel> LinkAccountmodel { get; set; }
        public List<listoftagsmodel> listoftagsmodel { get; set; }
        public List<listoftagsmodel> IamList { get; set; }
        public List<listoftagsmodel> prefertoList { get; set; }

        public string UniversityCode { get; set; }
    }
    public class updateUserModelviewprofile
    {
        
       public int age { get; set; }
        public int NeedUpdate { get; set; }
        //public string Facebook { get; set; }
        //public string instagram { get; set; }
        //public string snapchat { get; set; }
        public string whatAmILookingFor { get; set; }
        public string tiktok { get; set; }
        public string UserName { get; set; }
        public string UniversityCode { get; set; }
        public string DisplayedUserName { get; set; }
        public string Userid { get; set; }
        public int key { get; set; }
        public string Email { get; set; }
        public string UserImage { get; set; }
        public List<string> UserImages { get; set; }
        public string bio { get; set; }
        public string lang { get; set; }
        public string language { get; set; }
        public bool pushnotification { get; set; }
        public bool ghostmode { get; set; }
        public bool? ImageIsVerified { get; set; }
        public bool allowmylocation { get; set; }
        public decimal Manualdistancecontrol { get; set; }
        public int ageto { get; set; }
        public int agefrom { get; set; }
        public bool Filteringaccordingtoage { get; set; }
        public bool distanceFilter { get; set; }

        public bool personalSpace { get; set; }
        public List<int> MyAppearanceTypes { get; set; }
        public string lat { get; set; }
        public string Gender { get; set; }
        public string OtherGenderName { get; set; }
        public String birthdate { get; set; }
        public int? FrindRequestNumber  { get; set; }
        public int? Message_Count { get; set; }
        public int? notificationcount { get; set; }
public List<LinkAccountmodel> LinkAccountmodel { get; set; }
        public List<listoftagsmodel> listoftagsmodel { get; set; }
        public List<listoftagsmodel> IamList { get; set; }
        public List<listoftagsmodel> prefertoList { get; set; }
    }
    public class LinkAccountmodel
    {

        public string LinkAccountname { get; set; }
        public string LinkAccounturl { get; set; }
    }
    public class UserDetailsvm
    {
        public bool muit { get; set; }
        public bool isevent { get; set; }
        public bool isChatGroup { get; set; }
        public bool isCommunityGroup { get; set; }
        public bool IsChatGroupAdmin { get; set; }
        public int LeaveGroup { get; set; }
        public bool myevent { get; set; }
        public bool isfrind { get; set; }
        public bool? isactive { get; set; }
        public int Leavevent { get; set; }
        public bool Leaveventchat { get; set; }
        public string id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string messages { get; set; }
        public int messagestype { get; set; }
        public int message_not_Read { get; set; }
        public string messagesimage { get; set; }
        public string latestdate { get; set; }
        public string latesttime { get; set; }
        public Guid? eventtypeid { get; set; }

        public int? eventtypeintid { get; set; }
        public string eventtypecolor { get; set; }
        public string eventtype { get; set; }
        public DateTime latestdatevm { get; set; }
        public TimeSpan latesttimevm { get; set; }
        public int Key { get; set; }
        public bool IsWhiteLabel { get; set; }

    }
    public class listoftagsmodel
    {
        public string tagID { get; set; }
        public string tagname { get; set; }

    }
}
