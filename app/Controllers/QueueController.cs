using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using MidnightLizard.Commons.Domain.Interfaces;
using System.Threading.Tasks;

namespace MidnightLizard.Impressions.Processor.Controllers
{
    [Route("[controller]/[action]")]
    public class QueueController : Controller
    {
        private readonly IMessagingQueue queue;
        private readonly IApplicationLifetime appLifetime;

        public QueueController(IMessagingQueue impressionsQueue, IApplicationLifetime appLifetime)
        {
            this.queue = impressionsQueue;
            this.appLifetime = appLifetime;
        }

        [HttpPost]
        public async Task Start()
        {
            await this.queue.BeginProcessing(appLifetime.ApplicationStopping);
        }

        [HttpPost]
        public async void Pause()
        {
            await this.queue.PauseProcessing();
        }

        [HttpPost]
        public async void Resume()
        {
            await this.queue.ResumeProcessing(appLifetime.ApplicationStopping);
        }
    }
}
