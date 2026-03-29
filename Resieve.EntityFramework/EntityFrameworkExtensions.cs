using Microsoft.EntityFrameworkCore;

namespace Resieve.EntityFramework;

public static class EntityFrameworkExtensions
{
    /// <summary>
    /// Asynchronously applies filtering, sorting, and pagination, then materializes the result and total count using the provided async materializer functions.
    /// Intended for use with EF Core or any async-capable ORM.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="source">The source queryable.</param>
    /// <param name="model">The model containing filter, sort, and pagination parameters.</param>
    /// <param name="processor">The Resieve processor to use.</param>
    /// <returns>A task that resolves to a PaginatedResponse containing the items and pagination info.</returns>
    /// <exception cref="ArgumentNullException">Thrown if model, source, processor, or materializer functions are null.</exception>
    public static PaginatedResponse<IEnumerable<T>> ToResieveResultAsync<T>(
        this IQueryable<T> source,
        ResieveModel model,
        IResieveProcessor processor)
    {
        var filteredAndSortedQuery = source
            .FilterBy(model, processor)
            .SortBy(model, processor);
        
        var totalCount = filteredAndSortedQuery.Count();
        var paginatedResult = filteredAndSortedQuery.PaginateBy(model, processor).ToList();

        return paginatedResult.ToPaginatedResponse(model, processor, totalCount);
    }
    
    /// <summary>
    /// Asynchronously applies filtering, sorting, and pagination, then materializes the result and total count using the provided async materializer functions.
    /// Intended for use with EF Core or any async-capable ORM.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="source">The source queryable.</param>
    /// <param name="model">The model containing filter, sort, and pagination parameters.</param>
    /// <param name="processor">The Resieve processor to use.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns>A task that resolves to a PaginatedResponse containing the items and pagination info.</returns>
    /// <exception cref="ArgumentNullException">Thrown if model, source, processor, or materializer functions are null.</exception>
    public static async Task<PaginatedResponse<IEnumerable<T>>> ToResieveResultAsync<T>(
        this IQueryable<T> source,
        ResieveModel model,
        IResieveProcessor processor,
        CancellationToken cancellationToken)
    {
        var filteredAndSortedQuery = source
            .FilterBy(model, processor)
            .SortBy(model, processor);
        
        var totalCount = await filteredAndSortedQuery.CountAsync(cancellationToken: cancellationToken);
        var paginatedResult = await filteredAndSortedQuery.PaginateBy(model, processor).ToListAsync(cancellationToken: cancellationToken);

        return paginatedResult.ToPaginatedResponse(model, processor, totalCount);
    }
}