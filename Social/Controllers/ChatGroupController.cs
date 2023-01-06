using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Social.Entity.DBContext;
using Social.Entity.Enums;
using Social.Entity.Models;
using Social.Entity.ModelView;
using Social.Services.Attributes;
using Social.Services.FireBase_Helper;
using Social.Services.Helpers;
using Social.Services.ModelView;
using Social.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Social.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    [ServiceFilter(typeof(AuthorizeUser))]
    public class ChatGroupController : ControllerBase
    {

        private readonly IChatGroupService chatGroupService;
        private readonly IFirebaseManager firebaseManager;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IMessageServes messageServes;
        private readonly AuthDBContext authDBContext;
        public ChatGroupController(IChatGroupService chatGroupService, IFirebaseManager firebaseManager, IHttpContextAccessor httpContextAccessor, IMessageServes messageServes, AuthDBContext authDBContext)
        {

            this.chatGroupService = chatGroupService;
            this.firebaseManager = firebaseManager;
            this.httpContextAccessor = httpContextAccessor;
            this.messageServes = messageServes;
            this.authDBContext = authDBContext;
        }

        async Task ChatGroupActionNotifiaction(IQueryable<User> allusers, User currentuser, ResponseModel<ChatGroupVM> Result, string Title, string Action)
        {

            List<FireBaseData> fireBaseInfos = new List<FireBaseData>();
            List<FireBaseDatamodel> addnoti = new List<FireBaseDatamodel>();
            foreach (var user in allusers)
            {
                FireBaseData fireBaseInfo = new FireBaseData()
                {
                    muit = false,
                    Title = $"{ currentuser.DisplayedUserName }  {Title}",
                    //Title = $"{ currentuser.DisplayedUserName }  {Action}  {Result.Model.Name} Chat Group",
                    imageUrl = Result.Model.Image,
                    Body = "",
                    Action_code = Result.Model.ID.ToString(),
                    isAdmin = Result.Model.ChatGroupSubscribers.FirstOrDefault(x => x.userId == user.Id)?.ChatGroupSubscriberType == ChatGroupSubscriberType.Admin,
                    Action = Action
                };
                fireBaseInfos.Add(fireBaseInfo);
                addnoti.Add(messageServes.getFireBaseData(user.UserDetails.PrimaryId, fireBaseInfo));
                await firebaseManager.SendNotification(user.UserDetails.FcmToken, fireBaseInfo);

                //await messageServes.addFireBaseDatamodel(addnoti);
            }
            //await authDBContext.SaveChangesAsync();

            await authDBContext.FireBaseDatamodel.AddRangeAsync(addnoti);
            await authDBContext.SaveChangesAsync();




        }
        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create([FromForm] ChatGroupVM model)
        {
            var currentuser = httpContextAccessor.HttpContext.GetUser().User;
            var Result = await chatGroupService.Create(currentuser, model);
            if (Result.IsSuccessful == true)
            {
                var UsersToNotifiy = model.ChatGroupSubscribers.Where(x => x.userId != currentuser.Id).Select(x => x.userId).Distinct();
                var allusers = authDBContext.Users.Where(x => UsersToNotifiy.Contains(x.Id) == true);
                await ChatGroupActionNotifiaction(allusers, currentuser, Result, "Added You To the Group " + Result.Model.Name, "Joined_ChatGroup");
            }
            return StatusCode(Result.StatusCode, Result);
        }
        [HttpPost]
        [Route("GetChatGroup")]
        public async Task<IActionResult> GetChatGroup([FromForm] Guid ID, [FromForm] string Search)
        {
            var Result = await chatGroupService.GetObj(httpContextAccessor.HttpContext.GetUser().User, ID);
            Result.Model.ChatGroupSubscribers = Result.Model.ChatGroupSubscribers.Where(x => (Search == null) || (x.UserName.ToLower().Contains(Search.ToLower().Trim()))).ToList();
            return StatusCode(Result.StatusCode, Result);
        }
        [HttpPost]
        [Route("AddUsers")]
        public async Task<IActionResult> AddUsers([FromForm] ChatGroupVM model)
        {
            var currentuser = httpContextAccessor.HttpContext.GetUser().User;
            var Result = await chatGroupService.AddUsers(currentuser, model);
            if (Result.IsSuccessful == true)
            {
                var UsersToNotifiy = model.ChatGroupSubscribers.Where(x => x.userId != currentuser.Id).Select(x => x.userId).Distinct();
                var allusers = authDBContext.Users.Where(x => UsersToNotifiy.Contains(x.Id) == true);
                await ChatGroupActionNotifiaction(allusers, currentuser, Result, "Added You To the Group " + Result.Model.Name, "Joined_ChatGroup");

                //var UsersToNotifiy = model.ChatGroupSubscribers.Where(x => x.UserID != currentuser.Id).Select(x => x.UserID).Distinct();
                //var allusers = authDBContext.Users.Where(x => UsersToNotifiy.Contains(x.Id) == true);
                //foreach (var user in allusers)
                //{
                //    FireBaseData fireBaseInfo = new FireBaseData()
                //    {
                //        muit = false,
                //        Title = $"Added You  {Result.Model.Name} Chat Group",
                //        imageUrl = Result.Model.Image,
                //        Body = "",
                //        Action_code = Result.Model.ID.ToString(),
                //        Action = "Join Chat Group"
                //    };
                //    var addnoti = messageServes.getFireBaseData(user.UserDetails.PrimaryId, fireBaseInfo);
                //    await messageServes.addFireBaseDatamodel(addnoti);
                //    await firebaseManager.SendNotification(user.UserDetails.FcmToken, fireBaseInfo);
                //}
            }
            return StatusCode(Result.StatusCode, Result);
        }
        [HttpPost]
        [Route("GetChat")]

        public async Task<IActionResult> GetChat([FromForm] Guid ID, [FromForm] int PageNumber, [FromForm] int PageSize, [FromForm] string Search)
        {
            var Result = await chatGroupService.GetChat(httpContextAccessor.HttpContext.GetUser().User, ID, PageNumber, PageSize, Search);
            var subscribers = Result.subscribers;
            var responseModel = Result.pagedResponse;
            if (responseModel.IsSuccessful == true)
            {
                //return StatusCode(responseModel.StatusCode, new { pagedModel =responseModel.Model, subscribers =subscribers});
                return StatusCode(responseModel.StatusCode,
                 new ResponseModel<object>(StatusCodes.Status200OK, true,
                 "Chat", new
                 {
                     pagedModel = responseModel.Model,
                     subscribers = subscribers
                 }));
            }
            else
            {
                return StatusCode(responseModel.StatusCode, Result.pagedResponse);

            }
        }

        [HttpPost]
        [Route("GetAllChats")]

        public async Task<IActionResult> GetAllChats([FromForm] int PageNumber, [FromForm] int PageSize, [FromForm] string Search)
        {
            var Result = await chatGroupService.GetAllChats(httpContextAccessor.HttpContext.GetUser().User, PageNumber, PageSize, Search);



            return StatusCode(Result.StatusCode, Result);

        }
        [HttpPost]
        [Route("Update")]
        public async Task<IActionResult> Update([FromForm] ChatGroupVM model)
        {
            var Result = await chatGroupService.Edit(httpContextAccessor.HttpContext.GetUser().User, model);
            return StatusCode(Result.StatusCode, Result);
        }

        [HttpPost]
        [Route("Remove")]
        public async Task<IActionResult> Remove([FromForm] Guid ID)
        {
            var currentuser = httpContextAccessor.HttpContext.GetUser().User;
            var Result = await chatGroupService.Remove(currentuser, ID);

            return StatusCode(Result.StatusCode, Result);
        }


        [HttpPost]
        [Route("Leave")]
        public async Task<IActionResult> Leave([FromForm] ChatGroupVM model)
        {
            var Result = await chatGroupService.LeaveChatGroup(httpContextAccessor.HttpContext.GetUser().User, model);
            return StatusCode(Result.StatusCode, Result);
        }
        [HttpPost]
        [Route("MuteChatGroup")]
        public async Task<IActionResult> MuteChatGroup([FromForm] ChatGroupVM model)
        {
            var Result = await chatGroupService.MuteChatGroup(httpContextAccessor.HttpContext.GetUser().User, model);
            return StatusCode(Result.StatusCode, Result);
        }
        [HttpPost]
        [Route("ClearChatGroup")]
        public async Task<IActionResult> ClearChat([FromForm] ChatGroupVM model)
        {
            var Result = await chatGroupService.ClearChatGroup(httpContextAccessor.HttpContext.GetUser().User, model);
            return StatusCode(Result.StatusCode, Result);
        }
        [HttpPost]
        [Route("kickOutUser")]
        public async Task<IActionResult> kickOutUser([FromForm] ChatGroupVM model)
        {
            var currentuser = httpContextAccessor.HttpContext.GetUser().User;
            var Result = await chatGroupService.kickOutUser(currentuser, model);
            if (Result.IsSuccessful == true)
            {
                var UsersToNotifiy = model.ChatGroupSubscribers.Where(x => x.userId != currentuser.Id).Select(x => x.userId).Distinct();
                var allusers = authDBContext.Users.Where(x => UsersToNotifiy.Contains(x.Id) == true);
                await ChatGroupActionNotifiaction(allusers, currentuser, Result, "Removed You from the Group " + Result.Model.Name, "Kickedout_ChatGroup");

            }
            return StatusCode(Result.StatusCode, Result);
        }



    }
}
