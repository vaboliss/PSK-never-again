using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkerController : ControllerBase
    {
        private readonly IWorker workerService;
        private readonly ITopic topicService;

        public WorkerController(IWorker worker, ITopic topic)
        {
            workerService = worker;
            topicService = topic;
        }
        [HttpPost]
        public IActionResult GetWorkerByTopic([FromBody]int topicId)
        {
            var workers = workerService.GetWorkersByTopic(topicId);

            if (workers is null)
            {
                return NotFound();
            }
            else
            {
                return Ok(workers);
            }
        }
    }
}
