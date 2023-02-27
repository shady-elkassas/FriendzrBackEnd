using Social.Controllers;
using Social.FireBase;
using System.Threading.Tasks;

namespace Social.HangFireJobs
{
    public class NotifyUpdateProfileJob : INotifyUpdateProfileJob
    {
        private AccountupdateController _accountupdateController { get; set; }
        public NotifyUpdateProfileJob(AccountupdateController accountupdateController)
        {
            this._accountupdateController = accountupdateController;
        }
        public async Task SendUpdateProfileNotification()
        {
            // Stop Temporary
            // await this._accountupdateController.SendUpdateProfileNotification();           
        }
    }
}
