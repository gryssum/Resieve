using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Resieve.Example.Data;
using Resieve.Example.Entities;
using Resieve.Mappings;
using Resieve.Mappings.Interfaces;

namespace Resieve.Example.Repository;

public class ProductRepository
{
    private readonly AppDbContext _context;
    private readonly IResieveProcessor _processor;

    public ProductRepository(AppDbContext context, IResieveProcessor processor)
    {
        _context = context;
        _processor = processor;
    }

    public IEnumerable<Product> GetFilteredProducts(ResieveModel model)
    {
        return _processor.Process(model, _context.Products.Include(p => p.Tags));
    }
}

public class ResieveMappingForProduct : IResieveMapping
{
    public void Configure(ResieveMapper mapper)
    {
        mapper.ForProperty<Product>(x => x.Id).CanFilter().CanSort();
        mapper.ForProperty<Product>(x => x.Name).CanFilter().CanSort();
        mapper.ForProperty<Product>(x => x.Price).CanFilter().CanSort();
        mapper.ForProperty<Product>(x => x.Category).CanFilter().CanSort();
        mapper.ForKey<Product>("tags").CanFilter<CustomTagFilter>();
    }
}

public class CustomTagFilter : IResieveCustomFilter<Product>
{
    public Expression<Func<Product, bool>> BuildWhereExpression(string @operator, string value)
    {
        return x => x.Tags.Any(y => y.Name.Contains(value));
    }
}