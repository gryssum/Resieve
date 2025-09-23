using System.Linq.Expressions;
using Resieve.Tests.Mocks;
using Resieve.Exceptions;
using Resieve.Mappings;
using Resieve.Mappings.Interfaces;
using Shouldly;

namespace Resieve.Tests.Mappings;

public class ResieveMapperTests
{
    private readonly ResieveMapper _mapper = new ResieveMapper(new List<IResieveMapping>());

    [Fact]
    public void CanInstantiateResievePropertyMapper()
    {
        Assert.NotNull(_mapper);
    }

    [Fact]
    public void Mapper_CanFilterOnProperty_MapsAFilterableProperty()
    {
        _mapper
            .ForProperty<Product>(p => p.Name)
            .CanFilter();

        _mapper.PropertyMappings.ShouldContainKey(typeof(Product));
        _mapper.PropertyMappings.TryGetValue(typeof(Product), out var entityProperties);

        entityProperties.ShouldNotBeNull();
        entityProperties.ShouldContain(x => x.Key == nameof(Product.Name));

        var sieveMetaData = entityProperties.First(x => x.Key == nameof(Product.Name));
        sieveMetaData.Value.CanFilter.ShouldBeTrue();
        sieveMetaData.Value.CanSort.ShouldBeFalse();
        sieveMetaData.Value.CustomFilter.ShouldBeNull();
    }

    [Fact]
    public void Mapper_CanSortOnProperty_MapsASortableProperty()
    {
        _mapper
            .ForProperty<Product>(p => p.Name)
            .CanSort();

        _mapper.PropertyMappings.ShouldContainKey(typeof(Product));
        _mapper.PropertyMappings.TryGetValue(typeof(Product), out var entityProperties);

        entityProperties.ShouldNotBeNull();
        entityProperties.ShouldContain(x => x.Key == nameof(Product.Name));

        var sieveMetaData = entityProperties.First(x => x.Key == nameof(Product.Name));
        sieveMetaData.Value.CanSort.ShouldBeTrue();
        sieveMetaData.Value.CanFilter.ShouldBeFalse();
        sieveMetaData.Value.CustomSort.ShouldBeNull();
    }

    [Fact]
    public void Mapper_CanSortAndFilterOnProperty_MapsASortableAndFilterableProperty()
    {
        _mapper
            .ForProperty<Product>(p => p.Name)
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
    
    [Fact]
    public void Mapper_CanFilterWithCustomFilter_MapsACustomFilterableProperty()
    {
        _mapper
            .ForProperty<Product>(p => p.Name)
            .CanFilter<NameCustomFilter>();
        
        _mapper.PropertyMappings.ShouldContainKey(typeof(Product));
        _mapper.PropertyMappings.TryGetValue(typeof(Product), out var entityProperties);
        
        entityProperties.ShouldNotBeNull();
        entityProperties.ShouldContain(x => x.Key == nameof(Product.Name));
        
        entityProperties.TryGetValue(nameof(Product.Name), out var sieveMetaData).ShouldBeTrue();
        sieveMetaData.ShouldNotBeNull();
        sieveMetaData.CanFilter.ShouldBeTrue();
        sieveMetaData.CustomFilter.ShouldBe(typeof(NameCustomFilter));
    }

    [Fact]
    public void Mapper_CanSortWithCustomSort_MapsACustomSortableProperty()
    {
        _mapper
            .ForProperty<Product>(p => p.Name)
            .CanSort<NameCustomSort>();
    
        _mapper.PropertyMappings.ShouldContainKey(typeof(Product));
        _mapper.PropertyMappings.TryGetValue(typeof(Product), out var entityProperties);
    
        entityProperties.ShouldNotBeNull();
        entityProperties.ShouldContain(x => x.Key == nameof(Product.Name));
    
        entityProperties.TryGetValue(nameof(Product.Name), out var sieveMetaData).ShouldBeTrue();
        sieveMetaData.ShouldNotBeNull();
        sieveMetaData.CanSort.ShouldBeTrue();
        sieveMetaData.CustomSort.ShouldBe(typeof(NameCustomSort));
    }

    [Fact]
    public void Mapper_CanCustomFilterWithStringKey_ThrowsError()
    {
        _mapper
            .ForKey<Product>("CustomName")
            .CanFilter<NameCustomFilter>();
        
        _mapper.PropertyMappings.ShouldContainKey(typeof(Product));
        _mapper.PropertyMappings.TryGetValue(typeof(Product), out var entityProperties);
        
        entityProperties.ShouldNotBeNull();
        entityProperties.ShouldContain(x => x.Key == "CustomName");
        
        entityProperties.TryGetValue("CustomName", out var sieveMetaData).ShouldBeTrue();
        sieveMetaData.ShouldNotBeNull();
        sieveMetaData.CanFilter.ShouldBeTrue();
        sieveMetaData.CustomFilter.ShouldBe(typeof(NameCustomFilter));
    }

    [Fact]
    public void Mapper_CanFilterWithStringKey_ThrowsError()
    {
        var act = () => _mapper
            .ForKey<Product>("CustomName")
            .CanFilter();

        Assert.Throws<ResieveMappingException>(act);
    }
    
    private class NameCustomFilter : IResieveCustomFilter<Product>
    {
        public Expression<Func<Product, bool>> BuildWhereExpression(string @operator, string value)
        {
            throw new NotImplementedException();
        }
    }
    
    private class NameCustomSort : IResieveCustomSort<Product>
    {
        public IOrderedQueryable<Product> Apply(IQueryable<Product> source, string propertyName, bool descending)
        {
            throw new NotImplementedException();
        }
        public IOrderedQueryable<Product> ApplyThenBy(IOrderedQueryable<Product> source, string propertyName, bool isDescending)
        {
            throw new NotImplementedException();
        }
    }
}

