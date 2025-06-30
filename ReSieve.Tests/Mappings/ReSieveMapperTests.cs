using ReSieve.Example.Entities;
using ReSieve.Mappings;
using Shouldly;

namespace ReSieve.Tests.Mappings;

public class ReSieveMapperTests
{
    private readonly ReSieveMapper _mapper = new();

    [Fact]
    public void CanInstantiateReSievePropertyMapper()
    {
        Assert.NotNull(_mapper);
    }

    [Fact]
    public void Mapper_CanFilterOnProperty_MapsAFilterableProperty()
    {
        _mapper
            .Property<Product>(p => p.Name)
            .CanFilter();

        _mapper.FilterMappings.ShouldContainKey(typeof(Product));
        _mapper.FilterMappings.TryGetValue(typeof(Product), out var entityProperties);

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

        _mapper.FilterMappings.ShouldContainKey(typeof(Product));
        _mapper.FilterMappings.TryGetValue(typeof(Product), out var entityProperties);

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

        _mapper.FilterMappings.ShouldContainKey(typeof(Product));
        _mapper.FilterMappings.TryGetValue(typeof(Product), out var entityProperties);

        entityProperties.ShouldNotBeNull();
        entityProperties.ShouldContain(x => x.Key == nameof(Product.Name));

        var sieveMetaData = entityProperties.First(x => x.Key == nameof(Product.Name));
        sieveMetaData.Value.CanSort.ShouldBeTrue();
        sieveMetaData.Value.CanFilter.ShouldBeFalse();
    }
}