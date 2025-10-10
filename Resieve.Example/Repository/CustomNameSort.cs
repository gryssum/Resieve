using Resieve.Example.Entities;
using Resieve.Mappings.Interfaces;

namespace Resieve.Example.Repository;

public class CustomNameSort : IResieveCustomSort<Product>
{
    public IOrderedQueryable<Product> Apply(IQueryable<Product> source, bool isDescending)
    {
        return isDescending ? source.OrderByDescending(x => x.Name) : source.OrderBy(x => x.Name);
    }
    
    public IOrderedQueryable<Product> ApplyThenBy(IOrderedQueryable<Product> source, bool isDescending)
    {
        return isDescending ? source.ThenByDescending(x => x.Name) : source.ThenBy(x => x.Name);
    }
}