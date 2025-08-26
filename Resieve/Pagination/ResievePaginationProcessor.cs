using System.Linq;

namespace Resieve.Pagination
{
    public interface IResievePaginationProcessor
    {
        IQueryable<TEntity> Apply<TEntity>(ResieveModel reSieveModel, IQueryable<TEntity> source);
    }

    public class ResievePaginationProcessor : IResievePaginationProcessor
    {
        public IQueryable<TEntity> Apply<TEntity>(ResieveModel reSieveModel, IQueryable<TEntity> source)
        {
            var page = reSieveModel.Page;
            var pageSize = reSieveModel.PageSize;

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
            return source.Skip(skip).Take(pageSize);
        }
    }
}