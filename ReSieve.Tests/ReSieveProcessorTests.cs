using ReSieve.Example.Entities;
using ReSieve.Models;
using ReSieve.Tests.Builders;
using Xunit;
using Shouldly;

namespace ReSieve.Tests;

public class ReSieveProcessorTests
{
    private readonly ReSieveProcessor _processor;
    private readonly ReSieveMapper _mapper;

    private readonly IQueryable<Product> _products = new List<Product>
    {
        A.Product.WithId(1).WithName("Apple").WithPrice(1.99m).Build(),
        A.Product.WithId(2).WithName("Banana").WithPrice(0.99m).Build(),
        A.Product.WithId(3).WithName("Carrot").WithPrice(0.59m).Build(),
        A.Product.WithId(4).WithName("Donut").WithPrice(2.49m).Build(),
        A.Product.WithId(5).WithName("Eggplant").WithPrice(1.29m).Build()
    }.AsQueryable();

    public ReSieveProcessorTests()
    {
        _mapper = new ReSieveMapper();

        _mapper.Property<Product>(p => p.Name)
            .CanFilter()
            .CanSort();

        _processor = new ReSieveProcessor(_mapper);
    }

    [Theory]
    [InlineData(1, 2, new[] {1, 2})] // First page, size 2
    [InlineData(2, 2, new[] {3, 4})] // Second page, size 2
    [InlineData(3, 2, new[] {5})]    // Third page, size 2 (last page, single item)
    [InlineData(1, 5, new[] {1, 2, 3, 4, 5})] // All items in one page
    [InlineData(2, 5, new int[0])]   // Out-of-range page
    [InlineData(0, 2, new[] {1, 2})] // Page 0 treated as 1
    [InlineData(-1, 2, new[] {1, 2})] // Negative page treated as 1
    [InlineData(1, 0, new[] {1, 2, 3, 4, 5})] // Page size 0 returns all
    [InlineData(1, -1, new[] {1, 2, 3, 4, 5})] // Negative page size returns all
    public void Process_ApplyPagination_ReturnsPaginatedResult(int page, int pageSize, int[] expectedIds)
    {
        var reSieveModel = new ReSieveModel
        {
            Page = page,
            PageSize = pageSize
        };

        var appliedMapping = _processor.Process(reSieveModel, _products);
        var result = appliedMapping.ToList();
        result.Select(p => p.Id).ShouldBe(expectedIds, ignoreOrder: true);
    }
}