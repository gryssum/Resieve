using Microsoft.EntityFrameworkCore;
using Resieve.Example.Data;
using Resieve.Example.Entities;

namespace Resieve.Example.Repository;

public class ProductRepository(AppDbContext context, IResieveProcessor processor)
{
    public async Task<PaginatedResponse<IEnumerable<Product>>> GetFilteredProductsAsync(ResieveModel model)
    {
        var source = context
            .Products
            .Include(p => p.Tags)
            .AsNoTracking();

        return await source.ApplyAllAsync(
            model,
            processor,
            q => q.ToListAsync(),
            q => q.CountAsync()
        );
    }
}

public class ProductAdvancedRepository(AppDbContext context, IResieveProcessor processor)
{
    public async Task<PaginatedResponse<IEnumerable<Product>>> GetFilteredProductsAsync(ResieveModel model)
    {
        var source = context
            .Products
            .Include(p => p.Tags)
            .AsNoTracking();

        // Step 1: Apply filtering and sorting to get the total count (without pagination)
        var filteredAndSortedQuery = source
            .FilterBy(model, processor)
            .SortBy(model, processor);

        var totalCount = await filteredAndSortedQuery.CountAsync();

        // Step 2: Apply pagination only (filtering and sorting are skipped)
        var result = await filteredAndSortedQuery
            .PaginateBy(model, processor)
            .ToListAsync();

        // Step 3: Convert IQueryable to Paginated Response
        return result.ToPaginatedResponse(model, processor, totalCount);
    }
}