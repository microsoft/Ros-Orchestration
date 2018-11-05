using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace RobotOrchestrator
{
    public class Robot
    {
        [JsonProperty(PropertyName ="id")]
        public string DeviceId { get; set; }

        public RobotTelemetry Telemetry { get; set; }
    }
}
