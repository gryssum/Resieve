using System;
using System.Linq;
using Microsoft.Extensions.Options;

namespace Resieve.Pagination
{
    public interface IResievePaginationProcessor
    {
        IQueryable<TEntity> Apply<TEntity>(ResieveModel reSieveModel, IQueryable<TEntity> source);
    }

    public class ResievePaginationProcessor(IOptions<ResieveOptions>? options) : IResievePaginationProcessor
    {
        private readonly ResieveOptions _options = options?.Value ?? new ResieveOptions();
        
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