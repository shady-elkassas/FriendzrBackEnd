using Social.Entity.Models;
using Social.Services.ModelView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Social.Services.Services
{
    public interface IUserService
    {
        Task UpdateUserAddressFromGoogle(UserDetails userDetails, double latitude, double longitude);
        Task InitializeSuperAdminAccount();
        Task InitializeAdminAccount();
        void InsertUserDetails(UserDetails user);
        Task DeleteUser_StoredProcedure(UserDetails userDetails);
        void InsertLoggedInUser(LoggedinUser user);
        bool AddUserImages(List<UserImage> files);
        UserCodeCheck GetUserCodeByEmail(string email);
        void InsertUserCode(UserCodeCheck code);
        void UpdateUserCode(UserCodeCheck code);
        IQueryable<UserDetails> getallUserDetails();
        void DeleteUserCode(UserCodeCheck code);
        void UpdateLoggedInUser(LoggedinUser user);
        Task<LoggedinUser> GetLoggedInUser(string token);
        Task<List<LoggedinUser>> GetLoggedInUsers(string userId);
        Task<LoggedinUser> GetLoggedInUser(string userId, int projectId, int platformId);
        Task DeleteUser(LoggedinUser user, int primaryid);
        Task DeleteLoggedInUser(LoggedinUser user);
        UserDetails GetUserDetails(string userId);
        IEnumerable<UserDetails> GetLISTUserDetails(List<string> userId);
        List<UserDetails> GetUserDetails();
        LinkAccount GetLinkAccount(int userId);
        List<LinkAccount> GetallLinkAccount(int userId);
        void deleteLinkAccount(List<LinkAccount> LinkAccount);
        void Deletelistoftags(List<listoftags> listoftags);
        listoftags Getlistoftags(int userId);
        List<listoftags> Getalllistoftags(int userId);
        List<WhatBestDescripsMeList> GetallWhatBestDescripsMeList(int userId);
        List<Iprefertolist> GetallIprefertolist(int userId);
        void UpdateUserDetails(UserDetails userDetails);
        void UpdateLinkAccount(LinkAccount LinkAccount);

        bool checktooken(string tooken);
        Task glob();
        void addlistoftags(listoftags listoftags);
        void addWhatBestDescripsMe(WhatBestDescripsMeList WhatBestDescripsMe);
        void addIprefertolist(Iprefertolist Iprefertolist);
        void Updatelistoftags(listoftags listoftags);
        List<UserDetails> allusersaroundevent(double myLat, double myLon);
        public (List<UserDetails> userDetails, List<int> currentUserInterests) allusersdirection(double myLat, double myLon, string usertype, UserDetails userid, double degree, AppConfigrationVM AppConfigrationVM, bool sortByInterestMatch);
        public bool CHECKEVENTLOCATION(String myLat, String myLon, String EVENTLat, String EVENTLon, AppConfigrationVM AppConfigrationVM);


        EventData allEventDataaroundevent(int userid, double myLat, double myLon);
        (double, double) newetnewlocation(double Latitude, double Longitude);
        (List<UserDetails> userDetails, List<int> currentUserInterests) allusers(double myLat, double myLon, string usertype, UserDetails userid, AppConfigrationVM AppConfigrationVM, bool sortByInterestMatch);

        Task<(RecommendedPeopleViewModel, string)> RecommendedPeople(UserDetails userDeatil, string userId);
        Task<(RecommendedPeopleViewModel, string)> RecommendedPeopleFix(UserDetails userDeatil, string userId); 
        Task<(List<RecentlyConnectedViewModel>, string ,int)> RecentlyConnected(UserDetails userDeatil, int pageNumber, int pageSize);

        #region Public
        Task<List<UserDetails>> PublicAllUsersDirection(double myLat, double myLon, double degree, AppConfigrationVM AppConfigrationVM);
        Task<List<UserDetails>> PublicAllUsers(double myLat, double myLon, AppConfigrationVM AppConfigrationVM);
        Task ResetPassword(string userId);
        Task<string> LinkClicks(LoggedinUser loggedinUser, string key);
        #endregion

    }
}
