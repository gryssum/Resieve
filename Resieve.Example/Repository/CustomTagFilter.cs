using System.Linq.Expressions;
using Resieve.Example.Entities;
using Resieve.Mappings.Interfaces;

namespace Resieve.Example.Repository;

public class CustomTagFilter : IResieveCustomFilter<Product>
{
    public Expression<Func<Product, bool>> BuildWhereExpression(string @operator, string value)
    {
        return x => x.Tags.Any(y => y.Name.Contains(value));
    }
}