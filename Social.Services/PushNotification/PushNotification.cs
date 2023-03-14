using Microsoft.AspNetCore.Hosting;
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
using System.Net.Mail;
using System.Threading.Tasks;

namespace Social.Services.PushNotification
{
    public class PushNotification : IPushNotification
    {
        private readonly AuthDBContext _authContext;
        private readonly IMessageServes _messageServes;
        private readonly IConfiguration _configuration;
        private readonly IFirebaseManager _fireBaseManager;
        public IWebHostEnvironment _hostingEnvironment { get; }
        private string SenderMail = "Hello@friendzr.com";
        private string _pass = "asd@1234A";
        public PushNotification(AuthDBContext authContext,
            IMessageServes messageServes,
            IConfiguration configuration,
            IFirebaseManager fireBaseManager, IWebHostEnvironment hostingEnvironment)
        {
            _authContext = authContext;
            _messageServes = messageServes;
            _configuration = configuration;
            _fireBaseManager = fireBaseManager;
            _hostingEnvironment = hostingEnvironment;
        }

        #region Send Update Profile Notification
        public async Task SendUpdateProfileNotificationAfter24H()
        {
            try
            {
                var users = _authContext.UserDetails
                    .Include(u => u.User)
                    .Where(u => u.ProfileCompleted.Value == false
                                && EF.Functions
                                    .DateDiffDay(u.User.RegistrationDate, DateTime.Today) == 1)
                    .ToList();
                const string body = "You're almost there! Complete your profile to start connecting on Friendzr today.";
                const string action = "Edit_profile";

                await SendNotification(users, body, action);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public async Task SendUpdateProfileNotificationAfter72H()
        {
            try
            {
                var users = _authContext.UserDetails
                    .Include(u => u.User)
                    .Where(u => u.ProfileCompleted.Value == false
                                && EF.Functions
                                    .DateDiffDay(u.User.RegistrationDate, DateTime.Today) == 3)
                    .ToList();

                const string body = "Your profile is being viewed – complete your profile for a better chance of finding like-minded Friendzrs.";
                const string action = "Edit_profile";

                await SendNotification(users, body, action);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        #endregion

        #region Send Private Mode Notification For Women Only

        public async Task SendNotificationForWomenOnly()
        {
            try
            {
                var users = _authContext.UserDetails
                    .Include(u => u.User)
                    .Where(u => u.ghostmode == false
                                && u.Gender == "female"
                                && EF.Functions
                                    .DateDiffDay(u.User.RegistrationDate, DateTime.Today) == 5)
                    .ToList();

                const string body = "Only looking for female/male friends? Use Private Mode to hide your profile from anyone.";
                const string action = "Private_mode";

                await SendNotification(users, body, action);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        #endregion

        #region Send Notification If User Has Requests Unanswered

        public async Task SendNotificationIfUserHasRequestsUnanswered()
        {
            try
            {
                var usersRequestIds = _authContext.Requestes
                    .Include(a => a.User)
                    .Include(a => a.UserRequest)
                    .Where(a =>
                        a.status == 0
                        && EF.Functions.DateDiffDay(a.regestdata, DateTime.Today) == 3)
                    .Select(a => a.UserRequestId).Distinct().ToList();


                foreach (var requests in usersRequestIds.Select(id => _authContext.Requestes
                             .Include(a => a.User)
                             .Include(a => a.UserRequest)
                             .Where(a =>
                                 a.status == 0
                                 && a.UserRequestId == id
                                 && EF.Functions.DateDiffDay(a.regestdata, DateTime.Today) == 3)
                             .ToList()))
                {
                    switch (requests.Count())
                    {
                        case 1:
                        {
                            var users = new List<UserDetails>();
                            var userName = requests[0]?.User.userName;
                            var userRequest = requests[0]?.UserRequest;
                            users.Add(userRequest);
                            var body = $"{userName} sent you a friend request. Click here to view and connect.";
                            const string action = "Friend_Requests";
                            await SendNotification(users, body, action);
                            break;
                        }
                        default:
                        {
                            switch (requests.Count() > 1)
                            {
                                case true:
                                {
                                    var users = new List<UserDetails>();
                                    var requestsCount = requests.Count;
                                    var userRequest = requests[0]?.UserRequest;
                                    users.Add(userRequest);
                                    var body = $"You have {requestsCount} requests waiting...";
                                    const string action = "Friend_Requests";
                                    await SendNotification(users, body, action);
                                    break;
                                }
                            }

                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        #endregion

        #region Send Notification If User Has Messages NotRead

        public async Task SendNotificationIfUserHasMessagesNotRead()
        {
            try
            {
                var userIds = _authContext.UserMessages
                    .Include(a => a.User)
                    .Include(a => a.ToUser)
                    .Where(a =>
                        a.UserNotreadcount > 0
                        && EF.Functions.DateDiffDay(a.startedin, DateTime.Today) == 3)
                    .Select(a=>a.UserId).Distinct().ToList();


                foreach (var userMessages in userIds.Select(id => _authContext.UserMessages
                             .Include(a => a.User)
                             .Include(a => a.ToUser)
                             .Where(a =>
                                 a.UserId == id
                                 && a.UserNotreadcount > 0
                                 && EF.Functions.DateDiffDay(a.startedin, DateTime.Today) == 3)
                             .ToList()))
                {
                    switch (userMessages.Count)
                    {
                        case 1:
                        {
                            var users = new List<UserDetails>();
                            var userName = userMessages[0]?.ToUser.userName;
                            var userRequest = userMessages[0]?.User;
                            users.Add(userRequest);
                            var body = $"{userName} is waiting to hear from you. Click to read and reply to their message.";
                            const string action = "Inbox_chat";
                            await SendNotification(users, body, action);
                            break;
                        }
                        default:
                        {
                            switch (userMessages.Count >1)
                            {
                                case true:
                                {
                                    var users = new List<UserDetails>();
                                    var requestsCount = userMessages.Select(a => a.UserNotreadcount).Sum();
                                    var userRequest = userMessages[0]?.User;
                                    users.Add(userRequest);
                                    var body = $"You have {requestsCount} messages waiting...";
                                    const string action = "Inbox_chat";
                                    await SendNotification(users, body, action);
                                    break;
                                }
                            }

                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public async Task SendNotificationIfToUserHasMessagesNotRead()
        {
            try
            {
                var userIds = _authContext.UserMessages
                    .Include(a => a.User)
                    .Include(a => a.ToUser)
                    .Where(a =>
                        a.ToUserNotreadcount > 0
                        && EF.Functions.DateDiffDay(a.startedin, DateTime.Today) == 3)
                    .Select(a => a.ToUserId).Distinct().ToList();


                foreach (var userMessages in userIds.Select(id => _authContext.UserMessages
                             .Include(a => a.User)
                             .Include(a => a.ToUser)
                             .Where(a =>
                                 a.ToUserId == id
                                 && a.ToUserNotreadcount > 0
                                 && EF.Functions.DateDiffDay(a.startedin, DateTime.Today) == 3)
                             .ToList()))
                {
                    switch (userMessages.Count)
                    {
                        case 1:
                        {
                            var users = new List<UserDetails>();
                            var userName = userMessages[0]?.User.userName;
                            var userRequest = userMessages[0]?.ToUser;
                            users.Add(userRequest);
                            var body = $"{userName} is waiting to hear from you. Click to read and reply to their message.";
                            const string action = "Inbox_chat";
                            await SendNotification(users, body, action);
                            break;
                        }
                        default:
                        {
                            switch (userMessages.Count > 1)
                            {
                                case true:
                                {
                                    var users = new List<UserDetails>();
                                    var requestsCount = userMessages.Select(a => a.ToUserNotreadcount).Sum();
                                    var userRequest = userMessages[0]?.ToUser;
                                    users.Add(userRequest);
                                    var body = $"You have {requestsCount} messages waiting...";
                                    const string action = "Inbox_chat";
                                    await SendNotification(users, body, action);
                                    break;
                                }
                            }

                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }

        #endregion
        public async Task SendWelcomeEmailForTest(string email)
        {
            var path = _hostingEnvironment.WebRootPath + "/EmailTemplates/Welcome_Email.html";

            var template = await System.IO.File.ReadAllTextAsync(path); const string title = "Ready to get started?";

            await SendEmailJobs(email, title, template);

            
        }
        public async Task SendCompleteProfileEmailForTest(string email)
        {
            var path = _hostingEnvironment.WebRootPath + "/EmailTemplates/Complete_Profile.html";

            var template = await System.IO.File.ReadAllTextAsync(path); const string title = "Your new friends await";
            await SendEmailJobs(email, title, template);
        }
        public async Task SendWelcomeEmail()
        {
            var path = _hostingEnvironment.WebRootPath + "/EmailTemplates/Welcome_Email.html";

            var template =await System.IO.File.ReadAllTextAsync(path);
            const string title = "Ready to get started?";
            var users = _authContext.Users
                .Where(u =>
                    u.EmailConfirmed 
                    &&EF.Functions
                    .DateDiffDay(u.RegistrationDate, DateTime.Today) == 1)
                .ToList();
            foreach (var user in users)
            {
                await SendEmailJobs(user.Email, title, template);

            }
        }

        public async Task SendCompleteProfileEmail()
        {
            var path = _hostingEnvironment.WebRootPath + "/EmailTemplates/Complete_Profile.html";

            var template = await System.IO.File.ReadAllTextAsync(path);
            const string title = "Your new friends await";
            
            var users = _authContext.UserDetails
                .Include(u => u.User)
                .Where(u => u.ProfileCompleted.Value == false
                            && EF.Functions
                                .DateDiffDay(u.User.RegistrationDate, DateTime.Today) == 7)
                .ToList();
            foreach (var user in users)
            {
                await SendEmailJobs(user.Email, title, template);

            }
        }
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
        private async Task<bool> SendEmailJobs(string toEmailAddress, string title, string body)
        {
            var validEmail = toEmailAddress.Contains("@");
            if (!validEmail)
            {
               return false;
            }
            var m = new MailMessage();
            var sc = new System.Net.Mail.SmtpClient();
            m.From = new MailAddress(SenderMail);
            m.To.Add(toEmailAddress);
            m.Subject = title;
            m.Body = body;
            m.IsBodyHtml = true;
            sc.Host = "www.friendzsocialmedia.com";
            var str1 = "gmail.com";
            var str2 = SenderMail;
            if (str2.Contains(str1))
            {
                try
                {
                    sc.Port = 587;
                    sc.Credentials = new System.Net.NetworkCredential(SenderMail, _pass);
                    sc.EnableSsl = true;
                    sc.Send(m);
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            else
            {
                try
                {
                    sc.Port = 587;
                    sc.Credentials = new System.Net.NetworkCredential(SenderMail, _pass);
                    sc.EnableSsl = false;
                    sc.Send(m);
                    return true;

                }
                catch (Exception ex)
                {
                    return false;

                }
            }
        }
        #endregion
    }
}
