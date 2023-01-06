using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Social.Services.Services;

namespace Social.Sercices
{
    public class DerivedBackgroundPrinter : BackgroundService
    {
        private readonly ILogger<DerivedBackgroundPrinter> logger;
        private Timer timer;
        public IUserService Cont;
        public DerivedBackgroundPrinter(IServiceScopeFactory factory, ILogger<DerivedBackgroundPrinter> logger)
        {
            this.logger = logger;
            this.Cont = factory.CreateScope().ServiceProvider.GetRequiredService<IUserService>();


        }
           
     
          public Task StopAsync(CancellationToken cancellationToken)
        {
            timer = new Timer(o => logger.LogInformation(";;"), null, TimeSpan.Zero, TimeSpan.FromHours(5));
            return Task.CompletedTask;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            int hou = DateTime.Now.Hour;
            timer = new Timer(o => Cont.glob(), null, TimeSpan.Zero, TimeSpan.FromHours(5));

          return Task.CompletedTask;
        }
    }
}
