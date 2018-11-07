// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace RobotOrchestrator.FleetManager.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class TelemetryController : ControllerBase
    {
        private IFleetManager fleetManager;

        public TelemetryController(IFleetManager fleetManager)
        {
            this.fleetManager = fleetManager;
        }

        [HttpGet("{robotId}")]
        [EnableCors("AllowAllOrigin")]
        public async Task<ActionResult<IEnumerable<RobotTelemetry>>> GetTelemetriesAsync(string robotId, int? numTelemetry)
        {
            IEnumerable<RobotTelemetry> results = new List<RobotTelemetry>();
            try
            {
                results = await fleetManager.GetLatestTelemetriesAsync(robotId, numTelemetry);
            }
            catch(Exception ex)
            {
                var badResult = new BadRequestObjectResult(ex.Message);
                return badResult;
            }

            return new OkObjectResult(results);

        }

        [HttpPut]
        public async Task<ActionResult<IEnumerable<RobotTelemetry>>> PutTelemetriesAsync([FromBody] IEnumerable<RobotTelemetry> robotTelemetry)
        {
            try
            {
                await fleetManager.InsertTelemetriesAndUpdateRobotsAsync(robotTelemetry);
            }
            catch(Exception ex)
            {
                var badResult = new BadRequestObjectResult(ex.Message);
                return badResult;
            }

            var okResult = new OkObjectResult(robotTelemetry);
            return okResult;
        }
    }
}