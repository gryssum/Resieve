using System;
using System.Linq.Expressions;

namespace Resieve.Mappings.Interfaces
{
    public interface IResieveCustomFilter<TEntity>
    {
        Expression<Func<TEntity, bool>> BuildWhereExpression(string @operator, string value);
    }
}