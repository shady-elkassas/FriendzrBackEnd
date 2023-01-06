using System.Threading.Tasks;

namespace Social.HangFireJobs
{
    public interface INotifyUpdateProfileJob
    {
        Task SendUpdateProfileNotification();
    }
}
