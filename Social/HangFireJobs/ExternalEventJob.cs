using Social.Controllers;
using System.Threading.Tasks;

namespace Social.HangFireJobs
{
    public class ExternalEventJob : IExternalEventJob
    {
        private readonly PublicController _publicController;
        private readonly TicketMasterController _TicketMasterController;
        public ExternalEventJob(PublicController publicController , TicketMasterController TicketMasterController)
        {
            _publicController= publicController;

            _TicketMasterController = TicketMasterController;
        }
        public async Task ExportExternalEvents()
        {
            //TODO:Stop Temporary
           // await _publicController.AddExternalEvents();
        }
    }
}
