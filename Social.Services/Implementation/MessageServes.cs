using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Social.Entity.DBContext;
using Social.Entity.Enums;
using Social.Entity.Models;
using Social.Entity.ModelView;
using Social.Services.FireBase_Helper;
using Social.Services.Helpers;
using Social.Services.ModelView;
using Social.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Social.Services.Implementation
{
    public class MessageServes : IMessageServes
    {
        public AuthDBContext _authContext;
        private readonly IChatGroupService chatGroupService;
        private readonly IGlobalMethodsService globalMethodsService;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly IFirebaseManager firebaseManager;

        public MessageServes(IGlobalMethodsService globalMethodsService, IHttpContextAccessor httpContextAccessor, AuthDBContext authContext,
            IChatGroupService chatGroupService, IFirebaseManager firebaseManager, IConfiguration configuration)
        {
            this.globalMethodsService = globalMethodsService;
            this.httpContextAccessor = httpContextAccessor;
            this._authContext = authContext;
            this.chatGroupService = chatGroupService;
            this.firebaseManager = firebaseManager;
            _configuration = configuration;

        }
        public async Task<bool> CheckUserMessages(int currentuserid, int antherid)
        {

            var data = _authContext.UserMessages.Where(n => (n.ToUserId == currentuserid && n.UserId == antherid) || (n.UserId == currentuserid && n.ToUserId == antherid)).FirstOrDefault();
            return data == null ? false : true;

        }
        public UserMessages GetUserMessages(string currentuserid, string antherid)
        {

            var data = _authContext.UserMessages.Where(n => (n.ToUser.UserId == currentuserid && n.User.UserId == antherid) || (n.User.UserId == currentuserid && n.ToUser.UserId == antherid)).FirstOrDefault();
            return data;

        }
        public async Task updateUserMessages(UserMessages UserMessages)
        {
            _authContext.UserMessages.Update(UserMessages);
            await _authContext.SaveChangesAsync();
        }
        public async Task<string> addUserMessages(UserMessages UserMessages, bool firstone = true)
        {
            UserMessages.Id = Guid.NewGuid().ToString();
            if (firstone != false)
            {
                if (UserMessages.ToUserId != (httpContextAccessor.HttpContext.GetUser().User.UserDetails.PrimaryId))
                {
                    UserMessages.ToUserNotreadcount += 1;
                }
                else
                {
                    UserMessages.UserNotreadcount += 1;
                }

            }
            _authContext.UserMessages.Add(UserMessages);
            var data = await _authContext.SaveChangesAsync();
            return UserMessages.Id;
        }
        //public async Task addEventChat(EventChat EventChat)
        //{

        //    _authContext.EventChat.Add(EventChat);
        //    var data = await _authContext.SaveChangesAsync();

        //}
        public async Task<string> addMessagedata(Messagedata Messagedata)
        {
            try
            {
                Messagedata.Id = Guid.NewGuid().ToString();
                _authContext.Messagedata.Add(Messagedata);
                var data = await _authContext.SaveChangesAsync();
                return Messagedata.Id;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public async Task deleteMessagedata(List<Messagedata> Messagedata)
        {

            _authContext.Messagedata.RemoveRange(Messagedata);
            await _authContext.SaveChangesAsync();

        }


        public bool CheckUserMessagescolor(int currentuserid, string messagedataid)
        {

            var data = _authContext.Messagedata.Where(n => (n.UserId == currentuserid) && (n.Id == messagedataid)).FirstOrDefault();
            return data == null ? false : true;

        }
        public UserMessages getUserMessages(int currentuserid, int antherid, bool message = false)
        {

            var data = _authContext.UserMessages.Where(n => (n.ToUserId == currentuserid && n.UserId == antherid) || (n.UserId == currentuserid && n.ToUserId == antherid)).FirstOrDefault();
            if (message && data != null)
            {
                if (data.ToUserId != currentuserid)
                {
                    data.ToUserNotreadcount += 1;
                }
                else
                {
                    data.UserNotreadcount += 1;
                }
                _authContext.SaveChanges();
            }
            return data;

        }
        public int? geteventMessages(string eventid)
        {

            var data = _authContext.EventData.FirstOrDefault(n => (n.EntityId == eventid));
            return data?.Id;

        }
        //public EventChat getEventChat(string eventid)
        //{

        //    var data = _authContext.EventChat.Include(m => m.EventData).Where(n => (n.EventData.EntityId == eventid)).FirstOrDefault();
        //    return data;

        //}
        public List<Messagedata> getallMessagedata(string UserMessagesid)
        {

            var data = this._authContext.Messagedata.Include(m => m.User).Include(m => m.UserMessagess).Where(n => n.UserMessagessId == UserMessagesid).ToList();

            return data;

        }
        public (int TotalCount, IQueryable<Messagedata> messagedatas) GetUserMessageData(string CurrentUserID, string OtherUserID, PaginationFilter validFilter)
        {
            int TotalCount = 0;           

            DbFunctions DbF = EF.Functions;

           var currentUser= _authContext.UserDetails.Where(u=>u.UserId== CurrentUserID).FirstOrDefault();
           var OtherUser = _authContext.UserDetails.Where(u => u.UserId == OtherUserID).FirstOrDefault();

            UserMessages UserMessagesObj = _authContext.UserMessages.Where(x => (x.ToUser.UserId == CurrentUserID && x.User.UserId == OtherUserID)
                                        || (x.User.UserId == CurrentUserID && x.ToUser.UserId == OtherUserID)).FirstOrDefault();

            if(UserMessagesObj == null && currentUser.IsWhiteLabel.HasValue && currentUser.IsWhiteLabel.Value)
            {
                return (TotalCount, _authContext.Messagedata.AsQueryable().Where(m=>m.UserId== currentUser.PrimaryId && m.UserMessagessId== OtherUserID ||
                m.UserId == OtherUser.PrimaryId && m.UserMessagessId == CurrentUserID));
            }
            string UserMessagesID = UserMessagesObj?.Id;

            if (UserMessagesObj?.ToUser.UserId == CurrentUserID)
            {
                UserMessagesObj.ToUserNotreadcount = 0;
            }
            else
            {
                UserMessagesObj.UserNotreadcount = 0;
            }

            _authContext.SaveChanges();

            DateTime? CurrentUserDeleteDate = UserMessagesObj?.delete == CurrentUserID ? UserMessagesObj.deletedate : (UserMessagesObj?.Todelete == CurrentUserID ? UserMessagesObj.Userdeletedate : null);

            TimeSpan? CurrentUserDeleteTime = UserMessagesObj?.delete == CurrentUserID ? UserMessagesObj.deleteTime : (UserMessagesObj?.Todelete == CurrentUserID ? UserMessagesObj.UserdeleteTime : null);

            IQueryable<Messagedata> messages = _authContext.Messagedata.AsQueryable().Where(x => UserMessagesID != null && x.EventChatAttendId == null && x.UserMessagessId == UserMessagesID);

            if (CurrentUserDeleteDate != null && CurrentUserDeleteTime != null)
            {
                messages = messages.Where(x => (DbF.DateDiffDay(CurrentUserDeleteDate, x.Messagesdate) > 0) || ((DbF.DateDiffDay(CurrentUserDeleteDate, x.Messagesdate) == 0 && (DbF.DateDiffSecond(CurrentUserDeleteTime, x.Messagestime) > 0))));
            }

            //DbF.t(x.Messagesdate) > System.Data.Entity.DbFunctions.TruncateTime(CurrentUserDeleteDate)|| (CurrentUserDeleteTime == null || x.Messagestime > CurrentUserDeleteTime))) );
            //messages = _authContext.Messagedata.Where(x => UserMessagesID!=null&& x.EventChatAttendId == null && x.UserMessagessId == UserMessagesID && (CurrentUserDeleteDate == null || x.Messagesdate >= CurrentUserDeleteDate) && (CurrentUserDeleteTime == null || x.Messagestime > CurrentUserDeleteTime));

            TotalCount = messages.Count();
            var xx = messages.OrderByDescending(m => m.Messagesdate.Date).ThenByDescending(k => k.Messagestime).ToList();

            messages = messages.OrderByDescending(m => m.Messagesdate.Date).ThenByDescending(k => k.Messagestime).Skip((validFilter.PageNumber - 1) * validFilter.PageSize).Take(validFilter.PageSize);

            return (TotalCount, messages.Where(n => n.Messagetype == 4 ? (n.EventData.eventdateto.Value >= DateTime.Now) : true));

        }
        public bool getUserMessages(string UserMessagesid, string userid)
        {
            var data = this._authContext.UserMessages.Where(n => n.Id == UserMessagesid && (n.muit == userid || n.Tomuit == userid)).FirstOrDefault();

            return data == null ? false : true;

        }

        public async Task<bool> UpdateLiveLocationMessageData(UpdateLiveLocationDto dto)
        {
            var data = _authContext.Messagedata.FirstOrDefault(a => a.Id == dto.Id);
            if (data == null) return false;
            data.LocationName = dto.LocationName;
            data.Latitude = dto.Latitude;
            data.Longitude = dto.Longitude;
            _authContext.Messagedata.Update(data);
            
            return await _authContext.SaveChangesAsync() >0;

        }

        public async Task<bool> StopLiveLocationMessageData(string id)
        {
            var data = _authContext.Messagedata
                .Include(a=>a.UserMessagess)
                .ThenInclude(a=>a.ToUser)
                .ThenInclude(a=>a.User)
                .FirstOrDefault(a => a.Id == id);
            if (data == null) return false;

            data.IsLiveLocation = false;
            _authContext.Messagedata.Update(data);
            if (data.UserMessagess?.ToUser == null) return await _authContext.SaveChangesAsync() > 0;
            try
            {
                var fireBaseInfo = new FireBaseData()
                {
                    imageUrl = _configuration["BaseUrl"] + data.User?.UserImage,
                    Title = data.User?.userName,
                    name = data.User?.userName,
                    Body = "stop location",
                    muit = getUserMessages(data.UserMessagessId, data.UserMessagess?.ToUserId.ToString()),
                    Action_code = data.UserId.ToString(),
                    Action = "user_chat",
                    messageId = data.Id,
                    senderId = data.User?.UserId,
                    senderImage = _configuration["BaseUrl"] + data.User?.UserImage,

                };
                await firebaseManager.SendNotification(data.UserMessagess?.ToUser?.FcmToken, fireBaseInfo);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return await _authContext.SaveChangesAsync() > 0;

        }

        public async Task<GetLiveLocationDto> GetLiveLocationMessageData(string id)
        {
            var data = await _authContext.Messagedata.FirstOrDefaultAsync(a => a.Id == id);
            if (data == null) return new GetLiveLocationDto();
            var result = new GetLiveLocationDto
            {
                Id = data.Id,
                LocationName = data.LocationName,
                Latitude = data.Latitude,
                Longitude = data.Longitude,
                LocationPeriod = data.LocationPeriod,
                LocationStartTime = data.LocationStartTime,
                LocationEndTime = data.LocationEndTime,
                IsLiveLocation = data.IsLiveLocation ?? false,
            };
            return result;

        }
        public bool getmuitMessages(string UserMessagesid, string userid, List<UserMessages> UserMessages)
        {
            var data = UserMessages.Where(n => n.Id == UserMessagesid && (n.muit == userid || n.Tomuit == userid)).FirstOrDefault();

            return data == null ? false : true;
        }
        public FireBaseDatamodel getFireBaseData(int userid, FireBaseData model, DateTime? date = null, TimeSpan? time = null, bool IsCreatedByAdmin = false)
        {
            FireBaseDatamodel mod = new FireBaseDatamodel();
            mod.id = Guid.NewGuid().ToString();
            mod.Title = model.Title;
            mod.Body = model.Body;
            mod.imageUrl = model.imageUrl;
            mod.Messagetype = model.Messagetype;
            mod.userid = userid;
            mod.Action_code = model.Action_code;
            mod.muit = model.muit;
            //public int Action_Id2 { get; set; }
            mod.Action = model.Action;
            mod.IsCreatedByAdmin = IsCreatedByAdmin;
            DateTime dateTime = DateTime.UtcNow;
            //dateTime = new DateTime(
            //    dateTime.Ticks - (dateTime.Ticks % TimeSpan.TicksPerSecond),
            //    dateTime.Kind
            //    );

            mod.CreatedAt = dateTime;
            //if (date != null)
            //{
            //    if (time != null)
            //    {
            //        date.Value.TimeOfDay.Add(time.Value);
            //    }
            //    mod.CreatedAt = date.Value;
            //}

            return mod;

        }
        public List<FireBaseDatamodel> getFireBaseDatamodel(int userid)
        {

            var data = this._authContext.FireBaseDatamodel.Where(m => m.userid == userid).ToList();
            //data.ForEach(n => { n.read = true; });
            ////_authContext.Attach(Obj);
            ////_authContext.Entry(Obj).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

            //     _authContext.SaveChanges();
            foreach (var item in data)
            {
                item.read = true;
            }
            _authContext.FireBaseDatamodel.UpdateRange(data);
            _authContext.SaveChanges();


            return data.Where(m => m.CreatedAt.AddDays(2) >= DateTime.Now.Date).ToList();

        }
        public int getFireBasecount(int userid)
        {

            var data = this._authContext.FireBaseDatamodel.Where(m => m.userid == userid && m.CreatedAt.AddDays(2) >= DateTime.Now.Date && m.read != true).ToList();


            return data.Count();

        }

        public async Task addFireBaseDatamodel(FireBaseDatamodel model)
        {

            await _authContext.FireBaseDatamodel.AddAsync(model);
            var data = await _authContext.SaveChangesAsync();

        }
        public async Task addFireBaseDatamodel(List<FireBaseDatamodel> model)
        {

            _authContext.FireBaseDatamodel.AddRange(model);
            var data = await _authContext.SaveChangesAsync();

        }
        public List<Messagedata> geteventMessagedata(string UserMessagesid)
        {

            var data = this._authContext.Messagedata.Where(n => n.Messagetype == 4 ? (n.EventData.eventdateto.Value >= DateTime.Now) : true).Where(n => n.EventChatAttend.EventData.EntityId == UserMessagesid).ToList();

            return data;

        }


        //public void eventchate(int currentuserid, string curen)
        //{
        //    //var user_cookie = http.HttpContext.Request.Cookies["User_Id"];

        //    DateTime HistoryDate = DateTime.Now;
        //    _authContext.Database.ExecuteSqlRaw("eventchate @p0, @p1", parameters: new[] { currentuserid.ToString());
        //}

        public List<UserDetailsvm> getalllUserinconect_2(int currentuserid, string curen)
        {

            var ReturnedListData = new List<UserDetailsvm>();
            var allUserEventsData = _authContext.EventChatAttend.Where(xx => xx.UserattendId == currentuserid).Select(x => x.EventDataid).ToList();
            var MessageDataList = this._authContext.Messagedata.Where(x =>
            // If user sent message
            x.UserId == currentuserid
              ||
             //if user  Resivemessage in usermessages
             (/*x.UserMessagessId != null &&*/ (x.UserMessagess.ToUserId == currentuserid || x.UserMessagess.UserId == currentuserid))

             ||
             // For eventMessages
             (/*x.EventChatAttendId != null && */
             allUserEventsData.Contains(x.EventChatAttend.EventDataid) && (x.EventChatAttend.UserattendId == currentuserid))).
             Where(x => (x.UserMessagessId != null) || (x.EventChatAttendId != null && ((x.EventChatAttend.leaveeventDate == null
             ||
             (x.EventChatAttend.leaveeventtime != null && x.Messagesdate <= x.EventChatAttend.leaveeventDate && x.Messagestime < x.EventChatAttend.leaveeventtime))
             || (x.EventChatAttend.leveeventchatDate == null
             || (x.EventChatAttend.leveeventchattime != null && x.Messagesdate <= x.EventChatAttend.leveeventchatDate && x.Messagestime < x.EventChatAttend.leveeventchattime))
             || (x.EventChatAttend.deletedate == null
             || (x.EventChatAttend.delettime != null && x.Messagesdate <= x.EventChatAttend.deletedate && x.Messagestime < x.EventChatAttend.delettime))
             || (x.EventChatAttend.deletechatDate == null
             || (x.EventChatAttend.deletechattime != null && x.Messagesdate <= x.EventChatAttend.deletechatDate && x.Messagestime >= x.EventChatAttend.deletechattime))))).ToList();
            var list = MessageDataList.OrderByDescending(x => x.Messagesdate).OrderByDescending(x => x.Messagestime).GroupBy(x => x.EventChatAttendId == null ? x.UserMessagessId : x.EventChatAttendId.ToString()).Select(item =>
            {
                var MessageDataObj = item.FirstOrDefault();
                var UserMessagesObj = item.FirstOrDefault()?.UserMessagess;
                if (UserMessagesObj != null)
                {


                    return new UserDetailsvm
                    {
                        Name = (UserMessagesObj.ToUserId == currentuserid ? UserMessagesObj.User.User.DisplayedUserName : UserMessagesObj.ToUser.User.DisplayedUserName),
                        Image = (UserMessagesObj.ToUserId == currentuserid ? UserMessagesObj.User.UserImage : UserMessagesObj.ToUser.UserImage),
                        id = UserMessagesObj.ToUserId == currentuserid ? UserMessagesObj.User.UserId : UserMessagesObj.ToUser.UserId,

                        //isfrind =item.Key.UserMessagess.ToUser. friendtype == null ? false : friendtype.status == 1 ? true : false,
                        isfrind = true,
                        muit = item.Any(xx => xx.UserId == currentuserid && (xx.UserMessagess.Tomuit == curen || xx.UserMessagess.muit == curen)),
                        latestdate = MessageDataObj.Messagesdate.ConvertDateTimeToString(),
                        latesttime = MessageDataObj.Messagestime.ToString(@"hh\:mm"),
                        latestdatevm = MessageDataObj.Messagesdate,
                        latesttimevm = MessageDataObj.Messagestime,

                        isevent = false,
                        messages = MessageDataObj.Messages,
                        messagestype = MessageDataObj.Messagetype,
                        messagesimage = MessageDataObj.MessagesAttached,
                    };
                }
                else
                {
                    var EventChatAttendObj = item.FirstOrDefault()?.EventChatAttend;
                    return new UserDetailsvm
                    {
                        Name = EventChatAttendObj.EventData.Title,
                        Image = EventChatAttendObj.EventData.image,
                        id = EventChatAttendObj.EventData.EntityId,
                        muit = EventChatAttendObj.muit,
                        Leavevent = EventChatAttendObj.stutus == 0 ? 0 : (EventChatAttendObj.stutus == 1 ? 1 : 2),
                        latestdate = MessageDataObj.Messagesdate.ConvertDateTimeToString(),
                        latesttime = MessageDataObj.Messagestime.ToString(@"hh\:mm"),
                        isevent = true,
                        myevent = EventChatAttendObj.EventData.UserId == currentuserid,
                        messagestype = MessageDataObj.Messagetype,
                        messagesimage = MessageDataObj.MessagesAttached,
                        messages = MessageDataObj.Messages,
                        latestdatevm = MessageDataObj.Messagesdate,
                        latesttimevm = MessageDataObj.Messagestime,

                    };
                }


            });

            ReturnedListData.AddRange(list);
            //await Task.Run(() =>
            //          {
            //              var Obj = MessageDataList.Where(x => x.EventChatAttendId != null && ((x.EventChatAttend.leaveeventDate == null || (x.EventChatAttend.leaveeventtime != null && x.Messagesdate <= x.EventChatAttend.leaveeventDate && x.Messagestime < x.EventChatAttend.leaveeventtime)) ||
            //           (x.EventChatAttend.leveeventchatDate == null || (x.EventChatAttend.leveeventchattime != null && x.Messagesdate <= x.EventChatAttend.leveeventchatDate && x.Messagestime < x.EventChatAttend.leveeventchattime)) ||
            //           (x.EventChatAttend.deletedate == null || (x.EventChatAttend.delettime != null && x.Messagesdate <= x.EventChatAttend.deletedate && x.Messagestime < x.EventChatAttend.delettime)) || (x.EventChatAttend.deletechatDate == null ||
            //           (x.EventChatAttend.deletechattime != null && x.Messagesdate <= x.EventChatAttend.deletechatDate && x.Messagestime >= x.EventChatAttend.deletechattime)))).OrderByDescending(x => x.Messagesdate).OrderByDescending(x => x.Messagestime).GroupBy(x => x.EventChatAttendId);
            //              ReturnedListData.AddRange(Obj.Select(x => new UserDetailsvm
            //              {
            //                  Name = x.FirstOrDefault().EventChatAttend.EventData.Title,
            //                  Image = x.FirstOrDefault().EventChatAttend.EventData.image,
            //                  id = x.FirstOrDefault().EventChatAttend.EventData.EntityId,
            //                  muit = x.FirstOrDefault().EventChatAttend.muit,
            //                  Leavevent = x.FirstOrDefault().EventChatAttend.stutus == 0 ? 0 : (x.FirstOrDefault().EventChatAttend.stutus == 1 ? 1 : 2),
            //                  latestdate = x.FirstOrDefault().Messagesdate.ConvertDateTimeToString(),
            //                  latesttime = x.FirstOrDefault().Messagestime.ToString(@"hh\:mm"),
            //                  isevent = true,
            //                  myevent = x.FirstOrDefault().EventChatAttend.EventData.UserId == currentuserid,
            //                  messagestype = x.FirstOrDefault().Messagetype,
            //                  messagesimage = x.FirstOrDefault().MessagesAttached,
            //                  messages = x.FirstOrDefault().Messages,
            //                  latestdatevm = x.FirstOrDefault().Messagesdate,
            //                  latesttimevm = x.FirstOrDefault().Messagestime,

            //              }));
            //          });
            //await Task.Run(() =>
            //{
            //    var Obj = MessageDataList.Where(x => x.UserMessagessId != null).OrderByDescending(x => x.Messagesdate).OrderByDescending(x => x.Messagestime).GroupBy(x => x.UserMessagessId);
            //    ReturnedListData.AddRange(Obj.Select(item => new UserDetailsvm
            //    {
            //        Name = (item.FirstOrDefault().UserMessagess.ToUserId == currentuserid ? item.FirstOrDefault().UserMessagess.User.User.DisplayedUserName : item.FirstOrDefault().UserMessagess.ToUser.User.DisplayedUserName),
            //        Image = (item.FirstOrDefault().UserMessagess.ToUserId == currentuserid ? item.FirstOrDefault().UserMessagess.User.UserImage : item.FirstOrDefault().UserMessagess.ToUser.UserImage),
            //        id = item.FirstOrDefault().UserMessagess.ToUserId == currentuserid ? item.FirstOrDefault().UserMessagess.User.UserId : item.FirstOrDefault().UserMessagess.ToUser.UserId,

            //        //isfrind =item.Key.UserMessagess.ToUser. friendtype == null ? false : friendtype.status == 1 ? true : false,
            //        isfrind = true,
            //        muit = item.Any(xx => xx.UserId == currentuserid && (xx.UserMessagess.Tomuit == curen || xx.UserMessagess.muit == curen)),
            //        latestdate = item.FirstOrDefault().Messagesdate.ConvertDateTimeToString(),
            //        latesttime = item.FirstOrDefault().Messagestime.ToString(@"hh\:mm"),
            //        latestdatevm = item.FirstOrDefault().Messagesdate,
            //        latesttimevm = item.FirstOrDefault().Messagestime,

            //        isevent = false,
            //        messages = item.FirstOrDefault().Messages,
            //        messagestype = item.FirstOrDefault().Messagetype,
            //        messagesimage = item.FirstOrDefault().MessagesAttached,

            //    }));
            //});


            ////FristTry
            //var MessageDataList = this._authContext.Messagedata.Where(x =>
            //  (x.UserId == currentuserid && x.EventChatAttendId != null && x.UserMessagessId != null) ||
            //  (x.UserMessagess != null && (x.UserMessagess.ToUserId == currentuserid || x.UserMessagess.UserId == currentuserid)) ||
            //  (x.EventChatAttend != null && allUserEventsData.Contains(x.EventChatAttend.EventDataid) &&
            //  ((x.EventChatAttend.UserattendId == currentuserid) &&
            //  (x.EventChatAttend.leaveeventDate == null || (x.EventChatAttend.leaveeventtime != null && x.Messagesdate <= x.EventChatAttend.leaveeventDate && x.Messagestime < x.EventChatAttend.leaveeventtime)) ||
            //  (x.EventChatAttend.leveeventchatDate == null || (x.EventChatAttend.leveeventchattime != null && x.Messagesdate <= x.EventChatAttend.leveeventchatDate && x.Messagestime < x.EventChatAttend.leveeventchattime)) ||
            //  (x.EventChatAttend.deletedate == null || (x.EventChatAttend.delettime != null && x.Messagesdate <= x.EventChatAttend.deletedate && x.Messagestime < x.EventChatAttend.delettime)) || (x.EventChatAttend.deletechatDate == null ||
            //  (x.EventChatAttend.deletechattime != null && x.Messagesdate <= x.EventChatAttend.deletechatDate && x.Messagestime >= x.EventChatAttend.deletechattime))))).OrderByDescending(x => x.Messagesdate).OrderByDescending(x => x.Messagestime);
            //await Task.Run(() =>
            //{
            //    var Obj = MessageDataList.Where(x => x.EventChatAttendId != null).AsEnumerable().GroupBy(x => x.EventChatAttendId);
            //    ReturnedListData.AddRange(Obj.Select(x => new UserDetailsvm
            //    {
            //        Name = x.FirstOrDefault().EventChatAttend.EventData.Title,
            //        Image = x.FirstOrDefault().EventChatAttend.EventData.image,
            //        id = x.FirstOrDefault().EventChatAttend.EventData.EntityId,

            //        muit = x.FirstOrDefault().EventChatAttend.muit,
            //        Leavevent = x.FirstOrDefault().EventChatAttend.stutus == 0 ? 0 : (x.FirstOrDefault().EventChatAttend.stutus == 1 ? 1 : 2),
            //        latestdate = x.FirstOrDefault().Messagesdate.ConvertDateTimeToString(),
            //        latesttime = x.FirstOrDefault().Messagestime.ToString(@"hh\:mm"),
            //        isevent = true,
            //        myevent = x.FirstOrDefault().EventChatAttend.EventData.UserId == currentuserid,
            //        messagestype = x.FirstOrDefault().Messagetype,
            //        messagesimage = x.FirstOrDefault().MessagesAttached,
            //        messages = x.FirstOrDefault().Messages,
            //        latestdatevm = x.FirstOrDefault().Messagesdate,
            //        latesttimevm = x.FirstOrDefault().Messagestime,

            //    }));
            //});
            //await Task.Run(() =>
            //{
            //    var Obj = MessageDataList.Where(x => x.UserMessagessId != null).AsEnumerable().GroupBy(x => x.UserMessagessId);

            //    ReturnedListData.AddRange(Obj.Select(item => new UserDetailsvm
            //    {
            //        Name = (item.FirstOrDefault().UserMessagess.ToUserId == currentuserid ? item.FirstOrDefault().UserMessagess.User.User.DisplayedUserName : item.FirstOrDefault().UserMessagess.ToUser.User.DisplayedUserName),
            //        Image = (item.FirstOrDefault().UserMessagess.ToUserId == currentuserid ? item.FirstOrDefault().UserMessagess.User.UserImage : item.FirstOrDefault().UserMessagess.ToUser.UserImage),
            //        id = item.FirstOrDefault().UserMessagess.ToUserId == currentuserid ? item.FirstOrDefault().UserMessagess.User.UserId : item.FirstOrDefault().UserMessagess.ToUser.UserId,

            //        //isfrind =item.Key.UserMessagess.ToUser. friendtype == null ? false : friendtype.status == 1 ? true : false,
            //        isfrind = true,
            //        muit = item.Any(xx => xx.UserId == currentuserid && (xx.UserMessagess.Tomuit == curen || xx.UserMessagess.muit == curen)),
            //        latestdate = item.FirstOrDefault().Messagesdate.ConvertDateTimeToString(),
            //        latesttime = item.FirstOrDefault().Messagestime.ToString(@"hh\:mm"),
            //        latestdatevm = item.FirstOrDefault().Messagesdate,
            //        latesttimevm = item.FirstOrDefault().Messagestime,

            //        isevent = false,
            //        messages = item.FirstOrDefault().Messages,
            //        messagestype = item.FirstOrDefault().Messagetype,
            //        messagesimage = item.FirstOrDefault().MessagesAttached,

            //    }));
            //});

            return ReturnedListData;

        }
        public async Task<List<UserDetailsvm>> getalllUserinconect(int currentuserid, string curen, string search = null)
        {
            var currentuser = httpContextAccessor.HttpContext.GetUser().User;
            //var attch = this._authContext.MessageAttached.ToList();
            var allmessage = this._authContext.Messagedata.Include(e => e.EventChatAttend)

                .ToList();
            var UserMessages = allmessage.Where(m => m.UserMessagess != null).Where(n => n.UserMessagess.ToUserId == currentuserid || n.UserMessagess.UserId == currentuserid || n.UserId == currentuserid).ToList().OrderByDescending
                 (m => m.Messagesdate.Date).ThenByDescending(m => m.Messagestime).Select(m => m.UserMessagess).ToList();
            List<UserDetailsvm> returndate = new List<UserDetailsvm>();
            var Requestes = this._authContext.Requestes.ToList();
            var userevent = allmessage.Where(m => m.EventChatAttendId != null).ToList();
            var eventattend = userevent.Where(m => m.EventChatAttend.UserattendId == currentuserid && m.EventChatAttend.EventData.eventdateto.Value.Date.AddDays(5) >= DateTime.Now.Date).Select(m => m.EventChatAttend.EventDataid).ToList();
            var eventchat = userevent.Where(m => eventattend.Contains(Convert.ToInt32(m.EventChatAttend.EventDataid))).ToList();
            bool muit = EventChatmessage(currentuserid, eventchat.ToList(), returndate);
            muit = UserMessagedata(currentuserid, curen, allmessage.ToList(), UserMessages, Requestes, muit, returndate);

            var chatgroup = new List<UserDetailsvm>();
            var chatgrouplist = _authContext.ChatGroups
                .Where(chat => chat.Subscribers.Any(x => /*x.Status == ChatGroupSubscriberStatus.Joined &&*/ x.UserID == curen))
                .ToList();
            foreach (var chatgroupObj in chatgrouplist)
            {
                var userDetails = _authContext.UserDetails.Include(a=>a.User).FirstOrDefault(a => a.UserId == chatgroupObj.UserID);
                var chatgroupusersettings = chatgroupObj.Subscribers.First(x => x.UserID == curen);
                if (chatgroupusersettings.LeaveGroup != ChatGroupSubscriberStatus.Joined && chatgroupusersettings.ClearChatDateTime != null)
                    continue;
                {
                    //var validamessagedata= chatgroupObj.Messagedatas.Where(x=> chatgroupusersettings.Status)
                    var messagedata = await (await chatGroupService.UserChatGroupMessages(currentuser, chatgroupObj.ID, curen)).OrderByDescending(x => x.Messagesdate).ThenByDescending(x => x.Messagestime).FirstOrDefaultAsync();
                    chatgroup.Add(new UserDetailsvm
                    {
                        isChatGroup = true,
                        Name = chatgroupObj.Name,
                        Image = chatgroupObj.Image,
                        isCommunityGroup = (userDetails?.IsWhiteLabel.Value == true && userDetails?.User.RegistrationDate.ConvertDateTimeToString() == chatgroupObj.RegistrationDateTime.ConvertDateTimeToString() && userDetails?.User.RegistrationDate.ToString("HH:mm") == chatgroupObj.RegistrationDateTime.ToString("HH:mm")),
                        id = chatgroupObj.ID.ToString(),
                        muit = chatgroupusersettings.IsMuted,
                        LeaveGroup = (int)chatgroupusersettings.LeaveGroup,
                        Leavevent = 0,
                        message_not_Read = chatgroupusersettings.UserNotreadcount,
                        IsChatGroupAdmin = chatgroupusersettings.IsAdminGroup == ChatGroupSubscriberType.Admin,
                        latestdate = messagedata == null ? chatgroupObj.RegistrationDateTime.ConvertDateTimeToString() : messagedata.Messagesdate.ConvertDateTimeToString(),
                        latesttime = messagedata == null ? chatgroupObj.RegistrationDateTime.ToString(@"HH\:mm") : messagedata.Messagestime.ToString(@"hh\:mm"),
                        messagestype = messagedata == null ? ((int)Messagetype.Text) : messagedata.Messagetype,
                        messagesimage = messagedata == null ? null : messagedata.MessagesAttached,
                        messages = messagedata == null ? "" : messagedata.Messages,
                        latestdatevm = messagedata == null ? chatgroupObj.RegistrationDateTime : messagedata.Messagesdate,
                        latesttimevm = messagedata == null ? chatgroupObj.RegistrationDateTime.TimeOfDay : messagedata.Messagestime,                        

                    });
                }
            }

            returndate.AddRange(chatgroup);
            var returndate2 = returndate.OrderByDescending(m => m.latestdatevm).ThenByDescending(m => m.latesttimevm).ToList();
            if (search != null)
            {
                returndate2 = returndate2.Where(b => b.Name.ToLower().Contains(search.ToLower())).ToList();
            }
            return returndate2;
        }
        private bool UserMessagedata(int currentuserid, string curen, List<Messagedata> allmessage, List<UserMessages> UserMessages, List<Requestes> Requestes, bool muit, List<UserDetailsvm> returndate)
        {
            var user= _authContext.UserDetails.FirstOrDefault(u=>u.PrimaryId==currentuserid);
            var datatest = allmessage.Where(m => m.UserMessagessId != null);
            datatest = datatest.Where(n => n.UserMessagess.ToUserId == currentuserid || n.UserMessagess.UserId == currentuserid || n.UserId == currentuserid).OrderByDescending
                (m => m.Messagesdate.Date).ThenByDescending(m => m.Messagestime);

            var usercate = datatest.GroupBy(m => m.UserMessagessId).ToList();
            foreach (var itemdata in usercate)
            {
                {
                    var item = itemdata.Where(m => (m.UserMessagess.delete == curen ? (m.UserMessagess.deletedate.Value.Date <= m.Messagesdate.Date && (m.UserMessagess.deletedate.Value.Date == m.Messagesdate.Date ? m.UserMessagess.deleteTime < m.Messagestime : true)) : true));
                    item = item.Where(m => (m.UserMessagess.Todelete == curen ? (m.UserMessagess.Userdeletedate.Value.Date <= m.Messagesdate.Date && (m.UserMessagess.Userdeletedate.Value.Date == m.Messagesdate.Date ? m.UserMessagess.UserdeleteTime < m.Messagestime : true)) : true));
                    var a = item.Count();
                    if (a != 0)
                    {
                        string userid = (item.FirstOrDefault().UserMessagess.ToUserId == currentuserid ? item.FirstOrDefault().UserMessagess.ToUser.UserId : item.FirstOrDefault().UserMessagess.User.UserId);

                        var id = item.FirstOrDefault().UserMessagess.ToUserId == currentuserid ? item.FirstOrDefault().UserMessagess.User.UserId : item.FirstOrDefault().UserMessagess.ToUser.UserId;
                        var request = Requestes.Where(m => ((m.UserId == currentuserid && m.UserRequest.UserId == id) || (m.User.UserId == id && m.UserRequestId == currentuserid)));
                        var relation = request.FirstOrDefault(m => (m.status == 2));
                        var friendtype = request.FirstOrDefault();
                        if(relation != null && user.IsWhiteLabel.HasValue && user.IsWhiteLabel.Value)
                        {
                            UserDetailsvm data = new UserDetailsvm();
                            data.Name = (item.FirstOrDefault().UserMessagess.ToUserId == currentuserid ? item.FirstOrDefault().UserMessagess.User.User.DisplayedUserName : item.FirstOrDefault().UserMessagess.ToUser.User.DisplayedUserName);
                            data.Image = item.FirstOrDefault().UserMessagess.ToUserId == currentuserid
                                ? string.IsNullOrEmpty(item.FirstOrDefault().UserMessagess.User.UserImage)
                                    ? "" : item.FirstOrDefault().UserMessagess.User.UserImage
                                :string.IsNullOrEmpty(item.FirstOrDefault().UserMessagess.ToUser.UserImage)
                                    ? "" : item.FirstOrDefault().UserMessagess.ToUser.UserImage;
                            data.id = id;
                            muit = getmuitMessages(item.FirstOrDefault().UserMessagessId, userid, UserMessages);
                            data.isfrind = friendtype == null ? false : friendtype.status == 1 ? true : false; 
                            data.ImageIsVerified = (item.FirstOrDefault().UserMessagess.ToUserId == currentuserid ? item.FirstOrDefault().UserMessagess.User.ImageIsVerified ?? false  : item.FirstOrDefault().UserMessagess.ToUser.ImageIsVerified ?? false);
                            data.muit = muit;
                            data.message_not_Read = (item.FirstOrDefault().UserMessagess.ToUserId == currentuserid ? item.FirstOrDefault().UserMessagess.ToUserNotreadcount : item.FirstOrDefault().UserMessagess.UserNotreadcount);

                            data.latestdate = item.FirstOrDefault().Messagesdate.ConvertDateTimeToString();
                            data.latesttime = item.FirstOrDefault().Messagestime.ToString(@"hh\:mm");
                            data.latestdatevm = item.FirstOrDefault().Messagesdate;
                            data.latesttimevm = item.FirstOrDefault().Messagestime;
                            data.isevent = itemdata.Key == null ? true : false;
                            data.messages = item.FirstOrDefault().Messages;
                            data.messagestype = item.FirstOrDefault().Messagetype;
                            data.messagesimage = item.FirstOrDefault().MessagesAttached;
                            data.IsWhiteLabel= item.FirstOrDefault().UserMessagess.User.IsWhiteLabel.Value;
                            returndate.Add(data);
                        }
                        else if (relation == null)
                        {
                            UserDetailsvm data = new UserDetailsvm();
                            data.Name = (item.FirstOrDefault().UserMessagess.ToUserId == currentuserid ? item.FirstOrDefault().UserMessagess.User.User.DisplayedUserName : item.FirstOrDefault().UserMessagess.ToUser.User.DisplayedUserName);
                            data.Image = item.FirstOrDefault().UserMessagess.ToUserId == currentuserid
                                ? string.IsNullOrEmpty(item.FirstOrDefault().UserMessagess.User.UserImage)
                                    ? "" : item.FirstOrDefault().UserMessagess.User.UserImage
                                : string.IsNullOrEmpty(item.FirstOrDefault().UserMessagess.ToUser.UserImage)
                                    ? "" : item.FirstOrDefault().UserMessagess.ToUser.UserImage;
                            data.id = id;
                            muit = getmuitMessages(item.FirstOrDefault().UserMessagessId, userid, UserMessages);
                            data.isfrind = (item.FirstOrDefault().User.IsWhiteLabel.HasValue && item.FirstOrDefault().User.IsWhiteLabel.Value)==true?true:friendtype == null  ? false : friendtype.status == 1 ? true : false;
                            data.muit = muit;
                            data.IsWhiteLabel = item.FirstOrDefault().UserMessagess.User.IsWhiteLabel.Value;
                            data.message_not_Read = (item.FirstOrDefault().UserMessagess.ToUserId == currentuserid ? item.FirstOrDefault().UserMessagess.ToUserNotreadcount : item.FirstOrDefault().UserMessagess.UserNotreadcount);
                            data.ImageIsVerified = (item.FirstOrDefault().UserMessagess.ToUserId == currentuserid ? item.FirstOrDefault().UserMessagess.User.ImageIsVerified ?? false : item.FirstOrDefault().UserMessagess.ToUser.ImageIsVerified ?? false);
                            data.latestdate = item.FirstOrDefault().Messagesdate.ConvertDateTimeToString();
                            data.latesttime = item.FirstOrDefault().Messagestime.ToString(@"hh\:mm");
                            data.latestdatevm = item.FirstOrDefault().Messagesdate;
                            data.latesttimevm = item.FirstOrDefault().Messagestime;
                            data.isevent = itemdata.Key == null ? true : false;
                            data.messages = item.FirstOrDefault().Messages;
                            data.messagestype = item.FirstOrDefault().Messagetype;
                            data.messagesimage = item.FirstOrDefault().MessagesAttached;
                            returndate.Add(data);
                        }
                    }
                }
            }

            return muit;
        }

        private bool EventChatmessage(int currentuserid, List<Messagedata> eventchat, List<UserDetailsvm> returndate)
        {
            bool muit = false;
            var DbF = Microsoft.EntityFrameworkCore.EF.Functions;
            var eventcat1 = eventchat.ToList();
            var eventcat = eventcat1.GroupBy(m => m.EventChatAttend.EventDataid).ToList();
            foreach (var eveitem in eventcat)
            {
                int Leavevent = 0;
                bool Leaveventchat = false;
                var eveitemvalue = eveitem.OrderByDescending(m => m.Messagesdate.Date).ThenByDescending(m => m.Messagestime).ToList();
                var usereventdata = eveitemvalue.Where(m => m.EventChatAttend.UserattendId == currentuserid).Select(m => m.EventChatAttend).FirstOrDefault();
                eveitemvalue = eveitemvalue.Where(m => m.Messagesdate.Date >= usereventdata.JoinDate.Value.Date && (m.Messagesdate.Date == usereventdata.JoinDate.Value.Date ? (m.Messagestime >= usereventdata.Jointime) : true)).ToList();
                if (usereventdata.delete == true)
                {
                    eveitemvalue = eveitemvalue.Where(x => (DbF.DateDiffDay(usereventdata.deletechatDate, x.Messagesdate) > 0) || ((DbF.DateDiffDay(usereventdata.deletechatDate, x.Messagesdate) == 0 && (DbF.DateDiffSecond(usereventdata.deletechattime, x.Messagestime) > 0)))).ToList();

                    //eveitemvalue = eveitemvalue.Where(m => m.Messagesdate.Date > usereventdata.deletechatDate.Value.Date ||
                    // m.Messagestime > usereventdata.deletechattime ).ToList();

                }
                if (usereventdata.leave == true)
                {
                    eveitemvalue = eveitemvalue.Where(x => (DbF.DateDiffDay(usereventdata.leaveeventDate, x.Messagesdate) < 0) || ((DbF.DateDiffDay(usereventdata.leaveeventDate, x.Messagesdate) == 0 && (DbF.DateDiffSecond(usereventdata.leaveeventtime, x.Messagestime) < 0)))).ToList();

                    //eveitemvalue = eveitemvalue.Where(m => m.Messagesdate.Date < usereventdata.leaveeventDate.Value.Date ||m.Messagestime < usereventdata.leaveeventtime).ToList();
                    Leavevent = 1;
                }
                if (usereventdata.leavechat == true)
                {

                    eveitemvalue = eveitemvalue.Where(x => (DbF.DateDiffDay(usereventdata.leveeventchatDate, x.Messagesdate) < 0) || ((DbF.DateDiffDay(usereventdata.leveeventchatDate, x.Messagesdate) == 0 && (DbF.DateDiffSecond(usereventdata.leveeventchattime, x.Messagestime) < 0)))).ToList();
                    //eveitemvalue = eveitemvalue.Where(m => m.Messagesdate.Date < usereventdata.leveeventchatDate.Value.Date || m.Messagestime < usereventdata.leveeventchattime).ToList();
                    Leaveventchat = true;
                }
                if (usereventdata.removefromevent == true)
                {
                    eveitemvalue = eveitemvalue.Where(x => (DbF.DateDiffDay(usereventdata.deletedate, x.Messagesdate) < 0) || ((DbF.DateDiffDay(usereventdata.deletedate, x.Messagesdate) == 0 && (DbF.DateDiffSecond(usereventdata.delettime, x.Messagestime) < 0)))).ToList();

                    //eveitemvalue = eveitemvalue.Where(m => m.Messagesdate.Date <= usereventdata.deletedate.Value.Date && (m.Messagesdate.Date == usereventdata.deletedate.Value.Date ? (m.Messagestime< usereventdata.delettime) : true)).ToList();
                    Leavevent = 1;
                }

                // var attend = eventattend.Where(m => m.EventDataid == eveitemvalue.FirstOrDefault().EventChat.EventData.Id && m.UserattendId == currentuserid).ToList();
                //var blocate = attend.Where(m => m.stutus == 2).FirstOrDefault();
                if (eveitemvalue.Count != 0)
                {

                    {
                        var evu = eveitemvalue.FirstOrDefault(m => m.EventChatAttend.UserattendId == currentuserid);
                        UserDetailsvm data = new UserDetailsvm();
                        data.Name = eveitemvalue.FirstOrDefault().EventChatAttend.EventData.Title;
                        data.Image = eveitemvalue.FirstOrDefault().EventChatAttend.EventData.image;
                        data.id = eveitemvalue.FirstOrDefault().EventChatAttend.EventData.EntityId;
                        muit = usereventdata.muit;
                        data.muit = muit;
                        data.Leavevent = Leavevent;
                        data.Leaveventchat = Leaveventchat;
                        data.latestdate = eveitemvalue.FirstOrDefault().Messagesdate.ConvertDateTimeToString();
                        data.latesttime = eveitemvalue.FirstOrDefault().Messagestime.ToString(@"hh\:mm");
                        data.isevent = true;
                        data.myevent = eveitemvalue.FirstOrDefault().EventChatAttend.EventData.UserId == currentuserid;

                        data.message_not_Read = evu == null ? 0 : evu.EventChatAttend.UserNotreadcount;
                        data.messagestype = eveitemvalue.FirstOrDefault().Messagetype;
                        data.messagesimage = eveitemvalue.FirstOrDefault().MessagesAttached;
                        data.messages = eveitemvalue.FirstOrDefault(m => (m.Messages != "")) == null ? eveitemvalue.FirstOrDefault().Messages : eveitemvalue.FirstOrDefault(m => m.Messages != "").Messages;
                        data.isactive = eveitemvalue.FirstOrDefault().EventChatAttend.EventData.IsActive;
                        data.latestdatevm = eveitemvalue.FirstOrDefault().Messagesdate;
                        data.latesttimevm = eveitemvalue.FirstOrDefault().Messagestime;
                        data.eventtypeid = eveitemvalue.FirstOrDefault().EventChatAttend.EventData.EventTypeList.entityID;

                        data.eventtypeintid = eveitemvalue.FirstOrDefault().EventChatAttend.EventData.EventTypeList.ID;
                        data.eventtypecolor = eveitemvalue.FirstOrDefault().EventChatAttend.EventData.EventTypeList.color;
                        data.eventtype = eveitemvalue.FirstOrDefault().EventChatAttend.EventData.EventTypeList.Name;
                        returndate.Add(data);
                    }
                }
                else
                {
                    ////UserDetailsvm data = new UserDetailsvm();
                    ////                           data.Name = eveitem.FirstOrDefault().EventChatAttend.EventData.Title;
                    ////                           data.Image = eveitem.FirstOrDefault().EventChatAttend.EventData.image;
                    ////                           data.id = eveitem.FirstOrDefault().EventChatAttend.EventData.EntityId;
                    ////                           muit = usereventdata.muit;
                    ////                           data.muit = muit;

                    ////                   data.Leavevent = usereventdata.stutus == 0 ? 0 : (usereventdata.stutus == 1 ? 1 : 2);
                    ////                   data.latestdate = usereventdata.JoinDate.Value.ConvertDateTimeToString();
                    ////                           data.latesttime = usereventdata.Jointime.Value.ToString(@"hh\:mm");
                    ////                           data.isevent = true;
                    ////                   data.latestdatevm = usereventdata.JoinDate.Value;
                    ////                   data.latesttimevm = usereventdata.Jointime.Value;
                    ////                   data.myevent = eveitem.FirstOrDefault().EventChatAttend.EventData.UserId == currentuserid;
                    ////                           data.messagestype = eveitem.FirstOrDefault().Messagetype;
                    ////                           data.messagesimage = "";
                    ////                           data.messages = "";
                    ////                           returndate.Add(data);


                }
            }

            return muit;
        }

        //public string getmesattach(string Id, List<MessageAttached> attch)
        //{
        //    return attch.Where(m => m.MessagedataId == Id).LastOrDefault() == null ? "" : attch.Where(m => m.MessagedataId == Id).LastOrDefault().attached;
        //}
        //public bool getallattendevent(string id, int userid, List<eventattend> eventattend)
        //{
        //    var data = eventattend.Where(n => n.EventData.EntityId == id && n.UserattendId == userid && n.stutus != 1 && n.stutus != 2).Distinct().FirstOrDefault();

        //    return (data == null ? false : data.muit);
        //}

        public async Task<MessageVIEWDTO> addusermessage(MessageDTO MessageDTO, UserDetails userDeatils, string usermessid)
        {
            Messagedata Messagedata = new Messagedata();
            Messagedata.Messagesdate = DateTime.Parse(MessageDTO.Messagesdate);
            Messagedata.Messagestime = MessageDTO.Messagestime;
            Messagedata.linkable = MessageDTO.linkable;
            Messagedata.EventDataid = geteventMessages(MessageDTO.EventLINKid);
            Messagedata.UserMessagessId = usermessid;
            Messagedata.UserId = userDeatils.PrimaryId;
            Messagedata.Messages = MessageDTO.Message;
            Messagedata.Latitude = MessageDTO.Latitude;
            Messagedata.Longitude = MessageDTO.Longitude;
            Messagedata.LocationName = MessageDTO.LocationName;
            Messagedata.LocationPeriod = MessageDTO.LocationPeriod;
            Messagedata.LocationStartTime = MessageDTO.LocationStartTime;
            Messagedata.LocationEndTime = MessageDTO.LocationEndTime;
            Messagedata.IsLiveLocation = MessageDTO.IsLiveLocation;
            Messagedata.Messagetype = MessageDTO.Messagetype;
            MessageVIEWDTO MessageVIEWDTO = new MessageVIEWDTO();
            if (MessageDTO.Attach != null)
            {
                var UniqName = await globalMethodsService.uploadFileAsync("/Images/Messagedata/", MessageDTO.Attach[0]);
                Messagedata.MessagesAttached = "/Images/Messagedata/" + UniqName;
                MessageVIEWDTO.Attach = globalMethodsService.GetBaseDomain() + "/Images/Messagedata/" + UniqName;

            }

            string usermesdataid = Convert.ToString(await addMessagedata(Messagedata));
            MessageVIEWDTO.UserId = userDeatils.UserId;
            MessageVIEWDTO.Message = MessageDTO.Message;
            MessageVIEWDTO.Messagetype = MessageDTO.Messagetype;

            MessageVIEWDTO.Id = usermesdataid;
            return MessageVIEWDTO;
        }
        public async Task<MessageVIEWDTO> addeventmessage(EventMessageDTO MessageDTO, UserDetails userDeatils)
        {
            Messagedata Messagedata = new Messagedata();
            Messagedata.Messagesdate = MessageDTO.Messagesdate;
            Messagedata.Messagestime = MessageDTO.Messagestime;
            Messagedata.EventChatAttendId = MessageDTO.EventChatAttendid;
            Messagedata.UserId = userDeatils.PrimaryId;
            Messagedata.Messages = MessageDTO.Message;
            Messagedata.Latitude = MessageDTO.Latitude;
            Messagedata.Longitude = MessageDTO.Longitude;
            Messagedata.LocationName = MessageDTO.LocationName;
            Messagedata.LocationPeriod = MessageDTO.LocationPeriod;
            Messagedata.LocationStartTime = MessageDTO.LocationStartTime;
            Messagedata.LocationEndTime = MessageDTO.LocationEndTime;
            Messagedata.IsLiveLocation = MessageDTO.IsLiveLocation;
            Messagedata.linkable = MessageDTO.linkable;

            Messagedata.EventDataid = geteventMessages(MessageDTO.EventLINKid);
            Messagedata.Messagetype = MessageDTO.Messagetype;
            MessageVIEWDTO MessageVIEWDTO = new MessageVIEWDTO();
            if (MessageDTO.Attach != null)
            {

                var UniqName = await globalMethodsService.uploadFileAsync("/Images/Messagedata/", MessageDTO.Attach[0]);

                Messagedata.MessagesAttached = "/Images/Messagedata/" + UniqName;

                MessageVIEWDTO.Attach = globalMethodsService.GetBaseDomain() + "/Images/Messagedata/" + UniqName;

                //foreach (var item in MessageDTO.Attach)
                //{
                //    var UniqName = globalMethodsService.uploadFile("/Images/", item);
                //    MessageAttached MessageAttached = new MessageAttached();
                //    MessageAttached.MessagedataId = usermesdataid;
                //    MessageAttached.attached = "/Images/" + UniqName;
                //    await addMessageAttached(MessageAttached);
                //    MessageVIEWDTO.Attach = "https://backend.friendzr.com" + "/Images/" + UniqName;
                //}
            }

            string usermesdataid = Convert.ToString(await addMessagedata(Messagedata));
            MessageVIEWDTO.UserId = userDeatils.UserId;
            MessageVIEWDTO.Message = MessageDTO.Message;
            MessageVIEWDTO.Messagetype = MessageDTO.Messagetype;

            MessageVIEWDTO.Id = usermesdataid;
            return MessageVIEWDTO;
        }
        public async Task deleteeventmessage(string userid, string eventid)
        {
            var messdata = _authContext.Messagedata.Where(e => e.User.UserId == userid).ToList();
            var list = messdata.Select(n => n.Id).ToList();
            //var data = _authContext.MessageAttached.Where(m => list.Contains(m.MessagedataId)).ToList();
            //await deleteMessageAttached(data);
            await deleteMessagedata(messdata);
        }
        public int messagelogincount(string userid)
        {
            var messdata = _authContext.UserMessages
                .Where(e => e.ToUser.UserId == userid || e.User.UserId == userid).ToList();
            var tomes = messdata.Where(e => e.ToUser.UserId == userid && e.ToUserNotreadcount > 0).Sum(m => m.ToUserNotreadcount);
            var frommes = messdata.Where(e => e.User.UserId == userid && e.UserNotreadcount > 0).Sum(m => m.UserNotreadcount);
            var eventatten1 = _authContext.EventChatAttend.Where(e => e.Userattend.UserId == userid && e.EventData.eventdateto.Value.Date.AddDays(5) >= DateTime.Now.Date && e.UserNotreadcount > 0 && e.leavechat != true && e.leave != true).ToList();
            var eventatten = eventatten1.Sum(m => m.UserNotreadcount);
            var groupatten = _authContext.ChatGroupSubscribers.Where(e => e.UserID == userid && e.UserNotreadcount > 0).Sum(m => m.UserNotreadcount);

            return (int)((tomes) + (frommes) + eventatten + groupatten);
        }

    }
}
