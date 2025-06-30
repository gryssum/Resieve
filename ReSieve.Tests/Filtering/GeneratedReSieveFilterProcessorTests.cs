using ReSieve.Example.Entities;
using ReSieve.Filtering;
using ReSieve.Mappings;
using ReSieve.Tests.Builders;

namespace ReSieve.Tests.Filtering;

public class GeneratedReSieveFilterProcessorTests
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

    private static ReSieveMapper GetProductMapper()
    {
        var mapper = new ReSieveMapper();
        mapper.Property<Product>(x => x.Name).CanFilter();
        mapper.Property<Product>(x => x.Price).CanFilter();
        mapper.Property<Product>(x => x.Category).CanFilter();
        return mapper;
    }

    [Theory]
    [InlineData("Name==Apple", 1)]
    [InlineData("Price>=20", 4)]
    [InlineData("Category==Food", 3)]
    [InlineData("Name==(Apple|Banana)", 2)]
    [InlineData("Name==Apple,Price>=20", 0)]
    [InlineData("(Name|Category)==Food", 3)]
    public void Apply_SimpleAndOrFilters_ReturnsExpectedCount(string filter, int expectedCount)
    {
        var processor = new ReSieveFilterProcessor(GetProductMapper());
        var model = new ReSieveModel {Filters = filter};
        var data = GetProductData();
        var result = processor.Apply(model, data).ToList();
        Assert.Equal(expectedCount, result.Count);
    }

    [Fact]
    public void Apply_OrGroupWithAnd_ReturnsExpected()
    {
        var processor = new ReSieveFilterProcessor(GetProductMapper());
        var model = new ReSieveModel {Filters = "Category==(Food|Electronics),Price>=2.00"};
        var data = GetProductData();
        var result = processor.Apply(model, data).ToList();

        Assert.Equal(3, result.Count);
        Assert.Contains(result, x => x.Name == "Orange");
        Assert.Contains(result, x => x.Name == "Headphones");
        Assert.Contains(result, x => x.Name == "Laptop");
    }

    [Fact]
    public void Apply_CrossProductOr_ReturnsExpected()
    {
        var processor = new ReSieveFilterProcessor(GetProductMapper());
        var model = new ReSieveModel {Filters = "Name==Desk|Category==Food"};
        var data = GetProductData();
        var result = processor.Apply(model, data).ToList();
        Assert.Equal(4, result.Count);
        Assert.Contains(result, x => x.Name == "Apple");
        Assert.Contains(result, x => x.Name == "Banana");
        Assert.Contains(result, x => x.Name == "Orange");
        Assert.Contains(result, x => x.Name == "Desk");
    }

    [Theory]
    [InlineData("(Category==(Food|Electronics),Price>=2.00)|(Name==(Apple|Banana),Price<2.00)", 4)]
    [InlineData("(Name|Category)==(Desk|Food),Price<100|Price>=100", 4)]
    [InlineData("((Category==(Food|Electronics),Price>=2.00)|(Name==Apple,Price<2.00)),(Category==Clothing|Name==Desk)", 1)]
    [InlineData("(Category==Food|Category==Electronics),Price>=2.00,Name!=(T-Shirt|Jeans)", 2)]
    [InlineData("(Name|Category|Price)==(Apple|Food|999.99)", 2)]
    [InlineData("((Name|Category)==(Apple|Food),Price>=10)|(Category==Electronics,Price<100|Name==Desk)", 3)]
    public void Apply_ComplexQueries_ReturnsExpectedCount(string filter, int expectedCount)
    {
        var processor = new ReSieveFilterProcessor(GetProductMapper());
        var model = new ReSieveModel {Filters = filter};
        var data = GetProductData();
        var result = processor.Apply(model, data).ToList();
        Assert.Equal(expectedCount, result.Count);
    }

    [Theory]
    [InlineData("(Category!=(Food|Electronics),Price<2.00)|(Name!=(Apple|Banana),Price>2.00)", 4)]
    [InlineData("(Name|Category)!=Desk,Price<=100|Price>100", 8)]
    [InlineData("((Category!=(Food|Electronics),Price<2.00)|(Name!=Apple,Price>2.00)),(Category!=Clothing|Name!=Desk)", 6)]
    [InlineData("(Category!=Food|Category!=Electronics),Price<=2.00,Name!=(T-Shirt|Jeans)", 3)]
    [InlineData("(Name|Category|Price)!=(Apple|Food|999.99)", 5)]
    [InlineData("((Name|Category)!=(Apple|Food),Price<=10)|(Category!=Electronics,Price>100|Name!=Desk)", 6)]
    public void Apply_ComplexQueries_WithDifferentOperators_ReturnsExpectedCount(string filter, int expectedCount)
    {
        var processor = new ReSieveFilterProcessor(GetProductMapper());
        var model = new ReSieveModel {Filters = filter};
        var data = GetProductData();
        var result = processor.Apply(model, data).ToList();
        Assert.Equal(expectedCount, result.Count);
    }
}