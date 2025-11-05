using Microsoft.Extensions.Options;
using NSubstitute;
using Resieve.Tests.Mocks;
using Resieve.Filtering;
using Resieve.Pagination;
using Resieve.Sorting;
using Resieve.Tests.Builders;

namespace Resieve.Tests;

public class ResieveProcessorTests
{
    private readonly IResieveFilterProcessor _filterProcessor;
    private readonly IResieveSortingProcessor _sortingProcessor;
    private readonly IResievePaginationProcessor _paginationProcessor;
    private readonly ResieveProcessor _processor;
    private readonly IQueryable<Product> _products;

    public ResieveProcessorTests()
    {
        _filterProcessor = Substitute.For<IResieveFilterProcessor>();
        _sortingProcessor = Substitute.For<IResieveSortingProcessor>();
        _paginationProcessor = Substitute.For<IResievePaginationProcessor>();
        _processor = new ResieveProcessor(_sortingProcessor, _filterProcessor, _paginationProcessor, Options.Create(new ResieveOptions()));
        _products = new List<Product> { A.Product.WithId(1).WithName("Apple").WithPrice(1.99m).Build() }.AsQueryable();
    }

    [Fact]
    public void Filter_CallsApplyFiltering_AndReturnsResult()
    {
        var model = new ResieveModel { Filters = "Name==Apple" };
        _filterProcessor.Apply(model, Arg.Any<IQueryable<Product>>()).Returns(_products);
        var result = _processor.Filter(model, _products);
        _filterProcessor.Received(1).Apply(model, Arg.Any<IQueryable<Product>>());
        Assert.Equal(_products, result);
    }

    [Fact]
    public void Sort_CallsApplySorting_AndReturnsResult()
    {
        var model = new ResieveModel { Sorts = "Name" };
        _sortingProcessor.Apply(model, Arg.Any<IQueryable<Product>>()).Returns(_products);
        var result = _processor.Sort(model, _products);
        _sortingProcessor.Received(1).Apply(model, Arg.Any<IQueryable<Product>>());
        Assert.Equal(_products, result);
    }

    [Fact]
    public void Paginate_CallsApplyPagination_AndReturnsResult()
    {
        var model = new ResieveModel { Page = 1, PageSize = 10 };
        _paginationProcessor.Apply(model, Arg.Any<IQueryable<Product>>()).Returns(_products);
        var result = _processor.Paginate(model, _products);
        _paginationProcessor.Received(1).Apply(model, Arg.Any<IQueryable<Product>>());
        Assert.Equal(_products, result);
    }

    [Fact]
    public void ToPaginatedResponse_ReturnsCorrectPaginatedResponse()
    {
        var model = new ResieveModel { Page = 2, PageSize = 5 };
        var totalCount = 100;
        var response = _processor.ToPaginatedResponse(model, _products, totalCount);
        Assert.Equal(_products, response.Items);
        Assert.Equal(2, response.PageNumber);
        Assert.Equal(5, response.PageSize);
        Assert.Equal(100, response.TotalCount);
    }
}