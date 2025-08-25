using ReSieve.Example.Entities;

namespace ReSieve.Example.Repository;

public static class ProductDataSource
{
    public static List<Product> GetProducts()
    {
        var tags = new List<Tag>
        {
            new Tag(1, "Fresh", "Fresh produce"),
            new Tag(2, "Tech", "Electronics item"),
            new Tag(3, "Fashion", "Clothing item"),
            new Tag(4, "Home", "Home and furniture"),
            new Tag(5, "Popular", "Popular item"),
            new Tag(6, "Eco", "Environmentally friendly"),
            new Tag(7, "Luxury", "Premium quality"),
            new Tag(8, "Sale", "Discounted item"),
            new Tag(9, "New", "Recently added"),
            new Tag(10, "Limited", "Limited edition")
        };

        var random = new Random(42); // Using a seed for reproducible results
        var products = new List<Product>();
        
        // Food products
        var foodProducts = new[]
        {
            "Organic Apples", "Premium Bananas", "Fresh Strawberries", "Avocados", 
            "Whole Grain Bread", "Greek Yogurt", "Belgian Chocolate", "Italian Pasta",
            "Extra Virgin Olive Oil", "Local Honey", "Artisan Cheese", "Smoked Salmon"
        };
        
        // Electronics products
        var electronicsProducts = new[]
        {
            "Ultra HD Smart TV", "Wireless Headphones", "Gaming Laptop", "Smartphone", 
            "Digital Camera", "Bluetooth Speaker", "Tablet", "Smartwatch",
            "Wireless Earbuds", "Smart Home Hub", "Gaming Console", "Drone"
        };
        
        // Clothing products
        var clothingProducts = new[]
        {
            "Cotton T-Shirt", "Slim Fit Jeans", "Wool Sweater", "Leather Jacket", 
            "Summer Dress", "Running Shoes", "Casual Shorts", "Winter Coat",
            "Designer Sunglasses", "Silk Scarf", "Formal Suit", "Yoga Pants"
        };
        
        // Furniture products
        var furnitureProducts = new[]
        {
            "Ergonomic Office Chair", "King Size Bed", "Sectional Sofa", "Dining Table", 
            "Bookshelf", "Coffee Table", "Standing Desk", "Nightstand",
            "Kitchen Cabinet", "Lounge Chair", "TV Stand", "Bar Stool"
        };
        
        int id = 1;
        
        // Add food products
        foreach (var name in foodProducts)
        {
            var tagList = tags.Where(t => new[] { 1, 5, 6, 9 }.Contains(t.Id))
                              .OrderBy(_ => random.Next())
                              .Take(random.Next(1, 3))
                              .ToList();
                              
            products.Add(new Product(
                id++,
                name,
                Math.Round((decimal)(random.NextDouble() * 20 + 1), 2), // Lower price range for food
                Math.Round(random.NextDouble() * 2, 2), // Lighter weight
                (float)Math.Round(3.5 + random.NextDouble() * 1.5, 1), // Higher ratings
                DateTime.UtcNow.AddDays(-random.Next(1, 30)), // More recent
                Guid.NewGuid(),
                ProductCategory.Food,
                random.Next(5) < 4, // 80% available
                tagList
            ));
        }
        
        // Add electronics products
        foreach (var name in electronicsProducts)
        {
            var tagList = tags.Where(t => new[] { 2, 5, 7, 9, 10 }.Contains(t.Id))
                              .OrderBy(_ => random.Next())
                              .Take(random.Next(1, 4))
                              .ToList();
                              
            products.Add(new Product(
                id++,
                name,
                Math.Round((decimal)(random.NextDouble() * 900 + 100), 2), // Higher price range
                Math.Round(random.NextDouble() * 5 + 0.5, 2), // Medium weight
                (float)Math.Round(3.0 + random.NextDouble() * 2.0, 1),
                DateTime.UtcNow.AddDays(-random.Next(10, 180)), // Older items
                Guid.NewGuid(),
                ProductCategory.Electronics,
                random.Next(10) < 7, // 70% available
                tagList
            ));
        }
        
        // Add clothing products
        foreach (var name in clothingProducts)
        {
            var tagList = tags.Where(t => new[] { 3, 5, 6, 7, 8 }.Contains(t.Id))
                              .OrderBy(_ => random.Next())
                              .Take(random.Next(1, 3))
                              .ToList();
                              
            products.Add(new Product(
                id++,
                name,
                Math.Round((decimal)(random.NextDouble() * 150 + 15), 2), // Medium price range
                Math.Round(random.NextDouble() * 1, 2), // Very light weight
                (float)Math.Round(3.0 + random.NextDouble() * 2.0, 1),
                DateTime.UtcNow.AddDays(-random.Next(5, 90)), // Mix of new and older
                Guid.NewGuid(),
                ProductCategory.Clothing,
                random.Next(10) < 8, // 80% available
                tagList
            ));
        }
        
        // Add furniture products
        foreach (var name in furnitureProducts)
        {
            var tagList = tags.Where(t => new[] { 4, 6, 7, 8 }.Contains(t.Id))
                              .OrderBy(_ => random.Next())
                              .Take(random.Next(1, 3))
                              .ToList();
                              
            products.Add(new Product(
                id++,
                name,
                Math.Round((decimal)(random.NextDouble() * 800 + 50), 2), // Higher price range
                Math.Round(random.NextDouble() * 50 + 5, 2), // Heavy weight
                (float)Math.Round(3.0 + random.NextDouble() * 2.0, 1),
                DateTime.UtcNow.AddDays(-random.Next(20, 365)), // Older items
                Guid.NewGuid(),
                ProductCategory.Furniture,
                random.Next(10) < 6, // 60% available (less stock)
                tagList
            ));
        }
        
        // Add a few random products if we haven't reached 50
        while (products.Count < 50)
        {
            var category = (ProductCategory)random.Next(4);
            string name;
            decimal priceRange;
            double weightRange;
            
            switch (category)
            {
                case ProductCategory.Food:
                    name = $"Special {foodProducts[random.Next(foodProducts.Length)]}";
                    priceRange = (decimal)(random.NextDouble() * 20 + 1);
                    weightRange = random.NextDouble() * 2;
                    break;
                case ProductCategory.Electronics:
                    name = $"Premium {electronicsProducts[random.Next(electronicsProducts.Length)]}";
                    priceRange = (decimal)(random.NextDouble() * 900 + 100);
                    weightRange = random.NextDouble() * 5 + 0.5;
                    break;
                case ProductCategory.Clothing:
                    name = $"Designer {clothingProducts[random.Next(clothingProducts.Length)]}";
                    priceRange = (decimal)(random.NextDouble() * 150 + 15);
                    weightRange = random.NextDouble() * 1;
                    break;
                default: // Furniture
                    name = $"Custom {furnitureProducts[random.Next(furnitureProducts.Length)]}";
                    priceRange = (decimal)(random.NextDouble() * 800 + 50);
                    weightRange = random.NextDouble() * 50 + 5;
                    break;
            }
            
            var tagList = tags.OrderBy(_ => random.Next())
                              .Take(random.Next(1, 4))
                              .ToList();
                              
            products.Add(new Product(
                id++,
                name,
                Math.Round(priceRange, 2),
                Math.Round(weightRange, 2),
                (float)Math.Round(3.0 + random.NextDouble() * 2.0, 1),
                DateTime.UtcNow.AddDays(-random.Next(1, 365)),
                Guid.NewGuid(),
                category,
                random.Next(2) == 0,
                tagList
            ));
        }
        
        return products;
    }
}
