using CRM.Services.Wrappers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Social.Entity.DBContext;
using Social.Entity.Enums;
using Social.Entity.Models;
using Social.Entity.ModelView;
using Social.Sercices;
using Social.Services.FireBase_Helper;
using Social.Services.Helpers;
using Social.Services.ModelView;
using Social.Services.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Services.Implementation
{
    public class ChatGroupService : IChatGroupService
    {
        private readonly AuthDBContext authDBContext;
        private readonly IFirebaseManager firebaseManager;
        private readonly IGlobalMethodsService globalMethodsService;
        private readonly IStringLocalizer<SharedResource> localizer;
        private readonly IEventServ _Event;
        private readonly IConfiguration _configuration;
        private readonly string BaseDomainUrl;

        public ChatGroupService(IEventServ Event, IConfiguration configuration, AuthDBContext authDBContext, IFirebaseManager firebaseManager, IGlobalMethodsService globalMethodsService, IStringLocalizer<SharedResource> _localizer)
        {
            _Event = Event;
            _configuration = configuration;
            this.authDBContext = authDBContext;
            this.firebaseManager = firebaseManager;
            this.globalMethodsService = globalMethodsService;
            localizer = _localizer;
            BaseDomainUrl = globalMethodsService.GetBaseDomain();
        }
        public async Task<ResponseModel<ChatGroupVM>> Create(User CurrentUser, ChatGroupVM VM)
        {
            try
            {
                var Validate = ValidateChatGroupCreate(CurrentUser.Id, VM);
                if (Validate.isValid == false)
                {
                    return new ResponseModel<ChatGroupVM>(StatusCodes.Status406NotAcceptable, false,
                           Validate.ErrorMessage, null);
                }
                var Obj = Converter(CurrentUser, VM);                
                Obj.Image = VM.Image == null? (await globalMethodsService.uploadFileAsync("/Images/", VM.Image_File)) ??"ChatGroupTemp.JPG":VM.Image;
                Obj.Image = VM.Image == null?"/Images/" + Obj.Image: Obj.Image;                
                await authDBContext.ChatGroups.AddAsync(Obj);
                await authDBContext.SaveChangesAsync();
                var returnedData = Converter(CurrentUser.Id, Obj);
                return new ResponseModel<ChatGroupVM>(StatusCodes.Status200OK, true,
                    localizer["SavedSuccessfully"], returnedData);
            }
            catch (Exception ex)
            {
                return new ResponseModel<ChatGroupVM>(StatusCodes.Status406NotAcceptable, false,
                       ex.Message, null);
            }
        }
        public async Task<ResponseModel<ChatGroupVM>> AddUsers(User CurrentUser, ChatGroupVM VM)
        {
            try
            {
                var chatgroup = await authDBContext.ChatGroups.FindAsync(VM.ID);
                var Validate = ValidateChatGroupAddUsers(CurrentUser, VM, chatgroup);
                if (Validate.isValid == false)
                {
                    return new ResponseModel<ChatGroupVM>(StatusCodes.Status406NotAcceptable, false,
                           Validate.ErrorMessage, null);
                }
                //Select New To Add
                var NewMembers = VM.ChatGroupSubscribers.Where(x => x.ChatGroupSubscriberType == ChatGroupSubscriberType.Member && chatgroup.Subscribers.Any(xx => xx.UserID == x.userId) == false).Select(x => new ChatGroupSubscribers
                {
                    ID = Guid.NewGuid(),
                    JoinDateTime = VM.RegistrationDateTime,
                    ChatGroupID = VM.ID,
                    UserID = x.userId,
                    LeaveGroup = ChatGroupSubscriberStatus.Joined,
                    IsAdminGroup = ChatGroupSubscriberType.Member,
                    IsMuted = false,
                    LeaveDateTime = null,
                    RemovedDateTime = null,
                }).ToList();
                //Updatedmembers
                chatgroup.Subscribers.ToList().Where(x => x.LeaveGroup != ChatGroupSubscriberStatus.Joined && x.IsAdminGroup != ChatGroupSubscriberType.Admin && VM.ChatGroupSubscribers.Any(xx => xx.userId == x.UserID))
                     .ToList().ForEach(x =>
                 {
                     x.JoinDateTime = VM.RegistrationDateTime;
                     x.ChatGroupID = VM.ID;
                     x.LeaveGroup = ChatGroupSubscriberStatus.Joined;
                     x.IsAdminGroup = ChatGroupSubscriberType.Member;
                     x.IsMuted = false;
                     x.LeaveDateTime = null;
                     x.RemovedDateTime = null;
                 });

                authDBContext.ChatGroups.UpdateRange(chatgroup);
                await authDBContext.SaveChangesAsync();
                await authDBContext.ChatGroupSubscribers.AddRangeAsync(NewMembers);
                await authDBContext.SaveChangesAsync();
                var returnedData = Converter(CurrentUser.Id, (await authDBContext.ChatGroups.FindAsync(VM.ID)));

                return new ResponseModel<ChatGroupVM>(StatusCodes.Status200OK, true,
            localizer["AddedSuccessfully"], returnedData);
            }
            catch (Exception ex)
            {
                return new ResponseModel<ChatGroupVM>(StatusCodes.Status406NotAcceptable, false,
                       "You Can Not Add USer Now Try again", null);
            }
        }
        public async Task<(List<ChatGroupSubscribersVM> subscribers, ResponseModel<PagedResponse<List<MessagedataVM>>> pagedResponse)> GetChat(User CurrentUser, Guid ChatGroupID, int PageNumber, int PageSize, string Search)
        {
            try
            {
                var chatgroup = await authDBContext.ChatGroups.FindAsync(ChatGroupID);
                //var currentUserDetails= -authDBContext.UserDetails
                var CurrentUsersubscribtion = await authDBContext.ChatGroupSubscribers.FirstOrDefaultAsync(x => x.ChatGroupID == ChatGroupID && x.UserID == CurrentUser.Id);
                if (CurrentUsersubscribtion == null)
                {
                    return (null, new ResponseModel<PagedResponse<List<MessagedataVM>>>(StatusCodes.Status406NotAcceptable, false,
                           "UserNeverJoinedToChatBefor", null));
                }
                var allmessages = await UserChatGroupMessages(CurrentUser, ChatGroupID, CurrentUser.Id);
                allmessages = allmessages.Where(x => (Search == null || Search == "" || Microsoft.EntityFrameworkCore.EF.Functions.Like(x.Messages.ToLower().Trim(), $"%{Search.ToLower().Trim()}%")));
                var validFilter = new PaginationFilter(PageNumber, PageSize);
                var pagedMessages = allmessages.Where(n=>n.Messagetype == 4 ?( n.EventData.eventdateto.Value >= DateTime.Now):true).OrderByDescending(x => x.Messagesdate).ThenByDescending(x => x.Messagestime).Skip((validFilter.PageNumber - 1) * validFilter.PageSize).Take(validFilter.PageSize).ToList();
                var allateend = _Event.allattendevent().ToList();
                var data = pagedMessages.Select(item => new MessagedataVM
                {
                    currentuserMessage = item.UserId == CurrentUser.UserDetails.PrimaryId,
                    Messages = item.Messages,
                    Longitude = item.Longitude,
                    Latitude = item.Latitude,
                    Messagesdate = item.Messagesdate.ConvertDateTimeToString(),
                    Messagestime = item.Messagestime.ToString(@"hh\:mm"),
                    Username = item.User.userName,
                    Userimage = BaseDomainUrl + item.User.UserImage,
                    UserId = item.User.UserId,
                    Id = item.Id,
                    Messagetype = item.Messagetype,
                    linkable = item.linkable,
                    IsWhitelabel=CurrentUser.UserDetails.IsWhiteLabel.HasValue? CurrentUser.UserDetails.IsWhiteLabel.Value:false,
                    EventLINKid = item.Messagetype == 4 ? item.EventData.EntityId : null,
                    EventData = (item.Messagetype == 4 && item.EventData.eventdateto.Value >= DateTime.Now) ? (new EventDatalinka
                    {
                        eventdate = item.EventData.eventdate.Value.ConvertDateTimeToString(),

                        eventdateto = item.EventData.eventdateto.Value.ConvertDateTimeToString(),
                        MyEvent = item.EventData.UserId == CurrentUser.UserDetails.PrimaryId ? true : false,
                        categorie = item.EventData.categorie?.name,
                        categorieimage = _configuration["BaseUrl"] + item.EventData.categorie?.image,
                        Title = item.EventData.Title,
                        Image = (item.EventData.EventTypeListid != 3 ? globalMethodsService.GetBaseDomain() : "" )+ item.EventData.image,
                        Id = item.EventData.EntityId,
                        key = item.UserId == CurrentUser.UserDetails.PrimaryId ? 1 : (_Event.GetEventattend(allateend, item.EventData.EntityId, CurrentUser.UserDetails.PrimaryId).type == true ? 2 : 3),
                        eventtypeid = item.EventData.EventTypeList?.entityID,
                        eventtypecolor = item.EventData.EventTypeList?.color,
                        eventtype = item.EventData.EventTypeList?.Name,
                        totalnumbert = item.EventData.totalnumbert,
                        //interests = _Event.GetINterestdata(m.Id).Distinct(),
                        joined = _Event.GetEventattend(allateend, item.EventData.EntityId, CurrentUser.UserDetails.PrimaryId).count,
                    }) : null,
                    MessageAttachedVM = item.MessagesAttached == null || item.MessagesAttached == "" ? null :
                    new List<MessageAttachedVM>() {
                        new MessageAttachedVM { attached=BaseDomainUrl+item.MessagesAttached}}
                }).ToList();
                CurrentUsersubscribtion.UserNotreadcount = 0;
                authDBContext.ChatGroupSubscribers.Update(CurrentUsersubscribtion);
                authDBContext.SaveChanges();

                var pagedModel = new PagedResponse<List<MessagedataVM>>(data, PageNumber,
                            PageSize, allmessages.Count());
                return (Converter(CurrentUser.Id, chatgroup).ChatGroupSubscribers, new ResponseModel<PagedResponse<List<MessagedataVM>>>(StatusCodes.Status200OK, true,
                            "chat groupdata", pagedModel));
            }
            catch (Exception ex)
            {
                return (null, new ResponseModel<PagedResponse<List<MessagedataVM>>>(StatusCodes.Status406NotAcceptable, false, ex.Message, null));

            }
        }

        public async Task<ResponseModel<PagedResponse<List<UserDetailsvm>>>> GetAllChats(User CurrentUser, int PageNumber, int PageSize, string Search)
        {
            try
            {
                var basedomain = globalMethodsService.GetBaseDomain();
                var chatgroup = new List<UserDetailsvm>();
                var chatgrouplist = authDBContext.ChatGroups.Where(chat => chat.Subscribers.Any(x => (Search == null || Search == "" || Microsoft.EntityFrameworkCore.EF.Functions.Like(x.ChatGroup.Name.ToLower().Trim(), $"%{Search.ToLower().Trim()}%")) && x.UserID == CurrentUser.Id)).ToList();
                foreach (var chatgroupObj in chatgrouplist)
                {
                    var chatgroupusersettings = chatgroupObj.Subscribers.First(x => x.UserID == CurrentUser.Id);
                    if (chatgroupusersettings.LeaveGroup != ChatGroupSubscriberStatus.Joined && chatgroupusersettings.ClearChatDateTime != null)
                        continue;
                    {
                        //var validamessagedata= chatgroupObj.Messagedatas.Where(x=> chatgroupusersettings.Status)
                        var messagedata = await (await UserChatGroupMessages(CurrentUser, chatgroupObj.ID, CurrentUser.Id)).OrderByDescending(x => x.Messagesdate).ThenByDescending(x => x.Messagestime).FirstOrDefaultAsync();
                        chatgroup.Add(new UserDetailsvm
                        {
                            isChatGroup = true,
                            Name = chatgroupObj.Name,
                            Image = basedomain + chatgroupObj.Image,
                            id = chatgroupObj.ID.ToString(),
                            muit = chatgroupusersettings.IsMuted,
                            LeaveGroup = (int)chatgroupusersettings.LeaveGroup,
                            Leavevent = 0,
                            IsChatGroupAdmin = chatgroupusersettings.IsAdminGroup == ChatGroupSubscriberType.Admin,
                            latestdate = messagedata == null ? chatgroupObj.RegistrationDateTime.ConvertDateTimeToString() : messagedata.Messagesdate.ConvertDateTimeToString(),
                            latesttime = messagedata == null ? chatgroupObj.RegistrationDateTime.ToString(@"hh\:mm") : messagedata.Messagestime.ToString(@"hh\:mm"),
                            messagestype = messagedata == null ? ((int)Messagetype.Text) : messagedata.Messagetype,
                            messagesimage = messagedata == null ? null : messagedata.MessagesAttached,
                            messages = messagedata == null ? "" : messagedata.Messages,
                            latestdatevm = messagedata == null ? chatgroupObj.RegistrationDateTime : messagedata.Messagesdate,
                            latesttimevm = messagedata == null ? chatgroupObj.RegistrationDateTime.TimeOfDay : messagedata.Messagestime,

                        });
                    }
                }



                var validFilter = new PaginationFilter(PageNumber, PageSize);
                var pagedMessages = chatgroup.OrderByDescending(m => m.latestdatevm).ThenByDescending(m => m.latesttimevm).Skip((validFilter.PageNumber - 1) * validFilter.PageSize).Take(validFilter.PageSize).ToList();

                var pagedModel = new PagedResponse<List<UserDetailsvm>>(pagedMessages, validFilter.PageNumber,
                            pagedMessages.Count(), chatgroup.Count());
                return new ResponseModel<PagedResponse<List<UserDetailsvm>>>(StatusCodes.Status200OK, true,
                            "chat groupdata", pagedModel);
            }
            catch (Exception ex)
            {
                return new ResponseModel<PagedResponse<List<UserDetailsvm>>>(StatusCodes.Status500InternalServerError, false,
                            ex.Message, null);

            }
        }

        public async Task<IQueryable<Messagedata>> UserChatGroupMessages(User CurrentUser, Guid ChatGroupID, string UserID)
        {
            var CurrentUsersubscribtion = await authDBContext.ChatGroupSubscribers.FirstOrDefaultAsync(x => x.ChatGroupID == ChatGroupID && x.UserID == UserID);
            var DbF = Microsoft.EntityFrameworkCore.EF.Functions;
            var allmessages = authDBContext.Messagedata.Where(x => x.ChatGroupID == ChatGroupID);
            //get message after koinedate 
            allmessages = allmessages.Where(x => ((DbF.DateDiffDay(CurrentUsersubscribtion.JoinDateTime.Date, x.Messagesdate) > 0) || ((DbF.DateDiffDay(CurrentUsersubscribtion.JoinDateTime.Date, x.Messagesdate) == 0 && (DbF.DateDiffSecond(CurrentUsersubscribtion.JoinDateTime.TimeOfDay, x.Messagestime) > 0)))));

            if (CurrentUsersubscribtion.LeaveDateTime != null)
            {
                allmessages = allmessages.Where(x => (DbF.DateDiffDay(CurrentUsersubscribtion.LeaveDateTime.Value.Date, x.Messagesdate) < 0) || ((DbF.DateDiffDay(CurrentUsersubscribtion.LeaveDateTime.Value.Date, x.Messagesdate) == 0 && (DbF.DateDiffSecond(CurrentUsersubscribtion.LeaveDateTime.Value.TimeOfDay, x.Messagestime) < 0))));
            }
            if (CurrentUsersubscribtion.RemovedDateTime != null)
            {
                allmessages = allmessages.Where(x => (DbF.DateDiffDay(CurrentUsersubscribtion.RemovedDateTime.Value.Date, x.Messagesdate) < 0) || ((DbF.DateDiffDay(CurrentUsersubscribtion.RemovedDateTime.Value.Date, x.Messagesdate) == 0 && (DbF.DateDiffSecond(CurrentUsersubscribtion.RemovedDateTime.Value.TimeOfDay, x.Messagestime) < 0))));
            }
            if (CurrentUsersubscribtion.ClearChatDateTime != null)
            {

                allmessages = allmessages.Where(x => (DbF.DateDiffDay(CurrentUsersubscribtion.ClearChatDateTime.Value.Date, x.Messagesdate) > 0) || ((DbF.DateDiffDay(CurrentUsersubscribtion.ClearChatDateTime.Value.Date, x.Messagesdate) == 0 && (DbF.DateDiffSecond(CurrentUsersubscribtion.ClearChatDateTime.Value.TimeOfDay, x.Messagestime) > 0))));
            }
            return allmessages;
        }
        public async Task<ResponseModel<ChatGroupSendMessageVM>> SendMessage(User CurrentUser, ChatGroupSendMessageVM VM)
        {
            try
            {
                var validate = ValidateChatGroupMessage(CurrentUser, VM);
                if (validate.isValid == false)
                {
                    return new ResponseModel<ChatGroupSendMessageVM>(StatusCodes.Status406NotAcceptable, false, validate.ErrorMessage, null);
                }
                var Attach = VM.MessageType == Messagetype.Text ? null : (await globalMethodsService.uploadFileAsync("/Images/Messagedata/", VM.Attach_File));
                var messagedata = new Messagedata
                {
                    ChatGroupID = VM.ChatGroupID,
                    Id = Guid.NewGuid().ToString(),
                    //Messagesdate = VM.MessagesDateTime.Date, Old
                    //TODO: Change Date Time !!!!!!!!!! Abdelrahman 
                    Messagesdate = /*DateTime.Now.Date*/VM.MessagesDateTime.Value.Date,
                    //TODO: Change Date Time !!!!!!!!!! Abdelrahman 
                    //Messagestime = VM.MessagesDateTime.TimeOfDay,
                    Messagestime = /*DateTime.Now.TimeOfDay*/VM.MessagesDateTime.Value.TimeOfDay,
                    Messages = VM.MessageType == Messagetype.Text ? VM.Message : null,
                    Latitude = VM.Latitude ,
                    Longitude = VM.Latitude ,
                    Messagetype = (int)VM.MessageType,
                    MessagesAttached = "/Images/Messagedata/" + Attach,
                    linkable = VM.linkable,
                    EventDataid = geteventMessages(VM.EventLINKid),
                    UserId = CurrentUser.UserDetails.PrimaryId,
                };
                await authDBContext.Messagedata.AddAsync(messagedata);
                
                await authDBContext.SaveChangesAsync();

                VM.Attach = VM.MessageType == Messagetype.Text ? null : _configuration["BaseUrl"] + messagedata.MessagesAttached;
                //var currentusertype = messagedata.ChatGroup.Subscribers.FirstOrDefault(x => x.UserID == CurrentUser.Id)?.IsAdminGroup;
                var eventdata = _Event.getevent(VM.EventLINKid).FirstOrDefault();
                var allateend = _Event.allattendevent().ToList();
                var allSubscribers = messagedata.ChatGroup.Subscribers.Where(x => x.UserID != CurrentUser.Id).ToList();
                allSubscribers.ForEach(n => n.UserNotreadcount += 1);
                authDBContext.UpdateRange(allSubscribers);
                authDBContext.SaveChanges();
                foreach (var item in allSubscribers)
                {

                    FireBaseData fireBaseInfo = new FireBaseData()
                    {
                        Messagetype = (int)VM.MessageType,
                        //date = VM.MessagesDateTime.ConvertDateTimeToString(),
                        //time = VM.MessagesDateTime.ToString(@"hh\:mm"),
                        //Commented By Me (Abdelrahman Date & Time Must From Back_end)
                        date = DateTime.Now.ConvertDateTimeToString(),
                        time = DateTime.Now.ToString(@"hh\:mm"),
                        userimage = VM.Attach,
                        name = item.User.DisplayedUserName,
                        imageUrl = BaseDomainUrl + item.User.UserDetails.UserImage,
                        Title = CurrentUser.UserDetails.userName + "@" + messagedata.ChatGroup.Name,
                        // Body = VM.Message + _configuration["BaseUrl"] + VM.Attach,
                        Body = VM.MessageType == Messagetype.Text ?  VM.Message:((int)VM.MessageType == 4 ? "Shared Event" : (((int)VM.MessageType == 2 ? "photo" : "file"))) ,
                        
                        // Body = VM.Message + (VM.Attach != null ? _configuration["BaseUrl"] + VM.Attach : ""),
                        muit = item.IsMuted,
                        Action_code = VM.ChatGroupID.ToString(),
                        Action = "user_chatGroup",
                        isAdmin = item.IsAdminGroup == ChatGroupSubscriberType.Admin,
                        messageId = VM.ChatGroupID.ToString(),
                        senderId = CurrentUser.Id,
                        messsageImageURL = VM.Attach,

                        senderImage = _configuration["BaseUrl"] + CurrentUser.UserDetails.UserImage,
                        eventtypeid = eventdata?.EventTypeList.entityID,
                        eventtypecolor = eventdata?.EventTypeList.color,
                        eventtype = eventdata?.EventTypeList.Name,
                        senderDisplayName = CurrentUser.DisplayedUserName,
                        messsageLinkEveneventdate = eventdata?.eventdate.Value.ConvertDateTimeToString(),

                        messsageLinkEveneventdateto = eventdata?.eventdateto.Value.ConvertDateTimeToString(),
                        messsageLinkEvenMyEvent = eventdata?.UserId == CurrentUser.UserDetails .PrimaryId ? true : false,
                        messsageLinkEvencategorie = eventdata?.categorie?.name,
                        messsageLinkEvencategorieimage = _configuration["BaseUrl"] + eventdata?.categorie?.image,
                       
                        messsageLinkEvenTitle = eventdata?.Title,
                        messsageLinkEvenImage = _configuration["BaseUrl"] + eventdata?.image,
                        messsageLinkEvenId = eventdata?.EntityId,
                        // EncryptedID = StringCipher.EncryptString(item.EventData.EntityId),
                        messsageLinkEvenkey = eventdata?.UserId == CurrentUser.UserDetails.PrimaryId ? 1 : (_Event.GetEventattend(allateend, eventdata?.EntityId, CurrentUser.UserDetails.PrimaryId).type == true ? 2 : 3),
                        //lang = item.EventData.lang,
                        //lat = item.EventData.lat,
                        messsageLinkEventotalnumbert = eventdata!=null?eventdata.totalnumbert:0,
                        
                        messsageLinkEvenjoined = _Event.GetEventattend(allateend, eventdata?.EntityId, CurrentUser.UserDetails.PrimaryId).count,
                    };
                    await firebaseManager.SendNotification(item.User.UserDetails?.FcmToken, fireBaseInfo);
                }

                return new ResponseModel<ChatGroupSendMessageVM>(StatusCodes.Status200OK, true,
                    localizer["SavedSuccessfully"], VM);
            }
            catch (Exception ex)
            {
                return new ResponseModel<ChatGroupSendMessageVM>(StatusCodes.Status406NotAcceptable, false,
                       ex.Message, null);
            }
        }

        public async Task<ResponseModel<ChatGroupVM>> Edit(User CurrentUser, ChatGroupVM VM)
        {
            try
            {
                var Obj = await authDBContext.ChatGroups.FindAsync(VM.ID);
                var Validate = ValidateChatGroupEdit(CurrentUser, VM, Obj);
                if (Validate.isValid == false)
                {
                    return new ResponseModel<ChatGroupVM>(StatusCodes.Status406NotAcceptable, false,
                           Validate.ErrorMessage, null);
                }
                Converter(VM, Obj);
                if (VM.Image_File != null)
                {
                    globalMethodsService.DeleteFiles(Obj.Image, "");
                    var image = "/Images/" + (await globalMethodsService.uploadFileAsync("/Images/", VM.Image_File));



                    Obj.Image = image;

                }

                authDBContext.ChatGroups.Update(Obj);
                await authDBContext.SaveChangesAsync();
                //var returnedData = Converter(Obj);
                return new ResponseModel<ChatGroupVM>(StatusCodes.Status200OK, true,
                    localizer["SavedSuccessfully"], null);
            }
            catch (Exception ex)
            {
                return new ResponseModel<ChatGroupVM>(StatusCodes.Status406NotAcceptable, false,
                       ex.Message, null);
            }
        }
        public async Task<ResponseModel<ChatGroupVM>> LeaveChatGroup(User CurrentUser, ChatGroupVM VM)
        {
            try
            {
                var Obj = authDBContext.ChatGroupSubscribers.FirstOrDefault(x => x.ChatGroupID == VM.ID && x.UserID == CurrentUser.Id && x.LeaveGroup == ChatGroupSubscriberStatus.Joined);

                var Validate = ValidateChatGroupLeave(VM, Obj);
                if (Validate.isValid == false)
                {
                    return new ResponseModel<ChatGroupVM>(StatusCodes.Status406NotAcceptable, false,
                           Validate.ErrorMessage, null);
                }
                Obj.LeaveGroup = ChatGroupSubscriberStatus.Leaved;
                Obj.LeaveDateTime = VM.RegistrationDateTime;
                Obj.IsAdminGroup = ChatGroupSubscriberType.Member;
                VM.Name = Obj.ChatGroup.Name;
                VM.LeaveGroup = 1;
                VM.Image = _configuration["BaseUrl"] + Obj.ChatGroup.Image;
                authDBContext.ChatGroupSubscribers.Update(Obj);
                await authDBContext.SaveChangesAsync();
                //var returnedData = Converter(Obj);
                return new ResponseModel<ChatGroupVM>(StatusCodes.Status200OK, true,
                    localizer["LeavedSuccessfully"], VM);
            }
            catch (Exception ex)
            {
                return new ResponseModel<ChatGroupVM>(StatusCodes.Status406NotAcceptable, false,
                       ex.Message, null);
            }
        }
        public async Task<ResponseModel<ChatGroupVM>> ClearChatGroup(User CurrentUser, ChatGroupVM VM)
        {
            try
            {
                var Obj = authDBContext.ChatGroupSubscribers.FirstOrDefault(x => x.ChatGroupID == VM.ID && x.UserID == CurrentUser.Id);

                var Validate = ValidateChatGroupLeave(VM, Obj);
                if (Validate.isValid == false)
                {
                    return new ResponseModel<ChatGroupVM>(StatusCodes.Status406NotAcceptable, false,
                           Validate.ErrorMessage, null);
                }
                Obj.ClearChatDateTime = VM.RegistrationDateTime;
                authDBContext.ChatGroupSubscribers.Update(Obj);
                await authDBContext.SaveChangesAsync();
                //var returnedData = Converter(Obj);
                return new ResponseModel<ChatGroupVM>(StatusCodes.Status200OK, true,
                    localizer["SavedSuccessfully"], null);
            }
            catch (Exception ex)
            {
                return new ResponseModel<ChatGroupVM>(StatusCodes.Status406NotAcceptable, false,
                       ex.Message, null);
            }
        }
        public async Task<ResponseModel<ChatGroupVM>> MuteChatGroup(User CurrentUser, ChatGroupVM VM)
        {
            try
            {
                var Obj = authDBContext.ChatGroupSubscribers.FirstOrDefault(x => x.ChatGroupID == VM.ID && x.UserID == CurrentUser.Id && x.LeaveGroup == ChatGroupSubscriberStatus.Joined);
                if (Obj == null)
                {
                    return new ResponseModel<ChatGroupVM>(StatusCodes.Status406NotAcceptable, false,
                           localizer["InValidChatGroupID"], null);
                }
                Obj.IsMuted = VM.IsMuted;
                authDBContext.ChatGroupSubscribers.Update(Obj);
                await authDBContext.SaveChangesAsync();
                //var returnedData = Converter(Obj);
                return new ResponseModel<ChatGroupVM>(StatusCodes.Status200OK, true,
                    localizer["SavedSuccessfully"], null);
            }
            catch (Exception ex)
            {
                return new ResponseModel<ChatGroupVM>(StatusCodes.Status406NotAcceptable, false,
                       ex.Message, null);
            }
        }
        public async Task<ResponseModel<ChatGroupVM>> kickOutUser(User CurrentUser, ChatGroupVM VM)
        {
            try
            {
                var Obj = await authDBContext.ChatGroups.FindAsync(VM.ID);

                var Validate = ValidateChatGroupkickOutUsers(CurrentUser, VM, Obj);
                if (Validate.isValid == false)
                {
                    return new ResponseModel<ChatGroupVM>(StatusCodes.Status406NotAcceptable, false,
                           Validate.ErrorMessage, null);
                }
                // UpdatedMembers =
                Obj.Subscribers.ToList().Where(x => x.LeaveGroup == ChatGroupSubscriberStatus.Joined && x.IsAdminGroup == ChatGroupSubscriberType.Member && VM.ChatGroupSubscribers.Any(xx => xx.userId == x.UserID)).ToList().ForEach(x =>
            {
                x.JoinDateTime = x.JoinDateTime;
                x.ChatGroupID = x.ChatGroupID;
                x.UserID = x.UserID;
                x.ID = x.ID;
                x.LeaveGroup = ChatGroupSubscriberStatus.KickedOut;
                x.IsAdminGroup = ChatGroupSubscriberType.Member;
                x.IsMuted = true;
                x.UserNotreadcount = 0;
                x.LeaveDateTime = null;
                x.RemovedDateTime = VM.RegistrationDateTime;
            });

                authDBContext.ChatGroups.UpdateRange(Obj);
                await authDBContext.SaveChangesAsync();
                var returnedData = Converter(CurrentUser.Id, (await authDBContext.ChatGroups.FindAsync(VM.ID)));
                return new ResponseModel<ChatGroupVM>(StatusCodes.Status200OK, true,
                    localizer["UsersSuccessfullyKickedOut"], returnedData);
            }
            catch (Exception ex)
            {
                return new ResponseModel<ChatGroupVM>(StatusCodes.Status406NotAcceptable, false,
                       ex.Message, null);
            }
        }
        public async Task<ResponseModel<ChatGroupVM>> Remove(User CurrentUser, Guid ID)
        {
            var Obj = await authDBContext.ChatGroups.FindAsync(ID);
            if (Obj == null || Obj.Subscribers.Any(x => x.IsAdminGroup == ChatGroupSubscriberType.Admin && x.UserID == CurrentUser.Id) == false)
            {
                return new ResponseModel<ChatGroupVM>(StatusCodes.Status406NotAcceptable, false, localizer["UserHavenoPermissionForthisAction"], null);
            }

            try
            {
                authDBContext.Messagedata.RemoveRange(Obj.Messagedatas);
                authDBContext.ChatGroupSubscribers.RemoveRange(Obj.Subscribers);
                authDBContext.ChatGroups.Remove(Obj);
                await authDBContext.SaveChangesAsync();
                return new ResponseModel<ChatGroupVM>(200, true, localizer["RemovedSuccessfully"], null);



            }
            catch
            {
                var related = DependencyValidator<ChatGroup>.GetDependenciesNames(Obj).ToList();
                var Message = string.Format(localizer["PlzDeleteDataRelatedWithObjForCompleteObjectRemove"],
                   Obj?.Name,
                    //localizer[Obj.GetType().BaseType.Name),
                    related.Select(x => localizer[x].Value).Aggregate((x, y) => x + "," + y));
                return new ResponseModel<ChatGroupVM>(403, false, Message, null);

            }
        }
        public async Task<ResponseModel<ChatGroupVM>> GetObj(User CurrentUser, Guid ID)
        {
            var Obj = await authDBContext.ChatGroups.FindAsync(ID);
            if (Obj == null || Obj.Subscribers.Any(x => x.LeaveGroup == ChatGroupSubscriberStatus.Joined && x.UserID == CurrentUser.Id) == false)
            {
                return new ResponseModel<ChatGroupVM>(StatusCodes.Status406NotAcceptable, false, localizer["UserHavenoPermissionForthisAction"], null);
            }
            try
            {
                var returnedData = Converter(CurrentUser.Id, Obj);

                return new ResponseModel<ChatGroupVM>(200, true, "get chat Group ", returnedData);

            }
            catch
            {
                var related = DependencyValidator<ChatGroup>.GetDependenciesNames(Obj).ToList();
                var Message = string.Format(localizer["PlzDeleteDataRelatedWithObjForCompleteObjectRemove"],
                   Obj?.Name,
                    //localizer[Obj.GetType().BaseType.Name),
                    related.Select(x => localizer[x].Value).Aggregate((x, y) => x + "," + y));
                return new ResponseModel<ChatGroupVM>(403, false, Message, null);

            }
        }
        #region HelperMethodes 

        ChatGroup Converter(User CurrentUser, ChatGroupVM model)
        {

            model.ChatGroupSubscribers.Add(new ChatGroupSubscribersVM()
            {
                joinDate = model.RegistrationDateTime,
                LeaveGroup = ChatGroupSubscriberStatus.Joined,
                userId = CurrentUser.Id,
                ChatGroupSubscriberType = ChatGroupSubscriberType.Admin,
                ClearChatDateTime = null
            });
            var Obj = new ChatGroup()
            {
                ID = model.ID == default(Guid) ? Guid.NewGuid() : model.ID,
                Name = model.Name,
                UserID = CurrentUser.Id,
                IsActive = true,
                RegistrationDateTime = model.RegistrationDateTime,
                Subscribers = model.ChatGroupSubscribers.Select(x => new ChatGroupSubscribers
                {
                    JoinDateTime = x.joinDate,
                    LeaveGroup = x.LeaveGroup,
                    IsAdminGroup = x.ChatGroupSubscriberType,
                    UserID = x.userId,
                }).ToList()
            };
            return Obj;
        }

        void Converter(ChatGroupVM model, ChatGroup Obj)
        {
            Obj.Name = model.Name ?? Obj.Name;

        }

        ChatGroupVM Converter(string CurrentUserID, ChatGroup model)
        {
            var groupsubscribcurrentuser = model.Subscribers.FirstOrDefault(x => x.UserID == CurrentUserID);
            var Obj = new ChatGroupVM
            {
                ID = model.ID,
                Name = model.Name,

                Image = model.Image == null ? null : BaseDomainUrl + model.Image,
                RegistrationDateTime = model.RegistrationDateTime,
                LeaveGroup = (int)(groupsubscribcurrentuser?.LeaveGroup ?? ChatGroupSubscriberStatus.Pending),
                ChatGroupSubscribers = model.Subscribers.ToList().Select(x => new ChatGroupSubscribersVM
                {
                    joinDate = x.JoinDateTime,
                    LeaveDateTime = x.LeaveDateTime,
                    RemovedDateTime = x.RemovedDateTime,
                    IsMuted = x.IsMuted,
                    ClearChatDateTime = x.ClearChatDateTime,
                    LeaveGroup = x.LeaveGroup,
                    ChatGroupSubscriberType = x.IsAdminGroup,
                    userId = x.UserID,
                    UserName = x.User?.DisplayedUserName ?? (authDBContext.Users.Find(x.UserID)).DisplayedUserName,
                    image = ((x.User?.UserDetails?.UserImage ?? (authDBContext.Users.Find(x.UserID)).UserDetails.UserImage) == null) ? null : (BaseDomainUrl) + (x.User?.UserDetails?.UserImage ?? (authDBContext.Users.Find(x.UserID)).UserDetails.UserImage),
                }).Where(x => x.LeaveGroup == ChatGroupSubscriberStatus.Joined).OrderByDescending(x => x.ChatGroupSubscriberType).ToList()
            };
            return Obj;
        }
        (bool isValid, string ErrorMessage) ValidateChatGroupCreate(string curentuserid, ChatGroupVM model)
        {
            bool isValid = false;
            string Message = "";
            if (string.IsNullOrEmpty(model.ListOfUserIDs) == false)
            {
                {
                    try
                    {
                        var list = JsonConvert.DeserializeObject<List<string>>(model.ListOfUserIDs);
                        model.ChatGroupSubscribers = list.Distinct().Select(x => new ChatGroupSubscribersVM
                        {
                            joinDate = model.RegistrationDateTime,
                            LeaveGroup = ChatGroupSubscriberStatus.Joined,
                            userId = x,
                            ChatGroupSubscriberType = ChatGroupSubscriberType.Member
                        }).ToList();
                        model.ChatGroupSubscribers.RemoveAll(x => x.userId == curentuserid);
                    }
                    catch
                    {
                        model.ChatGroupSubscribers = new List<ChatGroupSubscribersVM>();
                        Message = localizer["SomeSelectedUsersNotValid"];
                        return (isValid, Message);
                    }
                }
            }
            if (model == null)
            {
                Message = localizer["NotValidData"];
            }
            else if (model.Image_File?.ContentType.ToLower().Contains("image") == false)
            {
                Message = localizer["NotValidImage"];
            }
            else if (string.IsNullOrEmpty(model.Name))
            {
                Message = localizer["ChatGroupNameIsRequired"];
            }
            else if ((model.RegistrationDateTime.Date - DateTime.Today.Date).Days < -1)
            {
                Message = localizer["ChatGroupDateCanNotBeInPast"];
            }
            //Validate UsersIn Group
            else if (model.ChatGroupSubscribers.Count > 0 && model.ChatGroupSubscribers.Any(x => authDBContext.Users.Any(xx => xx.Id == x.userId)) == false)
            {
                Message = localizer["SomeSelectedUsersNotValid"];
            }
            else
            {
                isValid = true;
            }
            return (isValid, Message);
        }
        (bool isValid, string ErrorMessage) ValidateChatGroupEdit(User CurrentUser, ChatGroupVM model, ChatGroup chatGroup)
        {
            bool isValid = false;
            string Message = "";
            if (model == null)
            {
                Message = localizer["NotValidData"];
            }
            else if (model.ID == default(Guid) || chatGroup == null)
            {
                Message = localizer["ChatGroupIDNotMatchedWithAnyExistedGroup"];
            }
            else if (chatGroup.Subscribers.Any(x => x.UserID == CurrentUser.Id && x.IsAdminGroup == ChatGroupSubscriberType.Admin && x.LeaveGroup == ChatGroupSubscriberStatus.Joined) == false)
            {
                Message = localizer["UserHavenoPermissionForthisAction"];
            }

            else if (model.Image_File?.ContentType.ToLower().Contains("image") == false)
            {
                Message = localizer["NotValidImage"];
            }
            else if (string.IsNullOrEmpty(model.Name))
            {
                Message = localizer["ChatGroupNameIsRequired"];
            }
            else
            {
                isValid = true;
            }
            return (isValid, Message);
        }
        (bool isValid, string ErrorMessage) ValidateChatGroupLeave(ChatGroupVM model, ChatGroupSubscribers chatGroupSubscribers)
        {
            bool isValid = false;
            string Message = "";
            if (model == null)
            {
                Message = localizer["NotValidData"];
            }
            else if (model.ID == default(Guid) || chatGroupSubscribers == null)
            {
                Message = localizer["UserNotMemberOfGroup"];
            }

            //else if ((model.RegistrationDateTime - chatGroupSubscribers.JoinDateTime).TotalSeconds < 0)
            //{
            //    Message = localizer["DateCanNotLessThanJoinDate"];
            //}
            else
            {
                isValid = true;
            }
            return (isValid, Message);
        }
        (bool isValid, string ErrorMessage) ValidateChatGroupAddUsers(User CurrentUser, ChatGroupVM model, ChatGroup chatGroup)
        {
            bool isValid = false;
            string Message = "";
            if (string.IsNullOrEmpty(model?.ListOfUserIDs) == false)
            {
                {
                    try
                    {
                        var list = model.ListOfUserIDs.StartsWith("[") ? JsonConvert.DeserializeObject<List<string>>(model.ListOfUserIDs) : new List<string>() { model.ListOfUserIDs };
                        model.ChatGroupSubscribers = list.Distinct().Select(x => new ChatGroupSubscribersVM
                        {
                            joinDate = model.RegistrationDateTime,
                            LeaveGroup = ChatGroupSubscriberStatus.Joined,
                            userId = x,
                            LeaveDateTime = null,
                            RemovedDateTime = null,
                            ChatGroupSubscriberType = ChatGroupSubscriberType.Member
                        }).ToList();
                        //Remove all user already joined in group
                        //model.ChatGroupSubscribers.RemoveAll(x => chatGroup.Subscribers.Any(xx => xx.UserID == x.UserID && xx.Status == ChatGroupSubscriberStatus.Joined));
                    }
                    catch
                    {
                        model.ChatGroupSubscribers = new List<ChatGroupSubscribersVM>();
                        Message = localizer["SomeSelectedUsersNotValid"];
                        return (isValid, Message);
                    }
                }
            }
            else
            {
                Message = localizer["atleastselectoneusertoadd"];

            }
            if (model == null)
            {
                Message = localizer["NotValidData"];
            }
            else if(CurrentUser.UserDetails.IsWhiteLabel.Value)
            {
                var userIDs = model.ChatGroupSubscribers.Select(x => x.userId);
                var commonUsers= authDBContext.UserDetails.Where(u => userIDs.Contains(u.UserId));
                if(commonUsers.Any(x=>x.Code != CurrentUser.UserDetails.Code))
                {
                    Message = localizer["OneOfUsersNotInTheCommunity"];
                }
                isValid = true;
            }
            else if (model.ID == default(Guid) || chatGroup == null)
            {
                Message = localizer["ChatGroupIDNotMatchedWithAnyExistedGroup"];
            }
            else if (chatGroup.Subscribers.Any(x => x.UserID == CurrentUser.Id && x.IsAdminGroup == ChatGroupSubscriberType.Admin && x.LeaveGroup == ChatGroupSubscriberStatus.Joined) == false)
            {
                Message = localizer["UserHavenoPermissionForthisAction"];
            }
            else if (model.ChatGroupSubscribers == null || model.ChatGroupSubscribers.Count == 0 || model.ChatGroupSubscribers.Any(x => authDBContext.Users.Any(xx => xx.Id == x.userId)) == false)
            {
                Message = localizer["SomeSelectedUsersNotValid"];
            }
            else if ((model.RegistrationDateTime.Date - DateTime.Today.Date).Days < -1)
            {
                Message = localizer["AdduserDateCanNotBeInPast"];
            }
            else if ((model.RegistrationDateTime - chatGroup.RegistrationDateTime).TotalSeconds < 0)
            {
                Message = localizer["AdduserDateCanNotBeOlderThanGroupDate"];
            }            
            else
            {
                isValid = true;
            }
            return (isValid, Message);
        }
        (bool isValid, string ErrorMessage) ValidateChatGroupkickOutUsers(User CurrentUser, ChatGroupVM model, ChatGroup chatGroup)
        {
            bool isValid = false;
            string Message = "";
            if (string.IsNullOrEmpty(model?.ListOfUserIDs) == false)
            {
                {
                    try
                    {
                        var list = model.ListOfUserIDs.StartsWith("[") ? JsonConvert.DeserializeObject<List<string>>(model.ListOfUserIDs) : new List<string>() { model.ListOfUserIDs };
                        model.ChatGroupSubscribers = list.Distinct().Select(x => new ChatGroupSubscribersVM
                        {
                            joinDate = model.RegistrationDateTime,
                            LeaveGroup = ChatGroupSubscriberStatus.Joined,
                            userId = x,
                            LeaveDateTime = null,
                            RemovedDateTime = null,
                            ChatGroupSubscriberType = ChatGroupSubscriberType.Member
                        }).ToList();
                        //Remove all user already joined in group
                        //model.ChatGroupSubscribers.RemoveAll(x => chatGroup.Subscribers.Any(xx => xx.UserID == x.UserID && xx.Status == ChatGroupSubscriberStatus.Joined));
                    }
                    catch
                    {
                        model.ChatGroupSubscribers = new List<ChatGroupSubscribersVM>();
                        Message = localizer["SomeSelectedUsersNotValid"];
                        return (isValid, Message);
                    }
                }
            }
            else
            {
                Message = localizer["atleastselectoneusertocickout"];

            }
            if (model == null)
            {
                Message = localizer["NotValidData"];
            }
            else if (model.ID == default(Guid) || chatGroup == null)
            {
                Message = localizer["ChatGroupIDNotMatchedWithAnyExistedGroup"];
            }
            else if (chatGroup.Subscribers.Any(x => x.UserID == CurrentUser.Id && x.IsAdminGroup == ChatGroupSubscriberType.Admin && x.LeaveGroup == ChatGroupSubscriberStatus.Joined) == false)
            {
                Message = localizer["UserHavenoPermissionForthisAction"];
            }
            else if (model.ChatGroupSubscribers == null || model.ChatGroupSubscribers.Count == 0 || model.ChatGroupSubscribers.Any(x => authDBContext.Users.Any(xx => xx.Id == x.userId)) == false)
            {
                Message = localizer["SomeSelectedUsersNotValid"];
            }

            else if ((model.RegistrationDateTime - chatGroup.RegistrationDateTime).TotalSeconds < 0)
            {
                Message = localizer["ckickoutuserDateCanNotBeOlderThanGroupDate"];
            }
            else
            {
                isValid = true;
            }
            return (isValid, Message);
        }
        (bool isValid, string ErrorMessage) ValidateChatGroupMessage(User CurrentUser, ChatGroupSendMessageVM model)
        {
            bool isValid = false;
            string Message = "";
            var ChatGroup = authDBContext.ChatGroups.FirstOrDefault(x => x.ID == model.ChatGroupID && x.Subscribers.Any(xx => xx.UserID == CurrentUser.Id && xx.LeaveGroup == ChatGroupSubscriberStatus.Joined));
            if (model == null)
            {
                Message = localizer["NotValidData"];
            }
            else if (ChatGroup == null)
            {
                Message = localizer["CanNotSentMessageForThisGroup"];
            }
            else if (model.MessageType == Messagetype.Image && (model.Attach_File == null || model.Attach_File?.ContentType.ToLower().Contains("image") == false))
            {
                Message = localizer["MessageTypeRequiredImage"];
            }
            else if (model.MessageType == Messagetype.File && (model.Attach_File == null || model.Attach_File?.ContentType.ToLower().Contains("image") == true))
            {
                Message = localizer["MessageTypeRequiredFile"];
            }
            else if (model.MessageType == Messagetype.Text && string.IsNullOrEmpty(model.Message))
            {
                Message = localizer["MessageTypeRequiredtext"];
            }
            else if(!model.MessagesDateTime.HasValue)
            {
                Message = localizer["DateTimeCanNotBeNullOrEmpty"];
            }
            //Commented By Me (Abdelrahman Date & Time Must From Back_end)
            else if ((model.MessagesDateTime.Value.Date - DateTime.Today.Date).Days < -1)
            {
                Message = localizer["MessageDateCanNotBeInPast"];
            }
            else if (ChatGroup.RegistrationDateTime > model.MessagesDateTime)
            {
                Message = localizer["MessageDateCanNotBeLessThanGroupRegistrationTime"];
            }
            else if (model.ChatGroupID == default(Guid))
            {
                Message = localizer["ChatGroupIDRequired"];

            }
            else if (model.MessageType == Messagetype.Location && string.IsNullOrEmpty(model.Longitude) && string.IsNullOrEmpty(model.Latitude))
            {
                Message = "Location Is Required";

            }

            else
            {
                isValid = true;
            }
            return (isValid, Message);
        }


        #endregion
        public int? geteventMessages(string eventid)
        {

            var data = authDBContext.EventData.FirstOrDefault(n => (n.EntityId == eventid));
            return data?.Id;

        }
    }

}



