using System;
using System.Collections.Generic;
using System.Linq;

namespace Resieve
{
    public static class ResieveQueryableExtensions
    {
        /// <summary>
        /// Applies filtering to the source using the provided ResieveModel and processor.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="source">The source queryable.</param>
        /// <param name="model">The model containing filter parameters.</param>
        /// <param name="processor">The Resieve processor to use.</param>
        /// <returns>A queryable with filtering applied.</returns>
        /// <exception cref="ArgumentNullException">Thrown if model, source, or processor is null.</exception>
        public static IQueryable<T> FilterBy<T>(this IQueryable<T> source, ResieveModel model, IResieveProcessor processor)
            => processor.Filter(model, source);

        /// <summary>
        /// Applies sorting to the source using the provided ResieveModel and processor.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="source">The source queryable.</param>
        /// <param name="model">The model containing sort parameters.</param>
        /// <param name="processor">The Resieve processor to use.</param>
        /// <returns>A queryable with sorting applied.</returns>
        /// <exception cref="ArgumentNullException">Thrown if model, source, or processor is null.</exception>
        public static IQueryable<T> SortBy<T>(this IQueryable<T> source, ResieveModel model, IResieveProcessor processor)
            => processor.Sort(model, source);

        /// <summary>
        /// Applies pagination to the source using the provided ResieveModel and processor.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="source">The source queryable.</param>
        /// <param name="model">The model containing pagination parameters.</param>
        /// <param name="processor">The Resieve processor to use.</param>
        /// <returns>A queryable with pagination applied.</returns>
        /// <exception cref="ArgumentNullException">Thrown if model, source, or processor is null.</exception>
        public static IQueryable<T> PaginateBy<T>(this IQueryable<T> source, ResieveModel model, IResieveProcessor processor)
            => processor.Paginate(model, source);

        /// <summary>
        /// Wraps the (already filtered, sorted, and paginated) source in a PaginatedResponse.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="source">The already paginated and filtered source.</param>
        /// <param name="model">The model containing pagination parameters.</param>
        /// <param name="processor">The Resieve processor to use.</param>
        /// <param name="totalCount">The total count before pagination.</param>
        /// <returns>A PaginatedResponse containing the items and pagination info.</returns>
        /// <exception cref="ArgumentNullException">Thrown if model, source, or processor is null.</exception>
        public static PaginatedResponse<IEnumerable<T>> ToPaginatedResponse<T>(
            this IEnumerable<T> source,
            ResieveModel model,
            IResieveProcessor processor,
            int totalCount) => processor.ToPaginatedResponse(model, source, totalCount);
    }
}
