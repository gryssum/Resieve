using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using Resieve.Filtering;
using Resieve.Pagination;
using Resieve.Sorting;

namespace Resieve
{
    /// <summary>
    /// Provides filtering, sorting, and pagination operations for queryable data sources.
    /// </summary>
    public class ResieveProcessor(
        IResieveSortingProcessor resieveSortingProcessor,
        IResieveFilterProcessor resieveFilterProcessor,
        IResievePaginationProcessor resievePaginationProcessor,
        IOptions<ResieveOptions>? options
    ) : IResieveProcessor
    {
        private readonly ResieveOptions _options = options?.Value ?? new ResieveOptions();

        /// <summary>
        /// Applies filtering to the source using the provided ResieveModel.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="resieveModel">The model containing filter parameters.</param>
        /// <param name="source">The source queryable.</param>
        /// <returns>A queryable with filtering applied.</returns>
        /// <exception cref="ArgumentNullException">Thrown if resieveModel or source is null.</exception>
        public IQueryable<TEntity> Filter<TEntity>(ResieveModel resieveModel, IQueryable<TEntity> source)
        {
            if (resieveModel == null) throw new ArgumentNullException(nameof(resieveModel));
            if (source == null) throw new ArgumentNullException(nameof(source));
            return resieveFilterProcessor.Apply(resieveModel, source);
        }

        /// <summary>
        /// Applies sorting to the source using the provided ResieveModel.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="resieveModel">The model containing sort parameters.</param>
        /// <param name="source">The source queryable.</param>
        /// <returns>A queryable with sorting applied.</returns>
        /// <exception cref="ArgumentNullException">Thrown if resieveModel or source is null.</exception>
        public IQueryable<TEntity> Sort<TEntity>(ResieveModel resieveModel, IQueryable<TEntity> source)
        {
            if (resieveModel == null) throw new ArgumentNullException(nameof(resieveModel));
            if (source == null) throw new ArgumentNullException(nameof(source));
            return resieveSortingProcessor.Apply(resieveModel, source);
        }

        /// <summary>
        /// Applies pagination to the source using the provided ResieveModel.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="resieveModel">The model containing pagination parameters.</param>
        /// <param name="source">The source queryable.</param>
        /// <returns>A queryable with pagination applied.</returns>
        /// <exception cref="ArgumentNullException">Thrown if resieveModel or source is null.</exception>
        public IQueryable<TEntity> Paginate<TEntity>(ResieveModel resieveModel, IQueryable<TEntity> source)
        {
            if (resieveModel == null) throw new ArgumentNullException(nameof(resieveModel));
            if (source == null) throw new ArgumentNullException(nameof(source));
            return resievePaginationProcessor.Apply(resieveModel, source);
        }
        
        /// <summary>
        /// Wraps the (already filtered, sorted, and paginated) source in a PaginatedResponse.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="resieveModel">The model containing pagination parameters.</param>
        /// <param name="source">The already paginated and filtered source.</param>
        /// <param name="totalCount">The total count before pagination.</param>
        /// <returns>A PaginatedResponse containing the items and pagination info.</returns>
        /// <exception cref="ArgumentNullException">Thrown if resieveModel or source is null.</exception>
        public PaginatedResponse<IEnumerable<TEntity>> ToPaginatedResponse<TEntity>(ResieveModel resieveModel, IEnumerable<TEntity> source, int totalCount)
        {
            if (resieveModel == null) throw new ArgumentNullException(nameof(resieveModel));
            if (source == null) throw new ArgumentNullException(nameof(source));
            return new PaginatedResponse<IEnumerable<TEntity>>(source, resieveModel.Page, resieveModel.PageSize ?? _options.DefaultPageSize, totalCount);
        }
    }

    /// <summary>
    /// Interface for filtering, sorting, and paginating queryable data sources.
    /// </summary>
    public interface IResieveProcessor
    {
        /// <summary>
        /// Applies filtering to the source using the provided ResieveModel.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="resieveModel">The model containing filter parameters.</param>
        /// <param name="source">The source queryable.</param>
        /// <returns>A queryable with filtering applied.</returns>
        IQueryable<TEntity> Filter<TEntity>(ResieveModel resieveModel, IQueryable<TEntity> source);

        /// <summary>
        /// Applies sorting to the source using the provided ResieveModel.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="resieveModel">The model containing sort parameters.</param>
        /// <param name="source">The source queryable.</param>
        /// <returns>A queryable with sorting applied.</returns>
        IQueryable<TEntity> Sort<TEntity>(ResieveModel resieveModel, IQueryable<TEntity> source);

        /// <summary>
        /// Applies pagination to the source using the provided ResieveModel.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="resieveModel">The model containing pagination parameters.</param>
        /// <param name="source">The source queryable.</param>
        /// <returns>A queryable with pagination applied.</returns>
        IQueryable<TEntity> Paginate<TEntity>(ResieveModel resieveModel, IQueryable<TEntity> source);

        /// <summary>
        /// Wraps the (already filtered, sorted, and paginated) source in a PaginatedResponse.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="resieveModel">The model containing pagination parameters.</param>
        /// <param name="source">The already paginated and filtered source.</param>
        /// <param name="totalCount">The total count before pagination.</param>
        /// <returns>A PaginatedResponse containing the items and pagination info.</returns>
        PaginatedResponse<IEnumerable<TEntity>> ToPaginatedResponse<TEntity>(ResieveModel resieveModel, IEnumerable<TEntity> source, int totalCount);
    }
}