using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace RobotOrchestrator
{
    public class Job
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Assigned robot for job
        /// </summary>
        public string RobotId { get; set; }

        /// <summary>
        /// Parent orderId for the job
        /// </summary>
        public Guid OrderId { get; set; }

        /// <summary>
        /// Status for the job
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]

        public JobStatus Status { get; set; }

        /// <summary>
        /// Start position of delivery job
        /// </summary>
        public Position StartPosition { get; set; }

        /// <summary>
        /// End position of delivery job
        /// </summary>
        public Position EndPosition { get; set; }
    }
}
