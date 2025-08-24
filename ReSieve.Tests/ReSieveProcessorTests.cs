using ReSieve.Example.Entities;
using ReSieve.Mappings;
using ReSieve.Tests.Builders;
using Shouldly;

namespace ReSieve.Tests;

public class ReSieveProcessorTests
{
    private readonly ReSieveMapper _mapper;
    private readonly ReSieveProcessor _processor;

    private readonly IQueryable<Product> _products = new List<Product>
    {
        A.Product.WithId(1).WithName("Apple").WithPrice(1.99m).Build(),
        A.Product.WithId(2).WithName("Banana").WithPrice(0.99m).Build(),
        A.Product.WithId(3).WithName("Carrot").WithPrice(0.59m).Build(),
        A.Product.WithId(4).WithName("Donut").WithPrice(2.49m).Build(),
        A.Product.WithId(5).WithName("Eggplant").WithPrice(1.29m).Build(),
        // Add more products for robust sorting/pagination tests
        A.Product.WithId(6).WithName("Apple").WithPrice(2.99m).Build(),
        A.Product.WithId(7).WithName("Banana").WithPrice(0.49m).Build(),
        A.Product.WithId(8).WithName("Carrot").WithPrice(1.59m).Build(),
        A.Product.WithId(9).WithName("Donut").WithPrice(1.19m).Build(),
        A.Product.WithId(10).WithName("Eggplant").WithPrice(2.09m).Build()
    }.AsQueryable();

    public ReSieveProcessorTests()
    {
        _mapper = new ReSieveMapper();

        _mapper.Property<Product>(p => p.Id)
            .CanFilter()
            .CanSort();
        
        _mapper.Property<Product>(p => p.Name)
            .CanFilter()
            .CanSort();

        _mapper.Property<Product>(p => p.Price)
            .CanSort();
        
        _processor = new ReSieveProcessor(_mapper);
    }

    [Theory]
    [InlineData(1, 2, new[] {1, 2})] // First page, size 2
    [InlineData(2, 2, new[] {3, 4})] // Second page, size 2
    [InlineData(3, 2, new[] {5, 6})] // Third page, size 2
    [InlineData(4, 2, new[] {7, 8})] // Fourth page, size 2
    [InlineData(5, 2, new[] {9, 10})] // Fifth page, size 2
    [InlineData(6, 2, new int[0])] // Out-of-range page
    [InlineData(1, 5, new[] {1, 2, 3, 4, 5})] // All items in one page
    [InlineData(2, 5, new[] {6, 7, 8, 9, 10})] // All items in one page (second batch)
    [InlineData(3, 5, new int[0])] // Out-of-range page
    [InlineData(0, 2, new[] {1, 2})] // Page 0 treated as 1
    [InlineData(-1, 2, new[] {1, 2})] // Negative page treated as 1
    [InlineData(1, 0, new[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10})] // Page size 0 returns all
    [InlineData(1, -1, new[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10})] // Negative page size returns all
    public void Process_ApplyPagination_ReturnsPaginatedResult(int page, int pageSize, int[] expectedIds)
    {
        var reSieveModel = new ReSieveModel {Page = page, PageSize = pageSize};

        var appliedMapping = _processor.Process(reSieveModel, _products);
        var result = appliedMapping.ToList();
        result.Select(p => p.Id).ShouldBe(expectedIds, true);
    }

    [Theory]
    [InlineData("Name", new[] {1, 6, 2, 7, 3, 8, 4, 9, 5, 10})] // Ascending by Name
    [InlineData("-Name", new[] {10, 5, 9, 4, 8, 3, 7, 2, 6, 1})] // Descending by Name
    [InlineData("Name,-Id", new[] {6, 1, 7, 2, 8, 3, 9, 4, 10, 5})] // Multi-sort: Name asc, Id desc
    public void Process_ApplySorting_ReturnsSortedResult(string sorts, int[] expectedIds)
    {
        var reSieveModel = new ReSieveModel {Sorts = sorts};

        var appliedMapping = _processor.Process(reSieveModel, _products);
        var result = appliedMapping.ToList();
        result.Select(p => p.Id).ShouldBe(expectedIds, true);
    }

    [Theory]
    [InlineData("Name", 1, 3, new[] {"Apple", "Apple", "Banana"})]
    [InlineData("-Name", 2, 4, new[] {"Carrot", "Carrot", "Banana", "Banana"})]
    public void Process_ApplySortingAndPagination_WorksTogether(string sorts, int page, int pageSize, string[] expectedNames)
    {
        var reSieveModel = new ReSieveModel {Sorts = sorts, Page = page, PageSize = pageSize};

        var appliedMapping = _processor.Process(reSieveModel, _products);
        var result = appliedMapping.ToList();
        result.Select(p => p.Name).ShouldBe(expectedNames, true);
    }

    [Fact]
    public void Process_ApplyMultiSortingAndPagination_ValidatesNameAndIdOrder()
    {
        var reSieveModel = new ReSieveModel {Sorts = "Name,-Id", Page = 2, PageSize = 4};

        var appliedMapping = _processor.Process(reSieveModel, _products);
        var result = appliedMapping.ToList();
        // Expecting: Banana, Banana, Carrot, Carrot (with descending Id within each group)
        var expected = new[] {("Carrot", 8), ("Carrot", 3), ("Donut", 9), ("Donut", 4)};
        result.Select(p => (p.Name, p.Id)).ShouldBe(expected, true);
    }
    
    [Fact]
    public void Process_ApplyFilteringSortingAndPagination_CombinedScenario()
    {
        var reSieveModel = new ReSieveModel
        {
            Filters = "Name==Apple|Name==Banana",
            Sorts = "-Price",
            Page = 1,
            PageSize = 3
        };

        var appliedMapping = _processor.Process(reSieveModel, _products);
        var result = appliedMapping.ToList();
        // Expecting: Apple (2.99), Apple (1.99), Banana (0.99)
        var expected = new[] {("Apple", 2.99m), ("Apple", 1.99m), ("Banana", 0.99m)};
        result.Select(p => (p.Name, p.Price)).ShouldBe(expected, true);
    }
    
    [Fact]
    public void Process_ApplyFilteringSortingAndPagination_NoResults()
    {
        var reSieveModel = new ReSieveModel
        {
            Filters = "Name==Zucchini", // No such product
            Sorts = "Price",
            Page = 1,
            PageSize = 5
        };

        var appliedMapping = _processor.Process(reSieveModel, _products);
        var result = appliedMapping.ToList();
        result.ShouldBeEmpty();
    }
    
    [Fact]
    public void Process_ApplyFiltering_ReturnsFilteredResults()
    {
        var reSieveModel = new ReSieveModel
        {
            Filters = "Name==Apple|Name==Banana"
        };

        var appliedMapping = _processor.Process(reSieveModel, _products);
        var result = appliedMapping.ToList();
        var expected = new[] {("Apple", 1.99m), ("Apple", 2.99m), ("Banana", 0.99m), ("Banana", 0.49m)};
        result.Select(p => (p.Name, p.Price)).ShouldBe(expected, true);
    }
}