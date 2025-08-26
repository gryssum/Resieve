using NSubstitute;
using Resieve.Example.Entities;
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
        _processor = new ResieveProcessor(_sortingProcessor, _filterProcessor, _paginationProcessor);
        _products = new List<Product> { A.Product.WithId(1).WithName("Apple").WithPrice(1.99m).Build() }.AsQueryable();
    }

    [Fact]
    public void Process_CallsApplyPagination_WhenPaginationIsSet()
    {
        var model = new ResieveModel { Page = 2, PageSize = 5 };
        _paginationProcessor.Apply(model, Arg.Any<IQueryable<Product>>()).Returns(_products);
        _filterProcessor.Apply(model, Arg.Any<IQueryable<Product>>()).Returns(_products);
        _sortingProcessor.Apply(model, Arg.Any<IQueryable<Product>>()).Returns(_products);

        _processor.Process(model, _products);

        _paginationProcessor.Received(1).Apply(model, Arg.Any<IQueryable<Product>>());
    }

    [Fact]
    public void Process_CallsApplySorting_WhenSortingIsSet()
    {
        var model = new ResieveModel { Sorts = "Name" };
        _paginationProcessor.Apply(model, Arg.Any<IQueryable<Product>>()).Returns(_products);
        _filterProcessor.Apply(model, Arg.Any<IQueryable<Product>>()).Returns(_products);
        _sortingProcessor.Apply(model, Arg.Any<IQueryable<Product>>()).Returns(_products);

        _processor.Process(model, _products);

        _sortingProcessor.Received(1).Apply(model, Arg.Any<IQueryable<Product>>());
    }

    [Fact]
    public void Process_CallsApplyFiltering_WhenFilteringIsSet()
    {
        var model = new ResieveModel { Filters = "Name==Apple" };
        _paginationProcessor.Apply(model, Arg.Any<IQueryable<Product>>()).Returns(_products);
        _filterProcessor.Apply(model, Arg.Any<IQueryable<Product>>()).Returns(_products);
        _sortingProcessor.Apply(model, Arg.Any<IQueryable<Product>>()).Returns(_products);

        _processor.Process(model, _products);

        _filterProcessor.Received(1).Apply(model, Arg.Any<IQueryable<Product>>());
    }
}