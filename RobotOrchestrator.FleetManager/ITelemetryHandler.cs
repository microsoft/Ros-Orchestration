using System.Collections.Generic;
using System.Threading.Tasks;

namespace RobotOrchestrator.FleetManager
{
    public interface ITelemetryHandler
    {
        Task InsertTelemetryAsync(IEnumerable<RobotTelemetry> robotTelemetry);

        Task<IEnumerable<RobotTelemetry>> GetLatestTelemetriesAsync(string robotId, int? n);
    }
}
