using NSubstitute;
using Resieve.Example.Entities;
using Resieve.Filtering;
using Resieve.Filtering.ExpressionTrees;
using Resieve.Mappings;
using Resieve.Tests.Builders;

namespace Resieve.Tests.Filtering;

public class GeneratedResieveFilterProcessorTests
{
    private static IQueryable<Product> GetProductData()
    {
        return new List<Product>
        {
            // Food
            A.Product.WithId(1).WithName("Apple").WithPrice(1.99m).WithCategory(ProductCategory.Food).WithIsAvailable(true)
                .WithTags([ new Tag(1, "Fruit", "Fresh fruit") ]).Build(),
            A.Product.WithId(4).WithName("Banana").WithPrice(0.99m).WithCategory(ProductCategory.Food).WithIsAvailable(true)
                .WithTags([ new Tag(1, "Fruit", "Fresh fruit") ]).Build(),
            A.Product.WithId(5).WithName("Orange").WithPrice(2.49m).WithCategory(ProductCategory.Food).WithIsAvailable(true)
                .WithTags([ new Tag(1, "Fruit", "Fresh fruit") ]).Build(),
            // Electronics
            A.Product.WithId(2).WithName("Laptop").WithPrice(999.99m).WithCategory(ProductCategory.Electronics).WithIsAvailable(true)
                .WithTags([ new Tag(2, "Tech", "Electronics tag") ]).Build(),
            A.Product.WithId(6).WithName("Headphones").WithPrice(49.99m).WithCategory(ProductCategory.Electronics).WithIsAvailable(true)
                .WithTags([ new Tag(2, "Tech", "Electronics tag") ]).Build(),
            // Clothing
            A.Product.WithId(3).WithName("T-Shirt").WithPrice(19.99m).WithCategory(ProductCategory.Clothing).WithIsAvailable(false)
                .WithTags([ new Tag(3, "Clothes", "Clothing tag") ]).Build(),
            A.Product.WithId(7).WithName("Jeans").WithPrice(39.99m).WithCategory(ProductCategory.Clothing).WithIsAvailable(true)
                .WithTags([ new Tag(3, "Clothes", "Clothing tag") ]).Build(),
            // Furniture
            A.Product.WithId(8).WithName("Desk").WithPrice(120.00m).WithCategory(ProductCategory.Furniture).WithIsAvailable(true)
                .WithTags([ new Tag(4, "Office", "Office furniture") ]).Build()
        }.AsQueryable();
    }

    private static ResieveMapper GetProductMapper()
    {
        var mapper = new ResieveMapper();
        mapper.ForProperty<Product>(x => x.Name).CanFilter();
        mapper.ForProperty<Product>(x => x.Price).CanFilter();
        mapper.ForProperty<Product>(x => x.Category).CanFilter();
        return mapper;
    }

    private static IExpressionTreeBuilder CreateExpressionTreeBuilder()
    {
        var serviceProvider = Substitute.For<IServiceProvider>();
        return new ExpressionTreeBuilder(serviceProvider);
    }

    private static ResieveFilterProcessor CreateProcessor()
    {
        return new ResieveFilterProcessor(GetProductMapper(), CreateExpressionTreeBuilder());
    }

    [Theory]
    [InlineData("Name==Apple", 1)]
    [InlineData("Price>=20", 4)]
    [InlineData("Category==Food", 3)]
    [InlineData("Name==Apple,Price>=20", 0)]
    public void Apply_SimpleAndOrFilters_ReturnsExpectedCount(string filter, int expectedCount)
    {
        var processor = CreateProcessor();
        var model = new ResieveModel {Filters = filter};
        var data = GetProductData();
        var result = processor.Apply(model, data).ToList();
        Assert.Equal(expectedCount, result.Count);
    }

    [Fact]
    public void Apply_CrossProductOr_ReturnsExpected()
    {
        var processor = CreateProcessor();
        var model = new ResieveModel {Filters = "Name==Desk|Category==Food"};
        var data = GetProductData();
        var result = processor.Apply(model, data).ToList();
        Assert.Equal(4, result.Count);
        Assert.Contains(result, x => x.Name == "Apple");
        Assert.Contains(result, x => x.Name == "Banana");
        Assert.Contains(result, x => x.Name == "Orange");
        Assert.Contains(result, x => x.Name == "Desk");
    }

    [Fact]
    public void Apply_AppleEqualFilter_ReturnsApple()
    {
        var processor = CreateProcessor();
        var model = new ResieveModel {Filters = "Name==Apple"};

        var data = GetProductData();
        var result = processor.Apply(model, data).ToList();
        Assert.Single(result);
    }

    [Fact]
    public void Apply_AppleNotEqualFilter_ReturnsApple()
    {
        var processor = CreateProcessor();
        var model = new ResieveModel {Filters = "Name!=Apple"};

        var data = GetProductData();
        var result = processor.Apply(model, data).ToList();
        Assert.Equal(7, result.Count);
    }
}