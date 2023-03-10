using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Social.Entity.Models;
using Social.Entity.ModelView;
using Social.Services;
using Social.Services.Attributes;
using Social.Services.FireBase_Helper;
using Social.Services.Helpers;
using Social.Services.ModelView;
using Social.Services.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Social.Areas.Admin.Controllers
{
    [Area("Admin")]
    [ServiceFilter(typeof(AuthorizeUser))]
    public class ExternalController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly IUserService _userService;
        private readonly IStringLocalizer<SharedResource> localizer;
        private readonly IEventServ _Event;
        private readonly IWebHostEnvironment _environment;
        private readonly IFirebaseManager firebaseManager;
        private readonly IGlobalMethodsService globalMethodsService;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;
        public ExternalController(IFirebaseManager firebaseManager, Microsoft.Extensions.Configuration.IConfiguration configuration, IWebHostEnvironment environment, IGlobalMethodsService globalMethodsService, UserManager<User> userManager, RoleManager<ApplicationRole> roleManager,
           IStringLocalizer<SharedResource> localizer, IEventServ Event, IUserService userService)
        {
            this.firebaseManager = firebaseManager;
            _configuration = configuration;
            this.userManager = userManager;
            this._Event = Event;
            this._environment = environment;
            this.globalMethodsService = globalMethodsService;
            this._userService = userService;
            this.localizer = localizer;
        }
        public IActionResult Index()
        {
            return View(new EventDataadminMV());
        }
        [HttpPost]
        public async Task<IActionResult> Create(EventDataadminMV model)
        {
            var Result = new CommonResponse<EventDataadminMV>() { Status=true,Code=200,Message="SavedSuccessfully"};
            try
            {

                string imageName = null;
                
                List <eventjson> addeddata = new List<eventjson>();
                var userid = _userService.getallUserDetails().FirstOrDefault(b => b.User.Email != "Owner@Owner.com").PrimaryId;
                if (model.ImageFile != null)
                {
                    if (Path.GetExtension(model.ImageFile.FileName).Contains("json") == false)
                    {
                        Result.Status = false;
                        Result.Code = 406;
                        Result.Message = "Only accepted File is .Json File ";
                    }

                    var UniqName = await globalMethodsService.adminuploadFileAsync("/Images/EventData/", model.ImageFile);

                    imageName = "Images/EventData/" + UniqName;
                    var nsjd = Path.Combine(_environment.WebRootPath, imageName);
                    String result = System.IO.File.ReadAllText(nsjd);
                    //String result = System.IO.File.ReadAllText(_configuration["BaseUrl"] + imageName);
                    addeddata= JsonConvert.DeserializeObject<List<eventjson>>(result).ToList();
                    List<EventData> list = new List<EventData>();
                    var olddata = _Event.getallexternalevent();
                    foreach (var item1 in addeddata)
                    {
                        var datavalid = olddata.FirstOrDefault(m => m.lat == item1.latitude &&
                         m.lang == item1.longitude && m.Title == item1.title && m.eventdate == Convert.ToDateTime(item1.eventdate));
                        try
                        {
                            double longd = Convert.ToDouble(item1.longitude);
                            double lat = Convert.ToDouble(item1.latitude);
                            if (datavalid == null)
                            {
                                EventData item = new EventData();

                                item.EventTypeListid = 3;
                                item.totalnumbert = 1000;
                                item.IsActive = true;
                                item.EntityId = Guid.NewGuid().ToString();
                                item.UserId = userid;

                                item.status = "creator";
                                item.eventdate = Convert.ToDateTime(item1.eventdate);
                                item.eventdateto = Convert.ToDateTime(item1.eventdateto);
                                item.allday = item1.allday;
                                item.description = item1.description;
                                item.eventfrom = item1.timefrom;
                                item.eventto = item1.timeto;

                                item.checkout_details = item1.checkout_details;
                                item.SubCategoriesIds = string.Empty;
                                item.Title = item1.title;
                                item.image = item1.image;
                                item.lat = item1.latitude == "-" ? "0" : item1.latitude;
                                item.lang = item1.longitude == "-" ? "0" : item1.longitude;
                                item.CreatedDate = DateTime.Now;
                                list.Add(item);
                            }
                        }
                       catch
                        {
                            continue;
                        }
                         //await _Event.Createrang(item);
                    }
                    await _Event.Createrang(list);

                }



                
            }
            catch (Exception ex)
            {
                Result.Status = false;
                Result.Code = 406;
                Result.Message = "File Json With Rong Format Plz Check Your File And Try Again";
            }
            return Ok(JObject.FromObject(Result, new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));

            //return Ok(JObject.FromObject(t==true?"Done":"Error", new Newtonsoft.Json.JsonSerializer() { ContractResolver = new DefaultContractResolver() }));
        }
    }
}
