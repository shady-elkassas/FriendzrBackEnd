using System;
using System.Collections.Generic;
using System.Text;

namespace Social.Entity.Enums
{
    public enum ApisWhiteLabelAccess
    {
        SendMessage,
        GetChat,
        Deletchat,// reviewed
        muitchat,
        GetAllChats,
        Chatdata,
        Remove,
        MuteChatGroup,
        kickOutUser,//reviewed
        ClearChatGroup,
        UsersChat,
        getMyEvent,
        UsersinChat,
        NotificationData,
        SendChatGroupMessage,
        SendEventMessage,
        EventChat,
        SearshUsersinChat,
        getEventAttende,
        Clickoutevent,// reviewed
        Create,
        AllFriendes,        
        GetChatGroup,
        AddUsers,
        Update, // reviewed
        Leave,// reviewed
        Userprofil, // reviewed and edit
        getAllReportReasons, //reviewed
        sendReport,// reviewed
        RequestFriendStatus// edited
    }
}
