using Resieve.Example.Entities;
using Resieve.Mappings;

namespace Resieve.Example.Repository;

public class ResieveMappingForProduct : IResieveMapping
{
    public void Configure(ResieveMapper mapper)
    {
        mapper.ForProperty<Product>(x => x.Id).CanFilter().CanSort();
        mapper.ForProperty<Product>(x => x.Name).CanFilter().CanSort();
        mapper.ForProperty<Product>(x => x.Price).CanFilter().CanSort();
        mapper.ForProperty<Product>(x => x.Category).CanFilter().CanSort();
        mapper.ForKey<Product>("Tags.Name").CanFilter<CustomTagFilter>();
    }
}