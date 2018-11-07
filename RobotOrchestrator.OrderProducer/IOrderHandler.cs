// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace RobotOrchestrator.OrderProducer
{
    public interface IOrderHandler
    {
        void HandleBatch(int batchSize);
    }
}
