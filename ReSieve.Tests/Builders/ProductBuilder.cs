using ReSieve.Example.Entities;

namespace ReSieve.Tests.Builders;

public class ProductBuilder
{
    private ProductCategory _category = ProductCategory.Food;
    private DateTime _createdAt = new(2024, 1, 1);
    private int _id = 1;
    private bool _isAvailable = true;
    private string _name = "Test Product";
    private decimal _price = 9.99m;
    private Guid _productGuid = Guid.NewGuid();
    private float _rating = 4.0f;

    private List<Tag> _tags = [ new TagBuilder().Build() ];
    private double _weight = 0.5d;

    public ProductBuilder WithId(int id)
    {
        _id = id;
        return this;
    }

    public ProductBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public ProductBuilder WithPrice(decimal price)
    {
        _price = price;
        return this;
    }

    public ProductBuilder WithWeight(double weight)
    {
        _weight = weight;
        return this;
    }

    public ProductBuilder WithRating(float rating)
    {
        _rating = rating;
        return this;
    }

    public ProductBuilder WithCreatedAt(DateTime createdAt)
    {
        _createdAt = createdAt;
        return this;
    }

    public ProductBuilder WithProductGuid(Guid guid)
    {
        _productGuid = guid;
        return this;
    }

    public ProductBuilder WithCategory(ProductCategory category)
    {
        _category = category;
        return this;
    }

    public ProductBuilder WithIsAvailable(bool isAvailable)
    {
        _isAvailable = isAvailable;
        return this;
    }

    public ProductBuilder WithTags(List<Tag> tags)
    {
        _tags = tags;
        return this;
    }

    public Product Build()
    {
        return new Product(_id, _name, _price, _weight, _rating, _createdAt, _productGuid, _category, _isAvailable, _tags);
    }
}

public static class A
{
    public static ProductBuilder Product => new();
}