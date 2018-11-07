// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RobotOrchestrator
{
    public interface IQueryHelper<T>
    {
        QueryHelper<T> ByExpression(Expression<Func<T, bool>> expression);

        QueryHelper<T> OrderDescending(Expression<Func<T, object>> orderDescendingExpression);

        QueryHelper<T> Take(int n);

        Task<IEnumerable<T>> GetQueryItemsAsync();
    }
}
