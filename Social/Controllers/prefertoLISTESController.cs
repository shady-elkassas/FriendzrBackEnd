using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Social.Entity.DBContext;
using Social.Entity.ModelView;
using Social.Services;
using Social.Services.Attributes;
using Social.Services.Helpers;
using Social.Services.ModelView;
using Social.Services.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
namespace Social.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    [ServiceFilter(typeof(AuthorizeAdmin))]
    public class prefertoLISTESController : ControllerBase
    {

        private readonly IIpreferto IIpreferto;

        private readonly IStringLocalizer<SharedResource> localizer;
        private readonly IUserService userService;
        private readonly AuthDBContext _authDBContext;
        public prefertoLISTESController(AuthDBContext authDBContext, IIpreferto IIpreferto, IStringLocalizer<SharedResource> localizer, IUserService userService)
        {
            this._authDBContext = authDBContext;

            this.localizer = localizer;
            this.IIpreferto = IIpreferto;
            this.userService = userService;
        }
        [Route("ADDpreferto")]

        [HttpPost]
        public async Task<IActionResult> ADDpreferto([FromForm] prefertoVM model)
        {
            var allWhatBestDescripsMe = IIpreferto.GetData();
            var CurrentUser = HttpContext.GetUser().User;
            var CurrentUserID = CurrentUser.Id;
            var isSavedInSharedInterestes = allWhatBestDescripsMe.Any(x => x.name?.ToLower().Trim() == model?.name?.ToLower().Trim());
            if (isSavedInSharedInterestes)
            {
                return StatusCode(StatusCodes.Status406NotAcceptable,
                    new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,
                         localizer["Name Already Exist"], null));
            }

            var UserReportVM = new prefertoVM()
            {
                IsActive = true,

                name = model.name,
                RegistrationDate = DateTime.UtcNow
            };
            var Result = await IIpreferto.Create(UserReportVM);

            return StatusCode(Result.Code,
         new ResponseModel<object>(Result.Code, Result.Status,
         "", new { Result.Data?.EntityId, Result.Data?.name }));
        }
        [Route("GETpreferto")]
        [Consumes("application/x-www-form-urlencoded")]
        public IActionResult GetAllpreferto()
        {
            try
            {
                //var loggedinUser = HttpContext.GetUser().User;
                // get user
                var user = HttpContext.GetUser().User;
                if (user == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound,
                    new ResponseModel<object>(StatusCodes.Status404NotFound, false,
                    localizer["userdoesnotexist"], null));
                }
                var allinterstes = IIpreferto.GetData();

                var data = allinterstes.Where(x => x.IsActive == true).Reverse().Select(x => new
                {
                    id = x.EntityId,
                    name = x.name,


                });


                return StatusCode(StatusCodes.Status200OK,
                new ResponseModel<object>(StatusCodes.Status200OK, true,
                " the preferto data!", data));


            }
            catch (Exception ex)
            {
                //await _errorLogService.InsertErrorLog(new BWErrorLog(HttpContext.GetUser().UserId, "Events/deltsInterests", ex));
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<object>(StatusCodes.Status500InternalServerError, false, ex.Message, null));

            }
        }
        [HttpPost]
        [Route("updatepreferto")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> updatepreferto([FromForm] prefertoVM model)
        {
            var Interst = _authDBContext.preferto.FirstOrDefault(x => x.EntityId == model.EntityId);
            if (Interst == null)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                new ResponseModel<object>(StatusCodes.Status404NotFound, false,
                "Sorry, the WhatBestDescripsMe does not exist!", null));
            }
            // get user
            var user = HttpContext.GetUser().User;
            var allinterstes = IIpreferto.GetData();

            var CurrentUserID = user.Id;
            var isSavedInSharedInterestes = allinterstes.Any(x => x.EntityId != model.EntityId && x.name?.ToLower().Trim() == model?.name?.ToLower().Trim());


            if (isSavedInSharedInterestes)
            {
                return StatusCode(StatusCodes.Status406NotAcceptable,
                    new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,
                         localizer["Name Already Exist"], null));
            }
            {
                Interst.name = model.name;

                Interst.IsActive = true;
                Interst.RegistrationDate = DateTime.Now;

                _authDBContext.SaveChanges();
                var Result = await IIpreferto.Edit(model);
                return StatusCode(Result.Code,
      new ResponseModel<object>(Result.Code, Result.Status,
      "", new { Result.Data?.EntityId, Result.Data?.name }));


            }




        }
        [HttpPost]
        [Route("Deletepreferto")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> Deletepreferto([FromForm] string interestID)
        {
            var Interst = IIpreferto.GetData(interestID);
            if (Interst == null)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                new ResponseModel<object>(StatusCodes.Status404NotFound, false,
                "Sorry, the WhatBestDescripsMe does not exist!", null));
            }
            // get user
            var user = HttpContext.GetUser().User;

            {

                var Result = await IIpreferto.Remove(interestID);
                return StatusCode(Result.Code,
      new ResponseModel<object>(Result.Code, Result.Status,
      Result.Message, null));


            }



        }
    }
}
