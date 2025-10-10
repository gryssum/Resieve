using Microsoft.EntityFrameworkCore;
using Resieve.Example.Data;
using Resieve.Example.Entities;

namespace Resieve.Example.Repository;

public class ProductRepository(AppDbContext context, IResieveProcessor processor)
{
    public IEnumerable<Product> GetFilteredProducts(ResieveModel model)
    {
        var source = context
            .Products
            .Include(p => p.Tags)
            .AsNoTracking();
        
        return processor.Process(model, source);
    }
}

public class ProductAdvancedRepository(AppDbContext context, IResieveProcessor processor)
{
    public PaginatedResponse<IEnumerable<Product>> GetFilteredProducts(ResieveModel model)
    {
        var source = context
            .Products
            .Include(p => p.Tags)
            .AsNoTracking();

        var count = source.Count();
        var result = processor.Process(model, context.Products);

        return new PaginatedResponse<IEnumerable<Product>>(result, model.Page, model.PageSize, count);
    }
}

public record PaginatedResponse<T>(T Items, int PageNumber, int PageSize, int TotalCount);