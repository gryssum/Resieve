using System;
using System.Linq;
using Microsoft.Extensions.Options;

namespace Resieve.Pagination
{
    /// <summary>
    /// Defines a contract for applying pagination to an <see cref="IQueryable{TEntity}"/> source.
    /// </summary>
    public interface IResievePaginationProcessor
    {
        /// <summary>
        /// Applies pagination to the specified <paramref name="source"/> based on the provided <paramref name="reSieveModel"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of the elements in the source query.</typeparam>
        /// <param name="reSieveModel">The model containing pagination parameters.</param>
        /// <param name="source">The queryable source to paginate.</param>
        /// <returns>A paginated <see cref="IQueryable{TEntity}"/>.</returns>
        IQueryable<TEntity> Apply<TEntity>(ResieveModel reSieveModel, IQueryable<TEntity> source);
    }

    /// <summary>
    /// Provides functionality to apply pagination to an <see cref="IQueryable{TEntity}"/> source using <see cref="ResieveModel"/> and <see cref="ResieveOptions"/>.
    /// </summary>
    public class ResievePaginationProcessor(IOptions<ResieveOptions>? options) : IResievePaginationProcessor
    {
        private readonly ResieveOptions _options = options?.Value ?? new ResieveOptions();
        
        /// <summary>
        /// Applies pagination to the specified <paramref name="source"/> based on the provided <paramref name="reSieveModel"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of the elements in the source query.</typeparam>
        /// <param name="reSieveModel">The model containing pagination parameters.</param>
        /// <param name="source">The queryable source to paginate.</param>
        /// <returns>A paginated <see cref="IQueryable{TEntity}"/>.</returns>
        public IQueryable<TEntity> Apply<TEntity>(ResieveModel reSieveModel, IQueryable<TEntity> source)
        {
            var page = reSieveModel.Page;
            var pageSize = reSieveModel.PageSize ?? _options.DefaultPageSize;
            var maxPageSize = _options.MaxPageSize > 0 ? _options.MaxPageSize : pageSize;

            // Treat page <= 0 as 1
            if (page <= 0)
            {
                page = 1;
            }

            // Treat pageSize <= 0 as 'all'
            if (pageSize <= 0)
            {
                return source;
            }

            var skip = (page - 1) * pageSize;
            return source.Skip(skip).Take(Math.Min(pageSize, maxPageSize));
        }
    }
}