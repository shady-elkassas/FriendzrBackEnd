using Social.Entity.ModelView;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Social.Services.FireBase_Helper
{
    public interface IFirebaseManager
    {
        Task<bool> SendNotification(List<string> clientTokens, FireBaseData fireBaseData, string ImageUrl = null);
        Task<bool> SendNotification(string clientToken, FireBaseData fireBaseData, string ImageUrl = null);
        Task<bool> SubscribeToTopicAsync(string topic, string[] clientTokens);
        Task<bool> SubscribeToTopicAsync(string topic, string clientToken);
        Task<bool> UnsubscribeFromTopicAsync(string topic, string[] clientTokens);
        Task<bool> UnsubscribeFromTopicAsync(string topic, string clientToken);
        Task RemoveToken(string token);
        Task<bool> SaveUserToken(string token, int UserID);
        IEnumerable<string> GetTokens(int UserID);
        IEnumerable<string> GetTokens();
        Task<bool> SendNotificationToTopic(string topic, string title, string body, string ImageUrl = null);
    }

}
