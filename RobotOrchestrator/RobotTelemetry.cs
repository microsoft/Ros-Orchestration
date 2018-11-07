// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace RobotOrchestrator
{
    public class RobotTelemetry
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        public DateTime CreatedDateTime { get; set; } = DateTime.UtcNow;

        public Position Position { get; set; } = new Position(0, 0);

        [JsonConverter(typeof(StringEnumConverter))]
        public RobotStatus Status { get; set; } = RobotStatus.Onboarding;

        public string OrderId { get; set; }

        public string RobotId { get; set; }
    }
}
