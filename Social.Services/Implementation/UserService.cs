using GoogleMaps.LocationServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Social.Entity.DBContext;
using Social.Entity.Enums;
using Social.Entity.Models;
using Social.Entity.ModelView;
using Social.FireBase;
using Social.Models;
using Social.Services.FireBase_Helper;
using Social.Services.Helpers;
using Social.Services.ModelView;
using Social.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Social.Services.Implementation
{
    public class UserService : IUserService
    {
        private readonly AuthDBContext _authContext;

        private readonly IMessageServes MessageServes;
        private readonly IHostingEnvironment _environment;
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> userManager;
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly IFirebaseManager firebaseManager;
        private readonly ICountryService countryService;
        private readonly ICityService cityService;
        private readonly IGoogleLocationService googleLocationService;
        private readonly IFrindRequest _frindRequest;


        public UserService(IConfiguration configuration, UserManager<User> userManager, RoleManager<ApplicationRole> RoleManager,
            IFirebaseManager firebaseManager,
            ICountryService countryService,
            ICityService cityService,
            IGoogleLocationService googleLocationService,
            IMessageServes MessageServes, AuthDBContext authContext, IHostingEnvironment environment, IFrindRequest frindRequest)
        {
            this.MessageServes = MessageServes;
            this._authContext = authContext;
            this._environment = environment;
            _frindRequest = frindRequest;
            _configuration = configuration;
            this.userManager = userManager;
            roleManager = RoleManager;
            this.firebaseManager = firebaseManager;
            this.countryService = countryService;
            this.cityService = cityService;
            this.googleLocationService = googleLocationService;
        }

        public async Task ResetPassword(string userId)
        {
            try
            {
                User user = await userManager.FindByIdAsync(userId);
                var token = await userManager.GeneratePasswordResetTokenAsync(user);

                var result = await userManager.ResetPasswordAsync(user, token, "P@55w0rd");
            }
            catch (Exception ex)
            {

            }
        }

        public async Task UpdateUserAddressFromGoogle(UserDetails userDetails, double latitude, double longitude)
        {
            try
            {
                var AddressData = await googleLocationService.GetAddressData(latitude, longitude);
                var Country = await countryService.Create(new CountryVM()
                {
                    GoogleName = AddressData.Country,
                    DisplayName = AddressData.Country
                });
                var City = await cityService.Create(new CityVM()
                {
                    GoogleName = string.IsNullOrEmpty(AddressData.City) ? AddressData.State : AddressData.City,
                    DisplayName = string.IsNullOrEmpty(AddressData.City) ? AddressData.State : AddressData.City
                });
                userDetails.CityID = City?.Data?.ID;
                userDetails.CountryID = Country?.Data?.ID;
                userDetails.ZipCode = AddressData.Zip;
                _authContext.Attach(userDetails);
                _authContext.Entry(userDetails).State = EntityState.Modified;
                await _authContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {

            }
        }
        public async Task InitializeSuperAdminAccount()
        {
            //var context = (ETIGroupDbContext)serviceProvider.GetService(typeof(ETIGroupDbContext));
            //var apppplicationRoleService = (IApplicationRoleService)serviceProvider.GetService(typeof(IApplicationRoleService));
            string[] roles = Enum.GetNames(typeof(StaticApplicationRoles));
            foreach (string role in roles)
            {

                await roleManager.CreateAsync(new ApplicationRole() { Name = role });
                //await      roleStore.CreateAsync(new ApplicationRole(role));

                //}
            }
            User Deafultuser = new User()
            {
                Email = "Owner@Owner.com",
                SecurityStamp = Guid.NewGuid().ToString("D"),
                DisplayedUserName = "Owner",
                logintypevalue = "1",
                UserName = "Owner",
                NormalizedEmail = "Owner@Owner.com",
                NormalizedUserName = "OWNER",
                EmailConfirmed = true

            };
            var SuperAdminUser = await userManager.Users.FirstOrDefaultAsync(u => u.UserName == Deafultuser.UserName);

            Deafultuser.Id = SuperAdminUser?.Id ?? Guid.NewGuid().ToString();
            if (SuperAdminUser == null)
            {
                var password = new PasswordHasher<User>();
                var hashed = password.HashPassword(Deafultuser, "OwnerPassword123");
                Deafultuser.PasswordHash = hashed;
                Deafultuser.UserDetails = new UserDetails
                {
                    Id = Guid.NewGuid().ToString(),
                    //UserId = Deafultuser.Id,
                    Email = Deafultuser.Email,
                    userName = Deafultuser.UserName,

                    pasword = "OwnerPassword123",

                    platform = 1,
                    allowmylocation = true,
                    Manualdistancecontrol = Convert.ToDecimal("0.25"),
                    pushnotification = true,
                    Filteringaccordingtoage = true,
                    agefrom = 14,

                    ageto = 85,
                    // UserImage = imageName != null ? "/Images/" + imageName : "/Images/WhatsApp Image 2021-06-29 at 10.14.15 PM.jpeg",
                };
                var Result = await userManager.CreateAsync(Deafultuser);
                if (Result.Succeeded)
                {
                }
                //var result = userStore.CreateAsync(Deafultuser);
            }
            var userDetails = new UserDetails
            {
                Id = Guid.NewGuid().ToString(),
                UserId = Deafultuser.Id,
                Email = Deafultuser.Email,
                userName = Deafultuser.UserName,

                pasword = "OwnerPassword",

                platform = 1,
                allowmylocation = true,
                Manualdistancecontrol = Convert.ToDecimal("0.25"),
                pushnotification = true,
                Filteringaccordingtoage = true,
                agefrom = 14,

                ageto = 85,
                // UserImage = imageName != null ? "/Images/" + imageName : "/Images/WhatsApp Image 2021-06-29 at 10.14.15 PM.jpeg",
            };
            if (_authContext.UserDetails.Any(x => x.User != null && x.UserId == Deafultuser.Id) == false)
            {
                InsertUserDetails(userDetails);
            }


            await AssignRoles(Deafultuser, new string[] { StaticApplicationRoles.SuperAdmin.ToString() });
            await InitializeAdminAccount();
            await InitializethairdAdminAccount();
            //await SaveChangesAsync();


        }
        public async Task InitializeAdminAccount()
        {
            //var context = (ETIGroupDbContext)serviceProvider.GetService(typeof(ETIGroupDbContext));
            //var apppplicationRoleService = (IApplicationRoleService)serviceProvider.GetService(typeof(IApplicationRoleService));
            string[] roles = Enum.GetNames(typeof(StaticApplicationRoles));
            foreach (string role in roles)
            {

                await roleManager.CreateAsync(new ApplicationRole() { Name = role });
                //await      roleStore.CreateAsync(new ApplicationRole(role));

                //}
            }
            User Deafultuser = new User()
            {
                Email = "Owner2@Owner2.com",
                SecurityStamp = Guid.NewGuid().ToString("D"),
                DisplayedUserName = "Owner2",
                logintypevalue = "1",
                UserName = "Owner2",
                NormalizedEmail = "Owner2@Owner2.com",
                NormalizedUserName = "Owner2",
                EmailConfirmed = true
            };
            var SuperOwner2User = await userManager.Users.FirstOrDefaultAsync(u => u.UserName == Deafultuser.UserName);

            Deafultuser.Id = SuperOwner2User?.Id ?? Guid.NewGuid().ToString();
            if (SuperOwner2User == null)
            {
                var password = new PasswordHasher<User>();
                var hashed = password.HashPassword(Deafultuser, "Owner2Password123");
                Deafultuser.PasswordHash = hashed;
                Deafultuser.UserDetails = new UserDetails
                {
                    Id = Guid.NewGuid().ToString(),
                    //UserId = Deafultuser.Id,
                    Email = Deafultuser.Email,
                    userName = Deafultuser.UserName,

                    pasword = "Owner2Password123",

                    platform = 1,
                    allowmylocation = true,
                    Manualdistancecontrol = Convert.ToDecimal("0.25"),
                    pushnotification = true,
                    Filteringaccordingtoage = true,
                    agefrom = 14,

                    ageto = 85,
                    // UserImage = imageName != null ? "/Images/" + imageName : "/Images/WhatsApp Image 2021-06-29 at 10.14.15 PM.jpeg",
                };
                var Result = await userManager.CreateAsync(Deafultuser);
                if (Result.Succeeded)
                {
                }
                //var result = userStore.CreateAsync(Deafultuser);
            }
            var userDetails = new UserDetails
            {
                Id = Guid.NewGuid().ToString(),
                UserId = Deafultuser.Id,
                Email = Deafultuser.Email,
                userName = Deafultuser.UserName,

                pasword = "Owner2Password",

                platform = 1,
                allowmylocation = true,
                Manualdistancecontrol = Convert.ToDecimal("0.25"),
                pushnotification = true,
                Filteringaccordingtoage = true,
                agefrom = 14,

                ageto = 85,
                // UserImage = imageName != null ? "/Images/" + imageName : "/Images/WhatsApp Image 2021-06-29 at 10.14.15 PM.jpeg",
            };
            if (_authContext.UserDetails.Any(x => x.User != null && x.UserId == Deafultuser.Id) == false)
            {
                InsertUserDetails(userDetails);
            }


            await AssignRoles(Deafultuser, new string[] { StaticApplicationRoles.Admin.ToString() });

            //await SaveChangesAsync();


        }
        public async Task InitializethairdAdminAccount()
        {
            //var context = (ETIGroupDbContext)serviceProvider.GetService(typeof(ETIGroupDbContext));
            //var apppplicationRoleService = (IApplicationRoleService)serviceProvider.GetService(typeof(IApplicationRoleService));
            string[] roles = Enum.GetNames(typeof(StaticApplicationRoles));
            foreach (string role in roles)
            {

                await roleManager.CreateAsync(new ApplicationRole() { Name = role });
                //await      roleStore.CreateAsync(new ApplicationRole(role));

                //}
            }
            User Deafultuser = new User()
            {
                Email = "Marketing@friendzr.com",
                SecurityStamp = Guid.NewGuid().ToString("D"),
                DisplayedUserName = "Marketing",
                logintypevalue = "1",
                UserName = "Marketing",
                NormalizedEmail = "Marketing@friendzr.com",
                NormalizedUserName = "Marketing",
                EmailConfirmed = true
            };
            var SuperOwner2User = await userManager.Users.FirstOrDefaultAsync(u => u.UserName == Deafultuser.UserName);

            Deafultuser.Id = SuperOwner2User?.Id ?? Guid.NewGuid().ToString();
            if (SuperOwner2User == null)
            {
                var password = new PasswordHasher<User>();
                var hashed = password.HashPassword(Deafultuser, "walkDownTheHill57");
                Deafultuser.PasswordHash = hashed;
                Deafultuser.UserDetails = new UserDetails
                {
                    Id = Guid.NewGuid().ToString(),
                    //UserId = Deafultuser.Id,
                    Email = Deafultuser.Email,
                    userName = Deafultuser.UserName,

                    //pasword = "walkDownTheHill57",

                    platform = 1,
                    allowmylocation = true,
                    Manualdistancecontrol = Convert.ToDecimal("0.25"),
                    pushnotification = true,
                    Filteringaccordingtoage = true,
                    agefrom = 14,

                    ageto = 85,
                    // UserImage = imageName != null ? "/Images/" + imageName : "/Images/WhatsApp Image 2021-06-29 at 10.14.15 PM.jpeg",
                };
                var Result = await userManager.CreateAsync(Deafultuser);
                if (Result.Succeeded)
                {
                }
                //var result = userStore.CreateAsync(Deafultuser);
            }
            var userDetails = new UserDetails
            {
                Id = Guid.NewGuid().ToString(),
                UserId = Deafultuser.Id,
                Email = Deafultuser.Email,
                userName = Deafultuser.UserName,

                pasword = "Owner2Password",

                platform = 1,
                allowmylocation = true,
                Manualdistancecontrol = Convert.ToDecimal("0.25"),
                pushnotification = true,
                Filteringaccordingtoage = true,
                agefrom = 14,

                ageto = 85,
                // UserImage = imageName != null ? "/Images/" + imageName : "/Images/WhatsApp Image 2021-06-29 at 10.14.15 PM.jpeg",
            };
            if (_authContext.UserDetails.Any(x => x.User != null && x.UserId == Deafultuser.Id) == false)
            {
                InsertUserDetails(userDetails);
            }


            await AssignRoles(Deafultuser, new string[] { StaticApplicationRoles.Admin.ToString() });

            //await SaveChangesAsync();


        }
        async Task AssignRoles(User user, string[] roles)
        {
            //UserManager<ApplicationUser> _userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            IdentityResult result = new IdentityResult();
            try
            {
                result = await userManager.AddToRolesAsync(user, roles);
                //return result;

            }
            catch (Exception ex)
            {

            }
            //return result;
        }
        public void InsertUserDetails(UserDetails user)
        {
            this._authContext.UserDetails.Add(user);
            this._authContext.SaveChanges();
        }

        public void InsertLoggedInUser(LoggedinUser user)
        {
            user.Id = Guid.NewGuid().ToString();
            this._authContext.LoggedinUser.Add(user);
            this._authContext.SaveChanges();
        }
        public void InsertUserCode(UserCodeCheck code)
        {
            this._authContext.UserCodeCheck.Add(code);
            this._authContext.SaveChanges();
        }
        public void UpdateUserCode(UserCodeCheck code)
        {
            this._authContext.UserCodeCheck.Update(code);
            this._authContext.SaveChanges();
        }
        public void DeleteUserCode(UserCodeCheck code)
        {
            this._authContext.UserCodeCheck.Remove(code);
            this._authContext.SaveChanges();
        }

        public UserCodeCheck GetUserCodeByEmail(string email)
        {
            return this._authContext.UserCodeCheck.FirstOrDefault(c => c.Email == email);
        }
        public void UpdateLoggedInUser(LoggedinUser user)
        {
            this._authContext.LoggedinUser.Update(user);
            this._authContext.SaveChanges();
        }
        public async Task<LoggedinUser> GetLoggedInUser(string token)
        {

            try
            {
                if (token == null) return null;
                token = token.Substring(7);
                return await this._authContext
                    .LoggedinUser.Include(m => m.User.UserDetails).FirstOrDefaultAsync(u => u.Token == token);
            }
            catch
            {
                return null;
            }
        }
        public async Task<LoggedinUser> GetLoggedInUser(string userId, int projectId, int platformId)
        {
            return await this._authContext.LoggedinUser.FirstOrDefaultAsync(u => u.UserId == userId &&
            u.ProjectId == projectId && u.PlatformId == platformId);
        }
        public async Task<List<LoggedinUser>> GetLoggedInUsers(string userId)
        {
            return await this._authContext.LoggedinUser.Where(u => u.UserId == userId).ToListAsync();
        }

        public async Task DeleteLoggedInUser(LoggedinUser user)
        {
            this._authContext.LoggedinUser.Remove(user);
            await this._authContext.SaveChangesAsync();
        }
        public async Task deleteEvent(int id)
        {
            //var eventchat = this._authContext.EventChat.Include(m => m.EventData).Where(r => r.EventData.UserId == id).ToList();

            var EventChatAttend = this._authContext.EventChatAttend.Include(m => m.EventData);
            var EventChat = EventChatAttend.Where(n => n.EventData.UserId == id).Select(M1 => M1.Id).ToList();
            var eventmessagechat = this._authContext.Messagedata.Include(m => m.EventChatAttend).Where(r => (r.EventChatAttend != null ? EventChat.Contains(Convert.ToInt32(r.EventChatAttend.Id)) : true)).ToList();
            _authContext.Messagedata.RemoveRange(eventmessagechat);
            _authContext.SaveChanges();

            var EventChatAttenddata = EventChatAttend.Where(n => n.EventData.UserId == id || n.UserattendId == id).ToList();
            _authContext.EventChatAttend.RemoveRange(EventChatAttenddata);

            _authContext.EventChatAttend.RemoveRange(this._authContext.EventChatAttend.Where(n => n.UserattendId == id));
            //_authContext.SaveChanges();
            _authContext.EventData.RemoveRange(GetEventbyid(id));
            await _authContext.SaveChangesAsync();
        }
        public IEnumerable<EventData> GetEventbyid(int id)
        {
            var data = this._authContext.EventData.Where(m => m.UserId == id);
            return (data);
        }


        private void AddToDeletedUserHistoryLog(UserDetails userDeatils)
        {
            try
            {

                DeletedUser deletedUser = new DeletedUser()
                {
                    IdentityUserId = userDeatils.UserId,
                    UserDetail = JsonConvert.SerializeObject(new { UserDetailId = userDeatils.PrimaryId, AspNetUserId = userDeatils.Id, RegistrationDate = userDeatils.User.RegistrationDate, UserloginId = userDeatils.User.UserloginId, Password = userDeatils.pasword }),
                    Email = userDeatils.Email,
                    UserName = userDeatils.User.DisplayedUserName,
                    Date = DateTime.Now
                };

                _authContext.DeletedUsers.Add(deletedUser);
            }
            catch (Exception ex)
            {

                throw ex;
            }


        }

        public async Task DeleteUser_StoredProcedure(UserDetails userDetails)
        {
            try
            {
                AddToDeletedUserHistoryLog(userDetails);

                var tt = await _authContext.Database.ExecuteSqlRawAsync($"EXEC deleteAccount @UserID_int = {userDetails.PrimaryId},@UserID_string = '{userDetails.UserId}'");
                if (tt > 0)
                {
                    _authContext.DeletedUsersLogs.Add(new DeletedUsersLog()
                    {
                        DateTime = DateTime.UtcNow,
                        latitude = userDetails.lat,
                        longitude = userDetails.lang,
                        Gender = userDetails.Gender,
                        UserDetailsJson = JsonConvert.SerializeObject(new { UserID = userDetails.Id, userDetails.userName })
                    });
                    _authContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {

            }

        }
        public async Task DeleteUser(LoggedinUser user, int primary)
        {
            _authContext.LoggedinUser.Remove(user);

            var request = _authContext.Requestes.Where(m => m.UserblockId == primary || m.UserId == primary || m.UserRequestId == primary);
            _authContext.Requestes.RemoveRange(request);

            var LinkAccount = _authContext.LinkAccount.Where(m => m.UserId == primary);
            _authContext.LinkAccount.RemoveRange(LinkAccount);

            var listoftags = _authContext.listoftags.Where(m => m.UserId == primary);
            _authContext.listoftags.RemoveRange(listoftags);

            await deleteEvent(primary);

            var FireBaseDatamodel = _authContext.FireBaseDatamodel.Where(m => m.userid == primary);
            _authContext.FireBaseDatamodel.RemoveRange(FireBaseDatamodel);


            var UserMessages = _authContext.UserMessages.Where(m => m.UserId == primary || m.ToUserId == primary);
            var Messagedata = _authContext.Messagedata.Where(m => m.UserId == primary || UserMessages.Select(m1 => m1.Id).Contains(m.UserMessagessId));




            var Requestes = _authContext.Requestes.Where(m => m.UserRequestId == primary || m.UserblockId == primary || m.UserId == primary);
            _authContext.Requestes.RemoveRange(Requestes);
            _authContext.UserMessages.RemoveRange(UserMessages);
            _authContext.Messagedata.RemoveRange(Messagedata);
            var UserDetails = _authContext.UserDetails.FirstOrDefault(m => m.PrimaryId == primary);
            _authContext.UserDetails.Remove(UserDetails);
            _authContext.SaveChanges();
        }
        public UserDetails GetUserDetails(string userId)
        {

            return this._authContext.UserDetails.FirstOrDefault(c => c.UserId == userId);

        }
        public bool AddUserImages(List<UserImage> files)
        {

            _authContext.UserImages.AddRange(files);
            
            return _authContext.SaveChanges() >0;

        }

        public bool DeleteUserImages(List<UserImage> files)
        {

            _authContext.UserImages.RemoveRange(files);

            return _authContext.SaveChanges() > 0;

        }
        public IEnumerable<UserDetails> GetLISTUserDetails(List<string> userId)
        {

            return this._authContext.UserDetails.Where(c => userId.Contains(c.UserId));

        }
        public List<UserDetails> GetUserDetails()
        {

            return this._authContext.UserDetails.Include(m => m.User).ToList();

        }
        private void CreateUserCookie_Languages(Languages language)
        {

        }
        public LinkAccount GetLinkAccount(int userId)
        {
            return this._authContext.LinkAccount.FirstOrDefault(c => c.Id == userId);

        }
        public listoftags Getlistoftags(int userId)
        {
            return this._authContext.listoftags.FirstOrDefault(c => c.Id == userId);

        }
        public List<LinkAccount> GetallLinkAccount(int userId)
        {
            return this._authContext.LinkAccount.Where(c => c.UserId == userId).ToList();

        }

        public List<UserImage> GetUserImages(int userId)
        {
            return this._authContext.UserImages.Where(c => c.UserDetailsId == userId).ToList();

        }
        public List<listoftags> Getalllistoftags(int userId)
        {
            var data = this._authContext.listoftags.Include(m => m.Interests).Where(c => c.UserId == userId).ToList();
            return data;
        }
        public List<WhatBestDescripsMeList> GetallWhatBestDescripsMeList(int userId)
        {
            var data = this._authContext.WhatBestDescripsMeList.Where(c => c.UserId == userId).ToList();
            return data;
        }
        public List<Iprefertolist> GetallIprefertolist(int userId)
        {
            var data = this._authContext.Iprefertolist.Where(c => c.UserId == userId).ToList();
            return data;
        }
        public void UpdateUserDetails(UserDetails userDetails)
        {
            userDetails.allowmylocation = true;
            this._authContext.UserDetails.Update(userDetails);
            this._authContext.SaveChanges();
        }
        public void UpdateLinkAccount(LinkAccount LinkAccount)
        {
            this._authContext.LinkAccount.Update(LinkAccount);
            this._authContext.SaveChanges();

        }
        public void Updatelistoftags(listoftags listoftags)
        {
            this._authContext.listoftags.Update(listoftags);
            this._authContext.SaveChanges();

        }
        public async Task glob()
        {
            //var Date1 = DateTime.Now.Date;
            //var Date2 = DateTime.Now.AddDays(1).Date;
            var ListOfFireBaseData = new List<FireBaseDatamodel>();
            var pro = this._authContext.EventChatAttend.
                Where(x => x.EventData.eventdate != null && x.removefromevent != true && x.leave != true && x.isrecivedremindernotification != true).ToList();
            pro = pro.Where(m => EqualsUpToSeconds(m.EventData.eventdate.Value.Date, DateTime.Now.Date.AddDays(1))).ToList();
            foreach (var item in pro)
            {
                var user = item.EventData.User;
                //var user= GetUserDetails(item.User.UserId);
                FireBaseData fireBaseInfo = new FireBaseData()
                {
                    //Title = "Event reminder",
                    //Body = $" hi { (user == null ? "Friendzr" : user.userName)} Event starting soon! (" + item.EventData.Title + ")  (" + item.EventData.eventdate.Value.Date.ToString("dd/MM/yyyy") + (item.EventData.allday == false ? " " + item.EventData.eventfrom.Value.ToString(@"hh\:mm") : "") + ")  ",
                    Title = item.EventData.Title,
                    Body = "Event starting soon!",
                    imageUrl = item.EventData.EventTypeListid == 3 ? _configuration["BaseUrl"] : "" + item.EventData.image,
                    muit = false,
                    Action_code = item.EventData.EntityId,
                    Action = "Event_reminder"
                };
                try
                {
                    SendNotificationcs sendNotificationcs = new SendNotificationcs();
                    if (user?.FcmToken != null)
                        //await sendNotificationcs.SendMessageAsync(user.FcmToken, " Event reminder", fireBaseInfo, _environment.WebRootPath);

                        await firebaseManager.SendNotification(user?.FcmToken, fireBaseInfo);
                    if (user != null)
                        ListOfFireBaseData.Add(MessageServes.getFireBaseData(user.PrimaryId, fireBaseInfo));
                    item.isrecivedremindernotification = true;
                    _authContext.SaveChanges();
                }
                catch
                {
                    continue;
                }

            }
            await MessageServes.addFireBaseDatamodel(ListOfFireBaseData);
        }
        public bool EqualsUpToSeconds(DateTime dt1, DateTime dt2)
        {
            var t = dt1.Year == dt2.Year && dt1.Month == dt2.Month && dt1.Day == dt2.Day;
            return t;
        }
        public bool checktooken(string tooken)
        {
            bool flage = true;

            var user = _authContext.UserDetails.Where(n => n.FcmToken == tooken).FirstOrDefault();
            if (user == null)
                flage = false;
            return flage;
        }

        public IQueryable<UserDetails> getallUserDetails()
        {
            var user = _authContext.UserDetails;
            return user;
        }

        public void addlinccount(LinkAccount LinkAccount)
        {
            LinkAccount.EntityId = Guid.NewGuid().ToString();
            this._authContext.LinkAccount.Add(LinkAccount);
            this._authContext.SaveChanges();
        }
        public void addlistoftags(listoftags listoftags)
        {
            listoftags.EntityId = Guid.NewGuid().ToString();
            this._authContext.listoftags.Add(listoftags);
            this._authContext.SaveChanges();
        }
        public void addWhatBestDescripsMe(WhatBestDescripsMeList listoftags)
        {
            listoftags.EntityId = Guid.NewGuid().ToString();
            this._authContext.WhatBestDescripsMeList.Add(listoftags);
            this._authContext.SaveChanges();
        }
        public void addIprefertolist(Iprefertolist listoftags)
        {
            listoftags.EntityId = Guid.NewGuid().ToString();
            this._authContext.Iprefertolist.Add(listoftags);
            this._authContext.SaveChanges();
        }
        public void deleteLinkAccount(List<LinkAccount> LinkAccount)
        {
            this._authContext.LinkAccount.RemoveRange(LinkAccount);
            this._authContext.SaveChanges();
        }

        public void Deletelistoftags(List<listoftags> listoftags)
        {
            this._authContext.listoftags.RemoveRange(listoftags);
            this._authContext.SaveChanges();
        }

        public (double, double) newetnewlocation(double Latitude, double Longitude)
        {
            var d1 = Latitude * (Math.PI / 180.0);
            var num1 = Longitude * (Math.PI / 180.0);
            //var d2 = lat * (Math.PI / 180.0);
            //var num2 = lang * (Math.PI / 180.0) - num1;
            //var d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) +
            //         Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);
            //var daa = 6376500.0 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));
            var new_latitude = Latitude + (d1 / 200) * (45 / Math.PI);
            var new_longitude = Longitude + (num1 / 200) * (45 / Math.PI) / Math.Cos(Latitude * Math.PI / 180);
            //var new_latitude = Latitude + d1;
            //var new_longitude = Longitude + num1;
            return (new_latitude, new_longitude);
        }
        public (List<UserDetails> userDetails, List<int> currentUserInterests) allusers(double myLat, double myLon, string usertype, UserDetails user, AppConfigrationVM AppConfigrationVM, bool sortByInterestMatch)
        {

            int distance = user.distanceFilter == false ? ((AppConfigrationVM.DistanceShowNearbyAccountsInFeed_Min == null ? 0 : (int)AppConfigrationVM.DistanceShowNearbyAccountsInFeed_Min) * 1000) : (int)(user.Manualdistancecontrol * 1000);
            int distancemax = user.distanceFilter == false ? ((AppConfigrationVM.DistanceShowNearbyAccountsInFeed_Max == null ? 0 : (int)AppConfigrationVM.DistanceShowNearbyAccountsInFeed_Max) * 1000) : (int)(user.Manualdistancecontrol * 1000);

            var alluserr = this._authContext.LoggedinUser.Include(n => n.User.UserDetails).Where(p => (p.User.UserDetails.listoftags != null && p.User.UserDetails.listoftags.Count() != 0) && p.User.UserDetails.lat != null && p.User.UserDetails.lang != null).ToList();
            var alluser = alluserr.Where(p => CalculateDistance(myLat, myLon, Convert.ToDouble(p.User.UserDetails.lat), Convert.ToDouble(p.User.UserDetails.lang)) <= ((user.Manualdistancecontrol == 0 ? Convert.ToDouble(distancemax) : Convert.ToDouble(user.Manualdistancecontrol * 1000))))

                .Select(m => m.User.UserDetails).ToList();
            alluser = alluser.Where(m => m.allowmylocation == true).ToList();
            alluser = alluser.Where(m => m.Gender != null).ToList();
            alluser = alluser.Where(p => (user.Filteringaccordingtoage == true ? birtdate(user.agefrom, user.ageto, (p.birthdate == null ? DateTime.Now.Date : p.birthdate.Value.Date)) : true)).ToList();

            alluser = alluser.Where(m => (m.ghostmode == true ? type(m.AppearanceTypes, usertype) : true)).ToList();
            alluser = alluser.Where(m => (user.ghostmode == true ? type(user.AppearanceTypes, m.Gender) : true)).ToList();

            List<int> currentUserInterests = user.listoftags.Select(q => q.InterestsId).ToList();

            if (!sortByInterestMatch)
            {
                return (alluser.OrderBy(p => CalculateDistance(myLat, myLon, Convert.ToDouble(p.User.UserDetails.lat), Convert.ToDouble(p.User.UserDetails.lang))).ToList(), currentUserInterests);
            }

            return (alluser.OrderByDescending(q => ((q.listoftags.Select(q => q.InterestsId).Intersect(currentUserInterests).Count() / Convert.ToDecimal(currentUserInterests.Count())) * 100)).ToList(), currentUserInterests);

        }
        public (List<UserDetails> userDetails, List<int> currentUserInterests) allusersInParallel(double myLat, double myLon, string usertype, UserDetails user, AppConfigrationVM AppConfigrationVM, bool sortByInterestMatch)
        {

            var distanceMax = user.distanceFilter == false ? ((AppConfigrationVM.DistanceShowNearbyAccountsInFeed_Max ?? 0) * 1000) : (int)(user.Manualdistancecontrol * 1000);

            var allLoginUsers = _authContext.LoggedinUser
                .Include(n => n.User.UserDetails)
                .ThenInclude(a=>a.AppearanceTypes)
                .Where(p => 
                    (p.User.UserDetails.listoftags != null && p.User.UserDetails.listoftags.Count() != 0) 
                            && p.User.UserDetails.lat != null && p.User.UserDetails.lang != null
                    && p.User.UserDetails.allowmylocation == true
                    && p.User.UserDetails.Gender != null
                )
                .ToList();

            var allUserDetails = allLoginUsers.Select(m => m.User.UserDetails).ToList();

            allUserDetails = GetClosedUsersByDistance(allUserDetails, user, myLat, myLon, user.Manualdistancecontrol,
                distanceMax, user.Gender);

            List<int> currentUserInterests = user.listoftags.Select(q => q.InterestsId).ToList();

            if (!sortByInterestMatch)
            {
                return (allUserDetails.OrderBy(p => CalculateDistance(myLat, myLon, Convert.ToDouble(p.User.UserDetails.lat), Convert.ToDouble(p.User.UserDetails.lang))).ToList(), currentUserInterests);
            }

            return (allUserDetails.OrderByDescending(q => ((q.listoftags.Select(q => q.InterestsId).Intersect(currentUserInterests).Count() / Convert.ToDecimal(currentUserInterests.Count())) * 100)).ToList(), currentUserInterests);

        }
        private List<UserDetails> GetClosedUsersByDistance(List<UserDetails> closedUsers, UserDetails user, double userLat, double userLong, decimal userManualDistanceControl, int userDistanceMax, string userGender)
        {
            var tasks = new List<Task<List<UserDetails>>>();
            for (int i = 0; i < closedUsers.Count; i += 500)
            {
                var gg = closedUsers.Skip(i).Take(500).ToList();
                tasks.Add(Task.Run(() => CalculateDistanceClosedUsers(gg , user, userLat, userLong, userManualDistanceControl, userDistanceMax, userGender).ToList()));

            }

            var m = (Task.WhenAll(tasks).Result).ToList();
            var oneList = m.SelectMany(a => a).ToList();
            return oneList;
        }

        private IEnumerable<UserDetails> CalculateDistanceClosedUsers(List<UserDetails> data,UserDetails user, double userLat, double userLong, decimal userManualDistanceControl, int userDistanceMax, string userGender)
        {
            data = data.Where(p => (user.Filteringaccordingtoage == true ? birtdate(user.agefrom, user.ageto, (p.birthdate == null ? DateTime.Now.Date : p.birthdate.Value.Date)) : true)).ToList();
            data = data.Where(p =>
                CalculateDistance(userLat, userLong, Convert.ToDouble(p.lat), Convert.ToDouble(p.lang)) <=
                ((user.Manualdistancecontrol == 0
                    ? Convert.ToDouble(userDistanceMax)
                    : Convert.ToDouble(user.Manualdistancecontrol * 1000)))).ToList();

            data = data.Where(m => (m.ghostmode == true ? type(m.AppearanceTypes, userGender) : true)).ToList();
            data = data.Where(m => (user.ghostmode == true ? type(user.AppearanceTypes, m.Gender) : true)).ToList();
            return data;
        }
        public IQueryable<UserDetails> allusers()
        {
            //var com = ("select * from UserDetails");
            return _authContext.UserDetails;
        }

        public (List<UserDetails> userDetails, List<int> currentUserInterests) allusersdirection(double myLat, double myLon, string usertype, UserDetails user, double degree, AppConfigrationVM AppConfigrationVM, bool sortByInterestMatch)
        {
            int distance = user.distanceFilter == false ? ((AppConfigrationVM.DistanceShowNearbyAccountsInFeed_Min == null ? 0 : (int)AppConfigrationVM.DistanceShowNearbyAccountsInFeed_Min) * 1000) : (int)(user.Manualdistancecontrol * 1000);
            int distancemax = user.distanceFilter == false ? ((AppConfigrationVM.DistanceShowNearbyAccountsInFeed_Max == null ? 0 : (int)AppConfigrationVM.DistanceShowNearbyAccountsInFeed_Max) * 1000) : (int)(user.Manualdistancecontrol * 1000);

            var all = this._authContext.LoggedinUser.Include(n => n.User.UserDetails).Where(p => (p.User.UserDetails.listoftags != null && p.User.UserDetails.listoftags.Count() != 0) && p.User.UserDetails.lat != null && p.User.UserDetails.lang != null)
                .Include(n => n.User.UserDetails).Include(m => m.User)
                .Select(m => m.User.UserDetails).Where(a => a.lang != null && a.lat != null).ToList();

            var alluser = all.Where(p => Cardinal2(myLat, myLon, Convert.ToDouble(p.lat), Convert.ToDouble(p.lang), degree)).ToList();
            //var alluser = all.Where(p => Cardinal(myLat, myLon, Convert.ToDouble(p.lat), Convert.ToDouble(p.lang), degree)).ToList();
            alluser = alluser.Where(p => CalculateDistance(myLat, myLon, Convert.ToDouble(p.User.UserDetails.lat), Convert.ToDouble(p.User.UserDetails.lang)) <= (user.Manualdistancecontrol == 0 ? Convert.ToDouble(distancemax) : Convert.ToDouble(user.Manualdistancecontrol * 1000)))
            .ToList();
            alluser = alluser.Where(m => m.allowmylocation == true).ToList();
            alluser = alluser.Where(m => m.Gender != null).ToList();
            alluser = alluser.Where(p => (user.Filteringaccordingtoage == true ? birtdate(user.agefrom, user.ageto, p.birthdate == null ? DateTime.Now.Date : p.birthdate.Value.Date) : true))
         .ToList();
            alluser = alluser.Where(m => m.ghostmode == true ? type(m.AppearanceTypes, usertype) : true).ToList();
            alluser = alluser.Where(m => (user.ghostmode == true ? type(user.AppearanceTypes, m.Gender) : true)).ToList();

            List<int> currentUserInterests = user.listoftags.Select(q => q.InterestsId).ToList();


            return (alluser.OrderByDescending(q => ((q.listoftags.Select(q => q.InterestsId).Intersect(currentUserInterests).Count() / Convert.ToDecimal(currentUserInterests.Count())) * 100)).ToList(), currentUserInterests);
        }

        public List<UserDetails> allusersaroundevent(double myLat, double myLon)
        {
            var alluser = this._authContext.UserDetails.Include(n => n.User).ToList().Where(p => CalculateDistance(myLat, myLon, Convert.ToDouble(p.lat), Convert.ToDouble(p.lang)) <= (Convert.ToDouble(1000)))
            //(minLat <=Convert.ToDouble( p.lat) && Convert.ToDouble(p.lat) <= maxLat) && (minLon <= Convert.ToDouble(p.lang )&& Convert.ToDouble(p.lang )<= maxLon))
                   .AsEnumerable().ToList();

            return alluser.ToList();
        }
        public bool CHECKEVENTLOCATION(String myLat, String myLon, String EVENTLat, String EVENTLon, AppConfigrationVM AppConfigrationVM)
        {

            int distance = ((AppConfigrationVM.DistanceShowNearbyEventsOnMap_Min == null ? 0 : (int)AppConfigrationVM.DistanceShowNearbyEventsOnMap_Min) * 1000);

            int distancemax = ((AppConfigrationVM.DistanceShowNearbyEventsOnMap_Max == null ? 0 : (int)AppConfigrationVM.DistanceShowNearbyEventsOnMap_Max) * 1000);
            bool FLAG = CalculateDistance(Convert.ToDouble(myLat), Convert.ToDouble(myLon), Convert.ToDouble(EVENTLat), Convert.ToDouble(EVENTLon)) <= (Convert.ToDouble(distancemax));


            return FLAG;
        }
        //public List<EventData> allEventDataaroundevent(int userid, double myLat, double myLon)
        //{
        //    var DbF = Microsoft.EntityFrameworkCore.EF.Functions;

        //    var allrequest = _authContext.Requestes.Where(n => n.status == 2 && (n.UserId == userid || n.UserRequestId == userid)).Select(m => m.UserId == userid ? m.UserRequestId : m.UserId);
        //    //var alluser = this._authContext.EventData.Where(n => DbF.DateDiffDay(DateTime.Now, n.eventdateto)  >=0  && !allrequest.Contains(n.UserId)).Where(p => CalculateDistance(myLat, myLon, Convert.ToDouble(p.lat), Convert.ToDouble(p.lang)) <= (Convert.ToDouble(1000)))

        //    var alluser = this._authContext.EventData.ToList().Where(n => n.eventdateto?.Date >= DateTime.Now.Date && !allrequest.Contains(n.UserId)).Where(p => CalculateDistance(myLat, myLon, Convert.ToDouble(p.lat), Convert.ToDouble(p.lang)) <= (Convert.ToDouble(1000)))
        //    //(minLat <=Convert.ToDouble( p.lat) && Convert.ToDouble(p.lat) <= maxLat) && (minLon <= Convert.ToDouble(p.lang )&& Convert.ToDouble(p.lang )<= maxLon))
        //           .AsEnumerable().ToList();

        //    return alluser.ToList();
        //}
        public EventData allEventDataaroundevent(int userid, double myLat, double myLon)
        {
            var allrequest = _authContext.Requestes.Where(n => n.status == 2 && (n.UserId == userid || n.UserRequestId == userid)).Select(m => m.UserId == userid ? m.UserRequestId : m.UserId);
            var alluser = this._authContext.EventData.ToList().Where(n => n.UserId != userid && n.EventTypeList.key != true && n.IsActive == true && n.eventdateto.Value.Date >= DateTime.Now.Date && !allrequest.Contains(n.UserId)).Where(p => CalculateDistance(myLat, myLon, Convert.ToDouble(p.lat), Convert.ToDouble(p.lang)) <= (Convert.ToDouble(1000)))
            //(minLat <=Convert.ToDouble( p.lat) && Convert.ToDouble(p.lat) <= maxLat) && (minLon <= Convert.ToDouble(p.lang )&& Convert.ToDouble(p.lang )<= maxLon))
                   .AsEnumerable();
            return alluser.FirstOrDefault();
        }

        public bool birtdate(int from, int to, DateTime birth)
        {
            bool flag = false;
            int years = DateTime.Now.Date.Year - birth.Date.Year;
            if (years >= from && years <= to)
                flag = true;
            return flag;
        }
        public static bool Cardinal(double lat1d, double long1d, double lat2d, double long2d, double degree)
        {
            var long1 = long1d.ToRadians();
            var lat1 = lat1d.ToRadians();
            var long2 = long2d.ToRadians();
            var lat2 = lat2d.ToRadians();
            double dLon = (long2 - long1);
            double y = Math.Sin(dLon) * Math.Cos(lat2);
            double x = Math.Cos(lat1) * Math.Sin(lat2) - Math.Sin(lat1) * Math.Cos(lat2) * Math.Cos(dLon);

            double brng = Math.Atan2(y, x);
            brng = brng.ToDegrees();
            brng = (brng + 360) % 360;
            var anglediff = (brng - degree + 180 + 360) % 360 - 180;
            if (anglediff <= 45 && anglediff >= -45)
                return true;
            return false;
        }

        public static bool Cardinal2(double myLat, double myLon, double endpointlat, double endpointlang, double degree)
        {
            var degreeDirction = getCardinalDirection(degree);


            var radians = Math.Atan2((endpointlang - myLon), (endpointlat - myLat));

            var compassReading = radians * (180 / Math.PI);

            var coordNames = new[] { "N", "NE", "E", "SE", "S", "SW", "W", "NW" };
            var coordIndex = Math.Round(compassReading / 45);
            if (coordIndex < 0)
            {
                coordIndex = coordIndex + 8;
            };
            var matchwith = coordNames[Convert.ToInt32(coordIndex)];
            var isvalid = matchwith == degreeDirction;
            return isvalid;

        }
        //public static bool Cardinal3(double lat1d, double long1d, double lat2d, double long2d, double degree)
        //{
        //  var degreeDirction=  getCardinalDirection(degree);

        //    var Direct =  Math.Round(Math.Atan2((lat1d - lat2d), (long1d - long2d)) * (8 / Math.PI));
        //    var DirectIndex = Direct < 0 ? Direct + 16 : Direct;
        //    var coordNames = new[] { "N", "NNE", "NE", "ENE", "E", "ESE", "SE", "SSE", "S", "SSW", "SW", "WSW", "W", "WNW", "NW", "NNW", "N" };
        //    //console.log(cardinals[cardIndex(carDirect(30.4, 30.5, 31.5, 3048))]);


        //    var matchwith = coordNames[Convert.ToInt32(DirectIndex)];
        //    var isvalid = matchwith == degreeDirction;
        //    return isvalid;

        //}
        static string getCardinalDirection(double angle)
        {
            //cardinals = ["N", "NNE", "NE", "ENE", "E", "ESE", "SE", "SSE", "S", "SSW", "SW", "WSW", "W", "WNW", "NW", "NNW", "N"];
            //const carDirect = (x0, y0, x1, y1) => Math.round(Math.atan2((x1 - x0), (y1 - y0)) * (8 / Math.PI));
            //const cardIndex = (dir) => dir < 0 ? dir + 16 : dir;
            //console.log(cardinals[cardIndex(0)]);
            //const directions = ['↑ N', '↗ NE', '→ E', '↘ SE', '↓ S', '↙ SW', '← W', '↖ NW'];
            string[] directions = new[] { "N", "NE", "E", "SE", "S", "SW", "W", "NW" };

            var val = (Math.Round(angle / 45) % 8);
            var index = (int)val;
            return directions[index];
        }
        public bool type(int gosttyp, string usertype)
        {
            bool Flag = true;

            if (gosttyp == 1)
            {
                Flag = false;
            }
            else if (gosttyp == 2 && usertype.ToLower() == "male".ToLower())
            {
                Flag = false;
            }
            else if (gosttyp == 3 && usertype.ToLower().Contains("femal".ToLower()))
            {
                Flag = false;
            }
            else if (gosttyp == 4 && usertype.ToLower().Contains("other".ToLower()))
            {
                Flag = false;
            }
            return Flag;
        }
        public bool type(ICollection<AppearanceTypes_UserDetails> gosttyps, string usertype)
        {
            var appearencetypesid = gosttyps.Select(x => x.AppearanceTypeID).ToList();
            bool Flag = true;

            if (appearencetypesid.Contains(1))
            {
                Flag = false;
            }
            else if (appearencetypesid.Contains(2) && usertype.ToLower() == "male".ToLower())
            {
                Flag = false;
            }
            else if (appearencetypesid.Contains(3) && usertype.ToLower().Contains("femal".ToLower()))
            {
                Flag = false;
            }
            else if (appearencetypesid.Contains(4) && usertype.ToLower().Contains("other".ToLower()))
            {
                Flag = false;
            }
            return Flag;
        }

        #region Calculate Distance Region !!!!

        //Old
        public static double CalculateDistance(double Latitude, double Longitude, double lat, double lang)
        {
            var d1 = Latitude * (Math.PI / 180.0);
            var num1 = Longitude * (Math.PI / 180.0);
            var d2 = lat * (Math.PI / 180.0);
            var num2 = lang * (Math.PI / 180.0) - num1;
            var d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) +
                     Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);
            var daa = 6376500.0 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));
            return daa;
        }



        //Old
        // Not Accurate !!!!!!! (Tested)
        public double distanceInMiles(double lon1d, double lat1d, double lon2d, double lat2d)
        {
            var lon1 = lon1d.ToRadians();
            var lat1 = lat1d.ToRadians();
            var lon2 = lon2d.ToRadians();
            var lat2 = lat2d.ToRadians();

            var deltaLon = lon2 - lon1;
            var c = Math.Acos(Math.Sin(lat1) * Math.Sin(lat2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Cos(deltaLon));
            var earthRadius = 3958.76;
            var distInMiles = earthRadius * c;

            return distInMiles;
        }


        #endregion

        public async Task<string> LinkClicks(LoggedinUser loggedinUser, string key)
        {
            string link = "";


            if (Enum.TryParse(key, out LinkClickTypeEnum type))
            {
                UserLinkClick userLinkClick = new UserLinkClick() { UserId = loggedinUser.User.UserDetails.PrimaryId, Date = DateTime.Now, Type = key };

                switch (type)
                {
                    case LinkClickTypeEnum.AboutUs:

                        _authContext.UserLinkClicks.Add(userLinkClick);
                        await _authContext.SaveChangesAsync();
                        link = "https://friendzr.com/about-us/";

                        break;
                    case LinkClickTypeEnum.Share:

                        _authContext.UserLinkClicks.Add(userLinkClick);
                        await _authContext.SaveChangesAsync();
                        link = "http://onelink.to/friendzr";

                        break;

                    case LinkClickTypeEnum.Help:

                        _authContext.UserLinkClicks.Add(userLinkClick);
                        await _authContext.SaveChangesAsync();
                        link = "";

                        break;

                    case LinkClickTypeEnum.PrivacyPolicy:

                        _authContext.UserLinkClicks.Add(userLinkClick);
                        await _authContext.SaveChangesAsync();
                        link = "https://friendzr.com/privacy-policy/";

                        break;

                    case LinkClickTypeEnum.SkipTutorial:

                        _authContext.UserLinkClicks.Add(userLinkClick);
                        await _authContext.SaveChangesAsync();
                        link = "";

                        break;

                    case LinkClickTypeEnum.SupportRequest:

                        _authContext.UserLinkClicks.Add(userLinkClick);
                        await _authContext.SaveChangesAsync();
                        link = "support@friendzr.com";

                        break;

                    case LinkClickTypeEnum.TermsAndConditions:

                        _authContext.UserLinkClicks.Add(userLinkClick);
                        await _authContext.SaveChangesAsync();
                        link = "https://friendzr.com/terms-conditions/";

                        break;

                    case LinkClickTypeEnum.TipsAndGuidance:

                        _authContext.UserLinkClicks.Add(userLinkClick);
                        await _authContext.SaveChangesAsync();
                        link = "https://friendzr.com/tips-and-guidance/";

                        break;
                }
            }

            return link;

        }

        public async Task<(RecommendedPeopleViewModel, string)> RecommendedPeople(UserDetails userDeatil, string userId)
        {

            if (!string.IsNullOrEmpty(userId) && !string.IsNullOrWhiteSpace(userId))
            {
                UserDetails userToSkipe = await _authContext.UserDetails.AsNoTracking().FirstOrDefaultAsync(q => q.UserId == userId);
                if (userToSkipe != null)
                {
                    bool skippedBefore = await _authContext.SkippedUsers.AnyAsync(q => q.UserId == userDeatil.PrimaryId && q.SkippedUserId == userToSkipe.PrimaryId);
                    if (userToSkipe != null && !skippedBefore)
                    {
                        _authContext.SkippedUsers.Add(new SkippedUser() { UserId = userDeatil.PrimaryId, SkippedUserId = userToSkipe.PrimaryId, Date = DateTime.Now });
                        await _authContext.SaveChangesAsync();
                    }
                }

            }

            List<int> skippedUsers = await _authContext.SkippedUsers.Where(q => q.UserId == userDeatil.PrimaryId).Select(q => q.SkippedUserId).ToListAsync();

            AppConfigration appConfigration = await _authContext.AppConfigrations.FirstOrDefaultAsync();

            int distanceMin = ((appConfigration.RecommendedPeopleArea_Min == null ? 0 : (int)appConfigration.RecommendedPeopleArea_Min));
            int distanceMax = ((appConfigration.RecommendedPeopleArea_Max == null ? 0 : (int)appConfigration.RecommendedPeopleArea_Max));

            List<Requestes> currentUserRequests = await _authContext.Requestes.Where(q => q.UserId == userDeatil.PrimaryId || q.UserRequestId == userDeatil.PrimaryId).ToListAsync();

            List<int> recentRequests = new List<int>();

            recentRequests.AddRange(currentUserRequests.Select(q => q.UserId.Value).ToList());
            recentRequests.AddRange(currentUserRequests.Select(q => q.UserRequestId.Value).ToList());

            recentRequests = recentRequests.Distinct().ToList();
            //  Return Users Logged in Only.
            var loggedinUsers = _authContext.LoggedinUser.Select(u=>u.UserId);

            // IEnumerable<UserDetails> userDetails = await _authContext.UserDetails.Include(q => q.User).Include(q => q.Requestesfor).Include(q => q.Requestesto).Include(q => q.listoftags).ThenInclude(q => q.Interests).ToListAsync();
            IEnumerable<UserDetails> userDetails = await _authContext.UserDetails.Include(q => q.User).Include(q => q.Requestesfor).Include(q => q.Requestesto)
                .Include(q => q.listoftags).ThenInclude(q => q.Interests).ToListAsync();
            userDetails = userDetails.Where(q => !string.IsNullOrEmpty(q.lat) && !string.IsNullOrEmpty(q.lang)).ToList();
            userDetails = userDetails.Where(q => q.birthdate != null).ToList();
            userDetails = userDetails.Where(q => q.listoftags.Any()).ToList();
            userDetails = userDetails.Where(q => q.PrimaryId != userDeatil.PrimaryId).ToList();
            userDetails = userDetails.Where(q => !skippedUsers.Contains(q.PrimaryId)).ToList();
            userDetails = userDetails.Where(q => !recentRequests.Contains(q.PrimaryId)).ToList();
            userDetails = userDetails.Where(q => loggedinUsers.Contains(q.UserId)).ToList();
            if (userDeatil.ghostmode)
            {
                userDetails = userDetails.Where(m => m.allowmylocation == true).ToList();
                userDetails = userDetails.Where(m => (m.ghostmode == true ? type(m.AppearanceTypes, userDeatil.Gender) : true)).ToList();
                userDetails = userDetails.Where(m => (userDeatil.ghostmode == true ? type(userDeatil.AppearanceTypes, m.Gender) : true)).ToList();
            }
            if (userDeatil.Filteringaccordingtoage)
            {
                userDetails = userDetails.Where(p => (userDeatil.Filteringaccordingtoage == true ? birtdate(userDeatil.agefrom, userDeatil.ageto, (p.birthdate == null ? DateTime.Now.Date : p.birthdate.Value.Date)) : true)).ToList();
            }
            List<UserDetails> userList = userDetails.ToList();

            List<int> currentUserInterests = userDeatil.listoftags.Select(q => q.InterestsId).ToList();


            //List<RecommendedPeopleViewModel> recommendedPeopleTest = userList.Select(q => new RecommendedPeopleViewModel()
            //{
            //    UserId = q.UserId,
            //    Name = q.User.DisplayedUserName,
            //    Image = $"{_configuration["BaseUrl"]}{q.UserImage}",
            //    DistanceFromYou = Math.Round(googleLocationService.CalculateDistance(Convert.ToDouble(q.lat), Convert.ToDouble(q.lang), Convert.ToDouble(userDeatil.lat), Convert.ToDouble(userDeatil.lang), 'M'), 2),
            //    InterestMatchPercent = (q.listoftags.Select(q => q.InterestsId).Intersect(currentUserInterests).Count() / Convert.ToDecimal(currentUserInterests.Count()) * 100),
            //    MatchedInterests = q.listoftags.Where(q => currentUserInterests.Contains(q.InterestsId)).Select(i => i.Interests.name).ToList()
            //}).Where(q => q.DistanceFromYou <= distanceMax && q.DistanceFromYou >= distanceMin).OrderByDescending(q => q.InterestMatchPercent).ToList();
            RecommendedPeopleViewModel recommendedPeople = userList.Select(q => new RecommendedPeopleViewModel()
            {
                UserId = q.UserId,
                Name = q.User.DisplayedUserName,
                Image = $"{_configuration["BaseUrl"]}{q.UserImage}",
                DistanceFromYou = Math.Round(googleLocationService.CalculateDistance(Convert.ToDouble(q.lat), Convert.ToDouble(q.lang), Convert.ToDouble(userDeatil.lat), Convert.ToDouble(userDeatil.lang), 'M'), 2),
                InterestMatchPercent = (q.listoftags.Select(q => q.InterestsId).Intersect(currentUserInterests).Count() / Convert.ToDecimal(currentUserInterests.Count()) * 100),
                MatchedInterests = q.listoftags.Where(q => currentUserInterests.Contains(q.InterestsId)).Select(i => i.Interests.name).ToList()
            }).Where(q => q.DistanceFromYou <= distanceMax && q.DistanceFromYou >= distanceMin).OrderByDescending(q => q.InterestMatchPercent).FirstOrDefault();

            string message = recommendedPeople != null ? "Your data" : "No more suggestions. Check back later or head to your Feed to see all Friendzrs currently online";

            return (recommendedPeople, message);
        }

        public async Task<(RecommendedPeopleViewModel, string)> RecommendedPeopleFix(UserDetails userDeatil, string userId, bool? previous)
        {
            var currentUserInterests = userDeatil.listoftags.Select(q => q.InterestsId).ToList();

            if ( previous is true)
            {
                var skipped =  _authContext.SkippedUsers
                    .Where(q => q.UserId == userDeatil.PrimaryId)
                    .OrderByDescending(a=>a.Date)
                    .FirstOrDefault();

                if (skipped != null)
                {
                    var previousUser = _authContext.UserDetails
                        .Include(q => q.User)
                        .Include(q => q.AppearanceTypes)
                        .Include(q => q.listoftags)
                        .ThenInclude(q => q.Interests)
                        .FirstOrDefault(q=>q.PrimaryId == skipped.SkippedUserId);
                    RecommendedPeopleViewModel recommendPeople;
                    if (previousUser != null)
                    {
                        recommendPeople = new RecommendedPeopleViewModel()
                        {
                            UserId = previousUser.UserId,
                            ImageIsVerified = previousUser.ImageIsVerified ?? false,
                            Name = previousUser.User.DisplayedUserName,
                            Key = _frindRequest.GetallkeyForFeed(userDeatil.PrimaryId,previousUser.PrimaryId),
                            Image = string.IsNullOrEmpty(previousUser.UserImage)
                                ? _configuration["DefaultImage"]
                                : $"{_configuration["BaseUrl"]}{previousUser.UserImage}",
                            DistanceFromYou = Math.Round(googleLocationService.CalculateDistance(
                                Convert.ToDouble(previousUser.lat),
                                Convert.ToDouble(previousUser.lang),
                                Convert.ToDouble(userDeatil.lat),
                                Convert.ToDouble(userDeatil.lang),
                                'M'), 2),
                            InterestMatchPercent = (previousUser.listoftags
                                    .Select(q => q.InterestsId)
                                    .Intersect(currentUserInterests).Count() /
                                Convert.ToDecimal(currentUserInterests.Count()) * 100),
                            MatchedInterests = previousUser.listoftags
                                .Where(q => currentUserInterests.Contains(q.InterestsId))
                                .Select(i => i.Interests.name).ToList(),
                        };
                        _authContext.SkippedUsers.Remove(skipped);
                        await _authContext.SaveChangesAsync();
                    }
                    else
                    {
                        recommendPeople = null;
                    }

                    var messageData = recommendPeople != null ? "Your data" : "No more suggestions. Check back later or head to your Feed to see all Friendzrs currently online";

                    return (recommendPeople, messageData);
                }
            }
            if (!string.IsNullOrEmpty(userId) && !string.IsNullOrWhiteSpace(userId))
            {
                var userToSkip = await _authContext.UserDetails
                    .AsNoTracking()
                    .FirstOrDefaultAsync(q => q.UserId == userId);

                if (userToSkip != null)
                {
                    var skippedBefore = await _authContext.SkippedUsers
                        .AnyAsync(q => q.UserId == userDeatil.PrimaryId 
                                       && q.SkippedUserId == userToSkip.PrimaryId);

                    if (!skippedBefore)
                    {
                        _authContext.SkippedUsers.Add(new SkippedUser()
                        {
                            UserId = userDeatil.PrimaryId,
                            SkippedUserId = userToSkip.PrimaryId,
                            Date = DateTime.Now
                        });
                        await _authContext.SaveChangesAsync();
                    }
                }

            }

            var skippedUsers = await _authContext.SkippedUsers
                .Where(q => q.UserId == userDeatil.PrimaryId)
                .Select(q => q.SkippedUserId).ToListAsync();

            var appConfig = await _authContext.AppConfigrations.FirstOrDefaultAsync();

            var distanceMin = appConfig.RecommendedPeopleArea_Min ?? 0;
            var distanceMax = appConfig.RecommendedPeopleArea_Max ?? 0;

            var currentUserRequests = await _authContext.Requestes
                .Where(q => q.UserId == userDeatil.PrimaryId 
                            || q.UserRequestId == userDeatil.PrimaryId)
                .ToListAsync();

            var recentRequests = new List<int>();

            recentRequests.AddRange(currentUserRequests.Select(q => q.UserId.Value).ToList());
            recentRequests.AddRange(currentUserRequests.Select(q => q.UserRequestId.Value).ToList());

            recentRequests = recentRequests.Distinct().ToList();
            //  Return Users Logged in Only.
            var loggedInUsers = _authContext.LoggedinUser.Select(u => u.UserId); 
            
            var usersDetails =  _authContext.UserDetails
                .Include(q=>q.User)
                .Include(q => q.AppearanceTypes)
                .Include(q => q.listoftags)
                .ThenInclude(q => q.Interests)
                .Where(
                 q =>  loggedInUsers.Contains(q.UserId)
                                  && !string.IsNullOrEmpty(q.lat) 
                                  && !string.IsNullOrEmpty(q.lang)
                                  && q.birthdate != null 
                                  && q.listoftags.Any() 
                                  && q.PrimaryId != userDeatil.PrimaryId 
                                  && !skippedUsers.Contains(q.PrimaryId) 
                                  && !recentRequests.Contains(q.PrimaryId)).ToList();


            var usersList = await GetRecommendedClosedUsersInParallelInWithBatchesV2(usersDetails, userDeatil,currentUserInterests,distanceMin,distanceMax);

            var recommendedPeople = usersList.OrderByDescending(q => q.InterestMatchPercent).FirstOrDefault();

            var message = recommendedPeople != null ? "Your data" : "No more suggestions. Check back later or head to your Feed to see all Friendzrs currently online";
           
            return (recommendedPeople, message);
        }

        public async Task<IEnumerable<RecommendedPeopleViewModel>> GetRecommendedClosedUsersInParallelInWithBatches(IEnumerable<UserDetails> users, UserDetails user,
            List<int> currentUserInterests, double distanceMin, double distanceMax)
        {
            var tasks = new List<Task<IEnumerable<RecommendedPeopleViewModel>>>();
            var batchSize = 500;
            int numberOfBatches = (int)Math.Ceiling((double)users.Count() / batchSize);

            for (int i = 0; i < numberOfBatches; i++)
            {
                var currentUsers = users.Skip(i * batchSize).Take(batchSize);
                tasks.Add(RecommendedClosedUsers(currentUsers, user, currentUserInterests, distanceMin, distanceMax));
            }

            return (await Task.WhenAll(tasks)).SelectMany(u => u);
        }
        public async Task<IEnumerable<RecommendedPeopleViewModel>> GetRecommendedClosedUsersInParallelInWithBatchesV2(IEnumerable<UserDetails> users, UserDetails user, List<int> currentUserInterests, double distanceMin, double distanceMax)
        {
            var batchSize = 500;
            var numberOfBatches = (int)Math.Ceiling((double)users.Count() / batchSize);
            var tasks = Enumerable.Range(0, numberOfBatches).Select(i =>
            {
                var currentUsers = users.Skip(i * batchSize).Take(batchSize);
                return GetRecommendedUsersV2(currentUsers, user, currentUserInterests, distanceMin, distanceMax);
            });
            var results = await Task.WhenAll(tasks);
            return results.SelectMany(u => u);
        }


        private async Task<IEnumerable<RecommendedPeopleViewModel>> RecommendedClosedUsers(IEnumerable<UserDetails> usersList, UserDetails user , List<int> currentUserInterests ,double distanceMin , double distanceMax)
            {
                usersList = usersList.Where(q => (q.ghostmode != true || type(q.AppearanceTypes, user.Gender)));

                if (user.ghostmode)
                {
                    usersList = usersList.Where(m => m.allowmylocation).ToList();
                    usersList = usersList.Where(m => (user.ghostmode != true || type(user.AppearanceTypes, m.Gender)));

                }
                if (user.Filteringaccordingtoage)
                {
                    usersList = usersList.Where(p => (user.Filteringaccordingtoage != true || birtdate(user.agefrom, user.ageto, p.birthdate?.Date ?? DateTime.Now.Date)));
                }

                var recommendPeople = usersList.Select(q => new RecommendedPeopleViewModel()
                {
                    UserId = q.UserId,
                    ImageIsVerified = q.ImageIsVerified ?? false,
                    Name = q.User.DisplayedUserName,
                    Key =0,/*frindRequest.GetallkeyForFeed(user.PrimaryId, q.PrimaryId),*/
                    Image = string.IsNullOrEmpty(q.UserImage)
                        ? _configuration["DefaultImage"]
                        : $"{_configuration["BaseUrl"]}{q.UserImage}",
                    DistanceFromYou = Math.Round(googleLocationService.CalculateDistance(Convert.ToDouble(q.lat),
                        Convert.ToDouble(q.lang),
                        Convert.ToDouble(user.lat),
                        Convert.ToDouble(user.lang),
                        'M'), 2),
                    InterestMatchPercent = (q.listoftags
                        .Select(q => q.InterestsId)
                        .Intersect(currentUserInterests).Count() / Convert.ToDecimal(currentUserInterests.Count()) * 100),
                    MatchedInterests = q.listoftags
                        .Where(q => currentUserInterests.Contains(q.InterestsId))
                        .Select(i => i.Interests.name).ToList(),
                }).Where(q => q.DistanceFromYou <= distanceMax
                              && q.DistanceFromYou >= distanceMin);

                return recommendPeople;
            }
        public bool IsAgeInRange(int fromAge, int toAge, DateTime birthdate)
        {
            var today = DateTime.Today;
            var age = today.Year - birthdate.Year;
            if (birthdate > today.AddYears(-age))
            {
                age--;
            }

            return age >= fromAge && age <= toAge;
        }

        public bool IsUserTypeValid(ICollection<AppearanceTypes_UserDetails> appearanceTypes, string userType)
        {
            if (appearanceTypes == null || !appearanceTypes.Any())
            {
                return true;
            }

            var targetType = userType.ToLowerInvariant();
            var isMale = targetType == "male";
            var isFemale = targetType.Contains("femal");
            var isOther = targetType.Contains("other");

            var hasNone = appearanceTypes.Any(x => x.AppearanceTypeID == 1);
            var hasMale = appearanceTypes.Any(x => x.AppearanceTypeID == 2 && isMale);
            var hasFemale = appearanceTypes.Any(x => x.AppearanceTypeID == 3 && isFemale);
            var hasOther = appearanceTypes.Any(x => x.AppearanceTypeID == 4 && isOther);

            return !hasNone && !hasMale && !hasFemale && !hasOther;
        }

        private async Task<IEnumerable<RecommendedPeopleViewModel>> GetRecommendedUsersV2(IEnumerable<UserDetails> users, UserDetails currentUser, List<int> currentUserInterests, double distanceMin, double distanceMax)
        {
            var filteredUsers = users.Where(u => (!u.ghostmode || IsUserTypeValid(u.AppearanceTypes, currentUser.Gender)))
                .Where(u => !currentUser.ghostmode || (u.allowmylocation && IsUserTypeValid(currentUser.AppearanceTypes, u.Gender)))
                .Where(u => !currentUser.Filteringaccordingtoage || IsAgeInRange(currentUser.agefrom, currentUser.ageto, u.birthdate?.Date ?? DateTime.Now.Date));

            var recommendedUsers = filteredUsers.Select(u => new RecommendedPeopleViewModel()
                {
                    UserId = u.UserId,
                    ImageIsVerified = u.ImageIsVerified ?? false,
                    Name = u.User.DisplayedUserName,
                    Key = 0, // TODO: Get the key for the feed
                    Image = string.IsNullOrEmpty(u.UserImage) ? _configuration["DefaultImage"] : $"{_configuration["BaseUrl"]}{u.UserImage}",
                    DistanceFromYou = Math.Round(googleLocationService.CalculateDistance(Convert.ToDouble(u.lat), Convert.ToDouble(u.lang), Convert.ToDouble(currentUser.lat), Convert.ToDouble(currentUser.lang), 'M'), 2),
                    InterestMatchPercent = u.listoftags.Select(t => t.InterestsId).Intersect(currentUserInterests).Count() / Convert.ToDecimal(currentUserInterests.Count()) * 100,
                    MatchedInterests = u.listoftags.Where(t => currentUserInterests.Contains(t.InterestsId)).Select(t => t.Interests.name).ToList()
                })
                .Where(u => u.DistanceFromYou <= distanceMax && u.DistanceFromYou >= distanceMin);

            return recommendedUsers;
        }
        // Not Used
        public async Task<(RecommendedPeopleViewModel, string)> RecommendedPeopleFixOld(UserDetails userDeatil, string userId)
            {

                if (!string.IsNullOrEmpty(userId) && !string.IsNullOrWhiteSpace(userId))
                {
                    UserDetails userToSkipe = await _authContext.UserDetails.AsNoTracking().FirstOrDefaultAsync(q => q.UserId == userId);
                    if (userToSkipe != null)
                    {
                        bool skippedBefore = await _authContext.SkippedUsers.AnyAsync(q => q.UserId == userDeatil.PrimaryId && q.SkippedUserId == userToSkipe.PrimaryId);
                        if (userToSkipe != null && !skippedBefore)
                        {
                            _authContext.SkippedUsers.Add(new SkippedUser() { UserId = userDeatil.PrimaryId, SkippedUserId = userToSkipe.PrimaryId, Date = DateTime.Now });
                            await _authContext.SaveChangesAsync();
                        }
                    }

                }

                List<int> skippedUsers = await _authContext.SkippedUsers.Where(q => q.UserId == userDeatil.PrimaryId).Select(q => q.SkippedUserId).ToListAsync();

                AppConfigration appConfigration = await _authContext.AppConfigrations.FirstOrDefaultAsync();

                int distanceMin = ((appConfigration.RecommendedPeopleArea_Min == null ? 0 : (int)appConfigration.RecommendedPeopleArea_Min));
                int distanceMax = ((appConfigration.RecommendedPeopleArea_Max == null ? 0 : (int)appConfigration.RecommendedPeopleArea_Max));

                List<Requestes> currentUserRequests = await _authContext.Requestes.Where(q => q.UserId == userDeatil.PrimaryId || q.UserRequestId == userDeatil.PrimaryId).ToListAsync();

                List<int> recentRequests = new List<int>();

                recentRequests.AddRange(currentUserRequests.Select(q => q.UserId.Value).ToList());
                recentRequests.AddRange(currentUserRequests.Select(q => q.UserRequestId.Value).ToList());

                recentRequests = recentRequests.Distinct().ToList();
                //  Return Users Logged in Only.
                var loggedinUsers = _authContext.LoggedinUser.Select(u => u.UserId);
                var userDetails = await _authContext.UserDetails.Include(q => q.User)
                    //.Include(q => q.Requestesfor).Include(q => q.Requestesto)
                    .Include(q => q.listoftags).ThenInclude(q => q.Interests)
                    .AsQueryable().Where(
                        q => !string.IsNullOrEmpty(q.lat) && !string.IsNullOrEmpty(q.lang) &&
                             q.birthdate != null && q.listoftags.Any() && q.PrimaryId != userDeatil.PrimaryId && !skippedUsers.Contains(q.PrimaryId) &&
                             !recentRequests.Contains(q.PrimaryId) && loggedinUsers.Contains(q.UserId)
                    ).ToListAsync();
                //userDetails = userDetails.Where(q => !string.IsNullOrEmpty(q.lat) && !string.IsNullOrEmpty(q.lang));
                //userDetails = userDetails.Where(q => q.birthdate != null);
                //userDetails = userDetails.Where(q => q.listoftags.Any());
                //userDetails = userDetails.Where(q => q.PrimaryId != userDeatil.PrimaryId);
                //userDetails = userDetails.Where(q => !skippedUsers.Contains(q.PrimaryId));
                //userDetails = userDetails.Where(q => !recentRequests.Contains(q.PrimaryId));
                //userDetails = userDetails.Where(q => loggedinUsers.Contains(q.UserId));
                if (userDeatil.ghostmode)
                {
                    userDetails = userDetails.Where(m => m.allowmylocation == true).ToList();
                    userDetails = userDetails.Where(m => (userDeatil.ghostmode == true ? type(userDeatil.AppearanceTypes, m.Gender) : true)).ToList();
                    //    alluser = alluser.Where(m => (user.ghostmode == true ? type(user.AppearanceTypes, m.Gender) : true)).ToList();

                }
                if (userDeatil.Filteringaccordingtoage)
                {
                    userDetails = userDetails.Where(p => (userDeatil.Filteringaccordingtoage == true ? birtdate(userDeatil.agefrom, userDeatil.ageto, (p.birthdate == null ? DateTime.Now.Date : p.birthdate.Value.Date)) : true)).ToList();
                }
                //List<UserDetails> userList = userDetails.ToList();
                userDetails = userDetails.Where(m => (m.ghostmode == true ? type(m.AppearanceTypes, userDeatil.Gender) : true)).ToList();

                List<int> currentUserInterests = userDeatil.listoftags.Select(q => q.InterestsId).ToList();


                //List<RecommendedPeopleViewModel> recommendedPeopleTest = userList.Select(q => new RecommendedPeopleViewModel()
                //{
                //    UserId = q.UserId,
                //    Name = q.User.DisplayedUserName,
                //    Image = $"{_configuration["BaseUrl"]}{q.UserImage}",
                //    DistanceFromYou = Math.Round(googleLocationService.CalculateDistance(Convert.ToDouble(q.lat), Convert.ToDouble(q.lang), Convert.ToDouble(userDeatil.lat), Convert.ToDouble(userDeatil.lang), 'M'), 2),
                //    InterestMatchPercent = (q.listoftags.Select(q => q.InterestsId).Intersect(currentUserInterests).Count() / Convert.ToDecimal(currentUserInterests.Count()) * 100),
                //    MatchedInterests = q.listoftags.Where(q => currentUserInterests.Contains(q.InterestsId)).Select(i => i.Interests.name).ToList()
                //}).Where(q => q.DistanceFromYou <= distanceMax && q.DistanceFromYou >= distanceMin).OrderByDescending(q => q.InterestMatchPercent).ToList();
                RecommendedPeopleViewModel recommendedPeople = userDetails.Select(q => new RecommendedPeopleViewModel()
                {
                    UserId = q.UserId,
                    ImageIsVerified = q.ImageIsVerified ?? false,
                    Name = q.User.DisplayedUserName,
                    Image = string.IsNullOrEmpty(q.UserImage) ? _configuration["DefaultImage"] : $"{_configuration["BaseUrl"]}{q.UserImage}",
                    DistanceFromYou = Math.Round(googleLocationService.CalculateDistance(Convert.ToDouble(q.lat), Convert.ToDouble(q.lang), Convert.ToDouble(userDeatil.lat), Convert.ToDouble(userDeatil.lang), 'M'), 2),
                    InterestMatchPercent = (q.listoftags.Select(q => q.InterestsId).Intersect(currentUserInterests).Count() / Convert.ToDecimal(currentUserInterests.Count()) * 100),
                    MatchedInterests = q.listoftags.Where(q => currentUserInterests.Contains(q.InterestsId)).Select(i => i.Interests.name).ToList(),
                }).Where(q => q.DistanceFromYou <= distanceMax && q.DistanceFromYou >= distanceMin).OrderByDescending(q => q.InterestMatchPercent).FirstOrDefault();

                string message = recommendedPeople != null ? "Your data" : "No more suggestions. Check back later or head to your Feed to see all Friendzrs currently online";

                return (recommendedPeople, message);
            }
            public async Task<(List<RecentlyConnectedViewModel>, string, int)> RecentlyConnected(UserDetails userDeatil, int pageNumber, int pageSize)
            {
                IQueryable<Requestes> data = _authContext.Requestes.Where(q => (q.UserId == userDeatil.PrimaryId || q.UserRequestId == userDeatil.PrimaryId) && q.status == 1);

                int totalRowCount = data.Count();
                //DateTime.ParseExact(this.Text, "dd/MM/yyyy", null)
                List<Requestes> requestes = await data.OrderByDescending(q => q.AcceptingDate).ThenBy(q => q.regestdata).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

                List<RecentlyConnectedViewModel> RecentlyConnected = requestes.Select(q => new RecentlyConnectedViewModel()
                {
                    UserId = q.UserId == userDeatil.PrimaryId ? q.UserRequest.UserId : q.User.UserId,
                    ImageIsVerified = q.UserRequest?.ImageIsVerified ?? false,
                    Name = q.UserId == userDeatil.PrimaryId ? q.UserRequest.User.DisplayedUserName : q.User.User.DisplayedUserName,
                    Image = q.UserId == userDeatil.PrimaryId 
                        ? string.IsNullOrEmpty(q.UserRequest.UserImage)
                            ? _configuration["DefaultImage"] : _configuration["BaseUrl"] + q.UserRequest.UserImage
                        : string.IsNullOrEmpty(q.User.UserImage) 
                            ? _configuration["DefaultImage"] : _configuration["BaseUrl"] + q.User.UserImage,
                    Date = q.AcceptingDate == null?q.regestdata.ToString("dd/MM/yyyy"):q.AcceptingDate.Value.ToString("dd/MM/yyyy")
                }).OrderByDescending(q => DateTime.ParseExact(q.Date, "dd/MM/yyyy", null)).ToList();

                string message = requestes.Count() != 0 ? "Your data" : "You have no connections yet. Feel free to connect with friendzrs!";

                return (RecentlyConnected, message, totalRowCount);
            }

            #region public

            public async Task<List<UserDetails>> PublicAllUsers(double myLat, double myLon, AppConfigrationVM AppConfigrationVM)
            {
                int distancemax = (int)((AppConfigrationVM.DistanceShowNearbyAccountsInFeed_Max) * 1000);

                var alluserr = await this._authContext.LoggedinUser.Include(n => n.User.UserDetails).Where(p => p.User.UserDetails.lat != null && p.User.UserDetails.lang != null).ToListAsync();

                var alluser = alluserr.Where(p => CalculateDistance(myLat, myLon, Convert.ToDouble(p.User.UserDetails.lat), Convert.ToDouble(p.User.UserDetails.lang)) <= Convert.ToDouble(distancemax)).Select(m => m.User.UserDetails).ToList();

                alluser = alluser.Where(m => m.allowmylocation == true).ToList();

                alluser = alluser.Where(m => m.Gender != null).ToList();

                return alluser.OrderBy(p => CalculateDistance(myLat, myLon, Convert.ToDouble(p.User.UserDetails.lat), Convert.ToDouble(p.User.UserDetails.lang))).ToList();
            }

            public async Task<List<UserDetails>> PublicAllUsersDirection(double myLat, double myLon, double degree, AppConfigrationVM AppConfigrationVM)
            {
                List<UserDetails> usersDetails = await this._authContext.LoggedinUser.Include(n => n.User.UserDetails).Where(p => p.User.UserDetails.lat != null && p.User.UserDetails.lang != null)
                    .Include(n => n.User.UserDetails).Include(m => m.User)
                    .Select(m => m.User.UserDetails).Where(a => a.lang != null && a.lat != null).ToListAsync();

                var alluser = usersDetails.Where(p => Cardinal2(myLat, myLon, Convert.ToDouble(p.lat), Convert.ToDouble(p.lang), degree)).ToList();


                alluser = alluser.Where(p => CalculateDistance(myLat, myLon, Convert.ToDouble(p.User.UserDetails.lat), Convert.ToDouble(p.User.UserDetails.lang)) <= myLon).ToList();
                alluser = alluser.Where(m => m.allowmylocation == true).ToList();
                alluser = alluser.Where(m => m.Gender != null).ToList();

                return alluser.ToList();
            }

            #endregion








    }

}
