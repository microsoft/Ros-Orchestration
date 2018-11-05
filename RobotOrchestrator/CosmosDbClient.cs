using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Linq.Expressions;
using System.Net;

namespace RobotOrchestrator
{
    public class CosmosDbClient<T> : ICosmosDbClient<T>
    {
        private string dataBaseName;
        private string collectionName;
        private readonly string partitionName;
        private readonly DocumentClient documentClient;

        private readonly IConfiguration config;
        private readonly IOptions<CosmosDbOptions<T>> options;
        private readonly ILogger logger;

        private DocumentCollection documentCollection;
        private Uri documentCollectionUri;

        public CosmosDbClient(IConfiguration config, IOptions<CosmosDbOptions<T>> options, ILogger<CosmosDbClient<T>> logger)
        {
            this.config = config;
            this.options = options;
            this.logger = logger;

            documentClient = CreateClient();

            dataBaseName = options?.Value.DbName;
            collectionName = options?.Value.DbCollectionName;
            partitionName = options?.Value.PartitionName;

            documentCollectionUri = UriFactory.CreateDocumentCollectionUri(dataBaseName, collectionName);
        }

        /// <summary>
        /// Create client from database config settings
        /// </summary>
        /// <returns></returns>
        private DocumentClient CreateClient()
        {
            string endpointUri = config.GetValue<string>("CosmosDbEndpointUri");
            string primaryKey = config.GetValue<string>("CosmosDbPrimaryKey");

            var documentClient = new DocumentClient(new Uri(endpointUri), primaryKey);
            return documentClient;
        }

        private async Task EnsureDatabaseAndCollectionCreatedAsync()
        {
            var collection = new DocumentCollection
            {
                Id = collectionName
            };

            if (!string.IsNullOrEmpty(partitionName))
            {
                collection.PartitionKey = new PartitionKeyDefinition
                {
                    Paths = new Collection<string> { partitionName }
                };
            }

            await documentClient.CreateDatabaseIfNotExistsAsync(new Database { Id = dataBaseName });

            documentCollection = await documentClient.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(dataBaseName), collection);
        }

        /// <summary>
        /// Create the collection if it doesn't exist, then write the item to collection
        /// </summary>
        /// <param name="itemToWrite"></param>
        /// <param name="partitionKey"></param>
        /// <returns></returns>
        public async Task<T> UpsertItemAsync(T itemToWrite, PartitionKey partitionKey)
        {
            // make sure that collection is created first
            if (documentCollection == null)
            {
                await EnsureDatabaseAndCollectionCreatedAsync();
            }

            try
            {
                await CreateItemIfNotExistsAsync(itemToWrite, partitionKey);
            }
            catch (DocumentClientException ex)
            {
                logger.LogError(ex.Message);
                throw;
            }

            return itemToWrite;
        }

        /// <summary>
        /// If the item doesn't already exist create it, otherwise log exception
        /// </summary>
        /// <param name="itemToWrite"></param>
        /// <param name="partitionKey"></param>
        /// <returns></returns>
        private async Task<Document> CreateItemIfNotExistsAsync(T itemToWrite, PartitionKey partitionKey)
        {
            Document document;

            try
            {
                document = await documentClient.UpsertDocumentAsync(
                    documentCollectionUri,
                    itemToWrite,
                    new RequestOptions { PartitionKey = partitionKey });

                logger.LogInformation("Created Item {0}", itemToWrite);
            }
            catch (DocumentClientException ex)
            {
                logger.LogError(ex.Message);
                throw;
            }

            return document;
        }

        public async Task DeleteItemAsync(string id, PartitionKey partitionKey)
        {
            try
            {
                await documentClient.DeleteDocumentAsync(
                    UriFactory.CreateDocumentUri(dataBaseName, collectionName, id), 
                    new RequestOptions { PartitionKey = partitionKey});
            }
            catch (DocumentClientException ex)
            {
                logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task UpdateItemAsync(string id, T document, PartitionKey partitionKey)
        {
            try
            {
                await documentClient.ReplaceDocumentAsync(
                    UriFactory.CreateDocumentUri(dataBaseName, collectionName, id),
                    document, 
                    new RequestOptions { PartitionKey = partitionKey });
            }
            catch (DocumentClientException ex)
            {
                logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> expression = null, Expression<Func<T, object>> orderByDescExpression = null, int? n = null)
        {
            var query = new QueryHelper<T>(documentClient, documentCollectionUri);

            if (expression != null)
            {
                query.ByExpression(expression);
            }

            if (orderByDescExpression != null)
            {
                query.OrderDescending(orderByDescExpression);
            }

            if (n != null)
            {
                query.Take(n.Value);
            }

            var items = await query.GetQueryItemsAsync();
            return items;
        }

        public async Task<T> GetItemAsync(string id, PartitionKey partitionKey)
        {
            ResourceResponse<Document> response;

            try
            {
                response = await documentClient.ReadDocumentAsync(
                    UriFactory.CreateDocumentUri(dataBaseName, collectionName, id),
                    new RequestOptions { PartitionKey = partitionKey });
            }
            catch (DocumentClientException ex)
            {
                logger.LogError(ex.Message);
                if (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new RecordNotFoundException("Id not exists.", ex);
                }
                else
                {
                    throw;
                }
            }

            var item = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(response.Resource.ToString());
            return item;
        }
    }
}
