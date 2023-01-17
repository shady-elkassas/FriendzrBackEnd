using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Social.Entity.Enums;
using Social.Entity.Models;
using Social.Entity.ModelView;
using Social.Services.Attributes;
using Social.Services.Helpers;
using Social.Services.ModelView;
using Social.Services.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Social.Areas.Admin.Controllers
{
    [Area("Admin")]
    [ServiceFilter(typeof(AuthorizeUser))]
    public class WhitelabelController : Controller
    {
        private readonly IWhiteLableUserService _whiteLableUserService;
        private readonly IChatGroupService _chatGroupService;
        public WhitelabelController(IWhiteLableUserService whiteLableUserService, IChatGroupService chatGroupService)
        {
            _whiteLableUserService = whiteLableUserService;
            _chatGroupService = chatGroupService;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> GetWhiteLableUsers()
        {
            var  data = await _whiteLableUserService.GetWhiteLableUsers();
            int totalRecord = data.Count();
            int filterRecord = totalRecord;
            var paginationProcess = new PaginationProcess();
            var paginationParamaters = paginationProcess.GetPaginationParamaters(Request);
            data = data.Skip(paginationParamaters.PageNumber).Take(paginationParamaters.PageSize);
            var returnObj = new
            {
                draw = paginationParamaters.Draw,
                recordsTotal = totalRecord,
                recordsFiltered = filterRecord,
                data = data
            };
            return Ok(JObject.FromObject(returnObj, new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));
        }


        [HttpPost]
        public async Task<IActionResult> Create(AddEditWhiteLableUserViewModel whiteLableUser)
        {
            CommonResponse<User> result = new CommonResponse<User>();
            CommonResponse<bool> finalResult = null;
            result = await _whiteLableUserService.CreatetWhiteLableUser(whiteLableUser);
            if(result.Code == 200)
            {
                var chatModel = this.CreateChatModel(result.Data);
                var Result = await _chatGroupService.Create(result.Data,chatModel);
                finalResult = new CommonResponse<bool>()
                {
                    Code = Result.StatusCode,
                    Message = Result.Message,
                    Status = Result.IsSuccessful
                };                    
            }           
            return Ok(JObject.FromObject(finalResult, new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));
        }

        private ChatGroupVM CreateChatModel(User user)
        {
            var modelChat= new ChatGroupVM()
            {
                Name = user.DisplayedUserName,
                RegistrationDateTime= System.DateTime.UtcNow,
                Image= user.UserDetails.UserImage,                
            };           

            return modelChat;
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AddEditWhiteLableUserViewModel editWhiteLableUser)
        {
            CommonResponse<bool> result = new CommonResponse<bool>();
            result = await _whiteLableUserService.EditWhiteLableUser(editWhiteLableUser);
            return Ok(JObject.FromObject(result, new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));
        }

        [HttpGet]
        public async Task<IActionResult> GetObj(string ID)
        {
            return Ok(JObject.FromObject(await _whiteLableUserService.ViewWhiteLableUserDetails(ID), new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));

        }

        [HttpPost]
        public async Task<IActionResult> ToggleSuspend(string userId)
        {
            CommonResponse<bool> result = new CommonResponse<bool>();
            result = await _whiteLableUserService.ToggleSuspendWhiteLableUser(userId);
            return Ok(JObject.FromObject(result, new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveObj(string ID)
        {
            CommonResponse<bool> result = new CommonResponse<bool>();
            result = await _whiteLableUserService.DeleteWhiteLableUser(ID);
            return Ok(JObject.FromObject(result, new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));
        }

    }
}
