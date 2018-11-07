// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Microsoft.Extensions.Logging;

namespace RobotOrchestrator.OrderProducer
{
    public class BatchManager : IBatchManager
    {
        private BatchJob batchJob;

        private static readonly Object batchJobLock = new Object();

        private readonly ILogger logger;

        public BatchManager(ILogger<BatchManager> logger)
        {
            this.logger = logger;
        }

        public void StartBatchJob(Action<int> handler, BatchJobOptions options)
        {
            lock (batchJobLock)
            {
                if (batchJob == null || !batchJob.IsRunning)
                {
                    batchJob = new BatchJob(options, logger);
                    batchJob.Start(handler);
                }
                else
                {
                    throw new InvalidOperationException("Batch job already started.");
                }
            }
        }

        public void StopBatchJob()
        {
            lock (batchJobLock)
            {
                if (batchJob != null)
                {
                    try
                    {
                        batchJob.Stop();
                    }
                    finally
                    {
                        batchJob = null;
                    }
                }
                else
                {
                    throw new InvalidOperationException("Batch job does not exist.");
                }
            }
        }

        public bool HasActiveBatchJob()
        {
            var hasActiveBatchJob = batchJob != null && batchJob.IsRunning;
            return hasActiveBatchJob;
        }
    }
}
