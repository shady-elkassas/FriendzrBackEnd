using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Social.Entity.DBContext;
using Social.Entity.Models;
using Social.Entity.ModelView;
using Social.Services.FireBase_Helper;
using Social.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Social.Services.PushNotification
{
    public class PushNotification : IPushNotification
    {
        private readonly AuthDBContext _authContext;
        private readonly IMessageServes _messageServes;
        private readonly IConfiguration _configuration;
        private readonly IFirebaseManager _fireBaseManager;
       
        public PushNotification(AuthDBContext authContext,
            IMessageServes messageServes,
            IConfiguration configuration,
            IFirebaseManager fireBaseManager)
        {
            _authContext = authContext;
            _messageServes = messageServes;
            _configuration = configuration;
            _fireBaseManager = fireBaseManager;
        }

        #region Send Update Profile Notification
        public async Task SendUpdateProfileNotificationAfter24H()
        {
            var users = _authContext.UserDetails
                .Include(u => u.User)
                .Where(u => u.ProfileCompleted.Value == false
                            && EF.Functions
                                .DateDiffDay(u.User.RegistrationDate, DateTime.Today) == 1)
                .ToList();
            const string body = "You're almost there!@Complete your profile to start connecting on Friendzr today";
            const string action = "Edit_profile";

            await SendNotification(users, body, action);
        }

        public async Task SendUpdateProfileNotificationAfter72H()
        {
            var users = _authContext.UserDetails
                .Include(u => u.User)
                .Where(u => u.ProfileCompleted.Value == false
                            && EF.Functions
                                .DateDiffDay(u.User.RegistrationDate, DateTime.Today) == 3)
                .ToList();

            const string body = "Your profile is being viewed @Complete your profile for a better chance of finding like-minded Friendzrs";
            const string action = "Edit_profile";

            await SendNotification(users, body, action);
        }

        #endregion

        #region Send Private Mode Notification For Women Only

        public async Task SendNotificationForWomenOnly()
        {
            var users = _authContext.UserDetails
                .Include(u => u.User)
                .Where(u => u.ghostmode == false
                            && u.Gender == "female"
                            && EF.Functions
                                .DateDiffDay(u.User.RegistrationDate, DateTime.Today) == 5)
                .ToList();

            const string body = "Only looking for [female/male] friends? @Use Private Mode to hide your profile";
            const string action = "Private_mode";

            await SendNotification(users, body, action);
        }

        #endregion

        #region Send Notification If User Has Requests Unanswered

        public async Task SendNotificationIfUserHasRequestsUnanswered()
        {
            //var requests = _authContext.Requestes
            //    .Include(a => a.User)
            //    .Include(a => a.UserRequest)
            //    .Where(a =>
            //        a.status == 0
            //        && EF.Functions.DateDiffDay(a.regestdata, DateTime.Today) == 3)
            //    .ToList();

            var requests = _authContext.Requestes
                .Include(a => a.User)
                .Include(a => a.UserRequest)
                .Where(a =>
                    a.Id == 17229).ToList();

            foreach (var request in requests)
            {
                var users = new List<UserDetails>();
                var userName = request?.User.userName;
                var userRequest = request?.UserRequest;
                users.Add(userRequest);
                var body = $"[{userName}] sent you a friend request.@Click here to view and connect";
                const string action = "Friend_Requests";
                await SendNotification(users, body, action);
            }

        }

        #endregion


        #region Private Functions
        private async Task SendNotification(List<UserDetails> users, string body, string action)
        {
            var tokens = users.Select(s=>s.FcmToken).ToList();
            var usersIds = users.Select(s=>s.PrimaryId).ToList();

            body = body.Replace("@", System.Environment.NewLine);

            var fireBaseInfo = new FireBaseData()
            {
                Title = "Friendzr",
                imageUrl = null,
                Body = body,
                Action = action
            };

             try
             {
                 await _fireBaseManager.SendNotification(tokens.Where(x => string.IsNullOrEmpty(x) == false).ToList(), fireBaseInfo);
                 var fireBaseDataModels = usersIds.Select(x => _messageServes.getFireBaseData(x, fireBaseInfo, DateTime.Today)).ToList();
                 await _messageServes.addFireBaseDatamodel(fireBaseDataModels);
                    
             }
             catch (Exception ex)
             {
                 Console.WriteLine(ex.Message); 
             }
            

        }
        #endregion
    }
}
