// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;

namespace RobotOrchestrator
{
    public interface ICosmosDbClient<T>
    {
        Task<T> UpsertItemAsync(T document, PartitionKey partitionKey);

        Task DeleteItemAsync(string id, PartitionKey partitionKey);

        Task UpdateItemAsync(string id, T document, PartitionKey partitionKey);

        Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> expression = null, Expression<Func<T, object>> orderByExpression = null, int? n = null);

        Task<T> GetItemAsync(string id, PartitionKey partitionKey);
    }
}
