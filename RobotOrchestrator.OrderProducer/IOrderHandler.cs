namespace RobotOrchestrator.OrderProducer
{
    public interface IOrderHandler
    {
        void HandleBatch(int batchSize);
    }
}
