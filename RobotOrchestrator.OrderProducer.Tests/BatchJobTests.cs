using Xunit;
using System.Threading.Tasks;
using Moq;
using Microsoft.Extensions.Logging;

namespace RobotOrchestrator.OrderProducer.Tests
{
    public class BatchJobTests
    {
        [Theory]
        [InlineData(1, 1)]
        [InlineData(10, 3)]
        [InlineData(50, 10)]
        public async Task StartBatchJob_WithMaxItemsAndBatchSize_RunsAppropriateNumberOfTimes(int maxItems, int batchSize)
        {
            var expectedIterations = (maxItems / batchSize) + (maxItems % batchSize == 0 ? 0 : 1);

            BatchJobOptions options = new BatchJobOptions()
            {
                MaxItems = maxItems,
                BatchSize = batchSize,
                DelayInSecs = 0
            };

            BatchJob job = new BatchJob(options, Mock.Of<ILogger>());

            IOrderHandler handler = Mock.Of<IOrderHandler>();
            Mock.Get(handler).Setup(h => h.HandleBatch(It.IsAny<int>())).Verifiable();

            await job.Start(handler.HandleBatch);

            Mock.Get(handler).Verify((m) => m.HandleBatch(It.IsAny<int>()), Times.Exactly(expectedIterations));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        public void StartBatchJob_WithInfiniteItems_RunsMoreThanArbitraryTimes(int arbitraryRuns)
        {
            var batchSize = 1;

            BatchJobOptions options = new BatchJobOptions()
            {
                MaxItems = -1,
                BatchSize = batchSize,
                DelayInSecs = 0
            };

            BatchJob job = new BatchJob(options, Mock.Of<ILogger>());

            IOrderHandler handler = Mock.Of<IOrderHandler>();
            Mock.Get(handler).Setup(h => h.HandleBatch(batchSize)).Verifiable();

            // fire and forget since it is infinite thread
            job.Start(handler.HandleBatch).Wait(1000);

            // give time for iterations
            job.Stop();

            Mock.Get(handler).Verify((m) => m.HandleBatch(batchSize), Times.AtLeast(arbitraryRuns));
        }
    }
}