using ReSieve.Example.Entities;

namespace ReSieve.Tests.Builders;

public class ProductBuilder
{
    private int _id = 1;
    private string _name = "Test Product";
    private decimal _price = 9.99m;

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

    public Product Build()
    {
        return new Product(_id, _name, _price);
    }
}

public static class A
{
    public static ProductBuilder Product => new();
}