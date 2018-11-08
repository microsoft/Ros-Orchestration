// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RobotOrchestrator
{
    public class QueryHelper<T> : IQueryHelper<T>
    {
        public IQueryable<T> Query { get; set; }

        public QueryHelper(IDocumentClient client, Uri documentCollectionUri)
        {
            // create base query
            Query = client.CreateDocumentQuery<T>(documentCollectionUri, new FeedOptions
            {
                EnableCrossPartitionQuery = true,
                JsonSerializerSettings = new JsonSerializerSettings()
                {
                    DateParseHandling = DateParseHandling.None
                }
            });
        }

        public QueryHelper<T> ByExpression(Expression<Func<T, bool>> expression)
        {
            Query = Query.Where(expression);
            return this;
        }

        public QueryHelper<T> OrderDescending(Expression<Func<T, object>> orderDescendingExpression)
        {
            Query = Query.OrderByDescending(orderDescendingExpression);
            return this;
        }

        public QueryHelper<T> Take(int n)
        {
            Query = Query.Take(n);
            return this;
        }

        public async Task<IEnumerable<T>> GetQueryItemsAsync()
        {
            var items = new List<T>();

            using (var queryable = Query.AsDocumentQuery())
            {
                while (queryable.HasMoreResults)
                {
                    foreach (var t in await queryable.ExecuteNextAsync<T>())
                    {
                        items.Add(t);
                    }
                }
            };

            return items;
        }
    }
}
