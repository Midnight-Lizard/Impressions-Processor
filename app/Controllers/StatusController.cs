using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MidnightLizard.Commons.Domain.Interfaces;

namespace MidnightLizard.Impressions.Processor.Controllers
{
    [Route("[controller]/[action]")]
    public class StatusController : Controller
    {
        private readonly IMessagingQueue queue;

        public StatusController(IMessagingQueue impressionsQueue)
        {
            this.queue = impressionsQueue;
        }

        public IActionResult IsReady()
        {
            return Ok("impressions processor is ready");
        }

        public IActionResult IsAlive()
        {
            if (this.queue.CheckStatus())
            {
                return Ok("impressions processor is alive");
            }
            return BadRequest("impressions processor has too many errors and should be restarted");
        }
    }
}
