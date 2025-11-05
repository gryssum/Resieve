using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
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

public class ProductAdvancedRepository(AppDbContext context, IResieveProcessor processor, IOptions<ResieveOptions> options)
{
    private readonly ResieveOptions _options = options?.Value ?? new ResieveOptions();
    
    public PaginatedResponse<IEnumerable<Product>> GetFilteredProducts(ResieveModel model)
    {
        var source = context
            .Products
            .Include(p => p.Tags)
            .AsNoTracking();

        var count = source.Count();
        var result = processor.Process(model, context.Products);

        return new PaginatedResponse<IEnumerable<Product>>(result, model.Page, model.PageSize ?? _options.DefaultPageSize, count);
    }
}

public record PaginatedResponse<T>(T Items, int PageNumber, int PageSize, int TotalCount);