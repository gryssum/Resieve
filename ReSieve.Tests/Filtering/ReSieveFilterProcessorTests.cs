using ReSieve.Example.Entities;
using ReSieve.Filtering;
using ReSieve.Mappings;
using ReSieve.Tests.Builders;

namespace ReSieve.Tests.Filtering;

public class ReSieveFilterProcessorTests
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
    
    [Fact]
    public void Apply_AppleEqualFilter_ReturnsApple()
    {
        var processor = new ReSieveFilterProcessor(GetProductMapper());
        var model = new ReSieveModel {Filters = "Name==Apple"};
        
        var data = GetProductData();
        var result = processor.Apply(model, data).ToList();
        Assert.Single(result);
    }
    
    [Fact]
    public void Apply_AppleNotEqualFilter_ReturnsApple()
    {
        var processor = new ReSieveFilterProcessor(GetProductMapper());
        var model = new ReSieveModel {Filters = "Name!=Apple"};
        
        var data = GetProductData();
        var result = processor.Apply(model, data).ToList();
        Assert.Equal(7, result.Count);
    }
    
    [Fact]
    public void Apply_AppleEqualIgnoreCaseFilter_ReturnsApple()
    {
        var processor = new ReSieveFilterProcessor(GetProductMapper());
        var model = new ReSieveModel {Filters = "Name==*apple"};
        
        var data = GetProductData();
        var result = processor.Apply(model, data).ToList();
        Assert.Equal(1, result.Count);
    }
    
    [Fact]
    public void Apply_AppleContainsFilter_ReturnsApple()
    {
        var processor = new ReSieveFilterProcessor(GetProductMapper());
        var model = new ReSieveModel {Filters = "Name@=ple"};
        
        var data = GetProductData();
        var result = processor.Apply(model, data).ToList();
        Assert.Equal(1, result.Count);
    }
    
    [Fact]
    public void Apply_PriceGreaterThenFilter_ReturnsProducts()
    {
        var processor = new ReSieveFilterProcessor(GetProductMapper());
        var model = new ReSieveModel {Filters = "Price>=20"};
        
        var data = GetProductData();
        var result = processor.Apply(model, data).ToList();
        Assert.Equal(4, result.Count);
    }

}