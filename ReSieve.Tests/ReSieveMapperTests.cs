using ReSieve.Example.Entities;
using Shouldly;

namespace ReSieve.Tests;

public class ReSieveMapperTests
{
    private readonly ReSieveMapper _mapper;

    public ReSieveMapperTests()
    {
        _mapper = new ReSieveMapper();
    }

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

        _mapper.Mappings.ShouldContainKey(typeof(Product));
        _mapper.Mappings.TryGetValue(typeof(Product), out var entityProperties);

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

        _mapper.Mappings.ShouldContainKey(typeof(Product));
        _mapper.Mappings.TryGetValue(typeof(Product), out var entityProperties);

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

        _mapper.Mappings.ShouldContainKey(typeof(Product));
        _mapper.Mappings.TryGetValue(typeof(Product), out var entityProperties);

        entityProperties.ShouldNotBeNull();
        entityProperties.ShouldContain(x => x.Key == nameof(Product.Name));
        
        var sieveMetaData = entityProperties.First(x => x.Key == nameof(Product.Name));
        sieveMetaData.Value.CanSort.ShouldBeTrue();
        sieveMetaData.Value.CanFilter.ShouldBeFalse();
    }
}