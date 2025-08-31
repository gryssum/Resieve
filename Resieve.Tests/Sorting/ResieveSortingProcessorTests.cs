using Resieve.Example.Entities;
using Resieve.Mappings;
using Resieve.Sorting;
using Resieve.Tests.Builders;
using Shouldly;

namespace Resieve.Tests.Sorting;

public class ResieveSortingProcessorTests
{
    private readonly ResieveSortingProcessor _processor = new(GetProductMapper());

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
        var mapper = new ResieveMapper();
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
        var model = new ResieveModel {Sorts = "Name,-Price"};
        var result = _processor.Apply(model, _products).ToList();
        // Should be sorted by Name ascending, then by Price descending for same Name
        result.Select(p => (p.Name, p.Price)).ShouldBe([
            ("Apple", 2.99m),
            ("Apple", 1.99m),
            ("Banana", 0.99m),
            ("Banana", 0.49m),
            ("Carrot", 0.59m),
            ("Donut", 2.49m),
            ("Eggplant", 1.29m)
        ]);
    }
    
    [Fact]
    public void Apply_NoMappedProperty_ThrowsException()
    {
        var model = new ResieveModel {Sorts = "Category"};
        Assert.Throws<ArgumentException>(() => _processor.Apply(model, _products));
    }

    
    [Fact]
    public void Apply_MappedPropertyButNotSortable_ThrowsException()
    {
        var mapper = new ResieveMapper();
        mapper.ForProperty<Product>(x => x.Category).CanFilter();
        
        var processor = new ResieveSortingProcessor(mapper);
        var model = new ResieveModel {Sorts = "Category"};
        Assert.Throws<ArgumentException>(() => processor.Apply(model, _products));
    }
}