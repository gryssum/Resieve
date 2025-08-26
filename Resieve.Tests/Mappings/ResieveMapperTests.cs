using Resieve.Example.Entities;
using Resieve.Mappings;
using Shouldly;

namespace Resieve.Tests.Mappings;

public class ResieveMapperTests
{
    private readonly ResieveMapper _mapper = new();

    [Fact]
    public void CanInstantiateResievePropertyMapper()
    {
        Assert.NotNull(_mapper);
    }

    [Fact]
    public void Mapper_CanFilterOnProperty_MapsAFilterableProperty()
    {
        _mapper
            .Property<Product>(p => p.Name)
            .CanFilter();

        _mapper.PropertyMappings.ShouldContainKey(typeof(Product));
        _mapper.PropertyMappings.TryGetValue(typeof(Product), out var entityProperties);

        entityProperties.ShouldNotBeNull();
        entityProperties.ShouldContain(x => x.Key == nameof(Product.Name));

        var sieveMetaData = entityProperties.First(x => x.Key == nameof(Product.Name));
        sieveMetaData.Value.CanFilter.ShouldBeTrue();
        sieveMetaData.Value.CanSort.ShouldBeFalse();
    }

    [Fact]
    public void Mapper_CanSortOnProperty_MapsASortableProperty()
    {
        _mapper
            .Property<Product>(p => p.Name)
            .CanSort();

        _mapper.PropertyMappings.ShouldContainKey(typeof(Product));
        _mapper.PropertyMappings.TryGetValue(typeof(Product), out var entityProperties);

        entityProperties.ShouldNotBeNull();
        entityProperties.ShouldContain(x => x.Key == nameof(Product.Name));

        var sieveMetaData = entityProperties.First(x => x.Key == nameof(Product.Name));
        sieveMetaData.Value.CanSort.ShouldBeTrue();
        sieveMetaData.Value.CanFilter.ShouldBeFalse();
    }

    [Fact]
    public void Mapper_CanSortAndFilterOnProperty_MapsASortableAndFilterableProperty()
    {
        _mapper
            .Property<Product>(p => p.Name)
            .CanSort()
            .CanFilter();

        _mapper.PropertyMappings.ShouldContainKey(typeof(Product));
        _mapper.PropertyMappings.TryGetValue(typeof(Product), out var entityProperties);

        entityProperties.ShouldNotBeNull();
        entityProperties.ShouldContain(x => x.Key == nameof(Product.Name));

        var sieveMetaData = entityProperties.First(x => x.Key == nameof(Product.Name));
        sieveMetaData.Value.CanSort.ShouldBeTrue();
        sieveMetaData.Value.CanFilter.ShouldBeTrue();
    }
}