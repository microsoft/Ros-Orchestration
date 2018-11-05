using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace RobotOrchestrator.OrderProducer
{
    public class BatchJob
    {
        public bool IsRunning 
        { 
            get; private set;
        }

        public int MaxItems { get; }

        public int BatchSize { get; }

        public int DelayInSecs { get; }

        private readonly ILogger logger;

        private CancellationTokenSource cancellationTokenSource;

        private Task backgroundBatchTask;

        private static readonly Object batchJobLock = new Object();

        public BatchJob(BatchJobOptions batchJobOptions, ILogger logger)
        {
            if (batchJobOptions == null)
            {
                throw new ArgumentNullException("BatchJobOptions cannot be null.");
            }
            
            batchJobOptions.Validate();

            MaxItems = batchJobOptions.MaxItems;
            BatchSize = batchJobOptions.BatchSize;
            DelayInSecs = batchJobOptions.DelayInSecs;
            IsRunning = false;

            this.logger = logger;
        }

        public Task Start(Action<int> handler)
        {
            lock (batchJobLock)
            {
                Task backgroundTask;

                if (!IsRunning)
                {
                    backgroundTask = startBatchJob(handler);
                }
                else
                {
                    throw new InvalidOperationException("Batch job task already started.");
                }

                return backgroundTask;
            }
        }

        public void Stop()
        {
            lock (batchJobLock)
            {
                if (IsRunning)
                {
                    stopBatchJob();
                }
                else
                {
                    throw new InvalidOperationException("Batch job task is not running.");
                }
            }
        }

        private Task startBatchJob(Action<int> handler)
        {
            logger.LogDebug("Starting batch job.");

            IsRunning = true;

            cancellationTokenSource = new CancellationTokenSource();

            var cancellationToken = cancellationTokenSource.Token;

            backgroundBatchTask = Task.Run(async () => await ProcessBatches(handler, cancellationToken),
                    cancellationToken)
                .ContinueWith((antecedent) =>
                {
                    IsRunning = false;
                });

            return backgroundBatchTask;
        }

        private void stopBatchJob()
        {
            try
            {
                logger.LogDebug("Stopping batch job.");

                cancellationTokenSource.Cancel();
                backgroundBatchTask.Wait();
            }
            catch (AggregateException e)
            {
                foreach (var v in e.InnerExceptions)
                {
                    logger.LogError(e.Message + " " + v.Message);
                }
            }
        }

        private async Task ProcessBatches(Action<int> handler, CancellationToken cancellationToken)
        {
            var itemsLeft = MaxItems;

            while (!cancellationToken.IsCancellationRequested && (itemsLeft > 0 || itemsLeft == -1))
            {
                var batchSize = BatchSize;
                
                if (MaxItems != -1)
                {
                    batchSize = Math.Min(itemsLeft, BatchSize);
                    itemsLeft -= batchSize;
                }

                handler?.Invoke(batchSize);
                await Task.Delay(DelayInSecs * 1000);
            }
        }
    }
}
