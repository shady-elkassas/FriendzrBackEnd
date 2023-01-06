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
    public class InterestsController : ControllerBase
    {
        private readonly IInterestsService interestsService;
        private readonly IWhatBestDescripsMe IWhatBestDescripsMe;

        private readonly IStringLocalizer<SharedResource> localizer;
        private readonly IUserService userService;
        private readonly AuthDBContext _authDBContext;
        public InterestsController(AuthDBContext authDBContext, IWhatBestDescripsMe IWhatBestDescripsMe, IInterestsService interestsService, IStringLocalizer<SharedResource> localizer, IUserService userService)
        {
            this._authDBContext = authDBContext;
            this.interestsService = interestsService;
            this.localizer = localizer;
            this.IWhatBestDescripsMe = IWhatBestDescripsMe;
            this.userService = userService;
        }
        [Route("userTag")]

        [HttpPost]
        public async Task<IActionResult> userTag([FromForm] InterestsVM model)
        {
            var allinterstes = interestsService.GetData();
            var CurrentUser = HttpContext.GetUser().User;
            var CurrentUserID = CurrentUser.Id;
            var isSavedInSharedInterestes = allinterstes.Any(x => (x.IsSharedForAllUsers == true || x.CreatedByUserID == CurrentUserID) && x.name?.ToLower().Trim() == model?.name?.ToLower().Trim());
            if (isSavedInSharedInterestes)
            {
                return StatusCode(StatusCodes.Status406NotAcceptable,
                    new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,
                         localizer["Name Already Exist"], null));
            }
            //if (userService.Getalllistoftags(CurrentUser?.UserDetails.PrimaryId ?? 0).Count >= 8)
            //{
            //    return StatusCode(StatusCodes.Status406NotAcceptable,
            //      new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,
            //      localizer["Allowed Created Tags is only eight tags"], null));
            //}
            //var Userexccluseveinterest = allinterstes.Count(x => x.IsSharedForAllUsers == false && x.CreatedByUserID == CurrentUserID);
            //if (Userexccluseveinterest >= 8)
            //{
            //    return StatusCode(StatusCodes.Status406NotAcceptable,
            //        new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,
            //             localizer["Allowed Created Tags is only eight tags"], null));

            //}
            var UserReportVM = new InterestsVM()
            {
                IsActive = true,
                IsSharedForAllUsers = true,
                name = model.name,
                RegistrationDate = DateTime.UtcNow
            };
            var Result = await interestsService.Create(UserReportVM);

            return StatusCode(Result.Code,
         new ResponseModel<object>(Result.Code, Result.Status,
         "", new { Result.Data?.EntityId, Result.Data?.name }));
        }
        [Route("ADDIam")]

        [HttpPost]
        public async Task<IActionResult> ADDIam([FromForm] WhatBestDescripsMeVM model)
        {
            var allWhatBestDescripsMe = IWhatBestDescripsMe.GetData();
            var CurrentUser = HttpContext.GetUser().User;
            var CurrentUserID = CurrentUser.Id;
            var isSavedInSharedInterestes = allWhatBestDescripsMe.Any(x => x.name?.ToLower().Trim() == model?.name?.ToLower().Trim());
            if (isSavedInSharedInterestes)
            {
                return StatusCode(StatusCodes.Status406NotAcceptable,
                    new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,
                         localizer["Name Already Exist"], null));
            }

            var UserReportVM = new WhatBestDescripsMeVM()
            {
                IsActive = true,
                IsSharedForAllUsers = false,
                name = model.name,
                RegistrationDate = DateTime.UtcNow
            };
            var Result = await IWhatBestDescripsMe.Create(UserReportVM);

            return StatusCode(Result.Code,
         new ResponseModel<object>(Result.Code, Result.Status,
         "", new { Result.Data?.EntityId, Result.Data?.name }));
        }
        [Route("GetAllInterests")]
        [Consumes("application/x-www-form-urlencoded")]
        public IActionResult GetAllInterests()
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
                var allinterstes = interestsService.GetData();

                var data = allinterstes.Where(x => x.IsActive == true && (x.IsSharedForAllUsers == true || x.CreatedByUserID == user.Id)).Reverse().Select(x => new
                {
                    id = x.EntityId,
                    name = x.name,
                    IsSharedForAll = x.IsSharedForAllUsers,

                });


                return StatusCode(StatusCodes.Status200OK,
                new ResponseModel<object>(StatusCodes.Status200OK, true,
                " the Interests data!", data));


            }
            catch (Exception ex)
            {
                //await _errorLogService.InsertErrorLog(new BWErrorLog(HttpContext.GetUser().UserId, "Events/deltsInterests", ex));
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<object>(StatusCodes.Status500InternalServerError, false, ex.Message, null));

            }
        }
        [Route("GETIam")]
        [Consumes("application/x-www-form-urlencoded")]
        public IActionResult GetAllWhatBestDescripsMe()
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
                var allinterstes = IWhatBestDescripsMe.GetData();

                var data = allinterstes.Where(x => x.IsActive == true).Reverse().Select(x => new
                {
                    id = x.EntityId,
                    name = x.name,


                }).ToList();


                return StatusCode(StatusCodes.Status200OK,
                new ResponseModel<object>(StatusCodes.Status200OK, true,
                " the What BestDescrips Me data!", data));


            }
            catch (Exception ex)
            {
                //await _errorLogService.InsertErrorLog(new BWErrorLog(HttpContext.GetUser().UserId, "Events/deltsInterests", ex));
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<object>(StatusCodes.Status500InternalServerError, false, ex.Message, null));

            }
        }
        [HttpPost]
        [Route("updateInterest")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> updateInterest([FromForm] InterestsVM model)
        {
            var Interst = interestsService.GetData(model.EntityId);
            if (Interst == null)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                new ResponseModel<object>(StatusCodes.Status404NotFound, false,
                "Sorry, the Interest does not exist!", null));
            }
            // get user
            var user = HttpContext.GetUser().User;
            var allinterstes = interestsService.GetData();

            var CurrentUserID = user.Id;
            var isSavedInSharedInterestes = allinterstes.Any(x => x.EntityId != model.EntityId && (x.IsSharedForAllUsers == true || x.CreatedByUserID == CurrentUserID) && x.name?.ToLower().Trim() == model?.name?.ToLower().Trim());


            if (isSavedInSharedInterestes)
            {
                return StatusCode(StatusCodes.Status406NotAcceptable,
                    new ResponseModel<object>(StatusCodes.Status406NotAcceptable, false,
                         localizer["Name Already Exist"], null));
            }
            //if (Interst.IsSharedForAllUsers == true || Interst.CreatedByUserID == user.Id)
            {
                model.CreatedByUserID = user.Id;
                model.Id = Interst.Id;
                model.IsSharedForAllUsers = true;
                model.IsActive = true;
                model.RegistrationDate = DateTime.Now;
                var Result = await interestsService.Edit(model);
                return StatusCode(Result.Code,
      new ResponseModel<object>(Result.Code, Result.Status,
      "", new { Result.Data?.EntityId, Result.Data?.name }));


            }
            //else
            //{
            //    return StatusCode(StatusCodes.Status404NotFound,
            // new ResponseModel<object>(StatusCodes.Status404NotFound, false,
            // localizer["sorry you can only update your interests"], null));
            //}


        }
        [HttpPost]
        [Route("DeleteInterest")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> DeleteInterest([FromForm] string interestID)
        {
            var Interst = interestsService.GetData(interestID);
            if (Interst == null)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                new ResponseModel<object>(StatusCodes.Status404NotFound, false,
                "Sorry, the Interest does not exist!", null));
            }
            // get user
            var user = HttpContext.GetUser().User;
            if (Interst.IsSharedForAllUsers == false && Interst.CreatedByUserID == user.Id)
            {

                var Result = await interestsService.Remove(interestID);
                return StatusCode(Result.Code,
      new ResponseModel<object>(Result.Code, Result.Status,
      Result.Message, null));


            }
            else
            {
                return StatusCode(StatusCodes.Status404NotFound,
             new ResponseModel<object>(StatusCodes.Status404NotFound, false,
             localizer["sorry you can only update your interests"], null));
            }


        }

        [HttpPost]
        [Route("updateIam")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> updateIam([FromForm] WhatBestDescripsMeVM model)
        {
            var Interst = _authDBContext.WhatBestDescripsMe.FirstOrDefault(x => x.EntityId == model.EntityId);
            if (Interst == null)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                new ResponseModel<object>(StatusCodes.Status404NotFound, false,
                "Sorry, the Iam does not exist!", null));
            }
            // get user
            var user = HttpContext.GetUser().User;
            var allinterstes = IWhatBestDescripsMe.GetData();

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
                Interst.IsSharedForAllUsers = false;
                Interst.IsActive = true;
                Interst.RegistrationDate = DateTime.Now;

                _authDBContext.SaveChanges();
                var Result = await IWhatBestDescripsMe.Edit(model);
                return StatusCode(Result.Code,
      new ResponseModel<object>(Result.Code, Result.Status,
      "", new { Result.Data?.EntityId, Result.Data?.name }));


            }



        }
        [HttpPost]
        [Route("DeleteIam")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> DeleteIam([FromForm] string interestID)
        {
            var Interst = IWhatBestDescripsMe.GetData(interestID);
            if (Interst == null)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                new ResponseModel<object>(StatusCodes.Status404NotFound, false,
                "Sorry, the WhatBestDescripsMe does not exist!", null));
            }
            // get user
            var user = HttpContext.GetUser().User;

            {

                var Result = await IWhatBestDescripsMe.Remove(interestID);
                return StatusCode(Result.Code,
      new ResponseModel<object>(Result.Code, Result.Status,
      Result.Message, null));


            }



        }
    }
}
