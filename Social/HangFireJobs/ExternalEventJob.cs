using Social.Controllers;
using System.Threading.Tasks;

namespace Social.HangFireJobs
{
    public class ExternalEventJob : IExternalEventJob
    {
        private readonly PublicController _publicController;
        public ExternalEventJob(PublicController publicController)
        {
            _publicController= publicController; 
        }
        public async Task ExportExternalEvents()
        {
            //TODO:Stop Temporary
           // await _publicController.AddExternalEvents();
        }
    }
}
