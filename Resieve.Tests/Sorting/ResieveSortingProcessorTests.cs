using Microsoft.Extensions.DependencyInjection;
using Resieve.Tests.Mocks;
using Resieve.Exceptions;
using Resieve.Mappings;
using Resieve.Mappings.Interfaces;
using Resieve.Sorting;
using Resieve.Tests.Builders;
using Shouldly;

namespace Resieve.Tests.Sorting;

public class ResieveSortingProcessorTests
{
    private readonly ServiceProvider _provider;
    private readonly ResieveSortingProcessor _processor;

    public ResieveSortingProcessorTests()
    {
        _provider = new ServiceCollection().BuildServiceProvider();
        _processor = new(_provider, GetProductMapper());
    }

    private readonly IQueryable<Product> _products = new List<Product>
    {
        A.Product.WithId(1).WithName("Apple").WithPrice(1.99m).Build(),
        A.Product.WithId(2).WithName("Banana").WithPrice(0.99m).Build(),
        A.Product.WithId(3).WithName("Carrot").WithPrice(0.59m).Build(),
        A.Product.WithId(4).WithName("Donut").WithPrice(2.49m).Build(),
        A.Product.WithId(5).WithName("Eggplant").WithPrice(1.29m).Build(),
        // Add duplicate names for multi-sorting test
        A.Product.WithId(6).WithName("Apple").WithPrice(2.99m).Build(),
        A.Product.WithId(7).WithName("Banana").WithPrice(0.49m).Build()
    }.AsQueryable();

    private static ResieveMapper GetProductMapper()
    {
        var mapper = new ResieveMapper(new List<IResieveMapping>());
        mapper.ForProperty<Product>(x => x.Name).CanSort();
        mapper.ForProperty<Product>(x => x.Price).CanSort();
        return mapper;
    }

    [Fact]
    public void Apply_ReturnsSource_WhenSortsIsEmpty()
    {
        var model = new ResieveModel {Sorts = string.Empty};
        var result = _processor.Apply(model, _products);
        result.ShouldBe(_products);
    }

    [Fact]
    public void Apply_ReturnsSource_WhenSortTermNameIsNullOrWhitespace()
    {
        var model = new ResieveModel {Sorts = "   "};
        var result = _processor.Apply(model, _products);
        result.ShouldBe(_products);
    }

    [Fact]
    public void Apply_SortsByNameAscending_WhenSortTermProvided()
    {
        // Arrange
        var model = new ResieveModel {Sorts = "Name"};

        // Act
        var result = _processor.Apply(model, _products).ToList();

        // Assert
        result
            .Select(p => p.Name)
            .ShouldBe([
                "Apple",
                "Apple",
                "Banana",
                "Banana",
                "Carrot",
                "Donut",
                "Eggplant"
            ]);
    }

    [Fact]
    public void Apply_SortsByNameCaseInsensitive_WhenSortTermProvided()
    {
        // Arrange
        var model = new ResieveModel {Sorts = "name"};

        // Act
        var result = _processor.Apply(model, _products).ToList();

        // Assert
        result
            .Select(p => p.Name)
            .ShouldBe([
                "Apple",
                "Apple",
                "Banana",
                "Banana",
                "Carrot",
                "Donut",
                "Eggplant"
            ]);
    }

    [Fact]
    public void Apply_SortsByNameDescending_WhenDescendingIsTrue()
    {
        var model = new ResieveModel {Sorts = "-Name"};
        var result = _processor.Apply(model, _products).ToList();
        result.Select(p => p.Name).ShouldBe([
            "Eggplant",
            "Donut",
            "Carrot",
            "Banana",
            "Banana",
            "Apple",
            "Apple"
        ]);
    }

    [Fact]
    public void Apply_SortsByMultipleTerms_NameThenPriceDescending()
    {
        var model = new ResieveModel {Sorts = "-Name,Price"};
        var result = _processor.Apply(model, _products).ToList();
        // Should be sorted by Name ascending, then by Price descending for same Name
        result.Select(p => (p.Name, p.Price)).ShouldBe([
            ("Eggplant", 1.29m),
            ("Donut", 2.49m),
            ("Carrot", 0.59m),
            ("Banana", 0.49m),
            ("Banana", 0.99m),
            ("Apple", 1.99m),
            ("Apple", 2.99m),
        ]);
    }

    [Fact]
    public void Apply_NoMappedProperty_ThrowsException()
    {
        var model = new ResieveModel {Sorts = "Category"};
        Assert.Throws<ResieveSortingException>(() => _processor.Apply(model, _products));
    }

    [Fact]
    public void Apply_MappedPropertyButNotSortable_ThrowsException()
    {
        var mapper = new ResieveMapper(new List<IResieveMapping>());
        mapper.ForProperty<Product>(x => x.Category).CanFilter();

        var processor = new ResieveSortingProcessor(_provider, mapper);
        var model = new ResieveModel {Sorts = "Category"};
        Assert.Throws<ResieveSortingException>(() => processor.Apply(model, _products));
    }

    [Fact]
    public void Apply_CustomSort_AppliesCustomSortLogic()
    {
        var serviceProvider = new ServiceCollection()
            .AddTransient<IResieveCustomSort<Product>, CustomPriceSort>()
            .BuildServiceProvider();

        var mapper = new ResieveMapper(new List<IResieveMapping>());
        mapper.ForProperty<Product>(x => x.Price).CanSort<CustomPriceSort>();

        var processor = new ResieveSortingProcessor(serviceProvider, mapper);
        var model = new ResieveModel {Sorts = "-Price"};

        // Since CustomPriceSort.Apply returns source as-is, the order should remain unchanged
        var result = processor.Apply(model, _products).ToList();
        result.First().Name.ShouldBe("Eggplant");
    }
    
    [Fact]
    public void Apply_MultipleCustomSort_AppliesCustomSortLogic()
    {
        var serviceProvider = new ServiceCollection()
            .AddTransient<IResieveCustomSort<Product>, CustomPriceSort>()
            .AddTransient<IResieveCustomSort<Product>, CustomIdSort>()
            .BuildServiceProvider();

        var mapper = new ResieveMapper(new List<IResieveMapping>());
        mapper.ForProperty<Product>(x => x.Price).CanSort<CustomPriceSort>();
        mapper.ForKey<Product>("SpecialId").CanSort<CustomIdSort>();

        var processor = new ResieveSortingProcessor(serviceProvider, mapper);
        var model = new ResieveModel {Sorts = "SpecialId,-Price"};

        // Since CustomPriceSort.Apply returns source as-is, the order should remain unchanged
        var result = processor.Apply(model, _products).ToList();
        result.First().Name.ShouldBe("Banana");
        result.First().Id.ShouldBe(7);
    }



    public class CustomPriceSort : IResieveCustomSort<Product>
    {
        public IOrderedQueryable<Product> Apply(IQueryable<Product> source, bool isDescending)
        {
            return isDescending 
                ? source.OrderByDescending(x => x.Name).ThenBy(x => x.Price)
                : source.OrderBy(x => x.Name).ThenBy(x => x.Price);
        }
        public IOrderedQueryable<Product> ApplyThenBy(IOrderedQueryable<Product> source, bool isDescending)
        {
            return isDescending 
                ? source.ThenByDescending(x => x.Name).ThenBy(x => x.Price)
                : source.ThenBy(x => x.Name).ThenBy(x => x.Price);
        }
    }

    public class CustomIdSort : IResieveCustomSort<Product>
    {

        public IOrderedQueryable<Product> Apply(IQueryable<Product> source,  bool isDescending)
        {
            return source.OrderByDescending(x => x.Id);
        }
        
        public IOrderedQueryable<Product> ApplyThenBy(IOrderedQueryable<Product> source, bool isDescending)
        {
            return source.ThenByDescending(x => x.Id);
        }
    }
}