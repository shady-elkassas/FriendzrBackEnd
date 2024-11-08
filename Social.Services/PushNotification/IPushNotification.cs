﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Social.Services.PushNotification
{
    public interface IPushNotification
    {
        Task SendUpdateProfileNotificationAfter24H();
        Task SendUpdateProfileNotificationAfter72H();
        Task SendUpdateProfileNotificationHasNoPhoto24();
        Task SendUpdateProfileNotificationHasNoPhoto72();
        Task SendNotificationForWomenOnly();
        Task SendNotificationIfUserHasRequestsUnanswered();
        Task SendNotificationIfUserHasMessagesNotRead();
        Task SendNotificationIfToUserHasMessagesNotRead();
        Task SendWelcomeEmail();
        Task SendCompleteProfileEmail();
        Task SendWelcomeEmailForTest(string email);
        Task SendCompleteProfileEmailForTest(string email);
    }
}
