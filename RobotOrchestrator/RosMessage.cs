using Newtonsoft.Json;

namespace RobotOrchestrator
{
    public class RosMessage<T>
    {
        [JsonProperty(PropertyName = "topic")]
        public string Topic { get; set; }

        [JsonProperty(PropertyName = "msg_type")]
        public string MessageType { get; set; }

        [JsonProperty(PropertyName = "payload")]
        public T Payload { get; set; }
    }
}
