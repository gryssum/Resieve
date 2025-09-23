using Microsoft.EntityFrameworkCore;
using Resieve.Example.Entities;

namespace Resieve.Example.Data;

public static class DbSeeder
{
    // Method for seeding data during application startup
    public static async Task SeedDatabaseAsync(AppDbContext context)
    {
        // Only seed if the database is empty
        if (await context.Products.AnyAsync() || await context.Tags.AnyAsync())
        {
            return; // Database already has data
        }

        // Seed Tags
        var tags = new[]
        {
            new Tag(Guid.NewGuid(), "Fresh", "Fresh produce"),
            new Tag(Guid.NewGuid(), "Tech", "Electronics item"),
            new Tag(Guid.NewGuid(), "Fashion", "Clothing item"),
            new Tag(Guid.NewGuid(), "Home", "Home and furniture"),
            new Tag(Guid.NewGuid(), "Popular", "Popular item"),
            new Tag(Guid.NewGuid(), "Eco", "Environmentally friendly"),
            new Tag(Guid.NewGuid(), "Luxury", "Premium quality"),
            new Tag(Guid.NewGuid(), "Sale", "Discounted item"),
            new Tag(Guid.NewGuid(), "New", "Recently added"),
            new Tag(Guid.NewGuid(), "Limited", "Limited edition")
        };

        await context.Tags.AddRangeAsync(tags);
        await context.SaveChangesAsync();

        // Seed Products
        var random = new Random(42);
        var products = new List<Product>();
        
        var foodProducts = new[]
        {
            "Organic Apples", "Premium Bananas", "Fresh Strawberries", "Avocados", 
            "Whole Grain Bread", "Greek Yogurt", "Belgian Chocolate", "Italian Pasta",
            "Extra Virgin Olive Oil", "Local Honey", "Artisan Cheese", "Smoked Salmon"
        };
        
        var electronicsProducts = new[]
        {
            "Ultra HD Smart TV", "Wireless Headphones", "Gaming Laptop", "Smartphone", 
            "Digital Camera", "Bluetooth Speaker", "Tablet", "Smartwatch",
            "Wireless Earbuds", "Smart Home Hub", "Gaming Console", "Drone"
        };
        
        var clothingProducts = new[]
        {
            "Cotton T-Shirt", "Slim Fit Jeans", "Wool Sweater", "Leather Jacket", 
            "Summer Dress", "Running Shoes", "Casual Shorts", "Winter Coat",
            "Designer Sunglasses", "Silk Scarf", "Formal Suit", "Yoga Pants"
        };
        
        var furnitureProducts = new[]
        {
            "Ergonomic Office Chair", "King Size Bed", "Sectional Sofa", "Dining Table", 
            "Bookshelf", "Coffee Table", "Standing Desk", "Nightstand",
            "Kitchen Cabinet", "Lounge Chair", "TV Stand", "Bar Stool"
        };
        
        // Get fresh references to tags from the database
        var freshTag = await context.Tags.FirstAsync(t => t.Name == "Fresh");
        var techTag = await context.Tags.FirstAsync(t => t.Name == "Tech");
        var fashionTag = await context.Tags.FirstAsync(t => t.Name == "Fashion");
        var homeTag = await context.Tags.FirstAsync(t => t.Name == "Home");
        var popularTag = await context.Tags.FirstAsync(t => t.Name == "Popular");
        var ecoTag = await context.Tags.FirstAsync(t => t.Name == "Eco");
        var luxuryTag = await context.Tags.FirstAsync(t => t.Name == "Luxury");
        var saleTag = await context.Tags.FirstAsync(t => t.Name == "Sale");
        var newTag = await context.Tags.FirstAsync(t => t.Name == "New");
        var limitedTag = await context.Tags.FirstAsync(t => t.Name == "Limited");
        
        // Add food products
        foreach (var name in foodProducts)
        {
            // Random tags for food products
            var foodTags = new List<Tag>();
            if (random.Next(2) == 0) foodTags.Add(freshTag);
            if (random.Next(2) == 0) foodTags.Add(popularTag);
            if (random.Next(2) == 0) foodTags.Add(ecoTag);
            if (random.Next(2) == 0) foodTags.Add(newTag);
            
            // Make sure at least one tag is added
            if (foodTags.Count == 0)
                foodTags.Add(freshTag);
                
            products.Add(new Product(
                Guid.NewGuid(),
                name,
                Math.Round((decimal)(random.NextDouble() * 20 + 1), 2),
                Math.Round(random.NextDouble() * 2, 2),
                (float)Math.Round(3.5 + random.NextDouble() * 1.5, 1),
                DateTime.UtcNow.AddDays(-random.Next(1, 30)),
                ProductCategory.Food,
                random.Next(5) < 4,
                foodTags
            ));
        }
        
        // Add electronics products
        foreach (var name in electronicsProducts)
        {
            // Random tags for electronics products
            var electronicsTags = new List<Tag>();
            if (random.Next(2) == 0) electronicsTags.Add(techTag);
            if (random.Next(2) == 0) electronicsTags.Add(popularTag);
            if (random.Next(2) == 0) electronicsTags.Add(luxuryTag);
            if (random.Next(2) == 0) electronicsTags.Add(newTag);
            if (random.Next(2) == 0) electronicsTags.Add(limitedTag);
            
            // Make sure at least one tag is added
            if (electronicsTags.Count == 0)
                electronicsTags.Add(techTag);
                
            products.Add(new Product(
                Guid.NewGuid(),
                name,
                Math.Round((decimal)(random.NextDouble() * 900 + 100), 2),
                Math.Round(random.NextDouble() * 5 + 0.5, 2),
                (float)Math.Round(3.0 + random.NextDouble() * 2.0, 1),
                DateTime.UtcNow.AddDays(-random.Next(10, 180)),
                ProductCategory.Electronics,
                random.Next(10) < 7,
                electronicsTags
            ));
        }
        
        // Add clothing products
        foreach (var name in clothingProducts)
        {
            // Random tags for clothing products
            var clothingTags = new List<Tag>();
            if (random.Next(2) == 0) clothingTags.Add(fashionTag);
            if (random.Next(2) == 0) clothingTags.Add(popularTag);
            if (random.Next(2) == 0) clothingTags.Add(luxuryTag);
            if (random.Next(2) == 0) clothingTags.Add(saleTag);
            
            // Make sure at least one tag is added
            if (clothingTags.Count == 0)
                clothingTags.Add(fashionTag);
                
            products.Add(new Product(
                Guid.NewGuid(),
                name,
                Math.Round((decimal)(random.NextDouble() * 150 + 15), 2),
                Math.Round(random.NextDouble() * 1.5 + 0.1, 2),
                (float)Math.Round(3.2 + random.NextDouble() * 1.8, 1),
                DateTime.UtcNow.AddDays(-random.Next(5, 90)),
                ProductCategory.Clothing,
                random.Next(5) < 4,
                clothingTags
            ));
        }
        
        // Add furniture products
        foreach (var name in furnitureProducts)
        {
            // Random tags for furniture products
            var furnitureTags = new List<Tag>();
            if (random.Next(2) == 0) furnitureTags.Add(homeTag);
            if (random.Next(2) == 0) furnitureTags.Add(popularTag);
            if (random.Next(2) == 0) furnitureTags.Add(luxuryTag);
            if (random.Next(2) == 0) furnitureTags.Add(limitedTag);
            
            // Make sure at least one tag is added
            if (furnitureTags.Count == 0)
                furnitureTags.Add(homeTag);
                
            products.Add(new Product(
                Guid.NewGuid(),
                name,
                Math.Round((decimal)(random.NextDouble() * 800 + 50), 2),
                Math.Round(random.NextDouble() * 25 + 5, 2),
                (float)Math.Round(3.0 + random.NextDouble() * 2.0, 1),
                DateTime.UtcNow.AddDays(-random.Next(15, 365)),
                ProductCategory.Furniture,
                random.Next(4) < 3,
                furnitureTags
            ));
        }

        await context.Products.AddRangeAsync(products);
        await context.SaveChangesAsync();
    }
}
