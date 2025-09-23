namespace Resieve.Tests.Mocks;

public enum ProductCategory
{
    Food,
    Electronics,
    Clothing,
    Furniture
}

public record Product(
    int Id,
    string Name,
    decimal Price,
    double Weight,
    float Rating,
    DateTime CreatedAt,
    Guid ProductGuid,
    ProductCategory Category,
    bool IsAvailable,
    List<Tag> Tags // Now a list of Tag objects with more details
);

public record Tag(
    int Id,
    string Name,
    string Description
);