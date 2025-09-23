namespace Resieve.Example.Entities;

public enum ProductCategory
{
    Food,
    Electronics,
    Clothing,
    Furniture
}

public class Product
{
    private ICollection<Tag> _tags = new List<Tag>();

    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public double Weight { get; set; }
    public float Rating { get; set; }
    public DateTime CreatedAt { get; set; }
    public ProductCategory Category { get; set; }
    public bool IsAvailable { get; set; }
    public virtual ICollection<Tag> Tags 
    { 
        get => _tags; 
        set => _tags = value; 
    }

    // Constructor for EF
    public Product() { }

    // Constructor to maintain compatibility with existing code
    public Product(Guid id, string name, decimal price, double weight, float rating, 
                   DateTime createdAt, ProductCategory category, 
                   bool isAvailable, List<Tag> tags)
    {
        Id = id;
        Name = name;
        Price = price;
        Weight = weight;
        Rating = rating;
        CreatedAt = createdAt;
        Category = category;
        IsAvailable = isAvailable;
        _tags = tags;
    }
}