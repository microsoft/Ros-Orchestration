// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace RobotOrchestrator.OrderProducer
{
    public class BatchJobOptions
    {
        // Set to -1 for infinite items
        public int MaxItems { get; set; } = 10;

        public int BatchSize { get; set; } = 1;

        public int DelayInSecs { get; set; } = 5;

        public void Validate() 
        {
            if (MaxItems < -1 || MaxItems == 0 || BatchSize <= 0 || DelayInSecs < 0) 
            {
                throw new ArgumentException("BatchJobOptions are not valid");
            }
        }
    }
}
