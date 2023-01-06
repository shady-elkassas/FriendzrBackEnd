
using Social.Entity.ModelView;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Social.FireBase
{
  public  class SendNotificationcs
    {
        private readonly IHostingEnvironment _environment;
        private readonly IConfiguration _configuration;

        public SendNotificationcs() { }
        public SendNotificationcs(IConfiguration configuration, IHostingEnvironment environment)
        {
            this._configuration = configuration;
            this._environment = environment;
        }
        public async Task SendMessageAsync(string to,string valueOfKey, FireBaseData fireBaseData,string pathFile)
        {
            try
            {

                FirebaseApp app = null;
                var token = to;
                Random random = new Random();
                var code = random.Next(1, 10000000).ToString("D7");
                try
                {
                    var path = Path.Combine(pathFile, "FireBase\\friendzr-1631017594822-firebase-adminsdk-3o7sj-26049eb40c.json");
                    app = FirebaseApp.Create(new AppOptions()
                    {
                        Credential = GoogleCredential.FromFile(path)
                    }, "myApp"+Convert.ToInt32(code.ToString()));
                }
                catch (Exception ex)
                {
                    app = FirebaseApp.GetInstance("myApp");
                }

                var fcm = FirebaseAdmin.Messaging.FirebaseMessaging.GetMessaging(app);
                Message message = new Message()
                {
                    Notification = new Notification
                    {
                        Title = fireBaseData.Title,
                        Body = fireBaseData.Body,
                        ImageUrl = fireBaseData.imageUrl,
                        //Action_code = fireBaseData.Action_code,
                        //Action_code = fireBaseData.Messagetype,
                    },
                    Data = new Dictionary<string, string>()
                 {
                     {"Action_code", fireBaseData.Action_code },
                     {"IsWhitelabel",fireBaseData.IsWhitelabel.ToString() },
                     {"Action", fireBaseData.Action },
                     {"Messagetype", fireBaseData.Messagetype.ToString() },
                     {"muit", fireBaseData.muit.ToString()},
                     {"userimage", fireBaseData.senderImage },
                     {"name", fireBaseData.name },
                     {"date", fireBaseData.date },
                     {"time", fireBaseData.time},
                     {"sound", "default" },

                 },

                    Token = token
                };

                string result = await fcm.SendAsync(message);


            }
            catch (Exception ex)
            {
                throw ex;

            }
        }
    }
}
