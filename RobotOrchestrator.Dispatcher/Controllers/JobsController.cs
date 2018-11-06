using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Devices.Common.Exceptions;
using Microsoft.Extensions.Logging;

namespace RobotOrchestrator.Dispatcher.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        private readonly IDispatcher dispatcher;
        private readonly ILogger logger;

        public JobsController(IDispatcher dispatcher, ILogger<JobsController> logger)
        {
            this.dispatcher = dispatcher;
            this.logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> PostJobAsync(Job job)
        {
            try
            {
                await dispatcher.SendJobAsync(job);

                var result = new OkResult();
                return result;
            }
            catch (DeviceNotFoundException)
            {
                var result = new NotFoundObjectResult("Device Not Found");
                return result;
            }
            catch (Exception ex)
            {
                var result = new BadRequestObjectResult(ex.Message);
                return result;
            }
        }
    }
}
