using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Resieve.Filtering.Lexers;
using Resieve.Mappings;

namespace Resieve.Filtering.ExpressionTrees
{
    public interface IExpressionTreeBuilder
    {
        Expression<Func<TEntity, bool>> BuildFromTokens<TEntity>(List<Token> tokens, Dictionary<string, ResievePropertyMap> customFilters);
    }
}
