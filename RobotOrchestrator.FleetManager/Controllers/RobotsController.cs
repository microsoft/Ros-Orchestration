// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RobotOrchestrator.FleetManager.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class RobotsController : ControllerBase
    {
        private readonly IFleetManager fleetManager;

        private readonly ILogger logger;

        public RobotsController(IFleetManager fleetManager, ILogger<RobotsController> logger)
        {
            this.fleetManager = fleetManager;
            this.logger = logger;
        }

        [HttpGet]
        [EnableCors("AllowAllOrigin")]
        public async Task<ActionResult<IEnumerable<Robot>>> GetRobotsAsync(RobotStatus? status)
        {
            IEnumerable<Robot> robots = new List<Robot>();

            try
            {
                robots = await fleetManager.GetRobotsAsync(status);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                var badResult = new BadRequestObjectResult(ex.Message);
                return badResult;
            }

            var robotsResult = new OkObjectResult(robots);
            return robotsResult;
        }

        [HttpGet("{deviceId}")]
        public async Task<ActionResult<Robot>> GetRobotAsync(string deviceId)
        {
            Robot robot;
            try
            {
                robot = await fleetManager.GetRobotAsync(deviceId);
            }
            catch (RecordNotFoundException)
            {
                var notFoundResult = new NotFoundResult();
                return notFoundResult;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                var notFound = new NotFoundObjectResult(ex.Message);
                return notFound;
            }

            var result = new OkObjectResult(robot);
            return result;
        }

        [HttpPut("{deviceId}/connection")]
        public async Task<IActionResult> PutRobotConnectionAsync(string deviceId)
        {
            string connectionString;
            try
            {
                connectionString = await fleetManager.CreateIfNotExistsRobotConnectionAsync(deviceId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                var badRequest = new BadRequestObjectResult(ex.Message);
                return badRequest;
            }

            var connectionResult = new CreatedResult(nameof(GetRobotAsync), connectionString);
            return connectionResult;
        }

        [HttpPost("{deviceId}")]
        public async Task<IActionResult> PostRobotAsync(string deviceId)
        {
            Robot robot;
            try
            {
                robot = await fleetManager.UpsertRobotAsync(deviceId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                var badRequest = new BadRequestObjectResult(ex.Message);
                return badRequest;
            }

            var robotResult = CreatedAtAction(nameof(GetRobotAsync), new { deviceId = robot.DeviceId }, robot);
            return robotResult;
        }

        [HttpPut("{deviceId}")]
        public async Task<IActionResult> UpdateRobotAsync([FromBody] Robot robot)
        {
            try
            {
                await fleetManager.UpdateRobotAsync(robot);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                var notFound = new NotFoundObjectResult(ex.Message);
                return notFound;
            }

            return new OkObjectResult(robot);

        }

        [HttpDelete("{deviceId}")]
        public async Task<IActionResult> DeleteRobotAsync(string deviceId)
        {
            try
            {
                await fleetManager.DeleteRobotAsync(deviceId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                var notFound = new NotFoundObjectResult(ex.Message);
                return notFound;
            }

            return new OkResult(); 
        }
    }
}