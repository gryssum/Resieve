using ReSieve.Example.Entities;
using ReSieve.Mappings;

namespace ReSieve.Example.Repository;

public class ProductRepository
{
    private readonly ReSieveProcessor _processor;

    public ProductRepository()
    {
        var mapper = new ReSieveMapper();
        mapper.Property<Product>(x => x.Id).CanFilter().CanSort();
        mapper.Property<Product>(x => x.Name).CanFilter().CanSort();
        mapper.Property<Product>(x => x.Price).CanFilter().CanSort();
        mapper.Property<Product>(x => x.Category).CanFilter().CanSort();
        
        _processor = new ReSieveProcessor(mapper);
    }

    public IEnumerable<Product> GetFilteredProducts(ReSieveModel model)
    {
        return _processor.Process(model, ProductDataSource.GetProducts().AsQueryable());
    }
}
