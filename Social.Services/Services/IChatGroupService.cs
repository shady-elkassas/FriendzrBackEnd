using CRM.Services.Wrappers;
using Social.Entity.Models;
using Social.Entity.ModelView;
using Social.Services.ModelView;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Services.Services
{
    public interface IChatGroupService
    {

    
        Task<ResponseModel<ChatGroupVM>> Create(User user, ChatGroupVM VM);
        Task<ResponseModel<ChatGroupVM>> AddUsers(User user, ChatGroupVM VM);
        Task<(List<ChatGroupSubscribersVM> subscribers, ResponseModel<PagedResponse<List<MessagedataVM>>> pagedResponse)> GetChat(User user,Guid ChatGroupID , int PageNumber,int PageSize,string Search);
        Task<ResponseModel<PagedResponse<List<UserDetailsvm>>>> GetAllChats(User user  , int PageNumber,int PageSize,string Search);
        Task<ResponseModel<ChatGroupSendMessageVM>> SendMessage(User user, ChatGroupSendMessageVM VM);
        Task<IQueryable<Messagedata>> UserChatGroupMessages(User user, Guid ChtGroupID, string UserID);
        Task<ResponseModel<ChatGroupVM>> Edit(User user, ChatGroupVM VM);
        Task<ResponseModel<ChatGroupVM>> GetObj(User user, Guid ID);
        Task<ResponseModel<ChatGroupVM>> Remove(User user, Guid ID);
        Task<ResponseModel<ChatGroupVM>> LeaveChatGroup(User user, ChatGroupVM VM);
        Task<ResponseModel<ChatGroupVM>> MuteChatGroup(User user, ChatGroupVM VM);
        Task<ResponseModel<ChatGroupVM>> ClearChatGroup(User user, ChatGroupVM VM);
        Task<ResponseModel<ChatGroupVM>> kickOutUser(User user, ChatGroupVM VM);

        //Task<ResponseModel<ChatGroupVM>> ToggleActiveConfigration(Guid ID, bool IsActive);
    }
}
