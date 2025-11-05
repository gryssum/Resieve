using NSubstitute;
using Resieve.Tests.Builders;
using Resieve.Tests.Mocks;

namespace Resieve.Tests;

public class ResieveQueryableExtensionsTests
{
    private readonly IResieveProcessor _processor = Substitute.For<IResieveProcessor>();
    private readonly IQueryable<Product> _products = new List<Product> {A.Product.WithId(1).WithName("Apple").WithPrice(1.99m).Build()}.AsQueryable();
    private readonly ResieveModel _model = new() {Page = 1, PageSize = 10, Filters = "Name==Apple", Sorts = "Name"};

    [Fact]
    public void FilterBy_CallsProcessorFilter_AndReturnsResult()
    {
        _processor.Filter(_model, _products).Returns(_products);
        var result = _products.FilterBy(_model, _processor);
        _processor.Received(1).Filter(_model, _products);
        Assert.Equal(_products, result);
    }

    [Fact]
    public void SortBy_CallsProcessorSort_AndReturnsResult()
    {
        _processor.Sort(_model, _products).Returns(_products);
        var result = _products.SortBy(_model, _processor);
        _processor.Received(1).Sort(_model, _products);
        Assert.Equal(_products, result);
    }

    [Fact]
    public void PaginateBy_CallsProcessorPaginate_AndReturnsResult()
    {
        _processor.Paginate(_model, _products).Returns(_products);
        var result = _products.PaginateBy(_model, _processor);
        _processor.Received(1).Paginate(_model, _products);
        Assert.Equal(_products, result);
    }

    [Fact]
    public async Task Resieve_CallsAllProcessorsInOrder_AndReturnsResult()
    {
        var filtered = new List<Product> {A.Product.WithId(2).WithName("Banana").WithPrice(2.99m).Build()}.AsQueryable();
        var sorted = new List<Product> {A.Product.WithId(3).WithName("Carrot").WithPrice(0.99m).Build()}.AsQueryable();
        var paginated = new List<Product> {A.Product.WithId(4).WithName("Date").WithPrice(3.99m).Build()}.AsQueryable();

        _processor.Filter(_model, _products).Returns(filtered);
        _processor.Sort(_model, filtered).Returns(sorted);
        _processor.Paginate(_model, sorted).Returns(paginated);
        _processor.ToPaginatedResponse(_model, Arg.Any<List<Product>>(), Arg.Any<int>()).Returns(new PaginatedResponse<IEnumerable<Product>>(paginated.ToList(), 1, 1, 1));
        var result = await _products.ApplyAllAsync(
            _model,
            _processor, 
            products => Task.FromResult(products.ToList()), 
            products => Task.FromResult(products.Count()));

        _processor.Received(1).Filter(_model, _products);
        _processor.Received(1).Sort(_model, filtered);
        _processor.Received(1).Paginate(_model, sorted);
        _processor.Received(1).ToPaginatedResponse(_model, Arg.Any<IEnumerable<Product>>(), Arg.Any<int>());
        Assert.Equal(paginated, result.Items);
    }

    [Fact]
    public async Task ApplyAllAsync_WithMaterializerFuncs_ReturnsCorrectPaginatedResponse()
    {
        var items = _products.ToList();
        var totalCount = 42;
        var model = new ResieveModel {Page = 2, PageSize = 5};
        var processor = Substitute.For<IResieveProcessor>();
        processor.Filter(model, _products).Returns(_products);
        processor.Sort(model, _products).Returns(_products);
        processor.Paginate(model, _products).Returns(_products);
        processor.ToPaginatedResponse(model, Arg.Any<List<Product>>(), Arg.Any<int>()).Returns(new PaginatedResponse<IEnumerable<Product>>(items, 2, 5, 42));
        var response = await _products.ApplyAllAsync(
            model,
            processor,
            _ => Task.FromResult(items),
            _ => Task.FromResult(totalCount)
        );
        
        Assert.Equal(items, response.Items);
        Assert.Equal(model.Page, response.PageNumber);
        Assert.Equal(model.PageSize, response.PageSize);
        Assert.Equal(totalCount, response.TotalCount);
    }
}