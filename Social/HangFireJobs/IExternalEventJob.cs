using System.Threading.Tasks;

namespace Social.HangFireJobs
{
    public interface IExternalEventJob
    {
        Task ExportExternalEvents();
    }
}
