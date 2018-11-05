using System;

namespace RobotOrchestrator.OrderProducer
{
    public interface IBatchManager
    {
        void StartBatchJob(Action<int> handler, BatchJobOptions options);

        void StopBatchJob();

        bool HasActiveBatchJob();
    }
}
