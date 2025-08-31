using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;
using Resieve.Example.Entities;
using Resieve.Filtering;
using Resieve.Mappings;
using Resieve.Mappings.Interfaces;
using Resieve.Tests.Builders;
using NSubstitute;
using Resieve.Exceptions;
using Resieve.Filtering.ExpressionTrees;

namespace Resieve.Tests.Filtering;

public class ResieveFilterProcessorTests
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

    private IExpressionTreeBuilder CreateExpressionTreeBuilder()
    {
        var serviceProvider = Substitute.For<IServiceProvider>();
        return new ExpressionTreeBuilder(serviceProvider);
    }

    [Fact]
    public void Apply_AppleEqualFilter_ReturnsApple()
    {
        var processor = new ResieveFilterProcessor(GetProductMapper(), CreateExpressionTreeBuilder());
        var model = new ResieveModel {Filters = "Name==Apple"};

        var data = GetProductData();
        var result = processor.Apply(model, data).ToList();
        Assert.Single(result);
    }

    [Fact]
    public void Apply_AppleNotEqualFilter_ReturnsApple()
    {
        var processor = new ResieveFilterProcessor(GetProductMapper(), CreateExpressionTreeBuilder());
        var model = new ResieveModel {Filters = "Name!=Apple"};

        var data = GetProductData();
        var result = processor.Apply(model, data).ToList();
        Assert.Equal(7, result.Count);
    }

    [Fact]
    public void Apply_AppleEqualIgnoreCaseFilter_ReturnsApple()
    {
        var processor = new ResieveFilterProcessor(GetProductMapper(), CreateExpressionTreeBuilder());
        var model = new ResieveModel {Filters = "Name==*apple"};

        var data = GetProductData();
        var result = processor.Apply(model, data).ToList();
        Assert.Single(result);
    }

    [Fact]
    public void Apply_AppleContainsFilter_ReturnsApple()
    {
        var processor = new ResieveFilterProcessor(GetProductMapper(), CreateExpressionTreeBuilder());
        var model = new ResieveModel {Filters = "Name@=ple"};

        var data = GetProductData();
        var result = processor.Apply(model, data).ToList();
        Assert.Single(result);
    }

    [Fact]
    public void Apply_PriceGreaterThenFilter_ReturnsProducts()
    {
        var processor = new ResieveFilterProcessor(GetProductMapper(), CreateExpressionTreeBuilder());
        var model = new ResieveModel {Filters = "Price>=20"};

        var data = GetProductData();
        var result = processor.Apply(model, data).ToList();
        Assert.Equal(4, result.Count);
    }

    [Fact]
    public void Apply_NameAndPrice_ReturnsTwoItems()
    {
        var processor = new ResieveFilterProcessor(GetProductMapper(), CreateExpressionTreeBuilder());
        var model = new ResieveModel {Filters = "Name==Apple,Price>=1"};

        var data = GetProductData();
        var result = processor.Apply(model, data).ToList();
        Assert.Single(result);
        Assert.Contains(result, p => p.Name == "Apple");
    }

    [Fact]
    public void Apply_CategoryFoodAndPrice_ReturnsTwoItems()
    {
        var processor = new ResieveFilterProcessor(GetProductMapper(), CreateExpressionTreeBuilder());
        var model = new ResieveModel {Filters = "Category==Food,Price>=1"};

        var data = GetProductData();
        var result = processor.Apply(model, data).ToList();
        Assert.Equal(2, result.Count);
        Assert.Contains(result, p => p.Name == "Apple");
        Assert.Contains(result, p => p.Name == "Orange");
    }

    [Fact]
    public void Apply_NameOrPrice_ReturnsTwoItems()
    {
        var processor = new ResieveFilterProcessor(GetProductMapper(), CreateExpressionTreeBuilder());
        var model = new ResieveModel {Filters = "Name==Apple|Price>=1"};

        var data = GetProductData();
        var result = processor.Apply(model, data).ToList();
        Assert.Equal(7, result.Count);
        Assert.Contains(result, p => p.Name == "Apple");
        Assert.Contains(result, p => p.Name == "Orange");
    }

    [Fact]
    public void Apply_Parenthesis_ReturnsTwoItems()
    {
        var processor = new ResieveFilterProcessor(GetProductMapper(), CreateExpressionTreeBuilder());
        var model = new ResieveModel {Filters = "Price>=1,(Category==Food|Category==Clothing)"};

        var data = GetProductData();
        var result = processor.Apply(model, data).ToList();
        Assert.Equal(4, result.Count);
        Assert.Contains(result, p => p.Name == "Apple");
        Assert.Contains(result, p => p.Name == "Orange");
        Assert.Contains(result, p => p.Name == "T-Shirt");
        Assert.Contains(result, p => p.Name == "Jeans");
    }

    [Fact]
    public void Apply_ExclusiveParenthesesExample_ReturnsExpectedItems()
    {
        var processor = new ResieveFilterProcessor(GetProductMapper(), CreateExpressionTreeBuilder());
        var model = new ResieveModel {Filters = "Name==Apple|(Category==Clothing,Price>100)"};

        var data = GetProductData();
        var result = processor.Apply(model, data).ToList();

        Assert.Contains(result, p => p.Name == "Apple");
        Assert.DoesNotContain(result, p => p.Name == "T-Shirt"); // Assuming T-Shirt is Clothing and >100
        Assert.DoesNotContain(result, p => p.Name == "Jeans"); // Assuming Jeans is Clothing and >100
        Assert.DoesNotContain(result, p => p.Name == "Orange");
        Assert.DoesNotContain(result, p => p.Name == "Hat"); // Assuming Hat is Clothing and <=100
    }

    [Fact]
    public void Apply_NoMappedProperty_ThrowsException()
    {
        var processor = new ResieveFilterProcessor(GetProductMapper(), CreateExpressionTreeBuilder());
        var model = new ResieveModel {Filters = "IsAvailable==true"};
        var data = GetProductData();
        Assert.Throws<ResieveFilterException>(() => processor.Apply(model, data));
    }

    [Fact]
    public void Apply_EmptyFilters_ReturnsAll()
    {
        var processor = new ResieveFilterProcessor(GetProductMapper(), CreateExpressionTreeBuilder());
        var model = new ResieveModel {Filters = ""};
        var data = GetProductData();
        var result = processor.Apply(model, data).ToList();
        Assert.Equal(data.Count(), result.Count);
    }

    [Fact]
    public void Apply_MappedPropertyThatCannotFilter_ThrowsException()
    {
        var mapper = new ResieveMapper();
        mapper.ForProperty<Product>(x => x.Name).CanSort();
        mapper.ForProperty<Product>(x => x.Category).CanSort();

        var processor = new ResieveFilterProcessor(mapper, CreateExpressionTreeBuilder());
        var model = new ResieveModel {Filters = "Category==Food,Name==Apple"};
        var data = GetProductData();
        Assert.Throws<ResieveFilterException>(() => processor.Apply(model, data));
    }

    [Fact]
    public void Apply_NotFoodAsCustomFilter_ReturnsTwoItems()
    {
        var serviceProvider = new ServiceCollection()
            .AddTransient<IResieveCustomFilter<Product>, FoodCategoryCustomFilter>()
            .BuildServiceProvider();
        
        var mapper = new ResieveMapper();
        mapper
            .ForKey<Product>("NotFood")
            .CanFilter<FoodCategoryCustomFilter>();

        var processor = new ResieveFilterProcessor(mapper, new ExpressionTreeBuilder(serviceProvider));
        var model = new ResieveModel {Filters = "NotFood>=30"};

        var data = GetProductData();
        var result = processor.Apply(model, data).ToList();
        Assert.Equal(4, result.Count);
        Assert.Contains(result, p => p.Name == "Jeans");
        Assert.Contains(result, p => p.Name == "Laptop");
    }

    [Fact]
    public void Apply_NotFoodCustomFilterOrName_ReturnsExpectedItems()
    {
        var serviceProvider = new ServiceCollection()
            .AddTransient<IResieveCustomFilter<Product>, FoodCategoryCustomFilter>()
            .BuildServiceProvider();
        
        var mapper = new ResieveMapper();
        mapper
            .ForKey<Product>("NotFood")
            .CanFilter<FoodCategoryCustomFilter>();
        mapper
            .ForProperty<Product>(p => p.Name)
            .CanFilter();

        var processor = new ResieveFilterProcessor(mapper, new ExpressionTreeBuilder(serviceProvider));
        // Combine custom filter with OR clause for Name
        var model = new ResieveModel {Filters = "NotFood>=2|Name==Banana"};

        var data = GetProductData();
        var result = processor.Apply(model, data).ToList();
        
        Assert.Equal(6, result.Count);
        Assert.Contains(result, p => p.Name == "Banana");
    }

    private class FoodCategoryCustomFilter : IResieveCustomFilter<Product>
    {
        public Expression<Func<Product, bool>> GetWhereExpression(string @operator, string value)
        {
             if(decimal.TryParse(value, out var decimalValue))
                return x => (x.Name == "Apple" || x.Price >= decimalValue) && x.Category != ProductCategory.Food;

             return _ => true;
        }
    }
}