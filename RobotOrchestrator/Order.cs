// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace RobotOrchestrator
{
    public class Order
    {
        /// <summary>
        /// Order Id
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Start position pickup of delivery order
        /// </summary>
        public Position StartPosition { get; set; }

        /// <summary>
        /// End position pickup of delivery order
        /// </summary>
        public Position EndPosition { get; set; }

        /// <summary>
        /// Set the status of the order
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public OrderStatus Status { get; set; } = OrderStatus.New;

        /// <summary>
        /// Jobs created for the order
        /// </summary>
        public List<Job> Jobs { get; set; } = new List<Job>();

        /// <summary>
        /// Time the order was created
        /// </summary>
        public DateTime CreatedDateTime { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Message accompanied with order
        /// Note: for now this is a placeholder and this can be any
        /// object for order creation later on
        /// </summary>
        public string Message { get; set; }
    }
}
