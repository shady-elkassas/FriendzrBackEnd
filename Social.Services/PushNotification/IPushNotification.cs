using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Social.Services.PushNotification
{
    public interface IPushNotification
    {
        Task SendUpdateProfileNotificationAfter24H();
        Task SendUpdateProfileNotificationAfter72H();
        Task SendNotificationForWomenOnly();
        Task SendNotificationIfUserHasRequestsUnanswered();
        Task SendNotificationIfUserHasMessagesNotRead();
        Task SendNotificationIfToUserHasMessagesNotRead();
    }
}
