namespace RobotOrchestrator
{
    /// <summary>
    /// Generic T is used to differentiate different options for the CosmosDbClient\<T> generic implementations.
    /// So, if multiple CosmosDbClients for different types were needed, then different options could be created for
    /// each IoC configuration.
    /// 
    /// e.g. CosmosDbClient<Robot> would use CosmosDbOptions<Robot>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CosmosDbOptions<T>
    {
        public string DbName { get; set; }
        public string DbCollectionName { get; set; }
        public string PartitionName { get; set; }
    }
}
