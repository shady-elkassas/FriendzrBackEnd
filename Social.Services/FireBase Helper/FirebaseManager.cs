using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Social.Entity.ModelView;
using Social.Services.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Social.Services.FireBase_Helper
{
    public class FirebaseManager : IFirebaseManager
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IGlobalMethodsService globalMethodsService;
        private readonly string DefaultLogoUrl;
        //private readonly string ExcecludeCurrentUserCondition;

        public FirebaseManager(IHttpContextAccessor httpContextAccessor, IGlobalMethodsService globalMethodsService)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.globalMethodsService = globalMethodsService;
            DefaultLogoUrl = globalMethodsService.GetBaseDomain() + "/Images/Logo.jpg";
            //ExcecludeCurrentUserCondition = $"!('{FirebaseTopics.UserID_.ToString() + int.Parse(httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value)}' in topics)";
        }

        public IEnumerable<string> GetTokens(int UserID)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetTokens()
        {
            throw new NotImplementedException();
        }

        public Task RemoveToken(string token)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SaveUserToken(string token, int UserID)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> SendNotification(List<string> clientTokens, FireBaseData fireBaseData, string ImageUrl = null)
        {
            try
            {
                ImageUrl = ImageUrl ?? fireBaseData.imageUrl;
                ImageUrl = string.IsNullOrEmpty(ImageUrl) ? DefaultLogoUrl : ImageUrl;
                var registrationTokens = clientTokens;
                var additionalData = new Dictionary<string, string>()
                 {
                     {"Action_code", fireBaseData.Action_code },
                     {"Action", fireBaseData.Action },
                     {"Messagetype", fireBaseData.Messagetype.ToString() },
                     {"muit", fireBaseData.muit.ToString()},

                     {"name", fireBaseData.name },
                     {"date", fireBaseData.date },

                     {"isAdmin", fireBaseData.isAdmin.ToString() },
                     {"time", fireBaseData.time},
                     {"messageId", fireBaseData.messageId },
                     {"senderId", fireBaseData.senderId },
                     {"senderImage", fireBaseData.senderImage },
                     {"senderDisplayName", fireBaseData.senderDisplayName},
                     {"messsageImageURL", fireBaseData.messsageImageURL },
                     {"messsageLinkEvenId", fireBaseData.messsageLinkEvenId },
                     {"messsageLinkEvenTitle", fireBaseData.messsageLinkEvenTitle },
                     {"messsageLinkEvenImage", fireBaseData.messsageLinkEvenImage },
                     {"messsageLinkEvencategorie", fireBaseData.messsageLinkEvencategorie},
                     {"messsageLinkEvenkey", fireBaseData.messsageLinkEvenkey.ToString() },
                     {"messsageLinkEvenjoined", fireBaseData.messsageLinkEvenjoined.ToString() },
                     {"messsageLinkEvencategorieimage", fireBaseData.messsageLinkEvencategorieimage },
                     {"messsageLinkEventotalnumbert", fireBaseData.messsageLinkEventotalnumbert.ToString() },
                     {"messsageLinkEveneventdateto", fireBaseData.messsageLinkEveneventdateto },
                     {"messsageLinkEvenMyEvent", fireBaseData.messsageLinkEvenMyEvent.ToString() },

                      {"Body", fireBaseData.Body },
                     {"Title", fireBaseData.Title },
                     {"ImageUrl", ImageUrl}
                 };
                if (!string.IsNullOrEmpty(fireBaseData.userimage))
                    additionalData.Add("userimage", fireBaseData.userimage);
                var message = new MulticastMessage()
                {
                    Tokens = registrationTokens,
                    Data = additionalData,
                    Apns = new ApnsConfig() { Aps = new Aps() { Sound = "default", Alert = new ApsAlert { Title = fireBaseData.Title, Body = fireBaseData.Body }, MutableContent = true } },
                    //  Notification = new Notification() { Body = fireBaseData.Body, Title = fireBaseData.Title, ImageUrl = ImageUrl }
                };

                //var response = await FirebaseMessaging.DefaultInstance.SendMulticastAsync(message).ConfigureAwait(true);
                var response = await FirebaseMessaging.DefaultInstance.SendMulticastAsync(message);
                var failedTokens = new List<string>();
                if (response.FailureCount > 0)
                {
                    for (var i = 0; i < response.Responses.Count; i++)
                    {
                        if (!response.Responses[i].IsSuccess)
                        {
                            // The order of responses corresponds to the order of the registration tokens.
                            failedTokens.Add(clientTokens[i]);
                        }
                    }
                    //RemoveToken(failedTokens);

                }
                await globalMethodsService.uploadFile("/CreatedLogJson/", JsonConvert.SerializeObject(new
                {
                    tokns = clientTokens,
                    FailureCount = response.FailureCount,
                    failedTokens = failedTokens,
                    SuccessCount = response.SuccessCount,
                    fireBaseData = fireBaseData,
                    Time = DateTime.Now
                }));


                return true;
            }
            catch (Exception ex)
            {
                await globalMethodsService.uploadFile("/CreatedLogJson/", JsonConvert.SerializeObject(new
                {
                    Exception = ex.Message,
                    innerException = ex.InnerException,
                    tokns = clientTokens,
                    Time = DateTime.Now

                }));
                return false;
            }
        }
        public async Task<bool> SendNotification(string clientToken, FireBaseData fireBaseData, string ImageUrl = null)
        {
            try
            {
                if (string.IsNullOrEmpty(clientToken))
                {
                    await globalMethodsService.uploadFile("/CreatedLogJson/", JsonConvert.SerializeObject(new
                    {
                        tokns = "No Fcm  token For User!",

                    }));
                    return false;
                }
                var response = await SendNotification(new List<string>() { clientToken }, fireBaseData, ImageUrl ?? fireBaseData.imageUrl);

                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> SendNotificationToTopic(string topic, string title, string body, string ImageUrl = null)
        {
            try
            {

                ImageUrl = string.IsNullOrEmpty(ImageUrl) ? DefaultLogoUrl : httpContextAccessor.HttpContext.Request.Scheme + "://" + httpContextAccessor.HttpContext.Request.Host.Value + ImageUrl;
                var message = new FirebaseAdmin.Messaging.Message()
                {
                    Data = new Dictionary<string, string>()
                       {
                        { "url","/"}//Onclick Notfication RedirectUrl
                       //{"title_ar", title},
                       //{"body_ar", body},
                       //      {"title_en", title},
                       //{"body_en", body},
                       },

                    //Condition = $"'{topic}' in topics && " + ExcecludeCurrentUserCondition,
                    Notification = new Notification() { Body = body, Title = title, ImageUrl = ImageUrl },
                };
                var response = await FirebaseMessaging.DefaultInstance.SendAsync(message).ConfigureAwait(true);


                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> SubscribeToTopicAsync(string topic, string[] clientTokens)
        {
            try
            {
                var response = await FirebaseMessaging.DefaultInstance.SubscribeToTopicAsync(clientTokens, topic).ConfigureAwait(true);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> SubscribeToTopicAsync(string topic, string clientToken)
        {
            try
            {
                var response = await SubscribeToTopicAsync(topic, new string[] { clientToken });
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> UnsubscribeFromTopicAsync(string topic, string[] clientTokens)
        {
            try
            {

                var response = await FirebaseMessaging.DefaultInstance.UnsubscribeFromTopicAsync(clientTokens, topic).ConfigureAwait(true);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> UnsubscribeFromTopicAsync(string topic, string clientToken)
        {
            try
            {
                var response = await UnsubscribeFromTopicAsync(topic, new string[] { clientToken });
                return true;
            }
            catch
            {
                return false;
            }
        }
        //public async Task<bool> SaveUserToken(string token, int UserID)
        //{
        //    var Repo = (IGenericService<, >)(httpContextAccessor.HttpContext.RequestServices.GetService(typeof(IGenericService<, >)));
        //    try
        //    {
        //        Repo.Add(new UsersFireBaseTokens() { token = token, UserID = UserID });
        //        await SubscribeToTopicAsync(FirebaseTopics.all.ToString(), token);
        //        _ = SubscribeToTopicAsync(FirebaseTopics.UserID_.ToString() + UserID, token);
        //        return true;
        //    }
        //    catch
        //    {

        //        return Repo.firstOrDefault(x => x.token == token) == null ? false : true;
        //    }
        //}
        //public async Task RemoveToken(string token)
        //{
        //    try
        //    {
        //        var Repo = (IGenericService<, UsersFireBaseTokens>)(httpContextAccessor.HttpContext.RequestServices.GetService(typeof(IGenericService<, UsersFireBaseTokens>)));
        //        Repo.Remove(Repo.firstOrDefault(x => x.token == token));
        //        var Result = await UnsubscribeFromTopicAsync(FirebaseTopics.all.ToString(), token);

        //    }
        //    catch
        //    {
        //    }
        //}
        //async void RemoveToken(IEnumerable<string> token)
        //{
        //    try
        //    {
        //        var Repo = (IGenericService<, UsersFireBaseTokens>)(httpContextAccessor.HttpContext.RequestServices.GetService(typeof(IGenericService<, UsersFireBaseTokens>)));
        //        Repo.RemoveRange(Repo.GetAll().Where(x => token.Any(xx => xx == x.token)));
        //        var Result = await UnsubscribeFromTopicAsync(FirebaseTopics.all.ToString(), token.ToArray());

        //    }
        //    catch
        //    {
        //    }
        //}
        //public IEnumerable<string> GetTokens(int UserID)
        //{
        //    var Repo = (IGenericService<, UsersFireBaseTokens>)(httpContextAccessor.HttpContext.RequestServices.GetService(typeof(IGenericService<, UsersFireBaseTokens>)));
        //    return Repo.GetAll().Where(x => x.UserID == UserID).Select(x => x.token);
        //}
        //public IEnumerable<string> GetTokens()
        //{
        //    var Repo = (IGenericService<, UsersFireBaseTokens>)(httpContextAccessor.HttpContext.RequestServices.GetService(typeof(IGenericService<, UsersFireBaseTokens>)));
        //    return Repo.GetAll().Select(x => x.token);
        //}
    }
}
