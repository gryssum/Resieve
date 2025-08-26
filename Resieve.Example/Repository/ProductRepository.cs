using Resieve.Example.Entities;
using Resieve.Mappings;

namespace Resieve.Example.Repository;

public class ProductRepository(IResieveProcessor processor)
{
    public IEnumerable<Product> GetFilteredProducts(ResieveModel model)
    {
        return processor.Process(model, ProductDataSource.GetProducts().AsQueryable());
    }
}

public class ResieveMappingForProduct : IResieveMapping
{
    public void Configure(ResieveMapper mapper)
    {
        mapper.Property<Product>(x => x.Id).CanFilter().CanSort();
        mapper.Property<Product>(x => x.Name).CanFilter().CanSort();
        mapper.Property<Product>(x => x.Price).CanFilter().CanSort();
        mapper.Property<Product>(x => x.Category).CanFilter().CanSort();
    }
}