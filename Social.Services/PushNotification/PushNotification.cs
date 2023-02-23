using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Social.Entity.DBContext;
using Social.Entity.Models;
using Social.Services.FireBase_Helper;
using Social.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Social.Entity.ModelView;
using Social.FireBase;
using System.Linq.Dynamic.Core.Tokenizer;

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
            const string action = "Private _mode";

            await SendNotification(users, body, action);
        }

        #endregion

        #region Private Functions
        private async Task SendNotification(List<UserDetails> users, string body, string action)
        {
            body = body.Replace("@", System.Environment.NewLine);
            foreach (var user in users)
            {
                var fireBaseInfo = new FireBaseData()
                {
                    muit = false,
                    Title = "Friendzr",
                    imageUrl = _configuration["BaseUrl"] + user.UserImage,
                    Body = body,
                    Action_code = user.UserId,
                    Action = action
                };
                try
                {
                    if (user.FcmToken != null)
                    {
                        var result = await _fireBaseManager.SendNotification(user.FcmToken, fireBaseInfo);
                        if (result)
                        {
                            var messageSent =
                                _messageServes.getFireBaseData(user.PrimaryId, fireBaseInfo, DateTime.Today);
                            await _messageServes.addFireBaseDatamodel(messageSent);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

        }
        #endregion
    }
}
